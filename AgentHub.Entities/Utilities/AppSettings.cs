using System;
using System.Configuration;

namespace AgentHub.Entities.Utilities
{
    public static class AppSettings
    {
        public static string IPAPIService 
        {
            get
            {
                return ConfigurationManager.AppSettings["IPAPIService"];
            }
        }

        public static string GoogleMapGeocodeAPIService
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleMapGeocodeAPIService"];
            }
        }

        public static string SmtpServer
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpServer"];
            }
        }

        public static string SmtpPort
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpPort"];
            }
        }

        public static string SmtpEnableSsl
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpEnableSsl"];
            }
        }

        public static string WebMasterEmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["WebMasterEmailAddress"];
            }
        }

        public static string WebMasterEmailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["WebMasterEmailPassword"];
            }
        }

        public static double AccessTokenExpirationInHours
        {
            get
            {
                var value = ConfigurationManager.AppSettings["AccessTokenExpirationInHours"].ToDouble();
                if (Math.Abs(value) <= 0)
                    value = 24;

                return value;
            }
        }

        public static string TwilioAccountSid
        {
            get
            {
                return ConfigurationManager.AppSettings["TwilioAccountSid"];
            }
        }

        public static string TwilioAuthToken
        {
            get
            {
                return ConfigurationManager.AppSettings["TwilioAuthToken"];
            }
        }
        
        public static string TwilioSmsAccountFrom
        {
            get
            {
                return ConfigurationManager.AppSettings["TwilioSmsAccountFrom"];
            }
        }
                
        public static int VietNamCountryId
        {
            get
            {
                var value = ConfigurationManager.AppSettings["VietNamCountryId"].ToInt();
                if (value == 0)
                    value = 24;

                return value;
            }
        }

        public static int MaxLogArchiveFilesInDays
        {
            get
            {
                var value = ConfigurationManager.AppSettings["MaxLogArchiveFilesInDays"].ToInt();
                if (value == 0)
                    value = 30;

                return value;
            }
        }

        public static bool ByPassRegisterConfirmation
        {
            get
            {
                return ConfigurationManager.AppSettings["ByPassRegisterConfirmation"].ToBool();
            }
        }
        
        public static bool AllowInsecureHttp
        {
            get
            {
                return ConfigurationManager.AppSettings["AllowInsecureHttp"].ToBool();
            }
        }

        public static string GoogleClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleClientId"];
            }
        }

        public static string GoogleClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleClientSecret"];
            }
        }

        public static string FacebookAppId
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookAppId"];
            }
        }

        public static string FacebookAppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookAppSecret"];
            }
        }

        public static string FacebookAppToken
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookAppToken"];
            }
        }

        public static bool CacheAPIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["CacheAPIKey"].ToBool();
            }
        }

        public static bool CachePageItems
        {
            get
            {
                return ConfigurationManager.AppSettings["CachePageItems"].ToBool();
            }
        }

        public static string ResetPasswordPageName
        {
            get
            {
                return ConfigurationManager.AppSettings["ResetPasswordPageName"];
            }
        }

        public static string ConfirmPasswordPageName
        {
            get
            {
                return ConfigurationManager.AppSettings["ConfirmPasswordPageName"];
            }
        }

        public static int ProviderThumbnailImageWidth
        {
            get
            {
                return ConfigurationManager.AppSettings["ProviderThumbnailImageWidth"].ToInt();
            }
        }

        public static int ProviderImageWidth
        {
            get
            {
                return ConfigurationManager.AppSettings["ProviderImageWidth"].ToInt();
            }
        }

        public static int SearchSlotsRadius
        {
            get
            {
                return ConfigurationManager.AppSettings["SearchSlotsRadius"].ToInt();
            }
        }
        
        
    }
}
