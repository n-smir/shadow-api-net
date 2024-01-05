using ShadowApiNet;
using ShadowApiNet.HttpHandlers;
using Xunit;

namespace ShadowApiNet.Tests
{
    public class HttpMethodHandlerFactoryTests
    {
        [Fact]
        public void Given_Get_When_GetHttpMethodHandler_Then_Return_HttpGetHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "GET";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpGetHandler>(handler);
        }

        [Fact]
        public void Given_Post_When_GetHttpMethodHandler_Then_Return_HttpPostHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "POST";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpPostHandler>(handler);
        }

        [Fact]
        public void Given_Put_When_GetHttpMethodHandler_Then_Return_HttpPutHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "PUT";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpPutHandler>(handler);
        }

        [Fact]
        public void Given_Patch_When_GetHttpMethodHandler_Then_Return_HttpPatchHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "PATCH";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpPatchHandler>(handler);
        }

        [Fact]
        public void Given_Delete_When_GetHttpMethodHandler_Then_Return_HttpDeleteHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "DELETE";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpDeleteHandler>(handler);
        }

        [Fact]
        public void Given_Options_When_GetHttpMethodHandler_Then_Return_HttpOptionsHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "OPTIONS";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpOptionsHandler>(handler);
        }

        [Fact]
        public void Given_Head_When_GetHttpMethodHandler_Then_Return_HttpHeadHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "HEAD";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpHeadHandler>(handler);
        }

        [Fact]
        public void Given_Connect_When_GetHttpMethodHandler_Then_Return_HttpConnectHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "CONNECT";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpConnectHandler>(handler);
        }

        [Fact]
        public void Given_Trace_When_GetHttpMethodHandler_Then_Return_HttpTraceHandler()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "TRACE";

            var handler = testInstance.GetHttpMethodHandler(httpMethod);

            Assert.NotNull(handler);
            Assert.IsType<HttpTraceHandler>(handler);
        }

        [Fact]
        public void Given_InvalidHttpMethod_When_GetHttpMethodHandler_Then_Throw_NotSupportedException()
        {
            var testInstance = new HttpMethodHandlerFactory();
            var httpMethod = "INVALID_HTTP_METHOD";

            Assert.Throws<NotSupportedException>(() => testInstance.GetHttpMethodHandler(httpMethod));
        }
    }
}