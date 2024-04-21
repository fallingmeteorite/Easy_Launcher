using System;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;
using static Awake.initialize;//这里引入全局参数库
namespace Awake.Views.Pages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataPage : INavigableView<ViewModels.DataViewModel>
    {
        public ViewModels.DataViewModel ViewModel
        { get; }
        public DataPage(ViewModels.DataViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            GetSystemInfo();
            foreach (string 显卡名 in initialize.显卡列表)
            {
                try
                {
                    显卡选择器.Items.Add(显卡名);
                    显卡选择器.SelectedIndex = initialize.显卡列表.Count - 1;
                }
                catch (Exception ex) { }
            }
            initialize._GPUname = 显卡选择器.SelectedItem.ToString();
            initialize._UseGPUindex = 显卡选择器.SelectedIndex;
        }

        public async void GetSystemInfo()
        {
            string cpuname = await Task.Run(() => hardinfo.GetCpuName());
            string Machinename = await Task.Run(() => hardinfo.GetComputerName());
            string systemType = await Task.Run(() => hardinfo.GetSystemType());
            float memorysize = await Task.Run(() => hardinfo.GetPhysicalMemory());
            int memorynum = await Task.Run(() => hardinfo.MemoryNumberCount());
            string gpuname = await Task.Run(() => hardinfo.GPUName());
            计算机CPU信息.Text = "CPU信息：" + cpuname;
            计算机名称类型.Text = "系统名称：" + Machinename + "   系统类型：" + systemType;
            计算机内存信息.Text = "内存信息：" + memorynum + " 插槽" + "  共计" + memorysize + " GB";
            计算机显卡信息.Text = "显卡信息：" + gpuname;
        }

        //这里是一些开关的控制器组件
        private void 启动后自动打开浏览器开关_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (启动后自动打开浏览器开关.IsChecked == true)
            {
                initialize.浏览器启动 = true;
        
            }
            else
            {
                initialize.浏览器启动 = false;
              
            }
        }
 
        private void 使用CPU进行推理_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (使用CPU进行推理.IsChecked == true)
            { 
                initialize.使用CPU进行推理 = true; 
            }
            else 
            { 
                initialize.使用CPU进行推理 = false;
            }
        }

        private void 关闭模型hash计算_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (关闭模型hash计算.IsChecked == true)
            {
                initialize.关闭模型hash计算 = true;
            }
            else
            {
                initialize.关闭模型hash计算 = false;
            }
        }


        private void 性能优化器配置面板_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            性能优化器开关面版.Show();
        }
        //通用
        private void 上投采样_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (上投采样.IsChecked == false)
            {
                initialize.上投采样 = false;
            }
            else
            {
                initialize.上投采样 = true;
            }
        }

        private void 关闭半精度计算_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (关闭半精度计算.IsChecked == false)
            { initialize.关闭半精度计算 = false; }
            else
            { initialize.关闭半精度计算 = true; }
        }


        //A卡|I卡
        private void 启用InvokeAI_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (启用InvokeAI.IsChecked == false)
            { initialize.启用InvokeAI = false; }
            else
            { initialize.启用InvokeAI = true; }
        }




        //N卡
        private void 缩放点积_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (缩放点积.IsChecked == false)
            { 
                initialize.缩放点积 = false;
            }
            else
            {
                initialize.缩放点积 = true; 
            }
        }

        private void SDP优化_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SDP优化.IsChecked == false)
            { 
                initialize.SDP优化 = false; 
            }
            else
            {
                initialize.SDP优化 = true;
            }
        }

        private void 启用xformers_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (启用xformers.IsChecked == false)
            {
                initialize.启用xformers = false;
            }
            else
            {
                initialize.启用xformers = true;
            }
        }

        private void 启用替代布局_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (启用替代布局.IsChecked == false)
            {
                initialize.启用替代布局 = false;
            }
            else
            {
                initialize.启用替代布局 = true;
            }
        }




        private void WebUI显存压力优化设置_DropDownClosed(object sender, EventArgs e)
        {
            if (WebUI显存压力优化设置.SelectedIndex == 0)
            {
                _WebUI显存压力优化设置 = " --lowvram";
            }
            if (WebUI显存压力优化设置.SelectedIndex == 1)
            {
                _WebUI显存压力优化设置 = " --medvram";
            }
            if (WebUI显存压力优化设置.SelectedIndex == 2)
            {
                _WebUI显存压力优化设置 = "";
            }
        }

        private void 显卡类型_DropDownClosed(object sender, EventArgs e)
        {
            if (显卡类型.SelectedIndex == 0)
            {
                initialize.显卡类型名 = "Intel";
            }
            if (显卡类型.SelectedIndex == 1)
            {
                initialize.显卡类型名 = "NVIDIA";
            }
            if (显卡类型.SelectedIndex == 2)
            {
                initialize.显卡类型名 = "Radeon";
            }
            显卡选择器.Items.Clear();
            foreach (string 显卡名 in initialize.显卡列表)
            {
                try
                {
                    if (显卡名.Contains(initialize.显卡类型名))
                    {
                        显卡选择器.Items.Add(显卡名);
                        显卡选择器.SelectedIndex = initialize.显卡列表.Count - 1;
                    }
                    
                }
                catch (Exception ex) { }
            }
            if (显卡选择器.Items.Count == 0)
            {
                显卡选择器.Items.Add("无可用显卡");
            }
        }


        private void 显卡选择器_DropDownClosed(object sender, EventArgs e)
        {
            initialize._GPUname = 显卡选择器.SelectedItem.ToString();
            initialize._UseGPUindex = 显卡选择器.SelectedIndex;
        }

        private void 开启API_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (启动api == false)
            { 启动api = true; }
            else
            { 启动api = false; }
        }

        private void WebUI主题颜色设置_DropDownClosed(object sender, EventArgs e)
        {
            if (WebUI主题颜色设置.SelectedIndex == 0) { }
            if (WebUI主题颜色设置.SelectedIndex == 1)
            { initialize._WebUI主题颜色 = " --theme light"; }
            if (WebUI主题颜色设置.SelectedIndex == 2)
            { initialize._WebUI主题颜色 = " --theme dark"; }
        }

        private void 分享WebUI到公网_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (分享WebUI到公网.IsChecked == false)
            { initialize.分享WebUI到公网 = true; }
            else
            { initialize.分享WebUI到公网 = false; }
        }


        private void 快速启动_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (initialize.快速启动 == false)
            { 
                initialize.快速启动 = true; 
            }
            else
            { 
                initialize.快速启动 = false; 
            }
        }




    }
}
