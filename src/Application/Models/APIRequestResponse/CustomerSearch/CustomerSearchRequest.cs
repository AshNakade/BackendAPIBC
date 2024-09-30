using System.Collections.Generic;

namespace Application.Models.APIRequestResponse.CustomerSearch

{

	public class CustomerSearchRequest
	{
		public required string searchString { get; set; }
		public int pageNumber { get; set; } = 0;


	}


	public class CustomerSearchResponse
   {
		public int pageNumber { get; set; }
		public List<CustomerResultset>? resultSet { get; set; } = new List<CustomerResultset>();
	}

	public class CustomerResultset
	{
		public string? customerName { get; set; } = "";
		public string? customerId { get; set; } = "";

	}
	
	public class GetCustomerResponse
    {
		public string? customerName { get; set; } = "";
		public string? customerId { get; set; } = "";

	}

	public class ConsultingPartnerSearchRequest
	{
		public required string searchString { get; set; }
		public int pageNumber { get; set; } = 0;
		public required string storeId { get; set; }


	}


	public class GetCPPartnerSearchRequest
	{
		public required string storeId { get; set; }


	}


	public class ConsultingPartnerSearchResponse
	{
		public int pageNumber { get; set; }
		public List<CPResultset>? resultSet { get; set; } = new List<CPResultset>();
	}

	public class CPResultset
	{
		public string? consultingPartnerName { get; set; } = "";
		public string? consultingPartnerId { get; set; } = "";
		public string? AccountName { get; set; } = "";
		public string? BSB { get; set; } = "";
		public string? AccountNumber { get; set; } = "";

	}

	public class  GetConsultingPartnerResponse
    {
	public string? consultingPartnerName { get; set; } = "";
	public string? consultingPartnerId { get; set; } = "";
		public string? AccountName { get; set; } = "";
	public string? BSB { get; set; } = "";
	public string? AccountNumber { get; set; } = "";
	}



}
