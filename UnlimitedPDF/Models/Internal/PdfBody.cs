namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents the body of a PDF document, which contains a collection of PDF objects.
/// </summary>
internal class PdfBody
{
    /// <summary>
    /// Gets or sets the list of PDF objects that make up the document's body.
    /// </summary>
    public List<PdfObject> Objects { get; set; } = new List<PdfObject>();

    /// <summary>
    /// Adds a <see cref="PdfObject"/> to the body of the PDF.
    /// </summary>
    /// <param name="pdfObject">The PDF object to add.</param>
    public void AddObject(PdfObject pdfObject)
    {
        Objects.Add(pdfObject);
    }

    /// <summary>
    /// Returns the string representation of the entire PDF body by concatenating all its objects.
    /// </summary>
    /// <returns>A string containing all PDF objects in the body.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var obj in Objects)
        {
            sb.AppendLine(obj.ToString());
        }
        return sb.ToString();
    }
}
