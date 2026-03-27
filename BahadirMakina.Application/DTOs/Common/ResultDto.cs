namespace BahadirMakina.Application.DTOs.Common;

public class ResultDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ResultDto<T> SuccessResult(T data, string? message = null)
    {
        return new ResultDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ResultDto<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ResultDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class ResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static ResultDto SuccessResult(string? message = null)
    {
        return new ResultDto
        {
            Success = true,
            Message = message
        };
    }

    public static ResultDto FailureResult(string message, List<string>? errors = null)
    {
        return new ResultDto
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}