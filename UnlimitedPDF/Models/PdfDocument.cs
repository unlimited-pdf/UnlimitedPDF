using System.Text;

namespace UnlimitedPDF.Models;

public class PdfDocument
{
    private readonly PdfHeader _header;
    public PdfDocument()
    {
        _header = new PdfHeader();
    }

    /// <summary>
    /// Writes a PDF document to the specified file path.
    /// </summary>
    /// <param name="path"></param>
    public void Write(string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(fs, Encoding.ASCII);

        // Keep track of object offsets to build the xref table
        var offsets = new List<long>();

        // PDF Header
        writer.WriteLine(_header.ToString());

        // 1. Catalog object
        offsets.Add(fs.Position);
        writer.WriteLine("1 0 obj");
        writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
        writer.WriteLine("endobj");

        // 2. Pages object
        offsets.Add(fs.Position);
        writer.WriteLine("2 0 obj");
        writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
        writer.WriteLine("endobj");

        // 3. Page object
        offsets.Add(fs.Position);
        writer.WriteLine("3 0 obj");
        writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << >> >>");
        writer.WriteLine("endobj");

        // 4. Content stream
        string content = "BT /F1 24 Tf 100 700 Td (hello from Unlimited PDF library) Tj ET";
        byte[] contentBytes = Encoding.ASCII.GetBytes(content);
        offsets.Add(fs.Position);
        writer.WriteLine("4 0 obj");
        writer.WriteLine("<< /Length " + contentBytes.Length + " >>");
        writer.WriteLine("stream");
        writer.Flush();
        fs.Write(contentBytes, 0, contentBytes.Length);
        writer.WriteLine("\nendstream");
        writer.WriteLine("endobj");

        // 5. Font object (define built-in Helvetica)
        offsets.Add(fs.Position);
        writer.WriteLine("5 0 obj");
        writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
        writer.WriteLine("endobj");

        // Update page resource dictionary to include font
        // (minimal example won't handle cross references here)

        // XRef position
        long xrefPosition = fs.Position;
        writer.WriteLine("xref");

        int size = offsets.Count + 1;
        writer.WriteLine($"0 {size}");

        // special entry for object 0
        writer.WriteLine("0000000000 65535 f ");

        // Write offsets
        foreach (var offset in offsets)
        {
            // Each offset is written as a 10-digit zero-padded number
            // followed by a space, 5-digit generation number (00000), a space, and 'n' for in-use
            writer.WriteLine($"{offset:D10} 00000 n ");
        }

        // Trailer
        writer.WriteLine("trailer");
        writer.WriteLine("<< /Root 1 0 R /Size 6 >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefPosition);
        writer.WriteLine("%%EOF");
    }
}
