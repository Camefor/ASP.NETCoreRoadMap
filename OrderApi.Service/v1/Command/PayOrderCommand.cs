using MediatR;
using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderApi.Service.v1.Command {
    public class PayOrderCommand :IRequest<Order>{
        public Order Order { get; set; }
    }
}
