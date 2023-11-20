using Comicful.Pages;
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
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Comicful
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            navigationViewControl.SelectedItem = navigationViewControl.MenuItems.FirstOrDefault(0);
            navigationViewControl.Header = navigationViewControl.SelectedItem.As<NavigationViewItem>().Content;
            contentFrame.Navigate(typeof(Home), null);

            contentFrame.Navigated += ContentFrame_Navigated;

            navigationViewControl.BackRequested += NavigationViewControl_BackRequested;
        }

        private void NavigationViewControl_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            contentFrame.GoBack();
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            navigationViewControl.IsBackEnabled = contentFrame.CanGoBack;

            if (e.SourcePageType == typeof(ReaderPage))
            {
                navigationViewControl.AlwaysShowHeader = false;
                navigationViewControl.IsPaneOpen = false;
                navigationViewControl.SelectedItem = null;
            }
            else
            {
                var navigationViewItem = navigationViewControl.MenuItems
                    .Concat(navigationViewControl.FooterMenuItems)
                    .Where(item => (item as NavigationViewItem).Tag.ToString() == e.SourcePageType.ToString())
                    .First();
                navigationViewControl.AlwaysShowHeader = true;
                navigationViewControl.SelectedItem = navigationViewItem;
                navigationViewControl.Header = navigationViewControl.SelectedItem?.As<NavigationViewItem>().Content;
            }
        }
    }
}
