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
    public class PostRepo : GenericRepo<Post>
    {

        public PostRepo() { }

        public PostRepo(exe201cContext context) : base(context) => _context = context;

        public async Task<IEnumerable<Post>> SearchAsync(string skill = null, decimal? minSalary = null, decimal? maxSalary = null, int? postTypeId = null)
        {
            return await _context.Posts.Where(p => p.Skills.Contains(skill) || p.BudgetOrSalary <= minSalary || p.BudgetOrSalary <= maxSalary || p.PostTypeId == postTypeId).ToListAsync();

           
        }


        public async Task<int> GetPostCount()
        {
            return await _context.Posts.CountAsync();
        }


    }


}

