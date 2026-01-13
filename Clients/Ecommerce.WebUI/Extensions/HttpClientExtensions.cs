using Ecommerce.WebUI.Handlers;
using Ecommerce.WebUI.Services.Concrete;
using Ecommerce.WebUI.Services.CargoServices.CargoCompanyServices;
using Ecommerce.WebUI.Services.CargoServices.CargoCustomerServices;
using Ecommerce.WebUI.Services.CartServices;
using Ecommerce.WebUI.Services.CatalogServices.AboutServices;
using Ecommerce.WebUI.Services.CatalogServices.CategoryServices;
using Ecommerce.WebUI.Services.CatalogServices.ContactServices;
using Ecommerce.WebUI.Services.CatalogServices.FeatureServices;
using Ecommerce.WebUI.Services.CatalogServices.OfferDiscountServices;
using Ecommerce.WebUI.Services.CatalogServices.ProductDetailServices;
using Ecommerce.WebUI.Services.CatalogServices.ProductImageSevices;
using Ecommerce.WebUI.Services.CatalogServices.ProductServices;
using Ecommerce.WebUI.Services.CatalogServices.SliderServices;
using Ecommerce.WebUI.Services.CatalogServices.SpecialOfferServices;
using Ecommerce.WebUI.Services.CatalogServices.SponsorBrandServices;
using Ecommerce.WebUI.Services.DiscountServices;
using Ecommerce.WebUI.Services.Interfaces;
using Ecommerce.WebUI.Services.MessageServices;
using Ecommerce.WebUI.Services.OrderServices.OrderAddressServices;
using Ecommerce.WebUI.Services.OrderServices.OrderOrderingServices;
using Ecommerce.WebUI.Services.ReviewServices;
using Ecommerce.WebUI.Services.StatisticServices.CatalogStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.DiscountStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.MessageStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.ReviewStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.UserStatisticServices;
using Ecommerce.WebUI.Services.UserIdentityServices;
using Ecommerce.WebUI.Settings;

namespace Ecommerce.WebUI.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            var values = configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();

            services.AddHttpClient<IClientCredentialTokenService, ClientCredentialTokenService>();

            services.AddHttpClient<IUserService, UserService>(opt =>
            {
                opt.BaseAddress = new Uri(values.IdentityServerUrl);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IUserStatisticService, UserStatisticService>(opt =>
            {
                opt.BaseAddress = new Uri(values.IdentityServerUrl);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IReviewStatisticService, ReviewStatisticService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Review.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<ICartService, CartService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Cart.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<ICargoCompanyService, CargoCompanyService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Cargo.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IReviewService, ReviewService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Review.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<ICatalogStatisticService, CatalogStatisticService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<ICargoCustomerService, CargoCustomerService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Cargo.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IUserIdentityService, UserIdentityService>(opt =>
            {
                opt.BaseAddress = new Uri(values.IdentityServerUrl);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IDiscountService, DiscountService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Discount.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IDiscountStatisticService, DiscountStatisticService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Discount.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IMessageService, MessageService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Message.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IMessageStatisticService, MessageStatisticService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Message.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IOrderOrderingService, OrderOrderingService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Order.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IOrderAddressService, OrderAddressService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Order.Path}/");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<ICategoryService, CategoryService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IProductService, ProductService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<ISpecialOfferService, SpecialOfferService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IFeatureSliderService, FeatureSliderService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IFeatureService, FeatureService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IOfferDiscountService, OfferDiscountService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<ISponsorBrandService, SponsorBrandService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IAboutService, AboutService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IProductImageService, ProductImageService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IProductDetailService, ProductDetailService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<IContactService, ContactService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/{values.Catalog.Path}/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            services.AddHttpClient<Ecommerce.WebUI.Services.ImageUpload.IImageUploadService, Ecommerce.WebUI.Services.ImageUpload.GcsImageUploadService>(opt =>
            {
                opt.BaseAddress = new Uri($"{values.OcelotUrl}/services/images/");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();
        }
    }
}
