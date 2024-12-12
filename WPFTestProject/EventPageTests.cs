using Calendar;
using CalendarWPF;
using static WPFTestProject.HomePageTests;

namespace WPFTestProject
{
    public class EventPageTests
    {
        public int numberOfEventsInFile = TestConstants.numberOfEventsInFile;
        public int numberOfCategoriesInFile = TestConstants.numberOfCategoriesInFile;
        public int maxIDInCategoryInFile = TestConstants.maxIDInCategoryInFile;
        Category firstCategoryInFile = TestConstants.firstCategoryInFile;
        int IDWithAllDayEventType = TestConstants.CategoryIDWithAllDayEventType;
        int IDWithAvailabilityType = TestConstants.CategoryIDWithAvailabilityType;
        private string TestDBFilePath = "testDBInput.db";
        public EventViewInterface eventViewInterface;
        public EventTestView AddEventView = new EventTestView();
        HomePageTestView homePageTest = new HomePageTestView();

        public class EventTestView : EventViewInterface
        {
            public bool calledMessageBoxOk = false;
            public bool calledMessageBoxYesNo = false;
            public bool calledUpdateCategories = false;
            public bool calledAddCategory = false;
            public bool calledCancelSavingEvent = false;
            public bool calledDisplayEventWindow = false;
            public bool calledSaveEvent = false;
            public bool calledUpdateEventInitialisation = false;
            public bool calledCloseApplication = false;
            public bool calledAddEventInitialisation = false;
            public bool calledUpdateEvent = false;
            public bool calledDeleteEvent = false;

            public void MessageBoxOK(string message, string test)
            {
                calledMessageBoxOk = true;
            }

            public void MessageBoxYesNo(string message, string test)
            {
                calledMessageBoxYesNo = true;
            }

            public void UpdateCategories()
            {
                calledUpdateCategories = true;
            }

            public void AddCategory()
            {
                calledAddCategory = true;
            }

            public void CancelSavingEvent()
            {
                calledCancelSavingEvent = true;
            }

            public void DisplayEventWindow()
            {
                calledDisplayEventWindow = true;
            }

            public void SaveEvent()
            {
                calledSaveEvent = true;
            }

            public void CloseApplication()
            {
                calledCloseApplication = true;
            }

            public void UpdateEventInitialization(object sender)
            {
                calledUpdateEventInitialisation = true;
            }

            public void AddEventWindowInitialization()
            {
                calledAddEventInitialisation = true;
            }

            public void UpdateEvent()
            {
                calledUpdateEvent = true;
            }

            public void DeleteEvent()
            {
                calledDeleteEvent = true;
            }

            public void CancelAddEvent()
            {

            }
        }

        [Fact]
        public void TestMessageBoxOK()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.MessageBoxOK("", "");

            // Assert
            Assert.True(view.calledMessageBoxOk);
        }

        [Fact]
        public void TestMessageBoxYesNo()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.MessageBoxYesNo("", "");

            // Assert
            Assert.True(view.calledMessageBoxYesNo);
        }

        [Fact]
        public void TestMessageUpdateCategories()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.UpdateCategories();

            // Assert
            Assert.True(view.calledUpdateCategories);
        }

        [Fact]
        public void CancelSavingEvent()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.CancelSavingEvent();

            // Assert
            Assert.True(view.calledCancelSavingEvent);
        }

        [Fact]
        public void DisplayEventWindow()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.DisplayEventWindow();

            // Assert
            Assert.True(view.calledDisplayEventWindow);
        }

        [Fact]
        public void SaveEvent()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.SaveEvent();

            // Assert
            Assert.True(view.calledSaveEvent);
        }

        [Fact]
        public void CloseApplication()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.CloseApplication();

            // Assert
            Assert.True(view.calledCloseApplication);
        }

        [Fact]
        public void AddCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy15.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            string descr = "New Category";
            Category.CategoryType type = Category.CategoryType.Event;
            numberOfCategoriesInFile = Presenter._homeCalendar.categories.List().Count;

            // Act
            Presenter.AddCategory(descr, type);
            List<Category> categoriesList = Presenter.GetCategories();
            int sizeOfList = categoriesList.Count;

            // Assert
            Assert.Equal(numberOfCategoriesInFile + 1, sizeOfList);
            Assert.Equal(descr, categoriesList[sizeOfList - 1].Description);
        }

        // ========================================================================

        [Fact]
        public void CategoriesMethod_List_ReturnsListOfCategories()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy16.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            numberOfCategoriesInFile = Presenter._homeCalendar.categories.List().Count;

            // Act
            List<Category> list = Presenter.GetCategories();

            // Assert
            Assert.Equal(numberOfCategoriesInFile, list.Count);

        }

        [Fact]
        public void SaveEvent_Test()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy17.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            Category category = firstCategoryInFile;
            string date = DateTime.Now.ToString();
            numberOfEventsInFile = Presenter._homeCalendar.events.List().Count;

            // Act
            Presenter.SaveEvent(category, date, "new_event", "10");
            List<Event> list = Presenter._homeCalendar.events.List();

            // Assert
            Assert.Equal(numberOfEventsInFile + 1, list.Count);
            Assert.Equal("new_event", list[numberOfEventsInFile].Details);
            Assert.Equal(10, list[numberOfEventsInFile].DurationInMinutes);
        }

        [Fact]
        public void ValidateCategory_EmptyDescription_ReturnsFalse()
        {
            Presenter.SetEventView(AddEventView);

            var result = Presenter.ValidateCategory("", Category.CategoryType.Event);

            Assert.False(result);

        }

        [Fact]
        public void ValidateCategory_EmptyDescription_ReturnsTrue()
        {
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy18.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);

            var result = Presenter.ValidateCategory("New category", Category.CategoryType.Event);

            Assert.True(result);

        }

        [Fact]
        public void IsValidDuration_ValidDuration_ReturnsTrue()
        {
            // Act
            var result = Presenter.IsValidDuration("60");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidDuration_InvalidDuration_ReturnsFalse()
        {
            // Act
            var result = Presenter.IsValidDuration("abc");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Test_UpdateEventInitialisation()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.UpdateEventInitialization(null);

            // Assert
            Assert.True(view.calledUpdateEventInitialisation);
        }

        [Fact]
        public void Test_AddEventInitialisation()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.AddEventWindowInitialization();

            // Assert
            Assert.True(view.calledAddEventInitialisation);
        }

        [Fact]
        public void Test_UpdateEvent()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.UpdateEvent();

            // Assert
            Assert.True(view.calledUpdateEvent);
        }

        [Fact]
        public void Test_DeleteEvent()
        {
            // Arrange
            EventTestView view = new EventTestView();

            // Act
            view.DeleteEvent();

            // Assert
            Assert.True(view.calledDeleteEvent);
        }

        [Fact]
        public void Test_UpdatingEvent()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy19.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            List<Event> eventsList = Presenter._homeCalendar.events.List();
            Event updatedEvent = null;

            // Act
            Presenter.UpdateEvent(eventsList[0].Id, eventsList[0].StartDateTime.ToString(), 4, "500", "changed event");
            List<Event> updatedList = Presenter._homeCalendar.events.List();
            foreach(Event e in updatedList)
            {
                if(e.Id == eventsList[0].Id)
                        updatedEvent = e;
            }

            // Assert
            Assert.Equal(4, updatedEvent.Category);
            Assert.Equal(500, updatedEvent.DurationInMinutes);
            Assert.Equal("changed event", updatedEvent.Details);

        }

        [Fact]
        public void Test_DeletingEvent()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy20.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            List<Event> eventsList = Presenter._homeCalendar.events.List();

            // Act
            Presenter.DeleteEvent(eventsList[0].Id);
            List<Event> updatedList = Presenter._homeCalendar.events.List();

            // Assert
            Assert.DoesNotContain(eventsList[0], updatedList);
        }
    }
}