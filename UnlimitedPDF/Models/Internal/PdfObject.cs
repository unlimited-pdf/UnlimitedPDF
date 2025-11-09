namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents a fundamental object in a PDF file. Each object has a unique number and a generation number.
/// </summary>
internal class PdfObject
{
    /// <summary>
    /// Gets or sets the unique identifier for the object within the PDF document.
    /// </summary>
    public int ObjectNumber { get; set; }

    /// <summary>
    /// Gets or sets the generation number of the object. For new objects, this is typically 0.
    /// </summary>
    public int GenerationNumber { get; set; } = 0;

    /// <summary>
    /// Gets or sets the string content of the object, which is usually a PDF dictionary.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Returns the string representation of the PDF object, formatted for inclusion in a PDF file.
    /// </summary>
    /// <returns>A string representing the complete PDF object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ObjectNumber} {GenerationNumber} obj");
        sb.AppendLine(Content);
        sb.AppendLine("endobj");
        return sb.ToString();
    }
}
