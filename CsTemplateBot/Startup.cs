using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Metrics;
using NLog;
using Take.MetricsUtils;
using Takenet.MessagingHub.Client.Listener;

namespace CsTemplateBot
{
    public class Startup : IStartable, IStoppable
    {
		public const string LogApplicationName = nameof(CsTemplateBot);

        public Startup(MySettings settings)
        {
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            MetricsConfig(settings.Metrics);
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Metric.Config.WithReporting(config => config.StopAndClearAllReports()); // Allow reports to cleanly shutdown (for instance, writing latest metrics)
            LogManager.Flush(TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void MetricsConfig(MetricsSettings settings)
        {
			if (settings.HttpEndpointPort > 0)
			{
				Metric.Config.WithHttpEndpoint($"http://*:{settings.HttpEndpointPort}/");
			}

			if (!string.IsNullOrWhiteSpace(settings.ReportingConnectionString))
			{
				Metric.Config.WithReporting(config => 
						config.WithReport(
							new SqlMetricsReport(settings.ReportingConnectionString, LogApplicationName), 
							TimeSpan.FromMinutes(settings.ReportingFrequencyInMinutes)));
			}
        }

    }
}
