using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Models.APIRequestResponse.StoreBalanceSummary
{

	public class StoreBalanceSummaryRequest
	{
		public required List<string>? CustomerIDs { get; set; }
	}


	public class StoreBalanceSummaryRequestResponse
	{
		public string? CustomerId { get; set; }
		public decimal Buffer { get; set; }
		public decimal	 OutstandingBmsBalanceDue { get; set; }
	}

	public class StoreBalanceSummary {
		public decimal OutstandingBmsBalanceDue { get; set; }
	}
}
