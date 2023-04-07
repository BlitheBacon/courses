using SQLite;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses.Terms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermDetails : ContentPage
    {
        // Fields
        private readonly SQLiteAsyncConnection _conn;
        private readonly Term _currentTerm;
        private ObservableCollection<Course> _courseList; // Allows for dynamic loading of content

        public TermDetails(Term term)
        {
            InitializeComponent();

            // Assignments
            Title        = term.Title; // Page Title will become the title of the term
            _conn        = DependencyService.Get<ISQLiteDb>().GetConnection();
            _currentTerm = term;

            courseListView.ItemTapped += Course_Tapped;
        }
        
        // Methods
        protected override async void OnAppearing()
        {
            // Populate with selected term information
            TermTitle.Text           = _currentTerm.Title;
            TermDetailStartDate.Text = _currentTerm.StartDate.ToString("MM/dd/yy");
            TermDetailEndDate.Text   = _currentTerm.EndDate.ToString("MM/dd/yy");

            // Creates a list of courses for the CourseList from the DB
            await _conn.CreateTableAsync<Course>();
            List<Course> l_courseList  = await _conn.QueryAsync<Course>($"Select * From Courses Where Term = '{_currentTerm.Id}'");
            _courseList                = new ObservableCollection<Course>(l_courseList);
            courseListView.ItemsSource = _courseList;

            // Loads the page
            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        // Tap logic for selecting a course | Brings up the contextual course
        private async void Course_Tapped(object sender, ItemTappedEventArgs e)
        {
            Course course = (Course)e.Item;
            await Navigation.PushModalAsync(new CourseDetail(course));
        }

        private async void Add_Course_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddCourse(_currentTerm));
        }

        private async void Edit_Term_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditTerm(_currentTerm));
        }

        private async void Drop_Term_Click(object sender, EventArgs e)
        {
            if (await DisplayAlert("Alert", "Are you sure you want to drop this term?", "Yes", "No"))
            {
                await _conn.DeleteAsync(_currentTerm);
                await Navigation.PopModalAsync();
            }
        }
    }
}