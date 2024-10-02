namespace Архиваторы;

// ReSharper disable once InconsistentNaming
public static class LZW
{
    public static void Compress(Stream inputStream, Stream outputStream)
    {
        var dictionary = new Dictionary<List<byte>, int>(new ArrayComparer());
        for (var i = 0; i < 256; i++)
            dictionary.Add([(byte)i], i);

        var writer = new NBitStream(outputStream, 9);

        var current = new List<byte>();
        while (inputStream.ReadByte() is { } @byte && @byte != -1)
        {
            var windowChain = new List<byte>(current) { (byte)@byte };
            if (dictionary.ContainsKey(windowChain))
            {
                current.Clear();
                current.AddRange(windowChain);
            }
            else
            {
                if (dictionary.TryGetValue(current, out var value))
                    writer.Write(value);
                else
                    throw new Exception("Error Encoding.");

                SetBitsCountByNum(dictionary.Count - 1, writer);
                dictionary.Add(windowChain, dictionary.Count);

                current.Clear();
                current.Add((byte)@byte);
            }
        }

        if (current.Count != 0)
            writer.Write(dictionary[current]);

        writer.EndWrite();
    }

    public static void Decompress(Stream inputStream, Stream outputStream)
    {
        var reader = new NBitStream(inputStream, 9);
        var firstByte = reader.Read();
        if (firstByte == -1) throw new Exception($"Пустой {nameof(inputStream)}");
        
        var dictionary = new Dictionary<long, List<byte>>(1024);
        for (var i = 0; i < 256; i++)
            dictionary.Add(i, [(byte)i]);

        var window = dictionary[firstByte];
        outputStream.Write(window.ToArray());

        while (reader.Read() is { } position && position != -1)
        {
            var entry = new List<byte>();
            if (dictionary.TryGetValue(position, out var value))
            {
                entry.AddRange(value);
            }
            else
            {
                entry.AddRange(window);
                entry.Add(window[0]);
            }

            outputStream.Write(entry.ToArray());
            SetBitsCountByNum(dictionary.Count, reader);
            dictionary.Add(dictionary.Count, new List<byte>(window) { entry[0] });

            window = entry;
        }
    }

    private static void SetBitsCountByNum(long num, NBitStream stream)
    {
        var bitsInChunk = GetBitsCount(num);
        if (bitsInChunk > stream.BitsInChunk)
            stream.SetBitsInChunk(bitsInChunk);
    }

    private static int GetBitsCount(long number)
    {
        var count = 0;
        while (number != 0)
        {
            number >>= 1;
            count++;
        }

        return count;
    }

    private class ArrayComparer : IEqualityComparer<List<byte>>
    {
        public bool Equals(List<byte>? left, List<byte>? right)
        {
            if (left == null || right == null)
                return false;
            return left.SequenceEqual(right);
        }

        public unsafe int GetHashCode(List<byte> obj)
        {
            var obj1 = obj.ToArray();
            var cbSize = obj1.Length;
            var hash = 0x811C9DC5;
            fixed (byte* pb = obj1)
            {
                var nb = pb;
                while (cbSize >= 4)
                {
                    hash ^= *(uint*)nb;
                    hash *= 0x1000193;
                    nb += 4;
                    cbSize -= 4;
                }

                switch (cbSize & 3)
                {
                    case 3:
                        hash ^= *(uint*)(nb + 2);
                        hash *= 0x1000193;
                        goto case 2;
                    case 2:
                        hash ^= *(uint*)(nb + 1);
                        hash *= 0x1000193;
                        goto case 1;
                    case 1:
                        hash ^= *nb;
                        hash *= 0x1000193;
                        break;
                }
            }

            return (int)hash;
        }
    }
}