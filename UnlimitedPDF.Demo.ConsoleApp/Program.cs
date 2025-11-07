using UnlimitedPDF.Models;

Console.WriteLine("Start: Unlimited PDF");

var pdfFilePath = @"D:\\Minimal.pdf";

var pdfDocument = new PdfDocument();

pdfDocument.Write(pdfFilePath);

Console.WriteLine("End: Unlimited PDF");

Console.ReadLine();