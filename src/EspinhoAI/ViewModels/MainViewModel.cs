using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HtmlAgilityPack;
using EspinhoAI.Models;
using System.Collections.Generic;
using System.Threading;
using System;

namespace EspinhoAI;

public partial class MainViewModel : ObservableObject
{
    List<string> _urlsVisited = new List<string>();

    readonly Repository _repository;

    const string biblioUrl = "https://bibliotecamunicipal.espinho.pt";
    public MainViewModel()
    {
        Url = "https://bibliotecamunicipal.espinho.pt/pt/documentacao/defesa-de-espinho/2023/";
        _repository = new Repository();
        TryLoadInitialCollection();
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
                OnPropertyChanged(nameof(WebUrl));
            }
        }
    }

    public string? WebUrl => Url;

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

    ItemScrapped? _currentItem;
    public ItemScrapped? CurrentItem
    {
        get { return _currentItem; }
        set
        {
            if (_currentItem != value)
            {
                SetProperty(ref _currentItem, value);
                if(CurrentItem != null)
                {
                    Url = CurrentItem.Url;
                }
            }
        }
    }

    [ObservableProperty]
    int _scrappedCount = 0;

   [ObservableProperty]
    ObservableCollection<ItemScrapped>? _items = new ObservableCollection<ItemScrapped>();

    [ObservableProperty]
    ObservableCollection<Doc>? _docs = new ObservableCollection<Doc>();

    [RelayCommand]
    void Stop()
    {
        try
        {
            _cts?.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    CancellationTokenSource? _cts;

    [RelayCommand]
    void CopyToClipboard(string link)
    {
        Clipboard.Default.SetTextAsync(link);
    }

    [RelayCommand]
    async Task Start()
    {
        try
        {
            var baseUrl = Url;
            try
            {
                _cts = new CancellationTokenSource();
               
                await Task.Run(async () =>
                {
                    Console.WriteLine($"Loaded: {Items.Count}");
                    await NavigateUrl(baseUrl, _cts.Token);
                    Console.WriteLine($"Visited: {_urlsVisited.Count}");

                }, _cts.Token);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //SerializeObject(Docs, nameof(Docs));
            //SerializeObject(Items, nameof(Items));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    void TryLoadInitialCollection()
    {
        var existingDocs = _repository.Docs();
        if (existingDocs != null && existingDocs.Count > 0)
            Docs = new ObservableCollection<Doc>(existingDocs);
        var existingItems = _repository.Items();
        if (existingItems != null && existingItems.Count > 0)
            Items = new ObservableCollection<ItemScrapped>(existingItems);

        ScrappedCount = Items.Count();
    }

    async Task NavigateUrl(string url, CancellationToken cancellationToken = default)
    {
        if (!ValidateUrl(url))
        {
            return;
        }

        System.Diagnostics.Debug.WriteLine($"Visiting {url}");
  
        _urlsVisited.Add(url);

        var html = string.Empty;
    
        try
        {
            bool exists = Items.FirstOrDefault(x => x.Url == url) != null;
            if (!exists)
            {
                System.Diagnostics.Debug.WriteLine("URL already exists on database");
            }
            if (url.EndsWith(".pdf"))
            {
                await LookPdf(url);
            }
            html = await DownloadHtml(url);
            if(!exists)
            {
                RegisterItem(url);
            }
           

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        if (!string.IsNullOrEmpty(html))
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            // Now you can use XPath or LINQ to navigate and extract data from the HTML
            var nodes = document.DocumentNode.SelectNodes("//a[@href]"); // Example: get all anchor tags with href attribute

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var href = node.GetAttributeValue("href", "");
                    var absoluteUrl = GetAbsoluteUrl(url, href);
                    if (!ValidateUrl(absoluteUrl))
                    {
                        continue;
                    }

                    await Task.Run(async () => await NavigateUrl(absoluteUrl, cancellationToken), cancellationToken);
                }
            }
        }
    }

    void RegisterItem(string url)
    {
        string filePath = GetFilePathForUrl(url);
        var item = new ItemScrapped
        {
            Url = url,
            DateScrapped = DateTime.Now,
            FilePath = filePath,
        };
        Items?.Add(item);
        _repository.Create(item);
        ScrappedCount = Items.Count();
    }

    static string GetFilePathForUrl(string url)
    {
        string cacheDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var filePath = $"{cacheDir}/htmlpages/{Utils.GetDeterministicHashCode(url)}.html";
        if (url.EndsWith(".pdf"))
        {
            var href = url.Replace(biblioUrl, "");
            filePath = $"{cacheDir}{href}";
        }

        return filePath;
    }

    bool ValidateUrl(string absoluteUrl)
    {
        if(absoluteUrl.EndsWith("#"))
        {
            absoluteUrl = absoluteUrl.Replace("#", "");
        }
        if (_urlsVisited.Contains(absoluteUrl))
        {
            return false;
        }
        if (absoluteUrl.EndsWith(".aspx") || !absoluteUrl.StartsWith(biblioUrl))
        {
            return false;
        }
        if (absoluteUrl.Contains("javascript"))
        {
            return false;
        }
        if (absoluteUrl.Contains("mailto"))
        {
            return false;
        }
        if (absoluteUrl.Contains("tel"))
        {
            return false;
        }
        if (absoluteUrl.Contains("facebook"))
        {
            return false;
        }
        if (absoluteUrl.Contains("/documentacao/defesa-de-espinho/") || absoluteUrl.Contains("/fotos/documentos/"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static string GetAbsoluteUrl(string baseUrl, string relativeUrl)
    {
        if (Uri.TryCreate(new Uri(baseUrl), relativeUrl, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }
        return relativeUrl;
    }

    async Task LookPdf(string url)
    {
        Console.WriteLine("Link: " + url);
        if (url.EndsWith(".pdf"))
        {
            var filePath = GetFilePathForUrl(url);

            if (Docs.FirstOrDefault(x => x.Url == url) != null)
            {
                System.Diagnostics.Debug.WriteLine("Doc already exists on database");
                return;
            }

            await DownloadAndSavePdf(url, filePath);
            var filename = Path.GetFileName(filePath);
            
            var doc = new Doc
            {
                Id = Utils.GetDeterministicHashCode(url),
                Url = url,
                Path = filePath,
                Publication = "Defesa de Espinho",
                ScrapDate = DateTime.Now,
                FileName = filename
            };

            string pattern = @"(\d+)_(\d+)_(\d+)_(\d+)_[a-f0-9]+\.pdf";
            Match match = Regex.Match(filename, pattern);

            if (match.Success)
            {
                var year = match.Groups[4].Value;
                var month = match.Groups[3].Value;
                var day = match.Groups[2].Value;
                var id = match.Groups[1].Value;
                Console.WriteLine($"YEar: {year}, Month: {month}, Day: {day}");
                doc.Day = day;
                doc.Month = month;
                doc.Year = year;
                //  doc.Year = year.ToString();
            }

            Docs?.Add(doc);
            _repository.Create(doc);
        }
    }

    async Task DownloadAndSavePdf(string pdfUrl, string filePath)
    {
        var stream = await Utils.DownloadPdf(pdfUrl);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        await File.WriteAllBytesAsync(filePath, stream);
    }


    static async Task<string> DownloadHtml(string url)
    {
        var htmlFilePath = GetFilePathForUrl(url);
        var filename = Path.GetFileName(htmlFilePath);
        if(!Path.Exists(htmlFilePath))
            Directory.CreateDirectory(htmlFilePath.Replace(filename,""));
        if (File.Exists(htmlFilePath))
        {
            // Returl the HTML content of the webpage from disk
            var html = await File.ReadAllTextAsync(htmlFilePath);
            return html;
        }

        using HttpClient client = new();
        try
        {
            // Download the HTML content of the webpage
            var html = await client.GetStringAsync(url);
          
            await File.WriteAllTextAsync(htmlFilePath, html);
            return html; ;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Handle 404 Not Found error
            Console.WriteLine("Page not found: " + url);
            return string.Empty;
        }
    }

    void SerializeObject(object docs, string name)
    {
        var json = JsonSerializer.Serialize(docs);
        Console.WriteLine(json);
        string cacheDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var filePath = $"{cacheDir}/{name}.json";
        var stream = Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes(filePath, stream);
    }

    T DeserializeObject<T>(string name)
    {
        string cacheDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var filePath = $"{cacheDir}/{name}.json";
        if (!File.Exists(filePath))
        {
            return default;
        }
        var stream = File.ReadAllBytes(filePath);
        var json = Encoding.UTF8.GetString(stream);
        return JsonSerializer.Deserialize<T>(json);
    }
}
