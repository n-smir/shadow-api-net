using ShadowApiNet.Example;
using ShadowApiNet.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BloggingContext>(ServiceLifetime.Singleton);
builder.Services.AddShadowApi(new BloggingContext());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// var context = new BloggingContext();

// var blog = new Blog {
//     BlogId = 1,
//     Name = "My Cool Blog"
// };

// var post = new Post {
//     PostId = 1,
//     Title = "First Post",
//     Content = "Awesome post about important stuff.",
//     BlogId = blog.BlogId,
//     Blog = blog
// };

// context.Blogs.Add(blog);
// context.Posts.Add(post);
// context.SaveChanges();

app.UseHttpsRedirection();

app.UseShadowApi();

app.MapGet("/helloworld", () =>
{
    return "Hello world!";
})
.WithName("GetHelloWorld")
.WithOpenApi();

app.Run();
