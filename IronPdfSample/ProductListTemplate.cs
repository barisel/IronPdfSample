using System.IO;
using System.Data;
using System.Linq;
using IronPdf;
using System.Security.Cryptography.X509Certificates;

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
                Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Print;
                Renderer.PrintOptions.CustomCssUrl = "https://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css";
                //Set the width of the resposive virtual browser window in pixels
                //Renderer.PrintOptions.MarginBottom = 0;
                

                Renderer.PrintOptions.Footer = new SimpleHeaderFooter() {
                    RightText = "Sayfa {page}",
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

        // kaşe veya imzalar için kullanılacak.
        public void WatermarkTst()
        {
            IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
            var pdf = Renderer.RenderUrlAsPdf("https://www.nuget.org/packages/IronPdf");
            pdf.WatermarkAllPages("<h2 style='color:red'>SAMPLE</h2>", PdfDocument.WaterMarkLocation.MiddleCenter, 50, -45, "https://www.nuget.org/packages/IronPdf");
            pdf.SaveAs(@"C:\tmp\document\Watermarked.pdf");
        }
    }
}
