using Lime.Messaging.Contents;
using Lime.Protocol;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Extensions.EventTracker;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationExtensions;

namespace CsTemplateBot.Services
{
    public class EventTrackService : IEventNotificator
    {
		private readonly IEventTrackExtension _eventTrack;
        private readonly ILogger _logger;

        public EventTrackService(IEventTrackExtension eventTrack, ILogger logger)
        {
            _eventTrack = eventTrack;
            _logger = logger;
        }

		public async Task<Document> RegisterEvent(Document eventDocument, Message originatorMessage)
        {
            if (eventDocument != null)
            {
                try
                {
					var from = originatorMessage.From.ToIdentity();
	                var data = (eventDocument as PlainText).Text;
                    var ev = JsonConvert.DeserializeObject<BotEvent>(data);
                    var extras = new Dictionary<string, string>
                        {
                            { "userId", from.ToString() },
                            { "originatorMessageId", originatorMessage.Id }
                        };

					await _eventTrack.AddAsync(ev.EventName, ev.ActionName, extras, CancellationToken.None, from);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error registering event.");
                }
            }

			return null;
        }
    }

    public class BotEvent
    {
        public string EventName { get; set; }
        public string ActionName { get; set; }
    }
}