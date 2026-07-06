using ASPtestShop.Services.PaymentProviders;

namespace ASPtestShop.Services.Interfaces
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetProvider(string paymentMethod);
    }
}