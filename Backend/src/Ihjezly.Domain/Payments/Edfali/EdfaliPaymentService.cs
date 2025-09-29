using Ihjezly.Application.Abstractions.Payment.Edfali;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace Ihjezly.Infrastructure.Payments.Edfali;

internal sealed class EdfaliPaymentService : IEdfaliPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly EdfaliOptions _options;

    public EdfaliPaymentService(HttpClient httpClient, IOptions<EdfaliOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> InitiateTransferAsync(string customerMobile, decimal amount)
    {
        var soapBody = $@"
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <DoPTrans xmlns=""http://tempuri.org/"">
      <Mobile>{_options.ReceiverPhone}</Mobile>
      <Pin>{_options.Pin}</Pin>
      <Cmobile>{customerMobile}</Cmobile>
      <Amount>{amount}</Amount>
      <PW>{_options.Password}</PW>
    </DoPTrans>
  </soap:Body>
</soap:Envelope>";

        var request = new HttpRequestMessage(HttpMethod.Post, "http://62.240.55.2:6187/BCDUssd/NewEdfali.asmx")
        {
            Content = new StringContent(soapBody, Encoding.UTF8, "text/xml")
        };
        request.Headers.Add("SOAPAction", "http://tempuri.org/DoPTrans");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        var doc = XDocument.Parse(content);
        return doc.Root?
            .Descendants(XName.Get("DoPTransResult", "http://tempuri.org/"))
            .FirstOrDefault()?.Value ?? "ERROR";
    }

    public async Task<string> ConfirmTransferAsync(string confirmationPin, string sessionId)
    {
        var soapBody = $@"
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <OnlineConfTrans xmlns=""http://tempuri.org/"">
      <Mobile>{_options.ReceiverPhone}</Mobile>
      <Pin>{confirmationPin}</Pin>
      <sessionID>{sessionId}</sessionID>
      <PW>{_options.Password}</PW>
    </OnlineConfTrans>
  </soap:Body>
</soap:Envelope>";

        var request = new HttpRequestMessage(HttpMethod.Post, "http://62.240.55.2:6187/BCDUssd/NewEdfali.asmx")
        {
            Content = new StringContent(soapBody, Encoding.UTF8, "text/xml")
        };
        request.Headers.Add("SOAPAction", "http://tempuri.org/OnlineConfTrans");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        var doc = XDocument.Parse(content);
        return doc.Root?
            .Descendants(XName.Get("OnlineConfTransResult", "http://tempuri.org/"))
            .FirstOrDefault()?.Value ?? "ERROR";
    }
}


public sealed class EdfaliOptions
{
    public string ReceiverPhone { get; set; } = string.Empty; // 9xxxxxxxx (merchant phone)
    public string Pin { get; set; } = string.Empty;           // merchant PIN
    public string Password { get; set; } = string.Empty;      // default: 123@xdsr$#!!
}