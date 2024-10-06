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
    public class MembershipRepo : GenericRepo<Membership>
    {
        public MembershipRepo() { }

        public MembershipRepo(exe201cContext context) : base(context) => _context = context;

        public async Task<Membership> GetByUserIdAsync(int userId)
        {
            return await _context.Memberships
                .Include(m => m.MembershipPlanAssignments)
                    .ThenInclude(mpa => mpa.Plan)
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<List<Membership>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Memberships
                .Where(m => m.UserId == userId)
                .Include(m => m.MembershipPlanAssignments)
                    .ThenInclude(mpa => mpa.Plan)
                .ToListAsync();
        }
    }
}
