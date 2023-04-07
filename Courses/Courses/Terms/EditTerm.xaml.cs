using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Courses
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTerm : ContentPage
    {
        private readonly SQLiteAsyncConnection _conn;
        private readonly Term _term;

        public EditTerm(Term currentTerm)
        {
            InitializeComponent();

            _term = currentTerm;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _conn.CreateTableAsync<Term>();
            TermTitle.Text = _term.Title;
            startDate.Date = _term.StartDate;
            endDate.Date   = _term.EndDate;

            base.OnAppearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _term.Title = TermTitle.Text;
            _term.StartDate = startDate.Date;
            _term.EndDate = endDate.Date;

            // Error handling for proper date setting and field entry
            if (_term.Title.Length > 0)
            {
                if (_term.StartDate < _term.EndDate)
                {
                    // Update and navigate away from current page
                    await _conn.UpdateAsync(_term);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error", "Start date must occur prior to End date.", "Ok");
            }
            else await DisplayAlert("Error", "All fields must be completed.", "Ok");
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
