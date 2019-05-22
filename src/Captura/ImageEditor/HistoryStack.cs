using System.Collections.Generic;

namespace Captura
{
    public class HistoryStack : NotifyPropertyChanged
    {
        readonly Stack<IHistoryItem> _stack = new Stack<IHistoryItem>();

        public void Push(IHistoryItem Item)
        {
            _stack.Push(Item);

            RaisePropertyChanged(nameof(Count));
        }

        public IHistoryItem Peek() => _stack.Peek();

        public IHistoryItem Pop()
        {
            try
            {
                return _stack.Pop();
            }
            finally
            {
                RaisePropertyChanged(nameof(Count));
            }
        }

        public void Clear()
        {
            _stack.Clear();

            RaisePropertyChanged(nameof(Count));
        }

        public int Count => _stack.Count;
    }
}