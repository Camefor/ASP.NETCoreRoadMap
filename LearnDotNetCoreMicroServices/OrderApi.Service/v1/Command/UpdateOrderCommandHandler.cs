﻿using MediatR;
using OrderApi.Data.Repository.v1;
using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderApi.Service.v1.Command {
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand> {
        private readonly IRepository<Order> _repository;
        public UpdateOrderCommandHandler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken) {
            await _repository.UpdateRangeAsync(request.Orders);
            return Unit.Value;
        }
    }
}
