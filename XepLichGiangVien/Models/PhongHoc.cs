

namespace XepLichGiangVien.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PhongHoc
    {

        [Key]
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public Nullable<int> SoLuongChoNgoi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichDay> LichDays { get; set; }
    }
}
