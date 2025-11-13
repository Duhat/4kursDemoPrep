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

namespace _4KursOsen1
{
    /// <summary>
    /// Логика взаимодействия для OrganizerWindow.xaml
    /// </summary>
    public partial class OrganizerWindow : Window
    {
        
        private User currentUser;
        public OrganizerWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            InitializeUserData();
            
           
        }

        private void InitializeUserData()
        {
            string imagePath = "\\img\\";
            
            
            

            //userImage.Source = imagePath;

            if (!string.IsNullOrEmpty(currentUser.Foto))
            {
                try
                {
                    
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(currentUser.Foto, UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    userImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    
                    LoadDefaultImage();
                }
            }
            else
            {
                
                LoadDefaultImage();
            }

            
            welcomeTextBlock.Text = $"{currentUser.Full_Name}!";
            int currentHour = DateTime.Now.Hour;
            if (currentHour >= 9 && currentHour < 11)
            {
                timeTextBlock.Text = "Доброе утро";
            }
            else if (currentHour >= 11 && currentHour < 18) {
                timeTextBlock.Text = "Добрый день";
            } 
            else if (currentHour >= 18 && currentHour <= 24) {
                timeTextBlock.Text = "Добрый вечер";
            } else
            {
                timeTextBlock.Text = "Здравствуйте";
            }
        }

        private void LoadDefaultImage()
        {
            try
            {
                
                BitmapImage defaultImage = new BitmapImage();
                defaultImage.BeginInit();
                defaultImage.UriSource = new Uri("img/foto1.jpg", UriKind.RelativeOrAbsolute);
                defaultImage.EndInit();
                userImage.Source = defaultImage;
            }
            catch (Exception ex)
            {
                
                userImage.Source = null;
                MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}");
            }
        }
        private void ParticipantsButton_Click(object sender, RoutedEventArgs e)
        {
            ParticipantsWindow participantsWindow = new ParticipantsWindow();
            participantsWindow.Show();
            

        }
        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            EventsWindow eventsWindow = new EventsWindow();
            eventsWindow.Show();
            

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            JudjeModListWindow judjeModListWindow = new JudjeModListWindow();
            judjeModListWindow.Show();
        }

        private void toProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
            
        }

        
    }
}
