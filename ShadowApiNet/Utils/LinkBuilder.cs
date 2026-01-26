using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ShadowApiNet.Dto;

namespace ShadowApiNet.Utils;

internal class LinkBuilder
{
    internal static List<LinkDto> CreateLinksForList(string requestPath)
    {
        return [
            new(requestPath, "self", HttpMethods.Get),
            new(requestPath, "create", HttpMethods.Post)
        ];
    }

    internal static List<LinkDto> CreateLinksForObject(string requestPath, string id = "")
    {
        string linkHref = (requestPath + id).TrimEnd('/');
        return [
            new(linkHref, "self", HttpMethods.Get),
            new(linkHref, "update", HttpMethods.Put),
            new(linkHref, "partial-update", HttpMethods.Patch),
            new(linkHref, "delete", HttpMethods.Delete)
        ];
    }
}
