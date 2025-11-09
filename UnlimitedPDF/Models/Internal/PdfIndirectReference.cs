namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents an indirect reference to another PDF object.
/// </summary>
internal class PdfIndirectReference
{
    /// <summary>
    /// Gets or sets the object number of the referenced object.
    /// </summary>
    public int ObjectNumber { get; set; }

    /// <summary>
    /// Gets or sets the generation number of the referenced object.
    /// </summary>
    public int GenerationNumber { get; set; } = 0;

    /// <summary>
    /// Returns the string representation of the indirect reference (e.g., "1 0 R").
    /// </summary>
    /// <returns>A string in the format "{ObjectNumber} {GenerationNumber} R".</returns>
    public override string ToString()
    {
        return $"{ObjectNumber} {GenerationNumber} R";
    }
}
