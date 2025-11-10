namespace UnlimitedPDF.Models.Table;

public record PdfColor(double R, double G, double B)
{
    public static PdfColor White => new(1, 1, 1);
    public static PdfColor Black => new(0, 0, 0);
    public static PdfColor LightGray => new(0.9, 0.9, 0.9);
}
