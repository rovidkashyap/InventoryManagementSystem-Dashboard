namespace IMS_Dashboard.Models.Responses
{
    public class ApiResponseDto<T>
    {
        public int Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
