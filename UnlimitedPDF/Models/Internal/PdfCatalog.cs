namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents the document catalog, the root object of a PDF's object hierarchy.
/// </summary>
internal sealed class PdfCatalog
{
    public PdfCatalog(PdfIndirectReference pages)
    {
        Pages = pages;
    }

    /// <summary>
    /// Gets the indirect reference to the page tree root (<see cref="PdfPages"/> object).
    /// </summary>
    public PdfIndirectReference Pages { get; }

    /// <summary>
    /// Returns the string representation of the PDF catalog dictionary.
    /// </summary>
    /// <returns>A string representing the catalog dictionary.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Catalog");
        sb.AppendLine($"/Pages {Pages}");
        sb.AppendLine(">>");
        return sb.ToString();
    }
}
