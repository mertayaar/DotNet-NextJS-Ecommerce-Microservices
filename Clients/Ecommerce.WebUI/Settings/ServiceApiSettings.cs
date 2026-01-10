namespace Ecommerce.WebUI.Settings
{
    public class ServiceApiSettings
    {
        public string OcelotUrl { get; set; }
        public string IdentityServerUrl { get; set; }
        public ServiceApi Catalog { get; set; }
        public ServiceApi Image { get; set; }
        public ServiceApi Discount { get; set; }
        public ServiceApi Order { get; set; }
        public ServiceApi Cart { get; set; }
        public ServiceApi Cargo { get; set; }
        public ServiceApi Review { get; set; }
        public ServiceApi Payment { get; set; }
        public ServiceApi Message { get; set; }
        public ServiceApi User { get; set; }
    }

    public class ServiceApi
    {
        public string Path { get; set; }
    }
}
