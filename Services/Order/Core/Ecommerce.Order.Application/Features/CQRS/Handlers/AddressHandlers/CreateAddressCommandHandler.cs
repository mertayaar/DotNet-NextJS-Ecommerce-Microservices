using Ecommerce.Order.Application.Features.CQRS.Commands.AddressCommands;
using Ecommerce.Order.Application.Interfaces;
using Ecommerce.Order.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Application.Features.CQRS.Handlers.AddressHandlers
{
    public class CreateAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;

        public CreateAddressCommandHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }
        public async Task Handle(CreateAddressCommand createAddressCommand)
        {
            await _repository.CreateAsync(new Address
            {
                Name = createAddressCommand.Name,
                Surname = createAddressCommand.Surname,
                Phone = createAddressCommand.Phone,
                Email = createAddressCommand.Email,
                AddressLine1 = createAddressCommand.AddressLine1,
                AddressLine2 = createAddressCommand.AddressLine2,
                City = createAddressCommand.City,
                District = createAddressCommand.District,
                State = createAddressCommand.State,
                ZipCode = createAddressCommand.ZipCode,
                Country = createAddressCommand.Country,
                UserId = createAddressCommand.UserId
            });
        }

    }
}
