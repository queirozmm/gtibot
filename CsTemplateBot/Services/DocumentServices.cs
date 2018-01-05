using Lime.Messaging.Contents;
using Lime.Protocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsTemplateBot.Services
{
    class DocumentServices
    {


        public DocumentServices()
        {

        }

        public Document CreatePlainText(string text)
        {
            PlainText doc = new PlainText { Text = text };
            return doc;
        }

        public Select CreateOptionText(List<string> options, string text)
        {
            Select select = new Select { Text = text, Options = CreateSelectOptions(options) };
            return select;
        }

        public DocumentSelect CreateQuickReply(string MenuText, List<DocumentSelectOption> options)
        {
            DocumentSelect document = new DocumentSelect();
            document.Scope = SelectScope.Immediate;
            document.Header = new DocumentContainer();
            document.Header.Value = new PlainText();
            (document.Header.Value as PlainText).Text = MenuText;


            DocumentSelectOption[] selectOption = new DocumentSelectOption[options.Count()];

            for (int i = 0; i < options.Count(); i++)
            {
                selectOption[i] = options[i];
            }

            document.Options = selectOption;

            return document;
        }

        public SelectOption[] CreateSelectOptions(List<string> options)
        {
            SelectOption[] myOptions = new SelectOption[options.Count()];
            for (int i = 0; i < options.Count(); i++)
            {
                myOptions[i] = new SelectOption { Text = options[i] };
            }
            return myOptions;
        }
        public Select CreateOptionButtonText(string option, string text)
        {
            Select select = new Select { Text = text, Options = CreateSelectButton(option) };
            return select;
        }

        private SelectOption[] CreateSelectButton(string options)
        {
            SelectOption[] myOptions = new SelectOption[1];

            myOptions[0] = new SelectOption { Text = options };

            return myOptions;
        }

        public Document CreateMediaDocument(string msg, string title = null, string text = null)
        {
            Document mediaResponse;
            Match m = Regex.Match(msg, @"(?<url>(http|https)://(?<subdomain>[a-zA-Z0-9]{1,20}.)?0mn.io/(?<id>[a-zA-Z0-9]{2,20}))");
            if (m.Success)
            {
                string url = m.Groups["url"].Value;

                string remoteId = m.Groups["id"].Value;
                string subdomain = m.Groups["subdomain"].Value;
                string mimeType = "audio/mp3";
                if (remoteId[0] == 'i')
                {
                    mimeType = "image/jpeg";
                }
				else if (remoteId[0] == 'v')
                {
                    mimeType = GetMediaBucketMediaMimeType(remoteId);
                }

                mediaResponse = CreateIrisDocument(url, mimeType, remoteId, text, 0, title);
            }
            else
            {
                mediaResponse = CreateMediaLink(title, text, new Uri(msg), "image/jpeg");
            }


            return mediaResponse;
        }

		public static string GetMediaBucketMediaMimeType(string remoteId)
        {
            string mimetype = "";
            
            try
            {
                string urlMediaBucket = "http://0mn.io/api/media/" + remoteId;


                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlMediaBucket);


                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Key 3e28aab2-5a98-47b8-b178-81c334f75150");
                using (var response = client.SendAsync(request).Result)
                {
                    //Na linha contentUrl, para poder colocar as imagens apontando para a AWS, deve-se alterar conforme exemplo abaixo
                    //contentUrl = jMedia["link"].ToString();
                    //além disso, uma propriedade no application.json (MessengerCarouselSkipUrlCheck) deve ser setado para false.
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string obj = response.Content.ReadAsStringAsync().Result;

                        JObject jObject = JObject.Parse(obj);
                        JToken jMedia = jObject["data"];                        
                        mimetype = (string)jMedia["mimetype"];
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return mimetype;
        }

        public DocumentSelectOption CreateDocumentSelectionOption(int? order, string text, object value = null)
        {
            value = value ?? order;

            var doc = new DocumentSelectOption();
            doc.Order = order;
            doc.Label = new DocumentContainer();
            doc.Label.Value = new PlainText { Text = $"{text}" };
            doc.Value = new DocumentContainer();
            doc.Value.Value = new PlainText { Text = $"{value}" };

            return doc;
        }

        public DocumentSelectOption CreateDocumentSelectionOption(int order, string text, WebLink value)
        {


            var doc = new DocumentSelectOption();
            doc.Order = order;
            doc.Label = new DocumentContainer();
            doc.Label.Value = value;
            //doc.Value = new DocumentContainer();
            //doc.Value.Value = value;

            return doc;
        }

        public static Document CreateIrisDocument(string url, string mimeType, string remoteId, string message, int size = 0, string title = null)
        {
            Document doc = null;

            try
            {
                var type = mimeType.Split('/')[0];
                var subtype = mimeType.Split('/')[1];

                if ((type == "image") || (type == "audio"))
                {

                    string mediaUrl = url;
                    //string 
                    if (size == 0)
                    {
                        mediaUrl = shortenUrl(getMediaBucketMediaUrl(remoteId, out size));
                    }


                    doc = new MediaLink
                    {
                        Text = message.Trim(),
                        Type = new MediaType(type, subtype),
                        PreviewType = new MediaType(type, subtype),
                        Uri = new Uri(mediaUrl),
                        PreviewUri = new Uri(mediaUrl),
                        Size = size
                    };

                    //If has title
                    if (!String.IsNullOrEmpty(title))
                    {
                        ((MediaLink)doc).Title = title;
                    }
                }
                else
                {
                    doc = new WebLink
                    {
                        Text = message.Trim(),
                        Uri = new Uri(url)
                    };

                    if (!String.IsNullOrEmpty(title))
                    {
                        ((WebLink)doc).Title = title;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return doc;
        }

        public static string shortenUrl(string longUrl)
        {
            string shortUrl = longUrl;
            try
            {
                string key = "AIzaSyC6XyVitfOrj1dsY5nyGk1rqTV9J5Ztnzo";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + key);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"longUrl\":\"" + longUrl + "\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();


                    JObject jObject = JObject.Parse(responseText);

                    shortUrl = (string)jObject["id"];
                }
            }
            catch (Exception ex)
            {

            }
            return shortUrl;
        }


        public static string getMediaBucketMediaUrl(string remoteId, out int size)
        {
            string contentUrl = "";
            size = 0;
            try
            {
                string urlMediaBucket = "http://0mn.io/api/media/" + remoteId;


                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlMediaBucket);


                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Key 3e28aab2-5a98-47b8-b178-81c334f75150");
                var response = client.SendAsync(request).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string obj = response.Content.ReadAsStringAsync().Result;

                    JObject jObject = JObject.Parse(obj);
                    JToken jMedia = jObject["data"];
                    contentUrl = (string)jMedia["link"];
                    size = (int)jMedia["size"];
                }
            }
            catch (Exception ex)
            {

            }

            return contentUrl;
        }

        public DocumentCollection CreateCarrossel(List<CarrosselCard> cards)
        {
            var docCollection = new DocumentCollection
            {
                ItemType = DocumentSelect.MediaType,
                Items = cards
                        .Select(card =>
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = card.CardMediaHeader
                                },
                                Options = CreateDocumentSelectOptions(card.options)
                            })
                        .ToArray()
            };

            return docCollection;
        }

        public DocumentSelectOption[] CreateDocumentSelectOptions(List<CarrosselOptions> options)
        {
            DocumentSelectOption[] opts = new DocumentSelectOption[options.Count];
            int i = 0;
            foreach (var option in options)
            {
                if (option.label.GetType() == typeof(WebLink))
                {
                    opts[i] = new DocumentSelectOption();
                    opts[i].Label = new DocumentContainer { Value = (WebLink)option.label };
                    opts[i].Value = new DocumentContainer { Value = option.value.ToString() };
                    i++;
                }
                else
                {
                    opts[i] = new DocumentSelectOption();
                    opts[i].Label = new DocumentContainer { Value = new PlainText { Text = option.label.ToString() } };
                    opts[i].Value = new DocumentContainer { Value = new PlainText { Text = option.value.ToString() } };
                    i++;

                }
            }
            return opts;

        }

        public MediaLink CreateMediaLink(string title, string text, Uri url, string mediatype)
        {
            var type = mediatype.Split('/')[0];
            var subtype = mediatype.Split('/')[1];
            MediaLink mediaLink = new MediaLink
            {
                Title = title,
                Text = text,
                PreviewUri = url,
                Uri = url,
                Size = 1,
                Type = new MediaType(type, subtype)

            };
            return mediaLink;
        }


    }

    public class CarrosselCard
    {
        public MediaLink CardMediaHeader { get; set; }
        public string CardContent { get; set; }
        public List<CarrosselOptions> options { get; set; }
    }

    public class CarrosselOptions
    {
        public object value { get; set; }
        public object label { get; set; }
    }
}
