namespace ShadowApiNet.Dto
{
    internal class LinkDto(string href, string rel, string type)
    {
        public string Href { get; set; } = href;
        public string Rel { get; set; } = rel;
        public string Type { get; set; } = type;
    }
}
