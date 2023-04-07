using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Term _term;

        public AddCourse(Term term)
        {
            InitializeComponent();

            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
            _term = term;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Course course = new Course
            {
                CourseName          = CourseName.Text,
                StartDate           = StartDate.Date,
                EndDate             = EndDate.Date,
                Status              = CourseStatus.SelectedItem.ToString(),
                InstructorName      = InstructorName.Text,
                InstructorPhone     = InstructorPhone.Text,
                InstructorEmail     = InstructorEmail.Text,
                NotificationEnabled = EnableNotifications.On == true ? 1 : 0,
                Notes               = Notes.Text,
                Term                = _term.Id
            };

            // Validates the following text fields are not empty
            if (CourseName.Text.Length      > 0 &&
                InstructorName.Text.Length  > 0 &&
                InstructorPhone.Text.Length > 0)
            {
                // Validates that the email field contains a valid address
                if (TextFieldValidation.IsValidEmail(InstructorEmail.Text))
                {
                    // Validates that the Start date occurs prior to the End date
                    if (course.StartDate < course.EndDate)
                    {
                        await _conn.InsertAsync(course);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error", "Start date must occur prior to End date.", "Ok");
                }
                else await DisplayAlert("Error", "Email must be valid.", "Ok");
            }
            else await DisplayAlert("Error", "All fields must be completed.", "Ok");
        }
    }
}