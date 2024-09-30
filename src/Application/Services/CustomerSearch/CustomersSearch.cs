using Application.Models.APIRequestResponse.CustomerSearch;
using Application.Models.BC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using System;
using System.ComponentModel.Design;


namespace Application.Services.CustoemrSearch

{
    public class CustomerSearch : ICustomersSearch
    {
        readonly IDownstreamApi _downstreamApi;
         public CustomerSearch(IDownstreamApi downstreamApi)
        {
            _downstreamApi = downstreamApi;
        }
        public async Task<JsonResult> SearchCustomers(CustomerSearchRequest customerSearchRequest, string companyId)
        {
            try
            {
                CustomerSearchResponse customerSearchResponse = new CustomerSearchResponse();
				customerSearchResponse.pageNumber = customerSearchRequest.pageNumber;
                string? BQCustomerUrl = Environment.GetEnvironmentVariable("BigQueryCustomers");
				if (BQCustomerUrl is null)
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
				string getCustomerUrl = BQCustomerUrl + "?$filter=displayName eq '{SearchString}' and ( generalBusinessPostingGroup eq 'STORE W PHARM MGMT' or generalBusinessPostingGroup eq 'STORE W/O PHARM MGMT' ) &$top=10&$skip={PageNumber}&$orderby=displayName"; 
				string pageNo = customerSearchRequest.pageNumber > 0 ? (customerSearchRequest.pageNumber * 10).ToString() : "0";
                string url = getCustomerUrl.Replace("{companyId}", companyId).Replace("{SearchString}", "*"+customerSearchRequest.searchString+"*").Replace("{PageNumber}", pageNo);
				BigQueryCustomersV2? response = await _downstreamApi.CallApiForAppAsync<BigQueryCustomersV2>("BCAPIDownStream", options => { options.RelativePath = url; });
				if (response is null || response.value is null)
				{
					return new JsonResult(customerSearchResponse) { StatusCode = StatusCodes.Status404NotFound }; 
				}
				if (response.value.Count == 0)
				{
				return new JsonResult(customerSearchResponse) { StatusCode = StatusCodes.Status404NotFound };
				}
				foreach (var value in response.value)
				{
					CustomerResultset Customerresultset = new CustomerResultset();
					Customerresultset.customerName = value?.displayName;
					Customerresultset.customerId = value?.id;
					customerSearchResponse.resultSet?.Add(Customerresultset);
				}
				return new JsonResult(customerSearchResponse);
            }
			catch (Exception e)
			{
				return new JsonResult("Error on Server  " + e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
			}

		}
		public async Task<JsonResult> SearchConsultingPartners(ConsultingPartnerSearchRequest ConsultingPartnerSearchRequest, string companyId)
		{
			try
			{
				ConsultingPartnerSearchResponse consultingPartnerSearchResponse = new ConsultingPartnerSearchResponse();
				consultingPartnerSearchResponse.pageNumber = ConsultingPartnerSearchRequest.pageNumber;
				string? bqCustomerRelationships = Environment.GetEnvironmentVariable("BigQueryCustomerRelationships");
				if (bqCustomerRelationships is null)
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
					// Check If store ID Is passed in the request JSON 
					try 
					{
					var guid = new Guid(ConsultingPartnerSearchRequest.storeId);
					}
					catch 
					{ 
						return new JsonResult("Incorrect Store Id format") { StatusCode = StatusCodes.Status400BadRequest }; 
					}
					string getConsultingPartnerUrl = bqCustomerRelationships + "?$filter=  relationshipCustomerPostingGroup eq 'CONSULTING PARTNER' and customerId eq {storeId}  and relationshipCustomerName eq '{SearchString}' &$top=10&$skip={PageNumber}&$orderby=relationshipCustomerName";
					string pageNo = ConsultingPartnerSearchRequest.pageNumber > 0 ? (ConsultingPartnerSearchRequest.pageNumber * 10).ToString() : "0";
					string url =  getConsultingPartnerUrl.Replace("{companyId}", companyId).Replace("{storeId}", ConsultingPartnerSearchRequest.storeId).Replace("{SearchString}", "*"+ ConsultingPartnerSearchRequest.searchString+"*").Replace("{PageNumber}", pageNo);
					BigQueryCustomerRelationships? response = await _downstreamApi.CallApiForAppAsync<BigQueryCustomerRelationships>("BCAPIDownStream", options => { options.RelativePath = url; });
				if (response is null || response.value is null)
				{
					return new JsonResult(consultingPartnerSearchResponse) { StatusCode = StatusCodes.Status404NotFound };
				}
				if ( response.value.Count == 0)
				{
					return new JsonResult(consultingPartnerSearchResponse) { StatusCode = StatusCodes.Status404NotFound };
				}
				foreach (var value in response.value)
				{
					CPResultset CPresultset = new CPResultset();
					CPresultset.consultingPartnerName = value?.relationshipCustomerName;
					CPresultset.consultingPartnerId = value?.relationshipCustomerId;
					CPresultset.AccountName = value?.relationshipBankName;
					CPresultset.BSB = value?.relationshipBankBranchNo;
					CPresultset.AccountNumber = value?.relationshipBankAccountNo;
					consultingPartnerSearchResponse.resultSet?.Add(CPresultset);
				}
     			return new JsonResult(consultingPartnerSearchResponse);
			}
			catch (Exception e)
			{
				return new JsonResult("Error on Server  " + e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
			}   

		}
		public async Task<JsonResult> GetCustomer(string companyId, string customerNo)
		{
			try
			{
				GetCustomerResponse getCustomerResponse = new GetCustomerResponse();
				string? BQCustomerUrl = Environment.GetEnvironmentVariable("BigQueryCustomers");
				if (BQCustomerUrl is null)
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
				string getCustomerUrl = BQCustomerUrl + "?$filter=id eq {customerID} and ( generalBusinessPostingGroup eq 'STORE W PHARM MGMT' or generalBusinessPostingGroup eq 'STORE W/O PHARM MGMT' )";
					string url = getCustomerUrl.Replace("{companyId}", companyId).Replace("{customerID}", customerNo);
					BigQueryCustomersV2? response = await _downstreamApi.CallApiForAppAsync<BigQueryCustomersV2>("BCAPIDownStream", options => { options.RelativePath = url; });
					if (response is  null || response.value is null ) 
					{
					return new JsonResult(getCustomerResponse) { StatusCode = StatusCodes.Status404NotFound };
					}
					if (response.value.Count == 0)
					{
						return new JsonResult(getCustomerResponse) { StatusCode = StatusCodes.Status404NotFound };
					}
					getCustomerResponse.customerId = response.value[0].id;
					getCustomerResponse.customerName = response.value[0].displayName;
					return new JsonResult(getCustomerResponse);
			}
			catch (Exception e)
			{

				return new JsonResult("Error on Server  " + e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
			}

		}
		public async Task<JsonResult> GetConsultingPartner(string companyId, string storeId, string consultingPartnerId)
        {

			try
			{
				GetConsultingPartnerResponse getConsultingPartnerResponse = new GetConsultingPartnerResponse();
				//string? getConsultingPartnerUrl = Environment.GetEnvironmentVariable("GetConsultingPartner");
				string? bqCustomerRelationships = Environment.GetEnvironmentVariable("BigQueryCustomerRelationships");
				if (bqCustomerRelationships is null)
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
					string getConsultingPartnerUrl = bqCustomerRelationships + "?$filter=  relationshipCustomerPostingGroup eq 'CONSULTING PARTNER' and customerId eq {storeId}  and relationshipCustomerId eq {cpID} ";
					string url = getConsultingPartnerUrl.Replace("{companyId}", companyId).Replace("{storeId}", storeId).Replace("{cpID}", consultingPartnerId);
					BigQueryCustomerRelationships? response = await _downstreamApi.CallApiForAppAsync<BigQueryCustomerRelationships>("BCAPIDownStream", options => { options.RelativePath = url; });
					if (response is  null || response.value is null )
					{
					return new JsonResult(getConsultingPartnerResponse) { StatusCode = StatusCodes.Status404NotFound };
				   }
				  if ( response.value.Count == 0)
				  {
					return new JsonResult(getConsultingPartnerResponse) { StatusCode = StatusCodes.Status404NotFound };
				  }
				getConsultingPartnerResponse.consultingPartnerId = response?.value[0]?.relationshipCustomerId;
				getConsultingPartnerResponse.consultingPartnerName = response?.value[0]?.relationshipCustomerName;
				getConsultingPartnerResponse.AccountName = response?.value[0]?.relationshipBankName;
				getConsultingPartnerResponse.BSB = response?.value[0]?.relationshipBankBranchNo;
				getConsultingPartnerResponse.AccountNumber = response?.value[0]?.relationshipBankAccountNo;
				return new JsonResult(getConsultingPartnerResponse);
			}
			catch (Exception e)
			{
				return new JsonResult("Error on Server  " + e.Message) { StatusCode = StatusCodes.Status500InternalServerError };
			}

		}

	}
}
