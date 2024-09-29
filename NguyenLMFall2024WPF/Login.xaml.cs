using BusinessObject;
using BusinessObject.Entity;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NguyenLMFall2024WPF
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly Admin admin;
        private readonly NewsArticleManagement newsArticleManagement;
        private readonly ProfileManagement profileManagement;
        public Login(ISystemAccountService systemAccountService,
                     Admin admin, NewsArticleManagement newsArticleManagement,
                     ProfileManagement profileManagement)
        {
            _systemAccountService = systemAccountService;
            this.admin = admin;
            this.newsArticleManagement = newsArticleManagement;
            this.profileManagement = profileManagement;
            InitializeComponent();


        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to close this window?", "Confirmation",
                                                       MessageBoxButton.OKCancel,
                                                       MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
                this.Close();
            else
                return;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool isAdmin = _systemAccountService.IsAdmin(new() { AccountEmail = txtEmail.Text, AccountPassword = txtPassword.Password });

            if (isAdmin)
            {
                MessageBox.Show("Login successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                admin.Show();
                Close();
            }
            else
            {
                var account = _systemAccountService.VerifyAccount(new() { AccountEmail = txtEmail.Text, AccountPassword = txtPassword.Password });

                if (account != null)
                {
                    if (account.AccountRole == 1)
                    {
                        MessageBox.Show("Login successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Hide();
                        UserInformation.UserInfo = account;
                        profileManagement.Show();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("You do not have permission!", "Warning",
                                                                    MessageBoxButton.OK,
                                                                    MessageBoxImage.Error);
                        return;
                    }
                }
                MessageBox.Show("Email or password is incorrect", "Warning",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
        }
        private void btnViewNewsArticle_Click(object sender, RoutedEventArgs e)
        {
            newsArticleManagement.Show();
            this.Hide();
        }
    }
}