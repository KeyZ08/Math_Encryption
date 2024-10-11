using System.Drawing.Imaging;
using Barcode;

//"460700952001"
//"871125300120"
//"590123412345"
var input = "460700952001".Select(x => int.Parse(x.ToString())).ToList();
input.Add(EAN13Codes.GetCheckSum(input));

var barcode = Printer.PrintBarcodeEAN13(input);
barcode.Save($"../../../Barcodes/{string.Join(string.Empty, input)}.png", ImageFormat.Png);