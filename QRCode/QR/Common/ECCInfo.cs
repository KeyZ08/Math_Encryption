// ReSharper disable InconsistentNaming

namespace QRCode.QR.Common;

// блоки = наборы кодовых слов
// кодовые слова = байты
public record struct ECCInfo(
    int TotalDataCodewords, //общее количество кодовых слов 
    int ECCPerBlock, // количество битов на блок для исправления ошибок
    int BlocksInGroup1, //Количество блоков во первой группе
    int CodewordsInGroup1, //Количество слов данных в первой группе
    int BlocksInGroup2, //Количество блоков во второй группе
    int CodewordsInGroup2); //Количество слов данных во второй группе