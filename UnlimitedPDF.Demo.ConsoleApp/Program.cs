// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

var path = "Minimal.pdf";

using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
using var writer = new StreamWriter(fs, Encoding.ASCII);

// PDF Header
writer.WriteLine("%PDF-1.4");

// 1. Catalog object
long pos1 = fs.Position;
writer.WriteLine("1 0 obj");
writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
writer.WriteLine("endobj");

// 2. Pages object
long pos2 = fs.Position;
writer.WriteLine("2 0 obj");
writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
writer.WriteLine("endobj");

// 3. Page object
long pos3 = fs.Position;
writer.WriteLine("3 0 obj");
writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << >> >>");
writer.WriteLine("endobj");

// 4. Content stream
string content = "BT /F1 24 Tf 100 700 Td (hello from Unlimited PDF library) Tj ET";
byte[] contentBytes = Encoding.ASCII.GetBytes(content);
long pos4 = fs.Position;
writer.WriteLine("4 0 obj");
writer.WriteLine("<< /Length " + contentBytes.Length + " >>");
writer.WriteLine("stream");
writer.Flush();
fs.Write(contentBytes, 0, contentBytes.Length);
writer.WriteLine("\nendstream");
writer.WriteLine("endobj");

// 5. Font object (define built-in Helvetica)
long pos5 = fs.Position;
writer.WriteLine("5 0 obj");
writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
writer.WriteLine("endobj");

// Update page resource dictionary to include font
// (minimal example won't handle cross references here)

// XRef position
long xrefPos = fs.Position;
writer.WriteLine("xref");
writer.WriteLine("0 6");
writer.WriteLine("0000000000 65535 f ");
writer.WriteLine($"{pos1:D10} 00000 n ");
writer.WriteLine($"{pos2:D10} 00000 n ");
writer.WriteLine($"{pos3:D10} 00000 n ");
writer.WriteLine($"{pos4:D10} 00000 n ");
writer.WriteLine($"{pos5:D10} 00000 n ");

// Trailer
writer.WriteLine("trailer");
writer.WriteLine("<< /Root 1 0 R /Size 6 >>");
writer.WriteLine("startxref");
writer.WriteLine(xrefPos);
writer.WriteLine("%%EOF");