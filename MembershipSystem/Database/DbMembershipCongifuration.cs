using MembershipSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MembershipSystem.Database
{
    internal class DbDataCardCongifuration : IEntityTypeConfiguration<DbDataCard>
    {
        public void Configure(EntityTypeBuilder<DbDataCard> builder)
        {
            builder.ToTable("DataCards");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Member)
                .WithOne()
                .HasForeignKey<DbMember>(x => x.Id);
        }
    }

    internal class DbMemberCongifuration : IEntityTypeConfiguration<DbMember>
    {
        public void Configure(EntityTypeBuilder<DbMember> builder)
        {
            builder.ToTable("Members");
            builder.HasKey(x => x.Id);
        }
    }

    internal class DbCompanyCongifuration : IEntityTypeConfiguration<DbCompany>
    {
        public void Configure(EntityTypeBuilder<DbCompany> builder)
        {
            builder.ToTable("Companies");
            builder.HasKey(x => x.Id);
        }
    }
}
