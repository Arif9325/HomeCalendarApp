using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Data.SQLite;

namespace CalendarCodeTests
{
    public class TestEvents
    {
        int numberOfEventsInFile = TestConstants.numberOfEventsInFile;
        String testInputFile = TestConstants.testEventsInputFile;
        int maxIDInEventFile = TestConstants.maxIDInEventFile;
        Event firstEventInFile = new Event(1, new DateTime(2021, 1, 10), 3, 40, "App Dev Homework");


        // ========================================================================

        [Fact]
        public void EventsObject_New()
        {
            // Arrange
            int id = 1;
            string details = "New Event";
            DateTime date = DateTime.Now;
            int categoryId = 1;
            double duration = 20.0;
            

            // Act
            Event eve = new Event(id, date, categoryId, duration, details);

            // Assert 
            Assert.IsType<Event>(eve);
            Assert.Equal(id, eve.Id);
            Assert.Equal(details, eve.Details);
            Assert.Equal(date, eve.StartDateTime);
            Assert.Equal(categoryId, eve.Category);
            Assert.Equal(duration, eve.DurationInMinutes);


        }


        // ========================================================================

        [Fact]
        public void EventsMethod_List_ReturnsListOfEvents()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);

            // Act
            List<Event> list = events.List();

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);

        }

        // ========================================================================



        [Fact]
        public void EventsMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existinDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(existinDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);

            // Act
            List<Event> list = events.List();
            Event firstEvent = list[0];

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);
            Assert.Equal(firstEventInFile.Id, firstEvent.Id);
            Assert.Equal(firstEventInFile.Details, firstEvent.Details);

        }

        // ========================================================================

        [Fact]

        public void EventsMethod_GetEventFromId()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int eventID = 7;

            // Act
            Event vent = events.GetEventFromId(eventID);

            // Assert
            Assert.Equal(eventID, vent.Id);

        }





        // ========================================================================

        [Fact]
        public void EventsMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            string details = "New Event";
            DateTime date = DateTime.Now;
            int categoryId = 1;
            double duration = 20.0;



            // Act
            events.Add(date, categoryId, duration, details);
            List<Event> eventList = events.List();
            int sizeOfList = events.List().Count;

            // Assert
            Assert.Equal(numberOfEventsInFile + 1, sizeOfList);
            Assert.Equal(details, eventList[sizeOfList - 1].Details);

        }

        // ========================================================================

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete()
        {
            // Arrang
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int IdToDelete = 3;

            // Act
            events.Delete(IdToDelete);
            List<Event> EventsList = events.List();
            int sizeOfList = EventsList.Count;

            // Assert
            Assert.Equal(numberOfEventsInFile - 1, sizeOfList);
            Assert.False(EventsList.Exists(e => e.Id == IdToDelete), "correct Category item deleted");

        }

        [Fact]
        public void EventsMethod_Delete_InvalidIDDoesntCrash()
        {

            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int IdToDelete = 9999;
            int sizeOfList = events.List().Count;

            // Act
            try
            {
                events.Delete(IdToDelete);
                Assert.Equal(sizeOfList, events.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }

        // ========================================================================

        [Fact]
        public void EventsMethod_UpdateCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events Events = new Events(conn);
            string details = "New Event";
            DateTime date = DateTime.Now;
            int categoryId = 1;
            double duration = 20.0;
            int id = 3;

            // Act
            Events.UpdateProperties(id, details, date, categoryId, duration);
            Event eve = Events.GetEventFromId(id);

            // Assert 
            Assert.Equal(details, eve.Details);
            Assert.Equal(date, eve.StartDateTime);
            Assert.Equal(categoryId, eve.Category);
            Assert.Equal(duration, eve.DurationInMinutes);

        }

    }
}

