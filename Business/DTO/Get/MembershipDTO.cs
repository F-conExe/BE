using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO.Get
{
    public class MembershipDTO
    {
        public int MembershipId { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public List<MembershipPlanAssignmentDTO> MembershipPlanAssignments { get; set; } = new List<MembershipPlanAssignmentDTO>();
    }

    public class MembershipPlanAssignmentDTO
    {
        public int PlanAssignmentId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; }
    }

    public class UserMembershipsDTO
    {
        public int UserId { get; set; }
        public List<MembershipDTO> Memberships { get; set; }
    }
}
