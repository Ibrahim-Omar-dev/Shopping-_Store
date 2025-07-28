using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataModel.Models;

namespace DataAccess.Repository
{
    class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext db;

        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OrderDetails entity)
        {
            db.OrderDetails.Update(entity);
        }
    }
}
