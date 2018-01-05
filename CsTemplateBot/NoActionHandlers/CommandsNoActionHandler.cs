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
using CsTemplateBot.Models;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Services;
using System.Collections.Generic;
using Lime.Messaging.Contents;

namespace CsTemplateBot.NoActionHandlers
{
    public class CommandsNoActionHandler : INoActionHandler
    {
	    private const string HubIACommand = "hubia";
        private const string SearchUser = "searchuser";
        private const string SearchLanguage = "searchlang";
        private const string SearchRepo = "searchrepo";
        private const string GetUser = "getuser";
        private readonly ILogger _logger;
        private readonly IMessagingHubSender _sender;
		private readonly INavigationExtension _navigation;
		private readonly INLPService _nlpService;
        private readonly IGitService _gitService;
        private readonly IContactService _contactService;
        private readonly IMpaService _mpaService;
        
        public CommandsNoActionHandler(IMessagingHubSender sender, INavigationExtension navigation, ILogger logger, 
            INLPService nlpService, IGitService gitService, IContactService contactService, IMpaService mpaService)
        {
            _logger = logger;
            _sender = sender;
            _navigation = navigation;
			_nlpService = nlpService;
            _gitService = gitService;
            _contactService = contactService;
            _mpaService = mpaService;
        }

		//"ping" noaction is just an example. You can delete it. 
        public string[] Tokens => new[] { HubIACommand, SearchLanguage, SearchUser, SearchRepo, GetUser };

        public async Task HandleAsync(string text, Message message, CancellationToken cancellationToken)
        {
            if (text.Contains(HubIACommand))
            {
                text = text.Replace($"#{HubIACommand}#", "");
                var param = text.Split('|');
                var resp = await _nlpService.ProcessAsync(param[0], message, cancellationToken);
            }
            else if (text.Contains(SearchLanguage))
            {
                text = text.Replace($"#{SearchLanguage}#", "");
                await SearchLanguageAsync(message.From, cancellationToken);
            }
            else if (text.Contains(SearchUser))
            {
                text = text.Replace($"#{SearchUser}#", "");
                await SearchUserAsync(message.From, cancellationToken);
            }
            else if (text.Contains(SearchRepo))
            {
                text = text.Replace($"#{SearchRepo}#", "");
                await SearchRepoAsync(message.From, cancellationToken);
            }
            else if (text.Contains(GetUser))
            {
                text = text.Replace($"#{GetUser}#", "");
                await GetUserAsync(text, message.From, cancellationToken);
            }
        }

        private async Task GetUserAsync(string text, Node from, CancellationToken cancellationToken)
        {
            GitUser user = await _gitService.GetUser(text);
            if (user.Name == null)
            {
                var contact = await _contactService.GetContactAsync(from, cancellationToken);
                contact.Name = user.Name;
                contact.PhotoUri = new Uri(user.AvatarUrl);
                var navParams = new Dictionary<string, string>() { { "varName", "content" } };
                await _contactService.SetContactAsync(contact, from, cancellationToken);

                var d = new MediaLink()
                {
                    Title = user.Name,
                    Text = "Olha aqui seu avatar kaka fei pa carai",
                    Type = MediaType.Parse("image/jpeg"),
                    AspectRatio = "1:1",
                    Uri = contact.PhotoUri
                };

                await _sender.SendMessageAsync(d, from, cancellationToken);
                await _mpaService.SendToMPAAsync(user.Name, from, cancellationToken, navParams); 
            }
        } 

        private async Task SearchRepoAsync(Node from, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync("Search repos", from, cancellationToken);
        }

        private async Task SearchUserAsync(Node from, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync("Search user", from, cancellationToken);
        }

        private async Task SearchLanguageAsync(Node from, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync("Search languages", from, cancellationToken);
        }
    }
}