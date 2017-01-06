using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NewEnglandBeerMapApplication.Models;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

namespace NewEnglandBeerMapApplication.Controllers
{
    [Authorize]
    public class BreweryEntryController : Controller
    {
        private NewEnglandBeerMapApplicationContext db = new NewEnglandBeerMapApplicationContext();
        string geocodeAPItoken = WebConfigurationManager.AppSettings["GoogleGeocodingAPIKey"];

        // GET: BreweryEntry
        public ActionResult Index()
        {
            return View(db.CraftBreweries.ToList());
        }

        [AllowAnonymous]
        public JsonResult GetGeoCode(int? id)
        {
            
            CraftBreweries craftBrewery = db.CraftBreweries.Find(id); 
            
            if (!string.IsNullOrEmpty(craftBrewery.BreweryAddress))
            {
                string address = craftBrewery.BreweryAddress + '+' + craftBrewery.BreweryCity + '+' + craftBrewery.BreweryState + '+' + craftBrewery.BreweryZipCode;
                string requestUrl = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false&key={1}", address, geocodeAPItoken);
                WebRequest request = WebRequest.Create(requestUrl);
                WebResponse response = request.GetResponse();                
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                return Json(responseFromServer, "text/x-json", JsonRequestBehavior.AllowGet);
                
            }
            else
            {                
                throw new ArgumentException("Brewery Address cannot be an empty", "craftBrewery.BreweryAddress");                
            }                      
        }

        [AllowAnonymous]
        public void GetGeoCodeAll()
        {
            foreach (var craftBrewery in db.CraftBreweries)
            {
                if (craftBrewery.latitude == 0)
                {
                    var ApiResponse = GetGeoCode(craftBrewery.Id);
                    var dataObject = ApiResponse.Data;
                    var convertedResponse = Convert.ToString(dataObject);
                    JToken parsedToken = JObject.Parse(convertedResponse);
                    var recvdLat = (decimal)parsedToken["results"][0]["geometry"]["location"]["lat"];
                    var recvdLong = (decimal)parsedToken["results"][0]["geometry"]["location"]["lng"];                    
                    craftBrewery.latitude = recvdLat;
                    craftBrewery.longitude = recvdLong;
                    //db.Entry(craftBrewery).State = EntityState.Modified;
                    

                }                
            }
            db.SaveChanges();
        }

        [AllowAnonymous]
        public void UpdateLatLong(CraftBreweries craftBrewery, int ? id)
        {
            var ApiResponse = GetGeoCode(id);
            var dataObject = ApiResponse.Data;
            var convertedResponse = Convert.ToString(dataObject);
            JToken parsedToken = JObject.Parse(convertedResponse);
            var recvdLat = (decimal)parsedToken["results"][0]["geometry"]["location"]["lat"];
            var recvdLong = (decimal)parsedToken["results"][0]["geometry"]["location"]["lng"];
            craftBrewery = db.CraftBreweries.Find(id);
            craftBrewery.latitude = recvdLat;
            craftBrewery.longitude = recvdLong;
            //db.Entry(craftBrewery).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: BreweryEntry/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CraftBreweries craftBreweries = db.CraftBreweries.Find(id);
            if (craftBreweries == null)
            {
                return HttpNotFound();
            }
            return View(craftBreweries);
        }

        // GET: BreweryEntry/Create
        public ActionResult Create()
        {
            List<SelectListItem> statelist = new List<SelectListItem>();
            statelist.Add(
                new SelectListItem { Text = "Connecticut", Value = "CT" }
            );
            statelist.Add(
                new SelectListItem { Text = "Maine", Value = "ME" }
            );
            statelist.Add(
                new SelectListItem { Text = "Massachusetts", Value = "MA" }
            );
            statelist.Add(
                new SelectListItem { Text = "New Hampshire", Value = "NH" }
            );
            statelist.Add(
                new SelectListItem { Text = "Rhode Island", Value = "RI" }
            );
            statelist.Add(
                new SelectListItem { Text = "Vermont", Value = "VT" }
            );

            ViewBag.BreweryState = statelist;
            return View();
        }

        // POST: BreweryEntry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BreweryName,BreweryAddress,BreweryCity,BreweryState,BreweryZipCode,latitude,longitude,BreweryWebsite")] CraftBreweries craftBreweries)
        {
            if (ModelState.IsValid)
            {
                db.CraftBreweries.Add(craftBreweries);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(craftBreweries);
        }

        // GET: BreweryEntry/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CraftBreweries craftBreweries = db.CraftBreweries.Find(id);
            if (craftBreweries == null)
            {
                return HttpNotFound();
            }
            return View(craftBreweries);
        }

        // POST: BreweryEntry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BreweryName,BreweryAddress,BreweryCity,BreweryState,BreweryZipCode,latitude,longitude,BreweryWebsite")] CraftBreweries craftBreweries)
        {
            if (ModelState.IsValid)
            {
                db.Entry(craftBreweries).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(craftBreweries);
        }

        // GET: BreweryEntry/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CraftBreweries craftBreweries = db.CraftBreweries.Find(id);
            if (craftBreweries == null)
            {
                return HttpNotFound();
            }
            return View(craftBreweries);
        }

        // POST: BreweryEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CraftBreweries craftBreweries = db.CraftBreweries.Find(id);
            db.CraftBreweries.Remove(craftBreweries);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
