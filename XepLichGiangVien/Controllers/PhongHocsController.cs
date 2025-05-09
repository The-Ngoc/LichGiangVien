using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XepLichGiangVien.Models;

namespace XepLichGiangVien.Controllers
{
    public class PhongHocsController : Controller
    {
        private AppDBContext db = new AppDBContext();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["MaVaiTro"] == null || (int)Session["MaVaiTro"] != 0)
            {
                // Không phải GiaoVu thì chuyển về trang login
                filterContext.Result = RedirectToAction("Login", "Home");
            }
            base.OnActionExecuting(filterContext);
        }
        // GET: PhongHocs
        public ActionResult Index()
        {
            return View(db.PhongHocs.ToList());
        }

        // GET: PhongHocs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhongHoc phongHoc = db.PhongHocs.Find(id);
            if (phongHoc == null)
            {
                return HttpNotFound();
            }
            return View(phongHoc);
        }

        // GET: PhongHocs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PhongHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaPhong,TenPhong,SoLuongChoNgoi")] PhongHoc phongHoc)
        {
            if (ModelState.IsValid)
            {
                db.PhongHocs.Add(phongHoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phongHoc);
        }

        // GET: PhongHocs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhongHoc phongHoc = db.PhongHocs.Find(id);
            if (phongHoc == null)
            {
                return HttpNotFound();
            }
            return View(phongHoc);
        }

        // POST: PhongHocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaPhong,TenPhong,SoLuongChoNgoi")] PhongHoc phongHoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phongHoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phongHoc);
        }

        // GET: PhongHocs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhongHoc phongHoc = db.PhongHocs.Find(id);
            if (phongHoc == null)
            {
                return HttpNotFound();
            }
            return View(phongHoc);
        }

        // POST: PhongHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PhongHoc phongHoc = db.PhongHocs.Find(id);
            db.PhongHocs.Remove(phongHoc);
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
