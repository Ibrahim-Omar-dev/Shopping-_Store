using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataModel.Models;

namespace DataAccess.Repository
{
    class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }



        public void Update(ApplicationUser obj)
        {
            _db.applicationUsers.Update(obj);
        }
    }
}
