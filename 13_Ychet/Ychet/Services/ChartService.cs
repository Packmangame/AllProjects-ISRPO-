using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Ychet.Models;

namespace Ychet.Services
{
    
        public class ChartService
        {
        public void CreatePieChart(Chart chart, IEnumerable<CategorySummary> data, string title)
        {
            chart.Series.Clear();
            chart.Titles.Clear();

            chart.Titles.Add(title);

            var series = new Series
            {
                Name = "Series1",
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };

            foreach (var item in data)
            {
                series.Points.AddXY(item.CategoryName, item.Amount);
            }

            chart.Series.Add(series);
            chart.Legends.Add(new Legend());
        }
    }
    
}
