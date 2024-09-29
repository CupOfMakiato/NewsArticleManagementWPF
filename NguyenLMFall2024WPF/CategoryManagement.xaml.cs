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
    /// Interaction logic for CategoryManagement.xaml
    /// </summary>
    public partial class CategoryManagement : Window
    {
        private readonly ICategoryService _categoryService;
        private readonly CategoryDetail categoryDetail;
        public CategoryManagement(ICategoryService categoryService,
                                  CategoryDetail categoryDetail)
        {
            InitializeComponent();
            _categoryService = categoryService;
            this.categoryDetail = categoryDetail;
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            ResetInput();
            LoadData();
        }
        public void LoadData()
        {
            var categoryList = _categoryService.GetAllCategory();
            dgvCategoryList.ItemsSource = categoryList;
        }

        public void ResetInput()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtParentCategoryId.Text = "";
            txtDescription.Text = "";
            dgvCategoryList.SelectedItem = null;
        }
        private void dgCategoryList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var category = dgvCategoryList.SelectedItem as Category;
                txtID.Text = category?.CategoryId.ToString();
                txtParentCategoryId.Text = category?.ParentCategoryId.ToString();
                if (category.IsActive == true)
                {
                    cbIsActive.IsChecked = true;
                }
                else
                {
                    cbIsActive.IsChecked = false;
                }
                txtName.Text = category?.CategoryName;
                txtDescription.Text = category?.CategoryDesciption;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            WindowOption.IsUpdateMode = false;
            WindowOption.IsCreateMode = true;
            categoryDetail.ShowDialog();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = (Category)dgvCategoryList.SelectedItem;
                if (row is null)
                {
                    MessageBox.Show("Please select a row to update!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                CategoryInformation.CategoryInfo = row;
                WindowOption.IsUpdateMode = true;
                WindowOption.IsCreateMode = false;
                categoryDetail.ShowDialog();
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
                var row = (Category)dgvCategoryList.SelectedItem;
                if (row is null)
                {
                    MessageBox.Show("Please select a row to delete!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Do you want to delete this category?", "Confirmation",
                                                                                         MessageBoxButton.OKCancel,
                                                                                         MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                    _categoryService.DeleteCategory(row);
                else
                    return;
                ResetInput();
                LoadData();
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
