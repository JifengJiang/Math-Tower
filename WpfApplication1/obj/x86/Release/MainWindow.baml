   M S B A M L   `   `   `  ���� ^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  S OMicrosoft.Kinect.Toolkit, Version=1.8.0.0, Culture=neutral, PublicKeyToken=nulleHclr-namespace:Microsoft.Kinect.Toolkit;assembly=Microsoft.Kinect.ToolkitMicrosoft.Kinect.Toolkit  WpfApplication1    WpfApplication1.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    kkt^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     \toolkitHclr-namespace:Microsoft.Kinect.Toolkit;assembly=Microsoft.Kinect.Toolkit     -     � Title$8  2The Math Tower - Interactive Media and Digital Art��$��Italic$�$��701.209��$
��1450��.��� $��0,0,17,1q� � RowDefinitions �� $
��319* ��� $
��357* � � ColumnDefinitions �� $��1201* ��� $	��10* ��� $��66 ��� $	��28* ��� $��0* ��� $	��11* ��� $��30 ��� $��9* ��� $	��28* ��� $��9* �.+���  �� Orientation$ Vertical=����	#FF46BC44$��0,0,-24,-189q�$��2��$��10��.+�� $��79��$��0,0,10,0q� �� $
��463* ��� $
��156* �.+��� -    ��Name$ 	playfield��$��525,245,0,-503q�$��Right=�$	��225��$��2������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#D9040833�� � Offset$ 0.238Q�� $ 	#FF46BC44��$ 1Q�� $ 	#E61747B2��$ 0.811Q�;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  $��0,100,0,-161q�$��Right=�$	��225��$	��140��$	��Top=�$��1��7   1Microsoft.Samples.Kinect.WpfViewers.KinectControl	   KinectSensorManager	 �� 4  .Microsoft.Kinect.Toolkit.KinectSensorChooserUI  -   
  Name$
 SensorChooserUI��   IsListening . $��Center=�$	��Top=�$��290,0,133.164,0q��� -   $ Carrier��$	��600��$��10,10,-212,10q�$
��Left=�$
��1200������  �� ImageSource$ images/mapBackground.png��.+��� -   $��StatusLabel��$��66��$��
No Gesture��$	��229�� �� ContentStringFormat$  ��$��45(� �� Left$	 449��
 �� Top$	 615��$��Impact)�$��Normal$����	#FF002EFF�� -   $
��life��$��66��$��Current lives: 1��$	��321��$  ��$��45(�$ 50��$	 -80��$��Impact)�$��Normal$�
���  ���� -    ��Name$ 	enableAec��
�������$��20(�$��	10,0,0,10q�$
��Left=�$��Bottom=�$y�	Collapsed=�{�.  .��.,Enable Echo Cancellation on Default Speakers�� -   $��HourArea��$��01��$	��Top=�$��25(����	#FF46BC44$��3��$��0,20,0,0q�$��30���� -	   $��HourSplitMinute��$��:��$	��Top=�$��25(����	#FF46BC44$��5��$��0,20,0,0q�$��30���� -
   $��
MinuteArea��$��00��$	��Top=�$��25(����	#FF46BC44$��6��$��0,20,0,0q�$��30���� -   $��MinuteSplitSecond��$��:��$	��Top=�$��25(����	#FF46BC44$��7��$��0,20,0,0q�$��30���� -   $��
SecondArea��$��00��$	��Top=�$��25(����	#FF46BC44$��8��$��0,20,0,0q�$��30��