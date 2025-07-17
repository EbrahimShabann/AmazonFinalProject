namespace Final_project.Repository.DiscountRepositoryFile
{
    public interface IDiscountRepo:IRepository<Discount>
    {
        public void delete(Discount entity);
    }
}
