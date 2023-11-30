using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EspinhoAI.Models;
using HtmlAgilityPack;

namespace EspinhoAI;

public partial class MainViewModel : ObservableObject
{
    string? _url;
    ObservableCollection<string>? _yourItemsSource;

    const string biblioUrl = "https://bibliotecamunicipal.espinho.pt";
    public MainViewModel()
    {
        Items = new ObservableCollection<string>();
        Url = "https://bibliotecamunicipal.espinho.pt/pt/documentacao/defesa-de-espinho/2023/";
        Items = DeserializeObject<ObservableCollection<string>>(nameof(Items)) ?? new ObservableCollection<string>();
        Docs = DeserializeObject<ObservableCollection<Doc>>(nameof(Docs)) ?? new ObservableCollection<Doc>();
    }

    public string? Url
    {
        get { return _url; }
        set
        {
            if (_url != value)
            {
                _url = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WebUrl));
            }
        }
    }

    public string? WebUrl => Url;

    public ObservableCollection<string>? Items
    {
        get { return _yourItemsSource; }
        set
        {
            if (_yourItemsSource != value)
            {
                _yourItemsSource = value;
                OnPropertyChanged();
            }
        }
    }

    List<string> _urlsVisited = new List<string>();

    [RelayCommand]
    async Task Stop()
    {
        try
        {
            _cts?.Cancel();
            SerializeObject(Docs, nameof(Docs));
            SerializeObject(Items, nameof(Items));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    CancellationTokenSource? _cts;

    [RelayCommand]
    async Task Start()
    {
        try
        {
            var baseUrl = Url;
            try
            {
                _cts = new CancellationTokenSource();
                await Task.Run(async () => await NavigateUrl(baseUrl, _cts.Token), _cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            SerializeObject(Docs, nameof(Docs));
            SerializeObject(Items, nameof(Items));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    async Task NavigateUrl(string url, CancellationToken cancellationToken = default)
    {
        if (!ValidateUrl(url))
        {
            return;
        }

        //  Url = url;
        _urlsVisited.Add(url);

        var html = string.Empty;
        try
        {
            if (url.EndsWith(".pdf"))
            {
                await LookPdf(url);
                return;
            }
            html = await DownloadHtml(url);
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
                    Items?.Add(href);
                    await Task.Run(() => NavigateUrl(absoluteUrl, cancellationToken), cancellationToken);
                }
            }
        }
    }

    bool ValidateUrl(string absoluteUrl)
    {
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

    string GetAbsoluteUrl(string baseUrl, string relativeUrl)
    {
        if (Uri.TryCreate(new Uri(baseUrl), relativeUrl, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }
        return relativeUrl;
    }

    void SerializeObject(object docs, string name)
    {
        var json = JsonSerializer.Serialize(docs);
        Console.WriteLine(json);
        string cacheDir = FileSystem.Current.AppDataDirectory;
        var filePath = $"{cacheDir}/{name}.json";
        var stream = Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes(filePath, stream);
    }

    T DeserializeObject<T>(string name)
    {
        string cacheDir = FileSystem.Current.AppDataDirectory;
        var filePath = $"{cacheDir}/{name}.json";
        if (!File.Exists(filePath))
        {
            return default;
        }
        var stream = File.ReadAllBytes(filePath);
        var json = Encoding.UTF8.GetString(stream);
        return JsonSerializer.Deserialize<T>(json);
    }

    async Task LookPdf(string href)
    {
        Console.WriteLine("Link: " + href);
        if (href.EndsWith(".pdf"))
        {
            string cacheDir = FileSystem.Current.AppDataDirectory;
            var filePath = $"{cacheDir}/{href}";

            var pdfUrl = $"{biblioUrl}{href}";

            if (Docs.FirstOrDefault(x => x.Url == Url) != null)
            {
                System.Diagnostics.Debug.WriteLine("Doc already exists on database");
                return;
            }

            // await DownloadAndSavePdf(pdfUrl, filePath);

            string pattern = @"(\d+)_(\d+)_(\d+)_(\d+)_[a-f0-9]+\.pdf";
            Match match = Regex.Match(Path.GetFileName(filePath), pattern);

            var doc = new Doc
            {
                Id = GetDeterministicHashCode(pdfUrl),
                Url = pdfUrl,
                Path = filePath,
                Publication = "Defesa de Espinho",
                ScrapDate = DateTime.Now
            };

            if (match.Success)
            {
                int year = Convert.ToInt32(match.Groups[4].Value);
                int month = Convert.ToInt32(match.Groups[3].Value);
                int day = Convert.ToInt32(match.Groups[2].Value);
                int id = Convert.ToInt32(match.Groups[1].Value);
                Console.WriteLine($"YEar: {year}, Month: {month}, Day: {day}");
                //  doc.Year = year.ToString();
            }

            Docs?.Add(doc);
        }
    }

    async Task DownloadAndSavePdf(string pdfUrl, string filePath)
    {
        var stream = await DownloadPdf(pdfUrl);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        File.WriteAllBytes(filePath, stream);
    }

    [ObservableProperty]
    ObservableCollection<Doc>? _docs = new ObservableCollection<Doc>();

    static async Task<string> DownloadHtml(string url)
    {
        string cacheDir = FileSystem.Current.AppDataDirectory;
        var htmlFilePath = $"{cacheDir}/{GetDeterministicHashCode(url)}.html";
        if (File.Exists(htmlFilePath))
        {
            return Encoding.UTF8.GetString(File.ReadAllBytes(htmlFilePath));
        }

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Download the HTML content of the webpage
                var html = await client.GetStringAsync(url);

                File.WriteAllBytes(htmlFilePath, Encoding.UTF8.GetBytes(html));
                return html; ;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle 404 Not Found error
                Console.WriteLine("Page not found: " + url);
                return string.Empty;
            }
        }
    }

    static int GetDeterministicHashCode(string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    static async Task<byte[]> DownloadPdf(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            // Download the HTML content of the webpage
            return await client.GetByteArrayAsync(url);
        }
    }
}
