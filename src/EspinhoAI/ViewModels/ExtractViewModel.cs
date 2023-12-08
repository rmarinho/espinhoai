using CommunityToolkit.Mvvm.ComponentModel;
using EspinhoAI.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Linq;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using EspinhoAI.Services;
using System.Collections.Generic;
using Xfinium.Pdf;
using Xfinium.Pdf.Rendering;
using Xfinium.Pdf.Graphics;

namespace EspinhoAI
{
    public partial class ExtractViewModel : ObservableObject
    {

        readonly Repository _repository;

        const string biblioUrl = "https://bibliotecamunicipal.espinho.pt";
        IConfiguration _config;
        AzureService _azureService;
        PdfTextService _pdfTextService;
        static string pathFolder = "/Users/ruimarinho/Library/Containers/com.companyname.espinhoai/Data/Documents/fotos/";


        public ExtractViewModel(IConfiguration config, AzureService azureService, PdfTextService pdfTextService)
        {
            _config = config;
            _azureService = azureService;
            _pdfTextService = pdfTextService;

            //  Url = "https://bibliotecamunicipal.espinho.pt/pt/documentacao/defesa-de-espinho/2023/";
            _repository = new Repository();
            Images = new ObservableCollection<ImageSource>();
            Docs = new ObservableCollection<Doc>(_repository.Docs());
            Docs.Add(new Doc
            {
                Year = "2023",
                Month = "05",
                Day = "25",
                FileName = "4751_25_05_2023_42421246564706dda77f98.pdf",

                Path = $"/Users/ruimarinho/Library/Containers/com.companyname.espinhoai/Data/Documents/fotos/documentos/4751_25_05_2023_42421246564706dda77f98.pdf"
            });
            CurrentDoc = Docs.LastOrDefault();

        }

        string? _url;
        public string? Url
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    SetProperty(ref _url, value);
                    OnPropertyChanged(nameof(PdfSource));
                }
            }
        }

        public string? PdfSource => Url;

        Doc? _currentDoc;
        public Doc? CurrentDoc
        {
            get { return _currentDoc; }
            set
            {
                if (_currentDoc != value)
                {
                    SetProperty(ref _currentDoc, value);
                    if (CurrentDoc != null)
                    {
                        Url = CurrentDoc.Path;
                        var filename = Path.GetFileNameWithoutExtension(CurrentDoc.FileName);
                        if (filename == null || CurrentDoc.Path == null)
                            return;

                        string folder = GetFolderForDoc(filename);
                        LoadExistingImages(folder);
                    }
                }
            }
        }


        [ObservableProperty]
        ImageSource? _currentImage;

        [ObservableProperty]
        ObservableCollection<ImageSource>? _images;

        [ObservableProperty]
        ObservableCollection<DocumentParagraph>? _paragraphs;

        [ObservableProperty]
        double _pageWidth;

        [ObservableProperty]
        double _pageHeight;

        [ObservableProperty]
        int _scrappedCount = 0;

        [ObservableProperty]
        ObservableCollection<Doc>? _docs = new ObservableCollection<Doc>();

        [RelayCommand]
        async Task GetTextFromPdf()
        {
            if (CurrentDoc == null)
                return;
            var folder = GetFolderForDoc(CurrentDoc.FileName);
            await ExtractPdfPageText(CurrentDoc.Path, folder);

        }

        [RelayCommand]
        async Task GetImageFromPdf()
        {
            if (CurrentDoc == null)
                return;

            CurrentImage = null;
            Images.Clear();
          
            var folder = GetFolderForDoc(CurrentDoc.FileName);
            await PdfToImage(CurrentDoc.Path, folder);

            ExtractAllImages(CurrentDoc.Path, folder);
        }

        [RelayCommand]
        async Task GetTextFromImage()
        {
            var imageFilePath = CurrentImage.ToString().Replace("File:", "").Trim();

            var pathFolder = Path.GetDirectoryName(imageFilePath);
            var filename = Path.GetFileNameWithoutExtension(imageFilePath);
            string finalPathFile = Path.Combine(pathFolder, $"{filename}.json");

            AnalyzeResult? result = null;
            if (File.Exists(finalPathFile))
            {
                var json = await File.ReadAllTextAsync(finalPathFile);
                try
                {
                    result = json.ToAnalyzeResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting result from file {ex.Message}");
                }
            }
            else
            {
                var analyze = await AnalyzeImage(imageFilePath);
                if (analyze.Result == null)
                {
                    Console.WriteLine($"Document analysis failed.");
                    return;
                }
                result = analyze.Result;
                Console.WriteLine("Saving result to 'result.json'...");
                //Save the result to a JSON file
                await File.WriteAllTextAsync(finalPathFile, analyze.Content);
            }
            if (result == null)
                throw new Exception("result is null");
            ProcessResult(result);
        }

        static string GetFolderForDoc(string fileName)
        {
            string folder = Path.Combine(pathFolder, fileName);
            Directory.CreateDirectory(folder);
            return folder;
        }

        //Save each pdf page as a image
        async Task PdfToImage(string pdfPath, string folder, double dpiX = 100, double dpiY = 100)
        {
            string xfin = Path.Combine(folder, "xfin");
            if (Directory.Exists(folder))
            {
                LoadExistingImages(xfin);
            }
            else
            {
                Directory.CreateDirectory(xfin);
                PdfFixedDocument doc = new PdfFixedDocument(pdfPath);
                int c = 0;
                foreach (var item in doc.Pages)
                {
                    c++;
                    PdfPageRenderer renderer = new PdfPageRenderer(item);
                    using var stream = new MemoryStream();
                    var pageImage = renderer.ConvertPageToImage(stream, PdfPageImageFormat.Png, new PdfRendererSettings(dpiX, dpiY));
                    await File.WriteAllBytesAsync($"{xfin}/page{c}.png", stream.ToArray());
                }
            }
        }

        void LoadExistingImages(string folder)
        {
            var folderToSearch = folder;
            if (!Directory.Exists(folder))
                return;
            string xfin = Path.Combine(folder, "xfin");
            if (Directory.Exists(xfin) && !folder.Contains("xfin"))
                folderToSearch = xfin;
            var images = Directory.EnumerateFiles(folderToSearch);
            //fallback to previous
            if (images.Count() == 0 && folderToSearch != folder)
            {
                images = Directory.EnumerateFiles(folder);
            }
            foreach (var image in images)
            {
                if(image.EndsWith(".jpg") || image.EndsWith(".png") || image.EndsWith(".jpeg"))
                    Images.Add(image);
            }
            CurrentImage = Images.FirstOrDefault();
        }

        async Task ExtractPdfPageText(string pdfPath, string folder)
        {
            await _pdfTextService.ExtractText(pdfPath, folder);
        }

        void ExtractAllImages(string pdfPath, string folder)
        {
            var extractor = new MyPdfImageExtractor(pdfPath);
            extractor.ExtractToDirectory(folder);

            LoadExistingImages(folder);
        }

        async Task<(string Content, AnalyzeResult Result)> AnalyzeImage(string imageFilePath)
        {
            ImageProcessingResult? image = null;
            try
            {
                var b = await File.ReadAllBytesAsync(imageFilePath);

                image = new ImageProcessingResult(b);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return await _azureService.GetAzureAnalyzeResult(image?.Image);
        }

       
        void ProcessResult(AnalyzeResult result)
        {
            Console.WriteLine($"AnalyzeResult Pages:{result.Pages.Count()}");
            foreach (DocumentPage page in result.Pages)
            {
                Console.WriteLine($"Document Page {page.PageNumber} has {page.Lines.Count} line(s), {page.Words.Count} word(s),");
                Console.WriteLine($"Page Width: {page.Width} Height: {page.Height}");
                Console.WriteLine($"and {page.SelectionMarks.Count} selection mark(s).");
                PageWidth = (double)page.Width;
                PageHeight = (double)page.Height;
                for (int i = 0; i < page.Lines.Count; i++)
                {
                    DocumentLine line = page.Lines[i];
                    Console.WriteLine($"  Line {i} has content: '{line.Content}'.");

                    Console.WriteLine($"    Its bounding polygon (points ordered clockwise):");

                    for (int j = 0; j < line.BoundingPolygon.Count; j++)
                    {
                        Console.WriteLine($"      Point {j} => X: {line.BoundingPolygon[j].X}, Y: {line.BoundingPolygon[j].Y}");
                    }
                }
            }

            foreach (DocumentParagraph paragraph in result.Paragraphs)
            {
                Console.WriteLine($"paragraph.Content {paragraph.Content}");
                for (int j = 0; j < paragraph.BoundingRegions.Count; j++)
                {
                }
            }

            foreach (DocumentStyle style in result.Styles)
            {
                // Check the style and style confidence to see if text is handwritten.
                // Note that value '0.8' is used as an example.

                bool isHandwritten = style.IsHandwritten.HasValue && style.IsHandwritten == true;

                if (isHandwritten && style.Confidence > 0.8)
                {
                    Console.WriteLine($"Handwritten content found:");

                    foreach (DocumentSpan span in style.Spans)
                    {
                        Console.WriteLine($"  Content: {result.Content.Substring(span.Index, span.Length)}");
                    }
                }
            }

            Console.WriteLine("Detected languages:");

            foreach (DocumentLanguage language in result.Languages)
            {
                Console.WriteLine($"  Found language with locale'{language.Locale}' with confidence {language.Confidence}.");
            }

            Paragraphs = new ObservableCollection<DocumentParagraph>(result.Paragraphs);
        }
    }

    public static class ImageExtensions
    {
        public static string ToJson(this AnalyzeResult result)
        {
            return JsonSerializer.Serialize(result);
        }

        public static AnalyzeResult? ToAnalyzeResult(this string json)
        {

            var jsonElement = JsonDocument.Parse(json).RootElement;
            var jsonElementAnalyzeResult = jsonElement.GetProperty("analyzeResult");

            var methodInfo = typeof(AnalyzeResult).GetMethod("DeserializeAnalyzeResult", BindingFlags.NonPublic | BindingFlags.Static);
            var analyzeResult = methodInfo?.Invoke(null, new object[] { jsonElementAnalyzeResult }) as AnalyzeResult;
            return analyzeResult;
        }

        public static string ToJson(this UglyToad.PdfPig.Content.Page page)
        {
            return JsonSerializer.Serialize(page);
        }
    }

}
