using Архиваторы;

namespace Tests;

[TestFixture]
public class NBitStreamTests
{
    [TestCaseSource(nameof(NBitStreamCases))]
    public static void NBitStreamTest((long number, int bitesCount) input)
    {
        var n = input.number;
        var bitesCount = input.bitesCount;
        
        using var stream = new MemoryStream();
        var nWriteStream = new NBitStream(stream, bitesCount);
        nWriteStream.Write(n);
        nWriteStream.EndWrite();
        stream.Position = 0;
        
        var nReadStream = new NBitStream(stream, bitesCount);
        var result = nReadStream.Read();
        Assert.That(result, Is.EqualTo(n));
    }
    
    [TestCaseSource(nameof(NBitStreamCases))]
    public static void NBitStreamMultipleTest((long number, int bitesCount) input)
    {
        var n1 = input.number;
        var n2 = input.number;
        var bitesCount = input.bitesCount;
        
        using var stream = new MemoryStream();
        var nWriteStream = new NBitStream(stream, bitesCount);
        nWriteStream.Write(n1);
        nWriteStream.Write(n2);
        nWriteStream.EndWrite();
        stream.Position = 0;
        
        var nReadStream = new NBitStream(stream, bitesCount);
        var result1 = nReadStream.Read();
        var result2 = nReadStream.Read();
        Assert.That(result1, Is.EqualTo(n1));
        Assert.That(result2, Is.EqualTo(n2));
    }
    
    [Test]
    public static void NBitStreamMyTest()
    {
        var data = new List<(long number, int bitesCount)>();
        for (var i = 0; i < 20; i++)
        {
            var randomNum = Random.Shared.NextInt64();
            var minBitsCount = GetBitsCount(randomNum);
            var randomBitsCount = Random.Shared.Next(minBitsCount, 63);
            
            data.Add((randomNum, randomBitsCount));
        }
        
        using var stream = new MemoryStream();
        var nWriteStream = new NBitStream(stream);
        foreach (var example in data)
        {
            nWriteStream.SetBitsInChunk(example.bitesCount);
            nWriteStream.Write(example.number);
        }
        nWriteStream.EndWrite();
        stream.Position = 0;
        
        
        var nReadStream = new NBitStream(stream);
        foreach (var example in data)
        {
            nReadStream.SetBitsInChunk(example.bitesCount);
            var result = nReadStream.Read();
            Assert.That(result, Is.EqualTo(example.number));
        }
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

    private static IEnumerable<(long number, int bitesCount)> NBitStreamCases()
    {
        yield return (111, 8);
        yield return (255, 8);
        yield return (255, 10);
        yield return (255, 14);
        yield return (255, 16);
        yield return (255, 17);
        yield return (1, 8);
        yield return (0, 8);
        yield return (0, 17);

        yield return (256, 9);
        yield return (511, 9);
        yield return (550, 10);
        yield return (550, 11);
        yield return (550, 15);
        yield return (550, 16);
        yield return (550, 17);
    }
}