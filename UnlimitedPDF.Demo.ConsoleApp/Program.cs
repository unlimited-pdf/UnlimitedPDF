using UnlimitedPDF.Models;

Console.WriteLine("Start: Unlimited PDF");

var pdfFilePath = @"D:\\Minimal.pdf";

var pdfDocument = new PdfDocument();

// Create content for the first page
var pageContent1 = new PdfPageContent();
pageContent1.AddText("Hello from Unlimited PDF library, this is the first line on the first page.", 50, 750, 12);
pageContent1.AddText("And this is the second line, placed right below.", 50, 730, 12);

// Add the first page with its content to the document
pdfDocument.AddPageFromContent(pageContent1);

// Create content for the second page
var pageContent2 = new PdfPageContent();
pageContent2.AddText("This is the first line on the second page.", 50, 750, 12);

// Add the second page with its content to the document
pdfDocument.AddPageFromContent(pageContent2);

pdfDocument.Write(pdfFilePath);

Console.WriteLine("End: Unlimited PDF");

Console.ReadLine();