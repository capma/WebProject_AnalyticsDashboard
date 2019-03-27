using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AnalyticsDashboard3.Models;
using Highsoft.Web.Mvc.Charts;

namespace AnalyticsDashboard3.Controllers
{
    public class SalesController : Controller
    {
        private AnalyticsDashboardDB db = new AnalyticsDashboardDB();

        // GET: Sales
        public ActionResult Index()
        {
            List<PieSeriesData> pieData = new List<PieSeriesData>();
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year;

            ViewData["pieData"] = GetSaleData(currentMonth, currentYear, "");

            GetDataList();
            FilterSale filterSale = new FilterSale(currentMonth, currentYear, "");
            return View(filterSale);
        }

        // POST: Sales
        [HttpPost]
        public ActionResult Index(int? SelectedMonth, int? SelectedYear, string SelectedCategory)
        {
            int getMonth, getYear;

            getMonth = SelectedMonth.GetValueOrDefault();
            getYear = SelectedYear.GetValueOrDefault();

            List<PieSeriesData> pieData = new List<PieSeriesData>();
            FilterSale filterSale = new FilterSale(getMonth, getYear, SelectedCategory);
            GetDataList();

            ViewData["pieData"] = GetSaleData(getMonth, getYear, SelectedCategory);

            return View(filterSale);
        }

        private List<PieSeriesData> GetSaleData(int selectedMonth, int selectedYear, string selectedCategory)
        {
            List<PieSeriesData> pieData = new List<PieSeriesData>();
            List<Sale> listSale = new List<Sale>();

            if (string.IsNullOrEmpty(selectedCategory))
            {
                if (selectedMonth == 0 && selectedYear == 0)
                {
                    listSale = db.Sales.ToList();
                }
                else if (selectedMonth != 0 && selectedYear != 0)
                {
                    listSale = db.Sales.Where(p => p.SaleDate.Value.Year == selectedYear && p.SaleDate.Value.Month == selectedMonth).ToList();
                }
                else if (selectedMonth != 0 && selectedYear == 0)
                {
                    listSale = db.Sales.Where(p => p.SaleDate.Value.Month == selectedMonth).ToList();
                }
                else if (selectedMonth == 0 && selectedYear != 0)
                {
                    listSale = db.Sales.Where(p => p.SaleDate.Value.Year == selectedYear).ToList();
                }
            }
            else
            {
                if (selectedMonth == 0 && selectedYear == 0)
                {
                    listSale = db.Sales.Where(p => p.Category == selectedCategory).ToList();
                }
                else if (selectedMonth != 0 && selectedYear != 0)
                {
                    listSale = db.Sales.Where(p => p.Category == selectedCategory
                                                && p.SaleDate.Value.Year == selectedYear
                                                && p.SaleDate.Value.Month == selectedMonth).ToList();
                }
                else if (selectedMonth == 0 && selectedYear != 0)
                {
                    listSale = db.Sales.Where(p => p.Category == selectedCategory && p.SaleDate.Value.Year == selectedYear).ToList();
                }
                else if (selectedMonth != 0 && selectedYear == 0)
                {
                    listSale = db.Sales.Where(p => p.Category == selectedCategory && p.SaleDate.Value.Month == selectedMonth).ToList();
                }
            }

            foreach (Sale eachSale in listSale)
            {
                pieData.Add(new PieSeriesData { Name = eachSale.SoftwareName, Y = (double)eachSale.Amount });
            }

            return pieData;
        }

        private void GetDataList()
        {
            ViewBag.Months = new SelectList(Enumerable.Range(1, 12)
                   .Select(x =>
                     new SelectListItem()
                     {
                         Text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(x),
                         Value = x.ToString()
                     }), "Value", "Text");

            int yearStart = DateTime.Today.Year - 5;
            ViewBag.Years = new SelectList(Enumerable.Range(yearStart, 6)
                .Select(x =>
                   new SelectListItem()
                   {
                       Text = x.ToString(),
                       Value = x.ToString()
                   }), "Value", "Text");

            var lstCategory = db.Sales.GroupBy(x => x.Category).ToList();

            ViewBag.Categories = new SelectList(lstCategory.AsEnumerable()
                                .Select(row => new SelectListItem()
                                {
                                    Text = row.Key,
                                    Value = row.Key
                                })
                                , "Value", "Text");
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
