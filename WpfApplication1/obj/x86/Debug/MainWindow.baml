   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   	   S OMicrosoft.Kinect.Toolkit, Version=1.8.0.0, Culture=neutral, PublicKeyToken=nulleHclr-namespace:Microsoft.Kinect.Toolkit;assembly=Microsoft.Kinect.ToolkitMicrosoft.Kinect.Toolkit 5   	    WpfApplication1    WpfApplication1.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   kkt^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   \toolkitHclr-namespace:Microsoft.Kinect.Toolkit;assembly=Microsoft.Kinect.Toolkit     5   	   -   5          � Title$8  2The Math Tower - Interactive Media and Digital Art��5   	   $��Italic$�6D   $��701.209��6�   $
��1450��6�   .��5      � $��0,0,17,1q�6    � RowDefinitions 5   
   �� 5	      $
��319* �6   �� 5
      $
��357* �6    � ColumnDefinitions 5   
   �� 5      $��1201* �6   �� 5      $	��10* �6   �� 5      $��66 �6   �� 5      $	��28* �6   �� 5      $��0* �6   �� 5      $	��11* �6   �� 5      $��30 �6   �� 5      $��9* �6   �� 5      $	��28* �6   �� 5      $��9* �6   .+�5   
   ��  �� Orientation$ Vertical=�6   ���	#FF46BC44$��0,0,-24,-189q�6C   $��2��6Y   $��10��6j   .+�5      � $��79��6   $��0,0,10,0q�6    5      �� 5      $
��463* �6'   �� 5      $
��156* �6'   .+�5      �� -   5         ��Name$ 	playfield��5      $��525,245,0,-503q�6O   $��Right=�6g   $	��225��6�   $��2��6�   ��5      �� 5        �� EndPoint$ 0.5,1&�6.    �� 
StartPoint$ 0.5,0&�6?   .��5!      �  � Color$ 	#D9040833��6+    � Offset$ 0.238Q�6=   � 5"      $ 	#FF46BC44��6+   $ 1Q�6=   � 5#      $ 	#E61747B2��6+   $ 0.811Q�6=   5$      5&      ;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  5'      $��0,100,0,-161q�6'   $��Right=�6=   $	��225��6Y   $	��140��6e   $	��Top=�6r   $��1��6�   7   1Microsoft.Samples.Kinect.WpfViewers.KinectControl	   KinectSensorManager	 6�   �� 6�   4  .Microsoft.Kinect.Toolkit.KinectSensorChooserUI  5(      -   5        
  Name$
 SensorChooserUI��5(   0      IsListening . $��Center=�6]   $	��Top=�6z   $��290,0,133.164,0q�6�   5)      �� 5*      -   5        $ Carrier��5*      $	��600��6&   $��10,10,-212,10q�63   $
��Left=�6J   $
��1200��6e   ��5+      �� 5,       �� ImageSource$ images/mapBackground.png��6!   .+�5.      �� -   5        $��StatusLabel��5.   $   $��66��6   $��
No Gesture��67   $	��229��6L    �� ContentStringFormat$  ��6X   $��45(�6o    �� Left$	 449��6}   
 �� Top$	 615��6�   $��Impact)�6�   $��Normal$�6�   ���	#FF002EFF6�   �� 5/      -   5        $
��life��5/   $   $��66��6   $��Current lives: 1��60   $	��321��6K   $  ��6W   $��45(�6n   $ 50��6|   $	 -80��6�   $��Impact)�6�   $��Normal$�6�   
���  ��6�   50      �� 52      -   5         ��Name$ 	enableAec��52      
�������$��20(�6=   $��	10,0,0,10q�6K   $
��Left=�6^   $��Bottom=�6y   $y�	Collapsed=�6�   {�.  .��6�   .,Enable Echo Cancellation on Default Speakers54      55      �� 57   
   -   5        $��HourArea��57      $��01��6   $	��Top=�6.   $��25(�6F   ���	#FF46BC44$��3��6k   $��0,20,0,0q�6{   $��30��6�   �� 58   
   -	   5        $��HourSplitMinute��58      $��:��6   $	��Top=�64   $��25(�6L   ���	#FF46BC44$��5��6q   $��0,20,0,0q�6�   $��30��6�   �� 59   
   -
   5        $��
MinuteArea��59      $��00��6   $	��Top=�60   $��25(�6H   ���	#FF46BC44$��6��6m   $��0,20,0,0q�6}   $��30��6�   �� 5:   
   -   5        $��MinuteSplitSecond��5:      $��:��6   $	��Top=�66   $��25(�6N   ���	#FF46BC44$��7��6s   $��0,20,0,0q�6�   $��30��6�   �� 5;   
   -   5        $��
SecondArea��5;      $��00��6   $	��Top=�60   $��25(�6H   ���	#FF46BC44$��8��6m   $��0,20,0,0q�6}   $��30��6�   5<      5=      