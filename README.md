# shadow-api-net
ShadowApiNet is a tool that allows seamless generation of RESTful API in your ASP.NET Core app.

ShadowApiNet can generate RESTful API based on DbContext that you provide (hence you should manage DB connection yourself. 
And expose your SQL Database in a form of fully REST compliant API. 

In repository you will find a test project that shows how the library can be used.

Please be aware that generated API will expose your DB models to the API consumers. (In later versions it may support Automapper configuration)
