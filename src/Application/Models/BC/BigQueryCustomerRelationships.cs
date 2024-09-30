using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.BC
{
	public class BigQueryCustomerRelationships
	{

		
			public string? odatacontext { get; set; }
			public List<CustomerRelationships>? value { get; set; } 


		public class CustomerRelationships
		{
			public string? odataetag { get; set; }
			public  string? id { get; set; }
			public string? customerNo { get; set; }
			public string? customerId { get; set; }
			public string? customerName { get; set; }
			public string? relationshipCustomerNo { get; set; }
			public string? relationshipCustomerId { get; set; }
			public string? relationshipCustomerName { get; set; }
			public string? relationshipCustomerPostingGroup { get; set; }
			public string? relationshipBankCode { get; set; }
			public string? relationshipBankName { get; set; }
			public string? relationshipBankAccountNo { get; set; }
			public string? relationshipBankBranchNo { get; set; }
		}


	}
}
