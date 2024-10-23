using System.Collections;

// ReSharper disable InconsistentNaming

namespace QRCode.QR;

public class QRCodeData
{
    public QRCodeData()
    {
        const int padding = 8;
        const int size = 21 + 9 * 4 + padding;
        ModuleMatrix = new List<BitArray>(size);
        for (var i = 0; i < size; i++)
            ModuleMatrix.Add(new BitArray(size));
    }

    public List<BitArray> ModuleMatrix { get; }
}