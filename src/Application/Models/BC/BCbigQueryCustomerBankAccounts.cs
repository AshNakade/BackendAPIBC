using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.BC
{
    public class BCbigQueryCustomerBankAccounts
	{

            public string? odatacontext { get; set; }
            public CustomerRecords[]? value { get; set; }


        public class CustomerRecords
        {
            public string? odataetag { get; set; }
            public required string id { get; set; }
            public string? customerNo { get; set; }
            public string? customerName { get; set; }
			public string? customerPostingGroup { get; set; }
			public string?   bankAccountName { get; set; }
            public string? bankBranchNo { get; set; }
            public string? bankAccountNo { get; set; }
        }


    }
}
