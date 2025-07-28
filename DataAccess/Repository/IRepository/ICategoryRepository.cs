using DataModel.Models;

namespace DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public void Update(Category entity);
    }
}
