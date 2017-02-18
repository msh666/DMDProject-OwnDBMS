using DMDProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMDProject.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorEntityProvider _provider = new DefaultPostgresProviderAuthor();
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
        public ActionResult Create(AuthorEntity author)
        {
            try {
                _provider.Insert(author);
            }
            catch(ArgumentNullException)
            {
                ModelState.AddModelError("", "Enter the Name.");
                return View(author);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(author);
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
            AuthorEntity ae = new AuthorEntity();
            ae.Name = result.ElementAt(0).Name;
            ae.Research_area = result.ElementAt(0).Research_area;
            ae.Institution = result.ElementAt(0).Institution;
            ae.Author_id = result.ElementAt(0).Author_id;
            return View(ae);
        }

        //POST: Edit
        [HttpPost]
        public ActionResult Edit(AuthorEntity author)
        {
            try { 
            _provider.Edit(author);
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("", "Enter the Name.");
                return View(author);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(author);
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

        //GET: RATE
        [Authorize]
        public ActionResult Rate()
        {
           var result = _provider.Rate();
            return View(result);
        }

        //GET: Search
        [Authorize]
        public ActionResult Search(string Name, string Research_area, string Institution)
        {
            var myobjects = _provider.Search(Name, Research_area, Institution);
            return View("Index", myobjects);
        }
    }
}