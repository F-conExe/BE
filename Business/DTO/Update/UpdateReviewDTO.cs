using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Update
{
    public class UpdateReviewDTO
    {
        public int ReviewId { get; set; }

        public int PostId { get; set; }

        public int ReviewerId { get; set; }

        public int RevieweeId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
