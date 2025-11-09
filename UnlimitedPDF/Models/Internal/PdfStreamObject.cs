namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents a PDF object that contains a stream of data, such as page content or an image.
/// </summary>
internal class PdfStreamObject : PdfObject
{
    /// <summary>
    /// Gets or sets the raw byte data for the stream.
    /// </summary>
    public byte[] StreamData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Returns the string representation of the PDF stream object.
    /// </summary>
    /// <returns>A string representing the complete PDF stream object, including its dictionary and data.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ObjectNumber} {GenerationNumber} obj");
        sb.AppendLine($"<< /Length {StreamData.Length} >>");
        sb.AppendLine("stream");
        sb.AppendLine(Encoding.ASCII.GetString(StreamData));
        sb.AppendLine("endstream");
        sb.AppendLine("endobj");
        return sb.ToString();
    }
}
