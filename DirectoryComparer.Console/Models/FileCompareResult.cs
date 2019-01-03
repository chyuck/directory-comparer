using System.Runtime.Serialization;

namespace DirectoryComparer.Console.Models
{
    [DataContract]
    public class FileCompareResult : BaseCompareResult
    {
        public FileCompareResult(string name, string fullPath1, string fullPath2, bool areSame) 
            : base(name, fullPath1, fullPath2, areSame)
        {
        }
    }
}