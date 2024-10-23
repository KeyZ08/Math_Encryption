using System.Collections;

// ReSharper disable InvalidXmlDocComment

namespace QRCode.QR.Common;

public static class Common
{
    /// <summary>
    ///     Генерим bitarray содержащий уровень исправления ошибок, версию маски и уровня исправления ошибок
    /// </summary>
    public static void GetFormatString(BitArray fStrEcc, int maskVersion)
    {
        BitArray getFormatGenerator =
            new(new[] { true, false, true, false, false, true, true, false, true, true, true });

        BitArray getFormatMask = new(new[]
            { true, false, true, false, true, false, false, false, false, false, true, false, false, true, false });

        fStrEcc.Length = 15;
        fStrEcc.SetAll(false);
        WriteEccLevelAndVersion();

        var index = 0;
        var count = 15;
        TrimLeadingZeros(fStrEcc, ref index, ref count);
        while (count > 10)
        {
            for (var i = 0; i < getFormatGenerator.Length; i++)
                fStrEcc[index + i] ^= getFormatGenerator[i];
            TrimLeadingZeros(fStrEcc, ref index, ref count);
        }

        fStrEcc.RightShift(index);

        // Перед битами исправления ошибок пишем уровень коррекции и версию маски
        fStrEcc.Length = 10 + 5;
        fStrEcc.LeftShift(10 - count + 5);
        WriteEccLevelAndVersion();

        fStrEcc.Xor(getFormatMask);

        void WriteEccLevelAndVersion()
        {
            fStrEcc[0] = true;
            // вставляем версию маски после битов уровня исправления ошибок
            IntToBin(maskVersion, 3, fStrEcc, 2);
        }
    }

    /// <summary>
    ///     Побитово записывает число в массив бит.
    /// </summary>
    /// <param name="bits">Количество бит, использующееся для записи числа.</param>
    /// <param name="index">Индекс массива, откуда начать запись.</param>
    /// <returns>Индекс последнего записаного бита</returns>
    public static int IntToBin(int num, int bits, BitArray bitList, int index)
    {
        for (var i = bits - 1; i >= 0; i--)
        {
            var bit = (num & (1 << i)) != 0;
            bitList[index++] = bit;
        }

        return index;
    }

    public static void GetVersionString(BitArray vStr, int version)
    {
        BitArray getVersionGenerator = new(new[]
            { true, true, true, true, true, false, false, true, false, false, true, false, true });

        const int length = 18;
        vStr.Length = length;
        vStr.SetAll(false);
        IntToBin(version, 6, vStr, 0);

        var count = length;
        var index = 0;
        TrimLeadingZeros(vStr, ref index, ref count); // затираем начальные нули

        // кодирование с исправлением ошибок с помощью генератора полиномов (_getVersionGenerator).
        while (count > 12) // версия должна занимать 12 бит
        {
            for (var i = 0; i < getVersionGenerator.Length; i++)
                vStr[index + i] ^= getVersionGenerator[i];

            TrimLeadingZeros(vStr, ref index,
                ref count); // затираем начальные нули чтобы сохранить правильную последовательность
        }

        vStr.RightShift(index); // сдвигаем массив битов, чтобы данные начинались с 0-ля.

        // для исправления ошибок добавляем 6 бит
        vStr.Length = 12 + 6;
        vStr.LeftShift(12 - count + 6);
        IntToBin(version, 6, vStr, 0);
    }

    /// <summary>
    ///     Конвертирует сегмент массива бит в число int
    /// </summary>
    public static int BinToDec(BitArray bitArray, int offset, int count)
    {
        var ret = 0;
        for (var i = 0; i < count; i++) ret ^= bitArray[offset + i] ? 1 << (count - i - 1) : 0;
        return ret;
    }

    private static void TrimLeadingZeros(BitArray fStrEcc, ref int index, ref int count)
    {
        while (count > 0 && !fStrEcc[index])
        {
            index++;
            count--;
        }
    }
}