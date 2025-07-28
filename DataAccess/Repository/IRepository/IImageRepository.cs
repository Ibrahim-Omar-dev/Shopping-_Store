using DataModel.Models;

namespace DataAccess.Repository.IRepository
{
    public interface IImageRepository : IRepository<ImageProduct>
    {
        public void Update(ImageProduct entity);
    }
}
