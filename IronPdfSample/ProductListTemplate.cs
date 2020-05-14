//
// This code is part of https://www.grapecity.com/documents-api-pdf/demos.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

                // Load the template - HTML file with {{mustache}} data references:
                var template = File.ReadAllText(@"C:\Users\barisel\source\repos\IronPdfSample\IronPdfSample\Resources\Misc\ProductListTemplate.html");
                // Bind the template to data:
                var builder = new Stubble.Core.Builders.StubbleBuilder();
                var boundTemplate = builder.Build().Render(template, new { Query = accounting });
                var tmp = Path.GetTempFileName();
                // Render the bound HTML:

                var Renderer = new HtmlToPdf();

                // Settings 
                Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
                Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen;
                //Renderer.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Portrait;
                //Renderer.PrintOptions.Header = new SimpleHeaderFooter() { CenterText = "Iron PDf C# Html to PDF Example", FontSize = 10, FontFamily = "Arial" };
                //Renderer.PrintOptions.Footer = new HtmlHeaderFooter() { HtmlFragment = "<div style='text-align:right'><em style='color:#333'>page {page} of {total-pages}</em></div>" };

                // Render the HTML as a PDF
                var PDF = Renderer.RenderHtmlAsPdf(boundTemplate);

                //  Editing the PDF by adding a watermark
                //PDF.WatermarkAllPages("<span style='color:red; font-size:44px; font-family:Arial'>Sample</example>", PdfDocument.WaterMarkLocation.MiddleCenter, 20, -45, "http://ironpdf.com");

                //  Save the PDF to a file
                var OutputPath = "HtmlToPDF.pdf";
                PDF.SaveAs(OutputPath);

                //var Renderer = new HtmlToPdf();
                //var PDF = Renderer.RenderHtmlAsPdf(boundTemplate);
                //var OutputPath = "HtmlToPDF.pdf";
                //PDF.SaveAs(OutputPath);
                //C:\Users\barisel\source\repos\IronPdfSample\IronPdfSample\bin\Debug\netcoreapp3.1
            }
            // Done.
        }
    }
}
