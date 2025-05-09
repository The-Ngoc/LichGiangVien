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
    public class LopHocPhansController : Controller
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
        // GET: LopHocPhans
        public ActionResult Index()
        {
            var lopHocPhans = db.LopHocPhans.Include(l => l.GiangVien);
            return View(lopHocPhans.ToList());
        }

        // GET: LopHocPhans/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LopHocPhan lopHocPhan = db.LopHocPhans.Find(id);
            if (lopHocPhan == null)
            {
                return HttpNotFound();
            }
            return View(lopHocPhan);
        }

        // GET: LopHocPhans/Create
        public ActionResult Create()
        {
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV");
            return View();
        }

        // POST: LopHocPhans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLHP,TenMH,SoTinChi,SoTietMoiTuan,MaGV")] LopHocPhan lopHocPhan)
        {
            if (ModelState.IsValid)
            {
                db.LopHocPhans.Add(lopHocPhan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", lopHocPhan.MaGV);
            return View(lopHocPhan);
        }

        // GET: LopHocPhans/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LopHocPhan lopHocPhan = db.LopHocPhans.Find(id);
            if (lopHocPhan == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", lopHocPhan.MaGV);
            return View(lopHocPhan);
        }

        // POST: LopHocPhans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLHP,TenMH,SoTinChi,SoTietMoiTuan,MaGV")] LopHocPhan lopHocPhan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lopHocPhan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", lopHocPhan.MaGV);
            return View(lopHocPhan);
        }

        // GET: LopHocPhans/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LopHocPhan lopHocPhan = db.LopHocPhans.Find(id);
            if (lopHocPhan == null)
            {
                return HttpNotFound();
            }
            return View(lopHocPhan);
        }

        // POST: LopHocPhans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            LopHocPhan lopHocPhan = db.LopHocPhans.Find(id);
            db.LopHocPhans.Remove(lopHocPhan);
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
