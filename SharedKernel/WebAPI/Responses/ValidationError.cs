using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SharedKernel.WebAPI.Responses
{
    public record ValidationError(string Property, string Message);
    
}
