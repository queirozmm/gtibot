using Lime.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace CsTemplateBot.Services
{
    public interface IContextManager
    {
        Task<bool> IsFirstInteractionAsync(Node from, CancellationToken cancellationToken);
		Task SetFirstInteractionAsync(Node from, CancellationToken cancellationToken);

        Task<bool> IsSuspendedAsync(Node from, CancellationToken cancellationToken);
        Task SetSuspendedAsync(Node from, bool value, CancellationToken cancellationToken);
    }
}

