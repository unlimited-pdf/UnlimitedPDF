using System.Text;
using UnlimitedPDF.Enums;
using UnlimitedPDF.Extensions;

namespace UnlimitedPDF.Models;

public class PdfDocument
{
    private readonly PdfHeader _header;
    private readonly PdfBody _body;
    private readonly PdfPages _pages;

    /// <summary>
    /// Gets or sets the default page size for the document.
    /// </summary>
    public PdfPageSize PageSize { get; set; }

    public PdfDocument(PdfPageSize pageSize = PdfPageSize.A4)
    {
        PageSize = pageSize;
        _header = new PdfHeader();
        _body = new PdfBody();
        _pages = new PdfPages();

        // Build the core PDF document structure
        var catalog = new PdfObject { ObjectNumber = 1, Content = new PdfCatalog { Pages = new PdfIndirectReference { ObjectNumber = 2 } }.ToString() };
        var pagesObject = new PdfObject { ObjectNumber = 2, Content = _pages.ToString() };

        // Define a font resource
        var font = new PdfObject { ObjectNumber = 3, Content = new PdfFont().ToString() };

        // Add all objects to the document body
        _body.AddObject(catalog);
        _body.AddObject(pagesObject);
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
    /// Adds a new page to the PDF document from the specified content.
    /// </summary>
    /// <param name="pageContent">The content to be added to the new page.</param>
    public void AddPageFromContent(PdfPageContent pageContent)
    {
        if (pageContent is null)
            throw new ArgumentNullException(nameof(pageContent), "Page content cannot be null.");

        // Find the pages object
        var pagesObject = _body.Objects.First(o => o.ObjectNumber == 2);
            
        // Create new page and content stream objects
        int pageObjectNumber = _body.Objects.Max(o => o.ObjectNumber) + 1;
        int contentStreamObjectNumber = pageObjectNumber + 1;

        var contentStream = new PdfPageContentStream(pageContent.ToString())
        {
            ObjectNumber = contentStreamObjectNumber
        };

        var page = new PdfPage
        {
            Parent = new PdfIndirectReference { ObjectNumber = 2 },
            Contents = new PdfIndirectReference { ObjectNumber = contentStreamObjectNumber },
            MediaBox = PageSize.ToMediaBoxString()
        };

        var pageObject = new PdfObject
        {
            ObjectNumber = pageObjectNumber,
            Content = page.ToString()
        };

        // Add new objects to the body
        _body.AddObject(pageObject);
        _body.AddObject(contentStream);

        // Update the Kids array in the pages object
        _pages.Kids.Add(new PdfIndirectReference { ObjectNumber = pageObjectNumber });
        pagesObject.Content = _pages.ToString();
    }
}
