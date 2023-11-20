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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Comicful.Pages
{
    public sealed partial class ReaderPage : Page
    {

        private class VM : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private Reader _reader;
            public Reader Reader {
                get { return _reader; }
                set
                {
                    _reader = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(Reader)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(PageIndicator)));
                    }
                }
            }

            private int _currentPage = 1;
            public int CurrentPage
            {
                get { return _currentPage; }
                set
                {
                    _currentPage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(PageIndicator)));
                    }
                }
            }

            public string PageIndicator
            {
                get
                {
                    return String.Format("{0}/{1}", _currentPage, _reader?.TotalPage ?? 0);
                }
            }
        }

        private VM ViewModel = new VM();

        public ReaderPage()
        {
            this.InitializeComponent();
            contentView.ZoomCompleted += ContentView_ZoomCompleted;
            image1.PointerPressed += Image1_PointerPressed;
        }

        private void Image1_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Handle clicks based on pointer position in set percentage-based section
            // First 10% = Previous page
            // Last 20% = Next page
            // Center (rest of the section) = Show/Hide Reader UI
            var pointerXPosition = e.GetCurrentPoint(image1).Position.X;
            var positionPercentage = (pointerXPosition / image1.ActualWidth) * 100;
            if (positionPercentage <= 10)
            {
                GoToPage(ViewModel.CurrentPage - 1);
            }
            else if (positionPercentage <= 80)
            {
                Debug.WriteLine("Center");
            }
            else
            {
                GoToPage(ViewModel.CurrentPage + 1);
            }
        }

        private void ContentView_ZoomCompleted(ScrollView sender, ScrollingZoomCompletedEventArgs args)
        {
            if (sender.ZoomFactor <= sender.MinZoomFactor)
            {
                zoomOutButton.IsEnabled = false;
            }
            else
            {
                zoomOutButton.IsEnabled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var file = e.Parameter as StorageFile;
            loadingStatus.Text = String.Format("Reading \"{0}\"...", file.Name);
            ReadFile(file);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.Reader.Dispose();
        }

        private async void ReadFile(StorageFile file)
        {
            Reader reader = null;
            switch (file.FileType)
            {
                case ".cbz":
                    {
                        reader = new CBZReader(file);
                        break;
                    }
                default: break;
            }

            if (reader != null)
            {
                await reader.Read();
                ViewModel.Reader = reader;
                image1.Source = await reader.GetPageImage(0);
                loaderUI.Visibility = Visibility.Collapsed;
                contentView.Visibility = Visibility.Visible;
                readerMenu.Visibility = Visibility.Visible;
            }
        }

        private void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            switch (args.KeyboardAccelerator.Key)
            {
                case Windows.System.VirtualKey.Left:
                    {
                        GoToPage(ViewModel.CurrentPage - 1); break;
                    }
                case Windows.System.VirtualKey.Right:
                    {
                        GoToPage(ViewModel.CurrentPage + 1); break;
                    }
            }
        }

        private async void GoToPage(int page)
        {
            if (page == 0 || page > ViewModel.Reader.TotalPage)
            {
                return;
            }

            image1.Source = await ViewModel.Reader.GetPageImage(page - 1);
            ViewModel.CurrentPage = page;
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            contentView.ZoomBy(-.5f, null);
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            contentView.ZoomBy(.5f, null);
            Debug.WriteLine(contentView.ZoomFactor);
        }

        private void fitContent_Click(object sender, RoutedEventArgs e)
        {
            image1.Width = container.ActualWidth;
            image1.Height = container.ActualHeight;
            contentView.ZoomTo(1f, null);
        }

        private void goToPageInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (goToPageInput.Text == "")
            {
                goToPageInput.Value = ViewModel.CurrentPage;
            }
        }

        private void goToPageSubmit_Click(object sender, RoutedEventArgs e)
        {
            pageIndicator.Flyout.Hide();
            GoToPage((int)goToPageInput.Value);
        }
    }
}
