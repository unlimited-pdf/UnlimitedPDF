namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents the trailer of a PDF document.
/// </summary>
internal sealed class PdfTrailer
{
    /// <summary>
    /// Gets or sets the byte offset of the cross-reference table.
    /// </summary>
    public long XrefPosition { get; set; }

    /// <summary>
    /// Gets or sets the total number of objects in the document.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the indirect reference to the document's catalog (root) object.
    /// </summary>
    public PdfIndirectReference Root { get; set; } = default!;

    /// <summary>
    /// Returns the string representation of the trailer section.
    /// </summary>
    /// <returns>A formatted string for the trailer.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("trailer");
        sb.AppendLine($"<< /Root {Root} /Size {Size} >>");
        sb.AppendLine("startxref");
        sb.AppendLine(XrefPosition.ToString());
        sb.AppendLine("%%EOF");
        return sb.ToString();
    }
}
