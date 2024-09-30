using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.BC
{
	public class BigQueryCustomers
	{


			public string? odatacontext { get; set; }
			public Customers[]? value { get; set; }
	
		public class Customers
		{
			public string? odataetag { get; set; }
			public  string? id { get; set; }
			public string? no { get; set; }
			public int buffer { get; set; }
		}

	}
}
