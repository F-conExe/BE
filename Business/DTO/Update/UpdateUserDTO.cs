using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Update
{
    public class UpdateUserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string UserType { get; set; }

        public string ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
