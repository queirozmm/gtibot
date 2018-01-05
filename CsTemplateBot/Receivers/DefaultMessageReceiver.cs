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
public class DefaultMessageReceiver : MessageReceiverBase
    {

		private const string GenericErrorCommand = "#ErroGenerico";
		public DefaultMessageReceiver(
			IMessagingHubSender sender, 
			MySettings settings, 
			IContextManager context, 
			IContactService contactService,
			IMpaService mpaService,
            IGenericErrorService genericErrorService,
			ILogger logger)
            : base(sender, settings, context, contactService, mpaService, genericErrorService, logger)
        {

        }

        protected override async Task ReceiveMessageAsync(Message message, CancellationToken cancellationToken)
        {
             var navResult = await _mpaService.SendToMPAAsync(message, cancellationToken);
            
            if (navResult.NavigationState == NavigationState.Error)
            {
                _logger.Error($"Error on sending message to MPA. User from: {message.From} user message: {(message.Content as PlainText).Text}");

                var result = await _mpaService.SendToMPAAsync(GenericErrorCommand, message.From, cancellationToken);

                if (result.NavigationState == NavigationState.Error)
                {
                    throw (new Exception());
                }
            }
        }

    }
}
