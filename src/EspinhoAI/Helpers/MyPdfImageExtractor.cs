using System;
using System.IO;
//using UglyToad.PdfPig.Content;
//using UglyToad.PdfPig;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace EspinhoAI
{
    public class MyPdfImageExtractor
    {
        private readonly string _pdfFileName;

        public MyPdfImageExtractor(string pdfFileName)
        {
            _pdfFileName = pdfFileName;
        }

        public void ExtractToDirectory(string directoryName)
        {
            using (var reader = new PdfReader(_pdfFileName))
            {
                // Avoid iText.Kernel.Crypto.BadPasswordException: https://stackoverflow.com/a/48065052/97803
                reader.SetUnethicalReading(true);

                using (var pdfDoc = new PdfDocument(reader))
                {
                    ExtractImagesOnAllPages(pdfDoc, directoryName);
                }
            }
        }

        void ExtractImagesOnAllPages(PdfDocument pdfDoc, string directoryName)
        {
            Console.WriteLine($"Number of pdf {pdfDoc.GetNumberOfPdfObjects()} objects");

            IEventListener strategy = new ImageRenderListener(Path.Combine(directoryName, @"page{0}.{1}"));
            PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
            for (var i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                parser.ProcessPageContent(pdfDoc.GetPage(i));
            }
        }
    }

}
