using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Advisor1.xaml
    /// </summary>
    public partial class Advisor1 : Window
    {
        private DispatcherTimer timer;
        private ProcessCount processCount;
        public Advisor1()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWin_Loaded);
           
        }
        /// 窗口加载事件 
/// </summary> 
/// <param name="sender"></param> 
/// <param name="e"></param> 
private void MainWin_Loaded(object sender, RoutedEventArgs e) 
{ 
//设置定时器 
timer = new DispatcherTimer(); 
timer.Interval = new TimeSpan(10000000); //时间间隔为一秒 
timer.Tick += new EventHandler(timer_Tick); 
//转换成秒数 
Int32 hour= Convert.ToInt32(HourArea.Text); 
Int32 minute = Convert.ToInt32(MinuteArea.Text); 
Int32 second = Convert.ToInt32(SecondArea.Text); 
//处理倒计时的类 
processCount = new ProcessCount(hour*3600+minute*60+second); 
CountDown += new CountDownHandler(processCount.ProcessCountDown); 
//开启定时器 
timer.Start(); 
} 
/// <summary> 
/// Timer触发的事件 
/// </summary> 
/// <param name="sender"></param> 
/// <param name="e"></param> 
private void timer_Tick(object sender, EventArgs e) 
{
    if (OnCountDown())
    {
        HourArea.Text = processCount.GetHour();
        MinuteArea.Text = processCount.GetMinute();
        SecondArea.Text = processCount.GetSecond();
    }
    else
        //timer.Stop(); 
        this.Close();
} 
/// <summary> 
/// 处理事件 
/// </summary> 
public event CountDownHandler CountDown; 
public bool OnCountDown() 
{ 
if (CountDown != null) 
return CountDown(); 
return false; 
} 
} 
/// <summary> 
/// 处理倒计时的委托 
/// </summary> 
/// <returns></returns> 
//public delegate bool CountDownHandler(); 
    }

