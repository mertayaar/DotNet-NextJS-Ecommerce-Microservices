using Ecommerce.Order.Application.Features.CQRS.Queries.OrderDetailQueries;
using Ecommerce.Order.Application.Features.CQRS.Results.OrderDetailResults;
using Ecommerce.Order.Application.Interfaces;
using Ecommerce.Order.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Application.Features.CQRS.Handlers.OrderDetailHandlers
{
    public class GetOrderDetailByOrderingIdQueryHandler
    {
        private readonly IRepository<OrderDetail> _repository;

        public GetOrderDetailByOrderingIdQueryHandler(IRepository<OrderDetail> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetOrderDetailByOrderingIdQueryResult>> Handle(GetOrderDetailByOrderingIdQuery query)
        {
            var values = await _repository.GetListByFilterAsync(x => x.OrderingId == query.Id);
            return values.Select(x => new GetOrderDetailByOrderingIdQueryResult
            {
                OrderDetailId = x.OrderDetailId,
                ProductAmount = x.ProductAmount,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                OrderingId = x.OrderingId,
                ProductPrice = x.ProductPrice,
                ProductTotalPrice = x.ProductTotalPrice,
                ProductImageUrl = x.ProductImageUrl
            }).ToList();
        }
    }
}
