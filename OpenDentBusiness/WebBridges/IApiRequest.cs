using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebBridges
{
    public interface IApiRequest
    {
        T SendRequest<T>(string urlEndpoint, HttpMethod method, AuthenticationHeaderValue authHeaderVal, string body, HttpContentType contentType = HttpContentType.Json);
        Task<T> SendRequestAsync<T>(string urlEndpoint, HttpMethod method, AuthenticationHeaderValue authHeaderVal, string body, HttpContentType contentType = HttpContentType.Json);
    }
}
