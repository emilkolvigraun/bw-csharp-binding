namespace BWBinding.Common
{
    public class Response
    {
        public string status { private set; get; }
        public string reason { private set; get; }

        public Response(string status, string reason)
        {
            this.status = status;
            this.reason = reason;
        }
    }
}
