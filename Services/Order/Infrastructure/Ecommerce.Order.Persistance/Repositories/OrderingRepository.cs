using Ecommerce.Order.Application.Interfaces;
using Ecommerce.Order.Domain.Entitites;
using Ecommerce.Order.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Persistance.Repositories
{
    public class OrderingRepository : IOrderingRepository
    {
        private readonly OrderContext _context;

        public OrderingRepository(OrderContext context)
        {
            _context = context;
        }

        public List<Ordering> GetOrderingsByUserId(string id)
        {
           var values = _context.Orderings.Where(x => x.UserId == id).ToList();
            return values;
        }
    }
}
