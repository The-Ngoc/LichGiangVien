using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XepLichGiangVien.Models;

namespace XepLichGiangVien.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string tenDangNhap, string matKhau)
        {
            AppDBContext appContext = new AppDBContext();

            var taiKhoan = appContext.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == tenDangNhap && t.MatKhau == matKhau);
            if (taiKhoan != null)
            {
                Session["TenDangNhap"] = taiKhoan.TenDangNhap;
                Session["MaVaiTro"] = taiKhoan.MaVaiTro;
                Session["MaGV"] = taiKhoan.MaGV;

                if (taiKhoan.MaVaiTro == 0)
                {
                    return RedirectToAction("GiaoVu", "Home");
                }
                else if (taiKhoan.MaVaiTro == 1)
                {
                    return RedirectToAction("GiangVien", "Home");
                }
            }

            else
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            return View();
        }

        public ActionResult GiaoVu()
        {
            if (Session["MaVaiTro"] == null || (int)Session["MaVaiTro"] != 0)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult GiangVien()
        {
            if (Session["MaVaiTro"] == null || (int)Session["MaVaiTro"] != 1)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}