using Ecommerce.Order.Application.Features.CQRS.Queries.AddressQueries;
using Ecommerce.Order.Application.Features.CQRS.Results.AddressResults;
using Ecommerce.Order.Application.Interfaces;
using Ecommerce.Order.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Application.Features.CQRS.Handlers.AddressHandlers
{
    public class GetAddressByIdQueryHandler
    {
        private readonly IRepository<Address> _repository;

        public GetAddressByIdQueryHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }
        public async Task<GetAddressByIdQueryResult> Handle(GetAddressByIdQuery query)
        {
            var values = await _repository.GetByIdAsync(query.Id);
            return new GetAddressByIdQueryResult
            {
                AddressId = values.AddressId,
                City = values.City,
                Detail = values.AddressLine1,
                District = values.District,
                UserId = values.UserId
            };
        }
    }
}
