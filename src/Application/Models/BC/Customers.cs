namespace Application.Models.BC
{

    public class BCCustomers
    {
        public string? odatacontext { get; set; }
        public List<Customers>? value { get; set; } = new List<Customers>();
    }

    public class Customers
    {
        public string? odataetag { get; set; }
        public required string id { get; set; }
        public string? number { get; set; }
        public string? displayName { get; set; }
        public string? type { get; set; }
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? country { get; set; }
        public string? postalCode { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? website { get; set; }
        public string? salespersonCode { get; set; }
        public float? balanceDue { get; set; }
        public int? creditLimit { get; set; }
        public bool taxLiable { get; set; }
        public string? taxAreaId { get; set; }
        public string? taxAreaDisplayName { get; set; }
        public string? taxRegistrationNumber { get; set; }
        public string? currencyId { get; set; }
        public string? currencyCode { get; set; }
        public string? paymentTermsId { get; set; }
        public string? shipmentMethodId { get; set; }
        public string? paymentMethodId { get; set; }
        public string? blocked { get; set; }
        public DateTime? lastModifiedDateTime { get; set; }
    }

}
