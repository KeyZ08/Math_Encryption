namespace QRCode.Polynoms;

public record PolynomItem(int Coefficient, int Exponent)
{
    public int Exponent { get; set; } = Exponent;
}