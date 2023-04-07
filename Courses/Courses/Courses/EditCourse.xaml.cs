using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCourse : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Course _currentCourse;
        public EditCourse(Course currentCourse)
        {
            InitializeComponent();

            _conn          = DependencyService.Get<ISQLiteDb>().GetConnection();
            _currentCourse = currentCourse;
        }

        protected async override void OnAppearing()
        {
            await _conn.CreateTableAsync<Course>();

            CourseName.Text           = _currentCourse.CourseName;
            CourseStatus.SelectedItem = _currentCourse.Status;
            StartDate.Date            = _currentCourse.StartDate;
            EndDate.Date              = _currentCourse.EndDate;
            InstructorName.Text       = _currentCourse.InstructorName;
            InstructorPhone.Text      = _currentCourse.InstructorPhone;
            InstructorEmail.Text      = _currentCourse.InstructorEmail;
            Notes.Text                = _currentCourse.Notes;
            EnableNotifications.On    = _currentCourse.NotificationEnabled == 1; // Assigns result of boolean check

            base.OnAppearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _currentCourse.CourseName          = CourseName.Text;
            _currentCourse.Status              = CourseStatus.SelectedItem.ToString();
            _currentCourse.StartDate           = StartDate.Date;
            _currentCourse.EndDate             = EndDate.Date;
            _currentCourse.InstructorName      = InstructorName.Text;
            _currentCourse.InstructorEmail     = InstructorEmail.Text;
            _currentCourse.InstructorPhone     = InstructorPhone.Text;
            _currentCourse.Notes               = Notes.Text;
            _currentCourse.NotificationEnabled = EnableNotifications.On == true ? 1 : 0;

            // Validates the following text fields are not empty
            if (CourseName.Text.Length      > 0 &&
                InstructorName.Text.Length  > 0 &&
                InstructorPhone.Text.Length > 0)
            {
                // Validates that the email field contains a valid address
                if (TextFieldValidation.IsValidEmail(InstructorEmail.Text))
                {
                    // Validates that the Start date occurs prior to the End date
                    if (_currentCourse.StartDate < _currentCourse.EndDate)
                    {
                        await _conn.UpdateAsync(_currentCourse);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error", "Start date must occur prior to End date.", "Ok");
                }
                else await DisplayAlert("Error", "Email must be valid.", "Ok");
            }
            else await DisplayAlert("Error", "All fields must be completed.", "Ok");
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}