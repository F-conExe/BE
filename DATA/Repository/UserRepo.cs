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
    public class UserRepo : GenericRepo<User>
    {
        public UserRepo() { }

        public UserRepo(exe201cContext context) : base(context) => _context = context;

        public async Task<User> FindByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
