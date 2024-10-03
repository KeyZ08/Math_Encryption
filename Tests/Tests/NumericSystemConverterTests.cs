using static NumericSystemConverter.NumericSystemConverter;

namespace Tests;

[TestFixture]
public class NumericSystemConverterTests
{
    [TestCaseSource(nameof(From10To2Cases))]
    public void From_10_To_2((long num10, long num2) input)
    {
        Assert.That(RecalculateFrom10(2, input.num10), Is.EqualTo(input.num2.ToString()));
    }

    [TestCaseSource(nameof(From10To2Cases))]
    public void From_2_To_10((long num10, long num2) input)
    {
        Assert.That(RecalculateTo10(2, input.num2), Is.EqualTo(input.num10));
    }

    [TestCaseSource(nameof(From10To3Cases))]
    public void From_10_To_3((long num10, long num3) input)
    {
        Assert.That(RecalculateFrom10(3, input.num10), Is.EqualTo(input.num3.ToString()));
    }

    [TestCaseSource(nameof(From10To3Cases))]
    public void From_3_To_10((long num10, long num3) input)
    {
        Assert.That(RecalculateTo10(3, input.num3), Is.EqualTo(input.num10));
    }

    [TestCaseSource(nameof(From10To8Cases))]
    public void From_10_To_8((long num10, long num8) input)
    {
        Assert.That(RecalculateFrom10(8, input.num10), Is.EqualTo(input.num8.ToString()));
    }

    [TestCaseSource(nameof(From10To8Cases))]
    public void From_8_To_10((long num10, long num8) input)
    {
        Assert.That(RecalculateTo10(8, input.num8), Is.EqualTo(input.num10));
    }

    [TestCaseSource(nameof(From3To8Cases))]
    public void From_3_To_8((long num3, long num8) input)
    {
        Assert.That(Recalculate(3, 8, input.num3), Is.EqualTo(input.num8.ToString()));
    }

    [TestCaseSource(nameof(From3To8Cases))]
    public void From_8_To_3((long num3, long num8) input)
    {
        Assert.That(Recalculate(8, 3, input.num8), Is.EqualTo(input.num3.ToString()));
    }

    [TestCaseSource(nameof(AdditionalCodeCases))]
    public void Additional_Code((int sys, string numIn, string numOut) input)
    {
        Assert.That(CalculateAdditionalCode(input.sys, input.numIn), Is.EqualTo(input.numOut));
    }

    [TestCaseSource(nameof(CalculateFloatNumberCases))]
    public void Calculate_Float_Number((decimal num, string expected) input)
    {
        Assert.That(CalculateFloatingNumber10To2(input.num, Floating.Float), Is.EqualTo(input.expected));
    }

    [TestCaseSource(nameof(CalculateDoubleNumberCases))]
    public void Calculate_Double_Number((decimal num, string expected) input)
    {
        Assert.That(CalculateFloatingNumber10To2(input.num, Floating.Double), Is.EqualTo(input.expected));
    }

    private static IEnumerable<(long num10, long num2)> From10To2Cases()
    {
        yield return (5, 101);
        yield return (126, 1111110);
        yield return (99999, 11000011010011111);
        yield return (0, 0);
        yield return (11, 1011);
        yield return (375, 101110111);
    }

    private static IEnumerable<(long num10, long num3)> From10To3Cases()
    {
        yield return (5, 12);
        yield return (126, 11200);
        yield return (999999, 1212210202000);
        yield return (0, 0);
        yield return (11, 102);
        yield return (375, 111220);
    }

    private static IEnumerable<(long num10, long num8)> From10To8Cases()
    {
        yield return (5, 5);
        yield return (126, 176);
        yield return (999999, 3641077);
        yield return (0, 0);
        yield return (11, 13);
        yield return (375, 567);
    }

    private static IEnumerable<(long num3, long num8)> From3To8Cases()
    {
        yield return (12, 5);
        yield return (11200, 176);
        yield return (1212210202000, 3641077);
        yield return (0, 0);
        yield return (102, 13);
        yield return (111220, 567);
    }

    private static IEnumerable<(int sys, string numIn, string numOut)> AdditionalCodeCases()
    {
        yield return (2, "00000001", "11111111");
        yield return (2, "00000101", "11111011");
        yield return (2, "01111111", "10000001");
    }

    private static IEnumerable<(decimal num, string expected)> CalculateFloatNumberCases()
    {
        yield return (decimal.Parse("-4,25"), "11000000100010000000000000000000");
        yield return (decimal.Parse("4,25"), "01000000100010000000000000000000");
        yield return (decimal.Parse("0,75"), "00111111010000000000000000000000");
        yield return (decimal.Parse("-0,75"), "10111111010000000000000000000000");
        yield return (decimal.Parse("-15,3333"), "11000001011101010101010100110010");
        yield return (decimal.Parse("1,0"), "00111111100000000000000000000000");
        yield return (decimal.Parse("9999,0"), "01000110000111000011110000000000");
        yield return (decimal.Parse("0,0"), "00000000000000000000000000000000");
        yield return (decimal.Parse("-81,123"), "11000010101000100011111011111010");
    }

    private static IEnumerable<(decimal num, string expected)> CalculateDoubleNumberCases()
    {
        yield return (decimal.Parse("-4,25"), "1100000000010001000000000000000000000000000000000000000000000000");
        yield return (decimal.Parse("4,25"), "0100000000010001000000000000000000000000000000000000000000000000");
        yield return (decimal.Parse("0,75"), "0011111111101000000000000000000000000000000000000000000000000000");
        yield return (decimal.Parse("-0,75"), "1011111111101000000000000000000000000000000000000000000000000000");
        yield return (decimal.Parse("-15,3333"), "1100000000101110101010101010011001001100001011111000001101111011");
        yield return (decimal.Parse("1,0"), "0011111111110000000000000000000000000000000000000000000000000000");
        yield return (decimal.Parse("9999,0"), "0100000011000011100001111000000000000000000000000000000000000000");
        yield return (decimal.Parse("0,0"), "0000000000000000000000000000000000000000000000000000000000000000");
    }
}