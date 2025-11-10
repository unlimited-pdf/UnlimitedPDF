namespace UnlimitedPDF.Models.Table;

public class PdfTableCell
{
    public string Text { get; set; } = "";
    public PdfCellHorizontalAlignment HAlign { get; set; } = PdfCellHorizontalAlignment.Left;
    public PdfCellVerticalAlignment VAlign { get; set; } = PdfCellVerticalAlignment.Middle;
    public PdfColor Background { get; set; } = PdfColor.White;
    public double Padding { get; set; } = 4;

    // Spanning
    public int RowSpan { get; set; } = 1;
    public int ColSpan { get; set; } = 1;

    // Per-side border
    public PdfBorder TopBorder { get; } = new PdfBorder();
    public PdfBorder BottomBorder { get; } = new PdfBorder();
    public PdfBorder LeftBorder { get; } = new PdfBorder();
    public PdfBorder RightBorder { get; } = new PdfBorder();
}
