using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DVDRental.Areas.Identity.Data;

namespace DVDRental.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Actor> Actor { get; set; }
        public DbSet<CastMember> CastMember { get; set; }
        public DbSet<DVDCategory> DVDCategory { get; set; }
        public DbSet<DVDCopy> DVDCopy { get; set; }
        public DbSet<DVDTitle> DVDTitle { get; set; }
        public DbSet<Loan> Loan { get; set; }
        public DbSet<LoanType> LoanType { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<MembershipCategory> MembershipCategory { get; set; }
        public DbSet<Producer> Producer { get; set; }
        public DbSet<Studio> Studio { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
