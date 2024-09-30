using Application.Models.APIRequestResponse.CPInvoice;
using Application.Models.APIRequestResponse.StoreBalanceSummary;
using Application.Services.CPInvoices;
using Application.Services.StoreBalanceSummary;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;
using System.Net;
using System.Text.Json;

namespace Blooms.BC.Integration.Functions
{
    public class StoreBalanceSummary
    {
        private readonly ILogger<StoreBalanceSummary> _logger;
		private readonly IStoreBalanceSummaryService _storeBalanceSummaryService;

		public StoreBalanceSummary(ILogger<StoreBalanceSummary> logger , IStoreBalanceSummaryService StoreBalanceSummaryService)
        {
            _logger = logger;
			_storeBalanceSummaryService = StoreBalanceSummaryService;
		}

        [Function("StoreBalanceSummary")]
		[OpenApiOperation("CustomerBalanceSummary", "CustomerBalanceSummary")]
		[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiParameter(name: "companyID", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **companyID** parameter")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(StoreBalanceSummaryRequest), Required = true, Description = "The **CustomerSearchRequest** parameter")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StoreBalanceSummaryRequestResponse), Description = "The OK response")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies({companyID})/StoreBalanceSummary")] HttpRequest req, string companyID)

		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			try
			{

				var request = await new StreamReader(req.Body).ReadToEndAsync();
				if (string.IsNullOrEmpty(request))
				{
					return new BadRequestObjectResult("Request body is required");
				}
				try
				{
					JsonSerializer.Deserialize<StoreBalanceSummaryRequest>(request);
				}
				catch
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
				StoreBalanceSummaryRequest? StoreBalanceSummaryRequest = JsonSerializer.Deserialize<StoreBalanceSummaryRequest>(request);
				if (StoreBalanceSummaryRequest is null)
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
				var response = await _storeBalanceSummaryService.GetCustomerBalanceSummary(companyID, StoreBalanceSummaryRequest);

				if (response.StatusCode == StatusCodes.Status404NotFound || response == null)
				{
					return new NotFoundResult();
				}
				else if (response.StatusCode == StatusCodes.Status500InternalServerError)
				{
					return new BadRequestObjectResult(response.Value);
				}
				else
				{

					return new OkObjectResult(response.Value);
				}
			}
			catch (RequestFailedException e)
			{
				return new BadRequestObjectResult(e.Message);
			}

		}
	}


}
