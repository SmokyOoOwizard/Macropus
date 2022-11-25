using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Macropus.Database.Db.Models
{
	internal class DatabaseConfig : IEntityTypeConfiguration<DatabaseDbModel>
	{
		public void Configure(EntityTypeBuilder<DatabaseDbModel> builder)
		{
			builder.HasKey(file => file.Id);
			builder.Property(file => file.Id).HasConversion(new GuidToStringConverter());
			builder.Property(file => file.Name).IsRequired();
			builder.Property(file => file.FileId).HasConversion(new GuidToStringConverter()).IsRequired();
		}
	}
}