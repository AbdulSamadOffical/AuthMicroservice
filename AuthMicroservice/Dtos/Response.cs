namespace AuthMicroservice.Dtos
{
    public class Response<T>
    {
        public int Status { get; set; } = 200;
        public string Message { get; set; }
        public T? Data { get; set; }
    }

    public class ResponseList<T>
    {
        public int Status { get; set; } = 200;
        public string Message { get; set; } 
        public IEnumerable<T>? Data { get; set; }
    }

    public class GatewayCustomResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } 
    }
}
