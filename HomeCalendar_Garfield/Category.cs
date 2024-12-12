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
    // CLASS: Category
    //        - An individual category for Calendar program
    //        - Valid category types: Event,AllDayEvent,Holiday
    // ====================================================================

    /// <summary>
    /// Represents an individual category for a calendar program.
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets or sets Id of category.
        /// </summary>
        public int Id { get;  }//propety set to readonly by not having a setter
        /// <summary>
        /// Gets or sets Description of Category.
        /// </summary>
        public String Description { get; }//propety set to readonly by not having a setter
        /// <summary>
        /// Gets or sets Type of category
        /// </summary>
        public CategoryType Type { get; }//propety set to readonly by not having a setter
        /// <summary>
        /// Gets Enum of types of categories
        /// </summary>
        public enum CategoryType
        {
            Event = 1,
            AllDayEvent,
            Holiday,
            Availability
        };

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Creates category object from id, description, type passed in.
        /// </summary>
        /// <param name="id">Id of category.</param>
        /// <param name="description">Description of category.</param>
        /// <param name="type">Type of category.</param>
        public Category(int id, String description, CategoryType type = CategoryType.Event)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================
        /// <summary>
        /// Copies this category's backing fields to the backing fields of the category passed in.
        /// </summary>
        /// <param name="category">Category to copy.</param>
        public Category(Category category)
        {
            this.Id = category.Id; ;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================
        /// <summary>
        /// Overrides ToString to return category Description.
        /// </summary>
        /// <returns>Returns Description.</returns>
        public override string ToString()
        {
            return Description;
        }

    }
}

