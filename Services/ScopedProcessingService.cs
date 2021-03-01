using Microsoft.Extensions.Logging;
using BatchProcessing.Services.Abstract;

namespace BatchProcessing.Services
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger logger;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
        {
            this.logger = logger;
        }

        public void DoWork()
        {
            this.logger.LogInformation("Scoped Processing Service is working.");
        }
    }
}
