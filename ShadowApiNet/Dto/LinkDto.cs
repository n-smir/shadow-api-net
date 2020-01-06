namespace ShadowApiNet.Dto
{
    internal class LinkDto
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Type { get; set; }

        public LinkDto(string href, string rel, string type)
        {
            this.Href = href;
            this.Rel = rel;
            this.Type = type;
        }
    }
}
