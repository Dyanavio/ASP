namespace ASP.Models.Rest
{
    public class RestStatus
    {
        public bool IsOk { get; set; } = true;
        public int StatusCode { get; set; } = 200;
        public string StatusMessage { get; set; } = "Ok";

        public static readonly RestStatus RestStatus400 = new()
        {
            IsOk = false,
            StatusCode = 400,
            StatusMessage = "Bad Request"
        };

        public static readonly RestStatus RestStatus401 = new()
        {
            IsOk = false,
            StatusCode = 401,
            StatusMessage = "Unauthorized"
        };
        public static readonly RestStatus RestStatus403 = new()
        {
            IsOk = false,
            StatusCode = 403,
            StatusMessage = "Forbidden"
        };
        public static readonly RestStatus RestStatus409 = new()
        {
            IsOk = false,
            StatusCode = 409,
            StatusMessage = "Conflict"
        };
        public static readonly RestStatus RestStatus418 = new()
        {
            IsOk = false,
            StatusCode = 418,
            StatusMessage = "I'm a teapot"
        };

        public static readonly RestStatus RestStatus500 = new()
        {
            IsOk = false,
            StatusCode = 500,
            StatusMessage = "Internal Server Error. See details in server logs"
        };

    }
}
