using System.Net;

namespace JoseJWTToken.Transport.WebClient
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public string ResponseBody { get; set; }
        public string StatusDescription { get; set; }
    }
}