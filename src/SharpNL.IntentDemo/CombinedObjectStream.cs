using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNL.IntentDemo
{
    class CombinedObjectStream<T> : IObjectStream<T>
    {
        private IList<IObjectStream<T>> _streams;
        private int _streamIndex = 0;

        public CombinedObjectStream(IList<IObjectStream<T>> streams)
        {
            _streams = streams;
            _streamIndex = 0;
        }

        public T Read()
        {
            T obj = default(T);

            while (_streamIndex < _streams.Count && obj == null)
            {
                obj = _streams[_streamIndex].Read();
                if (obj == null)
                {
                    _streamIndex += 1;
                }
            }

            return obj;
        }

        public void Reset()
        {
            _streamIndex = 0;
        }

        public void Dispose()
        {
            foreach (IObjectStream<T> stream in _streams)
            {
                stream.Dispose();
            }
        }
    }
}
