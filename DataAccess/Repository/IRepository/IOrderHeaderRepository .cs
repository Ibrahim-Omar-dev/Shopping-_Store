
using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public void  Update(OrderHeader entity);
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        public void UpdateStripePaymentStatus(int id,string sessionId, string paymentId);

    }
}
