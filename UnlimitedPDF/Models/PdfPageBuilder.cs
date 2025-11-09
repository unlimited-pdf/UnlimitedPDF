namespace UnlimitedPDF.Models;

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
    /// Compiles the content elements and updates the associated stream data.
    /// </summary>
    internal void UpdateContentStream()
    {
        var sb = new StringBuilder();
        foreach (var element in _contentElements)
        {
            sb.AppendLine(element.ToString());
        }
        _contentStream.StreamData = Encoding.ASCII.GetBytes(sb.ToString().Trim());
    }
}
