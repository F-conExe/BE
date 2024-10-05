using DATA.Base;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA.Repository
{
    public class ReviewRepo : GenericRepo<Review>
    {
        public ReviewRepo() { }

        public ReviewRepo(exe201cContext context) : base(context) => _context = context;
    }
}

