using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace InventorySystem.Common.Utilities.Responses;

public record Result
{
    public string? Message { get; set; }

    [JsonIgnore]
    public bool IsNotFound { get; private set; }

    private IImmutableList<ErrorMessage> Errors { get; set; } = [];
   
    public bool IsSuccess => Errors.Count == 0;

    protected Result() {}

    protected Result(List<ErrorMessage> messages)
    {
        Errors = messages.ToImmutableList();
    }

    protected Result(bool notFound, string? message = null)
    {
        IsNotFound = notFound;
        Message = message;
    }

    public static Result Success() => new Result();

    public static Result Failure(List<ErrorMessage> messages)
        => new(messages);

    public static Result NotFound(string? message = null)
        => new(true, message);

}

public record Result<T> : Result
{
    public T? Data { get; set; }

    private Result(T? data)
    {
        Data = data;
    }

    private Result(bool notFound, string? message) : base(notFound, message) {}

    private Result(List<ErrorMessage> messages) : base(messages) {}

    public static Result<T> Success(T data) => new(data);

    public new static Result<T> Failure(List<ErrorMessage> messages)
        => new(messages);

    public new static Result<T> NotFound(string? message = null)
        => new(true, message);
}