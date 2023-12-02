using CommunityToolkit.Mvvm.ComponentModel;
using EspinhoAI.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Linq;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System;
using iText.Commons.Utils;
using System.Threading.Tasks;

namespace EspinhoAI
{
    public partial class ExtractViewModel : ObservableObject
    {

        readonly Repository _repository;

        const string biblioUrl = "https://bibliotecamunicipal.espinho.pt";

        public ExtractViewModel()
        {
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
            CurrentDoc = Docs.FirstOrDefault();
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
                    }
                }
            }
        }

        [ObservableProperty]
        ImageSource? _pdfImage;

        [ObservableProperty]
        ObservableCollection<ImageSource>? _Images;

        [ObservableProperty]
        int _scrappedCount = 0;

        [ObservableProperty]
        ObservableCollection<Doc>? _docs = new ObservableCollection<Doc>();

        [RelayCommand]
        void GetImageFromPdf()
        {
            if (CurrentDoc == null)
                return;
            string pathFolder = "/Users/ruimarinho/Library/Containers/com.companyname.espinhoai/Data/Documents/fotos/";
            var filename = Path.GetFileNameWithoutExtension(CurrentDoc.FileName);
            string folder = Path.Combine(pathFolder, filename);
            Directory.CreateDirectory(folder);

            var extractor = new MyPdfImageExtractor(CurrentDoc.Path);
            extractor.ExtractToDirectory(folder);

            var images = Directory.EnumerateFiles(folder);
            foreach (var image in images)
            {
                Images.Add(image);
            }
            PdfImage = Images.FirstOrDefault();
        }

        [RelayCommand]
        async Task GetTextFromImage()
        {
            //use your `key` and `endpoint` environment variables to create your `AzureKeyCredential` and `DocumentAnalysisClient` instances
            string key = "";
            string endpoint = "https://paper-ocr.cognitiveservices.azure.com/";
            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            //sample document
            //  Uri fileUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/rest-api/read.png");

            //  AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-read", fileUri);
            var immm = PdfImage.ToString().Replace("File:", "").Trim();
            ImageProcessingResult? image = null;
            try
            {
                var b  = await File.ReadAllBytesAsync(immm);

                image = new  ImageProcessingResult(b);
            }
            catch (Exception ex)
            {

            }

            if (image?.Image == null)
                return;

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", new MemoryStream(image.Image));
            AnalyzeResult result = operation.Value;

            foreach (DocumentPage page in result.Pages)
            {
                Console.WriteLine($"Document Page {page.PageNumber} has {page.Lines.Count} line(s), {page.Words.Count} word(s),");
                Console.WriteLine($"and {page.SelectionMarks.Count} selection mark(s).");

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
                //    Console.WriteLine($"      Point {j} => X: {paragraph.BoundingRegions[j].BoundingPolygon}, Y: {paragraph.BoundingRegions[j].BoundingPolygon}");
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

            

        }

    }

}
