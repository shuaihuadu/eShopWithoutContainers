﻿global using Microsoft.Extensions.Logging;
global using Polly;
global using Polly.Retry;
global using RabbitMQ.Client;
global using RabbitMQ.Client.Exceptions;
global using System;
global using System.IO;
global using System.Net.Sockets;
global using EventBus.Abstractions;
global using EventBus.Events;
global using EventBus;
global using Autofac;
global using RabbitMQ.Client.Events;
global using System.Threading.Tasks;
global using System.Text;
global using System.Text.Json;