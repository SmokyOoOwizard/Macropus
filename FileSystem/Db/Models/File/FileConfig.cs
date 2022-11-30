using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Macropus.FileSystem.Db.Models.File;

public class FileConfig : IEntityTypeConfiguration<FileDbModel>
{
    public void Configure(EntityTypeBuilder<FileDbModel> builder)
    {
        builder.HasKey(file => file.Id);
        builder.Property(file => file.Id).HasConversion(new GuidToStringConverter());
        builder.Property(file => file.Name).IsRequired();
        builder.Property(file => file.ObjectName).IsRequired();
    }
}