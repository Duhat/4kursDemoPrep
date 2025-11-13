using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace _4KursOsen1
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private string currentCaptchaText;
        private Entities context;
        public AuthWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
            context = new Entities();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка каптчи
            if (captchaTextBox.Text != currentCaptchaText)
            {
                MessageBox.Show("Неверная каптча");
                GenerateCaptcha();
                captchaTextBox.Text = "";
                return;
            }

            // Проверка ID пользователя
            if (string.IsNullOrEmpty(idTextBox.Text))
            {
                MessageBox.Show("Введите ID пользователя");
                return;
            }

            if (!int.TryParse(idTextBox.Text, out int currentUserID))
            {
                MessageBox.Show("ID пользователя должен быть числом");
                return;
            }

            // Проверка пароля
            string password = passwordBox.Password;
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль"); // Исправлена опечатка "Вевдите"
                return;
            }

            try
            {
                // Поиск пользователя (исправлено на Users)
                User user = context.User.FirstOrDefault(u => u.User_ID == currentUserID && u.Password == password);

                if (user != null)
                {
                    OpenUserWindow(user);
                }
                else
                {
                    MessageBox.Show("Неверный ID пользователя или пароль");
                    GenerateCaptcha();
                    captchaTextBox.Text = "";
                    passwordBox.Password = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}");
            }
        }

        private void OpenUserWindow(User user){
            Window nextWindow;

            switch (user.Role?.ToLower()) 
            {
                case "организатор    ":
                    nextWindow = new OrganizerWindow(user);
                    break;
                case "участник       ":
                    nextWindow = new ParticipantsWindow();
                    break;
                default:
                    MessageBox.Show("Неизвестная роль");
                    return ;
                

            }
            nextWindow.Show();
            this.Close();

        }
        
        private void RefreshCaprchaButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            captchaTextBox.Text = "";
        }

        private void GenerateCaptcha()
        {
            captchaCanvas.Children.Clear();

            Random rnd = new Random();
            currentCaptchaText = "";
            for (int i = 0; i < 4; i++)
            {
                currentCaptchaText += rnd.Next(0, 10).ToString();
            }
            for (int i = 0; i < 50; i++)
            {
                Ellipse dot = new Ellipse
                {
                    Width = rnd.Next(2, 6),
                    Height = rnd.Next(2, 6),
                    Fill = new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(100, 256),
                        (byte)rnd.Next(100, 256),
                        (byte)rnd.Next(100, 256))),
                    Opacity = 0.7
                };
                Canvas.SetLeft(dot, rnd.Next(0, 300 - 10));
                Canvas.SetTop(dot, rnd.Next(0, 100 - 10));
                captchaCanvas.Children.Add(dot);
            }
            
            for (int i = 0; i < 4; i++)
            {
                TextBlock digit = new TextBlock
                {
                    Text = currentCaptchaText[i].ToString(),
                    FontSize = rnd.Next(20, 30),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(50, 200),
                        (byte)rnd.Next(50, 200),
                        (byte)rnd.Next(50, 200))),
                    RenderTransform = new RotateTransform(rnd.Next(-15, 15))
                };

                Canvas.SetLeft(digit, 50 + i * 40 + rnd.Next(-5, 5));
                Canvas.SetTop(digit, 50 + rnd.Next(-10, 10));
                captchaCanvas.Children.Add(digit);
            }
        }

        private void toMainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
