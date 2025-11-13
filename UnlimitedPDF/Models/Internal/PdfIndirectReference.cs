namespace UnlimitedPDF.Models.Internal;

/// <summary>
/// Represents an indirect reference to another PDF object.
/// </summary>
internal sealed class PdfIndirectReference
{
    public PdfIndirectReference(int objectNumber, int generationNumber = 0)
    {
        ObjectNumber = objectNumber;
        GenerationNumber = generationNumber;
    }

    /// <summary>
    /// Gets the object number of the referenced object.
    /// </summary>
    public int ObjectNumber { get; }

    /// <summary>
    /// Gets the generation number of the referenced object.
    /// </summary>
    public int GenerationNumber { get; }

    /// <summary>
    /// Returns the string representation of the indirect reference (e.g., "1 0 R").
    /// </summary>
    /// <returns>A string in the format "{ObjectNumber} {GenerationNumber} R".</returns>
    public override string ToString()
    {
        return $"{ObjectNumber} {GenerationNumber} R";
    }
}
