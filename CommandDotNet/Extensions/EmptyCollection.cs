using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Extensions
{
    internal class EmptyCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
        internal static readonly EmptyCollection<T> Instance = new EmptyCollection<T>();

        public int Count { get; } = 0;

        public bool IsReadOnly { get; } = true;

        public void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}