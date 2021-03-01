using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BatchProcessing.Services.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcessing.Services
{
    public class ConsumeScopedServiceHostedService : IHostedService
    {
        private readonly ILogger logger;

        public ConsumeScopedServiceHostedService(
            IServiceProvider services,
            ILogger<ConsumeScopedServiceHostedService> logger)
        {
            this.Services = services;
            this.logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                "Consume Scoped Service Hosted Service is starting.");

            this.DoWork();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            this.logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = this.Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IScopedProcessingService>();

                scopedProcessingService.DoWork();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
