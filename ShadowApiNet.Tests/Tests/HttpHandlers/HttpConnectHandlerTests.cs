using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.HttpHandlers;
using ShadowApiNet.Utils;

namespace ShadowApiNet.Tests;

public class HttpConnectHandlerTests
{
    private class TestDbContext : DbContext
    {
        public DbSet<TestEntity>? TestEntities { get; set; }
    }

    private class TestEntity
    {
        public int Id { get; set; }
    }

    [Fact]
    public async void When_Handle_Then_Return_Status501NotImplemented()
    {
        var mockContext = new DefaultHttpContext();
        var dbContext = new TestDbContext();
        var dbAccessor = new DbAccessor(dbContext);
        var connectHandler = new HttpConnectHandler(dbAccessor);

        var context = await connectHandler.Handle(mockContext, null, null);
        var response = context.Response;

        Assert.Equal(StatusCodes.Status501NotImplemented, response.StatusCode);
    }
}
