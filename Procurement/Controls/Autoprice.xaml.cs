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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Json;

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

        public class Socket
        {
            public int group { get; set; }
            public string attr { get; set; }
        }

        public class Property
        {
            public string name { get; set; }
            public List<List<object>> values { get; set; }
            public int displayMode { get; set; }
        }

        public class Requirement
        {
            public string name { get; set; }
            public List<List<object>> values { get; set; }
            public int displayMode { get; set; }
        }

        public class RootObject
        {
            public bool verified { get; set; }
            public int w { get; set; }
            public int h { get; set; }
            public int ilvl { get; set; }
            public string icon { get; set; }
            public string league { get; set; }
            public string id { get; set; }
            public List<Socket> sockets { get; set; }
            public string name { get; set; }
            public string typeLine { get; set; }
            public bool identified { get; set; }
            public bool corrupted { get; set; }
            public bool lockedToCharacter { get; set; }
            public List<Property> properties { get; set; }
            public List<Requirement> requirements { get; set; }
            public List<string> explicitMods { get; set; }
            public List<string> flavourText { get; set; }
            public int frameType { get; set; }
            public List<object> socketedItems { get; set; }
        }

        public Stream GenerateStreamFromString(string s)
        {
            s = Regex.Replace(s,"\"values\":[[.+]],", "");

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async void ReloadPoetrade()
        {
            var source = await LoadPageData("https://www.pathofexile.com/forum/view-forum/616");

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));

            var get_themes = new Regex("/forum/view-thread/[0-9]+#p[0-9]+");
            var get_item_data = new Regex("\\(new R\\(.+run\\(\\); }\\);");
            var matches = get_themes.Matches(source);
            foreach (var m in matches)
            {
                var shop = await LoadPageData("https://www.pathofexile.com" + m);

                var data = get_item_data.Matches(shop);
                if(data.Count > 0)
                {
                    string data_s = data[0].Value;
                    List<RootObject> items = new List<RootObject>();
                    int s_pos = data_s.FirstOrDefault(x => x == '[');
                    if (s_pos == default(int))
                        continue;

                    int e_pos = s_pos;
                    int br_count = 1;
                    while (s_pos < data_s.Length)
                    {
                        e_pos++;

                        if (data_s[e_pos] == ']')
                            br_count--;
                        if (data_s[e_pos] == '[')
                            br_count++;

                        if(br_count == 0)
                        {
                            s_pos = data_s.IndexOf('{', s_pos);

                            try
                            {
                                var stream = GenerateStreamFromString(data_s.Substring(s_pos, e_pos - s_pos - 3));

                                RootObject p2 = (RootObject)ser.ReadObject(stream);

                                items.Add(p2);
                            }
                            catch
                            {

                            }

                            e_pos++;
                            s_pos = e_pos;
                        }
                    }
                }
               // var items = ParseShop(shop);
            }
        }
    }
}
