using DATA.Base;
using DATA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA.Repository
{
    public class MembershipPlanAsmRepo : GenericRepo<MembershipPlanAssignment>
    {

        public MembershipPlanAsmRepo() { }

        public MembershipPlanAsmRepo(exe201cContext context) : base(context) => _context = context;

        public async Task<List<MembershipPlanAssignment>> GetMembershipPlansByUserIdAsync(int userId)
        {
            return await _context.MembershipPlanAssignments
                .Include(m => m.Membership)
                .Where(m => m.Membership.UserId == userId)
                .ToListAsync();
        }
    }
}
