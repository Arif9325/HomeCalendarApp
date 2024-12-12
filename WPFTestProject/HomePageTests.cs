using Calendar;
using CalendarWPF;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WPFTestProject.EventPageTests;

namespace WPFTestProject
{
    [Collection("Sequential")]

    public class HomePageTests
    {
        public HomePageViewInterface HomePageViewInterface;
        private string TestDBFilePath = "testDBInput.db";
        HomePageTestView homePageTest = new HomePageTestView();

        public class HomePageTestView : HomePageViewInterface
        {
            public bool calledOpenExistingDatabase = false;
            public bool calledOpenNewDatabase = false;
            public bool calledDisplayEventsByCategory = false;
            public bool calledDisplayEventsByMonth = false;
            public bool calledPopulateGrid = false;
            public bool calledUpdateGrid = false;
            public bool calledDisplayItemsByCategoryAndMonth = false;

            public void OpenExistingDatabase(string filename)
            {
                calledOpenExistingDatabase = true;
            }

            public void OpenNewDatabase(string filename)
            {
                calledOpenNewDatabase = true;
            }

            public void DisplayItemsByCategory(List<CalendarItemsByCategory> items)
            {
                calledDisplayEventsByCategory = true;
            }

            public void DisplayItemsByMonth(List<CalendarItemsByMonth> items)
            {
                calledDisplayEventsByMonth = true;
            }

            

            public void UpdateGrid()
            {
                calledUpdateGrid = true;
            }

            public void DisplayItemsByCategoryAndMonth(List<Dictionary<string, object>> items)
            {
                calledDisplayItemsByCategoryAndMonth = true;
            }

            public void PopulateGrid(DateTime? startDate = null, DateTime? endDate = null, int id = -1)
            {
                calledPopulateGrid = true;
            }

            public void MessageBoxOK(string message, string title)
            {
            }
        }

        [Fact]
        public void OpenExistingDatabase()
        {

            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            HomePageTestView view = new HomePageTestView();

            // Act
            view.OpenExistingDatabase(messyDB);

            // Assert
            Assert.True(view.calledOpenExistingDatabase);
        }

        [Fact]
        public void OpenNewDatabase()
        {

            // Arrange
            HomePageTestView view = new HomePageTestView();

            // Act
            view.OpenNewDatabase("newDb");

            // Assert
            Assert.True(view.calledOpenNewDatabase);
        }

        [Fact]
        public void TestCheckFilter_CategoryTrue_MonthFalse()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy1.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var cat = new Category(1, "Event", Category.CategoryType.Event); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, false, startDate, endDate, cat);

            // Assert
            // Add assertions here if needed
        }

        [Fact]
        public void TestCheckFilter_CategoryFalse_MonthTrue()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy2.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var cat = new Category(2, "AllDayEvent", Category.CategoryType.AllDayEvent); // Create a Category instance

            // Act
            Presenter.CheckFilter(false, true, startDate, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByMonth);
        }

        [Fact]
        public void TestCheckFilter_CategoryTrue_MonthTrue()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy3.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var cat = new Category(3, "Holiday", Category.CategoryType.Holiday); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, true, startDate, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayItemsByCategoryAndMonth);
        }

        [Fact]
        public void TestCheckFilter_CategoryFalse_MonthFalse()
        {
            // Arrange

            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy4.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var cat = new Category(4, "Availability", Category.CategoryType.Availability); // Create a Category instance

            // Act
            Presenter.CheckFilter(false, false, startDate, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledPopulateGrid);
        }

        [Fact]
        public void TestCheckFilter_NullStartDateAndEndDate_ByCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy5.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var cat = new Category(1, "Event", Category.CategoryType.Event); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, false, null, null, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByCategory);
        }

        [Fact]
        public void TestCheckFilter_NullStartDate_ByCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy6.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var endDate = DateTime.Now;
            var cat = new Category(2, "AllDayEvent", Category.CategoryType.AllDayEvent); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, false, null, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByCategory);

        }

        [Fact]
        public void TestCheckFilter_NullEndDate_ByCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy7.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var cat = new Category(3, "Holiday", Category.CategoryType.Holiday); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, false, startDate, null, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByCategory);

        }

        [Fact]
        public void TestCheckFilter_NullCategory_ByCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy8.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;

            // Act
            Presenter.CheckFilter(true, false, startDate, endDate, null);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByCategory);

        }

        [Fact]
        public void TestCheckFilter_NullStartDate_ByMonth()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy9.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var endDate = DateTime.Now;
            var cat = new Category(3, "Holiday", Category.CategoryType.Holiday); // Create a Category instance

            // Act
            Presenter.CheckFilter(false, true, null, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByMonth);
        }

        [Fact]
        public void TestCheckFilter_NullEndDate_ByMonth()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy10.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var cat = new Category(3, "Holiday", Category.CategoryType.Holiday); // Create a Category instance

            // Act
            Presenter.CheckFilter(false, true, startDate, null, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayEventsByMonth);
        }

        [Fact]
        public void TestCheckFilter_NullStartDate_ByMonthAndCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy11.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var endDate = DateTime.Now;
            var cat = new Category(4, "Availability", Category.CategoryType.Availability); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, true, null, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayItemsByCategoryAndMonth);
        }

        [Fact]
        public void TestCheckFilter_NullEndDate_ByMonthAndCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy12.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var cat = new Category(4, "Availability", Category.CategoryType.Availability); // Create a Category instance

            // Act
            Presenter.CheckFilter(true, true, startDate, null, cat);

            // Assert
            Assert.True(homePageTest.calledDisplayItemsByCategoryAndMonth);
        }

        [Fact]
        public void TestCheckFilter_NullStartDate_NoSorting()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy13.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var endDate = DateTime.Now;
            var cat = new Category(1, "Event", Category.CategoryType.Event); // Create a Category instance

            // Act
            Presenter.CheckFilter(false, false, null, endDate, cat);

            // Assert
            Assert.True(homePageTest.calledPopulateGrid);
        }

        [Fact]
        public void TestCheckFilter_NullEndDate_NoSorting()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestDBFilePath}";
            String messyDB = $"{folder}\\messy14.db";
            File.Copy(goodDB, messyDB, true);
            Presenter.Connect(messyDB, false, homePageTest);
            var startDate = DateTime.Now.AddDays(-7);
            var cat = new Category(1, "Event", Category.CategoryType.Event); // Create a Category instance

            // Act
            Presenter.CheckFilter(false, false, startDate, null, cat);

            // Assert
            Assert.True(homePageTest.calledPopulateGrid);
        }
    }
}
