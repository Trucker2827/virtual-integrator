// Walk the .rsrc section of a Windows PE (OCX/DLL/EXE) and dump every
// RT_BITMAP, RT_ICON, RT_CURSOR, RT_GROUP_ICON, RT_GROUP_CURSOR resource
// to disk. Also surfaces RT_STRING tables as JSON for reference.
//
//   node extract_pe_resources.js <input.ocx|.dll|.exe> <output-dir>

const fs = require('fs');
const path = require('path');

const [inputPath, outDir] = process.argv.slice(2);
if (!inputPath || !outDir) {
  console.error('usage: node extract_pe_resources.js <input> <output-dir>');
  process.exit(2);
}

(async () => {
const { NtExecutable, NtExecutableResource } = await import('resedit');

fs.mkdirSync(outDir, { recursive: true });

const bytes = fs.readFileSync(inputPath);
let exe;
try {
  // ignoreCert: handle Authenticode-signed binaries
  exe = NtExecutable.from(bytes, { ignoreCert: true });
} catch (e) {
  console.error(JSON.stringify({ source: inputPath, error: String(e.message || e) }));
  process.exit(0);
}

let res;
try {
  res = NtExecutableResource.from(exe);
} catch (e) {
  console.error(JSON.stringify({ source: inputPath, error: 'no .rsrc: ' + String(e.message || e) }));
  process.exit(0);
}

const base = path.basename(inputPath).replace(/\.[^.]+$/, '');
const index = [];

// Bitmaps: wrap the raw DIB in a 14-byte BMP file header
function bmpFromDib(dib) {
  const infoHeaderSize = dib.readUInt32LE(0); // biSize
  const bitCount = dib.readUInt16LE(14);
  const clrUsed = dib.readUInt32LE(32);
  const paletteSize = (clrUsed || (bitCount <= 8 ? 1 << bitCount : 0)) * 4;
  const pixelOffset = 14 + infoHeaderSize + paletteSize;
  const total = 14 + dib.length;
  const out = Buffer.alloc(total);
  out.writeUInt8(0x42, 0);
  out.writeUInt8(0x4D, 1);
  out.writeUInt32LE(total, 2);
  out.writeUInt32LE(0, 6);
  out.writeUInt32LE(pixelOffset, 10);
  dib.copy(out, 14);
  return out;
}

// Icons: group entry + image data
function icoFromGroup(groupBuf, entriesByOrdinal) {
  // GRPICONDIR: idReserved(2) idType(2) idCount(2) then idCount entries
  const idCount = groupBuf.readUInt16LE(4);
  const entries = [];
  for (let i = 0; i < idCount; i++) {
    const off = 6 + i * 14;
    const width = groupBuf.readUInt8(off);
    const height = groupBuf.readUInt8(off + 1);
    const colorCount = groupBuf.readUInt8(off + 2);
    const planes = groupBuf.readUInt16LE(off + 4);
    const bitCount = groupBuf.readUInt16LE(off + 6);
    const bytesInRes = groupBuf.readUInt32LE(off + 8);
    const ordinal = groupBuf.readUInt16LE(off + 12);
    const img = entriesByOrdinal.get(ordinal);
    if (!img) continue;
    entries.push({ width, height, colorCount, planes, bitCount, bytesInRes, img });
  }
  // ICONDIR: idReserved(2) idType(2 = ICO) idCount(2) then entries ICONDIRENTRY (16 bytes each)
  let dataOffset = 6 + entries.length * 16;
  let total = dataOffset + entries.reduce((s, e) => s + e.img.length, 0);
  const out = Buffer.alloc(total);
  out.writeUInt16LE(0, 0);
  out.writeUInt16LE(1, 2); // ICO
  out.writeUInt16LE(entries.length, 4);
  let p = 6;
  let d = dataOffset;
  for (const e of entries) {
    out.writeUInt8(e.width, p);
    out.writeUInt8(e.height, p + 1);
    out.writeUInt8(e.colorCount, p + 2);
    out.writeUInt8(0, p + 3);
    out.writeUInt16LE(e.planes, p + 4);
    out.writeUInt16LE(e.bitCount, p + 6);
    out.writeUInt32LE(e.img.length, p + 8);
    out.writeUInt32LE(d, p + 12);
    e.img.copy(out, d);
    d += e.img.length;
    p += 16;
  }
  return out;
}

// Pass 1: collect RT_ICON image data keyed by ordinal
const iconsByOrdinal = new Map();
for (const e of res.entries) {
  if (e.type === 3) { // RT_ICON
    iconsByOrdinal.set(Number(e.id), Buffer.from(e.bin));
  }
}
const cursorsByOrdinal = new Map();
for (const e of res.entries) {
  if (e.type === 1) { // RT_CURSOR
    cursorsByOrdinal.set(Number(e.id), Buffer.from(e.bin));
  }
}

for (const e of res.entries) {
  const buf = Buffer.from(e.bin);
  const safeId = String(e.id).replace(/[^A-Za-z0-9._-]/g, '_');
  const typeName = {
    1: 'CURSOR', 2: 'BITMAP', 3: 'ICON', 4: 'MENU', 5: 'DIALOG', 6: 'STRING',
    7: 'FONTDIR', 8: 'FONT', 9: 'ACCELERATOR', 10: 'RCDATA', 11: 'MESSAGETABLE',
    12: 'GROUP_CURSOR', 14: 'GROUP_ICON', 16: 'VERSION', 23: 'HTML', 24: 'MANIFEST',
  }[e.type] || `TYPE${e.type}`;

  if (e.type === 2) {
    const outName = `${base}__BITMAP_${safeId}.bmp`;
    try {
      fs.writeFileSync(path.join(outDir, outName), bmpFromDib(buf));
      index.push({ type: typeName, id: String(e.id), size: buf.length, file: outName });
    } catch (err) {
      index.push({ type: typeName, id: String(e.id), size: buf.length, error: String(err.message || err) });
    }
  } else if (e.type === 14) {
    const outName = `${base}__ICON_${safeId}.ico`;
    try {
      fs.writeFileSync(path.join(outDir, outName), icoFromGroup(buf, iconsByOrdinal));
      index.push({ type: typeName, id: String(e.id), size: buf.length, file: outName });
    } catch (err) {
      index.push({ type: typeName, id: String(e.id), size: buf.length, error: String(err.message || err) });
    }
  } else if (e.type === 12) {
    const outName = `${base}__CURSOR_${safeId}.cur`;
    try {
      fs.writeFileSync(path.join(outDir, outName), buf);
      index.push({ type: typeName, id: String(e.id), size: buf.length, file: outName });
    } catch (err) {
      index.push({ type: typeName, id: String(e.id), size: buf.length, error: String(err.message || err) });
    }
  } else if (e.type === 10) {
    // RCDATA — sometimes holds PNG/GIF/XML
    const outName = `${base}__RCDATA_${safeId}.bin`;
    fs.writeFileSync(path.join(outDir, outName), buf);
    index.push({ type: typeName, id: String(e.id), size: buf.length, file: outName });
  } else if (e.type === 6) {
    // String table
    index.push({ type: typeName, id: String(e.id), size: buf.length });
  }
}

const indexPath = path.join(outDir, `${base}__INDEX.json`);
fs.writeFileSync(indexPath, JSON.stringify({ source: inputPath, entries: index }, null, 2));
const counts = index.reduce((acc, r) => { acc[r.type] = (acc[r.type] || 0) + 1; return acc; }, {});
console.log(JSON.stringify({ source: path.basename(inputPath), counts }));
})().catch(e => { console.error(JSON.stringify({ source: inputPath, error: String(e.message || e) })); process.exit(1); });
