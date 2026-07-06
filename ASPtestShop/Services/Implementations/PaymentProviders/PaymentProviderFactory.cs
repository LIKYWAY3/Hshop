using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.PaymentProviders;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    // Factory dùng để chọn provider theo PaymentMethod
    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IEnumerable<IPaymentProvider> _providers;

        public PaymentProviderFactory(IEnumerable<IPaymentProvider> providers)
        {
            _providers = providers;
        }

        public IPaymentProvider GetProvider(string paymentMethod)
        {
            var provider = _providers.FirstOrDefault(p =>
                p.PaymentMethod.Equals(paymentMethod, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                throw new NotSupportedException($"Phương thức thanh toán '{paymentMethod}' chưa được hỗ trợ");
            }

            return provider;
        }
    }
}