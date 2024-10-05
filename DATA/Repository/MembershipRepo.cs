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
    }
}
