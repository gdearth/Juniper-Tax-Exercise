using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;
using Function.Entity;
using Interface.BusinessUtility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Function;

public class TaxCollectionFunction
{
    private readonly ITaxBu _taxBu;

    public TaxCollectionFunction(ITaxBu taxBu)
    {
        _taxBu = taxBu;
    }
    
    [FunctionName("TaxCollection")]
    [OpenApiOperation(operationId: "TaxRate", tags: new[] { "Tax Collection" })]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(Order), Description = "**Order** Body", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: MediaTypeNames.Application.Json, bodyType: typeof(TaxCollectionResponse), Description = "The OK response")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log, CancellationToken token)
    {
        
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var order = JsonConvert.DeserializeObject<Order>(requestBody);

        var response = new TaxCollectionResponse
        {
            AmountToCollect = await _taxBu.GetTaxByOrder(order, token)
        };

        return new OkObjectResult(response);
    }
}