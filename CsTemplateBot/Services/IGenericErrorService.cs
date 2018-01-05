using Lime.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace CsTemplateBot.Services
{
    public interface IGenericErrorService
    {
        Task SendGenericErrorAsync(Node from, CancellationToken cancellationToken);
    }
}

