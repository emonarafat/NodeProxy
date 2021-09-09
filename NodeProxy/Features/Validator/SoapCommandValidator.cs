using FluentValidation;

using NodeProxy.Features.SoapFeature;

using System;
using System.Xml.Linq;

namespace NodeProxy.Features.Validator
{
    public class SoapCommandValidator : AbstractValidator<ProxyCommand>
    {
        public SoapCommandValidator()
        {
            RuleFor(x => x.ServiceUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out _)).When(x => !string.IsNullOrEmpty(x.ServiceUrl)).WithMessage("Service Url Needs to be a valid Uri");
            RuleFor(x => x.Xml).NotEmpty().WithMessage("Xml should not be empty/null").Must(IsValidXml).When(x => !string.IsNullOrEmpty(x.Xml)).WithMessage("Please provide valid XML");
            RuleFor(x => x.Authorization).NotEmpty();
            RuleFor(x => x.SoapAction).NotEmpty();
        }
        private static bool IsValidXml(string xml)
        {
            try
            {
                _ = XDocument.Parse(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
