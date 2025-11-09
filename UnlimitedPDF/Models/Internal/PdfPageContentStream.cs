namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents a content stream object for a PDF page.
/// </summary>
internal class PdfPageContentStream : PdfStreamObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfPageContentStream"/> class with the specified content.
    /// </summary>
    /// <param name="content">The content string for the stream.</param>
    public PdfPageContentStream(string content)
    {
        StreamData = Encoding.ASCII.GetBytes(content);
    }
}
