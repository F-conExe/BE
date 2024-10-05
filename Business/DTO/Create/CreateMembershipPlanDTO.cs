using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Create
{
    public class CreateMembershipPlanDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public string Features { get; set; }

    }
}
