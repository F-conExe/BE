using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Update
{
    public class UpdatePostDTO
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        public int PostTypeId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal? BudgetOrSalary { get; set; }

        public string Skills { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
