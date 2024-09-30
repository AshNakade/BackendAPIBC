using Application.Models.APIRequestResponse.CustomerSearch;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.CustoemrSearch
{
    public interface ICustomersSearch
    {
        Task<JsonResult> SearchCustomers(CustomerSearchRequest customerSearch, string companyId);
        Task<JsonResult> GetCustomer(string companyId, string customerNo);

		Task<JsonResult> SearchConsultingPartners(ConsultingPartnerSearchRequest consultingPartnerSearchRequest, string companyId);
		Task<JsonResult> GetConsultingPartner(string companyId, string storeId , string consultingPartnerId);

	}
}
