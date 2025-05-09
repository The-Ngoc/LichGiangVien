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
    public class TaiKhoansController : Controller
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
        // GET: TaiKhoans
        public ActionResult Index()
        {
            var taiKhoans = db.TaiKhoans.Include(t => t.GiangVien).Include(t => t.VaiTro);
            return View(taiKhoans.ToList());
        }

        // GET: TaiKhoans/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }

        // GET: TaiKhoans/Create
        public ActionResult Create()
        {
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV");
            ViewBag.MaVaiTro = new SelectList(db.VaiTros, "MaVaiTro", "TenVaiTro");
            return View();
        }

        // POST: TaiKhoans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenDangNhap,MatKhau,MaGV,MaVaiTro")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                db.TaiKhoans.Add(taiKhoan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", taiKhoan.MaGV);
            ViewBag.MaVaiTro = new SelectList(db.VaiTros, "MaVaiTro", "TenVaiTro", taiKhoan.MaVaiTro);
            return View(taiKhoan);
        }

        // GET: TaiKhoans/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", taiKhoan.MaGV);
            ViewBag.MaVaiTro = new SelectList(db.VaiTros, "MaVaiTro", "TenVaiTro", taiKhoan.MaVaiTro);
            return View(taiKhoan);
        }

        // POST: TaiKhoans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TenDangNhap,MatKhau,MaGV,MaVaiTro")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taiKhoan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaGV = new SelectList(db.GiangViens, "MaGV", "TenGV", taiKhoan.MaGV);
            ViewBag.MaVaiTro = new SelectList(db.VaiTros, "MaVaiTro", "TenVaiTro", taiKhoan.MaVaiTro);
            return View(taiKhoan);
        }

        // GET: TaiKhoans/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }

        // POST: TaiKhoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            db.TaiKhoans.Remove(taiKhoan);
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
