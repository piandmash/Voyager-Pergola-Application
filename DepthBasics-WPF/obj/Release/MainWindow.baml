   M S B A M L   `   `   `  ����   DepthBasics-WPF5    /Microsoft.Samples.Kinect.DepthBasics.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     J FWpfAnimatedGif, Version=1.4.14.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  -   "  WpfAnimatedGif.ImageBehavior    AnimateInDesignMode  .  � Title$ Voyager Kinect��$
��1080��$
��1920�� � WindowState$ Normal=�
���   � � Topmost .  .��� -   $��
layoutGrid��$��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�(� $��1�� (� Stretch$ Uniform=�$��Center=�.��r� -   $	��Vid��$
��1920��$
��1080�� r� Source$T ND:\Collusion\KinectInfraRed\InfraredBasics-WPF\Images\MasonryMarkAnimation.mp4?� r� LoadedBehavior$ Manual=�(� -   $��KinectImageBox��$��1��$
 None=� J� RenderTransformOrigin$ 0.5,0.5&�$
��1920��$
��1080��$��0q�$��3��.���� -   $��Image��$	��640��$	��480��	 J� Opacity$	 0Q�$��-1300,0,0,-2400q�(� -   $��
HexagonBox��$��1��$
 None=�$ 0.5,0.5&�$
��1920��$
��1080��$��0q�$��3��.��F� -   
 F�Name$
 Hexagon�����#00daf4$��0�����#00daf4$
��1100��$	��960��(� $��1��$ Uniform=�$��Center=�.��r� -   $��ScanCompleteVideo��$
��1920��$
��1080��$y�Hidden=�$9 3D:\Collusion\Hexagon_grid_AME\Pergola Scan Blue.mp4?�$ Manual=�(� -	   $��Loading��$��1��$
 None=�$	 0,0&�$
��1920��$
��1080��$��0q�$��3��$
��Left=�$��Bottom=�.��r� -
   $��ScanningVid��$
��1920��$
��1080��$/ )D:\Collusion\Renders\Pergola Scanning.mp4?�$��-1680,0,0,280q�$ Manual=�(� -   $��Scanning��$��1��$y�Visible=�$ Uniform=�$��Bottom=�$	 0,0&�$
��Left=�$
��1080��z�V� .~���  �� Angle$	 270Q�T� .���  � Orientation$ 
Horizontal=�$��Center=�$��0q�.+���  �� Orientation$ 
Horizontal=�$
��Left=�$��Bottom=�$��0,7,0,5q�.+��� -   $��lblScanning�����#a6f3ff$	��130��$��Px437 IBM ISO9)�$��4(�$��Center=�.��
Scanning(� $��1��$ Uniform=�$��Center=�$��Bottom=�$��0,0,0,10q�$ 0.5,0.5&�z�V� .~��� �� �� $	 180Q�T� .��� -   $��ScanningProgress�� � IsIndeterminate . $��1��$	��100�����	#00E6E6E6 � Maximum$ 50Q���Z� ����  �� EndPoint$ 1,0.5&� �� 
StartPoint$ 0,0.5&�.���  � Color$ 	#0048F1F7�� � Offset$ 1Q�� $ 	#D800F4FD��$	 0.5Q�� $ 	#0048F1F7��$ 0Q�(� $��1��$ Uniform=�$��Center=�$	��Top=�$ 0.5,0.5&�z�V� .~��� �� �� $	 180Q�T� .��� -   $��UploadProgress�� . $��1��$	��100�����	#00E6E6E6��Z� ���� $ 1,0.5&�$ 0,0.5&�.��� $ 	#0048F1F7��$ 1Q�� $ 	#D800F4FD��$	 0.5Q�� $ 	#0048F1F7��$ 0Q�(� -   $��ConnectView��$��1��$y�Visible=�$ Uniform=�$	��Top=�$ 0.5,0.5&�z�V� .~���  �� ScaleX$	 0.8Q� �� ScaleY$	 0.8Q��� �� $ 0Q�T� .��� $ Vertical=�.+��� $ 
Horizontal=�.+��� 
�������$	��130��.��
IP Address�� -   $��txtIpAddress��$��16��.��192.168.1.1�� $ 
Horizontal=�.+��� 
�������$	��130��.��Port�� -   $��txtPort��$��16��$��Stretch=�.��5000�� -   $��Connect��$��20��$��70��.��	Connect�� $ 
Horizontal=�.+��� 
�������$	��130��.��
Hex Size %�� -   $��txtHexWidth��$��16��$��Stretch=�.��1100�� -   $��txtHexHeight��$��16��$��Stretch=�.��960�� -   $��
txtHexSize��$��16��$��Stretch=�.��88.88�� -   $��
HexSizeCal��$��20��$��70��.��Calc�� $ 
Horizontal=�.+��� 
�������$	��130���� $ 
Horizontal=�.+��� 
�������$	��130��.��Scanning Cancel�� -   $��chkScanningCancel�� d� 	IsChecked$
 trueY�$��Center=��� $ 
Horizontal=�.+��� 
�������$	��130��.��Scanning Mulitplier�� -   $��txtScanningMultiplier��$��16��$��Stretch=�.��2�� $ 
Horizontal=�.+��� 
�������$	��130��.��Scanning Bars�� -   $��chkScanningBars��$
 trueY�$��Center=��� $ 
Horizontal=�.+��� 
�������$	��130��.��Tower Trigger Wait (seconds)�� -   $��txtTowerTriggerWait��$��16��$��Stretch=�.��60�� $ 
Horizontal=�.+��� 
�������$	��130��.��Loading Image Path�� -   $��txtLoadingImagePath��$��16��$��Stretch=�$	��150��.��home�� -   $��btnLoadingImageBrowse��$��16��$��20��.��...�� $ 
Horizontal=�.+��� 
�������$	��130��.��Video Folder�� -   $��txtVideoFolderPath��$��16��$��Stretch=�$	��150��.��home�� -   $��btnVideoFolderBrowse��$��16��$��20��.��...�� $ 
Horizontal=�.+��� 
�������$	��130��.��
Video Path�� -   $��txtVideoPath��$��16��$��Stretch=�$	��150��.��home�� -    $��btnVideoBrowse��$��16��$��20��.��...�� $ 
Horizontal=�.+��� 
�������$	��130��.��Kinect Rotation�� -!   $��KinectRotation��$	��300�� � Minimum$ 0Q�$	 360Q� � Value$	 270Q� �� IsSnapToTickEnabled .  �� TickPlacement$ BottomRight=� �� TickFrequency$ 45Q��� -"   $��lblKinectRotation��
�������$��50��.��270�� $ 
Horizontal=�.+��� 
�������$	��130��.��Kinect Scale�� -#   $��KinectScale��$	��300��$	 0.5Q�$ 4Q�$ 4Q� . $ BottomRight=�$	 0.1Q��� -$   $��lblKinectScale��
�������$��50��.��4�� $ 
Horizontal=�.+��� 
�������$	��130��.��Kinect Left�� -%   $��
KinectLeft��$	��300��$ -2400Q�$
 2400Q�$ -1300Q� . $ BottomRight=�$	 100Q��� -&   $��lblKinectLeft��
�������$��50��.��-1300�� $ 
Horizontal=�.+��� 
�������$	��130��.��Kinect Bottom�� -'   $��KinectBottom��$	��300��$ -2400Q�$
 2400Q�$ -2400Q� . $ BottomRight=�$	 100Q��� -(   $��lblKinectBottom��
�������$��50��.��-2400�� $ 
Horizontal=�.+��� 
�������$	��130��.��Kinect Depth Cull�� -)   $��KinectDepthCull��$	��300��$ 0Q�$
 2000Q�$
 1000Q� . $ BottomRight=�$	 100Q��� -*   $��lblKinectDepthCull��
�������$��50��.��1000�� $ 
Horizontal=�.+��� 
�������$	��130��.��Kinect Opacity�� -+   $��KinectOpacity��$	��300��$ 0Q�$ 1Q�$ 0Q� . $ BottomRight=�$
 0.05Q��� -,   $��lblKinectOpacity��
�������$��50��.��0�� $ 
Horizontal=�.+��� 
�������$	��130��.��Hexagon Opacity�� --   $��HexagonOpacity��$	��300��$ 0Q�$ 1Q�$	 0.3Q� . $ BottomRight=�$
 0.05Q��� -.   $��lblHexagonOpacity��
�������$��50��.��0.3�� $ 
Horizontal=�.+��� 
�������$	��130��.��Sound Volume�� -/   $��SoundVolume��$	��300��$ 0Q�$ 1Q�$	 0.8Q� . $ BottomRight=�$
 0.05Q��� -0   $��lblSoundVolume��
�������$��50��.��0.8�� $ 
Horizontal=�.+��� 
�������$	��130��.��Udp Message�� -1   $��txtUdpMessage��$��16��$��Stretch=�$��76��.��
dnamatch�� -2   $��UdpSend��$��20��$��70��.��
Udp Send�� $ 
Horizontal=�.+��� 
�������$	��130��.��Udp Random Messages�� -3   $��txtUdpMessageRandom��$��16��$��Stretch=�.��dnamatch|dnamatch2|dnamatch3�� $ 
Horizontal=�.+��� 
�������$	��130���� -4   $
��Save��$��20��$��70��.��Save�� $ 
Horizontal=�.+��� -5   $��statusBarText��
�������$	��400��$��
Status Bar��$y�Visible=�$��-1 0 0 0q��� $ 
Horizontal=�.+��� -6   $��statusBarText0��
�������$	��400��$�� ��$y�Visible=�$��-1 0 0 0q��� $ 
Horizontal=�.+��� -7   $��statusBarHelpText��
�������$	��400��$y�Visible=�$��F1: Toggle Config��$��-1 0 0 0q��� $ 
Horizontal=�.+��� -8   $��statusBarHelpText1��
�������$	��400��$y�Visible=�$2��,F12: Toggle Full Screen, Space:Restart Video��$��-1 0 0 0q��� $ 
Horizontal=�.+��� -9   $��statusBarHelpText4��
�������$	��400��$y�Visible=�$��Escape: Quit��$��-1 0 0 0q�