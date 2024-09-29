using BusinessObject.Entity;
using BusinessObject;
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
    /// Interaction logic for NewsArticleDetail.xaml
    /// </summary>
    public partial class NewsArticleDetail : Window
    {
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly INewsArticleService _newsArticleService;
        public NewsArticleDetail(ICategoryService categoryService, ITagService tagService,
                                 INewsArticleService newsArticleService)
        {
            InitializeComponent();
            _categoryService = categoryService;
            _tagService = tagService;
            _newsArticleService = newsArticleService; 
        }
        private void Window_Activated(object sender, EventArgs e)
        {

            if (WindowOption.IsCreateMode)
            {
                txtID.IsEnabled = true;
                btnCreate.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtID.IsEnabled = false;
                btnUpdate.Visibility = Visibility.Visible;
                btnCreate.Visibility = Visibility.Collapsed;
            }
            //ResetInput();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                NewsArticle? articleData = NewsArticleInformation.ArticleInfo;
                var tagList = _tagService.GetAllTag();
                var cateList = _categoryService.GetAllCategory().Where(c => (bool)c.IsActive).ToList(); // Filter for active categories

                lboTag.ItemsSource = tagList.Select(t => new TagItem
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                }).ToList();

                // Bind the filtered categories to the combo box
                cboCategory.ItemsSource = cateList;
                cboCategory.DisplayMemberPath = "CategoryName";
                cboCategory.SelectedValuePath = "CategoryId";

                if (articleData is not null && WindowOption.IsUpdateMode)
                {
                    txtID.Text = articleData.NewsArticleId;
                    txtNewsArticleTitle.Text = articleData.NewsTitle;
                    txtHeadline.Text = articleData.Headline;
                    txtNewsSource.Text = articleData.NewsSource;
                    txtNewsArticleContent.Text = articleData.NewsContent;
                    cbActiveStatus.IsChecked = articleData.NewsStatus;
                    cboCategory.SelectedValue = articleData.CategoryId;
                    foreach (var tag in articleData.Tags)
                    {
                        var tagItem = lboTag.Items.OfType<TagItem>().FirstOrDefault(t => t.TagId == tag.TagId);
                        if (tagItem is not null) tagItem.IsSelected = true;
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtID.Text.Trim() == "" || txtNewsArticleTitle.Text.Trim() == "" || txtNewsArticleContent.Text.Trim() == "" || cboCategory.Text == "")
                {
                    MessageBox.Show("Please fill in all information", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (_newsArticleService.GetNewsArticleById(txtID.Text.Trim()) is not null)
                {
                    MessageBox.Show("News article ID already exists!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var inputedCategory = cboCategory.SelectedItem as Category;
                var createdBy = UserInformation.UserInfo;

                // Retrieve the selected tags (List<Tag>)
                var selectedTags = lboTag.Items.OfType<TagItem>().Where(t => t.IsSelected).Select(t => new Tag
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                }).ToList();

                string newsSource = string.IsNullOrWhiteSpace(txtNewsSource.Text.Trim()) ? "N/A" : txtNewsSource.Text.Trim();

                // Extract the TagIds from the selectedTags
                var selectedTagIds = selectedTags.Select(tag => tag.TagId).ToList();

                var article = new NewsArticle
                {
                    NewsArticleId = txtID.Text,
                    NewsTitle = txtNewsArticleTitle.Text,
                    NewsSource = txtNewsSource.Text,
                    Headline = txtHeadline.Text,
                    NewsContent = txtNewsArticleContent.Text,
                    NewsStatus = true,
                    CategoryId = inputedCategory?.CategoryId,
                    CreatedById = createdBy?.AccountId,
                    CreatedDate = DateTime.Now,
                    Tags = selectedTags // This is the list of Tag objects
                };

                // Pass the article and selectedTagIds (List<int>)
                _newsArticleService.AddNewsArticle(article, selectedTagIds);

                MessageBox.Show("Create news article successfully!", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                this.Hide();
                //ResetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var updateArticle = NewsArticleInformation.ArticleInfo;
                var inputedCategory = cboCategory.SelectedItem as Category;
                var createdBy = UserInformation.UserInfo;
                var selectedTags = lboTag.Items.OfType<TagItem>().Where(t => t.IsSelected).Select(t => new Tag
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                }).ToList();

                var selectedTagIds = selectedTags.Select(tag => tag.TagId).ToList();

                // Default NewsSource to "N/A" if it's null or empty
                string newsSource = string.IsNullOrWhiteSpace(txtNewsSource.Text.Trim()) ? "N/A" : txtNewsSource.Text.Trim();

                //if (selectedTags.Count == 0)
                //{
                //    MessageBox.Show("Please select at least one tag.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    return;
                //}

                var article = new NewsArticle
                {
                    NewsArticleId = txtID.Text,
                    NewsTitle = txtNewsArticleTitle.Text,
                    Headline = txtHeadline.Text,
                    NewsSource = txtNewsSource.Text,
                    NewsContent = txtNewsArticleContent.Text,
                    CategoryId = inputedCategory?.CategoryId,
                    NewsStatus = cbActiveStatus.IsChecked ?? true,
                    CreatedById = createdBy?.AccountId,
                    CreatedDate = updateArticle?.CreatedDate,
                    ModifiedDate = DateTime.Now,
                    Tags = selectedTags
                };

                _newsArticleService.UpdateNewsArticle(article, selectedTagIds);
                MessageBox.Show("Update news article successfully!", "Success",
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
        private void ResetInput()
        {
            txtID.Text = "";
            txtNewsArticleTitle.Text = "";
            txtNewsArticleContent.Text = "";
            txtHeadline.Text = "";
            txtNewsSource.Text = "";
            cboCategory.SelectedIndex = 0;
            cboCategory.Text = "";
            lboTag.SelectedIndex = 0;
        }
    }
}
