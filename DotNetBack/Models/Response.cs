namespace DotNetBack.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string StatusDescription { get; set; } = string.Empty;

        public Response()
        {
            StatusCode = 200;
            StatusMessage = "OK";
        }
    }
}
