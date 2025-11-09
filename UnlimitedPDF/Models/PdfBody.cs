using System.Text;

namespace UnlimitedPDF.Models;

public class PdfObject
{
    public int ObjectNumber { get; set; }
    public int GenerationNumber { get; set; } = 0;
    public string Content { get; set; } = string.Empty;
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ObjectNumber} {GenerationNumber} obj");
        sb.AppendLine(Content);
        sb.AppendLine("endobj");
        return sb.ToString();
    }
}

public class PdfStreamObject : PdfObject
{
    public byte[] StreamData { get; set; } = Array.Empty<byte>();
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

public class PdfIndirectReference
{
    public int ObjectNumber { get; set; }
    public int GenerationNumber { get; set; } = 0;
    public override string ToString()
    {
        return $"{ObjectNumber} {GenerationNumber} R";
    }
}

public class PdfDictionary
{
    private readonly Dictionary<string, string> _entries = new();
    public void AddEntry(string key, string value)
    {
        _entries[key] = value;
    }
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

public class PdfStream
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("stream");
        sb.AppendLine(Encoding.ASCII.GetString(Data));
        sb.AppendLine("endstream");
        return sb.ToString();
    }
}


public class PdfHeader
{
    public string Version { get; set; } = "1.7";

    public override string ToString()
    {
        return $"%PDF-{Version}";
    }
}

public class PdfBody
{
    public List<PdfObject> Objects { get; set; } = new List<PdfObject>();

    public void AddObject(PdfObject pdfObject)
    {
        Objects.Add(pdfObject);
    }

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

public class PdfCatalog
{
    public PdfIndirectReference Pages { get; set; } = new PdfIndirectReference();

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

public class PdfPages
{
    public List<PdfIndirectReference> Kids { get; set; } = new List<PdfIndirectReference>();

    public int Count => Kids.Count;

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

public class PdfPage
{
    public PdfIndirectReference Parent { get; set; } = new PdfIndirectReference();

    public PdfIndirectReference Contents { get; set; } = new PdfIndirectReference();

    public PdfDictionary Resources { get; set; } = new PdfDictionary();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<<");
        sb.AppendLine("/Type /Page");
        sb.AppendLine($"/Parent {Parent}");
        sb.AppendLine($"/Contents {Contents}");
        sb.AppendLine($"/Resources {Resources}");
        sb.AppendLine(">>");
        return sb.ToString();
    }
}

public class PdfFont
{
    public string FontName { get; set; } = "Helvetica";

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

public class PdfTextElement
{
    public string FontName { get; set; } = "F1";
    public int FontSize { get; set; } = 12;
    public int X { get; set; }
    public int Y { get; set; }
    public string Text { get; set; } = string.Empty;

    public override string ToString()
    {
        // BT = Begin Text, ET = End Text
        // Tf = Set font and size
        // Td = Move to start of next line
        // Tj = Show text
        return $"BT /{FontName} {FontSize} Tf {X} {Y} Td ({Text}) Tj ET";
    }
}

public class PdfPageContent
{
    private readonly List<object> _contentElements = new();

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