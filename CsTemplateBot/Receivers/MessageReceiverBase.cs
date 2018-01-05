using Lime.Messaging.Contents;
using Lime.Protocol;
using NLog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Services;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using Take.MetricsUtils;
using CsTemplateBot.Services;

namespace CsTemplateBot.Receivers
{
      public abstract class MessageReceiverBase : MetricsWrapperBase, IMessageReceiver
{
        private readonly string ACTIVATION_COMMAND = "#ATIVAR";
		private readonly string FIRST_INTERACTION_COMMAND = "#WELCOME";
        protected readonly IMessagingHubSender _sender;
        protected readonly IContextManager _context;
		protected readonly IContactService _contactService;
        protected readonly ILogger _logger;
		private readonly MySettings _settings;
        protected readonly IMpaService _mpaService;
		protected readonly IGenericErrorService _genericErrorService;

        private readonly string _receiverName;

        public MessageReceiverBase(
			IMessagingHubSender sender, 
			MySettings settings, 
			IContextManager context, 
			IContactService contactService,
			IMpaService mpaService,
            IGenericErrorService genericErrorService,
			ILogger logger)
			: base()
        {
            _sender = sender;
            _context = context;
			_contactService = contactService;
            _logger = logger;

            _receiverName = GetType().Name;
			_settings = settings;
            _mpaService = mpaService;
			_genericErrorService = genericErrorService;
        }

        public Task ReceiveAsync(Message message, CancellationToken cancellationToken = default(CancellationToken))
        {
			MappedDiagnosticsLogicalContext.Set("originator", message.From.ToIdentity().ToString());
			return RunRequestAsync(async () =>
			{
                try
                {
					var contact = await _contactService.GetContactAsync(message.From, cancellationToken);
                    if (await IsActiveAsync(message, cancellationToken))
                    {
                        await SendComposingAsync(message.From, cancellationToken);
                        LogRequest(message);
						
						var isInMPASession = await _mpaService.IsInMPASessionAsync(message.From, cancellationToken);
                        var isFirstInteration = await _context.IsFirstInteractionAsync(message.From, cancellationToken);

                        if (!isInMPASession && isFirstInteration)
                        {
                            message.Content = new PlainText { Text = FIRST_INTERACTION_COMMAND };
                            await _context.SetFirstInteractionAsync(message.From, cancellationToken);
                        }

                        await ReceiveMessageAsync(message, cancellationToken);
                    }
                    else
                    {
                        // Do nothing!
                        if (message.Content.ToString().ToLower().Contains(ACTIVATION_COMMAND.ToLower()))
                        {
                            try
                            {
                                await _context.SetSuspendedAsync(message.From, false, cancellationToken);
                                var result = await _mpaService.SendToMPAAsync(FIRST_INTERACTION_COMMAND, message.From, cancellationToken);
                            }
                            catch(Exception e)
                            {
                                _logger.Error($"Error on activate user! User Node: {message.From}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
					_logger.Error(e, $"Some error ocurred. Trying to send Error Message to user. User: {message.From}");
                    await OnExceptionAsync(message, e, cancellationToken);
                }
            });
        }

        protected abstract Task ReceiveMessageAsync(Message message, CancellationToken cancellationToken);

        protected async Task<bool> IsActiveAsync(Message message, CancellationToken cancellationToken)
        {
            return !(await _context.IsSuspendedAsync(message.From, cancellationToken));
        }

		 protected async Task OnExceptionAsync(Message message, Exception e, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Error(e, $"Exception while processing on {_receiverName}");
                await _genericErrorService.SendGenericErrorAsync(message.From, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"Error on Sending Error message to user. USER IS WITHOUT RESPONSE! User: {message.From}");
            }
        }

        private async Task SendComposingAsync(Node destination, CancellationToken cancellationToken)
        {
            await _sender.SendMessageAsync(
				new Message
				{
					Id = null,
					To = destination,
					Content = new ChatState { State = ChatStateEvent.Composing }
				},
            cancellationToken);
        }

		private void LogRequest(Message message)
        {
            var logMessage = $"Receiver: {GetType().Name}\tFrom: {message.From}\tContent: {message.Content}";
            _logger.Debug(logMessage);
        }
    }
}
