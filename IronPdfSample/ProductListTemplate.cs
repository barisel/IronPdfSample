using System.IO;
using System.Data;
using System.Linq;
using IronPdf;

namespace GcPdfWeb.Samples
{

    public class ProductListTemplate
    {
        public void CreatePDF()
        {
            using (var ds = new DataSet())
            {
                // Fetch data:
                ds.ReadXml(@"C:\Users\barisel\source\repos\IronPdfSample\IronPdfSample\Resources\data\GcNWind.xml");

                DataTable dtProds = ds.Tables["Statement"];

                var accounting =
                    from prod in dtProds.Select()
                    orderby prod["AccountSuffix"]
                    select new
                    {
                        TranDate = prod["TranDate"],
                        AccountNumber = prod["AccountNumber"],
                        AccountSuffix = prod["AccountSuffix"],
                        CurrencyCode = prod["CurrencyCode"],
                        Explanation = prod["Explanation"],
                        DebitAmount = $"{prod["DebitAmount"]} TRY",
                        CreditAmount = $"{prod["CreditAmount"]} TRY",
                        Balance = $"{prod["Balance"]} TRY",
                        CustomerName = prod["CustomerName"]
                    };
                var headerField = new { Name = "BARIŞ ELVANOĞLU", Address = "SULTAN ORHAN MAH.KIŞLA CD. 1136 SK." };

                var template = File.ReadAllText(@"C:\Users\barisel\source\repos\IronPdfSample\IronPdfSample\Resources\Misc\ProductListTemplate.html");
                
                var builder = new Stubble.Core.Builders.StubbleBuilder();
                var boundTemplate = builder.Build().Render(template, new { Query = accounting, Header = headerField });
                var tmp = Path.GetTempFileName();

                var Renderer = new HtmlToPdf();
                Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
                Renderer.PrintOptions.MarginTop = 50;  //millimeters
                Renderer.PrintOptions.MarginBottom = 50;
                Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Print;

                Renderer.PrintOptions.Footer = new SimpleHeaderFooter() {
                    RightText = "Sayfa {page}",
                    DrawDividerLine = true,
                    FontSize = 10
                };


                var PDF = Renderer.RenderHtmlAsPdf(boundTemplate);
                Renderer.PrintOptions.PrintHtmlBackgrounds = true;
                
                var OutputPath = "HtmlToPDF.pdf";
                PdfDocument.Merge(new PdfDocument(@"C:\Users\barisel\source\repos\IronPdfSample\IronPdfSample\bin\Debug\netcoreapp3.1\IronPdfExample.pdf"), PDF).SaveAs("Combined.Pdf");
                //PDF.SaveAs(OutputPath);
                //C:\Users\barisel\source\repos\IronPdfSample\IronPdfSample\bin\Debug\netcoreapp3.1
                
            }
        }
    }
}
