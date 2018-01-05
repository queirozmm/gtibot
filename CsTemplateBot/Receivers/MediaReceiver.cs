using Lime.Messaging.Contents;
using Lime.Protocol;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Services;
using Takenet.MessagingHub.Client.Sender;
using Takenet.Mpa.MpaTranslateLime;
using CsTemplateBot.Services;

namespace CsTemplateBot.Receivers
{
    public class MediaReceiver : MessageReceiverBase
    {
		private const string GenericErrorCommand = "#ErroGenerico";
        private readonly IMediaService _mediaService;

        public MediaReceiver(
             IMessagingHubSender sender,
             MySettings settings,
             IContextManager context,
             IContactService contactService,
             IMpaService mpaService,
             IGenericErrorService genericErrorService,
             ILogger logger,
             IMediaService mediaService)
            : base(sender, settings, context, contactService, mpaService, genericErrorService, logger)
        {
            _mediaService = mediaService;
        }

        protected override async Task ReceiveMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var keyword = _mediaService.GetMediaErrorKeyWord(message);

            var navResult = await _mpaService.SendToMPAAsync(keyword, message.From, cancellationToken);

            if (navResult.NavigationState == NavigationState.Error)
            {
                _logger.Error($"Error on sending message to MPA. User from: {message.From} user message: {keyword}");

                var result = await _mpaService.SendToMPAAsync(GenericErrorCommand, message.From, cancellationToken);

                if (result.NavigationState == NavigationState.Error)
                {
                    throw (new Exception());
                }
            }
        }
    }
}
