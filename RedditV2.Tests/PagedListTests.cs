using Microsoft.EntityFrameworkCore;
using Reddit.Repositories;

namespace RedditV2.Tests
{
    public class PagedListTests
    {
        private readonly TestDbContext _context;

        public PagedListTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TestDbContext(options);
        }

        private async Task SeedDataAsync(int count)
        {
            var items = Enumerable.Range(1, count)
                .Select(i => new TestItem { Id = i })
                .ToList();
            await _context.TestItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCorrectItemsForPage()
        {
          
            await SeedDataAsync(100);
            var query = _context.TestItems.OrderBy(x => x.Id);
            var pageNumber = 2;
            var pageSize = 10;
            var pagedList = await PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize);
            Assert.Equal(10, pagedList.Items.Count);
            Assert.Equal(11, pagedList.Items.First().Id);
            Assert.Equal(20, pagedList.Items.Last().Id);
            Assert.Equal(100, pagedList.TotalCount);
        }

        [Fact]
        public async Task CreateAsync_HasNextPageAndHasPreviousPage()
        {
            
            await SeedDataAsync(100);
            var query = _context.TestItems.OrderBy(x => x.Id);
            var pageNumber = 2;
            var pageSize = 10;

            
            var pagedList = await PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize);


            Assert.True(pagedList.HasNextPage);
            Assert.True(pagedList.HasPreviousPage);
            Assert.Equal(2, pagedList.PageNumber);
            Assert.Equal(10, pagedList.PageSize);
        }

        [Fact]
        public async Task CreateAsync_EmptyList()
        {
          
            var query = _context.TestItems.Where(x => false);
            var pageNumber = 1;
            var pageSize = 10;

         
            var pagedList = await PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize);

         
            Assert.Empty(pagedList.Items);
            Assert.False(pagedList.HasNextPage);
            Assert.False(pagedList.HasPreviousPage);
            Assert.Equal(0, pagedList.TotalCount);
        }

        [Fact]
        public async Task CreateAsync_PageSizeLargerThan50_ThrowsException()
        {
            
            await SeedDataAsync(100);
            var query = _context.TestItems.OrderBy(x => x.Id);
            var pageNumber = 1;
            var pageSize = 51;

            
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize));
        }

        [Theory]
        [InlineData(0, 10)]  
        [InlineData(1, 0)]   
        [InlineData(-1, 10)]
        [InlineData(1, -1)] 
        public async Task CreateAsync_InvalidParameters_ThrowsException(int pageNumber, int pageSize)
        {
            
            var query = _context.TestItems.OrderBy(x => x.Id);

            
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize));
        }

        [Fact]
        public async Task CreateAsync_LastPage_HasNoNextPage()
        {
            
            await SeedDataAsync(20);
            var query = _context.TestItems.OrderBy(x => x.Id);
            var pageNumber = 2;
            var pageSize = 10;

           
            var pagedList = await PagedList<TestItem>.CreateAsync(query, pageNumber, pageSize);


            Assert.False(pagedList.HasNextPage);
            Assert.True(pagedList.HasPreviousPage);
            Assert.Equal(20, pagedList.TotalCount);
        }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestItem> TestItems { get; set; }
    }

    public class TestItem
    {
        public int Id { get; set; }
    }
}
