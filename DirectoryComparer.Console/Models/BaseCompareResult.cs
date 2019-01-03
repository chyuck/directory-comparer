using System.Runtime.Serialization;
using Agero.Core.Checker;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DirectoryComparer.Console.Models
{
    [DataContract]
    public abstract class BaseCompareResult
    {
        protected BaseCompareResult(string name, string fullPath1, string fullPath2, bool areSame)
        {
            Check.ArgumentIsNullOrWhiteSpace(name, nameof(name));
            Check.Argument(!string.IsNullOrWhiteSpace(fullPath1) || !string.IsNullOrWhiteSpace(fullPath2), "!string.IsNullOrWhiteSpace(fullPath1) || !string.IsNullOrWhiteSpace(fullPath2)");           
            
            Name = name;
            FullPath1 = fullPath1;
            FullPath2 = fullPath2;
            AreSame = areSame;
        }

        [DataMember(Name = "name")]
        public string Name { get; }
        
        [DataMember(Name = "fullPath1")]
        public string FullPath1 { get; }
        
        [DataMember(Name = "fullPath2")]
        public string FullPath2 { get; }

        [DataMember(Name = "areSame")]
        public bool AreSame { get; }
    }
}