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
    /// Interaction logic for NewsArticleManagement.xaml
    /// </summary>
    public partial class NewsArticleManagement : Window
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ITagService _tagService;
        private readonly ICategoryService _categoryService;
        private readonly NewsArticleDetail newsArticleDetail;
        private readonly TagManagement tagManagement;
        public NewsArticleManagement(INewsArticleService newsArticleService, ITagService tagService,  
                                     ICategoryService categoryService, NewsArticleDetail newsArticleDetail,
                                     TagManagement tagManagement)
        {
            InitializeComponent();
            _newsArticleService = newsArticleService;
            _tagService = tagService;
            _categoryService = categoryService;
            this.newsArticleDetail = newsArticleDetail;
            this.tagManagement = tagManagement;
        }
        private void LoadCategoryList()
        {
            try
            {
                var cateList = _categoryService.GetAllCategory();
                cboCategory.ItemsSource = cateList;
                cboCategory.DisplayMemberPath = "CategoryName";
                cboCategory.SelectedValuePath = "CategoryId";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadTagList()
        {
            try
            {
                var tagList = _tagService.GetAllTag();
                lboTag.ItemsSource = tagList;
                lboTag.SelectedValuePath = "TagId";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        public void LoadNewsArticleList()
        {
            try
            {
                SystemAccount? account = UserInformation.UserInfo;
                if (account is null)
                {
                    btnCreate.Visibility = Visibility.Collapsed;
                    btnUpdate.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                }
                else if (account.AccountRole == 1)
                {
                    btnCreate.Visibility = Visibility.Visible;
                    btnUpdate.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Visible;
                }
                var newsArticleList = _newsArticleService.GetAllNewsArticle();
                if (account is null)
                    newsArticleList = newsArticleList.Where(a => a.NewsStatus == true).ToList();
                dgvNewsArticleList.ItemsSource = newsArticleList.Select(a => new
                {
                    Id = a.NewsArticleId,
                    Title = a.NewsTitle,
                    Headline = a.Headline,
                    Source = a.NewsSource,
                    Category = a.Category?.CategoryName,
                    Tag = string.Join(", ", a.Tags.Select(t => t.TagName)),
                    a.NewsStatus,
                    Content = a.NewsContent,
                    a.CreatedDate,
                    CreatedBy = a.CreatedBy?.AccountName,
                    a.ModifiedDate,
                }).ToList();
                ResetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ResetInput()
        {
            txtID.Text = "";
            txtNewsArticleTitle.Text = "";
            cbActiveStatus.IsChecked = false;
            txtNewsArticleContent.Text = "";
            cboCategory.SelectedIndex = 0;
            cboCategory.Text = "";
            lboTag.SelectedIndex = 0;
            dgvNewsArticleList.SelectedItem = null;
            txtSearch.Text = "";
        }
        public void LoadData()
        {
            SystemAccount? account = UserInformation.UserInfo;
            if (account is null)
            {
                btnCreate.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnAddTag.Visibility = Visibility.Collapsed;
            }
            else if (account.AccountRole == 1)
            {
                btnCreate.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                btnAddTag.Visibility = Visibility.Visible;
            }
            LoadCategoryList();
            LoadTagList();
            LoadNewsArticleList();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ResetInput();
            LoadData();
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            WindowOption.IsUpdateMode = false;
            WindowOption.IsCreateMode = true;
            newsArticleDetail.ShowDialog();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = (dynamic)dgvNewsArticleList.SelectedItem;
                if (row is null)
                {
                    MessageBox.Show("Please select a row to update!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                NewsArticleInformation.ArticleInfo = _newsArticleService.GetNewsArticleById(row.Id);
                WindowOption.IsUpdateMode = true;
                WindowOption.IsCreateMode = false;
                newsArticleDetail.ShowDialog();
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
                var row = (dynamic)dgvNewsArticleList.SelectedItem;
                if (row is null)
                {
                    MessageBox.Show("Please select a row to delete!", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                NewsArticleInformation.ArticleInfo = _newsArticleService.GetNewsArticleById(row.Id);

                if (NewsArticleInformation.ArticleInfo.NewsStatus == false)
                {
                    MessageBox.Show("This article had been deleted", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this article?", "Delete",
                                                                                          MessageBoxButton.YesNo,
                                                                                          MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var removeArticle = NewsArticleInformation.ArticleInfo;
                    var inputedCategory = cboCategory.SelectedItem as Category;
                    var createdBy = UserInformation.UserInfo;
                    var selectedTags = lboTag.Items.OfType<TagItem>().Where(t => t.IsSelected).Select(t => new Tag
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                    }).ToList();

                    var article = new NewsArticle
                    {
                        NewsArticleId = row.Id,
                        NewsTitle = row.Title,
                        NewsContent = row.Content,
                        Headline = row.Headline,
                        NewsSource = row.Source,
                        CategoryId = removeArticle.CategoryId,
                        NewsStatus = false,
                        CreatedById = createdBy?.AccountId,
                        CreatedDate = removeArticle?.CreatedDate,
                        ModifiedDate = DateTime.Now,
                    };

                    _newsArticleService.SoftDeleteNewsArticle(article);
                    LoadData();
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            tagManagement.ShowDialog();
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

        private void dgNewsArticleList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selectedRow = dgvNewsArticleList.SelectedItem;

                if (selectedRow is null) return;

                var row = (dynamic)selectedRow;

                txtID.Text = row.Id.ToString();
                txtNewsArticleTitle.Text = row.Title;
                txtNewsSource.Text = row.Source;

                if (row.NewsStatus == true)
                {
                    cbActiveStatus.IsChecked = true;
                }
                else
                {
                    cbActiveStatus.IsChecked = false;
                }

                txtNewsArticleContent.Text = row.Content;

                var article = _newsArticleService.GetNewsArticleById(row.Id.Trim());
                if (article != null)
                {
                    cboCategory.SelectedValue = article.CategoryId;
                    lboTag.ItemsSource = article.Tags;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var search = txtSearch.Text;
                var newsArticleList = _newsArticleService.GetAllNewsArticle()
                                                            .Where(a => a.NewsTitle != null
                                                                        && a.NewsTitle.Contains(search.Trim()));
                if (UserInformation.UserInfo is null)
                    newsArticleList = newsArticleList.Where(a => a.NewsStatus == true);
                dgvNewsArticleList.ItemsSource = newsArticleList.Select(a => new
                {
                    Id = a.NewsArticleId,
                    Title = a.NewsTitle,
                    Headline = a.Headline,
                    Source = a.NewsSource,
                    Category = a.Category?.CategoryName,
                    Tag = string.Join(", ", a.Tags.Select(t => t.TagName)),
                    a.NewsStatus,
                    Content = a.NewsContent,
                    a.CreatedDate,
                    CreatedBy = a.CreatedBy?.AccountName,
                    a.ModifiedDate,
                }).ToList();
                ResetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
