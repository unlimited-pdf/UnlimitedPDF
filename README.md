<h3 align="center">UnlimitedPDF Library</h3>

<p align="center">
A powerful and flexible library for creating, manipulating, and managing PDF documents with unlimited capabilities.
</p>

## Status

[![NuGet](https://img.shields.io/nuget/vpre/UnlimitedPDF)](https://www.nuget.org/packages/blazor.bootstrap/absoluteLatest)
[![NuGet](https://img.shields.io/nuget/dt/UnlimitedPDF.svg)](https://www.nuget.org/packages/blazor.bootstrap/absoluteLatest)

## Table of contents

### [Install](#install)

- **.NET CLI**
  
  Install with [NuGet](https://www.nuget.org/): `dotnet add package UnlimitedPDF --version 0.0.2`
  
- **Package Reference**
  
  Add package reference to project: `<PackageReference Include="UnlimitedPDF" Version="0.0.2" />`

## Sample Code

```cs

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
```

## Screenshot

<img width="931" height="829" alt="image" src="https://github.com/user-attachments/assets/c8e0f04d-b0e3-4046-bb25-e9bcc8f316aa" />
