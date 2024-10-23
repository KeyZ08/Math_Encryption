using System.Buffers;
using System.Collections;
using System.Drawing;
using System.Text;
using QRCode.Extensions;
using QRCode.Polynoms;
using QRCode.QR.Common;
using static QRCode.QR.Common.Common;

// ReSharper disable UseIndexFromEndExpression
// ReSharper disable InconsistentNaming

namespace QRCode.QR;

public class QRCodeGenerator
{
    private const int modeIndicatorLength = 16;
    private const int version = 10;
    private const int countIndicatorLength = 16;
    private const int capacity = 119;

    private static readonly ECCInfo EccInfo = new()
    {
        TotalDataCodewords = 122,
        ECCPerBlock = 28,
        BlocksInGroup1 = 6,
        CodewordsInGroup1 = 15,
        BlocksInGroup2 = 2,
        CodewordsInGroup2 = 16
    };

    private static readonly AlignmentPattern AlignmentPattern = CreateAlignmentPattern();

    private static readonly int[] _remainderBits =
    [
        0, 7, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0,
        0, 0, 0
    ];

    private static readonly BitArray repeatingPattern = new(new[]
        { true, true, true, false, true, true, false, false, false, false, false, true, false, false, false, true });

    public static QRCodeData GenerateQrCode(string plainText)
    {
        var codedText = PlainTextToBinaryByte(plainText);
        var dataInputLength = GetDataLength(codedText);

        if (capacity < dataInputLength + 2)
            throw new ArgumentException("Слишком много данных для QR кода");

        var completeBitArrayLength = modeIndicatorLength + countIndicatorLength + codedText.Length;

        var completeBitArray = new BitArray(completeBitArrayLength);

        //записываем вид кодирования
        var bitArrayOffset = IntToBin(4, 4, completeBitArray, 0); //num = 4 = 0100 = байтовый вид кодирования
        //записываем длину данных
        bitArrayOffset = IntToBin(dataInputLength, countIndicatorLength, completeBitArray, bitArrayOffset);
        // записываем данные
        for (var i = 0; i < codedText.Length; i++)
            completeBitArray[bitArrayOffset++] = codedText[i];

        return GenerateQrCode(completeBitArray);
    }

    private static QRCodeData GenerateQrCode(BitArray bitArray)
    {
        PadData();

        // считаем блоки коррекции
        var codeWordWithECC = CalculateECCBlocks();
        // Вычисляем длинну чередующихся кодовых слов
        var interleavedLength = CalculateInterleavedLength();
        // Чередуем кодовые слова
        var interleavedData = InterleaveData();

        // размещаем на qr коде
        var qrData = PlaceModules();

        return qrData;

        // заполняет битовый массив повторяющимся паттерном для достижения требуемой длины
        void PadData()
        {
            var dataLength = EccInfo.TotalDataCodewords * 8;
            var lengthDiff = dataLength - bitArray.Length;
            if (lengthDiff <= 0) return;

            var index = bitArray.Length;
            bitArray.Length = dataLength;

            // добиваем до кратности 8-ми
            if (index % 8 != 0)
                index += 8 - index % 8;

            // дозаполняем повторяющимся паттерном
            var repeatingPatternIndex = 0;
            while (index < dataLength)
            {
                bitArray[index++] = repeatingPattern[repeatingPatternIndex++];
                if (repeatingPatternIndex >= repeatingPattern.Length)
                    repeatingPatternIndex = 0;
            }
        }

        List<CodewordBlock> CalculateECCBlocks()
        {
            var codewordBlocks = new List<CodewordBlock>(EccInfo.BlocksInGroup1 + EccInfo.BlocksInGroup2);
            using (var generatorPolynom = CalculateGeneratorPolynom(EccInfo.ECCPerBlock))
            {
                AddCodeWordBlocks(EccInfo.BlocksInGroup1, EccInfo.CodewordsInGroup1, 0, generatorPolynom);
                var offset = EccInfo.BlocksInGroup1 * EccInfo.CodewordsInGroup1 * 8;
                AddCodeWordBlocks(EccInfo.BlocksInGroup2, EccInfo.CodewordsInGroup2, offset, generatorPolynom);
                return codewordBlocks;
            }

            void AddCodeWordBlocks(int blocksInGroup, int codewordsInGroup, int offset2, Polynom generatorPolynom)
            {
                var groupLength = codewordsInGroup * 8;
                for (var i = 0; i < blocksInGroup; i++)
                {
                    var eccWordList = CalculateECCWords(bitArray, offset2, groupLength, generatorPolynom);
                    codewordBlocks.Add(new CodewordBlock(offset2, groupLength, eccWordList));
                    offset2 += groupLength;
                }
            }
        }

        int CalculateInterleavedLength()
        {
            var length = 0;
            for (var i = 0; i < Math.Max(EccInfo.CodewordsInGroup1, EccInfo.CodewordsInGroup2); i++)
                length += codeWordWithECC.Count(codeBlock => codeBlock.CodeWordsLength / 8 > i) * 8;
            for (var i = 0; i < EccInfo.ECCPerBlock; i++)
                length += codeWordWithECC.Count(codeBlock => codeBlock.ECCWords.Length > i) * 8;
            length += _remainderBits[version - 1];
            return length;
        }

        BitArray InterleaveData()
        {
            var data = new BitArray(interleavedLength);
            var pos = 0;
            for (var i = 0; i < Math.Max(EccInfo.CodewordsInGroup1, EccInfo.CodewordsInGroup2); i++)
                foreach (var codeBlock in codeWordWithECC)
                    if (codeBlock.CodeWordsLength / 8 > i)
                        pos = bitArray.CopyTo(data, i * 8 + codeBlock.CodeWordsOffset, pos, 8);
            for (var i = 0; i < EccInfo.ECCPerBlock; i++)
                foreach (var codeBlock in codeWordWithECC)
                    if (codeBlock.ECCWords.Length > i)
                        pos = IntToBin(codeBlock.ECCWords[i], 8, data, pos);

            return data;
        }

        QRCodeData PlaceModules()
        {
            var qr = new QRCodeData();
            var size = qr.ModuleMatrix.Count - 8;
            var tempBitArray = new BitArray(18); //информация о версии занимает 18 бит
            using (var blockedModules = new ModulePlacer.BlockedModules(size))
            {
                ModulePlacer.PlaceFinderPatterns(qr, blockedModules);
                ModulePlacer.ReserveSeperatorAreas(size, blockedModules);
                ModulePlacer.PlaceAlignmentPatterns(qr, AlignmentPattern.PatternPositions, blockedModules);
                ModulePlacer.PlaceTimingPatterns(qr, blockedModules);
                ModulePlacer.PlaceDarkModule(qr, blockedModules);
                ModulePlacer.ReserveVersionAreas(size, blockedModules);
                ModulePlacer.PlaceDataWords(qr, interleavedData, blockedModules);
                var maskVersion = ModulePlacer.MaskCode(qr, version, blockedModules);
                GetFormatString(tempBitArray, maskVersion);
                ModulePlacer.PlaceFormat(qr, tempBitArray, true);
            }

            GetVersionString(tempBitArray, version);
            ModulePlacer.PlaceVersion(qr, tempBitArray, true);

            return qr;
        }
    }

    /// Вычисляет кодовые слова для исправления ошибок (ECC) для сегмента данных, используя предоставленную информацию ECC.
    private static byte[] CalculateECCWords(BitArray bitArray, int offset, int count, Polynom generatorPolynomBase)
    {
        var messagePolynom = CalculateMessagePolynom(bitArray, offset, count);
        var generatorPolynom = generatorPolynomBase.Clone();

        // Плюсуем к экспоненте чтобы полином расширился и мы могли поместить избыточные кодовые слова
        foreach (var t in messagePolynom)
            t.Exponent += EccInfo.ECCPerBlock;
        foreach (var t in generatorPolynom)
            t.Exponent += messagePolynom.Count - 1;

        // Делим многочлен сообщения на многочлен генератора, чтобы найти остаток.
        for (var i = 0; messagePolynom.Count > 0 && messagePolynom[messagePolynom.Count - 1].Exponent > 0; i++)
            if (messagePolynom[0].Coefficient == 0)
            {
                messagePolynom.RemoveAt(0);
                messagePolynom.Add(new PolynomItem(0, messagePolynom[messagePolynom.Count - 1].Exponent - 1));
            }
            else
            {
                var index0Coefficient = messagePolynom[0].Coefficient;
                index0Coefficient = index0Coefficient == 0 ? 0 : Galois.GetAlphaExpFromIntVal(index0Coefficient);
                var alphaNotation = new PolynomItem(index0Coefficient, messagePolynom[0].Exponent);

                var resPoly = MultiplyGeneratorPolynomByLeadterm(generatorPolynom, alphaNotation, i);
                ConvertToDecNotationInPlace(resPoly);
                var newPoly = XORPolynoms(messagePolynom, resPoly);

                resPoly.Dispose();
                messagePolynom.Dispose();

                messagePolynom = newPoly;
            }
        // в результате messagePolynom содержит в себе искомый остаток

        generatorPolynom.Dispose();

        // Преобразуем полученный полином в массив байтов, представляющий кодовые слова ECC.
        var ret = new byte[messagePolynom.Count];
        for (var i = 0; i < messagePolynom.Count; i++)
            ret[i] = (byte)messagePolynom[i].Coefficient;

        messagePolynom.Dispose();

        return ret;
    }

    /// <summary>
    ///     Преобразует все полиномиальные элементарные коэффициенты из их системы счисления в десятичное представление.
    /// </summary>
    private static void ConvertToDecNotationInPlace(Polynom poly)
    {
        for (var i = 0; i < poly.Count; i++)
            poly[i] = new PolynomItem(Galois.GetIntValFromAlphaExp(poly[i].Coefficient), poly[i].Exponent);
    }

    /// Вычисляет многочлен сообщения из массива битов, который представляет закодированные данные.
    private static Polynom CalculateMessagePolynom(BitArray bitArray, int offset, int count)
    {
        var messagePol = new Polynom(count /= 8);
        for (var i = count - 1; i >= 0; i--)
        {
            messagePol.Add(new PolynomItem(BinToDec(bitArray, offset, 8), i));
            offset += 8;
        }

        return messagePol;
    }

    private static Polynom CalculateGeneratorPolynom(int numEccWords) // numEccWords = количество избыточных символов
    {
        var generatorPolynom = new Polynom(2);
        generatorPolynom.Add(new PolynomItem(0, 1));
        generatorPolynom.Add(new PolynomItem(0, 0));

        using var multiplierPolynom = new Polynom(numEccWords * 2);
        for (var i = 1; i <= numEccWords - 1; i++)
        {
            multiplierPolynom.Clear();
            multiplierPolynom.Add(new PolynomItem(0, 1));
            multiplierPolynom.Add(new PolynomItem(i, 0));

            var newGeneratorPolynom = MultiplyAlphaPolynoms(generatorPolynom, multiplierPolynom);
            generatorPolynom.Dispose();
            generatorPolynom = newGeneratorPolynom;
        }

        return generatorPolynom;
    }

    private static int GetDataLength(BitArray codedText)
        => codedText.Length / 8;

    private static BitArray PlainTextToBinaryByte(string plainText)
    {
        var targetEncoding = Encoding.UTF8;

        var count = targetEncoding.GetByteCount(plainText);
        var codeBytes = new byte[count];
        targetEncoding.GetBytes(plainText, codeBytes);
        var bitArray = ToBitArray(codeBytes);

        return bitArray;
    }

    private static BitArray ToBitArray(
        ReadOnlySpan<byte> byteArray,
        int prefixZeros = 0)
    {
        var bitArray = new BitArray(byteArray.Length * 8 + prefixZeros);
        for (var i = 0; i < byteArray.Length; i++)
        {
            var byteVal = byteArray[i];
            for (var j = 0; j < 8; j++)
                bitArray[i * 8 + j + prefixZeros] = (byteVal & (1 << (7 - j))) != 0;
        }

        return bitArray;
    }

    private static Polynom XORPolynoms(Polynom messagePolynom, Polynom resPolynom)
    {
        Polynom longPoly, shortPoly;
        if (messagePolynom.Count >= resPolynom.Count)
        {
            longPoly = messagePolynom;
            shortPoly = resPolynom;
        }
        else
        {
            longPoly = resPolynom;
            shortPoly = messagePolynom;
        }

        var resultPolynom = new Polynom(longPoly.Count - 1);
        for (var i = 1; i < longPoly.Count; i++)
        {
            var polItemRes = new PolynomItem(
                longPoly[i].Coefficient ^ (shortPoly.Count > i ? shortPoly[i].Coefficient : 0),
                messagePolynom[0].Exponent - i
            );
            resultPolynom.Add(polItemRes);
        }

        return resultPolynom;
    }

    /// <summary>
    ///     Умножает многочлен генератора на многочлен старшего члена, уменьшая результат на заданный показатель
    ///     степени, используемый при построении кодовых слов для исправления ошибок QR-кода.
    /// </summary>
    private static Polynom MultiplyGeneratorPolynomByLeadterm(Polynom genPolynom, PolynomItem leadTerm,
        int lowerExponentBy)
    {
        var resultPolynom = new Polynom(genPolynom.Count);
        foreach (var polItemBase in genPolynom)
        {
            var polItemRes = new PolynomItem(
                (polItemBase.Coefficient + leadTerm.Coefficient) % 255,
                polItemBase.Exponent - lowerExponentBy
            );
            resultPolynom.Add(polItemRes);
        }

        return resultPolynom;
    }

    /// перемножает два полинома
    private static Polynom MultiplyAlphaPolynoms(Polynom polynomBase, Polynom polynomMultiplier)
    {
        var resultPolynom = new Polynom(polynomMultiplier.Count * polynomBase.Count);

        foreach (var polItemBase in polynomMultiplier)
        foreach (var polItemMulti in polynomBase)
        {
            var polItemRes = new PolynomItem
            (
                Galois.ShrinkAlphaExp(polItemBase.Coefficient + polItemMulti.Coefficient),
                polItemBase.Exponent + polItemMulti.Exponent
            );
            resultPolynom.Add(polItemRes);
        }

        // складываем полиномы с одинаковыми степенями
        var toGlue = GetNotUniqueExponents(resultPolynom);
        foreach (var exponent in toGlue)
        {
            var coefficient = 0;
            for (var i = 0; i < resultPolynom.Count; i++)
            {
                var polynomOld = resultPolynom[i];
                if (polynomOld.Exponent == exponent)
                {
                    coefficient ^= Galois.GetIntValFromAlphaExp(polynomOld.Coefficient);
                    resultPolynom.RemoveAt(i); //удаляем 
                    i--;
                }
            }

            resultPolynom.Add(new PolynomItem(Galois.GetAlphaExpFromIntVal(coefficient), exponent));
        }

        // Сортируем члены полинома
        resultPolynom.Sort((x, y) => -x.Exponent.CompareTo(y.Exponent));
        return resultPolynom;

        int[] GetNotUniqueExponents(Polynom list)
        {
            var dic = new Dictionary<int, bool>(list.Count);
            foreach (var row in list)
                if (!dic.TryAdd(row.Exponent, false))
                    dic[row.Exponent] = true;

            return dic.Where(x => x.Value).Select(x => x.Key).ToArray();
        }
    }

    private static AlignmentPattern CreateAlignmentPattern()
    {
        var intersections = new[] { 4, 26, 48 };
        var points = new List<Point>(intersections.Length * intersections.Length);
        foreach (var x in intersections)
        foreach (var y in intersections)
            points.Add(new Point(x, y));

        return new AlignmentPattern(points);
    }
}