using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalyticsDashboard3.Models
{
    public class FilterAdvertising
    {
        public int SelectedYear { get; set; }
        public string SelectedCategory { get; set; }

        public FilterAdvertising(int selectedYear, string selectedCategory)
        {
            this.SelectedYear = selectedYear;
            this.SelectedCategory = selectedCategory;
        }
    }
}