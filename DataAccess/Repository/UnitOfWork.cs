using DataAccess.Data;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public ICompanyRepository Company { get; }
        public IShoppingCartRepository ShoppingCart { get; }
        public IOrderDetailsRepository OrderDetails { get; }
        public IOrderHeaderRepository OrderHeader { get; }
        public IImageRepository Image { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Product = new ProductRepository(_context);
            Company = new CompanyRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderDetails = new OrderDetailsRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
            Image = new ImageRepository(_context);

        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}
