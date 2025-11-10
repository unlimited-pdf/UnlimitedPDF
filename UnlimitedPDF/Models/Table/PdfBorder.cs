namespace UnlimitedPDF.Models.Table;

public class PdfBorder
{
    public double Width { get; set; } = 0.5;
    public PdfColor Color { get; set; } = PdfColor.Black;
    public PdfBorderStyle Style { get; set; } = PdfBorderStyle.Solid;
}
