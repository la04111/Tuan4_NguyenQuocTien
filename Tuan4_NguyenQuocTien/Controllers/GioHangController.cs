using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using Tuan4_NguyenQuocTien.Models;
namespace Tuan4_NguyenQuocTien.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        MyDataDataContext db = new MyDataDataContext();
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGioHang(int id, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sp = lstGiohang.Find(x => x.masach == id);
            if (sp == null)
            {
                sp = new Giohang(id);
                lstGiohang.Add(sp);
                return Redirect(strURL);
            }
            else
            {
                sp.iSoluong++;
                return Redirect(strURL);
            }
        }
        public int TongSoLuong()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.iSoluong);
            }
            return tsl;
        }
        public int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }
        public double TongTien()
        {
            double tsl = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.dThanhtien);
            }
            return tsl;
        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult GioHangPartial()
        {
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult XoaGioHang(int id)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sp = lstGiohang.SingleOrDefault(x => x.masach == id);
            if (sp != null)
            {
                lstGiohang.RemoveAll(n => n.masach == id);
                return RedirectToAction("Giohang");
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult Capnhatgiohang(int id, FormCollection collection)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sp = lstGiohang.SingleOrDefault(n => n.masach == id);
            if (sp != null)
            {
                sp.iSoluong = int.Parse(collection["txtSoLg"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCaGioHang()
        {
            List<Giohang> listGioHang = Laygiohang();
            listGioHang.Clear();
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
               // MessageBox.Show("Vui lòng đăng nhập", "", MessageBoxButton.OK);
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
            /* Thông báo đặt hàng */
            //List<Giohang> listGioHang = Laygiohang();
            //var temp = listGioHang;

            //foreach(var item in temp)
            //{
            //    var change = db.Saches.Where(x=>x.masach == item.masach).FirstOrDefault();
            //    if(change != null)
            //    {
            //        if (change.soluongton >= item.iSoluong)
            //        {
            //            var tempsl = change.soluongton - item.iSoluong;
            //            change.soluongton = tempsl;
            //            UpdateModel(change);
            //            db.SubmitChanges();

            //        }
            //    }
            //}
            //return View(listGioHang);
            //listGioHang.Clear();
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            Sach s = new Sach();

            List<Giohang> listGioHang = Laygiohang();
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);

            dh.makh = kh.makh;
            dh.ngaydat = DateTime.Now;
            dh.ngaygiao = DateTime.Parse(ngaygiao);
            dh.giaohang = false;
            dh.thanhtoan = false;

            db.DonHangs.InsertOnSubmit(dh);
            db.SubmitChanges();
            foreach(var item in listGioHang)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.masach = item.masach;
                ctdh.soluong = item.iSoluong;
                ctdh.gia = (decimal)item.giaban;
                s = db.Saches.Single(n => n.masach == item.masach);
                s.soluongton -= ctdh.soluong;
                db.SubmitChanges();
                db.ChiTietDonHangs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("XacnhanDonhang", "GioHang");
          
        }
        public ActionResult XacnhanDonhang()
        {
            return View();
        }
    }
}
