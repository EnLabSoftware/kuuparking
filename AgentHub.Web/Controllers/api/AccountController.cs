using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AgentHub.Entities;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Service;
using AgentHub.Entities.Utilities;
using AgentHub.Entities.ViewModels;
using AgentHub.Service;
using AgentHub.Web.Identity;
using AgentHub.Web.Results;
using AutoMapper;
using Facebook;
using Microsoft.Owin.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ApplicationUser = AgentHub.Web.Identity.ApplicationUser;
using ApplicationUserManager = AgentHub.Web.Identity.ApplicationUserManager;

namespace AgentHub.Web.Controllers
{
    /// <summary>
    /// Account controller.
    /// </summary>
    [Authorize]
    [ApplicationAuthorization]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private readonly ICommonService _commonService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        /// <summary>
        /// Gets the user manager.
        /// </summary>
        /// <value>
        /// The user manager.
        /// </value>
        public ApplicationUserManager UserManager
        {
            get 
            {
                return _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? (_roleManager = Request.GetOwinContext().GetUserManager<ApplicationRoleManager>());
            }
            private set
            {
                _roleManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public ApplicationDbContext ApplicationDbContext
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationDbContext>();
            }
        }

        public AccountController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        /// <summary>
        /// Gets the user profile information.
        /// </summary>
        /// <returns></returns>
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public IHttpActionResult GetUserInfo()
        {
            try
            {
                var userLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
                if (userLogin == null)
                    return null;

                var user = GetUser(userLogin.Email, userLogin.Provider, userLogin.ProviderKey);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private ApplicationUser GetUser(string email, string provider = null, string providerKey = null)
        {
            ApplicationUser user = null;

            // Get user via UserId found in ApplicationUserLogin if email is not provided
            if (string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(provider) && !string.IsNullOrEmpty(providerKey))
            {
                user = (from u in ApplicationDbContext.Set<ApplicationUser>()
                    join login in ApplicationDbContext.Set<ApplicationUserLogin>() on u.Id equals login.UserId
                    where login.LoginProvider == provider && login.ProviderKey == providerKey
                    select u).FirstOrDefault();
            }
            else
            {
                // Find by email or else username
                user = UserManager.FindByEmail(email) ?? UserManager.FindByName(email);
            }
            if (user != null)
            {
                // Call to Common service to get user profile
                user.UserProfile = _commonService.GetUserProfile(email).Result;
            }
            return user;
        }

        /// <summary>
        /// Gets the user profile information.
        /// </summary>
        /// <returns></returns>
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UpdateUserProfile")]
        [HttpPost]
        public IHttpActionResult UpdateUserProfile(UserProfile userProfile)
        {
            try
            {
                //
                // Call to Common service to get user profile
                _commonService.UpdateUserProfile(userProfile);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Login with the specified username and password. Access token is returned upon successful login. 
        /// This access token is then used to call subsequent APIs.
        /// </summary>
        /// <param name="model">The user model containing username and password.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user != null)
                    {
                        // Another check to ensure user is a local user, not an external user
                        var userLogins = await UserManager.GetLoginsAsync(user.Id);
                        if (userLogins.Count == 0)
                        {
                            // Generate access token for front-end to access granted resources
                            user.AccessToken = GenerateAccessToken(user);
                            // 
                            // Track user login action
                            AuditService.AuditUserLogin(user.Id);

                            return Ok(user);
                        }
                    }
                }

                return GetErrorResult(new IdentityResult(StringTable.IncorrectUsernamePassword));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Confirms the user's email when registering a new accounnt
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="code">The confirmation token.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                code = code.Replace(" ", "+");

                var user = await UserManager.FindByIdAsync(userId);
                if (user.EmailConfirmed)
                    return Ok(StringTable.AccountIsAlreadyConfirmedEmail);
                
                var resultData = new ConfirmEmailBindingModel() { UserId = userId, EmailConfirmationToken = code };
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                resultData.Succeeded = result.Succeeded;
                if (resultData.Succeeded)
                {
                    //
                    // Signin the activated user
                    user.EmailConfirmed = true;
                    //
                    // Generate access token for client access
                    user.AccessToken = GenerateAccessToken(user);
                }

                return Ok(user);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = GetModelStateError();
                if (errorMessage.ToLower().Contains("is required"))
                    errorMessage = StringTable.ResetPasswordRequestIsExpired;

                return Ok(new { message = errorMessage });
            }
            var user = GetUser(model.Email);
            if (user == null)
            {
                return BadRequest(string.Format(StringTable.UserNotFoundByEmail, model.Email));
            }
            model.Code = model.Code.Replace(" ", "+");

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.NewPassword);
            if (!result.Succeeded)
            {
                var errorMessage = result.Errors.FirstOrDefault();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorMessage.Contains("Invalid token"))
                        errorMessage = StringTable.ResetPasswordRequestIsExpired;
                    else if (errorMessage.Contains("Passwords must be at least 6 characters") ||
                             errorMessage.Contains("lowercase ('a'-'z').") ||
                             errorMessage.Contains("uppercase ('A'-'Z')."))
                    {
                        errorMessage = StringTable.ValidPasswordPolicy;
                    }
                }

                return Ok(new {message = errorMessage});
            }

            return Ok(new {message = "success"});
        }

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public IHttpActionResult GetExternalLogin(string provider, string error = null)
        {
            try
            {
                var redirectUri = string.Empty;

                if (error != null)
                {
                    return BadRequest(Uri.EscapeDataString(error));
                }

                if (!User.Identity.IsAuthenticated)
                {
                    //
                    // Force user login in the login provider screen
                    return new ChallengeResult(provider, this);
                }

                var redirectUriValidationResult = ValidateClientAndRedirectUri(Request, ref redirectUri);

                if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
                {
                    return BadRequest(redirectUriValidationResult);
                }

                var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

                if (externalLogin == null)
                {
                    return InternalServerError();
                }

                if (externalLogin.Provider != provider)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    return new ChallengeResult(provider, this);
                }

                return Redirect(redirectUri);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [Route("CompleteOAuthSignIn", Name = "CompleteOAuthSignIn")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> CompleteOAuthSignIn()
        {
            try
            {
                var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
                if (string.IsNullOrWhiteSpace(externalLogin.Provider) || string.IsNullOrWhiteSpace(externalLogin.ExternalAccessToken))
                {
                    return BadRequest("Provider or external access token is not sent");
                }
                //
                // To ensure that this external access token is valid, and generated using our Facebook or Google application defined in our back-end API 
                var verifiedAccessToken = await VerifyExternalAccessToken(externalLogin.Provider, externalLogin.ExternalAccessToken);
                if (verifiedAccessToken == null)
                {
                    return BadRequest("Invalid provider or External access token");
                }
                //
                // Find if the external user is already registered in our local system or not
                var user = UserManager.FindAsync(new UserLoginInfo(externalLogin.Provider, externalLogin.ProviderKey)).Result;
                if (user != null)
                {
                    // To indicate the user is already registered with our local system and then sign the user in
                    user.HasLocalAccount = true;
                    //
                    // Generate access token for client access
                    user.AccessToken = GenerateAccessToken(user);
                }
                else
                {
                    // Query external user info from the selected provider and return to client to register this external user with our back-end API
                    user = GetExternalUserInfo(externalLogin.Provider, externalLogin.ExternalAccessToken).Result;
                }

                return Ok(user);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Registers the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = RegisterModelToApplicationUser(model);

                var result = await CreateUserAsync(user, null);

                if (!result.Succeeded) 
                    return GetErrorResult(result);                
                //
                // Ensure to get back user from database
                user = GetUser(user.Email);
                if (AppSettings.ByPassRegisterConfirmation)
                {
                    // Generate access token for client access
                    user.AccessToken = GenerateAccessToken(user);
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.Email))
                        await SendEmailConfirmRegistration(user);

                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        // TODO: Generate security code to send over SMS
                        await UserManager.SendSmsAsync(user.Id, "Mã kích hoạt tài khoản của bạn: 654321");
                    }                            
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task<IdentityResult> CreateUserAsync(ApplicationUser user, UserLoginInfo userLoginInfo)
        {
            // Create user first
            var result = await UserManager.CreateAsync(user, user.Password);
            if (!result.Succeeded)
                return result;
            // Create role if there's no existing one
            var roleName = user.UserProfileType.ToString();
            var role = RoleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName) {Id = Guid.NewGuid().ToString()};
                result = await RoleManager.CreateAsync(role);
                if (!result.Succeeded)
                    return result;
            }
            // Add user to the role
            var rolesForUser = UserManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                result = await UserManager.AddToRoleAsync(user.Id, role.Name);
            }
            user.UserRole = role;   // For use later to determine what role the user belongs to
            // Add the application user login if it's an external registration (register an external user)
            if (userLoginInfo != null)
            {
                result = await UserManager.AddLoginAsync(user.Id, userLoginInfo);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Get link to reset password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = GetUser(model.EmailAddress);
                if (user != null && string.IsNullOrEmpty(user.ExternalProviderName)) // Make sure to only allow reseting password of local users (not Google or Facebook users)
                {
                    var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var resetLink = string.Format("{0}://{1}/{2}?email={3}&code={4}", Request.RequestUri.Scheme, Request.RequestUri.Authority, AppSettings.ResetPasswordPageName, user.Email, code);

                    var fullName = (user.UserProfile != null ? CultureInfoHelper.GetFullName(user.UserProfile.FirstName, user.UserProfile.LastName, ApplicationKeyManager.ApplicationService.CultureInfoCode) : string.Empty);
                    var body = string.Format(StringTable.ForgotPasswordEmailConfirmationBody, fullName, resetLink); 

                    EmailHelper.SendEmail(model.EmailAddress, StringTable.ResetYourPasswordSubject, body, true);

                    return Ok(new { message = "sent" });
                }
                return Ok(new { message = StringTable.EmailIsNotFound });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Registers the external.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
                if (verifiedAccessToken == null)
                {
                    return BadRequest("Invalid Provider or External Access Token");
                }

                var externalLoginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    return InternalServerError();
                }

                var user = RegisterModelToApplicationUser(model);
                user.ExternalProviderName = model.Provider; // Ensure to store the external provider name to the auth.ApplicationUser table for reference later
                //user.Password = "Abc@123" + Guid.NewGuid().ToString(); // Random password to messup things so user can't get in by accident with their email when they registered as an external user

                var result = await CreateUserAsync(user, externalLoginInfo.Login);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                //
                // Generate access token for client access
                user.AccessToken = GenerateAccessToken(user);

                return Ok(user);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        #region Private helper methods

        private async Task SendEmailConfirmRegistration(ApplicationUser user)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = string.Format("{0}://{1}/{2}?userId={3}&code={4}", Url.Request.RequestUri.Scheme, Request.RequestUri.Authority, AppSettings.ConfirmPasswordPageName, user.Id, code);

            var fullName = (user.UserProfile != null ? CultureInfoHelper.GetFullName(user.UserProfile.FirstName, user.UserProfile.LastName, ApplicationKeyManager.ApplicationService.CultureInfoCode) : string.Empty);
            var body = string.Format(StringTable.AccountRegisterEmailConfirmationBody, fullName, callbackUrl);

            await UserManager.SendEmailAsync(user.Id, StringTable.ConfirmYourAccount, body);
        }

        private string GenerateAccessToken(ApplicationUser user, bool isPersistent = false)
        {
            var currentUtc = new SystemClock().UtcNow;
            var props = new AuthenticationProperties()
            {
                IsPersistent = isPersistent,
                IssuedUtc = currentUtc,
                ExpiresUtc = currentUtc.Add(TimeSpan.FromHours(AppSettings.AccessTokenExpirationInHours)),
            };

            var identity = User.Identity as ClaimsIdentity;
            if (identity != null && !identity.IsAuthenticated)
            {
                identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType);

                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                if (user.UserRole != null)
                    identity.AddClaim(new Claim("role", user.UserRole.Name));
                if (!string.IsNullOrEmpty(user.Provider))
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Provider));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.UserName));
            }
            var ticket = new AuthenticationTicket(identity, props);
            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            return accessToken;
        }

        /// <summary>
        /// Validates to ensure the client application (identified by the APIKey found in the Request) redirect to URL of that client and not of any other client application
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="redirectUriOutput">The redirect URI output.</param>
        /// <returns></returns>
        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {
            Uri redirectUri;

            var redirectUriString = GetQueryString(request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            var validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var apiKey = GetQueryString(request, "APIKey");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "APIKey is required";
            }

            if (ApplicationKeyManager.GetApplicationService(apiKey, request.RequestUri.Host + ";") == null)
            {
                return string.Format("The given URL is not allowed by APIKey '{0}' configuration.", apiKey);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;
        }

        private async Task<ApplicationUser> GetExternalUserInfo(string provider, string externalAccessToken)
        {
            ApplicationUser user = null;

            if (provider == "Facebook")
            {
                var facebookClient = new FacebookClient(externalAccessToken);
                dynamic userInfo = facebookClient.Get("me");
                if (userInfo != null)
                {
                    user = new ApplicationUser
                    {
                        Provider = provider,
                        ExternalAccessToken = externalAccessToken,
                        Email = userInfo.email,
                        FirstName = userInfo.first_name,
                        LastName = userInfo.last_name,
                        FullName = userInfo.name
                    };
                    if (string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.FullName))
                    {
                        var nameParts = user.FullName.Split(' ');
                        if (nameParts.Length > 1)
                        {
                            user.FirstName = nameParts[0];
                            user.LastName = nameParts[1];
                        }
                        else if (nameParts.Length > 0)
                            user.FirstName = nameParts[0];
                    }
                }
            }
            else if (provider == "Google")
            {
                var getUserInfoEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={0}", externalAccessToken);
                using (var client = new HttpClient())
                {
                    var uri = new Uri(getUserInfoEndPoint);
                    var response = client.GetAsync(uri);

                    var content = await response.Result.Content.ReadAsStringAsync();
                    dynamic userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    user = new ApplicationUser
                    {
                        Provider = provider,
                        ExternalAccessToken = externalAccessToken,
                        Email = userInfo.email,
                        FirstName = userInfo.given_name,
                        LastName = userInfo.family_name
                    };
                    user.FullName = CultureInfoHelper.GetFullName(user.FirstName, user.LastName,
                        ApplicationKeyManager.ApplicationService.CultureInfoCode);
                }
            }

            return user;
        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, AppSettings.FacebookAppToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            using (var client = new HttpClient())
            {
                var uri = new Uri(verifyTokenEndPoint);
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    parsedToken = new ParsedExternalAccessToken();

                    if (provider.Equals("facebook", StringComparison.CurrentCultureIgnoreCase))
                    {
                        parsedToken.app_id = jObj["data"]["app_id"];

                        if (!string.Equals(Startup.FacebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                        {
                            return null;
                        }                        
                    }
                    else if (provider.Equals("google", StringComparison.CurrentCultureIgnoreCase))
                    {
                        parsedToken.app_id = jObj["audience"];

                        if (!string.Equals(Startup.GoogleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                        {
                            return null;
                        }
                    }
                }
            }

            return parsedToken;
        }

        protected ApplicationUser RegisterModelToApplicationUser(RegisterBindingModel model)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<RegisterBindingModel, ApplicationUser>());
            var mapper = config.CreateMapper();
            var user = mapper.Map<ApplicationUser>(model);
            user.CreatedDate = DateTime.Now;
            user.ModifiedDate = user.CreatedDate;
            // Use email or phone number as
            user.UserName = (!string.IsNullOrEmpty(user.Email) ? user.Email : user.PhoneNumber);

            user.InternationalPhoneNumber = user.PhoneNumber;
            if (!string.IsNullOrEmpty(user.PhoneNumber) && !user.PhoneNumber.StartsWith("+"))
            {
                // Format phone number to include code region
                var countryRepositoryAsync = ObjectFactory.GetInstance<IRepositoryAsync<Country>>();
                var country = countryRepositoryAsync.Find(user.CountryId);
                if (country != null)
                {
                    user.InternationalPhoneNumber = string.Format("+{0}{1}", country.PhoneCallCode, user.PhoneNumber);
                }
            }

            return user;
        }

        /// <summary>
        /// Gets the error result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    var errorMessage = "";
                    foreach (var error in result.Errors)
                    {
                        if (!string.IsNullOrEmpty(errorMessage))
                            errorMessage += Environment.NewLine;

                        errorMessage += error;
                    }
                    return BadRequest(errorMessage);
                }
            }

            return BadRequest();
        }

        #endregion

        public class ExternalLoginData
        {
            public string Provider { get; set; }
            public string ProviderKey { get; set; }
            public bool HasLocalAccount { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string ExternalAccessToken { get; set; }
            public string AccessToken { get; set; }

            /// <summary>
            /// Gets the claims.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Claim> GetClaims()
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, Provider)
                };

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, Provider));
                }

                return claims;
            }

            /// <summary>
            /// Froms the identity.
            /// </summary>
            /// <param name="identity">The identity.</param>
            /// <returns></returns>
            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                var providerEmailClaim = identity.FindFirst(ClaimTypes.Email);

                if (providerKeyClaim == null && providerEmailClaim == null)
                    return null;

                return new ExternalLoginData
                {
                    Provider = (providerKeyClaim != null ? providerKeyClaim.Issuer : string.Empty),
                    ProviderKey = (providerKeyClaim != null && providerKeyClaim.Value != null ? providerKeyClaim.Value : string.Empty),
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    Email = (providerEmailClaim != null ? providerEmailClaim.Value : string.Empty),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken")
                };
            }
        }

        public class ParsedExternalAccessToken
        {
            public string user_id { get; set; }
            public string app_id { get; set; }
        }

        #endregion
    }
}
