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
    /// Interaction logic for CategoryDetail.xaml
    /// </summary>
    public partial class CategoryDetail : Window
    {
        private readonly ICategoryService _categoryService;
        public CategoryDetail(ICategoryService categoryService)
        {
            InitializeComponent();
            _categoryService = categoryService;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtName.Text.Trim() == "" || txtDescription.Text.Trim() == "")
                {
                    MessageBox.Show("Please fill in all information", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Category createCategory = new Category
                {
                    CategoryName = txtName.Text.Trim(),
                    CategoryDesciption = txtDescription.Text.Trim(),
                    IsActive = false
                };
                if (!string.IsNullOrWhiteSpace(txtParentCategoryId.Text))
                {
                    if (int.TryParse(txtParentCategoryId.Text.Trim(), out int parentCategoryId))
                    {
                        createCategory.ParentCategoryId = (short?)parentCategoryId;
                    }
                    else
                    {
                        MessageBox.Show("Parent Category ID must be a valid number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                _categoryService.AddCategory(createCategory);

                MessageBox.Show("Add category successfully!", "Success",
                                                       MessageBoxButton.OK,
                                                       MessageBoxImage.Information);

                this.Hide();
                //ResetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (WindowOption.IsCreateMode)
            {
                btnCreate.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnUpdate.Visibility = Visibility.Visible;
                btnCreate.Visibility = Visibility.Collapsed;
            }
            ResetInput();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                var category = CategoryInformation.CategoryInfo;
                if (category is not null && WindowOption.IsUpdateMode)
                {
                    txtID.Text = category?.CategoryId.ToString();
                    txtName.Text = category?.CategoryName;
                    txtParentCategoryId.Text = category?.CategoryId.ToString();
                    cbIsActive.IsChecked = category?.IsActive;
                    txtDescription.Text = category?.CategoryDesciption;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ResetInput()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtDescription.Text = "";
            txtParentCategoryId.Text = "";
            cbIsActive.IsChecked = false;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if required fields are empty
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtParentCategoryId.Text))
                {
                    MessageBox.Show("Please fill in all information", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ensure that CategoryId is provided (Assume it's being displayed in txtID)
                if (!int.TryParse(txtID.Text.Trim(), out int categoryId))
                {
                    MessageBox.Show("Invalid Category ID", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Retrieve the existing category from the database
                var existingCategory = _categoryService.GetCategoryById((short)categoryId);  // Make sure GetCategoryById exists in your service layer

                if (existingCategory == null)
                {
                    MessageBox.Show("Category not found.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update the category's properties with new values
                existingCategory.CategoryName = txtName.Text.Trim();
                existingCategory.CategoryDesciption = txtDescription.Text.Trim();
                existingCategory.IsActive = cbIsActive.IsChecked ?? false;

                // Handle optional ParentCategoryId
                if (int.TryParse(txtParentCategoryId.Text.Trim(), out int parentCategoryId))
                {
                    existingCategory.ParentCategoryId = (short?)parentCategoryId;
                }
                else
                {
                    existingCategory.ParentCategoryId = null; // or handle differently if necessary
                }

                // Save the updated category
                _categoryService.UpdateCategory(existingCategory);

                MessageBox.Show("Update category successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Hide();
                // ResetInput(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
