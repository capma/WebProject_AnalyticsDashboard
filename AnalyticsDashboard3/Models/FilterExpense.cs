using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalyticsDashboard3.Models
{
    public class FilterExpense
    {
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public string SelectedCategory { get; set; }

        public FilterExpense(int selectedMonth, int selectedYear, string SelectedCategory)
        {
            this.SelectedMonth = selectedMonth;
            this.SelectedYear = selectedYear;
            this.SelectedCategory = SelectedCategory;
        }
    }
}