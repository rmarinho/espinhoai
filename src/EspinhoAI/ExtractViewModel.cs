using CommunityToolkit.Mvvm.ComponentModel;
using EspinhoAI.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.IO;
using System.Linq;

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
        void GetTextFromImage()
        {

        }

    }

}
