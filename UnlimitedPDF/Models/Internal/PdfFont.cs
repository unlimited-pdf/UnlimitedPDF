namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents a font resource in a PDF document.
/// </summary>
internal class PdfFont
{
    /// <summary>
    /// Gets or sets the name of the font, such as "Helvetica".
    /// </summary>
    public string FontName { get; set; } = "Helvetica";

    /// <summary>
    /// Returns the string representation of the PDF font dictionary.
    /// </summary>
    /// <returns>A string representing the font dictionary.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Font");
        sb.AppendLine("/Subtype /Type1");
        sb.AppendLine($"/BaseFont /{FontName}");
        sb.AppendLine(">>");
        return sb.ToString();
    }
}
