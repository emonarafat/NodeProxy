using Newtonsoft.Json;

using System.Collections.Generic;
#nullable enable
namespace NodeProxy.Models
{
    public class ResponseModel : ApiResponse
    {
        public ResponseModel(int statusCode=200, string? message = null) : base(statusCode, message)
        {
            IsValid = true;
            ValidationMessages = new List<string>();
        }
        [JsonProperty("isValid", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsValid { get; set; }
        [JsonProperty("isSuccess", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSuccess { get; set; }
        [JsonProperty("validationMessages", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? ValidationMessages { get; set; }

        public class WithData :ResponseModel
        {
            public WithData() : base(200)
            {
            }
            public WithData(object data) : this()
            {

                Data = data;
            }

            [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
            public object? Data { get; set; }
        }
        public class NoData : ResponseModel
        {
            public NoData(int statusCode, string? message = null) : base(statusCode, message)
            {
            }
        }
    }
}

