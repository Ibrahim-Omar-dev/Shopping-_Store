namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public ICompanyRepository Company { get; }
        public IShoppingCartRepository ShoppingCart { get; }
        public IOrderHeaderRepository OrderHeader { get; }
        public IOrderDetailsRepository OrderDetails { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public IImageRepository Image { get; }
        public void Complete();
    }
}
