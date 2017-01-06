using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NewEnglandBeerMapApplication.Models;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NewEnglandBeerMapApplication.Controllers
{
    public class HomeController : Controller
    {
        private NewEnglandBeerMapApplicationContext db = new NewEnglandBeerMapApplicationContext();

        // class to collect server response after contact form submit
        public class formResponse
        {
            public string gCaptchaResponse { get; set; }
            public string submitMessage { get; set; }
            public Exception submitErrors { get; set; }
            public bool isMessageSent { get; set; }
        }

        // GET: /Map/
        public ActionResult Index()
        {
            return View("Index","~/Views/Shared/_MapLayout.cshtml", new ContactForm());
        }

        // Google reCAPTCHA Response class
        public class CaptchaResponse
        {
            [JsonProperty("success")]
            public string Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

        formResponse formResponseObject = new formResponse(); // create instance of formResponse class

        public bool ValidateCaptcha(ContactForm contactForm)
        {
            
            //secret that was generated in key value pair
            string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];

            var client = new System.Net.WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
                secret, contactForm.gRecaptchaResponse)); // verify captcha response

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse> (reply); // put response into instance of CaptchaResponse class

            //when captcha response is false check for the error message
            if (captchaResponse.Success == "false")
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return true;

                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        formResponseObject.gCaptchaResponse = "The secret parameter is missing.";
                        break;
                    case ("invalid-input-secret"):
                        formResponseObject.gCaptchaResponse = "The secret parameter is invalid or malformed.";
                        break;
                    case ("missing-input-response"):
                        formResponseObject.gCaptchaResponse = "The response parameter is missing.";
                        break;
                    case ("invalid-input-response"):
                        formResponseObject.gCaptchaResponse = "The response parameter is invalid or malformed.";
                        break;
                    default:
                        formResponseObject.gCaptchaResponse = "Error occurred. Please try again";
                        break;
                }               
                return false;
            }
            else
            {
                formResponseObject.gCaptchaResponse = "Valid";
                return true;
            }             
        }

        public class EmailService
        {
            //Asynchronous method to send email using an instance of the ContactForm class as a parameter
            public async static Task SendContactForm(ContactForm contactForm)
            {
                //Constructing the email
                string body = "<p>The following is a message from {0} {1} ({2}):</p><p>{3}</p>";
               
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress("mdailey77@gmail.com"));
                message.Subject = "From NE Craft Brewery App - Contact Form";
                message.Body = string.Format(body, contactForm.contactFirstName, contactForm.contactLastName, contactForm.contactEmail, contactForm.contactComments);
                message.IsBodyHtml = true;

                //Attempting to send the email

                using (SmtpClient smtpClient = new SmtpClient())
                {
                    await smtpClient.SendMailAsync(message);
                }
            }
        }

        [HttpPost]
        public async Task<Object> Submit(ContactForm contactForm) // method called from form submit AJAX call
        {
            formResponseObject.isMessageSent = true;
            formResponseObject.submitMessage = "<strong>Success!</strong> Your message has been sent.";

            if (ModelState.IsValid && ValidateCaptcha(contactForm))
            {
                try
                {
                    await EmailService.SendContactForm(contactForm);
                }
                catch (Exception ex)
                {
                    formResponseObject.submitErrors = ex;
                    formResponseObject.isMessageSent = false;
                    formResponseObject.submitMessage = "<strong>Error!</strong> An error has occurred and your message has not been sent. Please try again later.";
                }
            }
            else
            {
                formResponseObject.isMessageSent = false;
                formResponseObject.submitMessage = "<strong>Error!</strong> An error has occurred and your message has not been sent. Please try again later.";
            }
            return JsonConvert.SerializeObject(formResponseObject);
        }

        // Get list of all breweries from database to populate Google map
        public JsonResult GetAllMarkers()
        {
            using (db)
            {
                var v = db.CraftBreweries.OrderBy(a => a.BreweryState).ToList();
                return Json(new { Data = v }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
