using System.Net;

namespace FiresharpCore.Response
{
    public class SetResponse : FirebaseResponse
    {
        public SetResponse(string body, HttpStatusCode statusCode)
            : base(body, statusCode)
        {
        }
    }
}