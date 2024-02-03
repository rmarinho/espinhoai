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
    public partial class DocsViewModel : ObservableObject
    {
        Repository _repository;
        public DocsViewModel(Repository repository)
        {
            _repository = repository;
            LoadDocs();
        }

        [RelayCommand]
        public async Task LoadDocs()
        {  
            IsBusy = true;
            await Task.Run( () =>  Docs = new ObservableCollection<Doc>(_repository.Docs()));
            IsBusy = false;
        }

        [RelayCommand]
        public async Task GoToItem(Doc doc)
        {
            var navigationParameter = new ShellNavigationQueryParameters
            {
                { "doc", doc }
            };
            await Shell.Current.GoToAsync($"//docs/doc",navigationParameter);
        }

        [ObservableProperty]
        ObservableCollection<Doc>? _docs = new ObservableCollection<Doc>();

        [ObservableProperty]
        bool _isBusy;
    }
}