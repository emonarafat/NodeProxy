using Newtonsoft.Json;

using System.Collections.Generic;

namespace NodeProxy.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            IsValid = true;
            ValidationMessages = new List<string>();
        }
        [JsonProperty("isValid", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsValid { get; set; }
        [JsonProperty("isSuccess", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSuccess { get; set; }
        [JsonProperty("validationMessages", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ValidationMessages { get; set; }

        public class WithData<T> : ResponseModel
        {
            public WithData() : base()
            {
            }
            public WithData(T data):this()
            {
               
                Data = data;
            }

            [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
            public T Data { get; set; }
        }
    }
}
