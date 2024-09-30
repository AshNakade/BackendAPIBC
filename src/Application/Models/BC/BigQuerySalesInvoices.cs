using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.BC
{
	public class BigQuerySalesInvoices
	{
		
			public string? odatacontext { get; set; }
			public List<SalesHeader>? value { get; set; }

		public class SalesHeader
		{
			public string? odataetag { get; set; }
			public required string id { get; set; }
			public string? no { get; set; }
			public string? billToCustomerNo { get; set; }
			public string? billToCustomerName { get; set; }
			public string? billToCustomerId { get; set; }
			public string? customerPostingGroup { get; set; }
			public string? shortcutDimension4Code { get; set; }
			public string? documentDate { get; set; }
			public string? dueDate { get; set; }
			public decimal amount { get; set; }
			public decimal amountIncludingVAT { get; set; }
			public decimal remainingAmount { get; set; }
			public bool cancelled { get; set; }
			public string? closedAtDate { get; set; }
			public List<Salesinvoiceline>? salesInvoiceLines { get; set; }
		}

		public class Salesinvoiceline
		{
			public string? odataetag { get; set; }
			public required string id { get; set; }
			public string? documentNo { get; set; }
			public long lineNo { get; set; }
			public string? genProdPostingGroup { get; set; }
			public decimal amount { get; set; }
			public decimal amountIncludingVAT { get; set; }
		}


	}
}
