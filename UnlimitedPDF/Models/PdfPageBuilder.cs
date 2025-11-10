using UnlimitedPDF.Models.Table;

namespace UnlimitedPDF;

/// <summary>
/// Manages the content for a PDF page, allowing for the addition of elements like text.
/// </summary>
public class PdfPageBuilder
{
    private readonly PdfPageContentStream _contentStream;
    private readonly List<object> _contentElements = new();

    internal PdfPageBuilder(PdfPageContentStream contentStream)
    {
        _contentStream = contentStream;
    }

    /// <summary>
    /// Adds a text element to the content at the specified position with the given font settings.
    /// </summary>
    /// <param name="text">The text to be added to the content. Cannot be null.</param>
    /// <param name="x">The x-coordinate of the text's position, in points.</param>
    /// <param name="y">The y-coordinate of the text's position, in points.</param>
    /// <param name="fontSize">The size of the font, in points. The default value is 12.</param>
    /// <param name="fontName">The name of the font to use. The default value is "F1".</param>
    public void AddText(string text, int x, int y, int fontSize = 12, string fontName = "F1")
    {
        _contentElements.Add(new PdfTextElement
        {
            Text = text,
            X = x,
            Y = y,
            FontSize = fontSize,
            FontName = fontName
        });
    }

    /// <summary>
    /// Adds a new table to the page at the specified position and with the given width.
    /// </summary>
    /// <param name="x">The x-coordinate of the table's top-left corner.</param>
    /// <param name="y">The y-coordinate of the table's top-left corner.</param>
    /// <param name="width">The total width of the table.</param>
    /// <returns>A <see cref="PdfTable"/> object for configuration.</returns>
    public PdfTable AddTable(double x, double y, double width)
    {
        var table = new PdfTable
        {
            X = x,
            Y = y,
            Width = width
        };
        _contentElements.Add(table);
        return table;
    }

    /// <summary>
    /// Compiles the content elements and updates the associated stream data.
    /// </summary>
    internal void UpdateContentStream()
    {
        var sb = new StringBuilder();
        foreach (var element in _contentElements)
        {
            if (element is PdfTable table)
            {
                sb.Append(RenderTable(table));
            }
            else
            {
                sb.AppendLine(element.ToString());
            }
        }
        _contentStream.StreamData = Encoding.ASCII.GetBytes(sb.ToString().Trim());
    }

    private string RenderTable(PdfTable table)
    {
        var sb = new StringBuilder();
        var fixedTable = table.ToFixedTable();
        double currentY = fixedTable.Y;

        for (int r = 0; r < fixedTable.Rows; r++)
        {
            double currentX = fixedTable.X;
            for (int c = 0; c < fixedTable.Cols; c++)
            {
                var cell = fixedTable.Cells[r, c];
                double cellWidth = fixedTable.ColumnWidths[c];

                sb.AppendLine("q"); // Save graphics state

                // Draw background
                var bg = cell.Background;
                sb.AppendLine($"{bg.R:F3} {bg.G:F3} {bg.B:F3} rg"); // Set fill color
                sb.AppendLine($"{currentX} {currentY - fixedTable.RowHeight} {cellWidth} {fixedTable.RowHeight} re f"); // Draw and fill rectangle

                // Draw borders
                var border = cell.LeftBorder; // Using left as a proxy for all for now
                sb.AppendLine($"{border.Color.R:F3} {border.Color.G:F3} {border.Color.B:F3} RG"); // Set stroke color
                sb.AppendLine($"{border.Width} w"); // Border width
                sb.AppendLine($"{currentX} {currentY - fixedTable.RowHeight} {cellWidth} {fixedTable.RowHeight} re s"); // Stroke rectangle

                // Draw text
                if (!string.IsNullOrEmpty(cell.Text))
                {
                    sb.AppendLine("BT"); // Begin text
                    sb.AppendLine("0 0 0 rg"); // Set text color to black
                    sb.AppendLine($"/F1 12 Tf"); // Font

                    // Calculate text position based on horizontal alignment
                    double textWidth = GetTextWidth(cell.Text, 12);
                    double textX;

                    switch (cell.HAlign)
                    {
                        case HAlign.Right:
                            textX = currentX + cellWidth - textWidth - cell.Padding;
                            break;
                        case HAlign.Center:
                            textX = currentX + (cellWidth - textWidth) / 2;
                            break;
                        default: // HAlign.Left
                            textX = currentX + cell.Padding;
                            break;
                    }

                    double textY = currentY - (fixedTable.RowHeight / 2) - 4; // Simple middle alignment
                    sb.AppendLine($"1 0 0 1 {textX} {textY} Tm"); // Set text matrix for absolute position
                    sb.AppendLine($"({cell.Text}) Tj"); // Show text
                    sb.AppendLine("ET"); // End text
                }

                sb.AppendLine("Q"); // Restore graphics state

                currentX += cellWidth;
            }
            currentY -= fixedTable.RowHeight;
        }

        return sb.ToString();
    }

    // A simple approximation for text width. For accurate results, you would need font metrics.
    private double GetTextWidth(string text, int fontSize)
    {
        return text.Length * fontSize * 0.5;
    }
}
