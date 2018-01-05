using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace CsTemplateBot.Receivers
{
    public class AlertCommandReceiver : ICommandReceiver
    {
        private readonly IMessagingHubSender _sender;

        public AlertCommandReceiver(IMessagingHubSender sender)
        {
            _sender = sender;
        }

        public async Task ReceiveAsync(Command command, CancellationToken cancellationToken)
        {
            var response = new Command
            {
                Id = command.Id,
                To = command.From,
                Method = command.Method,
                Status = CommandStatus.Success,
                Resource = new PlainText { Text = "Ok" }
            };

            var query = command.Uri.Path.Split('?').Last();
            if (query == "mpa")
            {
                // Put your check logic here
                // If failure:
                SetFailure(response, "Descriptive failure message");
            }
            else
            {
                response.Status = CommandStatus.Failure;
                response.Reason = new Reason { Code = ReasonCodes.COMMAND_RESOURCE_NOT_FOUND, Description = "Invalid command" };
            }

            await _sender.SendCommandResponseAsync(response, cancellationToken);
        }

        private static void SetFailure(Command response, string alertDescription)
        {
            response.Resource = null;
            response.Status = CommandStatus.Failure;
            response.Reason = new Reason { Code = ReasonCodes.APPLICATION_ERROR, Description = alertDescription };
        }
    }
}
