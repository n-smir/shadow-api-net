using Microsoft.EntityFrameworkCore;
using ShadowApiNet.HttpHandlers;
using ShadowApiNet.Utils;

namespace ShadowApiNet.Tests
{
    public class HttpMethodHandlerFactoryTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<TestEntity>? TestEntities { get; set; }
        }

        private class TestEntity
        {
            public int Id { get; set; }
        }

        private static HttpMethodHandlerFactory CreateFactory()
        {
            var dbContext = new TestDbContext();
            var dbAccessor = new DbAccessor(dbContext);
            return new HttpMethodHandlerFactory(dbAccessor);
        }

        [Fact]
        public void Given_Get_When_GetHttpMethodHandler_Then_Return_HttpGetHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "GET";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpGetHandler>(handler);
        }

        [Fact]
        public void Given_GetHandler_Exists_When_GetHttpMethodHandler_Then_Return_HttpGetHandler_From_Cache()
        {
            var testInstance = CreateFactory();
            var httpMethod = "GET";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpGetHandler>(handler);

            var cachedHandler = testInstance.GetHttpMethodHandler(httpMethod);
            Assert.Equal(cachedHandler, handler);
        }

        [Fact]
        public void Given_Post_When_GetHttpMethodHandler_Then_Return_HttpPostHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "POST";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpPostHandler>(handler);
        }

        [Fact]
        public void Given_Put_When_GetHttpMethodHandler_Then_Return_HttpPutHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "PUT";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpPutHandler>(handler);
        }

        [Fact]
        public void Given_Patch_When_GetHttpMethodHandler_Then_Return_HttpPatchHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "PATCH";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpPatchHandler>(handler);
        }

        [Fact]
        public void Given_Delete_When_GetHttpMethodHandler_Then_Return_HttpDeleteHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "DELETE";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpDeleteHandler>(handler);
        }

        [Fact]
        public void Given_Options_When_GetHttpMethodHandler_Then_Return_HttpOptionsHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "OPTIONS";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpOptionsHandler>(handler);
        }

        [Fact]
        public void Given_Head_When_GetHttpMethodHandler_Then_Return_HttpHeadHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "HEAD";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpHeadHandler>(handler);
        }

        [Fact]
        public void Given_Connect_When_GetHttpMethodHandler_Then_Return_HttpConnectHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "CONNECT";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpConnectHandler>(handler);
        }

        [Fact]
        public void Given_Trace_When_GetHttpMethodHandler_Then_Return_HttpTraceHandler()
        {
            var testInstance = CreateFactory();
            var httpMethod = "TRACE";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpTraceHandler>(handler);
        }

        [Fact]
        public void Given_InvalidHttpMethod_When_GetHttpMethodHandler_Then_Throw_NotSupportedException()
        {
            var testInstance = CreateFactory();
            var httpMethod = "INVALID_HTTP_METHOD";

            Assert.Throws<NotSupportedException>(() => testInstance.GetHttpMethodHandler(httpMethod));
        }
    }
}