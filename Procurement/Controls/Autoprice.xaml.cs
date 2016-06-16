using Procurement.ViewModel;
using System.Windows.Controls;
using System.Diagnostics;
using System.Net.Http;
using System;
using System.Text;
using System.Net;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Procurement.Controls
{
    public partial class Autoprice : UserControl
    {
        public Autoprice()
        {
            InitializeComponent();
            ReloadPoetrade();
            //this.DataContext = new TradeSettingsViewModel();
        }

        private void Hyperlink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("http://www.pathofexile.com/private-messages/compose/to/poexyzis");
        }

        private async Task<string> LoadPageData(String path)
        {
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(path);
            String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);

            return source;
        }

        private async void ReloadPoetrade()
        {
            var source = await LoadPageData("https://www.pathofexile.com/forum/view-forum/616");

            var get_themes = new Regex("/forum/view-thread/[0-9]+#p[0-9]+");
            var matches = get_themes.Matches(source);
            foreach(var m in matches)
            {
                var shop = await LoadPageData("https://www.pathofexile.com" + m);

            }
        }
    }
}
