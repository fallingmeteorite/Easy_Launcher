using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using static Awake.Initialize;
using Application = System.Windows.Application;

namespace Awake.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : INavigationWindow
    {
        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService)
        {
            try
            {
                ViewModel = viewModel;
                DataContext = this;

                InitializeComponent();
                Loaded += (sender, args) =>
                {
                    Wpf.Ui.Appearance.Watcher.Watch(
                        this,                                  // Window class
                        Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                        true                                   // Whether to change accents automatically
                    );
                };
                GetSystemInfo();
                CheckStartPathFile();//从log读取工作路径
                if (File.Exists(@".AI_launther_log\UIalpha.txt"))
                {
                    string alphaStringent = File.ReadAllText(@".AI_launther_log\UIalpha.txt");
                    背景亮度 = int.Parse(alphaStringent);
                    图片亮度.Value = int.Parse(alphaStringent);
                }

                async void GetSystemInfo()
                {

                    string gpuname = await Task.Run(() => Hardinfo.getGPUNamelist());//这个函数内部对GPU写入了List,仅return GPUname供展示
                    try
                    {
                        _UseGPUindex = Initialize.显卡列表.Count - 1;//默认启动后选最后一张GPU，核显什么的排在前面
                    }
                    catch (Exception ex) { }


                    _GPUname = Initialize.显卡列表.Last();//开始对GPUname判断
                    if (_GPUname.Contains("Radeon"))
                    {
                        _显卡类型 = "Radeon";
                        A卡模式 = true;
                        XF加速模式 = false;

                    }

                    if (_GPUname.Contains("NVIDIA"))
                    {
                        _显卡类型 = "NVIDIA";
                        N卡模式 = true;
                        XF加速模式 = true;

                    }

                    //计算机名称类型.Text = "系统名称：" + Machinename + "   系统类型：" + systemType;
                    //计算机内存信息.Text = "内存信息：" + memorynum + " 插槽" + "  共计" + memorysize + " GB";
                    //计算机显卡信息.Text = "显卡信息：" + gpuname;
                }

                if (File.Exists(@".AI_launther_log\UI.txt"))
                {
                    // 如果文件存在，读取其中的内容到gitpath全局变量中
                    背景颜色 = File.ReadAllText(@".AI_launther_log\UI.txt");
                    if (File.Exists(@".AI_launther_log\UIpicture.txt"))
                    {
                        // 如果文件存在，读取其中的内容到gitpath全局变量中
                        背景图片 = File.ReadAllText(@".AI_launther_log\UIpicture.txt");
                    }



                    if (背景颜色 == "Mica")
                    {
                        basewindow.WindowBackdropType = BackgroundType.Mica;
                        主题背景图.Opacity = 0;
                        Mica.IsChecked = true;

                    }
                    if (背景颜色 == "Acrylic")
                    {
                        basewindow.WindowBackdropType = BackgroundType.Acrylic;
                        主题背景图.Opacity = 0;
                        Acrylic.IsChecked = true;
                    }
                    if (背景颜色 == "Tabbed")
                    {
                        basewindow.WindowBackdropType = BackgroundType.Tabbed;
                        主题背景图.Opacity = 0;
                        Tabbed.IsChecked = true;
                    }
                    if (背景颜色 == "Auto")
                    {
                        basewindow.WindowBackdropType = BackgroundType.Auto;
                        主题背景图.Opacity = 0;
                        Auto.IsChecked = true;
                    }
                    if (背景颜色 == "None")
                    {
                        basewindow.WindowBackdropType = BackgroundType.None;
                        主题背景图.Opacity = 0;
                        None.IsChecked = true;
                    }
                    if (背景颜色 == "Picture")
                    {
                        try
                        {
                            if (File.Exists(@".AI_launther_log\UIalpha.txt"))
                            {
                                string alphaStringent = File.ReadAllText(@".AI_launther_log\UIalpha.txt");
                                背景亮度 = int.Parse(alphaStringent);

                                图片亮度.Value = int.Parse(alphaStringent);
                                主题背景图.Opacity = 图片亮度.Value / 100; ;
                                string imagepath = 背景图片; // 获取选择的文件路径+文件名  
                                ImageSource imageSource = new BitmapImage(new Uri(imagepath)); // 设置Image的ImageSource为选择的图片  
                                主题背景图.ImageSource = imageSource; // 将选择的图片显示在Image控件中  

                            }
                        }
                        catch (Exception) { }

                    }
                }

                SetPageService(pageService);

                navigationService.SetNavigationControl(RootNavigation);

                _loadpage();
                //在启动的时候判断是否下载与安装SDwebUI
                已下载WebUI = CheckWebUIdownloaded();
                已安装WebUI = CheckWebUIinstelled();
                已解压WebUI = CheckWebUIunzip();
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }
        async Task _loadpage()
        {
            try
            {

                // 加载页的背景图计时器
                await Task.Delay(TimeSpan.FromSeconds(2.5));

                // 在UI线程中更改grid的visibility  
                Dispatcher.Invoke(() =>
                {
                    // 创建动画  
                    DoubleAnimation doubleAnimation = new DoubleAnimation();
                    doubleAnimation.From = 1; // 从100%开始  
                    doubleAnimation.To = 0; // 结束时为0%  
                    doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1)); // 动画时间为1秒  
                    doubleAnimation.AutoReverse = false; // 动画结束后自动反转回原来的状态  

                    // 将动画应用到grid的透明度属性上  
                    开屏画面.BeginAnimation(Grid.OpacityProperty, doubleAnimation);
                });
                主显示区.Visibility = Visibility.Visible;
                // 在UI线程中更改grid的visibility  
                Dispatcher.Invoke(() =>
                {
                    // 创建动画  
                    DoubleAnimation doubleAnimation = new DoubleAnimation();
                    doubleAnimation.From = 0; // 从0%开始  
                    doubleAnimation.To = 1; // 结束时为100%  
                    doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1)); // 动画时间为1秒  
                    doubleAnimation.AutoReverse = false; // 动画结束后自动反转回原来的状态  

                    // 将动画应用到grid的透明度属性上  
                    主显示区.BeginAnimation(Grid.OpacityProperty, doubleAnimation);
                });
                开屏画面.Visibility = Visibility.Collapsed;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }

        }

        #region INavigationWindow methods

        public System.Windows.Controls.Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods
        protected override void OnClosed(EventArgs e)
        {
            try
            {
                base.OnClosed(e);
                File.WriteAllText(@".AI_launther_log\UIalpha.txt", 图片亮度.Value.ToString());
                File.WriteAllText(@".AI_launther_log\UI.txt", 背景颜色);
                File.WriteAllText(@".AI_launther_log\UIpicture.txt", 背景图片);
                Application.Current.Shutdown();
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }
        private void 一键启动按钮_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //检查WebUI安装状态
                已下载WebUI = CheckWebUIdownloaded();
                已安装WebUI = CheckWebUIinstelled();
                已解压WebUI = CheckWebUIunzip();

                //通过对是否安装webUI已安装的判断做出提示安装或直接启动的操作
                if (已安装WebUI == false)
                {
                    //探出flyout提示用户去安装
                    安装提示.Show();
                }
                if (已安装WebUI == true)
                {
                    //AI,启动！
                    shell Shell = new shell();//shell会自动读取initilize中的参数变量
                    Shell.Show();
                }
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }
        private void 主题切换_Click(object sender, RoutedEventArgs e)
        {
            主题设置区.Show();
        }
        //下面是主题效果
        private void Mica_Click(object sender, RoutedEventArgs e)
        {
            try//Win10不怎么支持Win11的先进特效
            {
                basewindow.WindowBackdropType = BackgroundType.Mica;
                背景颜色 = "Mica";
                主题背景图.Opacity = 0;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }
        private void Acrylic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                basewindow.WindowBackdropType = BackgroundType.Acrylic;
                背景颜色 = "Acrylic";
                主题背景图.Opacity = 0;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void Tabbed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                basewindow.WindowBackdropType = BackgroundType.Tabbed;
                背景颜色 = "Tabbed";

                主题背景图.Opacity = 0;
            }
            catch { }

        }
        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                basewindow.WindowBackdropType = BackgroundType.Auto;
                背景颜色 = "Auto";

                主题背景图.Opacity = 0;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void None_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                basewindow.WindowBackdropType = BackgroundType.None;
                背景颜色 = "None";

                主题背景图.Opacity = 0;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void Picture_Click(object sender, RoutedEventArgs e)
        {
            主题背景图.Opacity = 图片亮度.Value / 100;
            背景颜色 = "Picture";
            string imagepath = 背景图片; // 获取选择的文件路径+文件名  
            try
            {
                ImageSource imageSource = new BitmapImage(new Uri(imagepath)); // 设置Image的ImageSource为选择的图片  
                主题背景图.ImageSource = imageSource; // 将选择的图片显示在Image控件中  

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("请选择一张图片");

            }
        }

        private void 图片亮度_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                主题背景图.Opacity = 图片亮度.Value / 100;
                背景亮度 = (int)图片亮度.Value;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void 图片亮度_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                主题背景图.Opacity = 图片亮度.Value / 100;
                背景亮度 = (int)图片亮度.Value;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void 图片亮度_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                主题背景图.Opacity = 图片亮度.Value / 100;
                背景亮度 = (int)图片亮度.Value;
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void 选择背景图片_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png"; // 只允许选择图片文件  
                if (ofd.ShowDialog() == true) // 用户选择了一个文件并点击了“确定”按钮  
                {
                    背景图片 = ofd.FileName;
                    string imagepath2 = ofd.FileName; // 获取选择的文件路径+文件名  
                    ImageSource imageSource = new BitmapImage(new Uri(imagepath2)); // 设置Image的ImageSource为选择的图片  
                    主题背景图.ImageSource = imageSource; // 将选择的图片显示在Image控件中  
                }
                主题背景图.Opacity = 图片亮度.Value / 100;
                背景颜色 = "Picture";
                string imagepath = 背景图片; // 获取选择的文件路径+文件名  
                try
                {
                    ImageSource imageSource = new BitmapImage(new Uri(imagepath)); // 设置Image的ImageSource为选择的图片  
                    主题背景图.ImageSource = imageSource; // 将选择的图片显示在Image控件中  

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("请先选择一张图片");

                }
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void Dark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Theme.Apply(ThemeType.Dark);
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void Light_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Theme.Apply(ThemeType.Light);
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }
        }

        private void 明暗切换_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Theme.GetAppTheme() == ThemeType.Dark)
                {
                    Theme.Apply(ThemeType.Light);
                }

                else
                {
                    Theme.Apply(ThemeType.Dark);
                }
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }

        }

        private void SD启动_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_SD启动 == false)
                {
                    _SD启动 = true;
                    SD启动.IsChecked = true;
                }
                else
                {
                    _SD启动 = false;
                    SD启动.IsChecked = false;
                }
            }
            catch (Exception error)
            {
                string str1 = error.Message;
                File.WriteAllText(@".\logs\error.txt", str1);
                throw;
            }

        }
    }


}