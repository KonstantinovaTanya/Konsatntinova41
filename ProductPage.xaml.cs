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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Konstantinova41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private List<Product> allProducts; // Отфильтрованные продукты
        private List<Product> currentProduct; // Все продукты из БД
        public ProductPage(User user)
        {
            InitializeComponent();

            FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            if (user.UserRole != 4)
            {
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;
                }
                RoleSP.Visibility = Visibility.Visible;
            }
            else
            {
                RoleSP.Visibility = Visibility.Collapsed;
            }

            allProducts = Konstantinova41Entities1.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;

            ComboType.SelectedIndex = 0;
            UpdateProduct();
            UpdateProductCount();
        }

        
        private void UpdateProduct()
        {
            currentProduct = allProducts.ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) <= 9.99)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 10 && Convert.ToInt32(p.ProductDiscountAmount) <= 14.99)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 15 && Convert.ToInt32(p.ProductDiscountAmount) <= 100)).ToList();
            }

            currentProduct = currentProduct.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ProductListView.ItemsSource = currentProduct.ToList();

            if (RBtnDown.IsChecked.Value)
            {
                currentProduct = currentProduct.OrderByDescending(p => p.ProductName).ToList();
            }
            if (RBtnUp.IsChecked.Value)
            {
                currentProduct = currentProduct.OrderBy(p => p.ProductName).ToList();
            }

            ProductListView.ItemsSource = currentProduct;

            UpdateProductCount();
        }

        // Метод для обновления счетчика записей
        private void UpdateProductCount()
        {
            int totalCount = allProducts?.Count ?? 0;
            int currentCount = currentProduct?.Count ?? 0;

            // Форматируем текст: "15 из 37"
            TBlockRecordCount.Text = $"кол-во {currentCount} из {totalCount}";
        }
        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }
        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }
        private void RBtnUp_Checked(object sender, EventArgs e)
        {
            UpdateProduct();
        }
        private void RBtnDown_Checked(object sender, EventArgs e)
        {
            UpdateProduct();
        }


        private void Button_Click (object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
