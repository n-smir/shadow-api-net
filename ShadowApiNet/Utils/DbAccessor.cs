using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShadowApiNet.Models;

namespace ShadowApiNet.Utils;

internal class DbAccessor
{
    private readonly DbContext _context;
    private readonly Dictionary<PropertyInfo, TableModel> _tables;

    public DbAccessor(DbContext context)
    {
        _context = context;
        _tables = [];

        foreach (PropertyInfo prop in _context.GetType().GetProperties())
        {
            if (prop.PropertyType.Name.Contains("DbSet"))
            {
                var type = prop.PropertyType.GetGenericArguments().FirstOrDefault();

                _tables.Add(prop, new TableModel
                {
                    Type = type,
                    Fields = type.GetProperties(),
                    PK = PrimaryKeyFinder.GetPrimaryKeyFromType(type)
                });
            }
        }
    }

    public async Task<List<object>> GetAllObjectInRange(string entityName, int offset, int limit)
    {
        var entityType = ResolveObjectType(entityName);

        var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
        var genericSetMethod = setMethod.MakeGenericMethod(entityType);
        var dbSet = genericSetMethod.Invoke(_context, null);

        var queryableType = typeof(IQueryable<>).MakeGenericType(entityType);
        var query = (IQueryable<object>)(IQueryable)dbSet;

        if (offset > 0)
        {
            query = query.Skip(offset);
        }

        if (limit > 0)
        {
            query = query.Take(limit);
        }

        return await query.ToListAsync();
    }

    public async Task<object> GetObjectByPrimaryKey(string entityName, string objectPK)
    {
        var propTablePair = ResolveObjectMetadata(entityName);
        var resolvedPrimaryKey = Convert.ChangeType(objectPK, propTablePair.Value.PK.PropertyType);
        return await _context.FindAsync(propTablePair.Value.Type, resolvedPrimaryKey);
    }

    public async Task CreateObject(object obj)
    {
        _context.Add(obj);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateObject(object obj)
    {
        _context.Update(obj);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteObject(object obj)
    {
        _context.Remove(obj);
        await _context.SaveChangesAsync();
    }

    public List<string> GetTableNames()
    {
        return _tables.Keys.Select(x => x.Name.ToLower()).ToList();
    }

    public PropertyInfo GetTablePrimaryKey(string entityName)
    {
        TableModel table = ResolveObjectMetadata(entityName).Value;
        return table.PK;
    }

    public KeyValuePair<PropertyInfo, TableModel> ResolveObjectMetadata(string entityName)
    {
        return _tables.First(pair => pair.Key.Name.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
    }

    public Type ResolveObjectType(string entityName)
    {
        return this.ResolveObjectMetadata(entityName).Value.Type;
    }
}
