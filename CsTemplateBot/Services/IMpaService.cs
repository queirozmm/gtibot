using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;

namespace CsTemplateBot.Services
{
    public interface IMpaService
    {
        Task<NavigationResult> SendToMPAAsync(List<Message> messageList, CancellationToken cancellationToken, Dictionary<string, string> navigationParameters = null);
        Task<NavigationResult> SendToMPAAsync(Message message, CancellationToken cancellationToken, Dictionary<string, string> navigationParameters = null);
        Task<NavigationResult> SendToMPAAsync(string message, Node from, CancellationToken cancellationToken, Dictionary<string, string> navigationParameters = null);
        Task<bool> IsInMPASessionAsync(Node from, CancellationToken cancellationToken);
    }
}
