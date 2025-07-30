using Final_project.Models;

namespace Final_project.Repository.WishlistRepositoryFile
{
    public interface IWishlistItemRepository : IRepository<wishlist_item>
    {
        List<wishlist_item> GetItemsByWishlistId(string wishlistId);
        wishlist_item GetByProductId(string wishlistId, string productId);
        void Remove(wishlist_item item);
        void Save();
    }
}
