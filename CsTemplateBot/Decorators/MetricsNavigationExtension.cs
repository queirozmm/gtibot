using System.Threading;
using System.Threading.Tasks;
using Metrics;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Take.MetricsUtils;

namespace CsTemplateBot.Decorators
{
    public class MetricsNavigationExtension : MetricsWrapperBase, INavigationExtension
    {
        private readonly INavigationExtension _navigationExtension;

        public MetricsNavigationExtension(INavigationExtension navigationExtension)
            : base(metricsNamePrefix: "Mpa", rateUnit: TimeUnit.Minutes)
        {
            _navigationExtension = navigationExtension;
        }

        public async Task<NavigationResponse> GetNavigationAsync(NavigationRequest request, CancellationToken cancellationToken)
        {
            return await RunRequestAsync(async () =>
            {
                var result = await _navigationExtension.GetNavigationAsync(request, cancellationToken);
                if (result.State == NavigationState.Error) RequestErrorMeter.Mark();
                return result;
            });
        }

        public Task<NavigationResult> ExecuteNavigationAsync(NavigationResponse response, CancellationToken cancellationToken)
        {
            return _navigationExtension.ExecuteNavigationAsync(response, cancellationToken);
        }

        public Task<NavigationResult> ExecuteNavigationAsync(NavigationResponse response, CancellationToken cancellationToken, string distributionList)
        {
            return _navigationExtension.ExecuteNavigationAsync(response, cancellationToken, distributionList);
        }
    }
}
