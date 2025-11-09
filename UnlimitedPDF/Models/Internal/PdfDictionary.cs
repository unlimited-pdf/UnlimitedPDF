namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents a PDF dictionary, a collection of key-value pairs.
/// </summary>
internal class PdfDictionary
{
    private readonly Dictionary<string, string> _entries = new();

    /// <summary>
    /// Adds or updates an entry in the dictionary. Keys should not include the leading slash.
    /// </summary>
    /// <param name="key">The key for the entry.</param>
    /// <param name="value">The value for the entry.</param>
    public void AddEntry(string key, string value)
    {
        _entries[key] = value;
    }

    /// <summary>
    /// Returns the string representation of the PDF dictionary.
    /// </summary>
    /// <returns>A formatted string enclosed in "<<" and ">>".</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        foreach (var entry in _entries)
        {
            sb.AppendLine($"/{entry.Key} {entry.Value}");
        }
        sb.AppendLine(">>");
        return sb.ToString();
    }
}
