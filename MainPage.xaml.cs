using Module05Exercise01.Services;
using MySql.Data.MySqlClient;
using Module05Exercise01.View;

namespace Module05Exercise01
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        public MainPage()
        {
            InitializeComponent();

            // Initialize database connection
            _dbConnectionService = new DatabaseConnectionService();

        }

        /**private async void OnTestConnectionClicked(object sender, EventArgs e)
        {
            var connectionString = _dbConnectionService.GetConnectionString();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    ConnectionStatusLabel.Text = "Connection Successful!";
                    ConnectionStatusLabel.TextColor = Colors.Green;
                }
            }
            catch (Exception ex)
            {
                ConnectionStatusLabel.Text = $"Connection Failed!";
                ConnectionStatusLabel.TextColor = Colors.Red;
            }
        }**/

        private async void OpenViewPersonal(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewEmployee());
        }

    }

}
