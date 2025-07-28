using DataModel.Models;

namespace DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        public void Update(Company entity);
    }
}
