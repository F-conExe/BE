using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Create
{
    public class CreateUserDTO
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string UserType { get; set; }

        public string ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int numberOfJobDonne {  get; set; }

        public string Location { get; set; }

        public string DeliveryTime { get; set; }

        public string LanguageLevel { get; set; }

        public string ImgUrl { get; set; }

    }
}
