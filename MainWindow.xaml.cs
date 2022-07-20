using System;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;


namespace WPFSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222");
            InitializeComponent();
            //webView.NavigationStarting += EnsureHttps;
            InitializeAsync();

        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            DevToolsProtocolHelper helper = webView.CoreWebView2.GetDevToolsProtocolHelper();
            await helper.Runtime.EnableAsync();
            helper.Runtime.ConsoleAPICalled += Runtime_ConsoleAPICalled;

            Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();
            Trace.WriteLine("Entering Main");
            Trace.WriteLine("Exiting Main");
            Trace.Unindent();

            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;

            webView.CoreWebView2.OpenDevToolsWindow();
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
        }

        private void Runtime_ConsoleAPICalled(object sender, Runtime.ConsoleAPICalledEventArgs e)
        {

            String test3 = e.Args[0].Value.ToString();
            Trace.WriteLine(test3);
        }

        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            addressBar.Text = uri;
            webView.CoreWebView2.PostWebMessageAsString(uri);
        }

        void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            String uri = args.Uri;
            if (!uri.StartsWith("https://"))
            {
                webView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
                args.Cancel = true;
            }
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("click!");
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(addressBar.Text);
            }
        }

    }

}

