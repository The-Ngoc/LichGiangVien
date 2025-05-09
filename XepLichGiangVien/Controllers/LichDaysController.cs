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

            // Gửi kỳ hiện tại sang View
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
                TempData["Error"] = "Không tìm được khung giờ phù hợp!";
            }

            return RedirectToAction("Index");
        }

        // AJAX: Lấy danh sách Giảng Viên theo Khoa
        public JsonResult GetGiangViens(string maKhoa)
        {
            var giangViens = db.GiangViens
                .Where(g => g.MaKhoa == maKhoa)
                .Select(g => new { g.MaGV, g.TenGV })
                .ToList();

            return Json(giangViens, JsonRequestBehavior.AllowGet);
        }

        // AJAX: Lấy danh sách Lớp Học Phần theo Giảng Viên
        public JsonResult GetLopHocPhans(string maGV)
        {
            var lhps = db.LopHocPhans
                .Where(l => l.MaGV == maGV)
                .Select(l => new { l.MaLHP, l.TenMH })
                .ToList();

            return Json(lhps, JsonRequestBehavior.AllowGet);
        }

        // GET: LichDays
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

        // ======= THUẬT TOÁN XẾP LỊCH =======
        private LichDay GenerateLichDay(string maLHP, string maPhong)
        {
            var lop = db.LopHocPhans.Find(maLHP);
            if (lop == null) return null;

            var phong = db.PhongHocs.Find(maPhong);
            if (phong == null) return null;

            string maGV = lop.MaGV;
            int soTiet = lop.SoTietMoiTuan;

            // Xác định kỳ hiện tại
            int month = DateTime.Now.Month;
            bool isKyLe = (month >= 7 && month <= 12);

            int tuanBatDau = isKyLe ? 3 : 24;
            int tuanKetThuc = isKyLe ? 18 : 42;

            LichDay bestSlot = null;
            int bestScore = int.MinValue;

            for (int thu = 2; thu <= 7; thu++) // Thứ 2 -> Thứ 7
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

            // Ưu tiên buổi sáng
            if (tietBD <= 5)
                score += 5;

            // Ưu tiên tiết đầu buổi
            if (tietBD == 1 || tietBD == 4)
                score += 2;

            // Ưu tiên thứ 2, 4, 6
            if (thu == 2 || thu == 4 || thu == 6)
                score += 2;

            // Ưu tiên lịch liền tiết
            bool coTietLien = db.LichDays.Any(ld =>
                ld.LopHocPhan.MaGV == maGV &&
                ld.Thu == thu &&
                (ld.TietKetThuc + 1 == tietBD || ld.TietBatDau - 1 == tietKT)
            );
            if (coTietLien)
                score += 3;

            return score;
        }

        // GET: LichDays/XemLich
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
