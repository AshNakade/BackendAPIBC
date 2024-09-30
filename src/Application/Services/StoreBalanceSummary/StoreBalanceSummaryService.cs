using Application.Models.APIRequestResponse.CPInvoice;
using Application.Models.APIRequestResponse.StoreBalanceSummary;
using Application.Models.BC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services.StoreBalanceSummary
{
	public class StoreBalanceSummaryService : IStoreBalanceSummaryService
	{
		readonly IDownstreamApi _downstreamApi;
		public StoreBalanceSummaryService(IDownstreamApi downstreamApi)
		{
			_downstreamApi = downstreamApi;
		}

		public async Task<JsonResult> GetCustomerBalanceSummary(string companyId, StoreBalanceSummaryRequest ListOfCustomerIds)
		{

			try
			{
				if(ListOfCustomerIds is null)
				{
					return new JsonResult("send a list of customers");
				}
				if (ListOfCustomerIds.CustomerIDs is null)
				{
					return new JsonResult("send a list of customers");
				}
				List<StoreBalanceSummaryRequestResponse> funcResponseGetCustomerBalanceSummary = new List<StoreBalanceSummaryRequestResponse>();
				string? BQCPInvoice = Environment.GetEnvironmentVariable("BigQueryCPInvoices");
				if (BQCPInvoice is null)
				{
					return new JsonResult("BigQueryCPInvoices url  is not set in app config ");
				}
				var currentDate = DateTime.Now.ToString("yyyy-MM-dd"); ;
				// as per spec check for last 12 months invoices only 
				string BCCPInvoicesuri = BQCPInvoice + "?$filter=customerPostingGroup eq 'STORE TRADING BMS' and dueDate le  {currentDate}  and  dueDate ge  {lastYear}";
				var last12Months = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");
				string url = BCCPInvoicesuri.Replace("{companyId}", companyId).Replace("{currentDate}", currentDate).Replace("{lastYear}", last12Months);
				BigQuerySalesInvoices? allCPInvoicesForlastYear = await _downstreamApi.CallApiForAppAsync<BigQuerySalesInvoices>("BCAPIDownStream", options => { options.RelativePath = url; });
				if (allCPInvoicesForlastYear is null)
				{
					return new JsonResult(funcResponseGetCustomerBalanceSummary);
				}
				else
				if (allCPInvoicesForlastYear.value is null && allCPInvoicesForlastYear.value?.Count == 0)
					{
						return new JsonResult(funcResponseGetCustomerBalanceSummary);
					}
					else
					{
						var BMSSalesInvoicesGroupByStore = (from BCCPInvoices in allCPInvoicesForlastYear?.value 	
															where ListOfCustomerIds.CustomerIDs.Contains(BCCPInvoices?.billToCustomerId ?? "")
															group BCCPInvoices by BCCPInvoices.billToCustomerId into g
															select new
															{
																CustomerID = g.Key,
																CustomersInvoices = g.ToList()
															}).ToList();
        				
						IDictionary<string, Application.Models.APIRequestResponse.StoreBalanceSummary.StoreBalanceSummary> returnResponse = new Dictionary<string, Application.Models.APIRequestResponse.StoreBalanceSummary.StoreBalanceSummary>();
						foreach ( var eachCustomerInvoicesList in BMSSalesInvoicesGroupByStore	)
						{
							Application.Models.APIRequestResponse.StoreBalanceSummary.StoreBalanceSummary storeBalanceSummary = new Application.Models.APIRequestResponse.StoreBalanceSummary.StoreBalanceSummary();
							storeBalanceSummary.OutstandingBmsBalanceDue = eachCustomerInvoicesList.CustomersInvoices.Sum(x => x.remainingAmount);
							returnResponse.Add(eachCustomerInvoicesList.CustomerID, storeBalanceSummary);
						}
						return new JsonResult(returnResponse);

					} 
				
				
			}
			catch (Exception e)
			{
				return new JsonResult(e.Message);
			}
		}
	}


}

