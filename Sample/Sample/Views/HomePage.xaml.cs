using Xamarin.Forms;

namespace Sample.Views
{
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        async void OnPickButtonClicked(System.Object sender, System.EventArgs e)
            => await Navigation.PushAsync(new PickPage());

        async void OnSaveButtonClicked(System.Object sender, System.EventArgs e)
            => await Navigation.PushAsync(new SavePage());
    }
}
