using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SharedKernel.WebAPI.Responses
{
    public record ValidationError(string Property, string Message);

    public class ControllerResponse<TResponse>
    {
        [JsonConstructor]
        protected ControllerResponse() { }

        protected ControllerResponse(TResponse? response, bool isSuccess, IReadOnlyList<ValidationError>? errors)
        {
            Object = response;
            Success = isSuccess;
            Errors = errors;
        }

        public TResponse? Object { get; set; }

        public bool Success { get; set; }

        public IReadOnlyList<ValidationError>? Errors { get; init; }

        public static ControllerResponse<TResponse> SuccessWith(TResponse response)
            => new(response, true, null);

        public static ControllerResponse<TResponse> FailureWith(params ValidationError[] errors)
            => new(default, false, errors);

        public static ControllerResponse<TResponse> FailureWith(IReadOnlyList<ValidationError> errors)
            => new(default, false, errors);

        public static ControllerResponse<TResponse> EmptySuccess()
            => new(default, true, null);
    }
}
