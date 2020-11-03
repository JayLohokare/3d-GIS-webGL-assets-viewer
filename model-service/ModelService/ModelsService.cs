using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace ModelService
{
    public class ModelsService
    {
        private readonly ModelDataContext _DbContext;
        public ModelsService(ModelDataContext dbContext)
        {
            _DbContext = dbContext;
        }

        [FunctionName("Models")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "models/")] HttpRequest req,
            ILogger log)
        {
            var result = _DbContext.Model.ToList();
            return new OkObjectResult(result);
        }
    }
}
