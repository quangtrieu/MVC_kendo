using System.Collections.Generic;
using System.Threading;

namespace BCS.Framework.Commons.Special
{
    internal abstract class BufferWriter<T>
    {
        private IList<T> items = new List<T>();

        private const int FLUSH_PERIOD = 10 * 1000; // 10 seconds
        private const int FLUSH_COUNT = 100;

        public BufferWriter()
        {
            Timer timer = new Timer(FlushAll, null, FLUSH_PERIOD, FLUSH_PERIOD);
        }

        public void Write(T item)
        {
            lock (this)
            {
                items.Add(item);

                if (items.Count > FLUSH_COUNT)
                {
                    OnFlush(items);

                    items = new List<T>();
                }
            }
        }

        public void Set(string key, T item)
        {
            
        }

        public T Get(string key)
        {
            return default(T);
        }

        private void FlushAll(object state)
        {
            lock (this)
            {
                OnFlush(items);

                items = new List<T>();
            }
        }

        protected abstract void OnFlush(IList<T> items);
    }
}