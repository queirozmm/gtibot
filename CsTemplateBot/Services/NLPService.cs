using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Extensions.ArtificialIntelligence;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.Sender;
using NLog;

namespace CsTemplateBot.Services
{
    public class NLPService : INLPService
    {
		private const string DontKnowCommand = "#DONTKNOWANSWER";
        private const string InitialFlowCommand = "#INICIO";
        private const string GenericErrorCommand = "#ErroGenerico";
        private readonly IArtificialIntelligenceExtension _artificialIntelligenceExtension;
        private readonly MySettings _settings;
        private readonly IMessagingHubSender _sender;
        private readonly IMpaService _mpaService;
        private readonly ILogger _logger;
        private readonly IGenericErrorService _genericErrorService;

        public NLPService(
            IArtificialIntelligenceExtension artificialIntelligenceExtension,
            MySettings settings,
            IMessagingHubSender sender,
            IMpaService mpaService,
            ILogger logger,
            IGenericErrorService genericErrorService
            )
        {
            _artificialIntelligenceExtension = artificialIntelligenceExtension;
            _settings = settings;
            _sender = sender;
            _mpaService = mpaService;
            _logger = logger;
            _genericErrorService = genericErrorService;
        }

        public async Task<AnalysisResponse> ProcessAsync(string input, Message messageOriginator, CancellationToken cancellationToken)
        {
            var request = new AnalysisRequest
            {
                Text = input
            };

            var analysisResponse = await _artificialIntelligenceExtension.AnalyzeAsync(request, cancellationToken);

            var bestIntention = analysisResponse
                .Intentions
                .OrderByDescending(i => i.Score)
                .FirstOrDefault();

            var bestIntentionAnswer = bestIntention.Answer.Value.ToString();

            if (HasLowConfidence(bestIntention))
            {
                await SendCommandToMPAAsync(DontKnowCommand, messageOriginator, cancellationToken);
                return analysisResponse;
            }

            //Send to MPA
            await _sender.SendMessageAsync(bestIntentionAnswer, messageOriginator.From, cancellationToken);
            await Task.Delay(_settings.MPASettings.WaitBetweenSends);

            await SendCommandToMPAAsync(InitialFlowCommand, messageOriginator, cancellationToken);
            return analysisResponse;
        }

        private async Task SendCommandToMPAAsync(string commmand, Message messageOriginator, CancellationToken cancellationToken)
        {
            var MessageToMPA = new Message(messageOriginator.Id)
            {
                To = messageOriginator.From,
                From = messageOriginator.From,
                Content = PlainText.Parse(commmand),
                Metadata = messageOriginator.Metadata
            };

            var firstNavResult = await _mpaService.SendToMPAAsync(MessageToMPA, cancellationToken);

            if (firstNavResult.NavigationState == NavigationState.Error)
            {
                _logger.Error($"Error on sending message to MPA. User from: {messageOriginator.From} message: {(MessageToMPA.Content as PlainText).Text}");

                var result = await _mpaService.SendToMPAAsync(GenericErrorCommand, messageOriginator.From, cancellationToken);

                if (result.NavigationState == NavigationState.Error)
                {
                    await _genericErrorService.SendGenericErrorAsync(messageOriginator.From, cancellationToken);
                }
            }
        }

        private bool HasLowConfidence(IntentionResponse bestIntention)
        {
            return (bestIntention.Score < _settings.TalkServiceSettings.ConfidenceThreshold);
        }
    }
}
