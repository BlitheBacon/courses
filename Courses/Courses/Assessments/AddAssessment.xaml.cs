using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessment : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Course _course;
        public AddAssessment(Course course)
        {
            InitializeComponent();

            _conn   = DependencyService.Get<ISQLiteDb>().GetConnection();
            _course = course;
        }

        protected override async void OnAppearing()
        {
            await _conn.CreateTableAsync<Assessment>();

            // Checks for existence of assessment types to determine button behavior
            var performanceCount = await _conn.QueryAsync<Assessment>($"Select Type From Assessments Where Course = '{_course.Id}' And Type = 'Performance'");
            var objectiveCount   = await _conn.QueryAsync<Assessment>($"Select Type From Assessments Where Course = '{_course.Id}' And Type = 'Objective'");

            // Performance Assessment Count Check
            if (performanceCount.Count == 0)
            {
                AssessmentType.Items.Add("Performance");
            }
            else
            {
                AssessmentType.Items.Remove("Performance");
            }

            // Objective Assessment Count Check
            if (objectiveCount.Count == 0)
            {
                AssessmentType.Items.Add("Objective");
            }
            else
            {
                AssessmentType.Items.Remove("Objective");
            }

            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Add_Assessment_Clicked(object sender, EventArgs e)
        {
            Assessment _assessment = new Assessment
            {
                Title     = AssessmentName.Text,
                StartDate = StartDate.Date,
                EndDate   = EndDate.Date,
                Course    = _course.Id,
                Type      = AssessmentType.SelectedItem.ToString()
            };

            // Determines if the Assessment name is valid and Start occurs prior to End
            if ((AssessmentName.Text.Length > 0))
            {
                if (_assessment.StartDate < _assessment.EndDate)
                {
                    // Inserts validated assessment
                    await _conn.InsertAsync(_assessment);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must occur prior to End date.", "Ok");
            }
            else await DisplayAlert("Error", "Assessment name cannot be empty.", "Ok");
        }
    }
}