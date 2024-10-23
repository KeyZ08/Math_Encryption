// ReSharper disable InconsistentNaming

namespace QRCode.QR.Common;

/// <param name="CodeWordsOffset">Смещение кодовых слов данных в основном массиве битовых данных.</param>
/// <param name="CodeWordsLength">Длина кодовых слов в битах в основном массиве битов.</param>
/// <param name="ECCWords">Массив кодовых слов для исправления ошибок в этом блоке.</param>
public record CodewordBlock(int CodeWordsOffset, int CodeWordsLength, byte[] ECCWords);