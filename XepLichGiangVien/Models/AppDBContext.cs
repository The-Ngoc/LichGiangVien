using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XepLichGiangVien.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext() : base("name=AppDbContext") { }

        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<Khoa> Khoas { get; set; }
        public DbSet<LichDay> LichDays { get; set; }
        public DbSet<LopHocPhan> LopHocPhans { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<PhongHoc> PhongHocs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }

    }
}
