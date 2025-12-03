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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string currentCaptcha;
        private int failedAttempts = 0;
        private DateTime? blockUntil = null; //время разюлокировки системы, сначала устанавливает значение на null
        public AuthPage()
        {
            InitializeComponent();
            //скрываем капчу
            HideCaptcha();
        }

        private void HideCaptcha()
        {
            captchOneWord.Visibility = Visibility.Collapsed;
            captchTwoWord.Visibility = Visibility.Collapsed;
            captchThreeWord.Visibility = Visibility.Collapsed;
            captchFourWord.Visibility = Visibility.Collapsed;
            CaptchaTB.Visibility = Visibility.Collapsed;
        }
        private string GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //const string chars = "111111";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void DisplayCaptcha(string captcha)
        {
            TextBlock[] captchaBlocks = { captchOneWord, captchTwoWord, captchThreeWord, captchFourWord }; //массив текстблоков

            Random random = new Random();
            for (int i = 0; i < captchaBlocks.Length; i++)
            {
                captchaBlocks[i].Visibility = Visibility.Visible; //видимый элемент
                captchaBlocks[i].Text = captcha[i].ToString();//символ капчи
                //смещение
                int leftMargin = i == 0 ? 30 + random.Next(-5, 5) : random.Next(-5, 5);
                captchaBlocks[i].Margin = new Thickness(leftMargin, random.Next(-5, 5), 0, 0);

                //перечеркивания
                captchaBlocks[i].TextDecorations = random.Next(2) == 0 ? TextDecorations.Strikethrough : null;
            }

        }
        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка блокировки системы
            if (blockUntil.HasValue && DateTime.Now < blockUntil.Value)
            {
                int secondsLeft = (int)(blockUntil.Value - DateTime.Now).TotalSeconds;
                MessageBox.Show($"Система заблокирована. Попробуйте через {secondsLeft} секунд");
                return;
            }

            string login = LoginTB.Text;
            string password = PassTB.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            TextBlock[] captchaBlocks = { captchOneWord, captchTwoWord, captchThreeWord, captchFourWord };

            // Если были неудачные попытки, проверяем капчу
            if (failedAttempts > 0)
            {
                CaptchaTB.Visibility = Visibility.Visible;

                if (string.IsNullOrEmpty(CaptchaTB.Text))
                {
                    MessageBox.Show("Введите капчу!");
                    return;
                }

                if (CaptchaTB.Text != currentCaptcha)
                {
                    MessageBox.Show("Неверная капча!");
                    blockUntil = DateTime.Now.AddSeconds(10); // Блокировка на 10 секунд
                    CaptchaTB.Text = "";
                    currentCaptcha = GenerateCaptcha();
                    DisplayCaptcha(currentCaptcha);
                    MessageBox.Show("Система заблокирована на 10 секунд!");
                    return;
                }
            }

            User user = Konstantinova41Entities1.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                failedAttempts = 0;
                CaptchaTB.Visibility = Visibility.Collapsed;
                for (int i = 0; i < captchaBlocks.Length; i++)
                {
                    captchaBlocks[i].Visibility = Visibility.Collapsed;
                }
                CaptchaTB.Text = "";
                Manager.MainFrame.Navigate(new ProductPage(user));
                LoginTB.Text = "";
                PassTB.Text = "";
            }
            else
            {
                failedAttempts++;
                if (failedAttempts > 0)
                {
                    currentCaptcha = GenerateCaptcha();
                    DisplayCaptcha(currentCaptcha);
                    CaptchaTB.Visibility = Visibility.Visible;
                    CaptchaTB.Text = "";
                    CaptchaTB.Focus();
                }
                MessageBox.Show("Введены неверные данные!");
            }
        }

        private void LogInButtonGuest_Click(object sender, RoutedEventArgs e)
        {
            TextBlock[] captchaBlocks = { captchOneWord, captchTwoWord, captchThreeWord, captchFourWord };

            User guestUser = new User
            {
                UserSurname = "Гость",
                UserName = "",
                UserPatronymic = "",
                UserRole = 4 //роль гостя будет 4
            };
            //очистка капчи
            failedAttempts = 0;
            CaptchaTB.Visibility = Visibility.Collapsed;
            for (int i = 0; i < captchaBlocks.Length; i++)
            {
                captchaBlocks[i].Visibility = Visibility.Collapsed;
            }
            CaptchaTB.Text = "";
            Manager.MainFrame.Navigate(new ProductPage(guestUser));
            LoginTB.Text = "";
            PassTB.Text = "";
        }
    }
}
