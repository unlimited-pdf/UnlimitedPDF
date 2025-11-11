namespace UnlimitedPDF.Models.Table;

public class PdfTableColumn
{
    public int Index { get; internal set; }

    /// <summary>
    /// Width in points for the column. 
    /// Null means 'auto' and will be distributed.
    /// </summary>
    public double? Width { get; set; } = null;

    public PdfTableColumn(int index) { Index = index; }
}
