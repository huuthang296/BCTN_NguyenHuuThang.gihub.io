using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MiniShopWebNHT.Models
{
    public class SanPhamGH
    {
        MiniShopEntities db = new MiniShopEntities();

        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string Mota { get; set; }
        public double DonGia { get; set; }
        public DateTime Ngay { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongMua { get; set; }
        public int MaLoai { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double TongTien => SoLuongMua * DonGia;

        public string TenLoai { get; set; }
    }
}