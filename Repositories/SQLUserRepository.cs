using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using System.Drawing;

namespace Reddit.Repositories
{
    public class SQLUserRepository
    {

        private readonly ApplicationDbContext dbContext;

        public SQLUserRepository(ApplicationDbContext dbCcontext)
        {
            this.dbContext = dbContext;
        }


        public async Task<User> GetAllAsync(int pageNumber = 1, int pageSize = 1000, bool? isAscending = true, string? sortKey = null, string? searchKey = null, string? filterOn = null)
        {

            var user = dbContext.Users.AsQueryable();


            if (string.IsNullOrEmpty(searchKey))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    user = user.Where(x => x.Name.ToLower().Contains(searchKey.ToLower()));
                }
            }




            if (string.IsNullOrWhiteSpace(sortKey) == false)
            {
                if(sortKey.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    user = isAscending??true? user.OrderBy(x => x.Name):user.OrderByDescending(x => x.Name);
                    
                
                
                }
            }


           

            


            var skipResults = (pageNumber - 1) * pageSize;
            return await user.Skip(skipResults).Take(pageSize).ToListAsync();


        }
    }
}
