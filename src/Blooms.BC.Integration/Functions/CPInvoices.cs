using Application.Models.APIRequestResponse.CPInvoice;
using Application.Models.APIRequestResponse.CustomerSearch;
using Application.Services.CPInvoices;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;
namespace Blooms.BC.Integration.Functions
{
    public class CPInvoices
	{
        private readonly ILogger<CPInvoices> _logger;
        private readonly ICPInvoicesService _cPInvoicesService;

		public CPInvoices(ILogger<CPInvoices> logger, ICPInvoicesService CPInvoicesService)
        {
            _logger = logger;
			_cPInvoicesService = CPInvoicesService;
		}
		[Function("CPInvoices")]
		[OpenApiOperation("CPInvoices", "CPInvoices")]
		[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiParameter(name: "companyID", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **companyID** parameter")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CPInvoiceRequest), Required = true, Description = "The **CustomerSearchRequest** parameter")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CPInvoiceResponse), Description = "The OK response")]
		
       
       public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies({companyID})/CPInvoices")] HttpRequest req, string companyID)
 
		{
			_logger.LogInformation("CPInvoices function processed a request.");
			try
			{
				var request = await new StreamReader(req.Body).ReadToEndAsync();
				if (string.IsNullOrEmpty(request))
				{
					return new BadRequestObjectResult("Request body is required");
				}
				try
				{
					JsonSerializer.Deserialize<CPInvoiceRequest>(request);
				}
				catch
				{
					return new BadRequestObjectResult("Invalid Request body");
				}

				CPInvoiceRequest? cPInvoiceRequest = JsonSerializer.Deserialize<CPInvoiceRequest>(request);
				if (cPInvoiceRequest is null)
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
				var response = await _cPInvoicesService.GetCPInvoices(companyID, cPInvoiceRequest);

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
