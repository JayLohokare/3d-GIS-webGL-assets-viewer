﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OrchestrationService
{
    class Health
    {
        [FunctionName("Health")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "health")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("Success");
        }
    }
}
