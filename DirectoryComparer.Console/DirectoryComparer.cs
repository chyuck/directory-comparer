using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agero.Core.Checker;
using DirectoryComparer.Console.Models;

namespace DirectoryComparer.Console
{
    public static class DirectoryComparer
    {
        public static IReadOnlyCollection<BaseCompareResult> Compare(string dir1Path, string dir2Path)
        {
            Check.ArgumentIsNullOrWhiteSpace(dir1Path, nameof(dir1Path));
            Check.ArgumentIsNullOrWhiteSpace(dir2Path, nameof(dir2Path));
            
            Check.Argument(Directory.Exists(dir1Path), "Directory.Exists(dir1Path)");
            Check.Argument(Directory.Exists(dir2Path), "Directory.Exists(dir2Path)");

            var fileCompareResults = CompareFilesInDirectories(dir1Path, dir2Path).Cast<BaseCompareResult>();
            var dirCompareResults = CompareDirectoriesInDirectories(dir1Path, dir2Path).Cast<BaseCompareResult>();

            return fileCompareResults.Concat(dirCompareResults).ToArray();
        }

        private static IReadOnlyCollection<FileCompareResult> CompareFilesInDirectories(string dir1Path, string dir2Path)
        {
            Check.ArgumentIsNullOrWhiteSpace(dir1Path, nameof(dir1Path));
            Check.ArgumentIsNullOrWhiteSpace(dir2Path, nameof(dir2Path));
            
            Check.Argument(Directory.Exists(dir1Path), "Directory.Exists(dir1Path)");
            Check.Argument(Directory.Exists(dir2Path), "Directory.Exists(dir2Path)");

            var filePathsInDir1 = Directory.GetFiles(dir1Path);
            var fileNamesInDir1 = filePathsInDir1.ToDictionary(Path.GetFileName, i => i);
            
            var filePathsInDir2 = Directory.GetFiles(dir2Path);
            var fileNamesInDir2 = filePathsInDir2.ToDictionary(Path.GetFileName, i => i);

            var fileResultsOnlyInDir1 = fileNamesInDir1
                .Where(kv => !fileNamesInDir2.ContainsKey(kv.Key))
                .Select(kv => new FileCompareResult(kv.Key, kv.Value, null, false));

            var fileResultsOnlyInDir2 = fileNamesInDir2
                .Where(kv => !fileNamesInDir1.ContainsKey(kv.Key))
                .Select(kv => new FileCompareResult(kv.Key, null, kv.Value, false));

            var fileResultsInBothDirs = fileNamesInDir1
                .Select(kv =>
                {
                    if (!fileNamesInDir2.TryGetValue(kv.Key, out var filePathInDir2))
                        return null;

                    var areSame = FileComparer.AreSame(kv.Value, filePathInDir2);

                    return new FileCompareResult(kv.Key, kv.Value, filePathInDir2, areSame);
                })
                .Where(r => r != null);

            return fileResultsOnlyInDir1.Concat(fileResultsOnlyInDir2).Concat(fileResultsInBothDirs).ToArray();
        }
        
        private static IReadOnlyCollection<DirectoryCompareResult> CompareDirectoriesInDirectories(string dir1Path, string dir2Path)
        {
            Check.ArgumentIsNullOrWhiteSpace(dir1Path, nameof(dir1Path));
            Check.ArgumentIsNullOrWhiteSpace(dir2Path, nameof(dir2Path));
            
            Check.Argument(Directory.Exists(dir1Path), "Directory.Exists(dir1Path)");
            Check.Argument(Directory.Exists(dir2Path), "Directory.Exists(dir2Path)");

            var dirPathsInDir1 = Directory.GetDirectories(dir1Path);
            var dirNamesInDir1 = dirPathsInDir1.ToDictionary(Path.GetFileName, i => i);
            
            var dirPathsInDir2 = Directory.GetDirectories(dir2Path);
            var dirNamesInDir2 = dirPathsInDir2.ToDictionary(Path.GetFileName, i => i);

            var dirResultsOnlyInDir1 = dirNamesInDir1
                .Where(kv => !dirNamesInDir2.ContainsKey(kv.Key))
                .Select(kv => new DirectoryCompareResult(kv.Key, kv.Value, null, false, items: GetDirectoryResults(kv.Value, isDir1: true)));

            var dirResultsOnlyInDir2 = dirNamesInDir2
                .Where(kv => !dirNamesInDir1.ContainsKey(kv.Key))
                .Select(kv => new DirectoryCompareResult(kv.Key, null, kv.Value, false, items: GetDirectoryResults(kv.Value, isDir1: false)));

            var dirResultsInBothDirs = dirNamesInDir1
                .Select(kv =>
                {
                    if (!dirNamesInDir2.TryGetValue(kv.Key, out var dirPathInDir2))
                        return null;

                    var items = Compare(kv.Value, dirPathInDir2);

                    var areSame = items.All(i => i.AreSame);

                    return new DirectoryCompareResult(kv.Key, kv.Value, dirPathInDir2, areSame, items);
                })
                .Where(r => r != null);

            return dirResultsOnlyInDir1.Concat(dirResultsOnlyInDir2).Concat(dirResultsInBothDirs).ToArray();
        }

        private static IReadOnlyCollection<BaseCompareResult> GetDirectoryResults(string dirPath, bool isDir1)
        {
            Check.ArgumentIsNullOrWhiteSpace(dirPath, nameof(dirPath));
            Check.Argument(Directory.Exists(dirPath), "Directory.Exists(dirPath)");

            var filePaths = Directory.GetFiles(dirPath);
            var fileResults = filePaths
                .Select(p => new FileCompareResult(Path.GetFileName(p), isDir1 ? p : null, isDir1 ? null: p, false))
                .Cast<BaseCompareResult>();

            var dirPaths = Directory.GetDirectories(dirPath);
            var dirResults = dirPaths
                .Select(p =>
                {
                    var items = GetDirectoryResults(p, isDir1);
                    
                    return new DirectoryCompareResult(Path.GetDirectoryName(p), isDir1 ? p : null, isDir1 ? null : p, false, items);
                })
                .Cast<BaseCompareResult>();

            return fileResults.Concat(dirResults).ToArray();
        }
    }
}