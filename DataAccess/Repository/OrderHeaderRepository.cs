using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataModel.Models;

namespace DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }
        public void Update(OrderHeader entity)
        {
            db.OrderHeaders.Update(entity);
        }
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(i => i.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
        public void UpdateStripePaymentStatus(int id, string sessionId, string paymentId)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(i => i.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentId))
                {
                    orderFromDb.PaymentIntentId = paymentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
