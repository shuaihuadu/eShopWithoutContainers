﻿namespace eShopWithoutContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;

public class Buyer : Entity, IAggregateRoot
{
    public string IdentityGuid { get; private set; }
    public string Name { get; private set; }
    public List<PaymentMethod> _paymentMethods;
    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    protected Buyer()
    {
        _paymentMethods = new List<PaymentMethod>();
    }

    public Buyer(string identity, string name) : this()
    {
        IdentityGuid = !string.IsNullOrEmpty(identity) ? identity : throw new ArgumentNullException(nameof(identity));
        Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
    }

    public PaymentMethod VerifyOrAddPaymentMethod(int cardTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName, DateTime expiration, int orderId)
    {
        var existsPayment = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

        if (existsPayment != null)
        {
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existsPayment, orderId));
            return existsPayment;
        }

        var payment = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
        _paymentMethods.Add(payment);

        AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

        return payment;
    }
}
