using ASPtestShop.Models.DTO.Order;

namespace ASPtestShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CheckoutResultDto> CheckoutAsync(string userId, CheckoutDto checkoutDto);

        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string userId);

        Task<OrderHistoryDto?> GetOrderDetailAsync(string userId, int orderId);
    }
}