using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sample.AzureFunction
{
    public class HealthCheckHttpTrigger
    {
        private readonly HealthCheckService healthCheckService;

        public HealthCheckHttpTrigger(HealthCheckService healthCheckService)
        {
            this.healthCheckService = healthCheckService;
        }

        [FunctionName(nameof(HealthCheckHttpTrigger))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var healthReport = await healthCheckService.CheckHealthAsync(cancellationToken);

            return healthReport.Status == HealthStatus.Unhealthy
                ? new ObjectResult(healthReport) { StatusCode = StatusCodes.Status503ServiceUnavailable }
                : new OkObjectResult(healthReport);
        }
    }
}