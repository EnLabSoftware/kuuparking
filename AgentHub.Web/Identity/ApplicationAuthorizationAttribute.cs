using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using AgentHub.Entities.Utilities;

namespace AgentHub.Web.Identity
{
    public class ApplicationAuthorizationAttribute : Attribute, IAuthenticationFilter
    {
        public const string AuthenticationScheme = "APIKey";

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;

            var apiKeyParameter = request.Headers.FirstOrDefault(_ => _.Key.Equals(AuthenticationScheme, StringComparison.CurrentCultureIgnoreCase));

            if (apiKeyParameter.Value != null)
            {
                //
                // Check if the request is allowed with by the APIKey and hostname from the request
                var apiKey = apiKeyParameter.Value.FirstOrDefault();
                var fromHostName = request.RequestUri.Host + ";";
                var applicationService = ApplicationKeyManager.GetApplicationService(apiKey, fromHostName);
                if (applicationService == null)
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            else
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }

        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {

            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }

        }

        private static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync();
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }
    }

    public class ResultWithChallenge : IHttpActionResult
    {
        private readonly IHttpActionResult _next;

        public ResultWithChallenge(IHttpActionResult next)
        {
            _next = next;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await _next.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(ApplicationAuthorizationAttribute.AuthenticationScheme));
            }

            return response;
        }
    }
}