using UnlimitedPDF.Models;

Console.WriteLine("Start: Unlimited PDF");

var pdfFilePath = @"D:\\Minimal.pdf";

var pdfDocument = new PdfDocument();

// Add the first page and add content to it.
var page1 = pdfDocument.AddPage();
page1.AddText("Hello from Unlimited PDF library, this is the first line on the first page.", 50, 750, 12);
page1.AddText("And this is the second line, placed right below.", 50, 730, 12);

// Add the second page and add content to it.
var page2 = pdfDocument.AddPage();
page2.AddText("This is the first line on the second page.", 50, 750, 12);

pdfDocument.Write(pdfFilePath);

Console.WriteLine("End: Unlimited PDF");

Console.ReadLine();