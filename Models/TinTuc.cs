//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiniShopWebNHT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TinTuc
    {
        public int ID { get; set; }
        public string TenBaiViet { get; set; }
        public string NoiDung { get; set; }
        public int MaLoaiTinTuc { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string MetaTitle { get; set; }
        public string AnhDaiDien { get; set; }
        public Nullable<System.DateTime> CapNhat { get; set; }
    
        public virtual LoaiTinTuc LoaiTinTuc { get; set; }
    }
}
