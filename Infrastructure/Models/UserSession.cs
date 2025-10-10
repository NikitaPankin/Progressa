using Application.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class UserSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string RefreshTokenHash { get; set; }

        public DateTime RefreshUpdatedAtUtc { get; set; }
    }
}
