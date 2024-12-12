using Microsoft.Win32;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Calendar;
using System.Windows.Input;
using System;

namespace CalendarWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, HomePageViewInterface
    {
        private bool _allowUpdate = false; // Indicates whether updating events is allowed.
        private Color _backColor = Colors.Cornsilk;
        private Color _foreColor = Colors.Black;
        
        public MainWindow()
        {
            InitializeComponent();
            BackGroundColorComboBox.ItemsSource = typeof(Colors).GetProperties();
            TextColorComboBox.ItemsSource = typeof(Colors).GetProperties();
            BackGroundColorComboBox.SelectedIndex=18;
            TextColorComboBox.SelectedIndex=7;
            SetBackgroundAndForegroundColor();

        }

        public void PopulateGrid(DateTime? startDate = null, DateTime? endDate = null, int id = -1)
        {
            
            List<CalendarItem> items = Presenter.GetCalendarItems(startDate, endDate, id);
            myDataGrid.ItemsSource = items;
            myDataGrid.Columns.Clear();                       // Clear all existing columns on the DataGrid control.
            var column = new DataGridTextColumn();
            var column2 = new DataGridTextColumn();
            var column3 = new DataGridTextColumn();
            var column4 = new DataGridTextColumn();
            var column5 = new DataGridTextColumn();

            column.Header = "Category";
            column2.Header = "Duration";
            column3.Header = "Description";
            column4.Header = "Start Date";
            column5.Header = "Busy Time";
            

            column.Binding = new Binding("Category");            // Bind to an object propery         
            column2.Binding = new Binding("DurationInMinutes");            // Bind to an object propery         
            column3.Binding = new Binding("ShortDescription");            // Bind to an object propery         
            column4.Binding = new Binding("StartDateTime");            // Bind to an object propery         
            column5.Binding = new Binding("BusyTime");            // Bind to an object propery         



            myDataGrid.Columns.Add(column);             // Add the defined column to the DataGrid
            myDataGrid.Columns.Add(column2);             // Add the defined column to the DataGrid
            myDataGrid.Columns.Add(column3);             // Add the defined column to the DataGrid

            myDataGrid.Columns.Add(column4);
            myDataGrid.Columns.Add(column5);

            // create a new style
            Style rightAligned = new Style();
            rightAligned.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // set the population column to right aligned
            column.CellStyle = rightAligned;
            column2.CellStyle = rightAligned;
            column3.CellStyle = rightAligned;
            column4.CellStyle = rightAligned;

            _allowUpdate = true; // Indicates whether updating events is allowed.
                                 // Create and add menu items
            
            //Readds the context menu adter unchecking the category or month checkboxes
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem { Header = "Update" };
            MenuItem menuItem2 = new MenuItem { Header = "Delete" };
            // Optionally, add click event handlers for menu items
            menuItem1.Click += btn_UpdateEvent;
            menuItem2.Click += DeleteEvent_Click;
            // Add menu items to the context menu
            contextMenu.Items.Add(menuItem1);
            contextMenu.Items.Add(menuItem2);

            // Assign the context menu to the DataGrid
            myDataGrid.ContextMenu = contextMenu;

            search.Visibility = Visibility.Visible;
        }

        //private void File_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    file_button.Visibility = Visibility.Collapsed;            
        //    file_combobox.Visibility = Visibility.Visible;
        //    file_combobox.IsEnabled = true;
        //}

        public void MessageBoxOK(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }



        private void btn_UpdateEvent(object sender, RoutedEventArgs e)
        {
            if(!_allowUpdate) // Exit the method if updates are not allowed, more specifically in our program when the grid is displayed by category or month
            {
                return;
            }

            AddEventWindow sw = new AddEventWindow(_backColor, _foreColor, this, true, sender);
            sw.Show();
        }
        private void btn_DarkMode(object sender, RoutedEventArgs e)
        {
            if (_backColor != Colors.Black && _foreColor != Colors.LightBlue)
            {
                SetDarkMode();
            }
            else
            {
                SetLightMode();
            }
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var startDate = StartDate.SelectedDate;
            var endDate = EndDate.SelectedDate;
            PopulateGrid(startDate, endDate);
        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var startDate = StartDate.SelectedDate;
            var endDate = EndDate.SelectedDate;
            PopulateGrid(startDate, endDate);
        }
        private void SetLightMode()
        {
            DarkModeButton.Content=" Dark Mode";
            _backColor=Colors.Cornsilk;
            _foreColor=Colors.Black;
            BackGroundColorComboBox.SelectedIndex=18;
            TextColorComboBox.SelectedIndex=7;
            SetBackgroundAndForegroundColor();

        }
        private void SetDarkMode()
        {
            DarkModeButton.Content=" Light Mode";
            _backColor=Colors.Black;
            _foreColor=Colors.Cornsilk;
            BackGroundColorComboBox.SelectedIndex=7;
            TextColorComboBox.SelectedIndex=18;
            SetBackgroundAndForegroundColor();
        }
        private void SetBackgroundAndForegroundColor()
        {
            this.Foreground = new SolidColorBrush(_foreColor);
            this.Background = new SolidColorBrush(_backColor);
     
            myDataGrid.Background = new SolidColorBrush(_backColor);
            
        }

        //private void File_Selected(object sender, RoutedEventArgs e)
        //{
        //    if (file_combobox.SelectedItem == new_file)
        //    {
        //        file_name.Visibility = Visibility.Visible;
        //        file_name_input_box.Visibility = Visibility.Visible;
        //        create_button.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        string filename;
        //        OpenFileDialog dlg = new OpenFileDialog();
        //        dlg.Filter = "Database Files (*.db) |*.db|All Files (*.*)|*.*";
        //        dlg.InitialDirectory = @"c:\";

        //        Nullable<bool> result = dlg.ShowDialog();

        //        if (result == true)
        //        {
        //            filename = dlg.FileName;
        //            OpenExistingDatabase(filename);

        //        }
        //    }
        //}

        public void OpenExistingDatabase(string filename)
        {
            // Open document
            bool succeededConnect = Presenter.Connect(filename, false, this);
            //file_combobox.Visibility = Visibility.Collapsed;
            //file_combobox.IsEnabled = false;
            //file_button.Visibility = Visibility.Visible;
            //string[] strings = filename.Split("\\");
            //file_button.Content = strings[strings.Length - 1];
            if (succeededConnect)
            {
                add_event.IsEnabled = true;
                Category_Checkbox.IsEnabled = true;
                Month_Checkbox.IsEnabled = true;
                StartDate.IsEnabled = true;//enables the datepickers
                EndDate.IsEnabled = true;
                UpdateCategories();
            }
            

        }
        private void Add_Event_btn_Clicked(object sender, RoutedEventArgs e)
        {
            //file_button.IsEnabled = false;
            
            AddEventWindow sw = new AddEventWindow(_backColor, _foreColor, this, false, null);
            sw.Show();
        }

        private void cmbColors_SelectionChangedBackgroudColor(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _backColor = (Color)(BackGroundColorComboBox.SelectedItem as PropertyInfo).GetValue(null, null);
            this.Background = new SolidColorBrush(_backColor);
            myDataGrid.Background = new SolidColorBrush(_backColor);
            

        }

        private void cmbColors_SelectionChangedTextColor(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _foreColor = (Color)(TextColorComboBox.SelectedItem as PropertyInfo).GetValue(null, null);
            this.Foreground = new SolidColorBrush(_foreColor);
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filename = file_name_input_box.Text + ".db";
            if (filename != "")
            {
                OpenNewDatabase(filename);
                file_opened.Text = filename;
            }
            else
            {
                MessageBox.Show("Please enter a valid filename", "Invalid file name", MessageBoxButton.OK);
            }
        }

        public void OpenNewDatabase(string filename)
        {
            bool succeededConnet = Presenter.Connect(filename, true, this);
            if (succeededConnet)
            {
                file_name.Visibility = Visibility.Collapsed;
                file_name_input_box.Visibility = Visibility.Collapsed;
                create_button.Visibility = Visibility.Collapsed;
                //file_combobox.Visibility = Visibility.Collapsed;
                //file_combobox.IsEnabled = false;
                //file_button.Visibility = Visibility.Visible;
                //file_button.Content = filename;
                add_event.IsEnabled = true;
                Category_Checkbox.IsEnabled = true;
                Month_Checkbox.IsEnabled = true;
                StartDate.IsEnabled = true;//enables the datepickers
                EndDate.IsEnabled = true;
                UpdateCategories();
            }
            
        }

        private void CategoryCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        public void DisplayItemsByCategory(List<CalendarItemsByCategory> items)
        {
            myDataGrid.Columns.Clear();
            myDataGrid.ItemsSource = null;
            myDataGrid.ItemsSource = items;
            myDataGrid.Columns.Clear();                       // Clear all existing columns on the DataGrid control.
            var column = new DataGridTextColumn();
            var column2 = new DataGridTextColumn();


            column.Header = "Category";
            column2.Header = "BusyTime";



            column.Binding = new Binding("Category");            // Bind to an object propery         
            column2.Binding = new Binding("TotalBusyTime");            // Bind to an object propery         

            myDataGrid.Columns.Add(column);             // Add the defined column to the DataGrid
            myDataGrid.Columns.Add(column2);             // Add the defined column to the DataGrid

            // create a new style
            Style rightAligned = new Style();
            rightAligned.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // set the population column to right aligned
            column.CellStyle = rightAligned;
            column2.CellStyle = rightAligned;

            _allowUpdate = false; // Disallow updates if the grid is displayed by category
            myDataGrid.ContextMenu = null; //removes the context menu if displayed by category


        }


        public void DisplayItemsByMonth(List<CalendarItemsByMonth> items)
        {
            myDataGrid.Columns.Clear();
            myDataGrid.ItemsSource = null;
            myDataGrid.ItemsSource = items;
            myDataGrid.Columns.Clear();                       // Clear all existing columns on the DataGrid control.
            var column = new DataGridTextColumn();
            var column2 = new DataGridTextColumn();


            column.Header = "Month";
            column2.Header = "BusyTime";



            column.Binding = new Binding("Month");            // Bind to an object propery         
            column2.Binding = new Binding("TotalBusyTime");            // Bind to an object propery         

            myDataGrid.Columns.Add(column);             // Add the defined column to the DataGrid
            myDataGrid.Columns.Add(column2);             // Add the defined column to the DataGrid

            // create a new style
            Style rightAligned = new Style();
            rightAligned.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            // set the population column to right aligned
            column.CellStyle = rightAligned;
            column2.CellStyle = rightAligned;


            _allowUpdate = false; // Disallow updates if the grid is displayed by month
            myDataGrid.ContextMenu = null;//removes the context menu if displayed by month or montha nd category

        }

        public void DisplayItemsByCategoryAndMonth(List<Dictionary<string, object>> items)
        {
            myDataGrid.ItemsSource = items;
            myDataGrid.Columns.Clear();

            foreach (var item in items)
            {
                foreach (string key in item.Keys)
                {
                    if (key.Contains("items:"))
                    {
                        // Skip columns with specific headers
                        continue;
                    }

                    // Check if a column with the same header already exists
                    bool columnExists = false;
                    foreach (var existingColumn in myDataGrid.Columns)
                    {
                        if (existingColumn.Header.ToString() == key)
                        {
                            columnExists = true;
                            break;
                        }
                    }

                    // If column with the same header doesn't exist, add a new column
                    if (!columnExists)
                    {
                        var column = new DataGridTextColumn();
                        column.Header = key;
                        column.Binding = new Binding($"[{key}]");
                        myDataGrid.Columns.Add(column);
                        // create a new style
                        Style rightAligned = new Style();
                        rightAligned.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

                        // set the population column to right aligned
                        column.CellStyle = rightAligned;
                    }
                }
            }

        }

        private void UpdateCategories()
        {
            Category_Filter.Items.Clear();
            List<Category> catList = Presenter.GetCategories();
            catList = catList.OrderBy(cat => cat.Description).ToList();  // Assuming 'Description' is the property to sort by
            Category_Filter.Items.Add("None");
            foreach (Category category in catList)
            {
                Category_Filter.Items.Add(category);
            }
        }

        private void Category_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            // Fetch the start and end dates from the DatePicker controls
            var startDate = StartDate.SelectedDate;
            var endDate = EndDate.SelectedDate;
            if (Category_Filter.SelectedItem == null || Category_Filter.SelectedItem == "None") 
            {
                Presenter.CheckFilter((bool)Category_Checkbox.IsChecked, (bool)Month_Checkbox.IsChecked, startDate, endDate);

            }
            else if(Category_Filter.SelectedItem is Category selectedCategory)
            {
                Presenter.CheckFilter((bool)Category_Checkbox.IsChecked, (bool)Month_Checkbox.IsChecked, startDate,endDate, selectedCategory);

            }

        }
        private void myDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        public void UpdateGrid()
        {
            List<CalendarItem> items = Presenter.GetCalendarItems();
            myDataGrid.ItemsSource = items;
            _allowUpdate = true; // Indicates whether updating events is allowed.

        }

        private void New_file_Click(object sender, RoutedEventArgs e)
        {
            file_name.Visibility = Visibility.Visible;
            file_name_input_box.Visibility = Visibility.Visible;
            create_button.Visibility = Visibility.Visible;
            
        }

        private void Existing_file_Click(object sender, RoutedEventArgs e)
        {
            file_name.Visibility = Visibility.Collapsed;
            file_name_input_box.Visibility = Visibility.Collapsed;
            create_button.Visibility = Visibility.Collapsed;
            string filename;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Database Files (*.db) |*.db|All Files (*.*)|*.*";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                filename = dlg.FileName;
                OpenExistingDatabase(filename);
                file_opened.Text = filename;
            }
        }
  
        // Event handler for deleting an event
        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            if (myDataGrid.SelectedItem is CalendarItem selectedEvent)
            {
                // Call the Presenter to delete the event
                Presenter.DeleteEvent(selectedEvent.EventID);
                // Update the grid to reflect the deletion
                PopulateGrid();
            }
        }



        private void Category_Filter_DropDownOpened(object sender, EventArgs e)
        {
            UpdateCategories();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(myDataGrid.ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = o =>
                {
                    var item = o as CalendarItem;
                    if (item == null) return false;
                    return item.ShortDescription.ToLower().Contains(SearchTextBox.Text.ToLower());
                };
            }
        }
    }
}