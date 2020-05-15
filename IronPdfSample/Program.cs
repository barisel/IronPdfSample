using System;
using GcPdfWeb.Samples;
using HtmlToPdfTutorial;
using IronPdf;

namespace IronPdfSample
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductListTemplate template = new ProductListTemplate();
            template.CreatePDF();
            template.WatermarkTst();
        }
    }
}
