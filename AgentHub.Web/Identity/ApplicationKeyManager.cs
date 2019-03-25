using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AgentHub.Entities.Models.Application;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Utilities;

namespace AgentHub.Web.Identity
{
    public class ApplicationKeyManager
    {
        private static IList<Application> _applicationKeys = null;
        private static ApplicationService _applicationService;

        internal static IList<Application> ApplicationKeys 
        {
            get
            {
                if (_applicationKeys == null || !AppSettings.CacheAPIKey)
                {
                    var applicationRepositoryAsync = ObjectFactory.GetInstance<IRepositoryAsync<Application>>();
                    _applicationKeys = applicationRepositoryAsync.Queryable().Include(_=>_.ApplicationServices).ToList();
                }

                return _applicationKeys;
            }
        }

        public static ApplicationService ApplicationService
        {
            get
            {
                return _applicationService;
            }
        }

        /// <summary>
        /// Gets the application key object based on the access token. The access token
        /// </summary>
        /// <param name="apiKey">The access token.</param>
        /// <returns></returns>
        internal static ApplicationService GetApplicationService(string apiKey, string fromHostName)
        {
            _applicationService = null;
            try
            {
                if (string.IsNullOrEmpty(apiKey))
                    return null;

                // Validate if the API key is valid
                var keyParts = apiKey.Split(':');
                if (keyParts.Length == 2)
                {
                    var appKey = keyParts[0];
                    var serviceKey = keyParts[1];

                    var applicationKey =
                        ApplicationKeys.FirstOrDefault(
                            _ =>
                                _.ApplicationKey == appKey &&
                                _.ApplicationServices.FirstOrDefault(service => service.ServiceKey == serviceKey) != null);
                    if (applicationKey != null)
                    {
                        _applicationService = applicationKey.ApplicationServices.FirstOrDefault(_ => _.ServiceKey == serviceKey && _.AllowedDomains.Contains(fromHostName));
                    }
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }

            return _applicationService;
        }
    }
}