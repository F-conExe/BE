using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Update
{
    public class UpdateMembershipDTO
    {
        public int MembershipId { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Status { get; set; }
    }
}
