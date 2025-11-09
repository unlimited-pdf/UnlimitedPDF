using UnlimitedPDF;

Console.WriteLine("Start: Unlimited PDF");

var pdfFilePath = @"D:\\unlimited-pdf-sample.pdf";

var pdfDocument = new PdfDocument();

// Add the first page and add content to it.
var page1 = pdfDocument.AddPage();
page1.AddText("UnlimitedPDF library - Sample PDF", 50, 750, 24);
page1.AddText("This is the first line on the first page.", 50, 730, 12);
page1.AddText("This is the second line on the first page.", 50, 710, 12);

// Add the second page and add content to it.
var page2 = pdfDocument.AddPage();
page2.AddText("This is the first line on the second page.", 50, 750, 12);

// Add the third page and add content to it.
var page3 = pdfDocument.AddPage();
page3.AddText("This is the first line on the third page.", 50, 750, 12);

pdfDocument.Write(pdfFilePath);

Console.WriteLine("End: Unlimited PDF");

Console.ReadLine();