Console.WriteLine($"Welcome to the VIM Reference application");
Console.WriteLine($"This application demonstrates how to parse a VIM file");

Console.WriteLine($"There are {args.Length} command-line arguments, expecting at least 1, a file-name");

var fileInfo = new FileInfo(args[0]);
Console.WriteLine($"File {fileInfo.Name} exists ({fileInfo.Exists}) and has {fileInfo.Length} bytes");

Console.WriteLine($"Opening {fileInfo.Name} as a read-only file stream");
using var stream = File.OpenRead(fileInfo.FullName);

Console.WriteLine("Creating a binary reader");
using var reader = new BinaryReader(stream);

var magic = reader.ReadUInt64();
Console.WriteLine($"Read magic number = {magic}, expecting {0xBFA5}");

var dataStart = reader.ReadUInt64();
Console.WriteLine($"Read data start = {dataStart}, expecting <= {fileInfo.Length}, and >= 64 + 16 * # arrays, and divisible by 64    ({dataStart % 64} == 0)");

var dataEnd = reader.ReadUInt64();
Console.WriteLine($"Read data end = {dataEnd}, expecting <= fileLength and >= {dataStart}");

var numArrays = reader.ReadUInt64();
Console.WriteLine($"# arrays = {numArrays}, expecting >= 1");

Console.WriteLine($"About to read range structures from position {stream.Position}, expecting 32");
var ranges = new List<(ulong Begin, ulong End)>();
for (var i = 0ul; i < numArrays; i++)
{
    var begin = reader.ReadUInt64();
    var end = reader.ReadUInt64();

    ranges.Add((begin, end));
    Console.WriteLine($"Range {i}, from {begin} to {end}");
}

Console.WriteLine($"Advancing to data start");
stream.Seek((long)dataStart, SeekOrigin.Begin);

var nameRange = ranges[0];
Console.WriteLine($"Stream position {stream.Position} should already be at the first buffer {nameRange.Begin}");

var nameByteCount = nameRange.End - nameRange.Begin;
Console.WriteLine($"Reading names, total byte count = {nameByteCount}");
var nameBytes = reader.ReadBytes((int)nameByteCount);

var names = System.Text.Encoding.UTF8.GetString(nameBytes).Split((char)0, StringSplitOptions.RemoveEmptyEntries).ToArray();
Console.WriteLine($"Found {names.Length} buffer names, expected {numArrays-1}");

for (var i = 0ul; i < numArrays - 1; i++)
    Console.WriteLine($"Buffer {i} = {names[i]}");

Console.WriteLine("Creating lookup of names to ranges");
var nameToRange = names.Zip(ranges.Skip(1)).ToDictionary(tuple => tuple.First, tuple => tuple.Second);

var hasHeader = nameToRange.TryGetValue("header", out var headerRange);
Console.WriteLine($"Has header = {hasHeader}");

var hasAssets = nameToRange.TryGetValue("assets", out var assetsRange);
Console.WriteLine($"Has assets = {hasAssets}");

var hasGeometry = nameToRange.TryGetValue("geometry", out var geometryRange);
Console.WriteLine($"Has geometry = {hasGeometry}");

var hasStrings = nameToRange.TryGetValue("strings", out var stringsRange);
Console.WriteLine($"Has assets = {hasStrings}");

var hasEntities = nameToRange.TryGetValue("entities", out var entitiesRange);
Console.WriteLine($"Has assets = {hasEntities}");

if (hasHeader)
{
    var headerByteCount = headerRange.End - headerRange.Begin;
    var headerBytes = reader.ReadBytes((int)headerByteCount);
    var header = System.Text.Encoding.UTF8.GetString(headerBytes);
    Console.WriteLine("Header Contents:");
    Console.WriteLine(header);
}

Console.ReadKey();