using System;

namespace Insta.Web.Models
{
    [Serializable]
    public class Result
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public static Result Success() => new Result
        {
            IsSuccess = true
        };

        public static Result Failure(string message) => new Result
        {
            Message = message
        };
    }

    [Serializable]
    public class Result<TContent> : Result
    {
        public TContent Content { get; set; }

        public static Result<TContent> Success(TContent content) => new Result<TContent>
        {
            IsSuccess = true,
            Content = content
        };

        public new static Result<TContent> Failure(string message) => new Result<TContent>
        {
            Message = message
        };
    }
}
