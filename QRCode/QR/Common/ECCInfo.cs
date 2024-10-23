// ReSharper disable InconsistentNaming

namespace QRCode.QR.Common;

public record struct ECCInfo(
    int TotalDataCodewords,
    int ECCPerBlock,
    int BlocksInGroup1,
    int CodewordsInGroup1,
    int BlocksInGroup2,
    int CodewordsInGroup2);