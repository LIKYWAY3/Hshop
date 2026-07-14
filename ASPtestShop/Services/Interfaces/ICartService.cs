using ASPtestShop.Models.DTO.Cart;

namespace ASPtestShop.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResultDto> AddToCartAsync(string userId, AddToCartDto dto);

        Task<CartResultDto> GetCartAsync(string userId);

        Task<CartResultDto> UpdateCartItemAsync(string userId, UpdateCartItemDto dto);

        Task<CartResultDto> RemoveCartItemAsync(string userId, int cartItemId);

        Task<CartResultDto> ClearCartAsync(string userId);

        Task<bool> HasCartItemsAsync(string userId);
    }
}