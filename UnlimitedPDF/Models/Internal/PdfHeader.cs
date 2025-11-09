namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents the header of a PDF file, specifying the PDF version.
/// </summary>
internal class PdfHeader
{
    /// <summary>
    /// Gets or sets the PDF version. Defaults to "1.7".
    /// </summary>
    public string Version { get; set; } = "1.7";

    /// <summary>
    /// Returns the formatted PDF header string.
    /// </summary>
    /// <returns>A string in the format "%PDF-{Version}".</returns>
    public override string ToString()
    {
        return $"%PDF-{Version}";
    }
}
