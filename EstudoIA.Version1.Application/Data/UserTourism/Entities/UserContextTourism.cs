using EstudoIA.Version1.Application.Data.UserContext.Abstractions;
using EstudoIA.Version1.Application.Data.UserTripPlans.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Data.UserTourism.Entities
{
    public class UserContextTourism
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            PasswordHash = PasswordHasher.Hash(password);
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHasher.Verify(PasswordHash, password);
        }
    }
}
