using Lime.Messaging.Contents;
using Lime.Protocol;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Sender;

namespace CsTemplateBot.Services
{
    public class GenericErrorService : IGenericErrorService
    {
        private readonly IMessagingHubSender _sender;
        private readonly MySettings _settings;
        private readonly ILogger _logger;

        public GenericErrorService(IMessagingHubSender sender, MySettings settings, ILogger logger)
        {
            _sender = sender;
            _settings = settings;
            _logger = logger;
        }

       public async Task SendGenericErrorAsync(Node from, CancellationToken cancellationToken)
        {
            var documentErrorList = new List<Document>();

            var document = new Select
            {
                Text = $"Ops, tivemos um problema. Vamos começar novamente.",
                Options = new SelectOption[] {
                    new SelectOption {
                        Text = "Início",
                        Value = new PlainText { Text = "Início" }
                    } }
            };

            documentErrorList.Add(document);

            foreach (var item in documentErrorList)
            {
                try
                {
                    await _sender.SendMessageAsync(item, from, cancellationToken);
                    await Task.Delay(_settings.MPASettings.WaitBetweenSends);

                }
                catch (Exception e)
                {
                    _logger.Fatal(e, $"Fatal error on GenericErrorService.");
                    throw new Exception("Fatal error on GenericErrorService.", e);
                }
            }
        }

    }
}

