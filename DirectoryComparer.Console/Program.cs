using System;
using System.Collections.Generic;
using System.IO;
using Agero.Core.Checker;
using CsvHelper;
using DirectoryComparer.Console.Models;
using Newtonsoft.Json;

namespace DirectoryComparer.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                System.Console.WriteLine("Two arguments must be provided with directories paths to be compared.");
                return;
            }

            var results = DirectoryComparer.Compare(args[0], args[1]);

            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "out");
            Directory.CreateDirectory(outputDir);
            
            var fileName = Guid.NewGuid().ToString("N");

            {
                var json = JsonConvert.SerializeObject(results);
                var outputFile = Path.Combine(outputDir, $"{fileName}.json");
                File.WriteAllText(outputFile, json);
                System.Console.WriteLine($"Compare result has been saved to '{outputFile}' file.");
            }

            {
                var flatResults = FlattenResults(results);
                
                var outputFile = Path.Combine(outputDir, $"{fileName}.csv");
                
                using (var writer = new StreamWriter(outputFile))
                using (var csv = new CsvWriter(writer))
                {    
                    csv.WriteRecords(flatResults);
                }
                
                System.Console.WriteLine($"Compare result has been saved to '{outputFile}' file."); 
            }
        }

        private static IEnumerable<BaseCompareResult> FlattenResults(IReadOnlyCollection<BaseCompareResult> results)
        {
            Check.ArgumentIsNull(results, nameof(results));

            foreach (var result in results)
            {
                yield return result;
                
                if (!(result is DirectoryCompareResult dirResult)) 
                    continue;

                var flatResults = FlattenResults(dirResult.Items);

                foreach (var flattenResult in flatResults)
                    yield return flattenResult;
            }
        }
    }
}