using Calendar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CalendarWPF
{
    /// <summary>
    /// Interaction logic for AddEventWindow.xaml
    /// </summary>
    public partial class AddEventWindow : Window, EventViewInterface
    {
        private Window _mw;
        private int eventId = 0;
        public AddEventWindow(Color selectedBackgroundColor, Color selectedTextColor, Window mw, bool IsUpdate, object sender)
        {
            InitializeComponent();

            DisplayEventWindow();
            if (IsUpdate)
            {
                if(sender is DataGrid)
                {
                    UpdateEventInitialization(sender);
                }
                else
                {
                    MenuItemInitialization(sender);
                }
                
            }
            else
            {
                AddEventWindowInitialization();
            }

            _mw = mw;
            this.Background = new SolidColorBrush(selectedBackgroundColor);
            this.Foreground = new SolidColorBrush(selectedTextColor);
        }
        public void UpdateEventInitialization(object sender)
        {
            var dataGrid = (DataGrid)sender;
            CalendarItem calendarItem;
            if (dataGrid.SelectedItem != null)
            {
                calendarItem = dataGrid.SelectedItem as CalendarItem;
                eventId = calendarItem.EventID;

                int index = -1;
                foreach (Category cat in Category_ComboBox.ItemsSource)
                {
                    index = index + 1;
                    if (cat.Id == calendarItem.CategoryID)
                    {
                        Category_ComboBox.SelectedIndex = index;
                    }
                }

                dpDate.Value = calendarItem.StartDateTime;
                desc.Text = calendarItem.ShortDescription;
                duration.Text = calendarItem.DurationInMinutes.ToString();
            }

            Header.Text = "Modify Event";
            FirstButton.Content = "Update Event";
            SecondButton.Content = "Delete Event";

        }
        public void MenuItemInitialization(object sender)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            // Retrieve the ContextMenu to which the MenuItem belongs
            var contextMenu = menuItem.Parent as ContextMenu;
            if (contextMenu == null) return;

            // Retrieve the DataGrid from the PlacementTarget property of the ContextMenu
            var dataGrid = contextMenu.PlacementTarget as DataGrid;
            if (dataGrid == null || dataGrid.SelectedItem == null) return;

            // Now, safely cast the selected item to CalendarItem
            var calendarItem = dataGrid.SelectedItem as CalendarItem;
            if (calendarItem == null) return;

            eventId = calendarItem.EventID;

            int index = -1;
            foreach (Category cat in Category_ComboBox.ItemsSource)
            {
                index++;
                if (cat.Id == calendarItem.CategoryID)
                {
                    Category_ComboBox.SelectedIndex = index;
                    break;
                }
            }

            dpDate.Value = calendarItem.StartDateTime;
            desc.Text = calendarItem.ShortDescription;
            duration.Text = calendarItem.DurationInMinutes.ToString();

            Header.Text = "Modify Event";
            FirstButton.Content = "Update Event";
            SecondButton.Content = "Delete Event";

        }
        public void AddEventWindowInitialization()
        {
            dpDate.Value = DateTime.Now;
            Header.Text = "Create an event";
            FirstButton.Content = "Save Event";
            SecondButton.Content = "Cancel Event";

        }
        public void DisplayEventWindow()
        {
            List<Category> catList = Presenter.GetCategories();

            Category newcat = new Category(0, "Add a new category", Category.CategoryType.Event);
            catList.Add(newcat);

            catList = catList.OrderBy(cat => cat.Description).ToList();  // Assuming 'Description' is the property to sort by

            Category_ComboBox.ItemsSource = catList;

            Category_Types_ComboBox.ItemsSource = Enum.GetValues(typeof(Category.CategoryType));
            //foreach (Category category in catList)
            //{
            //    Category_ComboBox.Items.Add(category);
            //}
            //foreach (Category.CategoryType categoryType in Enum.GetValues(typeof(Category.CategoryType)))
            //{
            //    Category_Types_ComboBox.Items.Add(categoryType);
            //}
            Presenter.SetEventView(this);
        }

        private void Category_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            result_text.Visibility = Visibility.Collapsed;
            result_text.Text = "";
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.SelectedItem is Category item)
            {
                if (item.Id == 0)
                    New_Category_Panel.Visibility = Visibility.Visible;

                else
                    New_Category_Panel.Visibility = Visibility.Collapsed;
            }
        }

        private void Add_Category_Button_Click(object sender, RoutedEventArgs e)
        {
            result_text.Visibility = Visibility.Visible;
            result_text.Text = "";
            AddCategory();
        }

        public void AddCategory()
        {
            if (Presenter.ValidateCategory(cat_description.Text, (Category.CategoryType?)Category_Types_ComboBox.SelectedItem))
            {
                Made_New_Category.Text = $"Added {cat_description.Text} to Categories";
                Category_Types_ComboBox.SelectedItem = null;
                cat_description.Text = "";
                UpdateCategories();
                //ComboBoxItem addNewCategoryItem = new ComboBoxItem
                //{
                //    Name = "New",
                //    Content = "Add a new category"
                //};
                //Category_ComboBox.Items.Add(addNewCategoryItem);
                //Category_ComboBox.SelectedItem = addNewCategoryItem;
            }
        }

        private void Btn_SaveForm(object sender, RoutedEventArgs e)
        {
            if (FirstButton.Content == "Save Event")
            {
                SaveEvent();
            }
            else
            {
                UpdateEvent();
            }
        }

        public void UpdateEvent()
        {
            if (Presenter.IsValidUpdate(eventId, dpDate.Value.ToString(), (Category)Category_ComboBox.SelectedItem, duration.Text, desc.Text, New_Category_Panel.Visibility))
                this.Close();
        }

        public void SaveEvent()
        {
            if (Category_ComboBox.SelectedItem is Category cat && cat.Description != "Add a new category")
            {
                if (Presenter.IsSaved(dpDate.Value.ToString(), desc.Text, duration.Text, (Category)Category_ComboBox.SelectedItem, New_Category_Panel.Visibility))
                {
                    result_text.Visibility = Visibility.Visible;
                    result_text.Text = "Event saved";

                    dpDate.Value = DateTime.Now;
                    New_Category_Panel.Visibility = Visibility.Collapsed;
                    desc.Text = "";
                    duration.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Please select a valid category from the dropdown.", "No category selected", MessageBoxButton.OK);

            }

        }

        private void Btn_CancelForm(object sender, RoutedEventArgs e)
        {
            if (SecondButton.Content == "Cancel Event")
            {
                CancelSavingEvent();
            }
            else
            {
                DeleteEvent();
            }
        }
        public void DeleteEvent()
        {
            Presenter.DeleteEvent(eventId);
            this.Close();
        }
        public void CancelSavingEvent()
        {
            Presenter.Cancel((Category)Category_ComboBox.SelectedItem, desc.Text, duration.Text, this);
        }
        private void Close_button_Clicked(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        public void CloseApplication()
        {
            this.Close();
            _mw.Close();
        }
        public void CancelAddEvent()
        {
            this.Close();
 
        }

        public void MessageBoxOK(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        public void MessageBoxYesNo(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.YesNo);
        }

        public void UpdateCategories()
        {
            List<Category> catList = Presenter.GetCategories();

            Category newcat = new Category(0, "Add a new category", Category.CategoryType.Event); 
            catList.Add(newcat);
 
            catList = catList.OrderBy(cat => cat.Description).ToList();  // Assuming 'Description' is the property to sort by

            Category_ComboBox.ItemsSource = catList;
            //Category_ComboBox.Items.Clear();  // Clear existing items before adding new ones
            //ComboBoxItem addNewCategoryItem = new ComboBoxItem
            //{
            //    Name = "New",
            //    Content = "Add a new category"
            //};
            //Category_ComboBox.Items.Add(addNewCategoryItem);
            //foreach (Category category in catList)
            //{
            //    // Check if the category already exists in the ComboBox
            //    if (!Category_ComboBox.Items.Contains(category))
            //    {
            //        // Add the category to the ComboBox
            //        Category_ComboBox.Items.Add(category);
            //    }
            //}
            Category_ComboBox.SelectedItem = newcat;
        }

    }
}
