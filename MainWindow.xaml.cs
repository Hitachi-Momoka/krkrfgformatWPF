using Li.Krkr.krkrfgformatWPF;
using Li.Krkr.krkrfgformatWPF.ViewModes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Li.Krkr.Fgformat
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isFullWindow = false;
        public MainWindow()
        {
            InitializeComponent();
            this.SetSelectedItemBinding();
            this.DataContext = new MainWindowViewModel();
        }
        private void addNewBox_Click(object sender, RoutedEventArgs e)
        {
            var index = fileGrid.Children.IndexOf((Button)sender);
            var box1 = new ListBox()
            {
                Style = (Style)FindResource("mylistbox") ?? default
            };
            fileGrid.Children.Insert(index, box1);
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            listBox.ItemsSource = null;
            var array = (string[])e.Data.GetData(DataFormats.FileDrop);
            var obsList = new ObservableCollection<string>();
            foreach (var item in array)
            {
                string ext = System.IO.Path.GetExtension(item).ToLower();
                if (ext ==".png"||ext==".tlg")
                {
                    obsList.Add(item);
                }
            }
            listBox.ItemsSource = obsList;
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void SetSelectedItemBinding()
        {
            foreach (var item in this.fileGrid.Children)
            {
                if(item is ListBox)
                {
                    ListBox listBox = item as ListBox;
                    int index = this.fileGrid.Children.IndexOf(listBox);
                    listBox.SetBinding(ListBox.SelectedItemProperty, 
                        new Binding() { Path = new PropertyPath("SelectItemTmp"), 
                            Mode = BindingMode.TwoWay,
                            Converter = new SelectedItemCombineIndexConverter(),
                            ConverterParameter = index 
                        });
                }
            }
        }

        private void clearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.fileGrid.Children)
            {
                if (item is ListBox)
                {
                    ListBox listBox = item as ListBox;
                    listBox.ItemsSource = null;
                }
            }
            slider1.Value = slider1.Minimum;
        }

        private void clearSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.fileGrid.Children)
            {
                if (item is ListBox)
                {
                    ListBox listBox = item as ListBox;
                    listBox.SelectedIndex = -1;
                }
            }
            slider1.Value = slider1.Minimum;
        }

        private void g1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta>0)
            {
                if (slider1.Value + 50 <= slider1.Maximum)
                    slider1.Value += 50;
                else
                    slider1.Value = slider1.Maximum;
            }
            if(e.Delta<0)
            {
                if (slider1.Value - 50 >= slider1.Minimum)
                    slider1.Value -= 50;
                else
                    slider1.Value = slider1.Minimum;
            }
            e.Handled = true;
        }

        private void imageboxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(!this.isFullWindow)
            {
                this.picBoxBorder.Visibility = Visibility.Collapsed;
                Grid.SetColumn(this.g1, 0);
                Grid.SetColumnSpan(this.g1, 2);
                this.controlGrid.Visibility = Visibility.Collapsed;
                this.isFullWindow = true;
            }else
            {
                this.controlGrid.Visibility = Visibility.Visible;
                Grid.SetColumn(this.g1, 1);
                Grid.SetColumnSpan(this.g1, 1);
                this.picBoxBorder.Visibility = Visibility.Visible;
                this.isFullWindow = false;
            }
        }
    }
}
