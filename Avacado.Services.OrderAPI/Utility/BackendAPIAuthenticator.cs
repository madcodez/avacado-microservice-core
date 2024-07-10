using Microsoft.AspNetCore.Authentication;

namespace Avacado.Services.OrderAPI.Utility
{
    public class BackendAPIAuthenticator : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public BackendAPIAuthenticator(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken cancellationToken) 
        {
            var token = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(req, cancellationToken);
        }
    }
}
