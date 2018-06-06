using System;
using System.Windows;
using System.Windows.Input;

using TeleSharp.TL;
using TLSharp.Core;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private string hash;
        private string NumberToSendMessage;
        private TelegramClient client;

        public Login()
        {
            InitializeComponent();        

            var session = new FileSessionStore();
            client = NewClient(session);
            //client.ConnectAsync();
            
            if (client.IsUserAuthorized())//if user already authorised than open main application
            {
                MainWindow mainWindow = new MainWindow(); // Inicialize main window
                mainWindow.Show();
                this.Close();
            }
        }

        private void window_close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void window_hide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client = new TelegramClient(235585, "2c9039610774b160a14c9c3aa64bbf8c");
            await client.ConnectAsync();
        }

        public static TelegramClient NewClient(FileSessionStore session)
        {
            try
            {
                return new TelegramClient(235585, "2c9039610774b160a14c9c3aa64bbf8c", session, "session");
            }
            catch (MissingApiConfigurationException ex)
            {
                throw new Exception($"Please add your API settings to the `app.config` file. (More info: {MissingApiConfigurationException.InfoUrl})",
                                    ex);
            }
        }

        private async void btn_setPhoneNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                NumberToSendMessage = txt_phone_number.Text;
                if (string.IsNullOrWhiteSpace(NumberToSendMessage)) return;

                string normalizedNumber;
                if (!NumberToSendMessage.StartsWith("+"))
                {
                    normalizedNumber = NumberToSendMessage;
                }
                else
                {
                    normalizedNumber = NumberToSendMessage.Substring(1, NumberToSendMessage.Length - 1);
                }
                hash = await client.SendCodeRequestAsync(normalizedNumber);

                lbl_phone_number.Visibility = Visibility.Hidden;
                txt_phone_number.Visibility = Visibility.Hidden;
                btn_setPhoneNumber.Visibility = Visibility.Hidden;

                lbl_received_code.Visibility = Visibility.Visible;
                txt_received_code.Visibility = Visibility.Visible;
                btn_setReceivedCode.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void btn_setReceivedCode_Click(object sender, RoutedEventArgs e)
        {
            var code = txt_received_code.Text; //get receiived code

            TLUser user = null;
            try
            {
                user = await client.MakeAuthAsync(txt_phone_number.Text, hash, code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //have to write it later
                //var password = await client.GetPasswordSetting();
                //var password_str = txtb_Password.Text;
                //user = await client.MakeAuthWithPasswordAsync(password, password_str);
            }
       
            try
            {
                MainWindow mainWindow = new MainWindow(); // Inicialize main window
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
