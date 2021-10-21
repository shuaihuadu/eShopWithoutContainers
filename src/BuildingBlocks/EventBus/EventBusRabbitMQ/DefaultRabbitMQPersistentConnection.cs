﻿using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace EventBusRabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        IConnection _connection;
        bool _disposed;

        object sync_root = new object();

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _retryCount = retryCount;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect.");

            lock (sync_root)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, "RabbitMQ client could not connect after {TimeOut}s ({ExcpetionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    });

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ client acquired a persistent connection to '{HostName}' and is subscribed to failure events.", _connection.Endpoint.HostName);
                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened.");
                    return false;
                }
            }
        }

        private void OnConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            _logger.LogWarning("A RabbitMQ connection is shudown. Trying to re-connect...");
            TryConnect();
        }

        private void OnCallbackException(object sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            _logger.LogWarning("A RabbitMQ connection throw excpetion. Trying to re-connect...");
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }
    }
}
