using System;
using System.IO;
using System.Linq;
using DirectoryComparer.Console.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DirectoryComparer.Console.Tests
{
    [TestClass]
    public class DirectoryComparerTests
    {
        [TestMethod]
        public void TestCompare_When_Directories_Are_Different()
        {
            var tempDir = Path.GetTempPath();
            var testDir = Path.Combine(tempDir, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testDir);

            try
            {
                // Arrange
                var dir1Path = Path.Combine(testDir, "1");
                Directory.CreateDirectory(dir1Path);
                
                File.WriteAllText(Path.Combine(dir1Path, "both_same.txt"), "1234567890");
                File.WriteAllText(Path.Combine(dir1Path, "both_diff.txt"), "010");
                File.WriteAllText(Path.Combine(dir1Path, "only1.txt"), "1234567890");
                
                Directory.CreateDirectory(Path.Combine(dir1Path, "both"));
                File.WriteAllText(Path.Combine(dir1Path, "both", "both_same.txt"), "1234567890");
                File.WriteAllText(Path.Combine(dir1Path, "both", "both_diff.txt"), "010");
                File.WriteAllText(Path.Combine(dir1Path, "both", "only1.txt"), "1234567890");
                Directory.CreateDirectory(Path.Combine(dir1Path, "only1"));
                File.WriteAllText(Path.Combine(dir1Path, "only1", "test.txt"), "1234567890");       
                           
                var dir2Path = Path.Combine(testDir, "2");
                Directory.CreateDirectory(dir2Path);
                
                File.WriteAllText(Path.Combine(dir2Path, "both_same.txt"), "1234567890");
                File.WriteAllText(Path.Combine(dir2Path, "both_diff.txt"), "020");
                File.WriteAllText(Path.Combine(dir2Path, "only2.txt"), "1234567890");
                
                Directory.CreateDirectory(Path.Combine(dir2Path, "both"));
                File.WriteAllText(Path.Combine(dir2Path, "both", "both_same.txt"), "1234567890");
                File.WriteAllText(Path.Combine(dir2Path, "both", "both_diff.txt"), "020");
                File.WriteAllText(Path.Combine(dir2Path, "both", "only2.txt"), "1234567890");
                Directory.CreateDirectory(Path.Combine(dir2Path, "only2"));
                File.WriteAllText(Path.Combine(dir2Path, "only2", "test.txt"), "1234567890");
                
                // Act
                var results = DirectoryComparer.Compare(dir1Path, dir2Path);

                // Assert
                var fileResults = results.OfType<FileCompareResult>().ToArray();
                Assert.AreEqual(4, fileResults.Length);       
                var sameFileResult = fileResults.Single(r => r.Name == "both_same.txt");
                Assert.IsTrue(sameFileResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "both_same.txt"), sameFileResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "both_same.txt"), sameFileResult.FullPath2);
                var notSameFileResult = fileResults.Single(r => r.Name == "both_diff.txt");
                Assert.IsFalse(notSameFileResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "both_diff.txt"), notSameFileResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "both_diff.txt"), notSameFileResult.FullPath2);
                var only1FileResult = fileResults.Single(r => r.Name == "only1.txt");
                Assert.IsFalse(only1FileResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "only1.txt"), only1FileResult.FullPath1);
                Assert.IsNull(only1FileResult.FullPath2);
                var only2FileResult = fileResults.Single(r => r.Name == "only2.txt");
                Assert.IsFalse(only2FileResult.AreSame);
                Assert.IsNull(only2FileResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "only2.txt"), only2FileResult.FullPath2);
                
                var dirResults = results.OfType<DirectoryCompareResult>().ToArray();
                
                Assert.AreEqual(3, dirResults.Length);       
                var bothDirResult = dirResults.Single(r => r.Name == "both");
                Assert.IsFalse(bothDirResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "both"), bothDirResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "both"), bothDirResult.FullPath2);
                Assert.AreEqual(4, bothDirResult.Items.Count);
                var only1DirResult = dirResults.Single(r => r.Name == "only1");
                Assert.IsFalse(only1DirResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "only1"), only1DirResult.FullPath1);
                Assert.IsNull(only1DirResult.FullPath2);
                Assert.AreEqual(1, only1DirResult.Items.Count);
                var only2DirResult = dirResults.Single(r => r.Name == "only2");
                Assert.IsFalse(only2DirResult.AreSame);
                Assert.IsNull(only2DirResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "only2"), only2DirResult.FullPath2);
                Assert.AreEqual(1, only2DirResult.Items.Count);
            }
            finally
            {
                Directory.Delete(testDir, recursive: true);
            }
        }
        
        [TestMethod]
        public void TestCompare_When_Directories_Are_Same()
        {
            var tempDir = Path.GetTempPath();
            var testDir = Path.Combine(tempDir, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testDir);

            try
            {
                // Arrange
                var dir1Path = Path.Combine(testDir, "1");
                Directory.CreateDirectory(dir1Path);
                
                File.WriteAllText(Path.Combine(dir1Path, "both_same.txt"), "1234567890");
                
                Directory.CreateDirectory(Path.Combine(dir1Path, "both"));
                File.WriteAllText(Path.Combine(dir1Path, "both", "both_same.txt"), "1234567890");
                     
                var dir2Path = Path.Combine(testDir, "2");
                Directory.CreateDirectory(dir2Path);
                
                File.WriteAllText(Path.Combine(dir2Path, "both_same.txt"), "1234567890");
                
                Directory.CreateDirectory(Path.Combine(dir2Path, "both"));
                File.WriteAllText(Path.Combine(dir2Path, "both", "both_same.txt"), "1234567890");
                
                // Act
                var results = DirectoryComparer.Compare(dir1Path, dir2Path);

                // Assert
                var fileResults = results.OfType<FileCompareResult>().ToArray();
                Assert.AreEqual(1, fileResults.Length);       
                var sameFileResult = fileResults.Single(r => r.Name == "both_same.txt");
                Assert.IsTrue(sameFileResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "both_same.txt"), sameFileResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "both_same.txt"), sameFileResult.FullPath2);
                
                var dirResults = results.OfType<DirectoryCompareResult>().ToArray();
                
                Assert.AreEqual(1, dirResults.Length);       
                var bothDirResult = dirResults.Single(r => r.Name == "both");
                Assert.IsTrue(bothDirResult.AreSame);
                Assert.AreEqual(Path.Combine(dir1Path, "both"), bothDirResult.FullPath1);
                Assert.AreEqual(Path.Combine(dir2Path, "both"), bothDirResult.FullPath2);
                Assert.AreEqual(1, bothDirResult.Items.Count);
            }
            finally
            {
                Directory.Delete(testDir, recursive: true);
            }
        }
    }
}