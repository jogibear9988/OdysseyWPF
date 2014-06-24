using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PasswordSafe.Controls;

namespace PasswordSafe.UserControls
{
    /// <summary>
    /// Interaction logic for LockScreen.xaml
    /// </summary>
    public partial class LockScreen : UserControl
    {
        
        public LockScreen()
        {
            InitializeComponent();
        }

        static LockScreen()
        {
            CommandManager.RegisterClassCommandBinding(typeof(LockScreen), new CommandBinding(LoginCommand, Login));
        }

        public static readonly RoutedUICommand LoginCommand = new RoutedUICommand("Login", "LoginCommand", typeof(LockScreen));

        private static void Login(object sender, ExecutedRoutedEventArgs e)
        {
            LockScreen screen = (LockScreen)sender;
            screen.Login();
        }

        public void FocusPassword()
        {
            password.Focus();
            password.SelectAll();
        }

        private void Login()
        {
            Main main = GetMain();
            if (main != null)
            {
                string password = this.password.Password;
                if (!main.Login(password))
                {
                    LoginInfo = "Wrong Password, try again...";
                    FocusPassword();
                }
                else
                {
                    // clear the typed in right after login to make sure it won't appear after logging out:
                    this.password.Password = "";
                    LoginInfo = "";
                }
            }

        }

        private Main GetMain()
        {
            FrameworkElement parent = this.Parent as FrameworkElement;
            while (parent != null && !(parent is Main))
            {
                parent = parent.Parent as FrameworkElement;
            }
            Main main = parent as Main;
            return main;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Login();
            }
            base.OnKeyDown(e);
        }



        public string LoginInfo
        {
            get { return (string)GetValue(LoginInfoProperty); }
            set { SetValue(LoginInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoginInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoginInfoProperty =
            DependencyProperty.Register("LoginInfo", typeof(string), typeof(LockScreen), new UIPropertyMetadata(""));

        private void OnChangePasswordClick(object sender, RoutedEventArgs e)
        {
            Main main = GetMain();
            main.DisplayType = DisplayType.ChangePassword;
        }


    }
}
