using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static WpfApp1.MainWindow;
using System.Windows.Controls;
using System.Net.Http.Json;

namespace WpfApp1
{
    public class ImageDictionary
    {
        public Dictionary<string, ImageBrush> ImageDict { get; set; }
        public string addr = ""; //Адрес со списком картинок
        public string Log = ""; //Адрес с логином
        public HttpClient client = new HttpClient();

        public ImageDictionary() 
        {
            avtoritiz();
            ImageDict = GetImage().Result;
        }

        public async Task<Dictionary<string, ImageBrush>> GetImage()
        {
            Dictionary<string, Byte[]> rep = new Dictionary<string, Byte[]>();
            Dictionary<string, ImageBrush> vv= new Dictionary<string, ImageBrush>();
            try
            {
                rep = await client.GetFromJsonAsync<Dictionary<string, Byte[]>>(addr);
            }
            catch (Exception ex)
            {
                rep.Add("None", null);
            }
            foreach (var kvp in rep)
            {
                vv.Add(kvp.Key,ByteArrToImage(kvp.Value));
            }
            return vv;
        }


        static ImageBrush ByteArrToImage(Byte[] byteArr)
        {
            if (byteArr != null)
            {
                Image img = new Image();
                ImageBrush Brua = new ImageBrush();
                MemoryStream stream = new MemoryStream(byteArr);
                img.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                Brua.ImageSource = img.Source;
                return Brua;
            }
            return null;
        }



        public async Task avtoritiz()
        {
            var values = new Dictionary<string, string>
                {
                    { "email", "Test"},
                    { "password", "Test!123" }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(Log, content);

            var responseString = await response.Content.ReadAsStringAsync();
        }

    }
}
