// Extract streams from an OLE compound document (.vbd / .app / .sub).
// Non-destructive — reads input only, writes copies to <output>/.
//
//   node extract_ole.js <input.vbd> <output-dir>

const fs = require('fs');
const path = require('path');
const CFB = require('cfb');

const [inputPath, outDir] = process.argv.slice(2);
if (!inputPath || !outDir) {
  console.error('usage: node extract_ole.js <input> <output-dir>');
  process.exit(2);
}

function detectExt(buf) {
  if (buf.length >= 8) {
    const h = buf.slice(0, 8);
    if (h[0] === 0x89 && h[1] === 0x50 && h[2] === 0x4E && h[3] === 0x47) return 'png';
    if (h[0] === 0xFF && h[1] === 0xD8 && h[2] === 0xFF) return 'jpg';
    if (h[0] === 0x47 && h[1] === 0x49 && h[2] === 0x46 && h[3] === 0x38) return 'gif';
    if (h[0] === 0x42 && h[1] === 0x4D) return 'bmp';
    if (h[0] === 0xD0 && h[1] === 0xCF && h[2] === 0x11 && h[3] === 0xE0) return 'cfbf';
    if (h[0] === 0x50 && h[1] === 0x4B && h[2] === 0x03 && h[3] === 0x04) return 'zip';
    if (h[0] === 0x3C && (h[1] === 0x3F || h[1] === 0x21)) return 'xml';
  }
  const asText = buf.slice(0, Math.min(buf.length, 256)).toString('utf8');
  if (/^<\?xml|^<[A-Za-z]/.test(asText.trim())) return 'xml';
  return 'bin';
}

fs.mkdirSync(outDir, { recursive: true });

const cfb = CFB.read(inputPath, { type: 'file' });

const index = [];
for (const stream of cfb.FullPaths.map((p, i) => ({ path: p, entry: cfb.FileIndex[i] }))) {
  const e = stream.entry;
  if (!e || e.type !== 2) continue; // 2 = stream
  const buf = Buffer.from(e.content);
  const ext = detectExt(buf);
  const safe = stream.path.replace(/^\//, '').replace(/[\/\\:*?"<>|]/g, '_') || 'root';
  const outName = `${safe}.${ext}`;
  const outPath = path.join(outDir, outName);
  fs.writeFileSync(outPath, buf);
  index.push({ stream: stream.path, size: buf.length, ext, file: outName });
}

const manifestPath = path.join(outDir, 'INDEX.json');
fs.writeFileSync(manifestPath, JSON.stringify({ source: inputPath, streams: index }, null, 2));
console.log(JSON.stringify({ source: inputPath, outDir, streamCount: index.length, byExt: index.reduce((acc, s) => { acc[s.ext] = (acc[s.ext] || 0) + 1; return acc; }, {}) }));
