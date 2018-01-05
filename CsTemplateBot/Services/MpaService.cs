using Lime.Messaging.Contents;
using Lime.Protocol;
using NLog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationActions;
using Takenet.Mpa.MpaTranslateLime;

namespace CsTemplateBot.Services
{
    public class MpaService : IMpaService
    {
        private readonly INavigationExtension _navigation;
        private readonly ILogger _logger;
        private readonly IMpaHelper _mpaHelper;
        private readonly MySettings _settings;



        public MpaService(INavigationExtension navigation,
            ILogger logger,
            IMpaHelper mpaHelper,
            MySettings settings)
        {
            _navigation = navigation;
            _logger = logger;
            _mpaHelper = mpaHelper;
            _settings = settings;
        }

        public async Task<NavigationResult> SendToMPAAsync(List<Message> messageList, CancellationToken cancellationToken, Dictionary<string, string> navigationParameters = null)
        {
            var from = messageList.FirstOrDefault().From;
            var request = new NavigationRequest
            {
                LimeMessages = messageList,
                NavigationType = NavigationType.Text,
                NavigationParameters = navigationParameters == null ? new Dictionary<string, string>() : navigationParameters
            };
            var result = await _navigation.GetNavigationAsync(request, cancellationToken);
            var navResult = await _navigation.ExecuteNavigationAsync(result, cancellationToken);
            if (result.State == NavigationState.Error || navResult?.NavigationState == NavigationState.Error)
            {
                var exception = navResult.Exception ?? result.NavigationActions.OfType<ErrorNavigationAction>().First().Exception;
                _logger.Error(exception, $"[NavigationError] Content: {from.ToString()};");
            }
            return navResult;
        }

        public async Task<NavigationResult> SendToMPAAsync(Message message, CancellationToken cancellationToken, Dictionary<string, string> navigationParameters = null)
        {
            message.Content = PlainText.Parse(Regex.Replace(message.Content.ToString(), @"\r\n?|\n", " "));
            return await SendToMPAAsync(new List<Message> { message }, cancellationToken, navigationParameters);
        }

        public async Task<NavigationResult> SendToMPAAsync(string message, Node from, CancellationToken cancellationToken, Dictionary<string, string> navigationParameters = null)
        {
            var plainContent = PlainText.Parse(Regex.Replace(message, @"\r\n?|\n", " "));
            return await SendToMPAAsync(new List<Message> { new Message(EnvelopeId.NewId()) { Content = plainContent, From = from, To = from, Metadata = new Dictionary<string, string>() } }, cancellationToken, navigationParameters);
        }

        public async Task<bool> IsInMPASessionAsync(Node from, CancellationToken cancellationToken)
        {
            return await _mpaHelper.HasSessionAsync(from.Name, $"{from.Domain}_{_settings.MPASettings.SessionIdentifier}", cancellationToken);
        }
    }
}
