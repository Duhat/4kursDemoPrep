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
    /// Логика взаимодействия для JudjeModListWindow.xaml
    /// </summary>
    public partial class JudjeModListWindow : Window
    {
        public JudjeModListWindow()
        {
            InitializeComponent();
            modsJudjsDataGrid.ItemsSource = Entities.GetContext().User.Where(u => u.Role == "жюри" || u.Role == "модератор").ToList();
        }

        private void toAddButton_Click(object sender, object e)
        {
            AddJudjeMod addJudjeMod = new AddJudjeMod();
            addJudjeMod.Show();
        }
    }
}
