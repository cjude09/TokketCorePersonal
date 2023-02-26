namespace Tokket
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Click {count} time";
            else
                CounterBtn.Text = $"Click {count} tmes";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}