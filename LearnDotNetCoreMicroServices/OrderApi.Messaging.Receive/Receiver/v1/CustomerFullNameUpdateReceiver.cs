﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderApi.Messaging.Receive.Options.v1;
using OrderApi.Service.v1.Models;
using OrderApi.Service.v1.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderApi.Messaging.Receive.Receiver.v1 {
    public class CustomerFullNameUpdateReceiver : BackgroundService {
        private IModel _channel;

        private IConnection _connection;

        private readonly ICustomerNameUpdateService _customerNameUpdateService;

        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public CustomerFullNameUpdateReceiver(ICustomerNameUpdateService customerNameUpdateService, IOptions<RabbitMqConfiguration> rabbitMqOptions) {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _customerNameUpdateService = customerNameUpdateService;

        }


        private void InitializeRabbitMqListener() {
            var factory = new ConnectionFactory {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };
            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += _connection_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e) {
            throw new NotImplementedException();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body);
                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<UpdateCustomerFullNameModel>(content);
                HandleMessage(updateCustomerFullNameModel);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(_queueName, false, consumer);
            return Task.CompletedTask;
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) {

        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) {

        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) {

        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) {

        }

        private void HandleMessage(UpdateCustomerFullNameModel updateCustomerFullNameModel) {
            _customerNameUpdateService.UpdateCustomerNameInOrders(updateCustomerFullNameModel);
        }

        public override void Dispose() {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }

}
