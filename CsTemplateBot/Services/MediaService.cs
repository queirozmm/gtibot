using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsTemplateBot.Services
{
    public class MediaService : IMediaService
    {
        private const string AudioErrorKey = "#ERRORAUDIO";
        private const string VideoErrorKey = "#ERRORVIDEO";
        private const string ImageErrorKey = "#ERRORIMAGE";
        private const string FileErrorKey = "#ERRORFILE";
        private const string MediaUnrecognizedKey = "#MIDIANAORECONHECIDA";
        private Dictionary<string, string> ErrorKeysMap { get; set; } 

        public MediaService()
        {
            ErrorKeysMap = new Dictionary<string, string>()
            {
                { "application", FileErrorKey },
                { "text", FileErrorKey },
                { "audio", AudioErrorKey },
                { "video", VideoErrorKey },
                { "image", ImageErrorKey }
            };
        }

        public string GetMediaErrorKeyWord(Message message)
        {
            var messageType = "";
            try
            {
                var doctype = message.Content.GetType();

                if (doctype.Name.Equals(nameof(MediaLink)))
                {
                    messageType = (message.Content as MediaLink).Type.Type.ToString();
                }
                else if (doctype.Name.Equals(nameof(DocumentCollection)))
                {
                    messageType = ((message.Content as DocumentCollection).Items.FirstOrDefault() as MediaLink).Type.Type.ToString();
                }
            }
            catch (Exception e)
            {

            }

            return ErrorKeysMap.TryGetValue(messageType, out string value) ? value : MediaUnrecognizedKey;
        }
    }
}
