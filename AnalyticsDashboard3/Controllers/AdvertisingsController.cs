using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AnalyticsDashboard3.Models;
using Highsoft.Web.Mvc.Charts;

namespace AnalyticsDashboard3.Controllers
{
    public class AdvertisingsController : Controller
    {
        private AnalyticsDashboardDB db = new AnalyticsDashboardDB();

        // GET: Advertisings
        public ActionResult Index()
        {
            List<Series> listSeries = new List<Series>();
            int currentYear = DateTime.Today.Year;

            listSeries = GetDataNumberOfClicks(currentYear, "");

            ViewData["ListSeriesPrice"] = listSeries;
            GetDataList();
            FilterAdvertising filterAdvertising = new FilterAdvertising(currentYear, "");

            return View(filterAdvertising);
        }

        // POST: Expenses
        [HttpPost]
        public ActionResult Index(int? SelectedYear, string SelectedCategory)
        {
            int getYear;

            getYear = SelectedYear.GetValueOrDefault();
            

            var listOfDate = db.Advertisings
                                    .Where(p => p.Category == SelectedCategory
                                            && p.Date.Value.Year == getYear)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.ToString()).ToList();

            ViewData["XMonths"] = listOfDate;
            
            List<Series> listSeries = new List<Series>();
            listSeries = GetDataNumberOfClicks(getYear, SelectedCategory);
            ViewData["ListSeriesPrice"] = listSeries;

            GetDataList();
            FilterAdvertising filterExpense = new FilterAdvertising(getYear, SelectedCategory);
            return View(filterExpense);
        }

        private List<Series> GetDataNumberOfClicks(int selectedYear, string selectedCategory)
        {
            List<Series> listSeries = new List<Series>();

            var listCategory = db.Advertisings
                                .GroupBy(p => p.Category)
                                .Select(group => group.FirstOrDefault().Category)
                                .ToList();

            var listMonths = db.Advertisings
                                .Where(p => p.Date.Value.Year == selectedYear)
                                .GroupBy(p => p.Date.Value.Month)
                                .Select(group => group.FirstOrDefault().Date.Value.Month)
                                .ToList();

            if (string.IsNullOrEmpty(selectedCategory))
            {
                foreach (string eachCategory in listCategory)
                {
                    List<LineSeriesData> amountData = new List<LineSeriesData>();

                    foreach (int eachMonth in listMonths)
                    {
                        var priceValues = db.Advertisings
                                        .Where(p => p.Category == eachCategory
                                                && p.Date.Value.Year == selectedYear
                                                && p.Date.Value.Month == eachMonth)
                                        .GroupBy(p => new { p.Date })
                                        .Select(g => g.Sum(item => item.NumberOfClicks)).ToList();


                        priceValues.ForEach(p => amountData.Add(new LineSeriesData { Y = p }));
                    }

                    listSeries.Add(new LineSeries
                    {
                        Name = eachCategory,
                        Data = amountData
                    });
                }
            }
            else
            {
                List<LineSeriesData> amountData = new List<LineSeriesData>();

                foreach (int eachMonth in listMonths)
                {
                    var priceValues = db.Advertisings
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Year == selectedYear
                                            && p.Date.Value.Month == eachMonth)
                                    .GroupBy(p => new { p.Date })
                                    .Select(g => g.Sum(item => item.NumberOfClicks)).ToList();


                    priceValues.ForEach(p => amountData.Add(new LineSeriesData { Y = p }));
                }

                listSeries.Add(new LineSeries
                {
                    Name = selectedCategory,
                    Data = amountData
                });
            }

            return listSeries;
        }


        private void GetDataList()
        {
            int startYear = DateTime.Today.Year - 3;
            ViewBag.Years = new SelectList(Enumerable.Range(startYear, 4)
                .Select(x =>
                   new SelectListItem()
                   {
                       Text = x.ToString(),
                       Value = x.ToString()
                   }), "Value", "Text");


            var lstCategory = db.Advertisings.GroupBy(x => x.Category).ToList();
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
