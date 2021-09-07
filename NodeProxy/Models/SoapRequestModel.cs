using System.ComponentModel.DataAnnotations;

namespace NodeProxy.Models
{
    public class SoapRequestModel
    {
        [DataType(DataType.Url),Required]
        public string ServiceUrl { get; set; }
        [DataType(DataType.Text),Required]
        public string Xml { get; set; }
    }
}
