using System;
using System.IO;
using CsvHelper;
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
                var outputFile = Path.Combine(outputDir, $"{fileName}.csv");
                
                using (var writer = new StreamWriter(outputFile))
                using (var csv = new CsvWriter(writer))
                {    
                    csv.WriteRecords(results);
                }
                
                System.Console.WriteLine($"Compare result has been saved to '{outputFile}' file."); 
            }

        }
    }
}