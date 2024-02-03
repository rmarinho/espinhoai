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
        Repository _repository;
        public DocumentViewModel(Repository repository)
        {
            _repository = repository;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Doc = query["doc"] as Doc;
        }

        [ObservableProperty]
        Doc? _doc;

        [ObservableProperty]
        bool _isBusy;
    }
}