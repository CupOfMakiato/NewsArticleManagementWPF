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
    /// Interaction logic for ProfileManagement.xaml
    /// </summary>
    public partial class ProfileManagement : Window
    {
        private SystemAccount? systemAccount;

        private readonly ISystemAccountService _systemAccountService;
        private readonly EditProfile editProfile;
        private readonly NewsArticleManagement newsArticleManagement;
        private readonly CategoryManagement categoryManagement;
        private readonly NewsHistory newsHistory;
        public ProfileManagement(ISystemAccountService systemAccountService,
                                 EditProfile editProfile,
                                 NewsArticleManagement newsArticleManagement,
                                 CategoryManagement categoryManagement,
                                 NewsHistory newsHistory)
        {
            InitializeComponent();
            _systemAccountService = systemAccountService;
            this.editProfile = editProfile;
            this.newsArticleManagement = newsArticleManagement;
            this.categoryManagement = categoryManagement;
            this.newsHistory = newsHistory;
        }
        private void LoadData()
        {
            systemAccount = UserInformation.UserInfo;
            if (systemAccount is not null)
            {
                var account = _systemAccountService.GetAccountById(systemAccount.AccountId);
                txtID.Text = account.AccountId.ToString();
                if (account.AccountRole == 1)
                    txtRole.Text = "staff";
                txtEmail.Text = account.AccountEmail;
                txtName.Text = account.AccountName;
                txtPassword.Password = account.AccountPassword;
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnNewsArticleManagement_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            newsArticleManagement.ShowDialog();
            if (newsArticleManagement.IsActive == false)
                this.Show();
        }

        private void btnCategoryManagement_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            categoryManagement.ShowDialog();
            if (categoryManagement.IsActive == false)
                this.Show();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            editProfile.ShowDialog();
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            UserInformation.UserInfo = systemAccount;
            this.Hide();
            newsHistory.ShowDialog();
            if (newsHistory.IsActive == false)
                this.Show();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to close this window?", "Confirmation",
                                                                      MessageBoxButton.OKCancel,
                                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
                this.Close();
            else
                return;
        }
    }
}
