// Scan binary files for embedded image magic numbers (PNG, JPEG, GIF, BMP, EMF).
// Non-destructive: reads only. Carves out hit regions to a sibling folder.
//
//   node scan_images.js <input-file> <output-dir>

const fs = require('fs');
const path = require('path');

const [inputPath, outDir] = process.argv.slice(2);
if (!inputPath || !outDir) {
  console.error('usage: node scan_images.js <input> <output-dir>');
  process.exit(2);
}

fs.mkdirSync(outDir, { recursive: true });
const buf = fs.readFileSync(inputPath);

// Magic byte sequences we recognize
const sigs = [
  { name: 'png', head: Buffer.from([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]), tail: Buffer.from([0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82]), ext: 'png' },
  { name: 'jpeg', head: Buffer.from([0xFF, 0xD8, 0xFF]), tail: Buffer.from([0xFF, 0xD9]), ext: 'jpg' },
  { name: 'gif87', head: Buffer.from('GIF87a'), tail: Buffer.from([0x00, 0x3B]), ext: 'gif' },
  { name: 'gif89', head: Buffer.from('GIF89a'), tail: Buffer.from([0x00, 0x3B]), ext: 'gif' },
  { name: 'bmp', head: Buffer.from('BM'), tail: null, ext: 'bmp' },
  { name: 'emf', head: Buffer.from([0x01, 0x00, 0x00, 0x00]), tail: null, ext: 'emf' },
];

let hits = 0;
const results = [];
for (const sig of sigs) {
  let from = 0;
  while (true) {
    const start = buf.indexOf(sig.head, from);
    if (start < 0) break;
    let end = -1;
    if (sig.tail) {
      const tailStart = buf.indexOf(sig.tail, start + sig.head.length);
      if (tailStart > 0) end = tailStart + sig.tail.length;
    }
    if (end < 0) {
      // No tail — skip (BMP/EMF need length from header; too noisy to carve blindly)
      from = start + sig.head.length;
      continue;
    }
    const slice = buf.subarray(start, end);
    if (slice.length < 64) { from = start + 1; continue; }
    const outName = `${path.basename(inputPath)}.offset_${start}.${sig.ext}`;
    fs.writeFileSync(path.join(outDir, outName), slice);
    results.push({ sig: sig.name, offset: start, size: slice.length, file: outName });
    hits++;
    from = end;
  }
}

console.log(JSON.stringify({ source: inputPath, hits, files: results }));
