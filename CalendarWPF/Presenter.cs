using Calendar;
using System.Windows;

namespace CalendarWPF
{

    public class Presenter
    {
        public static HomeCalendar _homeCalendar;
        private static EventViewInterface _eventViewInterface;
        private static HomePageViewInterface _mainViewInterface;

        public static bool Connect(string filename, bool newDb, HomePageViewInterface window)
        {
            _mainViewInterface = window;

            try
            {
                _homeCalendar = new HomeCalendar(filename, newDb);
                _mainViewInterface.PopulateGrid();
                return true;
            }
            catch
            {
                window.MessageBoxOK("Invalid database file.", "Error");
                return false;
            }
            
        }

        static public void SetEventView(EventViewInterface viewInterface)
        {
            _eventViewInterface = viewInterface;
        }

        public static bool ValidateCategory(string catDescription, Category.CategoryType? catType)
        {
            if (catDescription == "" || catType == null)
            {
                if (catDescription == "")
                    _eventViewInterface.MessageBoxOK("Please enter category description", "Invalid category description");
                else
                    _eventViewInterface.MessageBoxOK("Please enter category type", "Invalid category type");

                return false;
            }
            else
            {
                AddCategory(catDescription, (Category.CategoryType)catType);
                return true;
            }
        }

        static public List<Category> GetCategories()
        {
            return _homeCalendar.categories.List();
        }

        static public void AddCategory(string categoryDescription, Category.CategoryType type)
        {
            _homeCalendar.categories.Add(categoryDescription, type);
        }

        public static bool IsValidDuration(string durationInMinutes)
        {
            if (double.TryParse(durationInMinutes, out double d))
                return true;
            return false;
        }

        public static void SaveEvent(Category category, string date, string description, string duration)
        {
            double durationInMinutes = double.Parse(duration);
            DateTime dateTime = DateTime.Parse(date);
            _homeCalendar.events.Add(dateTime, category.Id, durationInMinutes, description);

            _mainViewInterface.PopulateGrid();
        }

        public static bool IsSaved(string date, string catDescription, string catDuration, Category selectedCat, Visibility v)
        {
            if (date != null
                && catDescription != ""
                && catDuration != "")
            {
                if (selectedCat != null && v != Visibility.Visible)
                {
                    if (IsValidDuration(catDuration))
                    {
                        SaveEvent(selectedCat, date, catDescription, catDuration);
                        _eventViewInterface.UpdateCategories();
                        return true;
                    }
                    else
                        _eventViewInterface.MessageBoxOK("Please enter duration in minutes", "Invalid duration of event");
                }
                else
                    _eventViewInterface.MessageBoxOK("Please select a valid category from the dropdown", "No category selected");
            }
            else
                _eventViewInterface.MessageBoxOK("Please enter valid event information", "Invalid or missing event information");
            return false;
        }

        public static void Cancel(Category selectedCat, string catDesc, string catDuration, EventViewInterface addEventWindow)
        {
            if (selectedCat == null && catDesc != "" && catDuration != "")
                CloseAddEventWindow(addEventWindow);
            else
            {
                MessageBoxResult result = MessageBox.Show("Would you like to finish with your unsaved changes", "Unsaved Changes", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        CloseAddEventWindow(addEventWindow);
                        break;
                }
            }
        }

        public static List<CalendarItem> GetCalendarItems(DateTime? startDate = null, DateTime? endDate = null ,int id = -1)
        {
            if(id == -1)
            {
                return _homeCalendar.GetCalendarItems(startDate, endDate, false, 1);

            }
            else
            {
                return _homeCalendar.GetCalendarItems(startDate, endDate, true, id);

            }
        }

        public static void Close(EventViewInterface addEventWindow)
        {
            addEventWindow.CloseApplication();
        }
        public static void CloseAddEventWindow(EventViewInterface addEventWindow)
        {
            addEventWindow.CancelAddEvent();
        }
        public static bool IsValidUpdate(int eventId, string date, Category category, string duration, string desc, Visibility v)
        {
            if (date != null
                && desc != ""
                && duration != "")
            {
                if (category != null && v != Visibility.Visible)
                {
                    if (IsValidDuration(duration))
                    {
                        UpdateEvent(eventId, date, category.Id, duration, desc);
                        return true;
                    }
                    else
                        _eventViewInterface.MessageBoxOK("Please enter duration in minutes", "Invalid duration of event");
                }
                else
                    _eventViewInterface.MessageBoxOK("Please select a valid category from the dropdown", "No category selected");
            }
            else
                _eventViewInterface.MessageBoxOK("Please enter valid event information", "Invalid or missing event information");
            return false;
        }

        public static void UpdateEvent(int eventId, string date, int categoryId, string duration, string details)
        {
            _homeCalendar.events.UpdateProperties(eventId, DateTime.Parse(date), categoryId, double.Parse(duration), details);
            _mainViewInterface.PopulateGrid();
        }
        public static void DeleteEvent(int eventId)
        {
            _homeCalendar.events.Delete(eventId);
            _mainViewInterface.PopulateGrid();
        }

        internal static void GetListByCategory(DateTime? startDate = null, DateTime? endDate = null,int id = -1)
        {
            List<CalendarItemsByCategory> list = null;
            if (id == -1)
            {
                list = _homeCalendar.GetCalendarItemsByCategory(startDate, endDate, false, 1);

            }
            else
            {
                list = _homeCalendar.GetCalendarItemsByCategory(startDate, endDate, true, id);

            }
            _mainViewInterface.DisplayItemsByCategory(list);
        }

        internal static void GetListByMonth(DateTime? startDate = null, DateTime? endDate = null, int id = -1)
        {
            List<CalendarItemsByMonth> list = null;
            if (id == -1)
            {
                list = _homeCalendar.GetCalendarItemsByMonth(startDate, endDate, false, 1);

            }
            else
            {
                list = _homeCalendar.GetCalendarItemsByMonth(startDate, endDate, true, id);

            }
            _mainViewInterface.DisplayItemsByMonth(list);
        }

        public static void GetDictionary(DateTime? startDate = null, DateTime? endDate = null, int id = -1)
        {
            List<Dictionary<string, object>> list = null;
            if (id == -1)
            {
                list = _homeCalendar.GetCalendarDictionaryByCategoryAndMonth(startDate, endDate, false, 1);

            }
            else
            {
                list = _homeCalendar.GetCalendarDictionaryByCategoryAndMonth(startDate, endDate, true, id);

            }
            _mainViewInterface.DisplayItemsByCategoryAndMonth(list);
        }

        public static void CheckFilter(bool category, bool month, DateTime? startDate, DateTime? endDate, Category cat = null)
        {
            if (category && !month)
            {
                if(cat != null)
                {
                    Presenter.GetListByCategory(startDate, endDate, cat.Id);

                }
                else
                {
                    Presenter.GetListByCategory(startDate, endDate);

                }
            }
            else if (month && !category)
            {
                if (cat != null)
                {
                    Presenter.GetListByMonth(startDate, endDate,cat.Id);

                }
                else
                {
                    Presenter.GetListByMonth(startDate, endDate);

                }
            }
            else if (!category && !month)
            {
                if (cat != null)
                {
                    _mainViewInterface.PopulateGrid(startDate, endDate, cat.Id);


                }
                else
                {
                    _mainViewInterface.PopulateGrid(startDate, endDate);

                }
            }
            else
            {
                if (cat != null)
                {
                    Presenter.GetDictionary(startDate, endDate, cat.Id);

                }
                else
                {
                    Presenter.GetDictionary(startDate, endDate);

                }
            }
        }
    }
}
