using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowApiNet.Dto
{
    internal class ResponseWithLinksDto
    {
        public object Value { get; set; }
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
