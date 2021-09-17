using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows.Controls;

namespace S4EE
{
    /// <summary>
    /// Interaktionslogik für AppWebView.xaml
    /// </summary>
    public partial class AppWebView : Page
    {
        private readonly string _cacheFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "S4EE");
        public AppWebView()
        {
            InitializeComponent();
            Load();
        }
        protected async void Load()
        {
            try
            {
                var webView2Environment = await CoreWebView2Environment.CreateAsync(null, _cacheFolderPath);
                await webView.EnsureCoreWebView2Async(webView2Environment);
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }
            catch (Exception)
            {

            }
        }
    }
}