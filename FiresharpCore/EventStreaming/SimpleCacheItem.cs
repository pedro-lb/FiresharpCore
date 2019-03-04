using System.Collections.Generic;

namespace FiresharpCore.EventStreaming
{
    internal class SimpleCacheItem
    {
        public List<SimpleCacheItem> Children { get; set; } = new List<SimpleCacheItem>();

        public SimpleCacheItem Parent { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
        public bool Created { get; set; }
    }
}