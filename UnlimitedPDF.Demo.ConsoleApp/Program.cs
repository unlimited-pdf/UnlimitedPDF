using UnlimitedPDF;
using UnlimitedPDF.Models.Table;
using System;

Console.WriteLine("Start: Unlimited PDF");

var pdfFilePath = @"D:\\unlimited-pdf-sample.pdf";

// Use a 'using' statement for proper resource management with IDisposable
var pdfDocument = new PdfDocument();

// --- Page 1: Invoice ---
var page1 = pdfDocument.AddPage();

// 1. Invoice Header
page1.AddText("INVOICE", 250, 800, 20, "F1");

// Seller and Buyer Details
page1.AddText("Seller:", 50, 760, 10, "F1");
page1.AddText("ABC Electronics Pvt. Ltd.", 50, 748, 10, "F1");
page1.AddText("123 Tech Park, Bangalore, KA 560001", 50, 736, 10, "F1");
page1.AddText("GSTIN: 29ABCDE1234F1Z5", 50, 724, 10, "F1");

page1.AddText("Buyer:", 350, 760, 10, "F1");
page1.AddText("Vikram Gaddam", 350, 748, 10, "F1");
page1.AddText("456 Code Street, Hyderabad, TS 500081", 350, 736, 10, "F1");

// Invoice Metadata
page1.AddText($"Invoice No: INV-{DateTime.Now:yyyy-MM-dd}-001", 50, 690, 10, "F1");
page1.AddText($"Date: {DateTime.Now:dd-MMM-yyyy}", 50, 678, 10, "F1");

// 2. Items Table
var itemsTable = page1.AddTable(50, 650, 512);
itemsTable.SetColumnWidths(40, 210, 60, 100, 102);

// Table Headers
var headerRow = itemsTable.AddRow();

var sNoCell = headerRow.GetCell(0);
sNoCell.Text = "S.No.";
sNoCell.Background = PdfColor.LightGray;

var descCell = headerRow.GetCell(1);
descCell.Text = "Item Description";
descCell.Background = PdfColor.LightGray;

var qtyCell = headerRow.GetCell(2);
qtyCell.Text = "Qty";
qtyCell.Background = PdfColor.LightGray;

var rateCell = headerRow.GetCell(3);
rateCell.Text = "Rate (INR)";
rateCell.Background = PdfColor.LightGray;

var amountCell = headerRow.GetCell(4);
amountCell.Text = "Amount (INR)";
amountCell.Background = PdfColor.LightGray;

// Item 1
var item1Row = itemsTable.AddRow();
item1Row.GetCell(0).Text = "1";
item1Row.GetCell(1).Text = "Wireless Keyboard";
item1Row.GetCell(2).Text = "2";
item1Row.GetCell(3).Text = "1250.00";
item1Row.GetCell(4).Text = "2500.00";

// Item 2
var item2Row = itemsTable.AddRow();
item2Row.GetCell(0).Text = "2";
item2Row.GetCell(1).Text = "USB-C Docking Station";
item2Row.GetCell(2).Text = "1";
item2Row.GetCell(3).Text = "4500.00";
item2Row.GetCell(4).Text = "4500.00";

// 3. Totals Section
double subtotal = 7000.00;
double cgst = subtotal * 0.09; // 9% CGST
double sgst = subtotal * 0.09; // 9% SGST
double total = subtotal + cgst + sgst;

var totalsTable = page1.AddTable(350, 550, 212);
totalsTable.SetColumnWidths(110, 102);

totalsTable.GetOrCreateCell(0, 0).Text = "Subtotal";
totalsTable.GetOrCreateCell(0, 1).Text = subtotal.ToString("F2");

totalsTable.GetOrCreateCell(1, 0).Text = "CGST @ 9%";
totalsTable.GetOrCreateCell(1, 1).Text = cgst.ToString("F2");

totalsTable.GetOrCreateCell(2, 0).Text = "SGST @ 9%";
totalsTable.GetOrCreateCell(2, 1).Text = sgst.ToString("F2");

var totalCell = totalsTable.GetOrCreateCell(3, 0);
totalCell.Text = "Total";
totalCell.Background = PdfColor.LightGray;

var totalValueCell = totalsTable.GetOrCreateCell(3, 1);
totalValueCell.Text = total.ToString("F2");
totalValueCell.Background = PdfColor.LightGray;

// 4. Terms and Conditions
page1.AddText("Terms & Conditions:", 50, 450, 10, "F1");
page1.AddText("1. Payment due within 30 days.", 50, 438, 9, "F1");
page1.AddText("2. Goods once sold will not be taken back.", 50, 426, 9, "F1");

// --- Page 2: Other Content ---
var page2 = pdfDocument.AddPage();
page2.AddText("This is the first line on the second page.", 50, 750, 12);

// --- Page 3: Other Content ---
var page3 = pdfDocument.AddPage();
page3.AddText("This is the first line on the third page.", 50, 750, 12);

pdfDocument.Write(pdfFilePath);


Console.WriteLine("End: Unlimited PDF");
Console.ReadLine();