using Lime.Protocol;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using CsTemplateBot.Extensions;

namespace CsTemplateBot.Services
{
    public class ContextManager : IContextManager
    {
        private const string FirstInteraction = nameof(FirstInteraction);
        private const string Suspended = nameof(Suspended);

        private readonly IBucketExtension _bucket;

        public ContextManager(IBucketExtension bucket)
        {
            _bucket = bucket;
        }

        public async Task<bool> IsFirstInteractionAsync(Node from, CancellationToken cancellationToken)
        {
            var content = await _bucket.GetStringAsync(GetBucketKey(from, FirstInteraction), cancellationToken);
            return string.IsNullOrWhiteSpace(content);
        }

        public Task SetFirstInteractionAsync(Node from, CancellationToken cancellationToken)
        {
            return _bucket.SetStringAsync(GetBucketKey(from, FirstInteraction), false.ToString(), cancellationToken);
        }

        public async Task<bool> IsSuspendedAsync(Node from, CancellationToken cancellationToken)
        {
            var content = await _bucket.GetStringAsync(GetBucketKey(from, Suspended), cancellationToken);
            return !string.IsNullOrWhiteSpace(content) && bool.Parse(content);
        }

        public Task SetSuspendedAsync(Node from, bool value, CancellationToken cancellationToken)
        {
            return _bucket.SetStringAsync(GetBucketKey(from, Suspended), value.ToString(), cancellationToken);
        }

        private string GetBucketKey(Node from, string keySuffix)
            => $"{from.Name}_{keySuffix}";

    }
}
