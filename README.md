# ShadowApiNet
[![](https://img.shields.io/nuget/v/ShadowApiNet?color=%231e96ff)](https://www.nuget.org/packages/ShadowApiNet/)
[![](https://github.com/n-smir/shadow-api-net/workflows/Build%20%26%20test/badge.svg?branch=master)](https://github.com/n-smir/shadow-api-net/actions?query=workflow%3A%22Build+%26+test%22)

#### Warning - the project is still in POC stage, so it might not feature a full functionality yet, functionality and usage might change as well.

ShadowApiNet is a tool that allows seamless generation of RESTful API in your ASP.NET Core app.

ShadowApiNet can generate RESTful API based on DbContext that you provide (hence you should manage DB connection yourself). 
And expose your SQL Database in a form of fully REST compliant API. 

### Installation

Using dotnet CLI:

```sh
dotnet add package ShadowApiNet
``` 
use ```--version``` to install spesific version.

Or using PackageManager Console:
```ps
PM> Install-Package ShadowApiNet
```
use ```-Version``` to install spesific version.

### Usage

The following section demonstrates how you can plug in and use ShadowApiNet in your ASP.Net Core application.

First create your DbContext: (example from official [EF Core docs](https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=netcore-cli))
``` cs
public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=blogging.db");
    }

    public class Blog
    {
        [Key]
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
```

And then in your `Startup.cs` add the following lines:
``` cs
using ShadowApiNet.Extensions;

...

public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BloggingContext>(ServiceLifetime.Singleton);
            services.AddShadowApi(new BloggingContext());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseShadowApi();
        }
    }
```

Now, when you start your application you'll be able to access generated API using `/dataapi` path.

For example:
``` sh
curl http://localhost:5000/dataapi/blogs
```
will return all the Blogs that are in the database.

And:

``` sh
curl http://localhost:5000/dataapi/blogs/1
```
will return Blog with Id 1 or `404 Not Found` if Blog with such id does not exist.



In repository you will find a test project that shows how the library can be used.

Please be aware that generated API will expose your DB models to the API consumers. (In later versions it may support Automapper configuration)
