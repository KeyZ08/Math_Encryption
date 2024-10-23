using System.Buffers;
using System.Text;

// ReSharper disable InconsistentNaming

namespace QRCode.Polynoms;

public struct Polynom(int count) : IDisposable
{
    private PolynomItem[] _polyItems = RentArray(count);

    public void Add(PolynomItem item)
    {
        if (_polyItems.Length < Count + 1) throw new Exception(nameof(_polyItems) + " переполнен.");
        _polyItems[Count++] = item;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)Count)
            throw new IndexOutOfRangeException();

        if (index < Count - 1)
            Array.Copy(_polyItems, index + 1, _polyItems, index, Count - index - 1);

        Count--;
    }

    public PolynomItem this[int index]
    {
        get
        {
            if ((uint)index >= Count)
                throw new IndexOutOfRangeException();
            return _polyItems[index];
        }
        set
        {
            if ((uint)index >= Count)
                throw new IndexOutOfRangeException();
            _polyItems[index] = value;
        }
    }

    public int Count { get; private set; } = 0;

    public void Clear()
        => Count = 0;

    public Polynom Clone()
    {
        var newPolynom = new Polynom(Count);
        Array.Copy(_polyItems, newPolynom._polyItems, Count);
        newPolynom.Count = Count;
        return newPolynom;
    }

    public void Sort(Func<PolynomItem, PolynomItem, int> comparer)
    {
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));

        var items = _polyItems ?? throw new ObjectDisposedException(nameof(Polynom));

        if (Count <= 1)
            return;

        QuickSort(0, Count - 1);

        void QuickSort(int left, int right)
        {
            var i = left;
            var j = right;
            var pivot = items[(left + right) / 2];

            while (i <= j)
            {
                while (comparer(items[i], pivot) < 0) i++;
                while (comparer(items[j], pivot) > 0) j--;

                if (i > j) continue;

                (items[i], items[j]) = (items[j], items[i]);
                i++;
                j--;
            }

            // рекурсивная сортировка
            if (left < j)
                QuickSort(left, j);
            if (i < right)
                QuickSort(i, right);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var polyItem in _polyItems)
            sb.Append("a^" + polyItem.Coefficient + "*x^" + polyItem.Exponent + " + ");

        if (sb.Length > 0)
            sb.Length -= 3;

        return sb.ToString();
    }

    public void Dispose()
    {
        ReturnArray(_polyItems);
        _polyItems = null!;
    }

    private static PolynomItem[] RentArray(int count)
        => ArrayPool<PolynomItem>.Shared.Rent(count);

    private static void ReturnArray(PolynomItem[] array)
        => ArrayPool<PolynomItem>.Shared.Return(array);

    public PolynumEnumerator GetEnumerator()
        => new(this);

    public struct PolynumEnumerator(Polynom polynom)
    {
        private int index = -1;

        public PolynomItem Current => polynom[index];

        public bool MoveNext()
            => ++index < polynom.Count;
    }
}