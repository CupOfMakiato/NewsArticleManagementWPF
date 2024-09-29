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
    /// Interaction logic for NewsHistory.xaml
    /// </summary>
    public partial class NewsHistory : Window
    {
        private readonly INewsArticleService _newsArticleService;
        public NewsHistory(INewsArticleService newsArticleService)
        {
            InitializeComponent();
            _newsArticleService = newsArticleService;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to close this window?", "Confirmation",
                                                                      MessageBoxButton.OKCancel,
                                                                      MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
                this.Hide();
            else
                return;
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var newsArticlesHistory = _newsArticleService.GetAllNewsArticle()
                                                                .Where(a => a.CreatedById == UserInformation.UserInfo?.AccountId)
                                                                .Select(a => new
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
                                                                })
                                                                .OrderByDescending(a => a.CreatedDate).ToList();

                dgvNewsHistory.ItemsSource = newsArticlesHistory;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
