// Walk a .NET managed PE file, find every .resources stream (magic 0xBEEFCACE),
// parse its name table (System.Resources.ResourceReader v2 format), and dump
// each named entry whose body contains a PNG/JPEG/GIF/BMP payload as a file
// named after the logical resource name.
//
//   node extract_resources_named.js <input.exe|.dll> <output-dir>
//
// Non-destructive: reads only.

const fs = require('fs');
const path = require('path');

const [inputPath, outDir] = process.argv.slice(2);
if (!inputPath || !outDir) {
  console.error('usage: node extract_resources_named.js <input> <output-dir>');
  process.exit(2);
}

fs.mkdirSync(outDir, { recursive: true });
const data = fs.readFileSync(inputPath);
const base = path.basename(inputPath);

// A single file reader with a position cursor (one shared state — scripts are serial)
class Reader {
  constructor(buf, pos) { this.buf = buf; this.pos = pos; }
  readUInt32LE() { const v = this.buf.readUInt32LE(this.pos); this.pos += 4; return v; }
  readInt32LE() { const v = this.buf.readInt32LE(this.pos); this.pos += 4; return v; }
  read7BitEncodedInt() {
    let result = 0, shift = 0;
    while (true) {
      if (shift === 35) throw new Error('bad 7bit-encoded int');
      const b = this.buf.readUInt8(this.pos++);
      result |= (b & 0x7F) << shift;
      if ((b & 0x80) === 0) break;
      shift += 7;
    }
    return result >>> 0;
  }
  readPrefixedBytes() { const len = this.read7BitEncodedInt(); const out = this.buf.subarray(this.pos, this.pos + len); this.pos += len; return out; }
  readUtf8String() { return this.readPrefixedBytes().toString('utf8'); }
  readUtf16LEString() { return this.readPrefixedBytes().toString('utf16le'); }
  skipTo8ByteAlignedFromAnchor(anchor) {
    // The pad byte count is `(8 - ((this.pos - anchor) & 7)) & 7`
    const pad = (8 - ((this.pos - anchor) & 7)) & 7;
    this.pos += pad;
  }
}

function detectImageExt(buf) {
  if (buf.length < 4) return null;
  if (buf[0] === 0x89 && buf[1] === 0x50 && buf[2] === 0x4E && buf[3] === 0x47) return 'png';
  if (buf[0] === 0xFF && buf[1] === 0xD8 && buf[2] === 0xFF) return 'jpg';
  if (buf[0] === 0x47 && buf[1] === 0x49 && buf[2] === 0x46 && buf[3] === 0x38) return 'gif';
  if (buf[0] === 0x42 && buf[1] === 0x4D) return 'bmp';
  return null;
}

// PNG/JPEG/GIF inside a resource entry may be wrapped by a ResourceTypeCode
// byte + length prefix (for ByteArray/Stream types, type code 0x20 / 0x21)
// or wrapped in a BinaryFormatter envelope (for System.Drawing.Bitmap/Icon).
// We just scan forward from the entry start for a known image magic.
function findImageInEntry(buf, entryStart, maxScan = 4096) {
  const end = Math.min(buf.length, entryStart + maxScan);
  for (let i = entryStart; i < end; i++) {
    const ext = detectImageExt(buf.subarray(i, i + 8));
    if (ext) return { offset: i, ext };
  }
  return null;
}

const MAGIC = Buffer.from([0xCE, 0xCA, 0xEF, 0xBE]); // 0xBEEFCACE LE
const streams = [];
let scanFrom = 0;
while (true) {
  const idx = data.indexOf(MAGIC, scanFrom);
  if (idx < 0) break;
  streams.push(idx);
  scanFrom = idx + 4;
}

const perStream = [];
for (const streamStart of streams) {
  try {
    const r = new Reader(data, streamStart);
    const magic = r.readUInt32LE();
    if (magic !== 0xBEEFCACE) continue;
    const headerVersion = r.readInt32LE();
    const numBytesToSkip = r.readInt32LE();
    const skipStart = r.pos;
    const readerType = r.readUtf8String();
    const resourceSetType = r.readUtf8String();
    r.pos = skipStart + numBytesToSkip;
    const dataSectionVersion = r.readInt32LE();
    if (dataSectionVersion !== 2 && dataSectionVersion !== 1) {
      perStream.push({ streamStart, error: `unsupported data-section version ${dataSectionVersion}` });
      continue;
    }
    const numResources = r.readInt32LE();
    const numResourceTypes = r.readInt32LE();
    const typeNames = [];
    for (let i = 0; i < numResourceTypes; i++) typeNames.push(r.readUtf8String());
    // Alignment: pad to 8-byte alignment relative to stream start
    r.skipTo8ByteAlignedFromAnchor(streamStart);
    const nameHashes = new Array(numResources);
    for (let i = 0; i < numResources; i++) nameHashes[i] = r.readInt32LE();
    const namePositions = new Array(numResources);
    for (let i = 0; i < numResources; i++) namePositions[i] = r.readInt32LE();
    const dataSectionStart = r.readInt32LE();
    const nameSectionStart = r.pos;

    const resources = [];
    for (let i = 0; i < numResources; i++) {
      r.pos = nameSectionStart + namePositions[i];
      const name = r.readUtf16LEString();
      const dataPos = r.readInt32LE();
      const entryStart = streamStart + dataSectionStart + dataPos;
      resources.push({ name, entryStart });
    }

    perStream.push({
      streamStart,
      headerVersion,
      dataSectionVersion,
      numResources,
      numResourceTypes,
      typeNames,
      dataSectionStart,
      resources,
    });
  } catch (e) {
    perStream.push({ streamStart, error: String(e.message || e) });
  }
}

// Extract named images and write to outDir
const extracted = [];
const namedSet = new Set();
for (const s of perStream) {
  if (!s.resources) continue;
  // Compute per-entry upper bound so we don't attribute the next entry's image
  // to this entry's name. Sort by entryStart, then look up the successor.
  const sortedByOffset = [...s.resources].sort((a, b) => a.entryStart - b.entryStart);
  const upperBound = new Map();
  for (let i = 0; i < sortedByOffset.length; i++) {
    const next = sortedByOffset[i + 1];
    upperBound.set(sortedByOffset[i].entryStart, next ? next.entryStart : data.length);
  }
  for (const res of s.resources) {
    const limit = upperBound.get(res.entryStart);
    const windowSize = Math.min(limit - res.entryStart, 2 * 1024 * 1024);
    const hit = findImageInEntry(data, res.entryStart, windowSize);
    if (!hit || hit.offset >= limit) continue;

    // Determine payload length from type-prefix if possible
    // Simplified: read ByteArray-style length (int32) placed right before the image bytes,
    // where the 4 bytes at (hit.offset - 4) are length-prefix. If length > file size, fall back to end-scan.
    let endOffset;
    if (hit.offset >= 4) {
      const possibleLen = data.readInt32LE(hit.offset - 4);
      if (possibleLen > 0 && hit.offset + possibleLen <= data.length) {
        endOffset = hit.offset + possibleLen;
      }
    }
    if (!endOffset) {
      // Fall back: scan to end-of-image marker for PNG/JPG/GIF
      if (hit.ext === 'png') {
        const tail = Buffer.from([0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82]);
        const t = data.indexOf(tail, hit.offset);
        endOffset = t > 0 ? t + tail.length : hit.offset + 512 * 1024;
      } else if (hit.ext === 'jpg') {
        const tail = Buffer.from([0xFF, 0xD9]);
        const t = data.indexOf(tail, hit.offset + 3);
        endOffset = t > 0 ? t + tail.length : hit.offset + 512 * 1024;
      } else if (hit.ext === 'gif') {
        const tail = Buffer.from([0x00, 0x3B]);
        const t = data.indexOf(tail, hit.offset);
        endOffset = t > 0 ? t + tail.length : hit.offset + 512 * 1024;
      } else {
        endOffset = hit.offset + 512 * 1024;
      }
    }

    const payload = data.subarray(hit.offset, endOffset);
    if (payload.length < 32) continue;

    const safeName = res.name.replace(/[\/\\:*?"<>|]/g, '_');
    let fname = `${base}__${safeName}`;
    if (!fname.toLowerCase().endsWith('.' + hit.ext)) fname += '.' + hit.ext;
    // Avoid collisions
    let candidate = fname;
    let n = 1;
    while (namedSet.has(candidate)) {
      candidate = fname.replace(/(\.[^.]+)$/, `.${n++}$1`);
    }
    namedSet.add(candidate);

    fs.writeFileSync(path.join(outDir, candidate), payload);
    extracted.push({ name: res.name, file: candidate, size: payload.length, ext: hit.ext });
  }
}

const idxPath = path.join(outDir, `${base}__NAMED_INDEX.json`);
fs.writeFileSync(idxPath, JSON.stringify({ source: inputPath, streams: perStream.map(s => ({ streamStart: s.streamStart, numResources: s.numResources || 0, error: s.error })), extracted }, null, 2));
console.log(JSON.stringify({ source: base, streams: streams.length, extracted: extracted.length, byExt: extracted.reduce((a, e) => { a[e.ext] = (a[e.ext] || 0) + 1; return a; }, {}) }));
