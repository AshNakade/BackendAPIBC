using Application.Models.APIRequestResponse.CPInvoice;
using Microsoft.Identity.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections;
using Application.Models.APIRequestResponse.CustomerSearch;
using Application.Models.BC;
using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;
using static Application.Models.APIRequestResponse.CPInvoice.CPInvoiceResponse;

namespace Application.Services.CPInvoices
{
	public class CPInvoicesService : ICPInvoicesService
	{
		readonly IDownstreamApi _downstreamApi;
		public CPInvoicesService(IDownstreamApi downstreamApi)
		{
			_downstreamApi = downstreamApi;
		}

		public async Task<JsonResult> GetCPInvoices(string companyId, CPInvoiceRequest CPInvoiceRequest)
		{

			try
			{
				if (CPInvoiceRequest is null)
{
					return new JsonResult("Null Reuqest passed ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
				if (CPInvoiceRequest.CustomerIDs is null)
				{
					return new JsonResult("Null Reuqest passed ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
			
				IDictionary<string, IDictionary<string, internalPartnerInvoices>> storeInvoices = new Dictionary<string, IDictionary<string, internalPartnerInvoices>>();
		     	//List <CPInvoiceResponse> FunctCPInvoiceResponse = new List<CPInvoiceResponse>();
				string? BQCustomerUrl = Environment.GetEnvironmentVariable("BigQueryCustomers");
				if (BQCustomerUrl is null )
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
				// Get Customer No for Customer ID sent in the http request of the Azzure Funtion 
				// Call Business Central Get Customer End point to get the Customer No for the Customer ID
				string url = BQCustomerUrl;
				//+ "?$filter=customerPostingGroup eq 'STORE TRADING BMS'"; 
				string? StoreSearchurl = url.Replace("{companyId}" , companyId);
				BigQueryCustomersV2? allTradingStores = await _downstreamApi.CallApiForAppAsync<BigQueryCustomersV2>("BCAPIDownStream", options => { options.RelativePath = StoreSearchurl; });
				if (allTradingStores?.value is null || allTradingStores?.value?.Count() == 0)
				{
					return new JsonResult("No Customers Found") { StatusCode = StatusCodes.Status404NotFound };
				}
				var allTradingStoreswithDimentionsCode = (from store in allTradingStores?.value 
														  where CPInvoiceRequest.CustomerIDs.Contains(store?.id ?? "" )
											              select store).ToList();
				string? BQCPInvoice = Environment.GetEnvironmentVariable("BigQueryCPInvoices");
				if (BQCPInvoice is null)
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
				if (allTradingStoreswithDimentionsCode.Count() > 0)
				{
					string? BCCPInvoicesuri = BQCPInvoice + "?$filter=customerPostingGroup eq 'CONSULTING PARTNER' and dueDate le  {currentDate}  and  dueDate ge  {lastYear}  &$expand=salesInvoiceLines($filter=genProdPostingGroup eq 'LOAN INTEREST')"; 
					var currentDate = DateTime.Now.ToString("yyyy-MM-dd"); ;
					// as per spec check for last 12 months invoices only 
					var last12Months = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");
					url =BCCPInvoicesuri.Replace("{companyId}", companyId).Replace("{currentDate}" , currentDate).Replace("{lastYear}" , last12Months);
					BigQuerySalesInvoices? allCPInvoicesForlastYear = await _downstreamApi.CallApiForAppAsync<BigQuerySalesInvoices>("BCAPIDownStream", options => { options.RelativePath = url; });

				if (allCPInvoicesForlastYear is null || allCPInvoicesForlastYear.value is null )
				{
					return new JsonResult(storeInvoices);
				}
				if (allCPInvoicesForlastYear.value?.Count == 0 )
				{
					return new JsonResult(storeInvoices);
				}
				// Load Customer relationship in the memory 
				string? bqCustomerRelationships = Environment.GetEnvironmentVariable("BigQueryCustomerRelationships");
				if (bqCustomerRelationships is null)
				{
					return new JsonResult("Set Back end and Base url in app settings ") { StatusCode = StatusCodes.Status500InternalServerError };
				}
				string? customerRelationshipsUrl = bqCustomerRelationships;
				//+ "?$filter=  relationshipCustomerPostingGroup eq 'WORKING PARTNER'";
				url = customerRelationshipsUrl.Replace("{companyId}", companyId);
				BigQueryCustomerRelationships? storeCPBankRelationship = await _downstreamApi.CallApiForAppAsync<BigQueryCustomerRelationships>("BCAPIDownStream", options => { options.RelativePath = url; });
				if (storeCPBankRelationship?.value is null && storeCPBankRelationship?.value?.Count == 0)
				{
					return new JsonResult("No Customer Relationships Found") { StatusCode = StatusCodes.Status404NotFound };
				}
				var CPInvoicesListForStore = ( from BCCPInvoices in allCPInvoicesForlastYear.value
				    								where (BCCPInvoices?.salesInvoiceLines?.Count > 0)
													join Store in allTradingStoreswithDimentionsCode on BCCPInvoices.shortcutDimension4Code equals Store.number
													select new
												    {
													   BCCPInvoices,
													   Store
													} into T1
													group T1 by T1.BCCPInvoices.shortcutDimension4Code into g
												    select new
												    {
													  storeDimentionCode = g.Key,
													  storeID = g.Select(x => x.Store.id).FirstOrDefault(),
													  storeCPInvoices = g.ToList()
												    }).ToList();
						foreach (var eachStoreInvoicesList in CPInvoicesListForStore)
						{
							IDictionary<string, internalPartnerInvoices> partnerInvoices = new Dictionary<string, internalPartnerInvoices>();
							var groupInvoicesForBillToCustomer = eachStoreInvoicesList.storeCPInvoices.GroupBy(x => x.BCCPInvoices.billToCustomerId) ;
							foreach (var eachConsultingPatnerInvoices in groupInvoicesForBillToCustomer)
							{
								internalPartnerInvoices ListOfInvoicesOfthePartner = new internalPartnerInvoices();
								ListOfInvoicesOfthePartner.Name = eachConsultingPatnerInvoices.Select(x => x.BCCPInvoices.billToCustomerName).FirstOrDefault();
								// Check if Bank account details are available for the Consulting Partner

								// If Bank details are Null which suggest no relation ship exist sent empty string
								decimal unpaidAmount = 0;
								string? BankAccountName = storeCPBankRelationship?.value?.Where(x => x.customerId == eachStoreInvoicesList?.storeID).Where(x => x?.relationshipCustomerId == eachConsultingPatnerInvoices?.Key).Select(x => x?.relationshipBankName)?.FirstOrDefault() ?? string.Empty;
								ListOfInvoicesOfthePartner.BankAccountName = BankAccountName is null ? "" : BankAccountName;
								string BankAccountNumber = storeCPBankRelationship?.value?.Where(x => x.customerId == eachStoreInvoicesList.storeID).Where(x => x.relationshipCustomerId == eachConsultingPatnerInvoices.Key).Select(x => x.relationshipBankAccountNo).FirstOrDefault() ??  string.Empty;
								ListOfInvoicesOfthePartner.BankAccountNumber = BankAccountNumber is null ? "" : BankAccountNumber;
								string BankAccountBsb = storeCPBankRelationship?.value?.Where(x => x.customerId == eachStoreInvoicesList.storeID).Where(x => x.relationshipCustomerId == eachConsultingPatnerInvoices.Key).Select(x => x.relationshipBankBranchNo).FirstOrDefault() ?? string.Empty;
								ListOfInvoicesOfthePartner.BankAccountBsb = BankAccountBsb is null ? "" : BankAccountBsb;
								// Calculate some Totale of all the invoices for the Consulting Partner
									foreach (var eachInvoice in eachConsultingPatnerInvoices)
									{
									
									    CPInvoiceResponse.typeBill ListOfInvoiceLines = new CPInvoiceResponse.typeBill();
										ListOfInvoiceLines.Number = eachInvoice.BCCPInvoices.no;
										ListOfInvoiceLines.Type = "Invoice";
								        ListOfInvoiceLines.DocumentDate = eachInvoice.BCCPInvoices.documentDate; 
								        ListOfInvoiceLines.DueDate = eachInvoice.BCCPInvoices.dueDate;
									// round Paid amount to two deciamls

									ListOfInvoiceLines.Paid = Convert.ToDecimal(eachInvoice.BCCPInvoices.amountIncludingVAT - eachInvoice.BCCPInvoices.remainingAmount).ToString();
									decimal paidAmount = Convert.ToDecimal(ListOfInvoiceLines.Paid);
                    				decimal? invoiceAmount = eachInvoice?.BCCPInvoices?.salesInvoiceLines?.FirstOrDefault()?.amount;
									decimal dueAmount = invoiceAmount != null && invoiceAmount - paidAmount > 0 	? invoiceAmount.Value - paidAmount : 0;
								    unpaidAmount =  unpaidAmount + dueAmount;
									ListOfInvoiceLines.Due = dueAmount.ToString();
									if (eachInvoice!.BCCPInvoices.cancelled)
									{
										ListOfInvoiceLines.Status = "Cancelled" ;
										ListOfInvoiceLines.PaidDate = eachInvoice.BCCPInvoices.closedAtDate;
									}
									else if(eachInvoice.BCCPInvoices.closedAtDate != "0001-01-01" )
									{
										ListOfInvoiceLines.Status = "Closed";
										ListOfInvoiceLines.PaidDate = eachInvoice.BCCPInvoices.closedAtDate;
									}
									else
									{
										ListOfInvoiceLines.Status = "Open";
										ListOfInvoiceLines.PaidDate = "";
									}
									ListOfInvoicesOfthePartner.Bills.Add(ListOfInvoiceLines);
									}
								
								ListOfInvoicesOfthePartner.Total = Convert.ToDecimal(unpaidAmount);
								partnerInvoices.Add(eachConsultingPatnerInvoices.Key!, ListOfInvoicesOfthePartner);
							}
							if (eachStoreInvoicesList.storeID is not null)
							{
								storeInvoices.Add(eachStoreInvoicesList.storeID, partnerInvoices);
							}
						}
						return new JsonResult(storeInvoices);
					
				}
				else
				{
					return new JsonResult($"No matching customers found. Please verify the accuracy of the customer IDs sent in the request.") { StatusCode = StatusCodes.Status404NotFound };
				}
			}
			catch (Exception e)
			{
				return new JsonResult(e.Message);
			}
		}
}
}
