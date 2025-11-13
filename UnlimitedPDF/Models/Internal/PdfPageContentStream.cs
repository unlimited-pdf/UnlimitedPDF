namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents a content stream object for a PDF page.
/// </summary>
internal sealed class PdfPageContentStream : PdfStreamObject
{
    public PdfPageContentStream(int objectNumber, string content, int generationNumber = 0)
        : base(objectNumber, content, generationNumber)
    {
        StreamData = Encoding.ASCII.GetBytes(content);
    }
}
