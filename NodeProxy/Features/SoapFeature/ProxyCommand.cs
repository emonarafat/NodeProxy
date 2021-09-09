using FluentValidation;

using MediatR;

using Newtonsoft.Json;

using NodeProxy.Extensions;
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

    public class ProxyCommand : SoapRequestModel, IRequest<ResponseModel.WithData>
    {
        public class ProxyCommandHandler : IRequestHandler<ProxyCommand, ResponseModel.WithData>
        {
            private readonly HttpClient _httpClient;
            private readonly IValidator<ProxyCommand> _validator;
            public ProxyCommandHandler(IValidator<ProxyCommand> validator)
            {
                this._httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.All });
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                _validator = validator;
            }

            public async Task<ResponseModel.WithData> Handle(ProxyCommand request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                var responseData = new ResponseModel.WithData();
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
                _request.Headers.Add("Authorization", request.Authorization);
                _request.Headers.Add("SOAPAction", request.SoapAction);

                var response = await _httpClient.SendAsync(_request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                 
                   
                    responseData.StatusCode = (int)response.StatusCode;
                    responseData.Message = "Error Accessing Soap Url and action";
                    responseData.IsSuccess = false;
                    return responseData;
                }
                try
                {
                    var resp = await response.Content.ReadAsStringAsync(cancellationToken);
                    var doc = XDocument.Load(resp);
                    var jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
                    return new ResponseModel.WithData(dyn);
                }
                catch(Exception ex)
                {
                    responseData.Message = ex.Message;
                    responseData.IsSuccess = false;
                    return responseData;
                }
            }
        }
    }

}
