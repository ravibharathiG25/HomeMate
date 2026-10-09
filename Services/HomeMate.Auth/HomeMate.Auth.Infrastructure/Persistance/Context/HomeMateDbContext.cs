using HomeMate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Infrastructure.Persistance.Context
{
    public class HomeMateDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public HomeMateDbContext(DbContextOptions<HomeMateDbContext> options) : base(options)
        {

        }
    }
}
