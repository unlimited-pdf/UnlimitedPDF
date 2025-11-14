namespace UnlimitedPDF;

public class PdfDocument
{
    private readonly PdfHeader _header;
    private readonly PdfBody _body;
    private readonly PdfPages _pages;
    private readonly List<PdfPageBuilder> _pageBuilders = new();

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
        var catalog = new PdfObject(objectNumber: 1, content: new PdfCatalog(pages: new PdfIndirectReference(objectNumber: 2)).ToString());
        var pagesObject = new PdfObject (objectNumber: 2, content: _pages.ToString());

        // Define a font resource
        var font = new PdfObject(objectNumber: 3, content: new PdfFont().ToString());

        // Add all objects to the document body
        _body.AddObject(catalog);
        _body.AddObject(pagesObject);
        _body.AddObject(font);
    }

    /// <summary>
    /// Writes a PDF document to the specified file path.
    /// </summary>
    /// <param name="path">The file path where the document will be saved.</param>
    public void Save(string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        WriteToStream(fs);
    }

    /// <summary>
    /// Writes a PDF document to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to which the document will be written.</param>
    public void Save(Stream stream)
    {
        WriteToStream(stream);
    }

    /// <summary>
    /// Asynchronously writes a PDF document to the specified file path.
    /// </summary>
    /// <param name="path">The file path where the document will be saved.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        await WriteToStreamAsync(fs, cancellationToken);
    }

    /// <summary>
    /// Asynchronously writes a PDF document to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to which the document will be written.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        await WriteToStreamAsync(stream, cancellationToken);
    }

    private void PrepareDocumentForSave()
    {
        // Finalize page content before writing
        foreach (var page in _pageBuilders)
        {
            page.UpdateContentStream();
        }

        // Update the content of the main pages object (object 2) to reflect all added pages.
        var pagesObject = _body.Objects.First(o => o.ObjectNumber == 2);
        pagesObject.Content = _pages.ToString();
    }

    private void WriteToStream(Stream stream)
    {
        PrepareDocumentForSave();

        using var writer = new StreamWriter(stream, Encoding.ASCII, 1024, leaveOpen: true);

        // Keep track of object offsets to build the xref table
        var offsets = new List<long>();

        // PDF Header
        writer.WriteLine(_header.ToString());
        writer.Flush();

        // Write PDF objects from the body
        foreach (var obj in _body.Objects)
        {
            offsets.Add(stream.Position);
            if (obj is PdfStreamObject streamObj)
            {
                // Custom writing for stream objects to handle byte arrays
                writer.WriteLine($"{streamObj.ObjectNumber} {streamObj.GenerationNumber} obj");
                writer.WriteLine($"<< /Length {streamObj.StreamData.Length} >>");
                writer.WriteLine("stream");
                writer.Flush();
                stream.Write(streamObj.StreamData, 0, streamObj.StreamData.Length);
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
        long xrefPosition = stream.Position;
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

    private async Task WriteToStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        PrepareDocumentForSave();

        using var writer = new StreamWriter(stream, Encoding.ASCII, 1024, leaveOpen: true);

        // Keep track of object offsets to build the xref table
        var offsets = new List<long>();

        // PDF Header
        await writer.WriteLineAsync(_header.ToString().AsMemory(), cancellationToken);
        await writer.FlushAsync();

        // Write PDF objects from the body
        foreach (var obj in _body.Objects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            offsets.Add(stream.Position);
            if (obj is PdfStreamObject streamObj)
            {
                // Custom writing for stream objects to handle byte arrays
                await writer.WriteLineAsync($"{streamObj.ObjectNumber} {streamObj.GenerationNumber} obj".AsMemory(), cancellationToken);
                await writer.WriteLineAsync($"<< /Length {streamObj.StreamData.Length} >>".AsMemory(), cancellationToken);
                await writer.WriteLineAsync("stream".AsMemory(), cancellationToken);
                await writer.FlushAsync();
                await stream.WriteAsync(streamObj.StreamData, cancellationToken);
                await writer.WriteLineAsync();
                await writer.WriteLineAsync("endstream".AsMemory(), cancellationToken);
                await writer.WriteLineAsync("endobj".AsMemory(), cancellationToken);
            }
            else
            {
                await writer.WriteLineAsync(obj.ToString().AsMemory(), cancellationToken);
            }
            await writer.FlushAsync();
        }

        // XRef position
        long xrefPosition = stream.Position;
        await writer.WriteLineAsync("xref".AsMemory(), cancellationToken);

        int size = offsets.Count + 1;
        await writer.WriteLineAsync($"0 {size}".AsMemory(), cancellationToken);

        // special entry for object 0
        await writer.WriteLineAsync("0000000000 65535 f ".AsMemory(), cancellationToken);

        // Write offsets
        foreach (var offset in offsets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync($"{offset:D10} 00000 n ".AsMemory(), cancellationToken);
        }

        // Trailer
        await writer.WriteLineAsync("trailer".AsMemory(), cancellationToken);
        await writer.WriteLineAsync($"<< /Root 1 0 R /Size {size} >>".AsMemory(), cancellationToken);
        await writer.WriteLineAsync("startxref".AsMemory(), cancellationToken);
        await writer.WriteLineAsync(xrefPosition.ToString().AsMemory(), cancellationToken);
        await writer.WriteLineAsync("%%EOF".AsMemory(), cancellationToken);
    }

    /// <summary>
    /// Adds a new page to the PDF document and returns a page object for adding content.
    /// </summary>
    /// <returns>A <see cref="PdfPageBuilder"/> object to which content can be added.</returns>
    public PdfPageBuilder AddPage()
    {
        // Determine object numbers for the new page and its content stream.
        int pageObjectNumber = _body.Objects.Max(o => o.ObjectNumber) + 1;
        int contentStreamObjectNumber = pageObjectNumber + 1;

        // Create an empty content stream object. It will be populated with content just before the document is written.
        var contentStream = new PdfPageContentStream(contentStreamObjectNumber, string.Empty);

        // Create pdf page internal object
        var pageInternal = new PdfPage
        {
            Parent = new PdfIndirectReference(objectNumber: 2), // Parent is the /Pages object
            Contents = new PdfIndirectReference(objectNumber: contentStreamObjectNumber),
            MediaBox = PageSize.ToMediaBoxString()
        };

        var pageObject = new PdfObject(objectNumber: pageObjectNumber, content: pageInternal.ToString());

        // Add the new page and content stream objects to the document body.
        _body.AddObject(pageObject);
        _body.AddObject(contentStream);

        // Update the /Kids array in the /Pages object to include the new page.
        _pages.Kids.Add(new PdfIndirectReference(objectNumber: pageObjectNumber));

        // Create a new Page builder, associate it with the content stream, and add it to our list.
        var pageBuilder = new PdfPageBuilder(contentStream);
        _pageBuilders.Add(pageBuilder);

        return pageBuilder;
    }
}