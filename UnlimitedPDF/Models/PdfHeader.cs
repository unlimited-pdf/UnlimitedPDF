namespace UnlimitedPDF.Models;

public class PdfHeader
{
    public string Version { get; set; } = "1.7";

    public override string ToString()
    {
        return $"%PDF-{Version}";
    }
}
