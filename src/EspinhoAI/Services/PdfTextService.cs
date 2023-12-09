using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig.DocumentLayoutAnalysis.Export;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.Util;

namespace EspinhoAI.Services
{
    public class PdfTextService
    {
        SvgTextExporter exporter = new SvgTextExporter();
        HOcrTextExporter hocrTextExporter = new HOcrTextExporter(DefaultWordExtractor.Instance, DocstrumBoundingBoxes.Instance);
        PageXmlTextExporter pageXmlTextExporter = new PageXmlTextExporter(
                     DefaultWordExtractor.Instance,
                     RecursiveXYCut.Instance,
                     UnsupervisedReadingOrderDetector.Instance);

        readonly ILogger _logger;
        public PdfTextService(ILogger<PdfTextService> logger)
        {
            _logger = logger;
        }

        public async Task<Models.OCR.PcGts?> GetOCRPage(string path, int page)
        {
            Models.OCR.PcGts? result = null;
            string folder1 = Path.Combine(path, "pdf_pig_page");
            if (Directory.Exists(folder1))
            {
                var xmlData = await File.ReadAllTextAsync($"{folder1}/page{page}/document.pagexml.xml");

                try
                {
                    var serializer = new XmlSerializer(typeof(Models.OCR.PcGts));

                    using (var reader = new StringReader(xmlData))
                    {
                        result = serializer.Deserialize(reader) as Models.OCR.PcGts;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Geting OCR PAGE xml");
                }
            }
            return result;
        }

        public async Task ExtractText(string filePath, string root)
        {
            string folder1 = Path.Combine(root, "pdf_pig_page");
            if (Directory.Exists(folder1) && Directory.Exists(Path.Combine(folder1, $"page0")))
            {
                return;
            }
            using (var document = UglyToad.PdfPig.PdfDocument.Open(filePath))
            {
                Directory.CreateDirectory(folder1);

                for (var i = 1; i < document.NumberOfPages; i++)
                {
                    var pageDirectory = Directory.CreateDirectory(Path.Combine(folder1, $"page{i}"));
                    var page = document.GetPage(i + 1);
                    var images = page.GetImages();
                    for (int f = 0; f < images.Count(); f++)
                    {
                        var im = images.ElementAt(f);
                        byte[] irm;
                        //  IReadOnlyList<byte> irm2;
                        if (im.TryGetPng(out irm))
                            await File.WriteAllBytesAsync($"{pageDirectory}/image{f}.png", irm);
                        //else if (im.TryGetBytes(out irm2))
                        //    await File.WriteAllBytesAsync($"{pageDirectory}/image{f}.jpg", irm2.ToArray());
                        //else
                        //    await File.WriteAllBytesAsync($"{pageDirectory}/image{f}.jpeg", im.RawBytes.ToArray());
                    }

                    // Either extract based on order in the underlying document with newlines and spaces.
                    var text = ContentOrderTextExtractor.GetText(page);

                    try
                    {
                        // Save text to an html file
                        await File.WriteAllTextAsync($"{pageDirectory}/content.txt", text);

                        // Convert page to text
                        string xml = pageXmlTextExporter.Get(page);

                        // Save text to an xml file
                        await File.WriteAllTextAsync($"{pageDirectory}/document.pagexml.xml", xml);

                        // Convert page to text
                        var svg = exporter.Get(page);
                        await File.WriteAllTextAsync($"{pageDirectory}/document.html", svg);

                        // Convert page to text
                        string html = hocrTextExporter.Get(page);

                        // Save text to an html file
                        await File.WriteAllTextAsync($"{pageDirectory}/document.hocr.html", html);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error ExtractText");
                    }
                }
            }
        }
    }
}

