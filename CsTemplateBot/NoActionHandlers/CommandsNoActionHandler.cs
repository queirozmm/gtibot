using Lime.Protocol;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationExtensions;
using Takenet.MessagingHub.Client.Sender;
using CsTemplateBot.Services;

namespace CsTemplateBot.NoActionHandlers
{
    public class CommandsNoActionHandler : INoActionHandler
    {
	    private const string HubIACommand = "hubia";
        private readonly ILogger _logger;
        private readonly IMessagingHubSender _sender;
		private readonly INavigationExtension _navigation;
		private readonly INLPService _nlpService;
        
        public CommandsNoActionHandler(IMessagingHubSender sender, INavigationExtension navigation, ILogger logger, INLPService nlpService)
        {
            _logger = logger;
            _sender = sender;
            _navigation = navigation;
			_nlpService = nlpService;
        }

		//"ping" noaction is just an example. You can delete it. 
        public string[] Tokens => new[] { "ping", HubIACommand };

        public async Task HandleAsync(string text, Message messageOriginator, CancellationToken cancellationToken)
        {
			if (text.Contains(HubIACommand))
            {
                text = text.Replace($"#{HubIACommand}#", "");
                var param = text.Split('|');
                var resp = await _nlpService.ProcessAsync(param[0], messageOriginator, cancellationToken);
            }
			//Example of if statement for "ping" noaction example. You can delete it.
			else  if (text.Contains("#ping#"))
            {
                await CommandPing(messageOriginator.From, cancellationToken);
            }
        }

		//Example of function for "ping" noaction example. You can delete it.
        private async Task CommandPing(Node from, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync("pong", from, cancellationToken);
        }
    }
}