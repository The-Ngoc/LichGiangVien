
namespace XepLichGiangVien.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GiangVien
    {

        [Key]
        public string MaGV { get; set; }
        public string TenGV { get; set; }
        public string MaKhoa { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public virtual Khoa Khoa { get; set; }
    
        public virtual ICollection<LopHocPhan> LopHocPhans { get; set; }
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
    }
}
