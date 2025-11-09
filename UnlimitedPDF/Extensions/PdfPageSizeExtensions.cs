using System.Globalization;

namespace UnlimitedPDF.Extensions;

/// <summary>
/// Provides different extension methods for the <see cref="UnlimitedPDF.Enums.PdfPageSize"/> enum.
/// </summary>
public static class PdfPageSizeExtensions
{
    // Default PPI (points per inch) used in PDF measurements
    private const double DefaultPPI = 72.0;

    public static (double Width, double Height) ToDimensions(this UnlimitedPDF.Enums.PdfPageSize pageSize)
    {
        return pageSize switch
        {
            UnlimitedPDF.Enums.PdfPageSize.A0 => (2383.94, 3370.39),
            UnlimitedPDF.Enums.PdfPageSize.A1 => (1683.78, 2383.94),
            UnlimitedPDF.Enums.PdfPageSize.A2 => (1190.55, 1683.78),
            UnlimitedPDF.Enums.PdfPageSize.A3 => (841.89, 1190.55),
            UnlimitedPDF.Enums.PdfPageSize.A4 => (595.28, 841.89),
            UnlimitedPDF.Enums.PdfPageSize.A5 => (419.53, 595.28),
            UnlimitedPDF.Enums.PdfPageSize.A6 => (297.64, 419.53),

            UnlimitedPDF.Enums.PdfPageSize.Executive => (522.0, 756.0),
            UnlimitedPDF.Enums.PdfPageSize.Ledger => (1224.0, 792.0),
            UnlimitedPDF.Enums.PdfPageSize.Legal => (612.0, 1008.0),
            UnlimitedPDF.Enums.PdfPageSize.Letter => (612.0, 792.0),
            UnlimitedPDF.Enums.PdfPageSize.Tabloid => (792.0, 1224.0),

            UnlimitedPDF.Enums.PdfPageSize.B4 => (729.0, 1032.0),
            UnlimitedPDF.Enums.PdfPageSize.B5 => (516.0, 729.0),
            UnlimitedPDF.Enums.PdfPageSize.Statement => (396.0, 612.0),
            UnlimitedPDF.Enums.PdfPageSize.HalfLetter => (396.0, 612.0),

            _ => throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, null)
        };
    }

    /// <summary>
    /// Converts the specified <see cref="UnlimitedPDF.Enums.PdfPageSize"/> to a string representation of its media box
    /// dimensions.
    /// </summary>
    /// <remarks>The returned string specifies the lower-left and upper-right corners of the media box in a
    /// coordinate system where the origin is at the lower-left corner of the page.</remarks>
    /// <param name="pageSize">The page size to convert.</param>
    /// <returns>A string representing the media box dimensions in the format "0 0 [width] [height]".</returns>
    public static string ToMediaBoxString(this UnlimitedPDF.Enums.PdfPageSize pageSize)
    {
        var (width, height) = pageSize.ToDimensions();
        return $"/MediaBox [0 0 {width.ToString(CultureInfo.InvariantCulture)} {height.ToString(CultureInfo.InvariantCulture)}]";
    }
}
