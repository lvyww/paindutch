using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace TypeB
{
    /// <summary>
    /// WinConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WinConfig : Window
    {
        public WinConfig()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] SkipedConfigItems =
            {
                "窗口坐标X","   447",
                "窗口坐标Y"   ,"119",
                "窗口高度", "600",
                "窗口宽度", "800",
                "字体大小", "30",
                "字体", "微软雅黑",
                "盲打模式", "否",
                "看打模式", "否",
                "成绩面板展开", "是",
                "获取更新", "QQ群775237860",
                "QQ窗口切换模式(1-2)","1",
                "载文模式(1-4)", "1",
                 "新版QQ", "否",
                 "开启程序调试Log", "否"

            };

            int counter = 0;
            var mg = new Thickness(10,3,10, 3);
            foreach(var item in Config.dicts)
            {

                if (SkipedConfigItems.Contains(item.Key))
                    continue;

                GridMain.RowDefinitions.Add(new RowDefinition());

                TextBlock tbk = new TextBlock();
                tbk.Text = item.Key;


         //       tbk.HorizontalAlignment = HorizontalAlignment.Center;
                tbk.VerticalAlignment = VerticalAlignment.Center;
                tbk.SetValue(Grid.RowProperty, counter);
                tbk.SetValue(Grid.ColumnProperty, 0);
                tbk.Margin = mg;
                tbk.FontSize = 14;

                GridMain.Children.Add(tbk);


               if (item.Value =="是" || item.Value == "否")
                {
                    CheckBox tbv = new CheckBox();
                    tbv.IsChecked = item.Value == "是";
                    tbv.VerticalAlignment = VerticalAlignment.Center;
                    tbv.SetValue(Grid.RowProperty, counter);
                    tbv.SetValue(Grid.ColumnProperty, 1);
                    tbv.Margin = mg;
                    tbv.FontSize = 14;

                    GridMain.Children.Add(tbv);
                }
               else
                {
                    TextBox tbv = new TextBox();
                    tbv.Text = item.Value;
                    //       tbv.HorizontalAlignment = HorizontalAlignment.Center;
                    tbv.VerticalAlignment = VerticalAlignment.Center;
                    tbv.SetValue(Grid.RowProperty, counter);
                    tbv.SetValue(Grid.ColumnProperty, 1);
                    tbv.Margin = mg;
                    tbv.FontSize = 14;

                    GridMain.Children.Add(tbv);
                }



                counter++;
            }
        }



        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
 

            this.Close();
        }

        public delegate void DelegateConfigSaved();

        public event DelegateConfigSaved ConfigSaved;
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            List<string> key = new List<string>();
            List<string> value = new List<string>();
            foreach (var item in GridMain.Children)
            {

                if (item.GetType() == typeof(TextBlock))
                {
                    var tb = (TextBlock)item;
                    key.Add(tb.Text);
                }
                if (item.GetType() == typeof(TextBox))
                {
                    var tb = (TextBox)item;
                    value.Add(tb.Text);
                }
                else if (item.GetType() == typeof(CheckBox))
                {
                    var tb = (CheckBox)item;
                    if (tb.IsChecked == true)
                        value.Add("是");
                    else 
                        value.Add("否");
                }

            }

            bool modified = false;
            for (int i = 0; i < key.Count; i++)
            {

                if (value[i] != Config.GetString(key[i]))
                {
                    modified = true;
                    Config.Set(key[i], value[i]);
                }

            }
            if (modified) 
            {
                ConfigSaved();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<string> key = new List<string>();
            List<string> value = new List<string>();
            foreach (var item in GridMain.Children)
            {

                if (item.GetType() == typeof(TextBlock))
                {
                    var tb = (TextBlock)item;
                    key.Add(tb.Text);
                }
                if (item.GetType() == typeof(TextBox))
                {
                    var tb = (TextBox)item;
                    value.Add(tb.Text);
                }
                else if (item.GetType() == typeof(CheckBox))
                {
                    var tb = (CheckBox)item;
                    if (tb.IsChecked == true)
                        value.Add("是");
                    else
                        value.Add("否");
                }

            }

            bool modified = false;
            for (int i = 0; i < key.Count; i++)
            {

                if (value[i] != Config.GetString(key[i]))
                {
                    modified = true;
                }

            }
            if (modified)
            {
                if (MessageBox.Show("设置已修改，是否保存？",
                                    "保存设置",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    for (int i = 0; i < key.Count; i++)
                    {
                        if (value[i] != Config.GetString(key[i]))
                        {
                            Config.Set(key[i], value[i]);
                        }

                    }

                    ConfigSaved();

                }
            }
        }
    }
}
