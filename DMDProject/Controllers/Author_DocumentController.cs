using DMDProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMDProject.Controllers
{
    public class Author_DocumentController : Controller
    {
        private readonly IAuthor_DocumentEntityProvider _provider = new DefaultPostgresProviderAuthor_Document();
        // GET: Author_Document
        [Authorize]
        public ActionResult Index()
        {
            var myobjects = _provider.GetAllEntities();
            return View(myobjects);
        }

        // GET: Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        public ActionResult Create(Author_DocumentEntity a_d)
        {
            try
            {
                _provider.Insert(a_d);
            }
            catch (InvalidCastException)
            {
                ModelState.AddModelError("", "Fill all fields.");
                return View(a_d);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(a_d);
            }
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }

        // GET: Delete
        [Authorize]
        public ActionResult Delete(string a_d_id)
        {
            var result = _provider.Find(a_d_id);
            return View(result);
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string a_d_id)
        {
            _provider.Delete(a_d_id);
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }

        // GET: Edit
        [Authorize]
        public ActionResult Edit(string a_d_id)
        {
            var result = _provider.Find(a_d_id);
            Author_DocumentEntity ad = new Author_DocumentEntity();
            ad.A_D_id = result.ElementAt(0).A_D_id;
            ad.Author_id = result.ElementAt(0).Author_id;
            ad.Document_id = result.ElementAt(0).Document_id;
            return View(ad);
        }

        //POST: Edit
        [HttpPost]
        public ActionResult Edit(Author_DocumentEntity a_d)
        {
            try
            {
                var oldaid = Request.Params["author"];
                var olddid = Request.Params["document"];
                //_provider.Edit(a_d, oldaid, olddid);
            }
            catch (InvalidCastException)
            {
                ModelState.AddModelError("", "Fill all fields.");
                return View(a_d);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(a_d);
            }
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }

        //GET: Details
        [Authorize]
        public ActionResult Details(string a_d_id)
        {
            var details = _provider.Details(a_d_id);
            return View(details);
        }
    }
}