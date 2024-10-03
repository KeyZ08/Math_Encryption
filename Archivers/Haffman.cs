using System.Collections;
using System.Text;

namespace Archivers;

// ReSharper disable once IdentifierTypo
// ReSharper disable once ClassNeverInstantiated.Global
public class Haffman
{
    private static bool IsLeaf(Node node)
    {
        return node.Left is null && node.Right is null;
    }

    public class Node
    {
        public char Symbol { get; init; }
        public long Frequency { get; init; }
        public Node? Right { get; init; }
        public Node? Left { get; init; }

        //рекурсивно достаем биты, означающие символ
        public List<bool>? Traverse(char symbol, List<bool> data)
        {
            if (IsLeaf(this))
                return symbol.Equals(Symbol) ? data : null;

            List<bool>? left = null;
            List<bool>? right = null;

            if (Left is not null)
            {
                var leftPath = new List<bool>();
                leftPath.AddRange(data);
                leftPath.Add(false);

                left = Left.Traverse(symbol, leftPath);
            }

            if (Right is not null)
            {
                var rightPath = new List<bool>();
                rightPath.AddRange(data);
                rightPath.Add(true);
                right = Right.Traverse(symbol, rightPath);
            }

            return left ?? right;
        }
    }

    public class HuffmanTree
    {
        private Node Root { get; set; } = default!;

        public void Build(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException();

            Dictionary<char, int> frequencyByChar = new();

            //находим частоты
            for (var i = 0; i < source.Length; i++)
                if (frequencyByChar.ContainsKey(source[i]))
                    frequencyByChar[source[i]]++;
                else
                    frequencyByChar.Add(source[i], 0);

            //создаем листья
            var nodes = frequencyByChar
                .Select(frequency => new Node { Symbol = frequency.Key, Frequency = frequency.Value })
                .ToList();

            while (nodes.Count > 1)
            {
                //сортируем по убыванию частоты
                nodes.Sort((node1, node2) => (int)(node1.Frequency - node2.Frequency));

                //берем 2 самые малые частоты и соединяем их в ноду
                var taken = nodes.TakeLast(2).ToArray();
                var parent = new Node
                {
                    Symbol = '*',
                    Frequency = taken[0].Frequency + taken[1].Frequency,
                    Left = taken[0],
                    Right = taken[1]
                };

                nodes.RemoveRange(nodes.Count - 2, 2);
                nodes.Add(parent);
            }

            Root = nodes.FirstOrDefault(); //корень древа
        }

        public BitArray Encode(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException();

            var encodedSource = new List<bool>();

            for (var i = 0; i < source.Length; i++)
            {
                var encodedSymbol = Root.Traverse(source[i], new List<bool>(0));
                encodedSource.AddRange(encodedSymbol);
            }

            var bits = new BitArray(encodedSource.ToArray());
            return bits;
        }

        public string Decode(BitArray bits)
        {
            var current = Root;
            var decoded = new StringBuilder();

            foreach (bool bit in bits)
            {
                if (IsLeaf(current ?? throw new InvalidOperationException()))
                {
                    decoded.Append(current.Symbol);
                    current = Root;
                }

                current = bit ? current.Right : current.Left;
            }

            return decoded.ToString();
        }
    }
}