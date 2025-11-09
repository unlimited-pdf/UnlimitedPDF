namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents the cross-reference (xref) table of a PDF document.
/// </summary>
internal class PdfXrefTable
{
    private readonly List<long> _offsets = new();

    /// <summary>
    /// Adds the byte offset of a PDF object to the table.
    /// </summary>
    /// <param name="offset">The byte offset of the object in the file stream.</param>
    public void AddOffset(long offset)
    {
        _offsets.Add(offset);
    }

    /// <summary>
    /// Returns the string representation of the xref table.
    /// </summary>
    /// <returns>A formatted string for the xref section.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("xref");
        sb.AppendLine($"0 {_offsets.Count + 1}");
        sb.AppendLine("0000000000 65535 f "); // Special entry for object 0

        foreach (var offset in _offsets)
        {
            sb.AppendLine($"{offset:D10} 00000 n ");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets the total number of objects in the xref table, including the special object 0.
    /// </summary>
    public int Size => _offsets.Count + 1;
}
