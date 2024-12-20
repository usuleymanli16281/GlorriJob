using GlorriJob.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Contexts
{
	public class GlorriJobDbContext : IdentityDbContext<IdentityUser>
	{
        public GlorriJobDbContext(DbContextOptions<GlorriJobDbContext> options) : base(options)
        {
            
        }
        public DbSet<Biography> Biographies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyDetail> CompanyDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<VacancyDetail> VacancyDetails { get; set;}
    }
}
