using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using AgentHub.Entities.ViewModels;

namespace AgentHub.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProviderLogin()
        {
            return View();
        }
        
        public ActionResult ConfirmEmail(string userId, string token)
        {
            var actionToRedirect = "ConfirmError";

            if (Request != null && Request.Url != null)
            {
                using (var client = new HttpClient())
                {
                    var hostName = Request.Url.Host;
                    if (Request.Url.Port != 80)
                        hostName = hostName + ":" + Request.Url.Port;
                    hostName = string.Format("{0}://{1}", Request.Url.Scheme, hostName);

                    client.BaseAddress = new Uri(hostName);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var confirmEmailApi = string.Format("{0}/api/Account/ConfirmEmail?userId={1}&token={2}", hostName, userId, token);
                    var response = client.GetAsync(confirmEmailApi).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var confirmResult = response.Content.ReadAsAsync<ConfirmEmailBindingModel>().Result;
                        if (confirmResult.Succeeded)
                        {
                            actionToRedirect = "Index";
                        }                            
                    }
                }
            }

            return RedirectToAction(actionToRedirect);
        }

        public ActionResult ForgetPassword(string email, string code)
        {
            return View();
        }

        public ActionResult ResetPassword(string email, string code)
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ReviewProvider()
        {
            return View();
        }
    }
}