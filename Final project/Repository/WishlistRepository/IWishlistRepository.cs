using Final_project.Models;

namespace Final_project.Repository.WishlistRepository
{
    public interface IWishlistRepository : IRepository<wishlist>
    {
        wishlist GetWishlistByUserId(string userId);
        void Save();
    }
}
