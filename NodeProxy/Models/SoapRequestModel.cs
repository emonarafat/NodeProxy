using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;

namespace NodeProxy.Models
{
    public class SoapRequestModel
    {
        [DataType(DataType.Url),Required]
        [FromBody]
        public string ServiceUrl { get; set; }
        [DataType(DataType.Text),Required]
        [FromBody]
        public string Xml { get; set; }
        [FromHeader(Name = "Authorization"),JsonIgnore]
        public string Authorization { get; set; }
        [FromHeader(Name = "SoapAction"), JsonIgnore]
        public string SoapAction { get; set; }
    }
}
