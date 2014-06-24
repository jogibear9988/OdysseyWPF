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
using PasswordSafe.Data.Biz;
using PasswordSafe.Properties;

namespace PasswordSafe.UserControls
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : UserControl
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        static ChangePassword()
        {
            CommandManager.RegisterClassCommandBinding(typeof(ChangePassword), new CommandBinding(ChangePasswordCommand, DoChangePassword, CanChangePassword));
        }

        public static readonly RoutedUICommand ChangePasswordCommand = new RoutedUICommand("Change Password", "ChangePasswordCommand", typeof(ChangePassword));

        private static void DoChangePassword(object sender, ExecutedRoutedEventArgs e)
        {
            ChangePassword screen = (ChangePassword)sender;
            screen.DoChangePassword();
        }

        private static void CanChangePassword(object sender, CanExecuteRoutedEventArgs e)
        {
            ChangePassword screen = (ChangePassword)sender;
            e.CanExecute = screen.confirmPassword.Password == screen.newPassword.Password;
        }


        internal void FocusPassword()
        {
            oldPassword.Focus();
            oldPassword.SelectAll();
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
            switch (e.Key)
            {
                case Key.Enter:
                    DoChangePassword();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    e.Handled = true;
                    BackToLogin();
                    break;

            }
            base.OnKeyDown(e);
        }


        private void DoChangePassword()
        {
            if (this.newPassword.Password == confirmPassword.Password)
            {
                Main main = GetMain();
                string newPassword = this.newPassword.Password;
                if (BizContext.Instance.ChangePassword(Settings.Default.PasswordsConnectionString, oldPassword.Password, newPassword))
                {
                    ClearInput();
                    main.Login(newPassword);
                    newPassword = null;
                }
            }
        }

        private void ClearInput()
        {
            oldPassword.Password = "";
            this.newPassword.Password = "";
            confirmPassword.Password = "";
        }

        private void OnBackToLoginClick(object sender, RoutedEventArgs e)
        {
            BackToLogin();
        }

        private void BackToLogin()
        {
            ClearInput();
            GetMain().DisplayType = DisplayType.Login;
        }
    }
}
