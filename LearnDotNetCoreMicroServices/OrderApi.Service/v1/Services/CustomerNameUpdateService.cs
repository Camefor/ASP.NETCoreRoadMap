using MediatR;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Models;
using OrderApi.Service.v1.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderApi.Service.v1.Services {

    public interface ICustomerNameUpdateService {
        void UpdateCustomerNameInOrders(UpdateCustomerFullNameModel updateCustomerFullNameModel);
    }


    public class CustomerNameUpdateService : ICustomerNameUpdateService {

        private readonly IMediator _mediator;

        public CustomerNameUpdateService(IMediator mediator) {
            _mediator = mediator;
        }
        public async void UpdateCustomerNameInOrders(UpdateCustomerFullNameModel updateCustomerFullNameModel) {
            try {
                var orderOfCustomer = await _mediator.Send(new GetOrderByCustomerGuidQuery {
                    CustomerCuid = updateCustomerFullNameModel.Id
                });

                if (orderOfCustomer.Count !=0) {
                    orderOfCustomer.ForEach(x => x.CustomerFullName = $"{updateCustomerFullNameModel.FirstName} {updateCustomerFullNameModel.LastName}");
                }

                await _mediator.Send(new UpdateOrderCommand {
                    Orders = orderOfCustomer
                });
            } catch (Exception ex) {
                //error
            }
        }
    }
}
