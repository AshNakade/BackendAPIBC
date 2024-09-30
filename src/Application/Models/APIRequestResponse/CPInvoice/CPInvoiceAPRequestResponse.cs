using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.APIRequestResponse.CPInvoice
{
	public class CPInvoiceRequest
	{
		public required List<string>? CustomerIDs { get; set; }
	}

	public class CPInvoiceResponse
	{

		
		
		public string? CustomerId { get; set; }
		public List<typePartnerInvoices> PartnerInvoices { get; set; } = new List<typePartnerInvoices>();


		public class typePartnerInvoices
		{
			public string? Name { get; set; } = "";
			public string? ConsultingPartnerid { get; set; } = "";
			public decimal Total { get; set; }
			public string? BankAccountName { get; set; } = "";
			public string? BankAccountNumber { get; set; } = "";
			public string? BankAccountBsb { get; set; } = "";
			public List<typeBill> Bills { get; set; } = new List<typeBill>();
		}

		public class internalPartnerInvoices
		{
			public string? Name { get; set; } = "";
			public string? BankAccountName { get; set; } = "";
			public string? BankAccountNumber { get; set; } = "";
			public string? BankAccountBsb { get; set; } = "";
			public decimal? Total { get; set; }
			public List<typeBill> Bills { get; set; } = new List<typeBill>();
		}


		public class typeBill
		{
			public string? Number { get; set; } = "";
			public string? Type { get; set; } = "";
			public string? DocumentDate { get; set; } = "";
			public string? DueDate { get; set; } = "";
			public string? Paid { get; set; }
			public string? Due { get; set; }
			public string? Status { get; set; } = "";
			public string? PaidDate { get; set; } = "";
			
		}




	}

	//public class CPInvoiceResponseold
	//{
	//	public string? CustomerID { get; set; }
	//	public List<typeBCConsultingPartners>? BCConsultingPartners { get; set; } = new List<typeBCConsultingPartners>();
	//	public class typeBCConsultingPartners
	//	{

	//		public string? ConsultingPartnerid { get; set; }
	//		public string? Name { get; set; }
	//		public string? BankAccountName { get; set; }
	//		public string? BankAccountNumber { get; set; }
	//		public string? BankAccountBsb { get; set; }
	//		public string? Total { get; set; }
	//		public List<Bill>? Bills { get; set; } = new List<Bill>();
	//	}

	//	public class Bill
	//	{
	//		public required string number { get; set; }
	//		public string? type { get; set; }
	//		public string? documentDate { get; set; }
	//		public string? dueDate { get; set; }
	//		public string? paid { get; set; }
	//		public string? due { get; set; }
	//		public string? status { get; set; }
	//		public string? paidDate { get; set; }
	//	}




	//}



}
