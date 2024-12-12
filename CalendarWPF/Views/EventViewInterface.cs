using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF
{
    public interface EventViewInterface
    {
        public void DisplayEventWindow();
        public void MessageBoxOK(string message, string title);
        public void MessageBoxYesNo(string message, string title);
        public void UpdateCategories();
        public void AddCategory();
        public void SaveEvent();
        public void CancelSavingEvent();
        public void CloseApplication();
        public void UpdateEventInitialization(object sender);
        public void AddEventWindowInitialization();
        public void UpdateEvent();
        public void DeleteEvent();
        public void CancelAddEvent();
    }
}
