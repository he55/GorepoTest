namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtension
    {
        public static ResultModel ResultSuccess(this ControllerBase _, object result)
        {
            return new ResultModel
            {
                Success = true,
                Message = "",
                Result = result
            };
        }

        public static ResultModel ResultFail(this ControllerBase _, string message)
        {
            return new ResultModel
            {
                Success = false,
                Message = message,
                Result = null
            };
        }
    }

    public struct ResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Result { get; set; }
    }
}
