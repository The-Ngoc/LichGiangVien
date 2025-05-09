using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using XepLichGiangVien.Models;

namespace XepLichGiangVien.Controllers
{
    public class LichDaysController : Controller
    {
        private AppDBContext db = new AppDBContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["MaVaiTro"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Home");
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: LichDays/XepLich
        public ActionResult XepLich()
        {
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            ViewBag.MaPhong = new SelectList(db.PhongHocs, "MaPhong", "TenPhong");

            int month = DateTime.Now.Month;
            ViewBag.KyHienTai = (month >= 1 && month <= 6) ? "Kỳ Lẻ" : "Kỳ Chẵn";

            return View();
        }

        // POST: LichDays/XepLich
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XepLich(string maLHP, string maPhong)
        {
            if ((int)Session["MaVaiTro"] != 0)
            {
                return RedirectToAction("Login", "Home");
            }

            var lop = db.LopHocPhans.Find(maLHP);
            if (lop == null || string.IsNullOrEmpty(maPhong))
            {
                TempData["Error"] = "Thông tin không hợp lệ.";
                return RedirectToAction("XepLich");
            }

            if (db.LichDays.Any(ld => ld.MaLHP == maLHP))
            {
                TempData["Error"] = "Lớp học phần này đã có lịch dạy!";
                return RedirectToAction("Index");
            }

            var lichMoi = GenerateLichDay(maLHP, maPhong);

            if (lichMoi != null)
            {
                db.LichDays.Add(lichMoi);
                db.SaveChanges();
                TempData["Message"] = "Xếp lịch thành công!";
            }
            else
            {
                if (lop.SoLuongSinhVien > db.PhongHocs.Find(maPhong)?.SoLuongChoNgoi)
                {
                    TempData["Error"] = "Phòng học không đủ chỗ cho số lượng sinh viên!";
                }
                else
                {
                    TempData["Error"] = "Không tìm được khung giờ phù hợp!";
                }
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetGiangViens(string maKhoa)
        {
            var giangViens = db.GiangViens
                .Where(g => g.MaKhoa == maKhoa)
                .Select(g => new { g.MaGV, g.TenGV })
                .ToList();

            return Json(giangViens, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLopHocPhans(string maGV)
        {
            var lhps = db.LopHocPhans
                .Where(l => l.MaGV == maGV)
                .Select(l => new { l.MaLHP, l.TenMH })
                .ToList();

            return Json(lhps, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            if ((int)Session["MaVaiTro"] != 0)
            {
                return RedirectToAction("Login", "Home");
            }

            var lich = db.LichDays
                .Include(l => l.LopHocPhan)
                .Include(l => l.PhongHoc)
                .ToList();

            return View(lich);
        }

        private LichDay GenerateLichDay(string maLHP, string maPhong)
        {
            var lop = db.LopHocPhans.Find(maLHP);
            if (lop == null) return null;

            var phong = db.PhongHocs.Find(maPhong);
            if (phong == null) return null;

            // Kiểm tra số lượng sinh viên
            if (lop.SoLuongSinhVien > phong.SoLuongChoNgoi)
            {
                return null;
            }

            string maGV = lop.MaGV;
            int soTiet = lop.SoTietMoiTuan;

            int month = DateTime.Now.Month;
            bool isKyLe = (month >= 7 && month <= 12);

            int tuanBatDau = isKyLe ? 3 : 24;
            int tuanKetThuc = isKyLe ? 18 : 42;

            LichDay bestSlot = null;
            int bestScore = int.MinValue;

            for (int thu = 2; thu <= 7; thu++)
            {
                for (int tietBD = 1; tietBD <= 10 - soTiet + 1; tietBD++)
                {
                    int tietKT = tietBD + soTiet - 1;

                    bool trungGV = db.LichDays.Any(ld =>
                        ld.LopHocPhan.MaGV == maGV &&
                        ld.Thu == thu &&
                        !(ld.TietKetThuc < tietBD || ld.TietBatDau > tietKT) &&
                        !(ld.TuanKetThuc < tuanBatDau || ld.TuanBatDau > tuanKetThuc)
                    );

                    bool trungPhong = db.LichDays.Any(ld =>
                        ld.MaPhong == maPhong &&
                        ld.Thu == thu &&
                        !(ld.TietKetThuc < tietBD || ld.TietBatDau > tietKT) &&
                        !(ld.TuanKetThuc < tuanBatDau || ld.TuanBatDau > tuanKetThuc)
                    );

                    if (!trungGV && !trungPhong)
                    {
                        int score = CalculateScore(thu, tietBD, maGV, tietKT);

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestSlot = new LichDay
                            {
                                MaLHP = maLHP,
                                MaPhong = maPhong,
                                Thu = thu,
                                TietBatDau = tietBD,
                                TietKetThuc = tietKT,
                                TuanBatDau = tuanBatDau,
                                TuanKetThuc = tuanKetThuc
                            };
                        }
                    }
                }
            }

            return bestSlot;
        }

        private int CalculateScore(int thu, int tietBD, string maGV, int tietKT)
        {
            int score = 0;

            if (tietBD <= 5) score += 5;
            if (tietBD == 1 || tietBD == 4) score += 2;
            if (thu == 2 || thu == 4 || thu == 6) score += 2;

            bool coTietLien = db.LichDays.Any(ld =>
                ld.LopHocPhan.MaGV == maGV &&
                ld.Thu == thu &&
                (ld.TietKetThuc + 1 == tietBD || ld.TietBatDau - 1 == tietKT)
            );
            if (coTietLien) score += 3;

            return score;
        }

        public ActionResult XemLich()
        {
            if (Session["MaVaiTro"] == null || (int)Session["MaVaiTro"] != 1)
            {
                return RedirectToAction("Login", "Home");
            }

            string maGV = Session["MaGV"] as string;

            var lich = db.LichDays
                .Where(ld => ld.LopHocPhan.MaGV == maGV)
                .Include(ld => ld.LopHocPhan)
                .Include(ld => ld.PhongHoc)
                .ToList();

            return View(lich);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LichDay lichDay = db.LichDays.Find(id);
            if (lichDay == null)
            {
                return HttpNotFound();
            }
            return View(lichDay);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LichDay lichDay = db.LichDays.Find(id);
            db.LichDays.Remove(lichDay);
            db.SaveChanges();
            TempData["Message"] = "Xóa lịch thành công!";
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
