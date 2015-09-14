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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Threading;

namespace GettingStarted
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;
        private DispatcherTimer timer;
        private ProcessCount processCount;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            this.Loaded += new RoutedEventHandler(MainWin_Loaded);
        }


        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {

            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
        }
        /// <summary> 
        /// 窗口加载事件 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        private void MainWin_Loaded(object sender, RoutedEventArgs e)////////////////////////////////////////
        {
            //设置定时器 
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000000); //时间间隔为一秒 
            timer.Tick += new EventHandler(timer_Tick);
            //转换成秒数 
            //Int32 hour = Convert.ToInt32(HourArea.Text);
            //Int32 minute = Convert.ToInt32(MinuteArea.Text);
            Int32 second = Convert.ToInt32(SecondArea.Text);
            //处理倒计时的类 
            processCount = new ProcessCount(second);
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
                //HourArea.Text = processCount.GetHour();
                //MinuteArea.Text = processCount.GetMinute();
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
        
        
   
        
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    //try
                    //{
                    //    args.NewSensor.DepthStream.Range = DepthRange.Near;
                    //    args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    //    args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    //}
                    //catch (InvalidOperationException)
                    //{
                    //    // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                    //    args.NewSensor.DepthStream.Range = DepthRange.Default;
                    //    args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    //    error = true;
                    //}
                }
                catch (InvalidOperationException)
                {
                    error = true;
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (!error)
                kinectRegion.KinectSensor = args.NewSensor;
        }

        private void ButtonOnClickA(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The Answer a is wrong");
            this.Close();
        }
        private void ButtonOnClickB(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The Answer b is wrong");
            this.Close();
        }
        private void ButtonOnClickC(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The Answer c is right");
            this.Close();
        }
        //public delegate bool CountDownHandler();
    }

    public delegate bool CountDownHandler(); ///////////////////////////
}
