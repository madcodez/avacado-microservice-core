
using static Avacado.Web.Utility.SD;

namespace Avacado.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccesToken { get; set; }

        public ContentType ContentType { get; set; } = ContentType.Json;

    }
}
