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
using System.Threading;

namespace EspinhoAI
{
    public partial class DocumentViewModel : ObservableObject, IQueryAttributable
    {
        static string _pathFolder = "/Users/ruimarinho/Library/Containers/com.companyname.espinhoai/Data/Documents/fotos/";

        readonly Repository _repository;
        public DocumentViewModel(Repository repository)
        {
            _repository = repository;
            Images = new ObservableCollection<ImageSource>();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Doc = query["doc"] as Doc;
            GetDocPages();  
        }

        void GetDocPages()
        {
            var filename = Path.GetFileNameWithoutExtension(Doc.FileName);

            if (filename == null || Doc.Path == null)
                return;

            string folder = GetFolderForDoc(filename);
            var existingPages = _repository.DocPages().Where(ff => ff.PdfPath == Doc.Path);

            if (existingPages != null && existingPages.Count() > 0)
            {
                DocPages = new ObservableCollection<DocPage>(existingPages);
               // CurrentDocPage = DocPages.FirstOrDefault();
            }

            LoadExistingImages(folder);

          

            // //extract images from pdf
            // ExtractAllImages(CurrentDoc.Path, folder);
        }

        [RelayCommand]
        void LoadDocPages()
        {
            var filename = Path.GetFileNameWithoutExtension(Doc.FileName);

            if (filename == null || Doc.Path == null)
                return;

            string folder = GetFolderForDoc(filename);
            //using the pdf to image
             PdfPagesToImages(Doc.Path, folder);
        }

        //Save each pdf page as a image
        async Task PdfPagesToImages(string pdfPath, string folder, double dpiX = 100, double dpiY = 100, CancellationToken token = default)
        {
            string xfin = Path.Combine(folder, "xfin");
            if (!Directory.Exists(xfin))
            {
                Directory.CreateDirectory(xfin);
                PdfFixedDocument doc = new PdfFixedDocument(pdfPath);
                int c = 1;
                foreach (var item in doc.Pages)
                {
                    var filePath = $"{xfin}/page{c}.png";

                    PdfPageRenderer renderer = new PdfPageRenderer(item);
                    using var stream = new MemoryStream();
                    var pageImage = renderer.ConvertPageToImage(stream, PdfPageImageFormat.Png, new PdfRendererSettings(dpiX, dpiY));
                    await File.WriteAllBytesAsync($"{xfin}/page{c}.png", stream.ToArray(), token);

                    DocPage docPage = new DocPage();
                    docPage.Id = Utils.GetDeterministicHashCode(filePath);
                    docPage.PageNumber = c;
                    docPage.Path = filePath;
                    docPage.Publication = Doc.Publication;
                    docPage.PdfPath = pdfPath;
                    docPage.CreatedDate = DateTime.Now;
                    _repository.Create(docPage);
                    DocPages.Add(docPage);
                    c++;
                }
            }
            LoadExistingImages(xfin);
        }

        void LoadExistingImages(string folder)
        {
            var folderToSearch = folder;
            if (!Directory.Exists(folder))
                return;
            string xfin = Path.Combine(folder, "xfin");
            if (Directory.Exists(xfin) && !folder.Contains("xfin"))
                folderToSearch = xfin;
            DirectoryInfo dir = new DirectoryInfo(folderToSearch);
            var images = dir.EnumerateFiles().OrderBy(b => b.CreationTime).Select(b => b.FullName);
            //fallback to previous
            if (images.Count() == 0 && folderToSearch != folder)
            {
                dir = new DirectoryInfo(folder);
                images = dir.EnumerateFiles().OrderBy(b => b.CreationTime).Select(b => b.FullName);
            }
            foreach (var image in images)
            {
                if (image.EndsWith(".jpg") || image.EndsWith(".png") || image.EndsWith(".jpeg"))
                    Images.Add(image);
            }
            CurrentImage = Images.FirstOrDefault();
        }

        

        static string GetFolderForDoc(string fileName)
        {
            string folder = Path.Combine(_pathFolder, Path.GetFileNameWithoutExtension(fileName));
            Directory.CreateDirectory(folder);
            return folder;
        }

        [ObservableProperty]
        DocPage? _currentDocPage;

        [ObservableProperty]
        ImageSource? _currentImage;

        [ObservableProperty]
        ObservableCollection<ImageSource>? _images;

        [ObservableProperty]
        ObservableCollection<DocPage>? _docPages = new ObservableCollection<DocPage>();

        [ObservableProperty]
        Doc? _doc;

        [ObservableProperty]
        bool _isBusy;
    }
}