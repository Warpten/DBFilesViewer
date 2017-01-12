using System.ComponentModel;
using DBFilesClient.NET.Types;

namespace DBFilesViewer.Data.Structures.Types
{
    [TypeConverter(typeof(ForeignKeyConverter))]
    public class MultiForeignKey<T> : IObjectType<int> where T : class, new()
    {
        public MultiForeignKey(int underlyingValue) : base(underlyingValue)
        {
        }

        public ForeignKey<T>[] Values { get; set; }

        public override string ToString()
        {
            return $"{typeof(T).Name}[] (#{Key})";
        }
    }
}
