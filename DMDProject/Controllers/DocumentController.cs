using DMDProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMDProject.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentEntityProvider _provider = new DefaultPostgresProviderDocument();
        // GET: Author
        [Authorize]
        public ActionResult Index()
        {
            var myobjects = _provider.GetAllEntities();
            return View(myobjects);
        }

        // GET: /Test/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Test/Create
        [HttpPost]
        public ActionResult Create(DocumentEntity document)
        {
            try {
                _provider.Insert(document);
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("", "Enter the Title.");
                return View(document);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(document);
            }
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }

        // GET: Delete
        [Authorize]
        public ActionResult Delete(string id)
        {
            var result = _provider.Find(id);
            return View(result);
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            _provider.Delete(id);
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }

        // GET: Edit
        [Authorize]
        public ActionResult Edit(string id)
        {
            var result = _provider.Find(id);
            DocumentEntity de = new DocumentEntity();
            de.Title = result.ElementAt(0).Title;
            de.Document_type = result.ElementAt(0).Document_type;
            de.Year = result.ElementAt(0).Year;
            de.Volume = result.ElementAt(0).Volume;
            de.Url = result.ElementAt(0).Url;
            de.Ee = result.ElementAt(0).Ee;
            de.Keywords = result.ElementAt(0).Keywords;
            de.Source_id = result.ElementAt(0).Source_id;
            de.Document_id = result.ElementAt(0).Document_id;
            return View(de);
        }

        //POST: Edit
        [HttpPost]
        public ActionResult Edit(DocumentEntity document)
        {
            try {
                _provider.Edit(document);
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("", "Enter the Title.");
                return View(document);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(document);
            }
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }

        //GET: Details
        [Authorize]
        public ActionResult Details(string id)
        {
            var details = _provider.Details(id);
            return View(details);
        }

        //GET: Search
        [Authorize]
        public ActionResult Search(string Title, string Year, string Document_type, string Keyword)
        {
            var myobjects = _provider.Search(Title, Document_type, Year, Keyword);
            return View("Index", myobjects);
        }
    }
}