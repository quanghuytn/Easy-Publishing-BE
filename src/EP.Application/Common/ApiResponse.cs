using Azure;
using System.Collections.Generic;
using System.Numerics;

namespace EP.Application.Common
{
    public class ApiResponse<T>
    {
        public int EC { get; set; }
        public string EM { get; set; }
        public T DT { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                EC = 0,
                EM = message,
                DT = data
            };
        }

        public static ApiResponse<T> Failure(string errorMessage)
        {
            return new ApiResponse<T>
            {
                EC = -1,
                EM = errorMessage,
                DT = default
            };
        }
    }
}
