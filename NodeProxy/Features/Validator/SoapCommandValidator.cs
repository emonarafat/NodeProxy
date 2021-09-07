

using FluentValidation;

using NodeProxy.Features.SoapFeature;

using System;
using System.Xml.Linq;

namespace NodeProxy.Features.Validator
{
    public class SoapCommandValidator : AbstractValidator<SoapCommand>
    {
        public SoapCommandValidator()
        {
            RuleFor(x => x.ServiceUrl).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.ServiceUrl));
            RuleFor(x => x.Xml).Must(IsValidXml).When(x => !string.IsNullOrEmpty(x.Xml));
            RuleFor(x => x.Headers).Must(v=>  v.ContainsKey("authorization") && v.ContainsKey("soapaction"));
        }
        private static bool IsValidXml( string xml)
        {
            try
            {
                XDocument.Parse(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
