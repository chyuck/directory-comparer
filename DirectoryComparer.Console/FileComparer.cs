using System.IO;
using Agero.Core.Checker;

namespace DirectoryComparer.Console
{
    public static class FileComparer
    {
        public static bool AreSame(string file1Path, string file2Path)
        {
            Check.ArgumentIsNullOrWhiteSpace(file1Path, nameof(file1Path));
            Check.ArgumentIsNullOrWhiteSpace(file2Path, nameof(file2Path));
            
            Check.Argument(File.Exists(file1Path), "File.Exists(file1Path)");
            Check.Argument(File.Exists(file2Path), "File.Exists(file2Path)");

            if (file1Path == file2Path)
                return true;

            using (var fs1 = new FileStream(file1Path, FileMode.Open))
            {
                using (var fs2 = new FileStream(file2Path, FileMode.Open))
                {
                    if (fs1.Length != fs2.Length)
                        return false;

                    for (;;)
                    {
                        var byte1 = fs1.ReadByte();
                        var byte2 = fs2.ReadByte();

                        if (byte1 != byte2)
                            return false;
                        
                        if (byte1 == -1)
                            break;
                    }
                }
            }

            return true;
        }
    }
}