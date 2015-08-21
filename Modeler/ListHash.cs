using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler {
    class ListHash<T> : IList<T> {
        private List<T> vals = new List<T>();
        private HashSet<T> valHash = new HashSet<T>();
        public int IndexOf(T item) {
            return this.vals.IndexOf(item);
        }

        public void Insert(int index, T item) {
            if (this.valHash.Contains(item)) {
                throw new Exception("We already contain have this value");
            }
            this.vals.Insert(index, item);
        }

        public void RemoveAt(int index) {
            var toRemove = this.vals[index];
            this.vals.RemoveAt(index);
            this.valHash.Remove(toRemove);
        }

        public T this[int index] {
            get {
                return this.vals[index];
            }
            set {
                if (this.valHash.Contains(value)) {
                    throw new Exception("We already contain this values");
                }
                var toRemove = this.vals[index];
                this.valHash.Remove(toRemove);
                this.vals[index] = value;
                this.valHash.Add(value);
            }
        }

        public void Add(T item) {
            this.vals.Add(item);
            this.valHash.Add(item);
        }

        public void Clear() {
            this.vals.Clear();
            this.valHash.Clear();
        }

        public bool Contains(T item) {
            return this.valHash.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this.vals.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return this.vals.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(T item) {
            var r = this.vals.Remove(item);
            if (!r) {
                return r;
            }
            r = this.valHash.Remove(item);
            if (!r) {
                throw new Exception("Removed from list but not hashset");
            }
            return true;
        }

        public IEnumerator<T> GetEnumerator() {
            return this.vals.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.vals.GetEnumerator();
        }
    }
}
