

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
            RuleFor(x => x.ServiceUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.ServiceUrl));
            RuleFor(x => x.Xml).NotEmpty().Must(IsValidXml).When(x => !string.IsNullOrEmpty(x.Xml));
            //RuleFor(x => x.Headers).NotNull().Must(v=>  v.ContainsKey("authorization") && v.ContainsKey("soapaction")).When(s=>s.Headers!=null);
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
