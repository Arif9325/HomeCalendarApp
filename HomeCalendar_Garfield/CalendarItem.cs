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
    // CLASS: CalendarItem
    //        A single calendar item, includes a Category and an Event
    // ====================================================================

    // ====================================================================
    // CLASS: CalendarItem
    //        A single calendar item, includes a Category and an Event
    // ====================================================================
    /// <summary>
    /// Represents a single calendar item, including a Category and an Event.
    /// </summary>
    public class CalendarItem
    {
        /// <summary>
        /// Gets or sets the ID for the CalendarItem.
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Gets or sets the EventID for the CalendarItem.
        /// </summary>
        public int EventID { get; set; }
        /// <summary>
        /// Gets or sets the StartDateTime for the CalendarItem.
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// Gets or sets the category for the CalendarItem.
        /// </summary>
        public String? Category { get; set; }
        /// <summary>
        /// Gets or sets the ShortDescription for the CalendarItem.
        /// </summary>
        public String? ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the BusyTime for the CalendarItem.
        /// </summary>
        public Double DurationInMinutes { get; set; }
        /// <summary>
        /// Gets or sets the BusyTime for the CalendarItem.
        /// </summary>
        public Double BusyTime 
        { 
            get; 
            set; 
        }

    }

    /// <summary>
    /// Represents a collection of calendar items grouped by month.
    /// </summary>
    public class CalendarItemsByMonth
    {
        /// <summary>
        /// Gets or sets month.
        /// </summary>
        public String? Month { get; set; }
        /// <summary>
        /// Gets or sets list of Items.
        /// </summary>
        public List<CalendarItem>? Items { get; set; }
        /// <summary>
        /// Gets or sets Totalbusytime
        /// </summary>
        public Double TotalBusyTime { get; set; }
    }

    /// <summary>
    /// Represents a collection of calendar items grouped by category.
    /// </summary>
    public class CalendarItemsByCategory
    {
        /// <summary>
        /// Gets or sets category.
        /// </summary>
        public String? Category { get; set; }
        /// <summary>
        /// Gets or sets list of Items.
        /// </summary>
        public List<CalendarItem>? Items { get; set; }
        /// <summary>
        /// Gets or sets TotalBusyTime.
        /// </summary>
        public Double TotalBusyTime { get; set; }

    }


}
