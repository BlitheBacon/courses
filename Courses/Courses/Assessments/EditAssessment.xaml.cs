using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessment : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Assessment _assessment;
        
        public EditAssessment(Assessment assessment)
        {
            InitializeComponent();

            _conn       = DependencyService.Get<ISQLiteDb>().GetConnection();
            _assessment = assessment;
        }

        protected async override void OnAppearing()
        {
            await _conn.CreateTableAsync<Assessment>();

            AssessmentName.Text = _assessment.Title;
            StartDate.Date      = _assessment.StartDate;
            EndDate.Date        = _assessment.EndDate;
            
            if (_assessment.NotificationEnabled == 1)
                EnableNotifications.On = true;

            base.OnAppearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _assessment.Title               = AssessmentName.Text;
            _assessment.StartDate           = StartDate.Date;
            _assessment.EndDate             = EndDate.Date;
            _assessment.NotificationEnabled = EnableNotifications.On == true ? 1 : 0;

            // Determines if the Assessment name is valid and Start occurs prior to End
            if ((AssessmentName.Text.Length > 0))
            {
                if (_assessment.StartDate < _assessment.EndDate)
                {
                    // Inserts validated assessment
                    await _conn.UpdateAsync(_assessment);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must occur prior to End date.", "Ok");
            }
            else await DisplayAlert("Error", "Assessment name cannot be empty.", "Ok");
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}