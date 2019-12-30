namespace ShadowApiNet.Interfaces
{
    internal interface IHttpHandlerFactory
    {
        IHttpMethodHandler GetHttpMethodHandler(string httpMethod);
    }
}
