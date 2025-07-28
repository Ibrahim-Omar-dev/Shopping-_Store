using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataModel.Models;

namespace DataAccess.Repository
{
    class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Product entity)
        {
            var product = db.products.FirstOrDefault(i => i.Id == entity.Id);
            if (product != null)
            {
                product.Title = entity.Title;
                product.Description = entity.Description;
                product.ISBN = entity.ISBN;
                product.Author = entity.Author;
                product.ListPrice = entity.ListPrice;
                product.Price = entity.Price;
                product.Price50 = entity.Price50;
                product.Price100 = entity.Price100;
                product.CategoryId = entity.CategoryId;
                product.ImageProduct = entity.ImageProduct;

                //if (entity.ImageUrl != null)
                //{
                //    product.ImageUrl = entity.ImageUrl;
                //}
            }
        }
    }
}
