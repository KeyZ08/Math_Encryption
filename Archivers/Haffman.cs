using System.Collections;

namespace Archivers;

// ReSharper disable once IdentifierTypo
// ReSharper disable once ClassNeverInstantiated.Global
public class Haffman
{
    private static bool IsLeaf(Node node)
    {
        return node.Left is null && node.Right is null;
    }

    private class Node
    {
        public byte Symbol { get; init; }
        public long Frequency { get; init; }
        public Node? Right { get; init; }
        public Node? Left { get; init; }

        //рекурсивно достаем биты, означающие символ
        public List<bool>? Traverse(byte symbol, List<bool> data)
        {
            if (IsLeaf(this))
                return symbol.Equals(Symbol) ? data : null;

            List<bool>? left = null;
            List<bool>? right = null;

            if (Left is not null)
                left = Left.Traverse(symbol, new List<bool>(data) { false });

            if (Right is not null)
                right = Right.Traverse(symbol, new List<bool>(data) { true });

            return left ?? right;
        }
    }

    public class HuffmanTree
    {
        private Dictionary<byte, List<bool>> encryptionTable;
        private Node Root { get; set; } = default!;

        public void Build(Stream source)
        {
            var frequencyByChar = new Dictionary<byte, int>();
            //находим частоты
            while (source.ReadByte() is { } @byte && @byte != -1)
            {
                var ch = (byte)@byte;
                if (frequencyByChar.ContainsKey(ch))
                    frequencyByChar[ch]++;
                else
                    frequencyByChar.Add(ch, 0);
            }

            //создаем листья
            var nodes = frequencyByChar
                .Select(frequency => new Node { Symbol = frequency.Key, Frequency = frequency.Value })
                .ToList();

            while (nodes.Count > 1)
            {
                //сортируем по убыванию частоты
                nodes.Sort((node1, node2) => (int)(node2.Frequency - node1.Frequency));

                //берем 2 самые малые частоты и соединяем их в ноду
                var taken = nodes.TakeLast(2).ToArray();
                var parent = new Node
                {
                    Symbol = 0,
                    Frequency = taken[0].Frequency + taken[1].Frequency,
                    Left = taken[0],
                    Right = taken[1]
                };

                nodes.RemoveRange(nodes.Count - 2, 2);
                nodes.Add(parent);
            }

            Root = nodes.FirstOrDefault(); //корень древа

            encryptionTable = new Dictionary<byte, List<bool>>(frequencyByChar.Keys.Count);
            foreach (var key in frequencyByChar.Keys)
                encryptionTable.Add(key, Root.Traverse(key, new List<bool>(0)));
        }

        public BitArray Encode(Stream input)
        {
            var encodedSource = new List<bool>();

            while (input.ReadByte() is { } @byte && @byte != -1)
            {
                var encodedSymbol = encryptionTable[(byte)@byte];
                encodedSource.AddRange(encodedSymbol);
            }

            var bits = new BitArray(encodedSource.ToArray());
            return bits;
        }

        public void Decode(BitArray bits, Stream output)
        {
            var current = Root;

            foreach (bool bit in bits)
            {
                if (IsLeaf(current ?? throw new InvalidOperationException()))
                {
                    output.WriteByte(current.Symbol);
                    current = Root;
                }

                current = bit ? current.Right : current.Left;
            }

            output.WriteByte(current.Symbol);
        }
    }
}