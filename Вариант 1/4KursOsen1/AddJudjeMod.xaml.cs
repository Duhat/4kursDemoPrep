using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddJudjeMod.xaml
    /// </summary>
    public partial class AddJudjeMod : Window
    {
        private Entities context;
        private string selectedImagePath;
        public AddJudjeMod()
        {
            InitializeComponent();
            context = new Entities();
            InitializeWindow();
            
        }

        private void InitializeWindow()
        {
            LoadComboBoxData();
            int newUserID = GenerateID();
            idTextBox.Text = newUserID.ToString();

        }
        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка полов
                sexComboBox.ItemsSource = context.Sex.ToList();
                sexComboBox.DisplayMemberPath = "Name";
                sexComboBox.SelectedValuePath = "Sex_ID";

                // Загрузка направлений (секций)
                sectionComboBox.ItemsSource = context.Section.ToList();
                sectionComboBox.DisplayMemberPath = "Name";
                sectionComboBox.SelectedValuePath = "Section_ID";

                // Загрузка мероприятий
                eventComboBox.ItemsSource = context.Event.ToList();
                eventComboBox.DisplayMemberPath = "Name";
                eventComboBox.SelectedValuePath = "Event_ID";

                // Установка ролей
                roleComboBox.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                User newUser = new User
                {
                    User_ID = Convert.ToInt32(idTextBox.Text),
                    Full_Name = fullNameTextBox.Text.Trim(),
                    Date_of_Birth = dateOfBirthPicker.SelectedDate.Value,
                    Role = (roleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Email = emailTextBox.Text.Trim(),
                    Phone_Number = FormatPhoneNumber(phoneTextBox.Text),
                    Password = passwordBox.Password,
                    Foto = SaveUserImage()
                };

                AddUserRelationships(newUser);

                context.User.Add(newUser);
                context.SaveChanges();
                MessageBox.Show("Пользователь успешно создан!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(fullNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО");
                return false;
            }

            // Проверка даты рождения
            if (dateOfBirthPicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату рождения");
                return false;
            }

            

            // Проверка пола
            if (sexComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пол");
                return false;
            }

            // Проверка email
            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email");
                return false;
            }

            // Проверка телефона
            if (!IsValidPhoneNumber(phoneTextBox.Text))
            {
                MessageBox.Show("Введите корректный номер телефона");
                return false;
            }

            // Проверка направления
            if (newSectionCheckBox.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(newSectionTextBox.Text))
                {
                    MessageBox.Show("Введите название нового направления");
                    return false;
                }
            }
            else
            {
                if (sectionComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите направление");
                    return false;
                }
            }

            // Проверка пароля
            if (!ValidatePassword(passwordBox.Password))
            {
                MessageBox.Show("Пароль не соответствует требованиям");
                return false;
            }

            if (passwordBox.Password != passwordBoxCheck.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            
            string cleanPhone = phone.Replace("+7", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            return cleanPhone.Length == 10 && cleanPhone.All(char.IsDigit);
        }

        private string FormatPhoneNumber(string phone)
        {
            string cleanPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (cleanPhone.StartsWith("7") && cleanPhone.Length == 11)
                cleanPhone = cleanPhone.Substring(1);

            if (cleanPhone.Length == 10)
            {
                return $"+7({cleanPhone.Substring(0, 3)})-{cleanPhone.Substring(3, 3)}-{cleanPhone.Substring(6, 2)}-{cleanPhone.Substring(8, 2)}";
            }
            return phone;
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 6)
                return false;

            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower))
                return false;

            if (!password.Any(char.IsDigit))
                return false;

            // Проверка на спецсимволы
            var regex = new Regex(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
            if (!regex.IsMatch(password))
                return false;

            return true;
        }

        private void AddUserRelationships(User user)
        {
            // Добавление пола
            if (sexComboBox.SelectedValue != null)
            {
                var selectedSex = context.Sex.Find(sexComboBox.SelectedValue);
                if (selectedSex != null)
                {
                    user.Sex.Add(selectedSex);
                }
            }

            // Добавление направления (секции)
            if (newSectionCheckBox.IsChecked == true)
            {
                // Создание нового направления
                Section newSection = new Section
                {
                    Name = newSectionTextBox.Text.Trim()
                };
                context.Section.Add(newSection);
                context.SaveChanges();
                user.Section.Add(newSection);
            }
            else if (sectionComboBox.SelectedValue != null)
            {
                var selectedSection = context.Section.Find(sectionComboBox.SelectedValue);
                if (selectedSection != null)
                {
                    user.Section.Add(selectedSection);
                }
            }

            // Привязка к мероприятию если отмечен чекбокс
            if (linkCheckBox.IsChecked == true && eventComboBox.SelectedValue != null)
            {
                User_Event userEvent = new User_Event
                {
                    User_ID = user.User_ID,
                    Event_ID = (int)eventComboBox.SelectedValue
                };
                context.User_Event.Add(userEvent);
            }
        }

        private string SaveUserImage()
        {
            if (string.IsNullOrEmpty(selectedImagePath))
                return null;

            try
            {
                string projectPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                string imgFolder = System.IO.Path.Combine(projectPath, "img");

                if (!Directory.Exists(imgFolder))
                    Directory.CreateDirectory(imgFolder);

                string fileName = $"user_{idTextBox.Text}{System.IO.Path.GetExtension(selectedImagePath)}";
                string destinationPath = System.IO.Path.Combine(imgFolder, fileName);

                File.Copy(selectedImagePath, destinationPath, true);
                return fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении фото: {ex.Message}");
                return null;
            }
        }

        public int GenerateID()
        {
            using (var context = new Entities())
            {
                int maxId = context.User.Any() ? context.User.Max(u => u.User_ID) : 0;
                return maxId + 1;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Обработчики событий для телефона
        private void PhoneTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (phoneTextBox.Text == "+7(___)___-__-__")
            {
                phoneTextBox.Text = "+7";
                phoneTextBox.CaretIndex = phoneTextBox.Text.Length;
            }
        }

        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(phoneTextBox.Text) || phoneTextBox.Text == "+7")
            {
                phoneTextBox.Text = "+7(___)___-__-__";
            }
            else
            {
                phoneTextBox.Text = FormatPhoneNumber(phoneTextBox.Text);
            }
        }

        private void PhoneTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Обработчики для чекбоксов
        private void NewSectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            sectionComboBox.IsEnabled = false;
            newSectionTextBox.Visibility = Visibility.Visible;
            newSectionTextBox.Clear();
        }

        private void NewSectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            sectionComboBox.IsEnabled = true;
            newSectionTextBox.Visibility = Visibility.Collapsed;
        }

        private void LinkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            eventComboBox.IsEnabled = true;
        }

        private void LinkCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            eventComboBox.IsEnabled = false;
            eventComboBox.SelectedIndex = -1;
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        #region Обработчики для пароля
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePasswordStrength(passwordBox.Password);
            CheckPasswordMatch();
        }

        private void PasswordBoxCheck_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckPasswordMatch();
        }

        private void ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                passwordErrorText.Text = "";
                return;
            }

            List<string> errors = new List<string>();

            if (password.Length < 6)
                errors.Add("• Менее 6 символов");

            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower))
                errors.Add("• Нет заглавных и строчных букв");

            if (!password.Any(char.IsDigit))
                errors.Add("• Нет цифр");

            var regex = new Regex(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
            if (!regex.IsMatch(password))
                errors.Add("• Нет спецсимволов");

            passwordErrorText.Text = string.Join("\n", errors);
        }

        private void CheckPasswordMatch()
        {
            if (string.IsNullOrEmpty(passwordBoxCheck.Password))
            {
                passwordMatchText.Text = "";
                return;
            }

            if (passwordBox.Password == passwordBoxCheck.Password)
            {
                passwordMatchText.Text = "Пароли совпадают";
                passwordMatchText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                passwordMatchText.Text = "Пароли не совпадают";
                passwordMatchText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
        #endregion

        private void UserFotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png",
                Title = "Выберите фото пользователя"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedImagePath = openFileDialog.FileName;
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedImagePath);
                    bitmap.EndInit();
                    userImage.Source = bitmap;
                    userImage.Visibility = Visibility.Visible;
                    userFotoButton.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }
    }
}
