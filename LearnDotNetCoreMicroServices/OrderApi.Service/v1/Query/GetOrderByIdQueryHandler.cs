using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Repository.v1;
using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderApi.Service.v1.Query {
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order> {

        private readonly IRepository<Order> _repository;
        public GetOrderByIdQueryHandler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken) {
            return await _repository.GetAll().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        }
    }
}
