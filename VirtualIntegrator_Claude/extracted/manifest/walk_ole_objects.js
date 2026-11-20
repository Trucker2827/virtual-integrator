// Walk the Graphics stream of a .vbd/.sub/.app/.fpt OLE compound document
// and enumerate every printable ASCII run (length 4–64 bytes of
// [A-Za-z0-9 _.-]) with its offset. The Circon graphic serializer uses a
// custom format — no clean MFC CArchive markers — but class names, device
// names, point names, and literal text labels all appear as inline ASCII.
// Non-destructive.
//
//   node walk_ole_objects.js <input> <output-dir>

const fs = require('fs');
const path = require('path');
const CFB = require('cfb');

const [inputPath, outDir] = process.argv.slice(2);
if (!inputPath || !outDir) {
  console.error('usage: node walk_ole_objects.js <input> <output-dir>');
  process.exit(2);
}
fs.mkdirSync(outDir, { recursive: true });

const cfb = CFB.read(inputPath, { type: 'file' });
const graphics = cfb.FullPaths.map((p, i) => ({ path: p, entry: cfb.FileIndex[i] }))
  .find(s => s.path.endsWith('Graphics'));
if (!graphics || !graphics.entry || !graphics.entry.content) {
  console.error(JSON.stringify({ source: inputPath, error: 'no Graphics stream' }));
  process.exit(0);
}
const buf = Buffer.from(graphics.entry.content);

function isPrintable(b) {
  return (b >= 0x20 && b <= 0x7E);
}

const strings = [];
let start = -1;
for (let i = 0; i <= buf.length; i++) {
  const b = i < buf.length ? buf[i] : 0;
  if (isPrintable(b)) {
    if (start < 0) start = i;
  } else {
    if (start >= 0) {
      const len = i - start;
      if (len >= 4 && len <= 64) {
        strings.push({ offset: start, text: buf.subarray(start, i).toString('ascii') });
      }
      start = -1;
    }
  }
}

// Partition strings into likely categories based on shape
const classLike = new Set();
const deviceLike = new Set();
const otherLabels = new Set();
for (const s of strings) {
  const t = s.text;
  // CXxxYyy — MFC-style class name
  if (/^C[A-Z][A-Za-z0-9]{2,}$/.test(t)) classLike.add(t);
  // UHC-300, VAV-1-01, PIU-1-12, RTU_Floor1, SMC300, APC-300 etc. — device identifiers
  else if (/^(UHC|VAV|PIU|RTU|SMC|APC|SCC|MAC|UVC|HPC|RTC|FCC|GPC|PRG)[-_A-Za-z0-9]{1,}$/.test(t)) deviceLike.add(t);
  else otherLabels.add(t);
}

const tally = (set) => [...set].sort();

const result = {
  source: inputPath,
  graphicsStreamSize: buf.length,
  totalStrings: strings.length,
  classCount: classLike.size,
  deviceIdCount: deviceLike.size,
  classes: tally(classLike),
  deviceIds: tally(deviceLike),
  labels: tally(otherLabels).slice(0, 200),
};

const base = path.basename(inputPath);
const safe = base.replace(/[\/\\:*?"<>|]/g, '_');
fs.writeFileSync(path.join(outDir, `${safe}.object-graph.json`), JSON.stringify(result, null, 2));
console.log(JSON.stringify({ source: base, classes: result.classCount, deviceIds: result.deviceIdCount, stringsTotal: result.totalStrings }));
