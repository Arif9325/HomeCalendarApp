using Calendar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF
{
    public interface HomePageViewInterface
    {
        public void OpenExistingDatabase(string filename);
        public void OpenNewDatabase(string filename);

        public void DisplayItemsByCategory(List<CalendarItemsByCategory> items);
        public void DisplayItemsByMonth(List<CalendarItemsByMonth> items);
        public void DisplayItemsByCategoryAndMonth(List<Dictionary<string, object>> items);
        public void PopulateGrid(DateTime? startDate = null, DateTime? endDate = null, int id = -1);
        public void UpdateGrid();

        public void MessageBoxOK(string message, string title);

    }
}
