using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using NodeProxy.Models;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NodeProxy.Features.SoapFeature
{

    public class SoapCommand : SoapRequestModel, IRequest<ResponseModel.WithData<object>>
    {
        [JsonIgnore]
        public IHeaderDictionary Headers { get; set; }

        public class SoapCommandHandler : IRequestHandler<SoapCommand, ResponseModel.WithData<object>>
        {
            private readonly HttpClient _httpClient;
            private readonly IValidator<SoapCommand> _validator;
            public SoapCommandHandler(IValidator<SoapCommand> validator)
            {
                this._httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.All });
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                _validator = validator;
            }

            public async Task<ResponseModel.WithData<object>> Handle(SoapCommand request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request,cancellationToken);
                var responseData = new ResponseModel.WithData<object>();
                List<string> ValidationMessages = new();
                if (!validationResult.IsValid)
                {
                    responseData.IsValid = false;
                    responseData.IsSuccess = false;
                    ValidationMessages.AddRange(validationResult.Errors.Select(failure => failure.ErrorMessage));
                    responseData.ValidationMessages = ValidationMessages;
                    return responseData;
                }
                _httpClient.BaseAddress = new Uri(request.ServiceUrl);
                var content = new StringContent(request.Xml, Encoding.UTF8, "text/xml");
                var _request = new HttpRequestMessage() { Content = content, Method = HttpMethod.Post };
                request.Headers.TryGetValue("authorization", out var authorization);
                request.Headers.TryGetValue("soapaction", out var soapaction);
                _request.Headers.Add("Authorization", authorization.ToString());
                _request.Headers.Add("SOAPAction", soapaction.ToString());

                var response = await _httpClient.SendAsync(_request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync(cancellationToken);
                    var doc = XDocument.Load(resp);
                    var jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
                    return new ResponseModel.WithData<dynamic>(dyn);
                }
                responseData.IsSuccess = false;
                return responseData;
            }
        }
    }

}
