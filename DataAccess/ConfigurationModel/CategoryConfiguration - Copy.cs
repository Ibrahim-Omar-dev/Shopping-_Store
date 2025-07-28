using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.ConfigurationModel
{
    public class CompaniesConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category { DisplayOrder = 1, Id = 1, Name = "Action" },
                new Category { DisplayOrder = 2, Id = 2, Name = "Adventure" },
                new Category { DisplayOrder = 3, Id = 3, Name = "Romance" }
            );
        }
    }
}
