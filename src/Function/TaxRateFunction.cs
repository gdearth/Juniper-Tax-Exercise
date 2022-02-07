using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;
using Function.Entity;
using Interface.BusinessUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Function
{
    public class TaxRateFunction
    {
        private readonly ITaxBu _taxBu;

        public TaxRateFunction(ITaxBu taxBu)
        {
            _taxBu = taxBu;
        }

        [FunctionName("TaxRate")]
        [OpenApiOperation(operationId: "TaxRate", tags: new[] { "Tax Rate" })]
        [OpenApiParameter(name: "country", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Two-letter ISO country code for given location.")]
        [OpenApiParameter(name: "zip", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Postal code for given location (5-Digit ZIP or ZIP+4).")]
        [OpenApiParameter(name: "state", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Two-letter ISO state code for given location.")]
        [OpenApiParameter(name: "city", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "City for given location.")]
        [OpenApiParameter(name: "street", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Street address for given location.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(TaxRateResponse), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log, CancellationToken token)
        {
            var location = new Location
            {
                ZipCode = req.Query["zip"],
                CountryCode = req.Query["country"],
                State = req.Query["state"],
                City = req.Query["city"],
                Street = req.Query["street"]
            };
            
            var response =  new TaxRateResponse
            {
                TaxRate = await _taxBu.GetTaxRateByLocation(location, token)
            };
            
            return new OkObjectResult(response);
        }
    }
}

