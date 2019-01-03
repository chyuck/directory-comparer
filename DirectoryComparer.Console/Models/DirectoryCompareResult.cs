using System.Collections.Generic;
using System.Runtime.Serialization;
using Agero.Core.Checker;

namespace DirectoryComparer.Console.Models
{
    [DataContract]
    public class DirectoryCompareResult : BaseCompareResult
    {
        public DirectoryCompareResult(string name, string fullPath1, string fullPath2, bool areSame, IReadOnlyCollection<BaseCompareResult> items) 
            : base(name, fullPath1, fullPath2, areSame)
        {
            Check.ArgumentIsNull(items, nameof(items));
            
            Items = items;
        }
        
        [DataMember(Name = "items")]
        public IReadOnlyCollection<BaseCompareResult> Items { get; }
    }
}