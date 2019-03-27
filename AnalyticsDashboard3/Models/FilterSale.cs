using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalyticsDashboard3.Models
{
    public class FilterSale
    {
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public string SelectedCategory { get; set; }

        public FilterSale(int selectedMonth, int selectedYear, string selectedCategory)
        {
            this.SelectedMonth = selectedMonth;
            this.SelectedYear = selectedYear;
            this.SelectedCategory = selectedCategory;
        }
    }
}