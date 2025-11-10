namespace UnlimitedPDF.Models.Table;

public class PdfTableRow
{
    public int Index { get; internal set; }
    // dynamic per-row list of cells (length equals current number of columns in parent table)
    private List<PdfTableCell> _cells;

    public IReadOnlyList<PdfTableCell> Cells => _cells;

    public PdfTableRow(int index, int initialCols)
    {
        Index = index;
        _cells = new List<PdfTableCell>(initialCols);
        for (int i = 0; i < initialCols; i++) _cells.Add(new PdfTableCell());
    }

    internal void EnsureColumns(int cols)
    {
        while (_cells.Count < cols) _cells.Add(new PdfTableCell());
    }

    public PdfTableCell GetCell(int colIndex)
    {
        if (colIndex < 0 || colIndex >= _cells.Count) throw new ArgumentOutOfRangeException(nameof(colIndex));
        return _cells[colIndex];
    }

    public void SetCell(int colIndex, PdfTableCell cell)
    {
        if (colIndex < 0) throw new ArgumentOutOfRangeException(nameof(colIndex));
        EnsureColumns(colIndex + 1);
        _cells[colIndex] = cell ?? new PdfTableCell();
    }

    internal void InsertColumnAt(int colIndex)
    {
        if (colIndex < 0 || colIndex > _cells.Count) throw new ArgumentOutOfRangeException(nameof(colIndex));
        _cells.Insert(colIndex, new PdfTableCell());
    }

    internal void RemoveColumnAt(int colIndex)
    {
        if (colIndex < 0 || colIndex >= _cells.Count) throw new ArgumentOutOfRangeException(nameof(colIndex));
        _cells.RemoveAt(colIndex);
    }

    // Helper to clone row cells (used when converting to fixed table)
    internal PdfTableCell CloneCellAt(int col)
    {
        var src = GetCell(col);
        return CloneCell(src);
    }

    private static PdfTableCell CloneCell(PdfTableCell src)
    {
        var dst = new PdfTableCell
        {
            Text = src.Text,
            HAlign = src.HAlign,
            VAlign = src.VAlign,
            Background = src.Background,
            Padding = src.Padding,
            RowSpan = src.RowSpan,
            ColSpan = src.ColSpan
        };

        // clone borders (shallow)
        dst.TopBorder.Width = src.TopBorder.Width;
        dst.TopBorder.Color = src.TopBorder.Color;
        dst.TopBorder.Style = src.TopBorder.Style;

        dst.BottomBorder.Width = src.BottomBorder.Width;
        dst.BottomBorder.Color = src.BottomBorder.Color;
        dst.BottomBorder.Style = src.BottomBorder.Style;

        dst.LeftBorder.Width = src.LeftBorder.Width;
        dst.LeftBorder.Color = src.LeftBorder.Color;
        dst.LeftBorder.Style = src.LeftBorder.Style;

        dst.RightBorder.Width = src.RightBorder.Width;
        dst.RightBorder.Color = src.RightBorder.Color;
        dst.RightBorder.Style = src.RightBorder.Style;

        return dst;
    }
}
