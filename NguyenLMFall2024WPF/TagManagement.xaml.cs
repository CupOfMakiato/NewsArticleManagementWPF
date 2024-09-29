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
    /// Interaction logic for TagManagement.xaml
    /// </summary>
    public partial class TagManagement : Window
    {
        private readonly ITagService _tagService;
        public TagManagement(ITagService tagService)
        {
            InitializeComponent();
            _tagService = tagService;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int tagId;
            try
            {
                tagId = int.Parse(txtID.Text.Trim());
            }
            catch (Exception)
            {
                MessageBox.Show("Tag ID must be a number!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (txtName.Text.Trim() == "" || txtNote.Text.Trim() == "")
                {
                    MessageBox.Show("Please fill in all information", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (_tagService.GetTagById(tagId) is not null)
                {
                    MessageBox.Show("Tag id already exists!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var addTag = new Tag();
                addTag.TagId = int.Parse(txtID.Text.Trim());
                addTag.TagName = txtName.Text;
                addTag.Note = txtNote.Text;

                _tagService.Add(addTag);

                MessageBox.Show("Add tag successfully!", "Success",
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
        public void ResetInput()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtNote.Text = "";
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
