using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace MyCyberQuiz.UI.Services.Handlers
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public JwtAuthorizationMessageHandler(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //try
            //{
                // Hämta krypterad token från sessionen
                var tokenResult = await _sessionStorage.GetAsync<string>("authToken"); //

                if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                {
                    // Klistra in token i headern
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
                }
            //}
            //catch
            //{
            //    // Viktigt: I Blazor Server kan man inte använda session storage under "prerendering" 
            //    // (innan sidan laddats klart). Try/catch fångar kraschen och låter anropet gå vidare.
            //}

            // Skicka iväg anropet till API:et
            return await base.SendAsync(request, cancellationToken);
        }
    }
}