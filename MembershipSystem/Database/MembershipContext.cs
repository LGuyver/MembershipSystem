﻿using MembershipSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace MembershipSystem.Database
{
    public class MembershipContext : DbContext
    {
        public virtual DbSet<DbDataCard> DataCards { get; set; }
        public virtual DbSet<DbMember> Members { get; set; }
        public virtual DbSet<DbCompany> Companies { get; set; }

        public MembershipContext(DbContextOptions<MembershipContext> options) : base(options)
        {
            _ = (SqlConnection)Database.GetDbConnection();
        }
    }
}
