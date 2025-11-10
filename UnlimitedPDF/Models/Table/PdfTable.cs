namespace UnlimitedPDF.Models.Table;

public record PdfColor(double R, double G, double B)
{
    public static PdfColor White => new(1, 1, 1);
    public static PdfColor Black => new(0, 0, 0);
    public static PdfColor LightGray => new(0.9, 0.9, 0.9);
}

public enum HAlign { Left, Center, Right }
public enum VAlign { Top, Middle, Bottom }
public enum BorderStyle { None, Solid, Dashed, Double }

public class BorderSide
{
    public double Width { get; set; } = 0.5;
    public PdfColor Color { get; set; } = PdfColor.Black;
    public BorderStyle Style { get; set; } = BorderStyle.Solid;
}

public class PdfTableCell
{
    public string Text { get; set; } = "";
    public HAlign HAlign { get; set; } = HAlign.Left;
    public VAlign VAlign { get; set; } = VAlign.Middle;
    public PdfColor Background { get; set; } = PdfColor.White;
    public double Padding { get; set; } = 4;

    // Spanning
    public int RowSpan { get; set; } = 1;
    public int ColSpan { get; set; } = 1;

    // Per-side border
    public BorderSide TopBorder { get; } = new BorderSide();
    public BorderSide BottomBorder { get; } = new BorderSide();
    public BorderSide LeftBorder { get; } = new BorderSide();
    public BorderSide RightBorder { get; } = new BorderSide();
}

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

public class PdfTable
{
    private readonly List<PdfTableRow> _rows = new();
    private readonly List<PdfTableColumn> _cols = new();

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double RowHeight { get; set; } = 20;

    public IReadOnlyList<PdfTableRow> Rows => _rows;
    public IReadOnlyList<PdfTableColumn> Columns => _cols;

    public int RowCount => _rows.Count;
    public int ColCount => _cols.Count;

    public void EnsureSize(int rows, int cols)
    {
        if (rows < 0 || cols < 0) throw new ArgumentOutOfRangeException();
        while (_cols.Count < cols) AddColumn();
        while (_rows.Count < rows) AddRow();
    }

    public PdfTableRow AddRow()
    {
        var row = new PdfTableRow(_rows.Count, ColCount);
        _rows.Add(row);
        return row;
    }

    public PdfTableRow InsertRow(int index)
    {
        if (index < 0 || index > _rows.Count) throw new ArgumentOutOfRangeException(nameof(index));
        var row = new PdfTableRow(index, ColCount);
        _rows.Insert(index, row);
        // fix indices
        for (int i = 0; i < _rows.Count; i++) _rows[i].Index = i;
        NormalizeSpans();
        return row;
    }

    public void RemoveRow(int index)
    {
        if (index < 0 || index >= _rows.Count) throw new ArgumentOutOfRangeException(nameof(index));
        _rows.RemoveAt(index);
        for (int i = 0; i < _rows.Count; i++) _rows[i].Index = i;
        NormalizeSpans();
    }

    public PdfTableColumn AddColumn()
    {
        var col = new PdfTableColumn(_cols.Count) { Width = null };
        _cols.Add(col);
        // ensure all rows extend
        foreach (var r in _rows) r.EnsureColumns(ColCount);
        return col;
    }

    public PdfTableColumn InsertColumn(int index)
    {
        if (index < 0 || index > _cols.Count) throw new ArgumentOutOfRangeException(nameof(index));
        var col = new PdfTableColumn(index) { Width = null };
        _cols.Insert(index, col);
        // fix indices
        for (int i = 0; i < _cols.Count; i++) _cols[i].Index = i;
        // ensure rows update their cell lists
        foreach (var r in _rows) r.InsertColumnAt(index);
        NormalizeSpans();
        return col;
    }

    public void RemoveColumn(int index)
    {
        if (index < 0 || index >= _cols.Count) throw new ArgumentOutOfRangeException(nameof(index));
        _cols.RemoveAt(index);
        for (int i = 0; i < _cols.Count; i++) _cols[i].Index = i;
        foreach (var r in _rows) r.RemoveColumnAt(index);
        NormalizeSpans();
    }

    /// <summary>
    /// Get or create cell at (row,col). Auto-expands table if necessary.
    /// </summary>
    public PdfTableCell GetOrCreateCell(int row, int col)
    {
        if (row < 0 || col < 0) throw new ArgumentOutOfRangeException();
        EnsureSize(row + 1, col + 1);
        return _rows[row].GetCell(col);
    }

    public PdfTableCell GetCell(int row, int col)
    {
        if (row < 0 || col < 0) throw new ArgumentOutOfRangeException();
        if (row >= RowCount || col >= ColCount) throw new ArgumentOutOfRangeException();
        return _rows[row].GetCell(col);
    }

    public void SetCell(int row, int col, PdfTableCell cell)
    {
        if (row < 0 || col < 0) throw new ArgumentOutOfRangeException();
        EnsureSize(row + 1, col + 1);
        _rows[row].SetCell(col, cell ?? new PdfTableCell());
        NormalizeSpans();
    }

    public void SetText(int row, int col, string text)
    {
        var cell = GetOrCreateCell(row, col);
        cell.Text = text ?? "";
    }
    
    /// <summary>
    /// Set absolute widths for columns. Length must be <= ColCount; if shorter it's padded.
    /// </summary>
    public void SetColumnWidths(params double[] widths)
    {
        if (widths == null) throw new ArgumentNullException(nameof(widths));
        // Only ensure the column count, not the row count.
        while (_cols.Count < widths.Length)
        {
            AddColumn();
        }

        for (int i = 0; i < _cols.Count; i++)
        {
            _cols[i].Width = (i < widths.Length) ? widths[i] : (widths.LastOrDefault());
        }
    }

    /// <summary>
    /// Return effective column widths: use fixed widths where set, distribute remaining equally.
    /// </summary>
    public double[] GetEffectiveColumnWidths()
    {
        if (ColCount == 0) return Array.Empty<double>();
        double totalFixed = 0;
        int fixedCount = 0;
        foreach (var c in _cols)
        {
            if (c.Width.HasValue) { totalFixed += c.Width.Value; fixedCount++; }
        }
        int flexible = ColCount - fixedCount;
        double remaining = Math.Max(0, Width - totalFixed);
        double flexWidth = flexible > 0 ? remaining / flexible : 0;
        var result = new double[ColCount];
        for (int i = 0; i < ColCount; i++)
        {
            result[i] = _cols[i].Width ?? flexWidth;
        }
        return result;
    }
    /// <summary>
    /// Ensure RowSpan/ColSpan are in-bounds and >=1 for every cell.
    /// </summary>
    public void NormalizeSpans()
    {
        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                var cell = _rows[r].GetCell(c);
                if (cell.RowSpan < 1) cell.RowSpan = 1;
                if (cell.ColSpan < 1) cell.ColSpan = 1;
                if (r + cell.RowSpan > RowCount) cell.RowSpan = Math.Max(1, RowCount - r);
                if (c + cell.ColSpan > ColCount) cell.ColSpan = Math.Max(1, ColCount - c);
            }
        }
    }

    /// <summary>
    /// Checks if neighbour at (nr,nc) belongs to same span owner as (ownerR,ownerC).
    /// </summary>
    public bool IsSameSpanCell(int nr, int nc, int ownerR, int ownerC)
    {
        if (nr < 0 || nc < 0 || nr >= RowCount || nc >= ColCount) return false;
        for (int r = 0; r <= nr; r++)
        {
            for (int c = 0; c <= nc; c++)
            {
                var cell = _rows[r].GetCell(c);
                int rs = Math.Max(1, cell.RowSpan);
                int cs = Math.Max(1, cell.ColSpan);
                if (r + rs > nr && c + cs > nc && r <= nr && c <= nc)
                    return (r == ownerR && c == ownerC);
            }
        }
        return false;
    }

    public void TrimEmptyTrailingRowsAndCols()
    {
        // trim trailing empty rows
        for (int r = RowCount - 1; r >= 0; r--)
        {
            bool any = false;
            for (int c = 0; c < ColCount; c++)
            {
                var t = _rows[r].GetCell(c).Text;
                if (!string.IsNullOrEmpty(t)) { any = true; break; }
            }
            if (!any) _rows.RemoveAt(r);
            else break;
        }

        // trim trailing cols
        bool colTrim = true;
        while (colTrim && ColCount > 0)
        {
            int last = ColCount - 1;
            bool any = false;
            foreach (var row in _rows)
            {
                var t = row.GetCell(last).Text;
                if (!string.IsNullOrEmpty(t)) { any = true; break; }
            }
            if (!any)
            {
                RemoveColumn(last);
            }
            else colTrim = false;
        }

        NormalizeSpans();
    }

    // Minimal "fixed" Table class used by existing renderer (keeps it simple).
    // You can adapt your renderer to accept PdfTable directly if you prefer.
    public class FixedTable
    {
        public PdfTableCell[,] Cells;
        public int Rows;
        public int Cols;
        public double X;
        public double Y;
        public double Width;
        public double RowHeight;
        public double[] ColumnWidths;
    }

    /// <summary>
    /// Convert to a simple fixed 2D structure used by the renderer.
    /// </summary>
    public FixedTable ToFixedTable()
    {
        // If there are no rows or columns, return an empty fixed table.
        if (RowCount == 0 || ColCount == 0)
        {
            return new FixedTable
            {
                Rows = 0,
                Cols = 0,
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                RowHeight = this.RowHeight,
                ColumnWidths = Array.Empty<double>(),
                Cells = new PdfTableCell[0, 0]
            };
        }

        int rows = RowCount;
        int cols = ColCount;
        var fixedTbl = new FixedTable
        {
            Rows = rows,
            Cols = cols,
            X = this.X,
            Y = this.Y,
            Width = this.Width,
            RowHeight = this.RowHeight,
            ColumnWidths = GetEffectiveColumnWidths()
        };

        fixedTbl.Cells = new PdfTableCell[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // clone cell from model if present, otherwise new
                PdfTableCell src = _rows[r].GetCell(c);
                fixedTbl.Cells[r, c] = CloneCell(src);
            }
        }

        // Normalize spans in fixed table
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                var cell = fixedTbl.Cells[r, c];
                if (cell.RowSpan < 1) cell.RowSpan = 1;
                if (cell.ColSpan < 1) cell.ColSpan = 1;
                if (r + cell.RowSpan > rows) cell.RowSpan = Math.Max(1, rows - r);
                if (c + cell.ColSpan > cols) cell.ColSpan = Math.Max(1, cols - c);
            }

        return fixedTbl;
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
        // clone borders
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
