using MediatR;
using OrderApi.Data.Repository.v1;
using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderApi.Service.v1.Command {
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order> {
        private readonly IRepository<Order> _repository;

        public CreateOrderCommandHandler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken) {
            return await _repository.AddAsync(request.Order);
        }
    }
}
