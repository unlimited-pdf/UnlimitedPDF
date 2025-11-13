namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents the header of a PDF file, specifying the PDF version.
/// </summary>
internal sealed class PdfHeader
{
    public PdfHeader(string version = "1.7")
    {
        Version = version;
    }

    /// <summary>
    /// Gets the version of the application or component.
    /// </summary>
    /// <remarks>The version string follows semantic versioning conventions and can be used to identify the
    /// specific release of the application or component.</remarks>
    public string Version { get; }

    /// <summary>
    /// Returns the formatted PDF header string.
    /// </summary>
    /// <returns>A string in the format "%PDF-{Version}".</returns>
    public override string ToString()
    {
        return $"%PDF-{Version}";
    }
}
