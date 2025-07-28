using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataModel.Models;

namespace DataAccess.Repository
{
    class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public ApplicationDbContext ApplicationDbContext { get; }
        public CategoryRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }

        public void Update(Category entity)
        {
            ApplicationDbContext.categories.Update(entity);
        }
    }
}
