using SQLite;

using System;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetail : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Course _currentCourse;
        public CourseDetail(Course course)
        {
            InitializeComponent();

            _conn          = DependencyService.Get<ISQLiteDb>().GetConnection();
            _currentCourse = course;
        }
        
        protected override void OnAppearing()
        {
            courseName.Text           = _currentCourse.CourseName;
            Status.Text               = _currentCourse.Status;
            StartDate.Text            = _currentCourse.StartDate.ToString("MM/dd/yy");
            EndDate.Text              = _currentCourse.EndDate.ToString("MM/dd/yy");
            InstructorName.Text       = _currentCourse.InstructorName;
            InstructorPhone.Text      = _currentCourse.InstructorPhone;
            InstructorEmail.Text      = _currentCourse.InstructorEmail;
            Notes.Text                = _currentCourse.Notes;
            NotificationsEnabled.Text = _currentCourse.NotificationEnabled == 1 ? "Yes" : "No";

            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Edit_Course_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditCourse(_currentCourse));
        }

        private async void Drop_Course_Click(object sender, EventArgs e)
        {
            // Displays a confirmation box to allow user the ability to stop the deletion
            if (await DisplayAlert("Alert", "Are you sure you want to drop this course?", "Yes", "No"))
            {
                await _conn.DeleteAsync(_currentCourse);
                await Navigation.PopModalAsync();
            }
        }

        private async void Assessments_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AssessmentsPage(_currentCourse));
        }

        // Shares the notes of the current course via several options
        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = Notes.Text,
                Title = "Share Notes"
            });
        }
    }
}