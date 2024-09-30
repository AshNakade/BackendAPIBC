using Microsoft.AspNetCore.Mvc;
using Application.Models.APIRequestResponse.CPInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CPInvoices
{
	public  interface ICPInvoicesService
	{
		Task<JsonResult> GetCPInvoices(string companyId, CPInvoiceRequest CPInvoiceRequest);

	}
}
