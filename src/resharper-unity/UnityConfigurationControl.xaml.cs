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

namespace JetBrains.ReSharper.Plugins.Unity
{
    /// <summary>
    /// Interaction logic for UnityConfigurationControl.xaml
    /// </summary>
    public partial class UnityConfigurationControl : UserControl
    {
        public UnityConfigurationControl()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            UnityClassesListBox.Items.Add("New Class");
        }

        private void Label_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void UnityClassesListBox_OnSelected(object sender, RoutedEventArgs e)
        {
        }
    }
}
