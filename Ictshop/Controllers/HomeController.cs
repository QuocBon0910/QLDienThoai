using Ictshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ictshop.Controllers
{
    public class HomeController : Controller
    {
        private Qlbanhang db = new Qlbanhang(); // Giả sử bạn sử dụng Entity Framework để kết nối database

        public ActionResult Index()
        {
       
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult SlidePartial()
        {
            return PartialView();

        }

        public ActionResult Search(string query, int? Mahang, int? Mahdh, decimal? MinPrice, decimal? MaxPrice, int? Thesim, int? Bonhotrong, int? Ram, string SortOrder)
        {
            ViewBag.SearchQuery = query;

            // Tìm kiếm sản phẩm
            var searchResults = db.Sanphams.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                searchResults = searchResults.Where(s => s.Tensp.Contains(query));
            }
            if (Mahang.HasValue)
            {
                searchResults = searchResults.Where(s => s.Mahang == Mahang);
            }
            if (Mahdh.HasValue)
            {
                searchResults = searchResults.Where(s => s.Mahdh == Mahdh);
            }
            if (MinPrice.HasValue)
            {
                searchResults = searchResults.Where(s => s.Giatien >= MinPrice);
            }
            if (MaxPrice.HasValue)
            {
                searchResults = searchResults.Where(s => s.Giatien <= MaxPrice);
            }
            if (Thesim.HasValue)
            {
                searchResults = searchResults.Where(s => s.Thesim == Thesim);
            }
            if (Bonhotrong.HasValue)
            {
                searchResults = searchResults.Where(s => s.Bonhotrong >= Bonhotrong);
            }
            if (Ram.HasValue)
            {
                searchResults = searchResults.Where(s => s.Ram >= Ram);
            }

            // Sắp xếp theo tùy chọn SortOrder
            switch (SortOrder)
            {
                case "price-asc":
                    searchResults = searchResults.OrderBy(s => s.Giatien);
                    break;
                case "price-desc":
                    searchResults = searchResults.OrderByDescending(s => s.Giatien);
                    break;
                case "name-asc":
                    searchResults = searchResults.OrderBy(s => s.Tensp);
                    break;
                case "name-desc":
                    searchResults = searchResults.OrderByDescending(s => s.Tensp);
                    break;
                case "newest":
                    searchResults = searchResults.OrderByDescending(s => s.Masp); // Giả sử Masp đại diện cho ID sản phẩm mới nhất
                    break;
            }

            return View("SearchResults", searchResults.ToList());
        }
    }
}