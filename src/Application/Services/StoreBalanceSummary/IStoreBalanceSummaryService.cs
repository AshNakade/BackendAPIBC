using Application.Models.APIRequestResponse.StoreBalanceSummary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.StoreBalanceSummary
{
	public  interface IStoreBalanceSummaryService
	{
		Task<JsonResult> GetCustomerBalanceSummary(string customerId , StoreBalanceSummaryRequest ListOfCustomerIds);
	}
}
