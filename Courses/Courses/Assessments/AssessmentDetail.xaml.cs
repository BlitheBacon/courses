using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentDetail : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Assessment _assessment;

        public AssessmentDetail(Assessment assessment)
        {
            InitializeComponent();

            _conn       = DependencyService.Get<ISQLiteDb>().GetConnection();
            _assessment = assessment;
        }

        protected override void OnAppearing()
        {
            AssessmentName.Text       = _assessment.Title;
            StartDate.Text            = _assessment.StartDate.ToString("MM/dd/yy");
            EndDate.Text              = _assessment.EndDate.ToString("MM/dd/yy");
            AssessmentType.Text       = _assessment.Type;
            NotificationsEnabled.Text = _assessment.NotificationEnabled == 1 ? "Yes" : "No";

            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Edit_Assessment_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditAssessment(_assessment));
        }

        private async void Drop_Assessment_Click(object sender, EventArgs e)
        {
            // Displays a confirmation box to allow user the ability to stop the deletion
            if (await DisplayAlert("Alert", "Are you sure you want to delete this assessment?", "Yes", "No"))
            {
                await _conn.DeleteAsync(_assessment);
                await Navigation.PopModalAsync();
            }
        }
    }
}