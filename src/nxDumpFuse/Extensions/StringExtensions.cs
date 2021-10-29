using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nxDumpFuse.Model.Enums;

namespace nxDumpFuse.Extensions
{
    public static class StringExtensions
    {
        public static List<string> GetInputFiles(this string inputFilePath, FileCase fileCase)
        {
            var inputDir = Path.GetDirectoryName(inputFilePath);
            if (string.IsNullOrEmpty(inputDir)) return new List<string>();
            var files = new List<string>();
            switch (fileCase)
            {
                case FileCase.XciNumeric: // .xci.00
                case FileCase.NspNumeric: // .nsp.00
                    files = Directory.GetFiles(inputDir)
                        .Where(f => int.TryParse(Path.GetExtension(f).Replace(".", ""), out _))
                        .ToList();
                    break;
                case FileCase.Xci: // .xc0
                case FileCase.Nsp: // .ns0
                    files = Directory.GetFiles(inputDir, $"{Path.GetFileNameWithoutExtension(inputFilePath)}*")
                        .ToList();
                    break;
                case FileCase.Numeric: // dir/00
                    files = Directory.GetFiles(inputDir)
                        .Where(f => int.TryParse(Path.GetFileName(f), out _))
                        .ToList();
                    break;
            }
            files.Sort();
            return files;
        }

        public static Tuple<string,FileCase> GetOutputFilePath(this string inputFilePath, string outputDir)
        {
            var fileName = Path.GetFileName(inputFilePath);
            string outputFilePath;
            const string xciExt = "xci";
            const string nspExt = "nsp";

            if (Path.HasExtension(fileName))
            {
                var ext = Path.GetExtension(fileName).Replace(".", string.Empty);
                var split = fileName.Split(".").ToList();

                if (int.TryParse(ext, out _) && split.Count >= 3 && split[^2] == xciExt) // .xci.00
                {
                    outputFilePath = Path.Join(outputDir, $"{string.Join("", split.Take(split.Count - 2))}.{xciExt}");
                    return new Tuple<string, FileCase>(outputFilePath, FileCase.XciNumeric);
                }

                if (int.TryParse(ext, out _) && split.Count >= 3 && split[^2] == nspExt) // .nsp.00
                {
                    outputFilePath = Path.Join(outputDir, $"{string.Join("", split.Take(split.Count - 2))}.{nspExt}");
                    return new Tuple<string, FileCase>(outputFilePath, FileCase.NspNumeric);
                }

                switch (ext[..2])
                {
                    // .xc0
                    case "xc" when int.TryParse(ext.Substring(ext.Length - 1, 1), out _):
                        outputFilePath = Path.Join(outputDir, $"{Path.GetFileNameWithoutExtension(fileName)}.{xciExt}");
                        return new Tuple<string, FileCase>(outputFilePath, FileCase.Xci);
                    // .ns0
                    case "ns" when int.TryParse(ext.Substring(ext.Length - 1, 1), out _):
                        outputFilePath = Path.Join(outputDir, $"{Path.GetFileNameWithoutExtension(fileName)}.{nspExt}");
                        return new Tuple<string, FileCase>(outputFilePath, FileCase.Nsp);
                }
            }
            else // dir/00
            {
                var inputDir = new FileInfo(inputFilePath).Directory?.Name;
                if (string.IsNullOrEmpty(inputDir))
                {
                    inputDir = Path.GetPathRoot(inputFilePath);
                    outputFilePath = $"{inputDir}.{nspExt}";
                    return new Tuple<string, FileCase>(outputFilePath, FileCase.Numeric);
                }

                var inputDirSplit = inputDir.Split(".");
                outputFilePath = Path.Join(outputDir, inputDirSplit.Length == 1
                    ? $"{inputDir}.{nspExt}"
                    : $"{string.Join("", (inputDirSplit).Take(inputDirSplit.Length - 1))}.{nspExt}");
                return new Tuple<string, FileCase>(outputFilePath, FileCase.Numeric);
            }

            return new Tuple<string, FileCase>(string.Empty, FileCase.Invalid);
        }
    }
}
