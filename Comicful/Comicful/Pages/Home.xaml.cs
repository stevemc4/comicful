using Comicful.Readers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Comicful.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }

        private async void openFile_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            var window = (Application.Current as App).CurrentWindow;
            var hWnd = WindowNative.GetWindowHandle(window);

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.FileTypeFilter.Add(".cbz");
            openPicker.FileTypeFilter.Add(".pdf");

            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    switch (file.FileType)
                    {
                        case ".cbz":
                        case ".pdf":
                            Frame.Navigate(typeof(ReaderPage), file);
                            break;
                        default:
                            {
                                ShowErrorReadingDialog(Reader.Errors.UNSUPPORTED_FILE);
                                break;
                            }
                    }
                } catch
                {
                    ShowErrorReadingDialog(Reader.Errors.OTHER_ERRORS);
                }
            }
        }

        private void importFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ShowErrorReadingDialog(Reader.Errors reason)
        {
            var dialog = new ContentDialog()
            {
                Title = "Can't read file",
                XamlRoot = this.XamlRoot,
                CloseButtonText = "Close"
            };

            switch (reason)
            {
                case Reader.Errors.UNSUPPORTED_FILE:
                    {
                        dialog.Content = "File not supported, please try another file";
                        break;
                    }
                default:
                    {
                        dialog.Content = "Something wrong happened, please try again";
                        break;
                    }
            }

            await dialog.ShowAsync();
        }
    }
}
