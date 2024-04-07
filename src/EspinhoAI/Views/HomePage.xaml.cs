using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using iText.Layout.Borders;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

#if WINDOWS
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
#endif

#if IOS || MACCATALYST
using UIKit;
using Foundation;
#endif

namespace EspinhoAI;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    void DropGestureDragLeave(object? sender, DragEventArgs e)
    {
		Out();
    }

    async void DropGestureDrop(object? sender, DropEventArgs e)
    {
        var filePaths = new List<string>();

#if WINDOWS
			if (e.PlatformArgs is not null && e.PlatformArgs.DragEventArgs.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items = await e.PlatformArgs.DragEventArgs.DataView.GetStorageItemsAsync();
				if (items.Any())
				{
					foreach (var item in items)
					{
						if (item is StorageFile file)
						{
							filePaths.Add(item.Path);
						}
					}

				}
			}
#endif


#if MACCATALYST

			var session = e.PlatformArgs?.DropSession;
			if (session == null)
			{
				return;
			}
			foreach (UIDragItem item in session.Items)
			{
				var result = await LoadItemAsync(item.ItemProvider, item.ItemProvider.RegisteredTypeIdentifiers.ToList());
				if (result is not null)
				{
					filePaths.Add(result.FileUrl?.Path!);
				}
			}
			foreach (var item in filePaths)
			{
				Debug.WriteLine($"Path: {item}");
			}

			static async Task<LoadInPlaceResult?> LoadItemAsync(NSItemProvider itemProvider, List<string> typeIdentifiers)
			{
				if (typeIdentifiers is null || typeIdentifiers.Count == 0)
				{
					return null;
				}

				var typeIdent = typeIdentifiers.First();

				if (itemProvider.HasItemConformingTo(typeIdent))
				{
					return await itemProvider.LoadInPlaceFileRepresentationAsync(typeIdent);
				}

				typeIdentifiers.Remove(typeIdent);

				return await LoadItemAsync(itemProvider, typeIdentifiers);
			}

#endif

        lblPath.Text = $"Loading: {filePaths.FirstOrDefault()}";
        Out();
    }

    void DropGestureDragOver(object? sender, DragEventArgs e)
    {
        In();
        Debug.WriteLine($"Dragging {e.Data?.Text}, {e.Data?.Image}");
    }

    void In()
    {
        border.StrokeThickness = 4;
        border.Background = Colors.LightGray.WithAlpha(0.5f);
		//lblPath.FontAttributes = FontAttributes.Bold;
    }

    void Out()
    {
        border.Background = Colors.Transparent;
        border.StrokeThickness = 1;
       // lblPath.FontFamily = FontAttributes.None;
    }

    async void TapGestureRecognizer_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
		try
		{
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.MacCatalyst, new[] { "PDF" } }, // UTType values
                });

            PickOptions options = new()
            {
                PickerTitle = "Please select a pdf file",
                FileTypes = customFileType,
            };
            var url = await PickAndShow(options);
        }
        catch (Exception ex)
		{

		}
    }

    async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }
}