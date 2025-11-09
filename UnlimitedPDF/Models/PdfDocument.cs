using System.Text;

namespace UnlimitedPDF.Models;

public class PdfDocument
{
    private readonly PdfHeader _header;
    private readonly PdfBody _body;

    public PdfDocument()
    {
        _header = new PdfHeader();
        _body = new PdfBody();

        // Build the core PDF document structure
        var catalog = new PdfObject { ObjectNumber = 1, Content = new PdfCatalog { Pages = new PdfIndirectReference { ObjectNumber = 2 } }.ToString() };
        var pages = new PdfObject { ObjectNumber = 2, Content = new PdfPages { Kids = new List<PdfIndirectReference> { new() { ObjectNumber = 3 } } }.ToString() };
        var page = new PdfObject { ObjectNumber = 3, Content = new PdfPage { Parent = new PdfIndirectReference { ObjectNumber = 2 }, Contents = new PdfIndirectReference { ObjectNumber = 4 } }.ToString() };

        // Create page content
        var pageContent = new PdfPageContent();
        pageContent.AddText("Hello from Unlimited PDF library", 100, 700, 24);
        pageContent.AddText("Hello from Unlimited PDF library2", 105, 705, 24);
        var contentStream = new PdfPageContentStream(pageContent.ToString()) { ObjectNumber = 4 };

        // Define a font resource
        var font = new PdfObject { ObjectNumber = 5, Content = new PdfFont().ToString() };

        // Add all objects to the document body
        _body.AddObject(catalog);
        _body.AddObject(pages);
        _body.AddObject(page);
        _body.AddObject(contentStream);
        _body.AddObject(font);
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
        writer.Flush();

        // Write PDF objects from the body
        foreach (var obj in _body.Objects)
        {
            offsets.Add(fs.Position);
            if (obj is PdfStreamObject streamObj)
            {
                // Custom writing for stream objects to handle byte arrays
                writer.WriteLine($"{streamObj.ObjectNumber} {streamObj.GenerationNumber} obj");
                writer.WriteLine($"<< /Length {streamObj.StreamData.Length} >>");
                writer.WriteLine("stream");
                writer.Flush();
                fs.Write(streamObj.StreamData, 0, streamObj.StreamData.Length);
                writer.WriteLine();
                writer.WriteLine("endstream");
                writer.WriteLine("endobj");
            }
            else
            {
                writer.WriteLine(obj.ToString());
            }
            writer.Flush();
        }

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
            writer.WriteLine($"{offset:D10} 00000 n ");
        }

        // Trailer
        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Root 1 0 R /Size {size} >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefPosition);
        writer.WriteLine("%%EOF");
    }

    /// <summary>
    /// Adds a new page to the PDF document.
    /// </summary>
    /// <remarks>The page is converted to a PDF object and appended to the document's body.  Ensure the
    /// <paramref name="page"/> parameter is properly initialized before calling this method.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page to be added.</param>
    public void AddPage(PdfPage page)
    {
        if (page is null)
            throw new ArgumentNullException(nameof(page), "Page cannot be null.");

        var pageObject = new PdfObject
        {
            ObjectNumber = _body.Objects.Count + 1,
            Content = page.ToString()
        };

        _body.AddObject(pageObject);
    }
}
