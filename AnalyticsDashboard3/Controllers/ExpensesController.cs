using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AnalyticsDashboard3.Models;
using Highsoft.Web.Mvc.Charts;

namespace AnalyticsDashboard3.Controllers
{
    public class ExpensesController : Controller
    {
        private AnalyticsDashboardDB db = new AnalyticsDashboardDB();

        // GET: Expenses
        public ActionResult Index()
        {
            List<Series> listSeries = new List<Series>();
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year;

            ViewData["ListSeriesPrice"] = GetDataExpense(currentMonth, currentYear, "");

            GetDataList();
            FilterExpense filterExpense = new FilterExpense(currentMonth, currentYear, "");
            return View(filterExpense);
        }

        // POST: Expenses
        [HttpPost]
        public ActionResult Index(int? SelectedMonth, int? SelectedYear, string SelectedCategory)
        {
            int getMonth, getYear;

            getMonth = SelectedMonth.GetValueOrDefault();
            getYear = SelectedYear.GetValueOrDefault();

            ViewData["ListSeriesPrice"] = GetDataExpense(getMonth, getYear, SelectedCategory);
            GetDataList();
            FilterExpense filterExpense = new FilterExpense(getMonth, getYear, SelectedCategory);
            return View(filterExpense);
        }

        private List<Series> GetDataExpense(int selectedMonth, int selectedYear, string selectedCategory)
        {
            List<Series> listSeries = new List<Series>();
            List<DateTime> listDates = null;
            List<string> listNames = null;

            if (string.IsNullOrEmpty(selectedCategory))
            {
                if (selectedMonth != 0 & selectedYear != 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Date.Value.Month == selectedMonth
                                            && p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Date.Value.Month == selectedMonth
                                            && p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();

                }
                else if (selectedMonth == 0 & selectedYear != 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
                else if (selectedMonth != 0 & selectedYear == 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Date.Value.Month == selectedMonth)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Date.Value.Month == selectedMonth)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
                else if (selectedMonth == 0 & selectedYear == 0)
                {
                    listDates = db.Expenses
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
            }
            else
            {
                if (selectedMonth != 0 & selectedYear != 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Month == selectedMonth
                                            && p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Month == selectedMonth
                                            && p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
                else if (selectedMonth == 0 & selectedYear != 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Year == selectedYear)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
                else if (selectedMonth != 0 & selectedYear == 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Month == selectedMonth)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Category == selectedCategory
                                            && p.Date.Value.Month == selectedMonth)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
                else if (selectedMonth == 0 & selectedYear == 0)
                {
                    listDates = db.Expenses
                                    .Where(p => p.Category == selectedCategory)
                                    .GroupBy(p => p.Date)
                                    .Select(g => g.FirstOrDefault().Date.Value).ToList();

                    listNames = db.Expenses
                                    .Where(p => p.Category == selectedCategory)
                                    .GroupBy(p => p.Name)
                                    .Select(g => g.FirstOrDefault().Name).ToList();
                }
            }

            List<string> listDateString = new List<string>();
            foreach(DateTime eachDate in listDates)
            {
                string strDate = eachDate.ToShortDateString();
                listDateString.Add(strDate);
            }
            ViewData["XDates"] = listDateString;

            foreach (string eachName in listNames)
            {
                List<ColumnSeriesData> amountData = new List<ColumnSeriesData>();

                foreach (DateTime eachDate in listDates)
                {
                    List<double> priceValues = new List<double>();

                    if (string.IsNullOrEmpty(selectedCategory))
                    {
                        priceValues = db.Expenses
                            .Where(p => p.Name == eachName
                                    && p.Date.Value.Year == eachDate.Year
                                    && p.Date.Value.Month == eachDate.Month
                                    && p.Date.Value.Day == eachDate.Day
                                    )
                            .GroupBy(p => new { p.Date })
                            .Select(g => g.Sum(item => item.Amount.Value)).ToList();
                    }
                    else
                    {
                        priceValues = db.Expenses
                            .Where(p => p.Category == selectedCategory
                                    && p.Name == eachName
                                    && p.Date.Value.Year == eachDate.Year
                                    && p.Date.Value.Month == eachDate.Month
                                    && p.Date.Value.Day == eachDate.Day
                                    )
                            .GroupBy(p => new { p.Date })
                            .Select(g => g.Sum(item => item.Amount.Value)).ToList();
                    }

                    if (priceValues.Count > 0)
                        priceValues.ForEach(p => amountData.Add(new ColumnSeriesData { Y = p }));
                    else
                        amountData.Add(new ColumnSeriesData { Y = 0 });
                }

                listSeries.Add(new ColumnSeries
                {
                    Name = eachName,
                    Data = amountData
                });
            }

            return listSeries;
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


            var lstCategory = db.Expenses.GroupBy(x => x.Category).ToList();

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
