using DATA.Base;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA.Repository
{
    public class PostTypeRepo : GenericRepo<PostType>
    {
        public PostTypeRepo() { }

        public PostTypeRepo(exe201cContext context) : base(context) => _context = context;
    }
}
