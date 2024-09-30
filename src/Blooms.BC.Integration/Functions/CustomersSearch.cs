using Application.Models.APIRequestResponse.CustomerSearch;
using Application.Models.BC;
using Application.Services.CustoemrSearch;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;
using System.Text.Json;

namespace BCFunctions.Functions
{
    public class CustomersSearch
    {

        private readonly ICustomersSearch _customersSearch;
        private readonly ILogger<CustomersSearch> _logger;
        public CustomersSearch(ILogger<CustomersSearch> logger, ICustomersSearch customersSearch)
        {
            _logger = logger;
            _customersSearch = customersSearch;
        }


        [Function("CustomersSearch")]
        [OpenApiOperation("CustomersSearch", "CustomersSearch")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "companyID", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **companyID** parameter")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CustomerSearchRequest), Required = true, Description = "The **CustomerSearchRequest** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CustomerSearchResponse), Description = "The OK response")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies({companyID})/CustomerSearch")] HttpRequest req, string companyID)
        {
            _logger.LogInformation("CustomersSearch function processed a request.");
            try
            {

				// Read json pay laod from request body
				var request = await new StreamReader(req.Body).ReadToEndAsync();
				if (string.IsNullOrEmpty(request))
				{
					return new BadRequestObjectResult("Request body is required");
				}
				try
				{
					JsonSerializer.Deserialize<CustomerSearchRequest>(request);
				}
				catch
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
				CustomerSearchRequest? customerSearchRequest = JsonSerializer.Deserialize<CustomerSearchRequest>(request) ;
				if (customerSearchRequest is null)
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
				var response = await _customersSearch.SearchCustomers(customerSearchRequest, companyID);
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

		[Function("ConsultingPartnerSearch")]
		[OpenApiOperation("ConsultingPartnerSearch", "ConsultingPartnerSearch")]
		[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiParameter(name: "companyID", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **companyID** parameter")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ConsultingPartnerSearchRequest), Required = true, Description = "The **CustomerSearchRequest** parameter")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ConsultingPartnerSearchResponse), Description = "The OK response")]
		public async Task<IActionResult> ConsultingPartnerSearch([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies({companyID})/ConsultingPartnerSearch")] HttpRequest req, string companyID)
		{
			_logger.LogInformation("ConsultingPartnerSearch function processed a request.");
			try
			{

				// Read json pay laod from request body
				var request = await new StreamReader(req.Body).ReadToEndAsync();
				if (string.IsNullOrEmpty(request))
				{
					return new BadRequestObjectResult("Request body is required");
				}
				try
				{
					 JsonSerializer.Deserialize<ConsultingPartnerSearchRequest>(request);
				}
				catch
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
			    ConsultingPartnerSearchRequest? consultingPartnerSearchRequest = JsonSerializer.Deserialize<ConsultingPartnerSearchRequest>(request);
				if (consultingPartnerSearchRequest is null)
				{
					return new BadRequestObjectResult("Invalid Request body");
				}
				var response = await _customersSearch.SearchConsultingPartners(consultingPartnerSearchRequest, companyID);
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


		[Function("GetCustomer")]
        [OpenApiOperation("GetCustomer", "GetCustomer")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "companyID", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **companyID** parameter")]
        [OpenApiParameter(name: "customerNo", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **customerID** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CustomerSearchResponse), Description = "The OK response")]

        public async Task<IActionResult> GetCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies({companyID})/GetCustomer/{customerId}")] HttpRequest req, string companyID, string customerId)
        {
            _logger.LogInformation("GetCustomer function processed a request.");
            try
            {
                var response = await _customersSearch.GetCustomer(companyID, customerId);

                if (response.StatusCode == StatusCodes.Status404NotFound  || response == null)
                {
                    return new NotFoundResult();
                }
                else if ( response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    return new BadRequestObjectResult(response.Value);
                }else
                { 

                return new OkObjectResult(response.Value);
                }

         
            }
            catch (RequestFailedException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }


		[Function("GetConsultingPartner")]
		[OpenApiOperation("GetConsultingPartner", "GetConsultingPartner")]
		[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiParameter(name: "companyId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **companyID** parameter")]
		[OpenApiParameter(name: "consultingPartnerId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **ConsultingPartnerID** parameter")]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(GetCPPartnerSearchRequest), Required = true, Description = "The **CustomerSearchRequest** parameter")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetConsultingPartnerResponse), Description = "The OK response")]

		public async Task<IActionResult> GetConsultingPartner([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies({companyID})/GetConsutingPartner/{customerId}")] HttpRequest req, string companyID , string customerId )
		{
			_logger.LogInformation("GetConsultingPartner function processed a request.");
			try
			{

				var request = await new StreamReader(req.Body).ReadToEndAsync();
				if (string.IsNullOrEmpty(request))
				{
					return new BadRequestObjectResult("Request body is required");
				}
				try
				{
					JsonSerializer.Deserialize<GetCPPartnerSearchRequest>(request);
				}
				catch
				{
					return new BadRequestObjectResult("Invalid Request body");
				}



				// Read the CP  ID from Path  parameter
				string ConsultingPartnerID = customerId;
				// Read the store ID from Request Body 
				string storeId = string.Empty;
				GetCPPartnerSearchRequest? CPPartnerSearchRequest = JsonSerializer.Deserialize<GetCPPartnerSearchRequest>(request);
				if (CPPartnerSearchRequest is null)
				{
					return new BadRequestObjectResult("ConsultingPartnerID and StoreId are required");
				}
				if (CPPartnerSearchRequest.storeId is null) {
					return new BadRequestObjectResult("ConsultingPartnerID and StoreId are required");
				}
				storeId = CPPartnerSearchRequest.storeId;
							
				if (string.IsNullOrEmpty(ConsultingPartnerID) || string.IsNullOrEmpty(storeId))
				{
					return new BadRequestObjectResult("ConsultingPartnerID and StoreId are required");
				}



				var response = await _customersSearch.GetConsultingPartner(companyID, storeId, ConsultingPartnerID);


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
