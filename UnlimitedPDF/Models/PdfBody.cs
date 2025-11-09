using System.Text;

namespace UnlimitedPDF.Models;

/// <summary>
/// Represents a fundamental object in a PDF file. Each object has a unique number and a generation number.
/// </summary>
public class PdfObject
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

/// <summary>
/// Represents a PDF object that contains a stream of data, such as page content or an image.
/// </summary>
public class PdfStreamObject : PdfObject
{
    /// <summary>
    /// Gets or sets the raw byte data for the stream.
    /// </summary>
    public byte[] StreamData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Returns the string representation of the PDF stream object.
    /// </summary>
    /// <returns>A string representing the complete PDF stream object, including its dictionary and data.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ObjectNumber} {GenerationNumber} obj");
        sb.AppendLine($"<< /Length {StreamData.Length} >>");
        sb.AppendLine("stream");
        sb.AppendLine(Encoding.ASCII.GetString(StreamData));
        sb.AppendLine("endstream");
        sb.AppendLine("endobj");
        return sb.ToString();
    }
}

/// <summary>
/// Represents an indirect reference to another PDF object.
/// </summary>
public class PdfIndirectReference
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

/// <summary>
/// Represents a PDF dictionary, a collection of key-value pairs.
/// </summary>
public class PdfDictionary
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

/// <summary>
/// Represents the header of a PDF file, specifying the PDF version.
/// </summary>
public class PdfHeader
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

/// <summary>
/// Represents the body of a PDF document, which contains a collection of PDF objects.
/// </summary>
public class PdfBody
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

/// <summary>
/// Represents the document catalog, the root object of a PDF's object hierarchy.
/// </summary>
public class PdfCatalog
{
    /// <summary>
    /// Gets or sets the indirect reference to the page tree root (<see cref="PdfPages"/> object).
    /// </summary>
    public PdfIndirectReference Pages { get; set; } = new PdfIndirectReference();

    /// <summary>
    /// Returns the string representation of the PDF catalog dictionary.
    /// </summary>
    /// <returns>A string representing the catalog dictionary.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Catalog");
        sb.AppendLine($"/Pages {Pages}");
        sb.AppendLine(">>");
        return sb.ToString();
    }
}

/// <summary>
/// Represents a page tree node, which contains a list of pages or other page tree nodes.
/// </summary>
public class PdfPages
{
    /// <summary>
    /// Gets or sets the list of indirect references to child pages (<see cref="PdfPage"/> objects).
    /// </summary>
    public List<PdfIndirectReference> Kids { get; set; } = new List<PdfIndirectReference>();

    /// <summary>
    /// Gets the number of pages in this node.
    /// </summary>
    public int Count => Kids.Count;

    /// <summary>
    /// Returns the string representation of the PDF pages dictionary.
    /// </summary>
    /// <returns>A string representing the pages dictionary.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Pages");
        sb.AppendLine($"/Kids [{string.Join(" ", Kids)}]");
        sb.AppendLine($"/Count {Count}");
        sb.AppendLine(">>");
        return sb.ToString();
    }
}

/// <summary>
/// Represents a single page in a PDF document.
/// </summary>
public class PdfPage
{
    /// <summary>
    /// Gets or sets the indirect reference to the parent page tree node (<see cref="PdfPages"/> object).
    /// </summary>
    public PdfIndirectReference Parent { get; set; } = new PdfIndirectReference();

    /// <summary>
    /// Gets or sets the page's dimensions, represented as a PDF MediaBox string (e.g., "[0 0 595 842]"):
    /// </summary>
    public string MediaBox { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the indirect reference to the page's content stream (<see cref="PdfStreamObject"/>).
    /// </summary>
    public PdfIndirectReference Contents { get; set; } = new PdfIndirectReference();

    /// <summary>
    /// Gets or sets the dictionary of resources for the page, such as fonts.
    /// </summary>
    public PdfDictionary Resources { get; set; } = new PdfDictionary();

    /// <summary>
    /// Returns the string representation of the PDF page dictionary.
    /// </summary>
    /// <returns>A string representing the page dictionary.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Page");
        sb.AppendLine($"/Parent {Parent}");
        if (!string.IsNullOrEmpty(MediaBox))
        {
            sb.AppendLine($"/MediaBox {MediaBox}");
        }
        sb.AppendLine($"/Contents {Contents}");
        sb.AppendLine($"/Resources {Resources}");
        sb.AppendLine(">>");
        return sb.ToString();
    }
}

/// <summary>
/// Represents a font resource in a PDF document.
/// </summary>
public class PdfFont
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

/// <summary>
/// Represents a text element to be drawn on a PDF page.
/// </summary>
public class PdfTextElement
{
    /// <summary>
    /// Gets or sets the font resource name (e.g., "F1").
    /// </summary>
    public string FontName { get; set; } = "F1";

    /// <summary>
    /// Gets or sets the font size in points.
    /// </summary>
    public int FontSize { get; set; } = 12;

    /// <summary>
    /// Gets or sets the X coordinate for the text's starting position.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate for the text's starting position.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Gets or sets the text string to be displayed.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Returns the PDF content stream command for drawing this text element.
    /// </summary>
    /// <returns>A string representing the text drawing command.</returns>
    public override string ToString()
    {
        // BT = Begin Text, ET = End Text
        // Tf = Set font and size
        // Td = Move to start of next line
        // Tj = Show text
        return $"BT /{FontName} {FontSize} Tf {X} {Y} Td ({Text}) Tj ET";
    }
}

/// <summary>
/// Manages the content stream for a PDF page, allowing for the addition of elements like text.
/// </summary>
public class PdfPageContent
{
    private readonly List<object> _contentElements = new();

    /// <summary>
    /// Adds a text element to the page content.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontName">The font resource name.</param>
    public void AddText(string text, int x, int y, int fontSize = 12, string fontName = "F1")
    {
        _contentElements.Add(new PdfTextElement
        {
            Text = text,
            X = x,
            Y = y,
            FontSize = fontSize,
            FontName = fontName
        });
    }

    /// <summary>
    /// Returns the complete string for the page's content stream by combining all elements.
    /// </summary>
    /// <returns>A string representing the full content stream.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var element in _contentElements)
        {
            sb.AppendLine(element.ToString());
        }
        return sb.ToString().Trim();
    }
}

/// <summary>
/// Represents a content stream object for a PDF page.
/// </summary>
public class PdfPageContentStream : PdfStreamObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfPageContentStream"/> class with the specified content.
    /// </summary>
    /// <param name="content">The content string for the stream.</param>
    public PdfPageContentStream(string content)
    {
        StreamData = Encoding.ASCII.GetBytes(content);
    }
}