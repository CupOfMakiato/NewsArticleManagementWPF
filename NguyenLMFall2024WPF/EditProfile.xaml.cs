using BusinessObject;
using BusinessObject.Entity;
using Repository;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        private SystemAccount? systemAccount;
        private readonly ISystemAccountService _systemAccountService;
        public EditProfile(ISystemAccountService systemAccountService)
        {
            InitializeComponent();
            _systemAccountService = systemAccountService;
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            LoadData();
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pattern = @"^[A-Za-z][\w\-\.]*@FUNewsManagement\.org$";
                Regex regex = new Regex(pattern);
                if (txtID.Text.Trim() == "" || txtName.Text.Trim() == "" || txtEmail.Text.Trim() == "" || txtPassword.Password.Trim() == "")
                {
                    MessageBox.Show("Please fill in all information", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!regex.IsMatch(txtEmail.Text))
                {
                    MessageBox.Show("Email is invalid!\n" +
                                    "Format email: <String> + <@FUNewsManagement.org>", "Warn",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }
                SystemAccount updateAccount = new SystemAccount();

                updateAccount.AccountId = short.Parse(txtID.Text);
                if (txtRole.Text.Trim().ToLower() == "staff")
                    updateAccount.AccountRole = 1;
                updateAccount.AccountName = txtName.Text;
                updateAccount.AccountEmail = txtEmail.Text;
                updateAccount.AccountPassword = txtPassword.Password;

                _systemAccountService.UpdateAccount(updateAccount);

                MessageBoxResult result = MessageBox.Show("Do you want to update your account information?", "Success",
                                                   MessageBoxButton.OKCancel,
                                                   MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                    this.Hide();
                else
                    return;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadData()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close this window?", "Close",
                                                                       MessageBoxButton.YesNo,
                                                                       MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                this.Hide();
            else
                return;
        }
    }
}
