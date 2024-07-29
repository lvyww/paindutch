using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

namespace TypeB
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WinArticle : Window
    {
        public WinArticle()
        {
            InitializeComponent();
        }

        private void InitTxtFiles()
        {

       
            CbFiles.ItemsSource = ArticleManager.Articles.Keys;

            string cur = ArticleManager.Title;
            if (CbFiles.Items.Contains(cur))
            {
                CbFiles.SelectedItem = cur;
            }
            CbFiles.Items.Refresh();
        }

        bool AllLoaded;
        private void InitControls()
        {



            SldSecLen.Value = ArticleManager.SectionSize;


            CbFilter.IsChecked = ArticleManager.EnableFilter;
            CbRemoveSpace.IsChecked = ArticleManager.RemoveSpace;


        }

        public void UpdateDisplay()
        {
            
            SldProgress.Maximum = ArticleManager.MaxIndex;

            if (SldProgress.Maximum != ArticleManager.MaxIndex)
                SldProgress.Maximum = ArticleManager.MaxIndex;

            if (SldProgress.Value != ArticleManager.Index)
                SldProgress.Value = ArticleManager.Index;


            TbTest.Text = ArticleManager.GetFormattedCurrentSection();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTxtFiles();
            InitControls();
            UpdateDisplay();
            AllLoaded = true;


        }

       
        private void Reload()
        {
            ArticleManager.ReadFiles();
            AllLoaded = false;
            InitTxtFiles();
            InitControls();
            UpdateDisplay();
            AllLoaded = true;

        }



 
        private void CbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllLoaded)
            {
                ArticleManager.Title = CbFiles.SelectedItem.ToString();



            }

        }

        private void SldSecLen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AllLoaded)
            {
                ArticleManager.SectionSize = (int)SldSecLen.Value;

            }

        }



        private void SldProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AllLoaded)
            {
     //           ArticleManager.Progress = (ArticleManager.SectionSize * (int)(SldProgress.Value - 1));

  
            }

        }

        private void Search()
        {
            if (TbSearch.Text == "")
                return;

            int startindex = 0;
     //       if (ArticleManager.Progress > 0)
               startindex =  Math.Min(ArticleManager.Progress + ArticleManager.SectionSize, ArticleManager.TotalSize - 1);

            int s = ArticleManager.Search(TbSearch.Text, startindex);
            if (s >= 0)
            {
                ArticleManager.Progress = s;
          //      UpdateDisplay(); UpdateTxt();
            }
            else if (ArticleManager.Progress > 0)
            {
                if (MessageBox.Show("查找不到，是否从头开始查找？", "查找", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    int s0 = ArticleManager.Search(TbSearch.Text, 0);
                    if (s0 >= 0)
                    {
                        ArticleManager.Progress = s0;
                //        UpdateDisplay(); UpdateTxt();
                    }
                    else
                    {
                        MessageBox.Show("查找不到", "查找", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("查找不到", "查找", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        public void Prev()
        {
            ArticleManager.PrevSection();
       //     UpdateDisplay(); UpdateTxt();
        }
        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        public void Next()
        {
            ArticleManager.NextSection();
      //      UpdateDisplay(); UpdateTxt();
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + "文章" ;
            Process.Start(folderPath);
        }



        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {

            ((MainWindow)App.Current.Windows[0]).SendArticle();
         //   MainWindow.SendArticle();

        }



        private void CbFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (AllLoaded)
            {
                ArticleManager.EnableFilter = (CbFilter.IsChecked == true);
                Reload();
            }

        }

        private void CbFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AllLoaded)
            {
                ArticleManager.EnableFilter = (CbFilter.IsChecked == true);
                Reload();
            }

        }

        private void CbRemoveSpace_Checked(object sender, RoutedEventArgs e)
        {
            if (AllLoaded)
            {
                ArticleManager.RemoveSpace = (CbRemoveSpace.IsChecked == true);
                Reload();
            }
        }

        private void CbRemoveSpace_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AllLoaded)
            {
                ArticleManager.RemoveSpace = (CbRemoveSpace.IsChecked == true);
                Reload();
            }
        }

        private void TbSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search  ();
            }
        }

        private void SldProgress_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (AllLoaded)
            {
               ArticleManager.Progress = (ArticleManager.SectionSize * (int)(SldProgress.Value - 1));

            }
        }


    }
}

