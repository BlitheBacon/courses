using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        public readonly MainPage _mainPage;
        
        public AddTerm(MainPage mainPage)
        {
            InitializeComponent();

            _conn     = DependencyService.Get<ISQLiteDb>().GetConnection();
            _mainPage = mainPage;
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Term term = new Term
            {
                Title     = TermTitle.Text,
                StartDate = startDate.Date,
                EndDate   = endDate.Date
            };

            // Validates title is not empty
            if (term.Title.Length > 0)
            {
                // Validates Start date occurs prior to End date
                if (term.StartDate < term.EndDate)
                {
                    await _conn.InsertAsync(term);
                    _mainPage._termList.Add(term);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must occur prior to End date.", "Ok");
            }
            else await DisplayAlert("Error", "All fields must be completed.", "Ok");
        }
    }
}