using SQLite;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;
using Plugin.LocalNotifications;

using Courses.Terms;

namespace Courses
{
    // Defines the SQL Tables for terms, courses, and assessments
    #region SQLite Tables
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Term { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Notes { get; set; }
        public int NotificationEnabled { get; set; }
    }
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public int Course { get; set; }
        public int NotificationEnabled { get; set; }
    }
    #endregion

    public partial class MainPage : ContentPage
    {
        // Sets the SQL connection
        private readonly SQLiteAsyncConnection _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        
        // Sets notification flag
        private bool pushNotification = true;
        
        // Initialize the term list
        public ObservableCollection<Term> _termList;

        // Constructs the page
        public MainPage()
        {
            InitializeComponent();
            termListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(Term_Click);
        }

        // Data loaded upon the page appearing
        protected override async void OnAppearing()
        {
            // Reset the input data to prepopulated values
            /*
            await _conn.DropTableAsync<Term>();
            await _conn.DropTableAsync<Course>();
            await _conn.DropTableAsync<Assessment>();
            */

            // Creates the tables if non exist
            await _conn.CreateTableAsync<Term>();
            await _conn.CreateTableAsync<Course>();
            await _conn.CreateTableAsync<Assessment>();
            
            // Copies the table entries into their own lists
            var termList       = await _conn.Table<Term>().ToListAsync();
            var courseList     = await _conn.Table<Course>().ToListAsync();
            var assessmentList = await _conn.Table<Assessment>().ToListAsync();
            
            // If Term contains no entries, populate all lists data at startup
            if(!termList.Any())
            {
                // Term Data population
                Term term_mockData = new Term
                {
                    Title     = "Term 1",
                    StartDate = new DateTime(DateTime.Today.Ticks),                         // Mock start set to current date
                    EndDate   = new DateTime(DateTime.Today.AddMonths(6).Ticks)             // Mock end set to 6 months from start
                };

                // Inserts Mock Data and appends to the Term List
                await _conn.InsertAsync(term_mockData);
                termList.Add(term_mockData);

                // Course Data population
                Course course_mockData = new Course
                {
                    CourseName          = "C971 — Mobile Application Development",
                    StartDate           = new DateTime(DateTime.Today.Ticks),              // Mock start set to current date
                    EndDate             = new DateTime(DateTime.Today.AddMonths(6).Ticks), // Mock end set to 6 months from start
                    Status              = "In-Progress",
                    InstructorName      = "Josh Hagins",
                    InstructorPhone     = "803-414-4529",
                    InstructorEmail     = "jhagins@wgu.edu",
                    NotificationEnabled = 1,                                               // Notifications set to 1 (true)
                    Notes               = "Class C971\n\nCreating a mobile app with Xamarin Forms in C#.",
                    Term                = term_mockData.Id                                 // Weak Relationship to Term
                };

                //Inserts the Mock Course data
                await _conn.InsertAsync(course_mockData);

                // Objective Assessment Data population
                Assessment objAssessment_mockData = new Assessment
                {
                    Title               = "Objective Assessment",
                    StartDate           = course_mockData.StartDate,
                    EndDate             = course_mockData.EndDate,
                    Course              = course_mockData.Id,                      // Weak Relationship to Course
                    Type                = "Objective",
                    NotificationEnabled = 1                                        // Notifications set to 1 (true)
                };

                // Inserts the Mock Object Assessment data
                await _conn.InsertAsync(objAssessment_mockData);

                // Performance Assessment Data population
                Assessment perfAssessment_mockData = new Assessment
                {
                    Title               = "Performance Assessment",
                    StartDate           = course_mockData.StartDate,
                    EndDate             = course_mockData.EndDate,
                    Course              = course_mockData.Id,                     // Weak Relationship to Course
                    Type                = "Performance",
                    NotificationEnabled = 1                                       // Notifications set to 1 (true)
                };

                // Inserts the Mock Performance Assessment data
                await _conn.InsertAsync(perfAssessment_mockData);
                await _conn.CloseAsync();
            }

            // Notification Handlers:
            // If notifications are set to true, iterate through each course and each assessment
            //     to check for conditions to notify
            if(pushNotification == true)
            {
                pushNotification = false; // Ensures notifications only push on initial load of the program
                var _idCount = 0;         // Variable to track IDs for notifications. Using Object IDs can lead to accidental
                                          //    overwriting of IDs in notification queue
                // Loops through each course and each assessment, checking for the notification flag.                 
                foreach (Course course in courseList)
                {
                    if(course.NotificationEnabled == 1)
                    {
                        if (course.StartDate.Date == DateTime.Today)
                        {
                            CrossLocalNotifications.Current.Show("Course", $"{course.CourseName} begins today!", _idCount++, DateTime.Now.Date);
                        }
                        if (course.EndDate == DateTime.Today)
                        {
                            CrossLocalNotifications.Current.Show("Course", $"{course.CourseName} ends today!", _idCount++, DateTime.Now.Date);
                        }
                    }
                }
                foreach (Assessment assessment in assessmentList)
                {
                    if (assessment.NotificationEnabled == 1)
                    {
                        if (assessment.StartDate == DateTime.Today)
                        {
                            CrossLocalNotifications.Current.Show("Assessment", $"{assessment.Title} opens today!", _idCount++, DateTime.Now.Date);
                        }
                        if (assessment.EndDate == DateTime.Today)
                        {
                            CrossLocalNotifications.Current.Show("Assessment", $"{assessment.Title} closes today!", _idCount++, DateTime.Now.Date);
                        }
                    }
                }
            }
            _termList = new ObservableCollection<Term>(termList);
            termListView.ItemsSource = _termList;

            base.OnAppearing();
        }
        async private void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddTerm(this));
        }

        async private void Term_Click(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushModalAsync(new TermDetails(term));
        }
    }
}
