// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace Кодирование;

public static class NumericSystemConverter
{
    public enum Floating
    {
        Float,
        Double
    }

    public static string Recalculate(int sysFrom, int sysTo, long original)
    {
        if (sysFrom == sysTo)
            return original.ToString();

        original = RecalculateTo10(sysFrom, original);
        Console.WriteLine(Environment.NewLine);

        return sysTo != 10 ? RecalculateFrom10(sysTo, original) : original.ToString();
    }

    public static long RecalculateTo10(int sysFrom, long original)
    {
        if (sysFrom == 10)
            return original;

        Console.WriteLine($@"Перевод из {sysFrom}-ой в {10}-ную систему числа {original}.");
        var nums = original.ToString().Reverse().Select(x => x.ToString()).ToList();

        long sum = 0;
        for (var i = 0; i < nums.Count; i++)
        {
            var s = int.Parse(nums[i]) * (long)Math.Pow(sysFrom, i);
            sum += s;
            Console.WriteLine($@"{nums[i]} * {sysFrom}^{i} = {s}");
        }

        Console.WriteLine($@"{original}({sysFrom}) = {sum}({10})");
        return sum;
    }

    public static string RecalculateFrom10(int sysTo, long original)
    {
        var cacheOriginal = original;
        if (sysTo == 10 || original == 0)
            return original.ToString();

        Console.WriteLine($@"Перевод из {10}-ой в {sysTo}-ную систему числа {original}.");

        var remaindersOfDivision = new List<int>();
        while (original != 0)
        {
            Console.Write($@"{original} % {sysTo} = ");
            var remainder = original % sysTo;
            original /= sysTo;
            remaindersOfDivision.Add((int)remainder);
            Console.Write($@"{remainder}{Environment.NewLine}");
        }

        remaindersOfDivision.Reverse();
        var result = string.Join(string.Empty, remaindersOfDivision);

        Console.WriteLine($@"{cacheOriginal}({10}) = {result}({sysTo})");
        return result;
    }

    public static string CalculateAdditionalCode(int sys, string original)
    {
        var maxNum = sys - 1;
        var nums = original
            .Select(x => int.Parse(x.ToString()))
            .ToList();

        var count = nums.Count;
        for (var i = 0; i < count; i++)
            nums[i] = maxNum - nums[i];

        for (var i = count - 1; i >= 0; i--)
        {
            if (nums[i] + 1 < sys)
            {
                nums[i] += 1;
                break;
            }

            nums[i] = 0;
        }

        return string.Join("", nums);
    }

    public static string CalculateFloatingNumber10To2(decimal original, Floating floating)
    {
        Console.WriteLine("Представление вещественного числа в памяти компьютера");

        var firstByte = original < 0 ? "1" : "0";
        original = Math.Abs(original);

        var slices = original.ToString().Split(',');

        var intPart = long.Parse(slices[0]);
        var intPartStr = RecalculateFrom10(2, intPart);

        var fractionalPart = original - long.Parse(slices[0]);
        var fractionalPartStr = RecalculateFractionalPart10To2(fractionalPart);

        Console.WriteLine($"Переведя число {original} в 2-ную систему получим {intPartStr}.{fractionalPartStr}");

        var mantissa = Normalize(intPartStr + fractionalPartStr, intPartStr.Length, out var degree);
        if (mantissa == "00") 
            degree = -GetExponentOffset(floating);
        mantissa = SetOrDeleteBytes(mantissa, GetMantissaBytesCount(floating), true);
        var exponentOffset = GetExponentOffset(degree, floating);

        Console.WriteLine($"Первый байт {firstByte}");
        Console.WriteLine(
            $"Степень получается {degree}, что в системе будет храниться как {exponentOffset} ({degree} + 127)");
        Console.WriteLine($"Мантисса будет храниться как {mantissa}");
        Console.WriteLine($"В результате получается {firstByte} {exponentOffset} {mantissa}");
        Console.WriteLine(Environment.NewLine);

        return firstByte + exponentOffset + mantissa;
    }

    private static string Normalize(string mantissa, int intPartLength, out int degree)
    {
        if (mantissa == "00")
        {
            degree = 0;
            return mantissa;
        }

        if (intPartLength == 1 && mantissa[0] == '0')
        {
            degree = mantissa.TakeWhile(x => x == '0').Count();
            mantissa = mantissa[(degree + 1)..];
            degree = -degree;
        }
        else
        {
            mantissa = mantissa[1..];
            degree = intPartLength - 1;
        }

        return mantissa;
    }

    private static string GetExponentOffset(int degree10, Floating floating)
    {
        var exponentBytesCount = GetExponentBytesCount(floating);
        var offset = GetExponentOffset(floating);

        var exponentOffset = RecalculateFrom10(2, degree10 + offset);
        return SetOrDeleteBytes(exponentOffset, exponentBytesCount, false);
    }

    private static int GetExponentOffset(Floating floating)
        => floating switch
        {
            Floating.Float => 127,
            Floating.Double => 1023,
            _ => throw new ArgumentOutOfRangeException(nameof(floating), floating, null)
        };

    private static int GetExponentBytesCount(Floating floating)
        => floating switch
        {
            Floating.Float => 8,
            Floating.Double => 11,
            _ => throw new ArgumentOutOfRangeException(nameof(floating), floating, null)
        };

    private static int GetMantissaBytesCount(Floating floating)
        => floating switch
        {
            Floating.Float => 23,
            Floating.Double => 52,
            _ => throw new ArgumentOutOfRangeException(nameof(floating), floating, null)
        };

    private static string RecalculateFractionalPart10To2(decimal number)
    {
        if (number == 0) return 0.ToString();
        const int maxLength = 100;
        var iterCounter = 0;

        var result = new List<int>();
        while (number != 0 && maxLength > iterCounter)
        {
            number *= 2;
            var intPart = (int)number;
            number -= intPart;
            result.Add(intPart);
            iterCounter++;
        }

        return string.Join("", result);
    }

    private static string SetOrDeleteBytes(string number, int targetLength, bool toEnd)
    {
        var needWriting = targetLength - number.Length;
        if (needWriting > 0)
        {
            if (toEnd)
                number += new string('0', needWriting);
            else
                number = new string('0', needWriting) + number;
        }
        else if (needWriting < 0)
        {
            var needRounding = number[targetLength] == '1';
            number = toEnd ? number[..targetLength] : number[-needWriting..];
            if (!needRounding) return number;
            
            var numArray = number.Select(x => x).ToArray();
            for (var i = targetLength - 1; i >= 0; i--)
            {
                if (numArray[i] == '0')
                {
                    numArray[i] = '1';
                    break;
                }
                numArray[i] = '0';
            }

            number = string.Join(string.Empty, numArray);
        }
        
        return number;
    }
}