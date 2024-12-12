using System.Data;
using System.Data.SQLite;
using System.Globalization;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Calendar
{
    // ====================================================================
    // CLASS: HomeCalendar
    //        - Combines a Categories Class and an Events Class
    //        - One File defines Category and Events File
    //        - etc
    // ====================================================================
    /// <summary>
    /// Combines categories and events into a single calendar entity, with different methods to return a list or dictionary organizing the events.
    /// </summary>
    public class HomeCalendar
    {
        private Categories _categories;
        private Events _events;
        private SQLiteConnection _con;


        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (categories and events object)
        /// <summary>
        /// Gets the categories backing field.
        /// </summary>
        public Categories categories { get { return _categories; } }
        /// <summary>
        /// Gets the event backing field.
        /// </summary>
        public Events events { get { return _events; } }


        // -------------------------------------------------------------------
        // Constructor (existing calendar/ new calendar ... must specify file)
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates HomeCalendar object from file passed in.
        /// </summary>
        /// <param name="calendarFileName">File path to create the HomeCalendar object.</param>
        /// /// <example>
        /// <code>
        ///     <![CDATA[
        ///      HomeCalendar homeCalendar = new HomeCalendar("calendar.db");
        /// ]]>
        /// </code>
        /// </example>
        public HomeCalendar(String calendarFileName)
        {

            // ====================================================================
            // If the filename is null or empty, an exception is thrown.
            // It is commented out since we are already checking for null or empty filename in the Database method
            // ====================================================================
            //if (string.IsNullOrEmpty(calendarFileName))
            //    throw new ArgumentNullException("Filename passed as the argument is null or empty!");

            // ====================================================================
            // If the database file does not exist, a new database file is created and the categories are added
            // ====================================================================
            if (!File.Exists(calendarFileName))
            {
                Database.NewDatabase(calendarFileName);
                _con = Database.dbConnection;
                _categories = new Categories(Database.dbConnection, true);
                _events = new Events(Database.dbConnection);
            }

            // ====================================================================
            // If the database file exists, the existing database is accessed and categories object is made
            // ====================================================================
            else
            {
                Database.existingDatabase(calendarFileName);
                _con = Database.dbConnection;
                _categories = new Categories(Database.dbConnection, false);
                _events = new Events(Database.dbConnection);

            }
        }

        #region GetList



        // ============================================================================
        // Get all events list
        // ============================================================================

        /// <summary>
        /// Retrieves a list of calendar items within the specified time frame. If filter flag is true will filter for only Items with specified category Id.
        /// Start and end time is inclusive.
        /// </summary>
        /// <param name="Start">Earliest time for the item.</param>
        /// <param name="End">Latest time for the item.</param>
        /// <param name="FilterFlag">If true will filter by value passed in CategoryID, if false will nt filter by category.</param>
        /// <param name="CategoryID">Category Id that the items must be in to be returned in the list, if FilterFlag is true. </param>
        /// <returns>List of CalendarITems within the parameters sent in.</returns>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     string database = "./database.db"
        ///     HomeCalendar hc1 = new HomeCalendar(database);
        ///     List<CalendarItem> itemList =  hc1.GetCalendarItems(null, null, false , 11);
        ///     foreach (CalendarITems item in itemList)
        ///     {
        ///        Console.WriteLine($"Category: {item.Category}, Busy Time {item.BusyTime}");
        ///     }
        ///     ]]>
        /// </code>
        /// Sample output:
        /// <code>
        ///     Category: Fun, Busy Time 40
        ///     Category: Work, Busy Time 100
        ///     Category: Work, Busy Time 115
        ///     Category: Canadian Holidays, Busy Time 1555
        ///     Category: Vacation, Busy Time 2995
        ///     Category: Vacation, Busy Time 4435
        ///     Category: Birthdays, Busy Time 5875
        ///     Category: On call, Busy Time 6055
        /// </code>
        /// </example>
        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            string startstring = Start?.ToString("yyyy-MM-dd H:mm:ss");
            string endstring = End?.ToString("yyyy-MM-dd H:mm:ss");


            Double totalBusyTime = 0;

            using var cmd = new SQLiteCommand(_con);
            List<CalendarItem> newList = new List<CalendarItem>();
            cmd.CommandText = "SELECT e.*, c.Description, c.TypeId FROM Events e INNER JOIN categories c ON e.CategoryId = c.Id WHERE e.StartDateTime <= @end AND e.StartDateTime >= @start ORDER BY e.StartDateTime";
            cmd.Parameters.Add(new SQLiteParameter("@end", endstring));
            cmd.Parameters.Add(new SQLiteParameter("@start", startstring));

            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                if (FilterFlag && CategoryID != rdr.GetInt32(4))
                {
                    continue;
                }
                double duration = rdr.GetDouble(2);
                // If the category is of type availability, it should not counted as busy time 
                //if ((Category.CategoryType)rdr.GetInt32(6) != Category.CategoryType.Availability)
                totalBusyTime += duration;

                newList.Add(new CalendarItem
                {
                    CategoryID = rdr.GetInt32(4),
                    EventID = rdr.GetInt32(0),
                    ShortDescription = rdr.GetString(3),
                    StartDateTime = DateTime.Parse(rdr.GetString(1)),
                    DurationInMinutes = duration,
                    Category = rdr.GetString(5),
                    BusyTime = totalBusyTime
                });
            }
            return newList;
        }

        // ============================================================================
        // Group all events month by month (sorted by year/month)
        // returns a list of CalendarItemsByMonth which is 
        // "year/month", list of calendar items, and totalBusyTime for that month
        // ============================================================================
        /// <summary>
        /// Retrieves a list of calendar items grouped by month from the database within the specified time frame, returns a list of month, items, and total busy time for each month.
        /// If filter flag is true will filter for only Items with specified category Id. Start and end time is inclusive.
        /// </summary>
        /// <param name="Start">Earliest time for the item.</param>
        /// <param name="End">Latest time for the item.</param>
        /// <param name="FilterFlag">If true will filter by value passed in CategoryID, if false will nt filter by category.</param>
        /// <param name="CategoryID">Category Id that the items must be in to be returned in the list, if FilterFlag is true. </param>
        /// <returns>List of CalendarItemsByMonth organized by category within the given parameters.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///        HomeCalendar homeCalendar = new HomeCalendar("calendar.db");
        ///        List<CalendarItemsByMonth> itemListByMonth = hc1.GetCalendarItemsByMonth(null, null, false, 11);
        ///        foreach (CalendarItemsByMonth month in itemListByMonth)
        ///        {
        ///            Console.WriteLine(month.Month);
        ///            foreach (CalendarItem item in month.Items)
        ///            {
        ///                Console.WriteLine($"Category: {item.Category}, Busy Time {item.BusyTime}");
        ///            }
        ///            Console.WriteLine();
        ///        }
        ///        ]]>
        /// </code>
        /// Sample output:
        /// <code>
        ///     2018/01
        ///     Category: Fun, Busy Time 40
        ///     Category: Work, Busy Time 100
        ///     Category: Work, Busy Time 115
        ///
        ///     2020/01
        ///     Category: Canadian Holidays, Busy Time 1555
        ///     Category: Vacation, Busy Time 2995
        ///     Category: Vacation, Busy Time 4435
        ///     Category: Birthdays, Busy Time 5875
        ///     Category: On call, Busy Time 6055
        /// </code>
        /// </example>
        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            string startstring = Start?.ToString("yyyy-MM-dd H:mm:ss");
            string endstring = End?.ToString("yyyy-MM-dd H:mm:ss");

            List<CalendarItemsByMonth> calendarItemsByMonths = new List<CalendarItemsByMonth>();

            // ------------------------------------------------------------------------
            // Establishing connection to the database and running the query to retrieve
            // calendar items grouped by month
            // ------------------------------------------------------------------------
            using var cmd = new SQLiteCommand(_con);
            if (FilterFlag)
            {
                cmd.CommandText = "SELECT e.*, c.Description, c.TypeId " +
                "FROM Events e INNER JOIN categories c " +
                "ON e.CategoryId = c.Id " +
                "WHERE e.StartDateTime <= @end AND e.StartDateTime >= @start AND e.CategoryId = @catId " +
                "ORDER BY e.StartDateTime";
                cmd.Parameters.Add(new SQLiteParameter("@end", endstring));
                cmd.Parameters.Add(new SQLiteParameter("@start", startstring));
                cmd.Parameters.Add(new SQLiteParameter("@catId", CategoryID));
            }
            else
            {

                cmd.CommandText = "SELECT e.*, c.Description, c.TypeId " +
                    "FROM Events e INNER JOIN categories c " +
                    "ON e.CategoryId = c.Id " +
                    "WHERE e.StartDateTime <= @end AND e.StartDateTime >= @start " +
                    "ORDER BY e.StartDateTime";
                cmd.Parameters.Add(new SQLiteParameter("@end", endstring));
                cmd.Parameters.Add(new SQLiteParameter("@start", startstring));
            }

            DateTime currentStartDate;
            string currentMonthYear;
            DateTime nextStartDate;
            string nextMonthYear;
            int currentCatId;

            // ------------------------------------------------------------------------
            // Running the query command
            // ------------------------------------------------------------------------
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                // ------------------------------------------------------------------------
                // Reading the first record from the output read from the database
                // Initialising a list for the current month and setting total busy time for this month to 0
                // ------------------------------------------------------------------------
                currentStartDate = DateTime.Parse(rdr.GetString(1));
                currentMonthYear = currentStartDate.Year.ToString("D4") + "/" + currentStartDate.Month.ToString("D2");
                List<CalendarItem> monthlyList = new List<CalendarItem>();
                Double totalBusyTime = 0;

                do
                {
                    // ------------------------------------------------------------------------
                    // Saving the next year/month
                    // ------------------------------------------------------------------------
                    nextStartDate = DateTime.Parse(rdr.GetString(1));
                    nextMonthYear = nextStartDate.Year.ToString("D4") + "/" + nextStartDate.Month.ToString("D2");







                    // ------------------------------------------------------------------------
                    // If the next year/month is not the same year/month, the monthly list is added to the
                    // list of calendar items by month.
                    // ------------------------------------------------------------------------
                    if (currentMonthYear != nextMonthYear)
                    {

                        calendarItemsByMonths.Add(new CalendarItemsByMonth
                        {
                            Month = currentMonthYear,
                            Items = monthlyList,
                            TotalBusyTime = totalBusyTime
                        });

                        // ------------------------------------------------------------------------
                        // List of monthly items and total busy time are reset. Current month is changed to next.
                        // ------------------------------------------------------------------------
                        monthlyList = new List<CalendarItem>();
                        currentMonthYear = nextMonthYear;
                        totalBusyTime = 0;

                    }

                    // ------------------------------------------------------------------------
                    // If the current year/month and next year/month are equal, busy time is increased.
                    // If the category is of type availability, it should not counted as busy time.
                    // ------------------------------------------------------------------------ 
                    double currentDuration = rdr.GetDouble(2);
                    if ((Category.CategoryType)rdr.GetInt32(6) != Category.CategoryType.Availability)
                        totalBusyTime += currentDuration;

                    // ------------------------------------------------------------------------
                    // If the current year/month and next year/month are equal, new calendar item is added to the list.
                    // ------------------------------------------------------------------------
                    monthlyList.Add(new CalendarItem
                    {
                        CategoryID = rdr.GetInt32(4),
                        EventID = rdr.GetInt32(0),
                        ShortDescription = rdr.GetString(3),
                        StartDateTime = nextStartDate,
                        DurationInMinutes = currentDuration,
                        Category = rdr.GetString(5),
                        BusyTime = totalBusyTime
                    });

                } while (rdr.Read());

                calendarItemsByMonths.Add(new CalendarItemsByMonth
                {
                    Month = currentMonthYear,
                    Items = monthlyList,
                    TotalBusyTime = totalBusyTime
                });




            }

            return calendarItemsByMonths;
        }

        // ============================================================================
        // Group all events by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Retrieves a list of calendar items grouped by category within the specified time frame, returns a list of category, items, and total busy time for each month.
        /// If filter flag is true will filter for only Items with specified category Id. Start and end time is inclusive.
        /// </summary>
        /// <param name="Start">Earliest time for the item.</param>
        /// <param name="End">Latest time for the item.</param>
        /// <param name="FilterFlag">If true will filter by value passed in CategoryID, if false will nt filter by category.</param>
        /// <param name="CategoryID">Category Id that the items must be in to be returned in the list, if FilterFlag is true. </param>
        /// <returns>List of CalendarItemsByCategory organized by category within the given parameters.</returns>
        /// <example>
        /// <code>
        ///     <![CDATA[
        ///     string calendardb = "./category.db";
        ///     HomeCalendar hc1 = new HomeCalendar(calendardb);
        ///     List<CalendarItemsByCategory> itemListByCategory = hc1.GetCalendarItemsByCategory(null, null, false, 11);
        ///     foreach (CalendarItemsByCategory cat in itemListByCategory)
        ///     {
        ///        Console.WriteLine(cat.Category);
        ///        foreach (CalendarItem item in cat.Items) 
        ///        {
        ///          Console.WriteLine($"Date: {item.StartDateTime}, Busy Time {item.BusyTime}");
        ///        } 
        ///     }
        ///     ]]>
        /// </code>
        /// Sample output:
        /// <code>
        ///     Birthdays
        ///     Date: 2020-01-12 12:00:00 AM, Busy Time 5875
        ///
        ///     Canadian Holidays
        ///     Date: 2020-01-01 12:00:00 AM, Busy Time 1555
        ///
        ///     Fun
        ///     Date: 2018-01-10 10:00:00 AM, Busy Time 40
        ///
        ///     On call
        ///     Date: 2020-01-20 11:00:00 AM, Busy Time 6055
        ///
        ///     Vacation
        ///     Date: 2020-01-09 12:00:00 AM, Busy Time 2995
        ///     Date: 2020-01-10 12:00:00 AM, Busy Time 4435
        ///
        ///     Work
        ///     Date: 2018-01-11 10:15:00 AM, Busy Time 100
        ///     Date: 2018-01-11 7:30:00 PM, Busy Time 115
        /// </code>
        /// </example>
        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            Start ??= new DateTime(1900, 1, 1);
            End ??= new DateTime(2500, 1, 1);

            string startString = Start?.ToString("yyyy-MM-dd H:mm:ss");
            string endString = End?.ToString("yyyy-MM-dd H:mm:ss");

            //Initializes a list for the results, a list of CalendarItemsByCategory
            List<CalendarItemsByCategory> resultList = new List<CalendarItemsByCategory>();

            // ------------------------------------------------------------------------
            // Establishing connection to the database and running the query to retrieve
            // calendar items from the Database
            // ------------------------------------------------------------------------

            using var cmd = new SQLiteCommand(_con);
            if (FilterFlag)
            {
                cmd.CommandText = "SELECT e.*, c.Description, c.TypeId FROM Events e INNER JOIN categories c ON e.CategoryId = c.Id WHERE e.StartDateTime <= @end AND e.StartDateTime >= @start AND e.CategoryId = @catId ORDER BY c.Description, e.StartDateTime";
                cmd.Parameters.Add(new SQLiteParameter("@end", endString));
                cmd.Parameters.Add(new SQLiteParameter("@start", startString));
                cmd.Parameters.Add(new SQLiteParameter("@catId", CategoryID));
            }
            else
            {

                cmd.CommandText = "SELECT e.*, c.Description, c.TypeId FROM Events e INNER JOIN categories c ON e.CategoryId = c.Id WHERE e.StartDateTime <= @end AND e.StartDateTime >= @start ORDER BY c.Description, e.StartDateTime";

                // Add parameters to the SQL command for start and end dates
                cmd.Parameters.Add(new SQLiteParameter("@end", endString));
                cmd.Parameters.Add(new SQLiteParameter("@start", startString));
            }

            using SQLiteDataReader rdr = cmd.ExecuteReader();


            //To check for rows in the result of the query
            if (rdr.Read())
            {
                //The category descripiton for the first row which will be used to compare Categories
                string currentCategory = rdr.GetString(5);
                int currentCategoryID;
                //Initializes a list to store CalendarItems for the current category
                List<CalendarItem> categoryItems = new List<CalendarItem>();
                //Initializes a busy time variable for the current category
                double totalBusyTime = 0;
                do
                {
                    //Retrieves the next rows CategoryDescription on the first iteration the current and next categoryDescription will be the same
                    string nextCategoryDescription = rdr.GetString(5);


                    //Checks if the next category does not equal the current one, if its not it means theres no more calendar items left for that category
                    //and if it is it skips the following if statement and it adds the next set of CalendarItems to the list of CalItems for the current category
                    if (nextCategoryDescription != currentCategory)
                    {
                        // Add a new CalendarItemsByCategory object to the result list for the current category
                        resultList.Add(new CalendarItemsByCategory
                        {
                            Category = currentCategory,
                            Items = categoryItems,
                            TotalBusyTime = totalBusyTime
                        });
                        //Resets the list of calendar items and the total busy time so that it can be used for  the next Category
                        categoryItems = new List<CalendarItem>();
                        totalBusyTime = 0;
                        //Updates the current Category to the next one so that we can continue to compare them
                        currentCategory = nextCategoryDescription;
                    }


                    double duration = rdr.GetDouble(2);

                    //Events that are not used to show availability
                    if ((Category.CategoryType)rdr.GetInt32(6) != Category.CategoryType.Availability)
                    {
                        // Accumulate the busy time for the current calendar item to calculate total busy time for the current category
                        totalBusyTime += duration;
                    }
                    //Creates and adds a calendar item to the list of calendar items for the current category
                    categoryItems.Add(new CalendarItem
                    {
                        CategoryID = rdr.GetInt32(4),
                        EventID = rdr.GetInt32(0),
                        ShortDescription = rdr.GetString(3),
                        StartDateTime = DateTime.Parse(rdr.GetString(1)),
                        DurationInMinutes = duration,
                        Category = currentCategory,
                        BusyTime = totalBusyTime
                    });
                } while (rdr.Read());

                resultList.Add(new CalendarItemsByCategory
                {
                    Category = currentCategory,
                    Items = categoryItems,
                    TotalBusyTime = totalBusyTime
                });


            }
            //Returns the result list
            return resultList;
        }



        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects with:
        //          one dictionary object per month,
        //          and one dictionary object for the category total busy times
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <name of month>
        //           "TotalBusyTime", <the total durations for the month>
        //             for each category for which there is an event in the month:
        //             "items:category", a List<CalendarItem>
        //             "category", the total busy time for that category for this month
        // The one dictionary for the category total busy times has the following key value pairs:
        //             for each category for which there is an event in ANY month:
        //             "category", the total busy time for that category for all the months
        // ============================================================================

        /// <summary>
        /// Gets the Calendar Items organized by month, then category within each month, returns a list of dictionaries for each month and total.
        /// If filter flag is true will filter for only Items with specified category Id. Start and end time is inclusive.
        /// </summary>
        /// <param name="Start">Earliest time for the item.</param>
        /// <param name="End">Latest time for the item.</param>
        /// <param name="FilterFlag">If true, will filter by value passed in CategoryID, if false, will not filter by category.</param>
        /// <param name="CategoryID">Category Id that the items must be in to be returned in the list, if FilterFlag is true. </param>
        /// <returns>Returns a list of dictionaries for each month and total.</returns>
        /// <example>
        /// <code>
        ///        <![CDATA[
        ///        string file = "./test.calendar";
        ///        HomeCalendar hc1 = new HomeCalendar();
        ///        hc1.ReadFromFile(file);
        ///        List<Dictionary<string, object>> data = hc1.GetCalendarDictionaryByCategoryAndMonth(null, null, false, 11);
        ///        foreach (Dictionary<string, object> dict in data)
        ///        {
        ///            foreach (KeyValuePair<string, object> category in dict)
        ///            {
        ///               if (!category.Key.Contains("items"))
        ///               {
        ///
        ///                    Console.WriteLine($"{category.Key}: {category.Value}");
        ///               }
        ///            }
        ///            Console.WriteLine();
        ///        }
        ///        ]]>
        /// </code>
        ///  Sample output:
        /// <code>
        ///     Month: 2018/01
        ///     TotalBusyTime: 115
        ///     Fun: 40
        ///     Work: 75
        ///
        ///     Month: 2020/01
        ///     TotalBusyTime: 5940
        ///     Birthdays: 1440
        ///     Canadian Holidays: 1440
        ///     On call: 180
        ///     Vacation: 2880
        ///
        ///     Month: TOTALS
        ///     Work: 75
        ///     Fun: 40
        ///     On call: 180
        ///     Canadian Holidays: 1440
        ///     Vacation: 2880
        ///     Birthdays: 1440
        /// </code>
        /// </example>
        public List<Dictionary<string, object>> GetCalendarDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<CalendarItemsByMonth> GroupedByMonth = GetCalendarItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalBusyTimePerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["TotalBusyTime"] = MonthGroup.TotalBusyTime;

                // break up the month items into categories
                var GroupedByCategory = MonthGroup.Items.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of items
                    double totalCategoryBusyTimeForThisMonth = 0;
                    var details = new List<CalendarItem>();

                    foreach (var item in CategoryGroup)
                    {
                        totalCategoryBusyTimeForThisMonth = totalCategoryBusyTimeForThisMonth + item.DurationInMinutes;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["items:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;

                    // keep track of totals for each category
                    if (totalBusyTimePerCategory.TryGetValue(CategoryGroup.Key, out Double currentTotalBusyTimeForCategory))
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = currentTotalBusyTimeForCategory + totalCategoryBusyTimeForThisMonth;
                    }
                    else
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalBusyTimePerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
