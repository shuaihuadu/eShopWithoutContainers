﻿namespace eShopWithoutContainers.Services.Ordering.API.Application.Commands;
public class ShipOrderCommand : IRequest<bool>
{
    [DataMember]
    public int OrderNumber { get; set; }

    public ShipOrderCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
