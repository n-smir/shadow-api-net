# ShadowApiNet
[![](https://img.shields.io/nuget/v/ShadowApiNet?color=%231e96ff)](https://www.nuget.org/packages/ShadowApiNet/)
[![](https://github.com/n-smir/shadow-api-net/workflows/Build%20%26%20test/badge.svg?branch=master)](https://github.com/n-smir/shadow-api-net/actions?query=workflow%3A%22Build+%26+test%22)

ShadowApiNet is a tool that allows seamless generation of RESTful API in your ASP.NET Core app.

ShadowApiNet can generate RESTful API based on DbContext that you provide (hence you should manage DB connection yourself. 
And expose your SQL Database in a form of fully REST compliant API. 

In repository you will find a test project that shows how the library can be used.

Please be aware that generated API will expose your DB models to the API consumers. (In later versions it may support Automapper configuration)
