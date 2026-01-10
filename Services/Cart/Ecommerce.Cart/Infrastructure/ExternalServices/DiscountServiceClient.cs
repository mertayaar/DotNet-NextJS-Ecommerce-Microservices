using Ecommerce.Cart.Infrastructure.Dtos;
using Ecommerce.Cart.Infrastructure.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce.Cart.Infrastructure.ExternalServices
{
    
    
    
    
    
    public class DiscountServiceClient : IDiscountServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DiscountServiceClient> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DiscountServiceClient(
            HttpClient httpClient, 
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DiscountServiceClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return null;
            }

            var discountServiceUrl = _configuration["DiscountServiceUrl"] ?? "http://localhost:7221";
            
            try
            {
                
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Replace("Bearer ", "");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(
                    $"{discountServiceUrl}/api/discounts/GetCodeDetailByCode?code={couponCode}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Discount service returned {StatusCode} for coupon {CouponCode}", 
                        response.StatusCode, couponCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<DiscountApiResponse>(content, _jsonOptions);

                if (apiResponse?.Data == null)
                {
                    return null;
                }

                
                return new CouponDto
                {
                    CouponId = apiResponse.Data.CouponId,
                    CouponCode = apiResponse.Data.CouponCode,
                    CouponRate = apiResponse.Data.CouponRate,
                    IsActive = apiResponse.Data.IsActive,
                    ValidDate = apiResponse.Data.ValidDate
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to communicate with Discount service for coupon {CouponCode}", couponCode);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse Discount service response for coupon {CouponCode}", couponCode);
                return null;
            }
        }

        
        
        private class DiscountApiResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public DiscountData? Data { get; set; }
        }

        private class DiscountData
        {
            public int CouponId { get; set; }
            public string CouponCode { get; set; } = string.Empty;
            public int CouponRate { get; set; }
            public bool IsActive { get; set; }
            public DateTime ValidDate { get; set; }
        }
    }
}
