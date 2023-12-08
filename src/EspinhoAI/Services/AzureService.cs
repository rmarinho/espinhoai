using System;
using Azure;
using System.Net;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace EspinhoAI.Services
{
    public class AzureService
    {
        IConfiguration _config;
        string _key;
        string _endpoint;
        AzureKeyCredential _credential;
        Uri _endpointUrl;
        public AzureService(IConfiguration configuration)
        {
            _config = configuration;
            var settings = _config.GetRequiredSection("Settings").Get<EspinhoAI.Models.Settings>();
            _key = settings.Azure?.Key;
            _endpoint = settings.Azure?.Endpoint;
            if (string.IsNullOrEmpty(_key))
                throw new ArgumentNullException(nameof(_key));
            _credential = new AzureKeyCredential(_key);
            _endpointUrl = new Uri(_endpoint);
        }

        //we need the content for serialization to work
        public async Task<(string Content, AnalyzeResult Result)> GetAzureAnalyzeResult(byte[] image, CancellationToken cancellationToken = default)
        {
            using var imageStream = new MemoryStream(image);
            DocumentAnalysisClient client = new DocumentAnalysisClient(_endpointUrl, _credential);

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", imageStream, cancellationToken: cancellationToken);

            var response = await operation.WaitForCompletionResponseAsync();
            var responseContent = response.Content.ToString();
            AnalyzeResult result = operation.Value;
            return (responseContent, result);
        }
    }
}

