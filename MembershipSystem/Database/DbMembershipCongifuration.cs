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
        }
    }

    internal class DbMemberCongifuration : IEntityTypeConfiguration<DbMember>
    {
        public void Configure(EntityTypeBuilder<DbMember> builder)
        {
            builder.ToTable("Members");
        }
    }
}
