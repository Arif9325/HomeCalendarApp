using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Events
    //        - A collection of Event items,
    //        - Read / write to the database
    //        - etc
    // ====================================================================
    /// <summary>
    /// Represents a collection of events with methods to read, write, add, delete, and list events.
    /// </summary>
    public class Events
    {
        private SQLiteConnection _con;

        /// <summary>
        /// An events object is made with the database file passed.
        /// </summary>
        /// <param name="con">The SQLite connection to the database (new or existing)</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection con = Database.dbConnection;
        ///     conn.Open();
        ///     Events e = new Events(con);
        /// ]]>
        /// </code>
        /// </example>
        public Events(SQLiteConnection con)
        {
            _con = con;
        }

        // ====================================================================
        // Add Event
        // ====================================================================

        /// <summary>
        /// Adds an event to event table in the database
        /// </summary>
        /// <param name="date">Start date and time of the event</param>
        /// <param name="category">Category of the event</param>
        /// <param name="duration">Duration of the event</param>
        /// <param name="details">A short description of the event</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection conn = Database.dbConnection;
        ///     conn.Open();
        ///     Events e = new Events(conn);
        ///     e.Add(new DateTime(), 4, 3.5, "Meeting");
        /// ]]>
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double duration, string details)
        {
            try
            {
                using var cmd = new SQLiteCommand(_con);
                cmd.CommandText = "INSERT INTO Events (StartDateTime, Details, DurationInMinutes, CategoryId) VALUES(@startTime, @details, @duration, @catID)";

                cmd.Parameters.AddWithValue("@startTime", date);
                cmd.Parameters.AddWithValue("@details", details);
                cmd.Parameters.AddWithValue("@duration", duration);
                cmd.Parameters.AddWithValue("@catID", category);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Uknown error, " + e.Message);
            }

        }

        // ====================================================================
        // Delete Event
        // ====================================================================

        /// <summary>
        /// Deletes an event to event table in the database
        /// </summary>
        /// <param name="Id">Id of the event to be deleted</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection conn = Database.dbConnection;
        ///     conn.Open();
        ///     Events e = new Events(conn);
        ///     e.Delete(4);
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            try
            {

                using var cmd = new SQLiteCommand(_con);
                cmd.CommandText = "DELETE FROM events where Id = @id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Uknown error, " + e.Message);
            }

        }

        // ====================================================================
        // List Events
        // ====================================================================

        /// <summary>
        /// List events from the event table in the database.
        /// </summary>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection conn = Database.dbConnection;
        ///     conn.Open();
        ///     Events e = new Events(conn);
        ///     List<Event> eventsList = e.List();
        /// ]]>
        /// </code>
        /// </example>
        public List<Event> List()
        {
            List<Event> newList = new List<Event>();
            string stm = "SELECT * FROM Events ORDER BY Id";

            using var cmd = new SQLiteCommand(stm, _con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                DateTime date = DateTime.Parse(rdr.GetString(1));
                Event ev = new Event(rdr.GetInt32(0), date, rdr.GetInt32(4), rdr.GetDouble(2), rdr.GetString(3));
                newList.Add(ev);

            }
            return newList;
        }

        // ====================================================================
        // Gets event from its Id
        // ====================================================================

        /// <summary>
        /// Gets event from its Id from the event table in the database.
        /// </summary>
        /// <param name="eventID">Id of the event to be retrieved</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection conn = Database.dbConnection;
        ///     conn.Open();
        ///     Events e = new Events(conn);
        ///     Event event = e.GetEventFromId(5);
        /// ]]>
        /// </code>
        /// </example>
        public Event GetEventFromId(int eventID)
        {
            Event ev = null;
            try
            {
                using var cmd = new SQLiteCommand(_con);

                cmd.CommandText = $"SELECT * FROM Events WHERE Id = @Id";
                cmd.Parameters.Add(new SQLiteParameter("@Id", eventID));
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    DateTime date = DateTime.Parse(rdr.GetString(1));
                    ev = new Event(rdr.GetInt32(0), date, rdr.GetInt32(4), rdr.GetDouble(2), rdr.GetString(3));

                }
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving Event: " + e.Message);
            }

            return ev;
        }

        // ====================================================================
        // Updates properties of an event
        // ====================================================================

        /// <summary>
        /// Updates properties of an event in the event table in the database.
        /// </summary>
        /// <param name="eventID">Id of the event to be retrieved</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///     Database.existingDatabase("calendar.db");
        ///     SQLiteConnection conn = Database.dbConnection;
        ///     conn.Open();
        ///     Events e = new Events(conn);
        ///     e.UpdateProperties(3, "Availability", new DateTime(), 5, 4.0);
        /// ]]>
        /// </code>
        /// </example>
        public void UpdateProperties(int id, string details, DateTime date, int categoryId, double duration)
        {
            try
            {
                using var cmd = new SQLiteCommand(_con);
                cmd.CommandText = "UPDATE Events SET StartDateTime = @date, Details = @details, DurationInMinutes = @duration, CategoryId = @categoryId  WHERE Id = @id";

                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@details", details);
                cmd.Parameters.AddWithValue("@duration", duration);
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Error updating event properties: " + e.Message);
            }
        }

        //cmd.CommandText = "INSERT INTO Events (StartDateTime, Details, DurationInMinutes, CategoryId) VALUES(@startTime, @details, @duration, @catID)";

    }
}

