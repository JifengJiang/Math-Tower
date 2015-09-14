using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;
using Microsoft.Runtime;
using WpfApplication1.Utils;
using System.Timers;
using System.Diagnostics;
using System.IO;




namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    struct monster
    {
        public int Monster_Id;
        public int Pos_X, Pos_Y;
        public float Point_X, Point_Y;
    }

    //class Process 
    //{
 
    //}

    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
               "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(MainWindow),
                new PropertyMetadata(null));

        #region Private State

        private DispatcherTimer timer;
        private ProcessCount processCount;




        private const int TimerResolution = 2;  // ms
        private const int NumIntraFrames = 3;
        private const int MaxShapes = 80;
        private const double MaxFramerate = 70;
        private const double MinFramerate = 15;

        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();

        private DateTime lastFrameDrawn = DateTime.MinValue;
        private DateTime predNextFrame = DateTime.MinValue;
        private double actualFrameTime;

        private Skeleton[] skeletonData;

        // Player(s) placement in scene (z collapsed):
        private Rect playerBounds;
        private Rect screenRect;

        //private double targetFramerate = MaxFramerate;
        private double targetFramerate = MaxFramerate;
        private int frameCount;
        private bool runningGameThread;
        private int playersAlive;

        int countPlayer = 0;
        int countAction = 0;
        int Done = 0;
        int sit = 0;

        private int skeletonStatus = 0;
        private const int STAND = 1;
        private const int SIT = 2;
        private const int LIEDOWN = 3;
        private const int WALK = 4;

        int diretionStatus= 0;
        private const int LEFT = 4;
        private const int RIGHT = 3;
        private const int UP = 2;
        private const int DOWN = 1;

        int StatInit = 0;
        const int StatusArrayLen= 90;
        private int[] StatusArray = new int[StatusArrayLen];
        private int[] DirectionArray = new int[20];

        private static System.Timers.Timer tim = new System.Timers.Timer(5000);

        #endregion Private State




        #region Map components
        /// <summary>
        /// Variables of the static blocks shown on the canvas Carrier
        /// </summary>
        //Image block1;        //Image block2;
        //Image block3;
        //Image block4;
        //Set the size of tile as 100 pixels
        const double tile = 100;

        /// <Summary>
        /// Variables of the postion Matrix
        /// Including a 5*5 boolean array: posMatrix
        /// current position: (int)curPos_X, curPos_Y
        /// destination postion: (int)endPos_X, endPos_Y
        /// </Summary>
        /// //Define a 5*5 position matrix to indicate if the position is reachable or not, true is reachable, false is unreachable
        bool[,] posMatrix = new bool[12, 6];
        //Define global variables of curPos_X, curPos_Y
        int curPos_X, curPos_Y;
        int endPos_X = 11, endPos_Y = 0;
        int startPos_X = 0, startPos_Y = 5; 

        /// <Summary>
        /// Variables of the dynamic movement of the image
        /// Including position of start point: (Point)startPoint
        /// position of current point: (Point)currentPoint
        /// moving distance in x-axis and y-axis: (double)distance_X, (double)distance_Y 
        /// directions: movingLeft(1), movingRight(-1), movingUp(2), movingDown(-2)
        /// </Summary>
        //Define start position of the retangle
        Point startPoint;
        Point currentPoint;

        //Define movement distance for x-axie and y-axie
        const double distance_X = tile, distance_Y = tile;

        //Define variable direction to indicate to direction of movement
        int direction;
        //Set values to directions--left:1, right:-1, up:2, down:-2
        const int movingLeft = 1, movingRight = -1, movingUp = 2, movingDown = -2;

        /// <Summary>
        /// Variables of the moving images
        /// Includes: (Image) imageMovement--displaying image on the canvas
        /// (int)count--counting which pictures to be displayed
        /// </Summary>
        //Define the move iamges
        Image imageMovement;
        Image monster1;
        Image monster2;
        Image monster3;
        Image monster4;
        Image monster5;
        //Create a counter to count the number of pics under displaying
        int count = 1;
        #endregion Map Components





        bool showQuesDia = false;
        int Lastsecond = 0;
        
        


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWin_Loaded);


            this.KinectSensorManager = new KinectSensorManager();
            this.KinectSensorManager.KinectSensorChanged += this.KinectSensorChanged;
            this.DataContext = this.KinectSensorManager;
            InitMonster1Image();
            InitMonster2Image();
            InitMonster3Image();
            InitMonster4Image();
            InitMonster5Image();
            InitializeComponent();

            this.SensorChooserUI.KinectSensorChooser = sensorChooser;
            sensorChooser.Start();

            // Bind the KinectSensor from the sensorChooser to the KinectSensor on the KinectSensorManager
            var kinectSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.KinectSensorManager, KinectSensorManager.KinectSensorProperty, kinectSensorBinding);

            this.RestoreWindowState();

            #region Maze Part
            //Set values of posMatrix[5,5]
            setValuesMatrix();
            //set value of startPos(posMatrix[0, 0]), endPos(posMatrix[4, 0])
            int startPos_X = 0, startPos_Y = 5;
            //int endPos_X = 4, endPos_Y = 0;

            //At the beginning, set current position as start position
            curPos_X = startPos_X;
            curPos_Y = startPos_Y;

            //Set value of startPoint in the central of the tile as (25, 25)
            startPoint.X = 25;
            startPoint.Y = 525;

            //Initialize Blocks
            //InitBlocks();

            //Initialize the image
            direction = movingUp;
            initImageMovement();

            //Show message when the player reach the end position
            //if (curPos_X == endPos_X && curPos_Y == endPos_Y)
            //{
            //    MessageBox.Show("Congratulations, you've reached the destination!");
            //}
            #endregion Maze Part




        }
     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void RestoreWindowState()
        {
            // Restore window state to that last used
            Rect bounds = Properties.Settings.Default.PrevWinPosition;
            if (bounds.Right != bounds.Left)
            {
                this.Top = bounds.Top;
                this.Left = bounds.Left;
                this.Height = bounds.Height;
                this.Width = bounds.Width;
            }

            this.WindowState = (WindowState)Properties.Settings.Default.WindowState;
        }

        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        [DllImport("Winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern int TimeBeginPeriod(uint period);


        private void KinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            if (null != args.OldValue)
            {
                this.UninitializeKinectServices(args.OldValue);
            }

            if (null != args.NewValue)
            {
                this.InitializeKinectServices(this.KinectSensorManager, args.NewValue);
            }
        }

        private void WindowLoaded(object sender, EventArgs e)
        {
            playfield.ClipToBounds = true;


            this.UpdatePlayfieldSize();

            TimeBeginPeriod(TimerResolution);
            var myGameThread = new Thread(this.GameThread);
            myGameThread.SetApartmentState(ApartmentState.STA);
            myGameThread.Start();

        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            sensorChooser.Stop();

            this.runningGameThread = false;
            Properties.Settings.Default.PrevWinPosition = this.RestoreBounds;
            Properties.Settings.Default.WindowState = (int)this.WindowState;
            Properties.Settings.Default.Save();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            this.KinectSensorManager.KinectSensor = null;
        }

        private void EnableAecChecked(object sender, RoutedEventArgs e)
        {
            var enableAecCheckBox = (CheckBox)sender;
            //this.UpdateEchoCancellation(enableAecCheckBox);
        }



        private void InitializeKinectServices(KinectSensorManager kinectSensorManager, KinectSensor sensor)
        {
            // Application should enable all streams first.
            kinectSensorManager.ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
            kinectSensorManager.ColorStreamEnabled = true;

            sensor.SkeletonFrameReady += this.SkeletonsReady;
            kinectSensorManager.TransformSmoothParameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };
            kinectSensorManager.SkeletonStreamEnabled = true;
            kinectSensorManager.KinectSensorEnabled = true;
        }

        private void UninitializeKinectServices(KinectSensor sensor)
        {
            sensor.SkeletonFrameReady -= this.SkeletonsReady;


            //enableAec.Visibility = Visibility.Collapsed;
        }

        #region Kinect Skeleton processing
        private void SkeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (StatInit == 0)
            {
                for (int i = 0; i < StatusArrayLen; i++)
                {
                    StatusArray[i] = 0;
                }
                StatInit = 1;
            }
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    int skeletonSlot = 0;

                    if ((this.skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);

                    foreach (Skeleton skeleton in this.skeletonData)
                    {
                        if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                        {
                            Player player;
                            if (this.players.ContainsKey(skeletonSlot))
                            {
                                player = this.players[skeletonSlot];
                            }
                            else
                            {
                                player = new Player(skeletonSlot);
                                player.SetBounds(this.playerBounds);
                                this.players.Add(skeletonSlot, player);
                            }

                            player.LastUpdated = DateTime.Now;

                            // Update player's bone and joint positions
                            if (skeleton.Joints.Count > 0)
                            {
                                player.IsAlive = true;

                                // Head, hands, feet (hit testing happens in order here)
                                player.UpdateJointPosition(skeleton.Joints, JointType.Head);
                                player.UpdateJointPosition(skeleton.Joints, JointType.HandLeft);
                                player.UpdateJointPosition(skeleton.Joints, JointType.HandRight);
                                player.UpdateJointPosition(skeleton.Joints, JointType.FootLeft);
                                player.UpdateJointPosition(skeleton.Joints, JointType.FootRight);

                                // Hands and arms
                                player.UpdateBonePosition(skeleton.Joints, JointType.HandRight, JointType.WristRight);
                                player.UpdateBonePosition(skeleton.Joints, JointType.WristRight, JointType.ElbowRight);
                                player.UpdateBonePosition(skeleton.Joints, JointType.ElbowRight, JointType.ShoulderRight);

                                player.UpdateBonePosition(skeleton.Joints, JointType.HandLeft, JointType.WristLeft);
                                player.UpdateBonePosition(skeleton.Joints, JointType.WristLeft, JointType.ElbowLeft);
                                player.UpdateBonePosition(skeleton.Joints, JointType.ElbowLeft, JointType.ShoulderLeft);

                                // Head and Shoulders
                                player.UpdateBonePosition(skeleton.Joints, JointType.ShoulderCenter, JointType.Head);
                                player.UpdateBonePosition(skeleton.Joints, JointType.ShoulderLeft, JointType.ShoulderCenter);
                                player.UpdateBonePosition(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);

                                // Legs
                                player.UpdateBonePosition(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                                player.UpdateBonePosition(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                                player.UpdateBonePosition(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                                player.UpdateBonePosition(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                                player.UpdateBonePosition(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                                player.UpdateBonePosition(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);

                                player.UpdateBonePosition(skeleton.Joints, JointType.HipLeft, JointType.HipCenter);
                                player.UpdateBonePosition(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                                // Spine
                                player.UpdateBonePosition(skeleton.Joints, JointType.HipCenter, JointType.ShoulderCenter);
                                //countPlayer = skeleton.Joints.Count;

                                //rrr
                                playfield.Children.Clear();
                                player.Draw(playfield.Children);
                                
                                skeletonStatus= GetSkeletonStatus(skeleton);
                                StatusArray[countAction%StatusArrayLen] = skeletonStatus;

                                if (skeletonStatus == WALK)
                                {
                                    for (int i = 0; i < StatusArrayLen; i++)
                                        if (StatusArray[i] == STAND)
                                            StatusArray[i] = WALK;
                                }
                                skeletonStatus = GetCommon(StatusArray);



                                switch (skeletonStatus)
                                {
                                    case STAND:
                                        this.StatusLabel.Content = "Standing";
                                        break;
                                    case SIT:
                                        this.StatusLabel.Content = "Sitting";
                                        break;
                                    case LIEDOWN:
                                        this.StatusLabel.Content = "Lying Down";
                                        break;
                                    case WALK:
                                        this.StatusLabel.Content = "Walking";
                                        break;
                                    default:
                                        this.StatusLabel.Content = "No Gesture";
                                        break;
                                }

                                if (skeletonStatus == SIT)
                                    ResetPosition();
                                

                                if ( countAction%20 == 0 && Done == 0 && skeletonStatus == WALK)
                                {
                                    double t = CaculateDeg(skeleton.Joints[JointType.ElbowRight].Position, skeleton.Joints[JointType.WristRight].Position);
                                    if (Math.Abs(t - 270) < 15)
                                    {
                                        ResponsetoDownKey();
                                    }
                                    else if (Math.Abs(t - 90) < 15)
                                    {
                                        ResponsetoUpKey();
                                    }
                                    else if (Math.Abs(t) < 15 || Math.Abs(t-360) < 15)
                                    {
                                        ResponsetoRightKey();
                                    }
                                    else if (Math.Abs(t - 180) < 15)
                                    {
                                        ResponsetoLeftKey();
                                    }

                                }
                                countAction++;
                                //rrr
                            }
                            
                        }

                        skeletonSlot++;
                    }
                }
            }
            playfield.Children.Clear();
            foreach (var player in this.players)
            {
                player.Value.Draw(playfield.Children);
            }
        }

        int GetCommon(int[] arr)
        {
            int ret= 0;
            int maxx= 0;
            int cnt= 0;
            for(int stat= 1; stat < 5; stat++){
                cnt = 0;
                for(int i= 0; i < StatusArrayLen; i++)
                    if(arr[i] == stat)
                        cnt++;
                if (cnt > maxx)
                {
                    maxx = cnt;
                    ret = stat;
                }
            }  
            return ret;
        }

        float GetMin(float u, float v)
        {
            if (u < v)
                return u;
            return v;
        }

        float GetMax(float u, float v)
        {
            if (u < v)
                return v;
            return u;
        }

        private int GetSkeletonStatus(Skeleton mySkeleton)
        {
            int invalidStatus = 0;
            double DisHipHead = CaculateDistance(mySkeleton.Joints[JointType.Head].Position, mySkeleton.Joints[JointType.HipCenter].Position);
            if (Math.Abs(mySkeleton.Joints[JointType.Head].Position.X-mySkeleton.Joints[JointType.HipCenter].Position.X) >= (float)DisHipHead * 0.6)
                return LIEDOWN;
            if (Math.Abs(mySkeleton.Joints[JointType.Head].Position.X - mySkeleton.Joints[JointType.HipCenter].Position.X) >= (float)DisHipHead * 0.3)
                return invalidStatus;
            double DisHipKnee = (CaculateDistance(mySkeleton.Joints[JointType.HipLeft].Position, mySkeleton.Joints[JointType.KneeLeft].Position)
                + CaculateDistance(mySkeleton.Joints[JointType.HipRight].Position, mySkeleton.Joints[JointType.KneeRight].Position)) / 2;
            float yLeftDisHipKnee = Math.Abs(mySkeleton.Joints[JointType.HipLeft].Position.Y-mySkeleton.Joints[JointType.KneeLeft].Position.Y);
            float yRightDisHipKnee = Math.Abs(mySkeleton.Joints[JointType.HipRight].Position.Y-mySkeleton.Joints[JointType.KneeRight].Position.Y);
            if(GetMax(yLeftDisHipKnee, yRightDisHipKnee) <= (float)DisHipKnee*0.8)
                return SIT;
            if(GetMin(yLeftDisHipKnee, yRightDisHipKnee) > (float)DisHipKnee*0.8)
                return STAND;
            if(Math.Abs(yLeftDisHipKnee-yRightDisHipKnee) >= (float)DisHipKnee*0.2)
                return WALK;
            return invalidStatus;
        }

        private double CaculateDistance(SkeletonPoint pos1, SkeletonPoint pos2)
        {
            return Math.Sqrt((pos2.X - pos1.X) * (pos2.X - pos1.X) + (pos2.Y - pos1.Y) * (pos2.Y - pos1.Y));
        }

        private double CaculateDeg(SkeletonPoint pos1, SkeletonPoint pos2)
        {
            double deg = 0;
            if (Math.Abs(pos2.Y - pos1.Y) < 0.001)
                return deg = 90;
            double len = CaculateDistance(pos1, pos2);
            deg = Math.Acos((pos2.X - pos1.X) / len);
            deg = deg * 180 / Math.Acos(-1.0);
            if (pos2.Y - pos1.Y < 0)
                deg = 360 - deg;
            return deg;
        }


        private void CheckPlayers()
        {
            foreach (var player in this.players)
            {
                if (!player.Value.IsAlive)
                {
                    // Player left scene since we aren't tracking it anymore, so remove from dictionary
                    this.players.Remove(player.Value.GetId());
                    break;
                }
            }

            // Count alive players
            int alive = this.players.Count(player => player.Value.IsAlive);

            if (alive != this.playersAlive)
            {
                if (alive == 2)
                {
                    //this.myFallingThings.SetGameMode(GameMode.TwoPlayer);
                }
                else if (alive == 1)
                {
                    //this.myFallingThings.SetGameMode(GameMode.Solo);
                }
                else if (alive == 0)
                {
                    //this.myFallingThings.SetGameMode(GameMode.Off);
                }

                if ((this.playersAlive == 0)/* && (this.mySpeechRecognizer != null)*/)
                {
                    /*BannerText.NewBanner(
                        Properties.Resources.Vocabulary,
                        this.screenRect,
                        true,
                        System.Windows.Media.Color.FromArgb(200, 255, 255, 255));*/
                }

                this.playersAlive = alive;
            }
        }

        private void PlayfieldSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePlayfieldSize();
        }

        private void UpdatePlayfieldSize()
        {
            // Size of player wrt size of playfield, putting ourselves low on the screen.
            this.screenRect.X = 0;
            this.screenRect.Y = 0;
            this.screenRect.Width = this.playfield.ActualWidth;
            this.screenRect.Height = this.playfield.ActualHeight;

            BannerText.UpdateBounds(this.screenRect);

            this.playerBounds.X = 0;
            this.playerBounds.Width = this.playfield.ActualWidth;
            this.playerBounds.Y = this.playfield.ActualHeight * 0.2;
            this.playerBounds.Height = this.playfield.ActualHeight * 0.75;

            foreach (var player in this.players)
            {
                player.Value.SetBounds(this.playerBounds);
            }

            Rect fallingBounds = this.playerBounds;
            fallingBounds.Y = 0;
            fallingBounds.Height = playfield.ActualHeight;

        }
        #endregion Kinect Skeleton processing


        #region GameTimer/Thread
        private void GameThread()
        {
            this.runningGameThread = true;
            //playfield.Children.Clear();
            foreach (var player in this.players)
            {
                player.Value.Draw(playfield.Children);
            }

 
        }
        #endregion GameTimer/Thread

        //private void textBox_result_TextChanged(object sender, TextChangedEventArgs e)
        //{
          
        //    textBox_result.Text= countPlayer.ToString();
        //    //textBox_result.Text = this.players.Count().ToString();
        //}

        public void ResetPosition()
        {
            //Reset current Position as Start Point
            currentPoint.X = startPoint.X;
            currentPoint.Y = startPoint.Y;
            //Reset current Position as Start Position in the Matrix
            curPos_X = startPos_X;
            curPos_Y = startPos_Y;
            //Create animation of moving to the next position
            createAnimation(currentPoint.X, currentPoint.Y);
            direction = movingRight;
        }




        #region Initialize Map Components

        Image block1;
        //    public void InitBlocks()
        //{
        //    InitBlock1();
        //    InitBlock2();
        //    InitBlock3();
        //    InitBlock4();
        //}

        //public void InitBlock1()
        //{
        //    String filename;
        //    filename = "Images/Q1.png";
        //    block1 = new Image();
        //    block1.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
        //    //imageMovement.Source = new BitmapImage((new Uri(@imageFileName, UriKind.Relative)));
        //    //The size of the block is 100*300
        //    block1.Width = tile;    //100
        //    block1.Height = 3 * tile;   //300
        //    Carrier.Children.Add(block1);
        //    //Set position as (100, 0)
        //    Canvas.SetLeft(block1, 100);
        //    Canvas.SetTop(block1, 0);
        //    Canvas.setV = false;
        //}

        //    public void InitBlock2()
        //{
        //    String filename;
        //    filename = "images/wall1.png";
        //    block2 = new Image();
        //    block2.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
        //    //The size of the block is 100*100
        //    block2.Width = tile;    //100
        //    block2.Height = tile;   //100
        //    Carrier.Children.Add(block2);
        //    //Set postion as (100, 400)
        //    Canvas.SetLeft(block2, 100);
        //    Canvas.SetTop(block2, 400);
        //}

        //    public void InitBlock3()
        //{
        //    String filename;
        //    filename = "images/wall1.png";
        //    block3 = new Image();
        //    block3.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
        //    //The size of the block is 100*200
        //    block3.Width = tile;    //100
        //    block3.Height = 2 * tile;   //200
        //    Carrier.Children.Add(block3);
        //    //Set postion as (300, 100)
        //    Canvas.SetLeft(block3, 300);
        //    Canvas.SetTop(block3, 100);
        //}

        //    public void InitBlock4()
        //{
        //    String filename;
        //    filename = "images/wall1.png";
        //    block4 = new Image();
        //    block4.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
        //    //The size of the block is 100*100
        //    block4.Width = tile;    //100
        //    block4.Height = tile;   //100
        //    Carrier.Children.Add(block4);
        //    //Set postion as (300, 400)
        //    Canvas.SetLeft(block4, 300);
        //    Canvas.SetTop(block4, 400);
        //}
        public void InitMonster1Image()
        {
            String filename;
            filename = "images/monster1.png";
            monster1 = new Image();
            monster1.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            //The size of the image is 50*80
            monster1.Width = 50;
            monster1.Height = 80;
            Carrier.Children.Add(monster1);
            Canvas.SetLeft(monster1, 125);
            Canvas.SetTop(monster1, 210);
        }
        public void InitMonster2Image()
        {
            String filename;
            filename = "images/monster2.png";
            monster2 = new Image();
            monster2.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            //The size of the image is 50*80
            monster2.Width = 50;
            monster2.Height = 80;
            Carrier.Children.Add(monster2);
            Canvas.SetLeft(monster2, 525);
            Canvas.SetTop(monster2, 210);
        }
        public void InitMonster3Image()
        {
            String filename;
            filename = "images/monster3.png";
            monster3 = new Image();
            monster3.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            //The size of the image is 50*80
            monster3.Width = 50;
            monster3.Height = 80;
            Carrier.Children.Add(monster3);
            Canvas.SetRight(monster3, 125);
            Canvas.SetTop(monster3, 310);
        }
        public void InitMonster4Image()
        {
            String filename;
            filename = "images/monster4.png";
            monster4 = new Image();
            monster4.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            monster4.Width = 100;
            monster4.Height = 160;
            Carrier.Children.Add(monster4);
            Canvas.SetRight(monster4, 0);
            Canvas.SetTop(monster4, 70);
        }
        public void InitMonster5Image()
        {
            String filename;
            filename = "images/monster5.png";
            monster5 = new Image();
            monster5.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            monster5.Width = 50;
            monster5.Height = 80;
            Carrier.Children.Add(monster5);
            Canvas.SetLeft(monster5, 425);
            Canvas.SetTop(monster5, 410);
            monster5 = new Image();
            monster5.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            monster5.Width = 50;
            monster5.Height = 80;
            Carrier.Children.Add(monster5);
            Canvas.SetLeft(monster5, 25);
            Canvas.SetTop(monster5, 10);
            monster5 = new Image();
            monster5.Source = new BitmapImage((new Uri(@filename, UriKind.Relative)));
            monster5.Width = 50;
            monster5.Height = 80;
            Carrier.Children.Add(monster5);
            Canvas.SetLeft(monster5, 625);
            Canvas.SetBottom(monster5, 10);
        }

        //private void monster()
        //{
        //    int size = 4;
        //    monster[] Monsters = new monster[size];

        //    //set values for paramters
        //    int monster_ID_1 = 1;
        //    int monster_PosX_1 = 1;
        //    int monster_PosY_1 = 2;
        //    float monster_PointX_1 = 125;
        //    float monster_PointY_1 = 225;
        //    int monster_ID_2 = 2;
        //    int monster_PosX_2 = 2;
        //    int monster_PosY_2 = 5;
        //    float monster_PointX_2 = 225;
        //    float monster_PointY_2 = 525;
        //    int monster_ID_3 = 3;
        //    int monster_PosX_3 = 3;
        //    int monster_PosY_3 = 10;
        //    float monster_PointX_3 = 325;
        //    float monster_PointY_3 = 1025;
        //    int monster_ID_4 = 4;
        //    int monster_PosX_4 = 1;
        //    int monster_PosY_4 = 11;
        //    float monster_PointX_4 = 125;
        //    float monster_PointY_4 = 1125;
        //    for (int i = 0; i < size; i++)
        //    {
        //        if (i == 0)
        //        {

        //            Monsters[i].Monster_Id = monster_ID_1;
        //        }
        //        else if (i == 1)
        //        {
        //            Monsters[i].Monster_Id = monster_ID_1;
        //        }
        //        else if (i == 2)
        //        {
        //            Monsters[i].Monster_Id = monster_ID_2;
        //        }
        //        else if (i == 3)
        //        {
        //            Monsters[i].Monster_Id = monster_ID_3;
        //        }
        //    }
        //}
            public void initImageMovement()
            {
                currentPoint.X = startPoint.X;
                currentPoint.Y = startPoint.Y;
                imageMovement = new Image();
                imageMovement.Width = 50;
                imageMovement.Height = 50;
                Carrier.Children.Add(imageMovement);
                Canvas.SetLeft(imageMovement, currentPoint.X);
                Canvas.SetTop(imageMovement, currentPoint.Y);

                //Create a new thread
                DispatcherTimer dispatchTimer = new DispatcherTimer();
                dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatchTimer.Interval = TimeSpan.FromMilliseconds(90);
                dispatchTimer.Start();
            }
       
        #endregion Initialize Map Components

            private void MainWin_Loaded(object sender, RoutedEventArgs e)
            {
                //设置定时器 
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(10000000); //时间间隔为一秒 
                timer.Tick += new EventHandler(timer_Tick);
                //转换成秒数 
                Int32 hour = Convert.ToInt32(HourArea.Text);
                Int32 minute = Convert.ToInt32(MinuteArea.Text);
                Int32 second = Convert.ToInt32(SecondArea.Text);
                //处理倒计时的类 
                processCount = new ProcessCount(hour * 3600 + minute * 60 + second);
                CountDown += new CountDownHandler(processCount.ProcessCountDown);
                //开启定时器 
                timer.Start();
            }
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
                {
                   // this.Close();
                   // MessageBox.Show("Sorry, time is up!>_<!");
                    Game_over a = new Game_over();
                   a.ShowDialog();
                    this.Close();
                }
            }
            public event CountDownHandler CountDown;
            public bool OnCountDown()
            {
                if (CountDown != null)
                    return CountDown();
                return false;
            } 


        #region Logic behind Map
            /*
            * Method that set values of the posMatrix[row,column], in this case, [1][0], [1][1], [1][2], [1][4], [3][1], [3][2], [3][4] are false, 
             * the others are all true
            */
            public void setValuesMatrix()
            {
                for (int row = 0; row < 6; row++)
                {
                    for (int column = 0; column < 12; column++)
                    {
                        if (column == 0 && row == 1)
                            posMatrix[column, row] = false;
                        else if (column == 1 && row == 1)
                            posMatrix[column, row] = false;
                        else if (column == 1 && row == 3)
                            posMatrix[column, row] = false;
                        else if (column == 1 && row == 5)
                            posMatrix[column, row] = false;
                        else if (column == 2 && row == 3)
                            posMatrix[column, row] = false;
                        else if (column == 3 && row == 1)
                            posMatrix[column, row] = false;
                        else if (column == 3 && row == 4)
                            posMatrix[column, row] = false;
                        else if (column == 4 && row == 2)
                            posMatrix[column, row] = false;
                        else if (column == 4 && row == 3)
                            posMatrix[column, row] = false;
                        else if (column == 5 && row == 0)
                            posMatrix[column, row] = false;
                        else if (column == 5 && row == 3)
                            posMatrix[column, row] = false;
                        else if (column == 5 && row == 4)
                            posMatrix[column, row] = false;
                        else if (column == 5 && row == 5)
                            posMatrix[column, row] = false;
                        else if (column == 6 && row == 0)
                            posMatrix[column, row] = false;
                        else if (column == 6 && row == 1)
                            posMatrix[column, row] = false;
                        else if (column == 6 && row == 4)
                            posMatrix[column, row] = false;
                        else if (column == 7 && row == 1)
                            posMatrix[column, row] = false;
                        else if (column == 7 && row == 3)
                            posMatrix[column, row] = false;
                        else if (column == 8 && row == 3)
                            posMatrix[column, row] = false;
                        else if (column == 8 && row == 5)
                            posMatrix[column, row] = false;
                        else if (column == 9 && row == 1)
                            posMatrix[column, row] = false;
                        else if (column == 9 && row == 5)
                            posMatrix[column, row] = false;
                        else if (column == 10 && row == 0)
                            posMatrix[column, row] = false;
                        else if (column == 10 && row == 2)
                            posMatrix[column, row] = false;
                        else if (column == 10 && row == 4)
                            posMatrix[column, row] = false;
                        else if (column == 10 && row == 5)
                            posMatrix[column, row] = false;
                        else
                            posMatrix[column, row] = true;
                    }
                }
            }

        #region Response to Input Control

            ///// <summary>
            ///// Response to the KeyDown Event
            ///// </summary>
            //private void KeyDown(object sender, KeyEventArgs e)
            //{
            //    if (curPos_X == endPos_X && curPos_Y == endPos_Y)
            //    {
            //        MessageBox.Show("Great, you've reached the destination");
            //        Done = 1;
            //    }
            //    //Response to KeyDown event: Key--Left, Right, Up, Down
            //    switch (e.Key)
            //    {
            //        case Key.Left:
            //            ResponsetoLeftKey();
            //            e.Handled = true;
            //            break;
            //        case Key.Right:
            //            ResponsetoRightKey();
            //            e.Handled = true;
            //            break;
            //        case Key.Up:
            //            ResponsetoUpKey();
            //            e.Handled = true;
            //            break;
            //        case Key.Down:
            //            ResponsetoDownKey();
            //            e.Handled = true;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            /// <summary>
            /// Response to Left Key
            /// </summary>
            public void ResponsetoLeftKey()
            {
                bool reachable = true;
                direction = movingLeft;
                reachable = verifyNextLeft(curPos_X, curPos_Y);
                if (reachable == true)
                    moveLeft();
                //MessageBox.Show("Current position X: " + curPos_X + ", Y: " + curPos_Y);
                if (Done == 0 && curPos_X == endPos_X && curPos_Y == endPos_Y)
                {
                    if (Done > 0)
                        return;
                    //MessageBox.Show("Great, you've reached the destination");
                    Done++;
                    //Searching.Content = "Great, just found the treasure!";
                }


            }

            /// <summary>
            /// Response to Right Key
            /// </summary>
            public void ResponsetoRightKey()
            {
                bool reachable = true;
                direction = movingRight;
                reachable = verifyNextRight(curPos_X, curPos_Y);
                if (reachable == true)
                    moveRight();
                //MessageBox.Show("Current position X: " + curPos_X + ", Y: " + curPos_Y);

                if (Done == 0 &&curPos_X == endPos_X && curPos_Y == endPos_Y)
                {
                    if (Done > 0)
                        return;
                    //MessageBox.Show("Great, you've reached the destination");
                    Done++;
                    //Searching.Content = "Great, just found the treasure!";
                }
            }


            /// <summary>
            /// Response to Up Key
            /// </summary>
            public void ResponsetoUpKey()
            {
                bool reachable = true;
                direction = movingUp;
                reachable = verifyNextUp(curPos_X, curPos_Y);
                if (reachable == true)
                    moveUp();
                //MessageBox.Show("Current position X: " + curPos_X + ", Y: " + curPos_Y);

                if (Done == 0 && curPos_X == endPos_X && curPos_Y == endPos_Y)
                {
                    if (Done > 0)
                        return;
                    //MessageBox.Show("Great, you've reached the destination");
                    Done++;
                    //Searching.Content = "Great, just found the treasure!";
                }
              
            }
       
            /// <summary>
            /// Response to Down Key
            /// </summary>
            public void ResponsetoDownKey()
            {
                bool reachable = true;
                direction = movingDown;
                reachable = verifyNextDown(curPos_X, curPos_Y);
                if (reachable == true)
                    moveDown();
                //MessageBox.Show("Current position X: " + curPos_X + ", Y: " + curPos_Y);
                if (Done == 0 && curPos_X == endPos_X && curPos_Y == endPos_Y)
                {
                    if (Done > 0)
                        return;
                    //MessageBox.Show("Great, you've reached the destination");
                    Done++;
                    //Searching.Content = "Great, just found the treasure!";
                }
            }

        #endregion  Reponse to Control

        #region Movement on Canvas
 
            /// <summary>
            /// Make the player move to the next Left tile on canvas
            /// </summary>
            public void moveLeft()
            {
                //Define the position after moved: aftermovePos_X, aftermovePos_Y
                double aftermovePos_X, aftermovePos_Y;
                aftermovePos_X = currentPoint.X - distance_X;
                aftermovePos_Y = currentPoint.Y;
                //Create animation of moving to the next position
                createAnimation(aftermovePos_X, aftermovePos_Y);
                //Set current postion currentPoint(x,y) as the after moved position aftermovePos_X, aftermovePos_Y
                currentPoint.X = aftermovePos_X;
                currentPoint.Y = aftermovePos_Y;
                //set current position currentPos[X, Y] as currentPos[X-1, Y]
                curPos_X = curPos_X - 1;
            }

            /// <summary>
            /// Make the player move to the next Right tile on canvas
            /// </summary>
            public void moveRight()
            {
                //Define the position after moved: aftermovePos_X, aftermovePos_Y
                double aftermovePos_X, aftermovePos_Y;
                aftermovePos_X = currentPoint.X + distance_X;
                aftermovePos_Y = currentPoint.Y;
                //Create animation of moving to the next position
                createAnimation(aftermovePos_X, aftermovePos_Y);
                //Set current postion as the after moved position
                currentPoint.X = aftermovePos_X;
                currentPoint.Y = aftermovePos_Y;
                //set current position currentPos[X, Y] as currentPos[X+1, Y]
                curPos_X = curPos_X + 1;
            }

            /// <summary>
            /// Make the player move to the next Up tile on canvas
            /// </summary>
            public void moveUp()
            {
                //Define the position after moved: aftermovePos_X, aftermovePos_Y
                double aftermovePos_X, aftermovePos_Y;
                aftermovePos_X = currentPoint.X;
                aftermovePos_Y = currentPoint.Y - distance_Y;
                //Create animation of moving to the next position
                createAnimation(aftermovePos_X, aftermovePos_Y);
                //Set current postion as the after moved position
                currentPoint.X = aftermovePos_X;
                currentPoint.Y = aftermovePos_Y;
                //set current position currentPos[X, Y] as currentPos[X, Y-1]
                curPos_Y = curPos_Y - 1;
            }

            /// <summary>
            /// Make the player move to the next Down tile on canvas
            /// </summary>
            public void moveDown()
            {
                //Define the position after moved: aftermovePos_X, aftermovePos_Y
                double aftermovePos_X, aftermovePos_Y;
                aftermovePos_X = currentPoint.X;
                aftermovePos_Y = currentPoint.Y + distance_Y;
                //Create animation of moving to the next position
                createAnimation(aftermovePos_X, aftermovePos_Y);
                //Set current postion as the after moved position
                currentPoint.X = aftermovePos_X;
                currentPoint.Y = aftermovePos_Y;
                //set current position currentPos[X, Y] as currentPos[X, Y+1]
                curPos_Y = curPos_Y + 1;
            }

        #endregion Movement on Canvas

        #region Animation Creation
            /// <summary>
            /// Animation responding to different direction while moving on the same place
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void dispatcherTimer_Tick(object sender, EventArgs e)
            {
                switch (direction)
                {
                    case movingLeft:
                        String imageFileName;
                        imageFileName = "images/movingleft" + count + ".png";
                        imageMovement.Source = new BitmapImage((new Uri(@imageFileName, UriKind.Relative)));
                        count = count == 2 ? 0 : count + 1;
                        break;
                    case movingRight:
                        imageFileName = "images/movingRight" + count + ".png";
                        imageMovement.Source = new BitmapImage((new Uri(@imageFileName, UriKind.Relative)));
                        count = count == 2 ? 0 : count + 1;
                        break;
                    case movingUp:
                        imageFileName = "images/movingUp" + count + ".png";
                        imageMovement.Source = new BitmapImage((new Uri(@imageFileName, UriKind.Relative)));
                        count = count == 2 ? 0 : count + 1;
                        break;
                    case movingDown:
                        imageFileName = "images/movingDown" + count + ".png";
                        imageMovement.Source = new BitmapImage((new Uri(@imageFileName, UriKind.Relative)));
                        count = count == 2 ? 0 : count + 1;
                        break;
                    default:
                        break;
                }
            }    
        
            /// <summary>
            /// Method to create anmiation while moving to next position
            /// </summary>
            public void createAnimation(double x, double y)
            {
                //Define the position after moved: aftermovePos_X, aftermovePos_Y
                double aftermovePos_X = x;
                double aftermovePos_Y = y;
                //Create storyboard to display the animation
                Storyboard storyboard = new Storyboard();

                //Create animation of X-axie
                DoubleAnimation doubleAnimation = new DoubleAnimation(currentPoint.X, aftermovePos_X, new Duration(TimeSpan.FromMilliseconds(500)));
                Storyboard.SetTarget(doubleAnimation, imageMovement);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(doubleAnimation);

                //Create animation of Y-axie
                doubleAnimation = new DoubleAnimation(currentPoint.Y, aftermovePos_Y, new Duration(TimeSpan.FromMilliseconds(500)));
                Storyboard.SetTarget(doubleAnimation, imageMovement);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(doubleAnimation);

                //Dynamically add animation into the resource
                if (!Resources.Contains("imageMovementAnimation"))
                {
                    Resources.Add("imageMovementAnimation", storyboard);
                }

                //Play animation
                storyboard.Begin();
            }
        
        #endregion Animation Creation

        #region Next Position Verification
            /// <sumamry>
            /// Verify if the next left position is reachable
            /// </summary>
            public bool verifyNextLeft(int x, int y)
            {
                //set default value of result is false
                bool result = false;
                //define next position: nextPos_X, nextPos_Y
                int nextPos_X, nextPos_Y;

                try
                {
                    nextPos_X = x - 1;
                    nextPos_Y = y;
                    result = posMatrix[nextPos_X, nextPos_Y];
                    if (nextPos_X == 0 && nextPos_Y == 0) //Position of Advisor 2
                    {

                        if (!showQuesDia)
                        {
                            showQuesDia = true; //Flag to stop the scan of the Kinect
                            //Record the current second
                            Lastsecond = int.Parse(processCount.GetSecond());
                        }
                        //Get current second
                        if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                            int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                            Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                            Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                        {
                            this.KinectSensorManager.KinectSensor.Stop();

                            Advisor2 a = new Advisor2();
                            a.ShowDialog();

                            if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                            //Verify the Status of the Kinect
                            {
                                this.KinectSensorManager.KinectSensor.Start();
                                //Start a new thread
                            }
                        }


                    }
                    else if (nextPos_X == 6 && nextPos_Y == 5) //Position of Advisor 3
                    {
                        if (!showQuesDia)
                        {
                            showQuesDia = true; //Flag to stop the scan of the Kinect
                            //Record the current second
                            Lastsecond = int.Parse(processCount.GetSecond());
                        }
                        //Get current second
                        if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                            int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                            Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                            Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                        {
                            this.KinectSensorManager.KinectSensor.Stop();

                            Advisor3 a = new Advisor3();
                            a.ShowDialog();

                            if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                            //Verify the Status of the Kinect
                            {
                                this.KinectSensorManager.KinectSensor.Start();
                                //Start a new thread
                            }
                        }

                    }
                    //if (result == false)
                        //MessageBox.Show("That position is unavailable");
                }
                catch
                {
                    //MessageBox.Show("That position is unavailable");
                }

                return result;
            }

        /// <sumamry>
        /// Verify if the next right position is reachable
        /// </summary>
            public bool verifyNextRight(int x, int y)
            {
                //set default value of result is false
                bool result = false;
                
                //define next position: nextPos_X, nextPos_Y
                int nextPos_X, nextPos_Y;
                
                //Catch invalid position exception
                try
                {
                    nextPos_X = x + 1;
                    nextPos_Y = y;
                    result = posMatrix[nextPos_X, nextPos_Y];
                    //if (result == false)
                        //MessageBox.Show("That position is unavailable");
                    if (nextPos_X == 1 && nextPos_Y == 2)
                    {
                        //    @try a = new @try();
                        /*******************/
                        //Monster 1
                            if(!showQuesDia)
                            {
                                showQuesDia = true; //Flag to stop the scan of the Kinect
                                //Record the current second
                                Lastsecond =  int.Parse(processCount.GetSecond());
                            }
                           //Get current second
                            if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                                int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                                Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                                Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                            {                                
                                this.KinectSensorManager.KinectSensor.Stop();

                                Process aprocess = new Process();
                                aprocess.StartInfo.FileName = "E:/Xtian/Carleton School/TIM Program/Winter14/SYSC 5409/Project/Game Code/Full Project/Maze Game - Copy/Libraries/GettingStarted/bin/Debug/GettingStarted.exe";
                                //this.KinectSensorManager.KinectSensor = null;
                            
                                aprocess.Start();
                                //Start new process for the question section
                                while (!aprocess.WaitForExit(1000));
                                    aprocess.Close();
                                    if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                                        //Verify the Status of the Kinect
                                    {
                                            this.KinectSensorManager.KinectSensor.Start();
                                        //Start a new thread
                                    }
                            }
                    }
                    else if (nextPos_X == 10 && nextPos_Y == 3)
                    {
                        //    @try a = new @try();
                        /*******************/
                        //Monster 3
                        if (!showQuesDia)
                        {
                            showQuesDia = true; //Flag to stop the scan of the Kinect
                            //Record the current second
                            Lastsecond = int.Parse(processCount.GetSecond());
                        }
                        //Get current second
                        if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                            int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                            Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                            Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                        {
                            this.KinectSensorManager.KinectSensor.Stop();

                            Process aprocess = new Process();
                            aprocess.StartInfo.FileName = "E:/Xtian/Carleton School/TIM Program/Winter14/SYSC 5409/Project/Game Code/Full Project/Maze Game - Copy/Temp/GettingStarted/bin/Debug/GettingStarted.exe";
                            //this.KinectSensorManager.KinectSensor = null;

                            aprocess.Start();
                            //Start new process for the question section
                            while (!aprocess.WaitForExit(1000)) ;
                            aprocess.Close();
                            if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                            //Verify the Status of the Kinect
                            {
                                this.KinectSensorManager.KinectSensor.Start();
                                //Start a new thread
                            }
                        }


                    }
                    /*******************/
                    else
                    {
                        showQuesDia = false;
                    }
                    /*******************/
                }
                catch
                {
                   //MessageBox.Show("That position is unavailable");
                }

                return result;
            }

        /// <sumamry>
        /// Verify if the next up position is reachable
        /// </summary>
            public bool verifyNextUp(int x, int y)
            {
                //set default value of result is false
                bool result = false;
                //define next position: nextPos_X, nextPos_Y
                int nextPos_X, nextPos_Y;

                try
                {
                    nextPos_X = x;
                    nextPos_Y = y - 1;
                    result = posMatrix[nextPos_X, nextPos_Y];
                   // if (result == false)
                        //MessageBox.Show("That position is unavailable");
                    if (nextPos_X == 11 && nextPos_Y == 0)
                    {
                        //Win status
                            Win a = new Win();
                            this.Close();
                            a.ShowDialog();
                            
                    }

                    if (nextPos_X == 11 && nextPos_Y == 1)
                    {
                        //    @try a = new @try();
                        /*******************/
                        //Dragon
                        if (!showQuesDia)
                        {
                            showQuesDia = true; //Flag to stop the scan of the Kinect
                            //Record the current second
                            Lastsecond = int.Parse(processCount.GetSecond());
                        }
                        //Get current second
                        if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                            int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                            Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                            Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                        {
                            this.KinectSensorManager.KinectSensor.Stop();

                            Process aprocess = new Process();
                            aprocess.StartInfo.FileName = "E:/Xtian/Carleton School/TIM Program/Winter14/SYSC 5409/Project/Game Code/Full Project/Maze Game - Copy/TempPE/GettingStarted/bin/Debug/GettingStarted.exe";
                            //this.KinectSensorManager.KinectSensor = null;

                            aprocess.Start();
                            //Start new process for the question section
                            while (!aprocess.WaitForExit(1000)) ;
                            aprocess.Close();
                            if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                            //Verify the Status of the Kinect
                            {
                                this.KinectSensorManager.KinectSensor.Start();
                                //Start a new thread
                            }
                        }

                    }
                    else if (nextPos_X == 4 && nextPos_Y == 4)//Advisor 1
                    {

                        if (!showQuesDia)
                        {
                            showQuesDia = true; //Flag to stop the scan of the Kinect
                            //Record the current second
                            Lastsecond = int.Parse(processCount.GetSecond());
                        }
                        //Get current second
                        if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                            int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                            Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                            Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                        {
                            this.KinectSensorManager.KinectSensor.Stop();

                            Advisor1 a = new Advisor1();
                            a.ShowDialog(); 
                            
                            if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                            //Verify the Status of the Kinect
                            {
                                this.KinectSensorManager.KinectSensor.Start();
                                //Start a new thread
                            }
                        }

                    }
                }
                catch
                {
                    //MessageBox.Show("That position is unavailable");
                }

                return result;
            }

        /// <sumamry>
        /// Verify if the next down position is reachable
        /// </summary>
            public bool verifyNextDown(int x, int y)
            {
                //set default value of result is false
                bool result = false;
                //define next position: nextPos_X, nextPos_Y
                int nextPos_X, nextPos_Y;

                try
                {
                    nextPos_X = x;
                    nextPos_Y = y + 1;
                    result = posMatrix[nextPos_X, nextPos_Y];
                    //if (result == false)
                      //  MessageBox.Show("That position is unavailable");
                    if (nextPos_X == 5 && nextPos_Y == 2)
                    {

                        
                            //Win status
                            Game_over a = new Game_over();
                            this.Close();
                            a.ShowDialog();

                     

                        //Monster 2
                        //if (!showQuesDia)
                        //{
                        //    showQuesDia = true; //Flag to stop the scan of the Kinect
                        //    //Record the current second
                        //    Lastsecond = int.Parse(processCount.GetSecond());
                        //}
                        ////Get current second
                        //if (int.Parse(processCount.GetSecond()) - Lastsecond <= 5 &&
                        //    int.Parse(processCount.GetSecond()) - Lastsecond >= 0 ||
                        //    Lastsecond - int.Parse(processCount.GetSecond()) <= 5 &&
                        //    Lastsecond - int.Parse(processCount.GetSecond()) >= 0)
                        //{
                        //    this.KinectSensorManager.KinectSensor.Stop();

                        //    Process aprocess = new Process();
                        //    aprocess.StartInfo.FileName = "E:/Xtian/Carleton School/TIM Program/Winter14/SYSC 5409/Project/Game Code/Full Project/Maze Game - Copy/Obj/GettingStarted/bin/Debug/GettingStarted.exe";
                        //    //this.KinectSensorManager.KinectSensor = null;

                        //    aprocess.Start();
                        //    //Start new process for the question section
                        //    while (!aprocess.WaitForExit(1000)) ;
                        //    aprocess.Close();
                        //    if (this.KinectSensorManager.KinectSensor.IsRunning == false)
                        //    //Verify the Status of the Kinect
                        //    {
                        //        this.KinectSensorManager.KinectSensor.Start();
                        //        //Start a new thread
                        //    }
                        //}

                    }
                }
                catch
                {
                    //MessageBox.Show("That position is unavailable");
                }

                return result;
            }

        #endregion Next Position Verification

            private void WindowLoaded(object sender, RoutedEventArgs e)
            {

            }


        #endregion Logic behind Map

            private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
            {

            }
    }
    public delegate bool CountDownHandler(); 

}
