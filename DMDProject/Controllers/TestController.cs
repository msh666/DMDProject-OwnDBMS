using DMDProject.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMDProject.Controllers
{
    public class TestController : Controller
    {
        private readonly ISourceEntityProvider _provider = new DefaultPostgresProvider("Server=127.0.0.1;Port=5432;User Id=postgres;Password=123456;Database=postgres;");
        // GET: Test
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
        public ActionResult Create(SourceEntity source)
        {
            try {
                _provider.Insert(source);
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("", "Fill all fields.");
                return View(source);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(source);
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
            SourceEntity se = new SourceEntity();
            se.Source_id = result.ElementAt(0).Source_id;
            se.Title = result.ElementAt(0).Title;
            se.Type = result.ElementAt(0).Type;
            return View(se);
        }

        //POST: Edit
        [HttpPost]
        public ActionResult Edit(SourceEntity source)
        {
            try { 
            _provider.Edit(source);
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("", "Fill all fields.");
                return View(source);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Wrong format.");
                return View(source);
            }
            var myobjects = _provider.GetAllEntities();
            return View("Index", myobjects);
        }
    }
}