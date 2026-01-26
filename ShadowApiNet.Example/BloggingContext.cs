namespace ShadowApiNet.Example;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "blogging.db");
        Console.WriteLine(DbPath);
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Blog
{
    [Key]
    public int BlogId { get; set; }
    public required string Name { get; set; }

    public List<Post> Posts { get; } = new();
}

public class Post
{
    [Key]
    public int PostId { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }

    public int BlogId { get; set; }
    public required Blog Blog { get; set; }
}
