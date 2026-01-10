using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Application.Features.Mediator.Results.OrderingResults
{
    public class GetOrderingByUserIdQueryResult
    {
        public int OrderingId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string? DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
    }
}
