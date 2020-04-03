using System.Collections;
using System.Collections.Generic;

namespace Captura.MouseKeyHook
{
    class KeyRecords : IEnumerable<IKeyRecord>
    {
        readonly List<IKeyRecord> _records = new List<IKeyRecord>();

        public KeyRecords(int Size)
        {
            this.Size = Size;
        }

        public int Size { get; }

        public void Clear() => _records.Clear();

        public void Add(IKeyRecord KeyRecord)
        {
            if (_records.Count == Size)
            {
                _records.RemoveAt(0);
            }

            _records.Add(KeyRecord);
        }
        
        public IKeyRecord Last
        {
            get => _records.Count == 0 ? null : _records [_records.Count - 1];
            set
            {
                if (_records.Count == 0)
                    _records.Add(value);
                else _records[_records.Count - 1] = value;
            }
        } 

        public IEnumerator<IKeyRecord> GetEnumerator()
        {
            for (var i = _records.Count - 1; i >= 0; --i)
                yield return _records[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}