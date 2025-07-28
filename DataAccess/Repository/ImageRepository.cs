using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataModel.Models;

namespace DataAccess.Repository
{
    class ImageRepository : Repository<ImageProduct>, IImageRepository
    {
        public ApplicationDbContext ApplicationDbContext { get; }
        public ImageRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }

        public void Update(ImageProduct entity)
        {
            ApplicationDbContext.imageProducts.Update(entity);
        }
    }
}
