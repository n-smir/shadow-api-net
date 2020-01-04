namespace ShadowApiNet.Abstractions
{
    internal interface IHttpHandlerFactory
    {
        IHttpMethodHandler GetHttpMethodHandler(string httpMethod);
    }
}
