using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace SaveFileFromUrlWithWebclient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string message)
        {
            ViewBag.message = message;
            return View();
        }

        // GET: 
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string url)
        {
            if (ModelState.IsValid) { 
                try
                {
                    if (url == null || url.Length == 0)
                    {
                        ModelState.AddModelError("url", "No URI is Specifyed");                        
                    }

                    WebClient webClient = new WebClient();
                    using (webClient)
                    {
                        string mimetype = GetMimeType(url);

                        if (GetApprovedMimetypes().Contains(mimetype))
                        {
                            string path = GetApp_DataPath();
                            string filename = GetFilename(url);
                            string pathAndFilename = Path.Combine(path,filename);
                            webClient.DownloadFile(url, pathAndFilename);
                            return RedirectToAction("Index", new {message= String.Format("The file \"{0}\" is uploaded to App_Data", filename) });
                        }
                        else {
                            ModelState.AddModelError("url", String.Format("Mimetype: {0} not allowed", mimetype));                            
                        }
                    }                    
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("url", exception.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// is url on list of approved mime types?
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool isMimetypeApproved(string url)
        {
            string mimetype = GetMimeType(url);
            List<string> allowedMimetypes = GetApprovedMimetypes();
            if (allowedMimetypes.Contains(mimetype)){
                return true;
            }
            else {
                return false;
            }          
        }

        /// <summary>
        /// list of approved mime types
        /// </summary>
        /// <returns></returns>
        private static List<string> GetApprovedMimetypes()
        {
            return new List<string>() { "application/pdf","image/jpeg", "image/png", "image/gif" };          
        }


        private string GetApp_DataPath()
        {
            string dirToSaveFiles = "~/App_Data";
            if (!Directory.Exists(Server.MapPath(dirToSaveFiles))) { 
                Directory.CreateDirectory(Server.MapPath(dirToSaveFiles));
            };
            return Server.MapPath(dirToSaveFiles);
        }

        private static string GetMimeType(string url)
        {
            string result = "";
            WebRequest webRequest = WebRequest.Create(url);
            WebResponse webResponse = webRequest.GetResponse();
            result = webResponse.ContentType;
            webResponse.Close();
            return result;
        }

        private static string GetFilename(string url)
        {
            string result = "";
            var uri = new Uri(url);
            var fullName = uri.LocalPath.TrimStart('/');
            result = Path.GetFileName(fullName);
            return result;
        }

    }
}