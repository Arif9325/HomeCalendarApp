using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Event
    //        - An individual event for calendar program
    // ====================================================================
    /// <summary>
    /// Represents an individual event for a calendar program.
    /// </summary>
    public class Event
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets Id of event.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets start date of event.
        /// </summary>
        public DateTime StartDateTime { get; }
        /// <summary>
        /// Gets or sets duration of event.
        /// </summary>
        public Double DurationInMinutes { get; set; }
        /// <summary>
        /// Gets or sets details of event.
        /// </summary>
        public String Details { get; set; }
        /// <summary>
        /// Gets or sets category of event.
        /// </summary>
        public int Category { get; set; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the event category exists in the
        //        categories object
        // ====================================================================

        /// <summary>
        /// Creates Event object with id, date, category, duration, details.
        /// </summary>
        /// <param name="id">Id of event.</param>
        /// <param name="date">Date of event.</param>
        /// <param name="category">Category of event.</param>
        /// <param name="duration">Duration of event.</param>
        /// <param name="details">Details of event.</param>
        public Event(int id, DateTime date, int category, Double duration, String details)
        {
            this.Id = id;
            this.StartDateTime = date;
            this.Category = category;
            this.DurationInMinutes = duration;
            this.Details = details;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Copies id, date, category, duration, details of this event to the ones of the passed in event.
        /// </summary>
        /// <param name="obj">Event to be copied.</param>
        public Event(Event obj)
        {
            this.Id = obj.Id;
            this.StartDateTime = obj.StartDateTime;
            this.Category = obj.Category;
            this.DurationInMinutes = obj.DurationInMinutes;
            this.Details = obj.Details;

        }
    }
}
