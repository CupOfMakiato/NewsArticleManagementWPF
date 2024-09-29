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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly ReportStatistic reportStatistic;
        private readonly AccountDetail accountDetail;
        public Admin(ISystemAccountService systemAccountService, 
                     ReportStatistic reportStatistic,
                     AccountDetail accountDetail)
        {
            InitializeComponent();
            _systemAccountService = systemAccountService;
            this.reportStatistic = reportStatistic;
            this.accountDetail = accountDetail;
            
        }
        private void LoadData()
        {
            try
            {
                var accountList = _systemAccountService.GetAllAccount();
                dgvAccountList.ItemsSource = accountList.Select(a => new
                {
                    Id = a.AccountId,
                    Name = a.AccountName,
                    Email = a.AccountEmail,
                    Password = new string('*', a.AccountPassword.Length),
                    Role = a.AccountRole == 1 ? "staff" : "lecturer",
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ResetInput()
        {
            txtID.Text = "";
            txtEmail.Text = "";
            txtName.Text = "";
            txtPassword.Password = "";
            txtRole.Text = "";
            dgvAccountList.SelectedItem = null;
            txtSearch.Text = "";
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            WindowOption.IsUpdateMode = false;
            WindowOption.IsCreateMode = true;
            accountDetail.ShowDialog();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = (dynamic)dgvAccountList.SelectedItem;
                if (row is null)
                {
                    MessageBox.Show("Please select a row to update!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                UserInformation.UserInfo = _systemAccountService.GetAccountById(row.Id);
                WindowOption.IsUpdateMode = true;
                WindowOption.IsCreateMode = false;
                accountDetail.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var deleteAccount = dgvAccountList.SelectedItem;
                if (deleteAccount is null)
                {
                    MessageBox.Show("Please select an account to delete", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Do you want to delete this account?", "Confirmation",
                                                                         MessageBoxButton.OKCancel,
                                                                         MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    SystemAccount systemAccount = new SystemAccount
                    {
                        AccountId = short.Parse(deleteAccount.GetType().GetProperty("Id").GetValue(deleteAccount).ToString()),
                        AccountName = deleteAccount.GetType().GetProperty("Name").GetValue(deleteAccount).ToString(),
                        AccountEmail = deleteAccount.GetType().GetProperty("Email").GetValue(deleteAccount).ToString(),
                        AccountPassword = deleteAccount.GetType().GetProperty("Password").GetValue(deleteAccount).ToString(),
                        AccountRole = deleteAccount.GetType().GetProperty("Role").GetValue(deleteAccount).ToString() == "staff" ? 1 : 2,
                    };
                    _systemAccountService.DeleteAccount(systemAccount);
                    LoadData();
                }
                else
                    return;
                ResetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnReportStatistic_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            reportStatistic.ShowDialog();
            if (reportStatistic.IsActive == false)
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // search by name 
                var searchValue = txtSearch.Text.ToLower().Trim();
                var accountList = _systemAccountService.GetAllAccount()
                                                        .Where(a => a.AccountName.ToLower().Contains(searchValue.Trim()));
                dgvAccountList.ItemsSource = accountList.Select(a => new
                {
                    Id = a.AccountId,
                    Name = a.AccountName,
                    Email = a.AccountEmail,
                    Password = new string('*', a.AccountPassword.Length),
                    Role = a.AccountRole == 1 ? "staff" : "lecturer",
                }).ToList();
                ResetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void dgAccountList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedAccount = dgvAccountList.SelectedItem;
                if (selectedAccount is not null)
                {
                    txtID.Text = selectedAccount.GetType().GetProperty("Id").GetValue(selectedAccount).ToString();
                    txtName.Text = selectedAccount.GetType().GetProperty("Name").GetValue(selectedAccount).ToString();
                    txtEmail.Text = selectedAccount.GetType().GetProperty("Email").GetValue(selectedAccount).ToString();
                    txtPassword.Password = selectedAccount.GetType().GetProperty("Password").GetValue(selectedAccount).ToString();
                    txtRole.Text = selectedAccount.GetType().GetProperty("Role").GetValue(selectedAccount).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            ResetInput();
            LoadData();
        }
    }
}


