﻿using EventBus.Abstractions;
using System.Threading.Tasks;

namespace EventBus.Tests
{
    public class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; set; }
        public TestIntegrationEventHandler()
        {
            Handled = false;
        }
        public async Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;
        }
    }
}
