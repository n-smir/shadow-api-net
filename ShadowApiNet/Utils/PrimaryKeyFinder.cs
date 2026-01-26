using System;
using System.Linq;
using System.Reflection;

namespace ShadowApiNet.Utils;

public class PrimaryKeyFinder
{
    private const string PRIMARY_KEY_ATTRIBUTE_VALUE = "KeyAttribute";
    private static PropertyInfo GetPrimaryKeyByKeyAttribute(Type type)
    {
        return type.GetProperties()
            .FirstOrDefault(
                p => p.CustomAttributes.Any(
                    ca => ca.AttributeType.Name == PRIMARY_KEY_ATTRIBUTE_VALUE
                )
            );
    }

    private const string PRIMARY_KEY_CONVENTION_SUFFIX = "Id";
    private static PropertyInfo GetPrimaryKeyByConventionName(Type type)
    {
        return type.GetProperties().FirstOrDefault(p => p.Name == (type.Name + PRIMARY_KEY_CONVENTION_SUFFIX));
    }

    internal static PropertyInfo GetPrimaryKeyFromType(Type type)
    {
        return GetPrimaryKeyByKeyAttribute(type) ?? GetPrimaryKeyByConventionName(type);
    }
}
