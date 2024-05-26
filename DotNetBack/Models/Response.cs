namespace DotNetBack.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public Response()
        {
            StatusCode = 200;
        }

        public Response(int statusCode, string message = null, object data = null)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }
}