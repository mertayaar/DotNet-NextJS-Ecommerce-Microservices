using Ecommerce.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Ecommerce.Order.Application.Interfaces;
using Ecommerce.Order.Domain.Entitites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class CreateOrderingCommandHandler : IRequestHandler<CreateOrderingCommand>
    {
        private readonly IRepository<Ordering> _orderingRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public CreateOrderingCommandHandler(
            IRepository<Ordering> orderingRepository,
            IRepository<OrderDetail> orderDetailRepository)
        {
            _orderingRepository = orderingRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task Handle(CreateOrderingCommand request, CancellationToken cancellationToken)
        {
            
            var ordering = new Ordering
            {
                OrderDate = request.OrderDate,
                TotalPrice = request.TotalPrice,
                UserId = request.UserId,
                DiscountCode = request.DiscountCode,
                DiscountRate = request.DiscountRate
            };

            await _orderingRepository.CreateAsync(ordering);

            
            if (request.OrderItems != null && request.OrderItems.Count > 0)
            {
                foreach (var item in request.OrderItems)
                {
                    await _orderDetailRepository.CreateAsync(new OrderDetail
                    {
                        OrderingId = ordering.OrderingId,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductPrice = item.ProductPrice,
                        ProductAmount = item.ProductAmount,
                        ProductTotalPrice = item.ProductTotalPrice,
                        ProductImageUrl = item.ProductImageUrl
                    });
                }
            }
        }
    }
}
