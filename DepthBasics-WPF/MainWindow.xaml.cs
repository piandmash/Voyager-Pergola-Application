//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.DepthBasics
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    using VVVV_OSC;
    using System.Net.Sockets;
    using System.Net;
    using System.Text;

    using WpfAnimatedGif;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] colorPixels;

        private MediaPlayer soundPlayer = new MediaPlayer();

        //OSC
        private OSCTransmitter transmitter;
        private UdpClient udpClient;
        private IPAddress address = IPAddress.Parse("127.0.0.1");
        private int port = 8400;
        private DateTime lastTowerTrigger = DateTime.Now.AddDays(-1);

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //do calculation for the hexagon in the middle

            LoadSettings();
        }

        private void LoadSettings()
        {
            txtIpAddress.Text = Properties.Settings.Default.IpAddress;
            txtPort.Text = Properties.Settings.Default.Port;
            chkScanningCancel.IsChecked = Properties.Settings.Default.ScanningCancel;
            txtScanningMultiplier.Text = Properties.Settings.Default.ScanningMultiplier;
            chkScanningBars.IsChecked = Properties.Settings.Default.ScanningBars;
            txtTowerTriggerWait.Text = Properties.Settings.Default.TowerTriggerWait;
            txtLoadingImagePath.Text = Properties.Settings.Default.LoadingImagePath;
            try
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(txtLoadingImagePath.Text);
                img.EndInit();
                //ImageBehavior.SetAnimatedSource(LoadingImage, img);
            }
            catch { }
            //txtVideoPath.Text = Properties.Settings.Default.VideoPath;
            //try
            //{
            //    Vid.Stop();
            //    Vid.Source = new Uri(txtVideoPath.Text);
            //    RestartVideo(Vid);
            //}
            //catch { }
            KinectRotation.Value = Properties.Settings.Default.KinectRotation;
            KinectScale.Value = Properties.Settings.Default.KinectScale;
            KinectLeft.Value = Properties.Settings.Default.KinectLeft;
            KinectBottom.Value = Properties.Settings.Default.KinectBottom;
            KinectDepthCull.Value = Properties.Settings.Default.KinectDepthCull;
            KinectOpacity.Value = Properties.Settings.Default.KinectOpacity;
            HexagonOpacity.Value = Properties.Settings.Default.HexagonOpacity;
            SoundVolume.Value = Properties.Settings.Default.SoundVolume;
            txtUdpMessage.Text = Properties.Settings.Default.UdpMessage;
            txtUdpMessageRandom.Text = Properties.Settings.Default.UdpMessageRandom;
            txtHexSize.Text = Properties.Settings.Default.HexSize;
            txtHexHeight.Text = Properties.Settings.Default.HexHeight;
            txtHexWidth.Text = Properties.Settings.Default.HexWidth;
            txtVideoFolderPath.Text = Properties.Settings.Default.VideoFolderPath;
            //start videos
            SetMainAndScanningVideos();
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.IpAddress = txtIpAddress.Text;
            Properties.Settings.Default.Port = txtPort.Text;
            Properties.Settings.Default.ScanningCancel = chkScanningCancel.IsChecked == true;
            Properties.Settings.Default.ScanningMultiplier = txtScanningMultiplier.Text;
            Properties.Settings.Default.ScanningBars = chkScanningBars.IsChecked == true;
            Properties.Settings.Default.TowerTriggerWait = txtTowerTriggerWait.Text;
            Properties.Settings.Default.LoadingImagePath = txtLoadingImagePath.Text;
            Properties.Settings.Default.VideoPath = txtVideoPath.Text;
            Properties.Settings.Default.KinectRotation = KinectRotation.Value;
            Properties.Settings.Default.KinectScale = KinectScale.Value;
            Properties.Settings.Default.KinectLeft = KinectLeft.Value;
            Properties.Settings.Default.KinectBottom = KinectBottom.Value;
            Properties.Settings.Default.KinectDepthCull = KinectDepthCull.Value;
            Properties.Settings.Default.KinectOpacity = KinectOpacity.Value;
            Properties.Settings.Default.SoundVolume = SoundVolume.Value;
            Properties.Settings.Default.HexagonOpacity = HexagonOpacity.Value;
            Properties.Settings.Default.UdpMessage = txtUdpMessage.Text;
            Properties.Settings.Default.UdpMessageRandom = txtUdpMessageRandom.Text;
            Properties.Settings.Default.HexSize = txtHexSize.Text;
            Properties.Settings.Default.HexHeight = txtHexHeight.Text;
            Properties.Settings.Default.HexWidth = txtHexWidth.Text;
            Properties.Settings.Default.VideoFolderPath = txtVideoFolderPath.Text;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the depth stream to receive depth frames
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                
                // Allocate space to put the depth pixels we'll receive
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the color pixels we'll create
                this.colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new depth frame data
                this.sensor.DepthFrameReady += this.SensorDepthFrameReady;

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                this.sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        private double ScanningProgressCount = 0;
        private double UploadProgressCount = 0;
        private double CompleteCount = 0;
        private bool Triggerd = false;
        private double DotsCounter = 0;

        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            double scanningMultiplyer = double.Parse(txtScanningMultiplier.Text);

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            if (skeletons.Length != 0)
            {
                int count = 0;
                foreach (Skeleton skel in skeletons)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked) count += 1;
                }
                if (count > 0)
                {
                    ScanningProgressCount += 2;
                    UploadProgressCount += 0.5;
                    Scanning.Visibility = Visibility.Visible;
                    Loading.Visibility = Visibility.Visible;
                    ShowStatusText("Skeletons In View = " + count.ToString());
                } else
                {
                    CompleteCount = 0;
                    Triggerd = false;
                    if (chkScanningCancel.IsChecked == true)
                    {
                        ScanningProgressCount = 0;
                        UploadProgressCount = 0;
                    }
                    else
                    {
                        ScanningProgressCount -= 2;
                        UploadProgressCount -= 2;
                    }
                    Scanning.Visibility = Visibility.Hidden;
                    Loading.Visibility = Visibility.Hidden;
                    if (ScanningProgressCount < 0) ScanningProgressCount = 0;
                    if (UploadProgressCount < 0) UploadProgressCount = 0;
                }
            } else
            {
                Scanning.Visibility = Visibility.Hidden;
                Loading.Visibility = Visibility.Hidden;
            }

            ScanningProgress.Value = ScanningProgressCount;
            UploadProgress.Value = UploadProgressCount;
            if (ScanningProgressCount > 0 || UploadProgressCount > 0)
            {
                ShowStatusText("Scanning progress: " + ScanningProgressCount.ToString() + ", upload:" + UploadProgressCount.ToString());
            }

            if(ScanningProgressCount > 0 && ScanningProgressCount < 20 * scanningMultiplyer)
            {
                lblScanning.Content = "Lifeforms detected - starting scan";
            }
            if (ScanningProgressCount > 20 * scanningMultiplyer && ScanningProgressCount < 90 * scanningMultiplyer)
            {
                double progress = (100 / (100 * scanningMultiplyer)) * ScanningProgressCount;
                lblScanning.Content = "Scanning in progress " + progress.ToString();
            }
            if (ScanningProgressCount > 100 * scanningMultiplyer && UploadProgressCount < 90 * scanningMultiplyer)
            {
                double progress = (100 / (100 * scanningMultiplyer)) * UploadProgressCount;
                lblScanning.Content = "Uploading to Voyager " + progress.ToString();
            }
            if (ScanningProgressCount > 100 * scanningMultiplyer && UploadProgressCount > 90 * scanningMultiplyer)
            {
                if (UploadProgressCount <= 100 * scanningMultiplyer ||
                    (UploadProgressCount > 100 * scanningMultiplyer && !Triggerd))
                {
                    DotsCounter += 1;
                    if (DotsCounter > 3) DotsCounter = 0;
                    string dots = "";
                    for (var x = 0; x < DotsCounter; x++)
                    {
                        dots += ".";
                    }
                    lblScanning.Content = "Uploading complete " + dots;
                }
            }

            //reset after complete count
            if(CompleteCount > 50)
            {
                CompleteCount = 0;
                Triggerd = false;
                if (chkScanningCancel.IsChecked == true)
                {
                    ScanningProgressCount = 0;
                    UploadProgressCount = 0;
                }
                else
                {
                    ScanningProgressCount -= 2;
                    UploadProgressCount -= 2;
                }
                Scanning.Visibility = Visibility.Hidden;
                Loading.Visibility = Visibility.Hidden;
                if (ScanningProgressCount < 0) ScanningProgressCount = 0;
                if (UploadProgressCount < 0) UploadProgressCount = 0;
            }

            if (ScanningProgressCount > 100 * scanningMultiplyer)
            {
                ScanningProgressCount = 100 * scanningMultiplyer;
                //send message
                //ShowStatusText("Scanning complete");
            }
            if(UploadProgressCount == 100 * scanningMultiplyer)
            {
                CompleteCount += 1;
                int towerTrigger = int.Parse(txtTowerTriggerWait.Text);
                if (lastTowerTrigger < DateTime.Now.AddSeconds(0 - towerTrigger))
                {
                    lblScanning.Content = "Unique DNA match recieved";
                    UdpSendMessage(txtUdpMessageRandom.Text);
                    //reset recieved date
                    lastTowerTrigger = DateTime.Now;
                    Triggerd = true;
                } else
                {
                    Triggerd = false;
                }
                //ShowStatusText("Upload complete");
            }
            if (CompleteCount > 0)
            {
                CompleteCount += 1;
            }
            if (UploadProgressCount > 100.5 * scanningMultiplyer)
            {
                UploadProgressCount = 100.5 * scanningMultiplyer;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    // Get the min and max reliable depth for the current frame
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth - maxDepthOffset;

                    // Convert the depth to RGB
                    int colorPixelIndex = 0;
                    for (int i = 0; i < this.depthPixels.Length; ++i)
                    {
                        // Get the depth for this pixel
                        short depth = depthPixels[i].Depth;

                        // To convert to a byte, we're discarding the most-significant
                        // rather than least-significant bits.
                        // We're preserving detail, although the intensity will "wrap."
                        // Values outside the reliable depth range are mapped to 0 (black).

                        // Note: Using conditionals in this loop could degrade performance.
                        // Consider using a lookup table instead when writing production code.
                        // See the KinectDepthViewer class used by the KinectExplorer sample
                        // for a lookup table example.
                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                        // Write out blue byte
                        this.colorPixels[colorPixelIndex++] = intensity;

                        // Write out green byte
                        this.colorPixels[colorPixelIndex++] = intensity;

                        // Write out red byte                        
                        this.colorPixels[colorPixelIndex++] = intensity;
                                                
                        // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                        // If we were outputting BGRA, we would write alpha here.
                        ++colorPixelIndex;
                    }

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);

                    var transformGroup = new TransformGroup();
                    //var scaleTransform = new ScaleTransform(1.6875, 1.6875);
                    //var scaleTransform = new ScaleTransform(4, 4); //-1300,0,0,-2400
                    //var scaleTransform = new ScaleTransform(3, 3); //-700,0,0,-2400

                    var scaleTransform = new ScaleTransform(KinectScale.Value, KinectScale.Value); //-1300,0,0,-2400
                    Image.Margin = new Thickness(KinectLeft.Value, 0, 0, KinectBottom.Value);
                    var rotateTransform = new RotateTransform(KinectRotation.Value, 0, 0);
                    transformGroup.Children.Add(scaleTransform);
                    transformGroup.Children.Add(rotateTransform);

                    // crop the writeable bitmap based on proper dimension
                    //var cropped = CropImage(this.colorBitmap, X, Y, WIDTH, HEIGHT);

                    //this.colorBitmap.Rotate((int)KinectRotation.Value);
                    // create an image brush from the cropped image
                    var imageBrush = new ImageBrush
                    {
                        ImageSource = this.colorBitmap,
                        Stretch = Stretch.UniformToFill
                    };

                    var transformGroup2 = new TransformGroup();
                    var rotateTransform2 = new RotateTransform(KinectRotation.Value, 0.5, 0.5);
                    transformGroup2.Children.Add(rotateTransform2);
                    imageBrush.RelativeTransform = transformGroup2;

                    // fill the shape with the image brush that we created
                    Hexagon.Fill = imageBrush;

                    //render image
                    KinectImageBox.Child.RenderTransform = transformGroup;


                }
            }

            //set values
            lblKinectBottom.Content = KinectBottom.Value;
            lblKinectDepthCull.Content = KinectDepthCull.Value;
            lblKinectLeft.Content = KinectLeft.Value;
            lblKinectOpacity.Content = KinectOpacity.Value;
            lblKinectRotation.Content = KinectRotation.Value;
            lblKinectScale.Content = KinectScale.Value;
            lblHexagonOpacity.Content = HexagonOpacity.Value;
            lblSoundVolume.Content = SoundVolume.Value;

            if (chkScanningBars.IsChecked == true)
            {
                ScanningProgress.Visibility = (ScanningProgress.Value > 0) ? Visibility.Visible : Visibility.Hidden;
                UploadProgress.Visibility = (UploadProgress.Value > 0) ? Visibility.Visible : Visibility.Hidden;
            } else
            {
                ScanningProgress.Visibility = Visibility.Hidden;
                UploadProgress.Visibility = Visibility.Hidden;
            }
        }
          
        /// <summary>
        /// Handles the user clicking on the screenshot button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ButtonScreenshotClick(object sender, RoutedEventArgs e)
        {
            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.ConnectDeviceFirst;
                return;
            }

            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            string path = Path.Combine(myPhotos, "KinectSnapshot-" + time + ".png");

            // write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                this.statusBarText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", Properties.Resources.ScreenshotWriteSuccess, path);
            }
            catch (IOException)
            {
                this.statusBarText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", Properties.Resources.ScreenshotWriteFailed, path);
            }
        }
        
        /// <summary>
        /// Handles the checking or unchecking of the near mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxNearModeChanged(object sender, RoutedEventArgs e)
        {
            //if (this.sensor != null)
            //{
            //    // will not function on non-Kinect for Windows devices
            //    try
            //    {
            //        if (this.checkBoxNearMode.IsChecked.GetValueOrDefault())
            //        {
            //            this.sensor.DepthStream.Range = DepthRange.Near;
            //        }
            //        else
            //        {
            //            this.sensor.DepthStream.Range = DepthRange.Default;
            //        }
            //    }
            //    catch (InvalidOperationException)
            //    {
            //    }
            //}
        }


        private void SetMainAndScanningVideos()
        {
            try
            {
                // Open document 
                string filename = txtVideoFolderPath.Text + @"\Pergola Main.mp4";
                Vid.Stop();
                Vid.Source = new Uri(filename);
                RestartVideo(Vid);

                // Open document 
                filename = txtVideoFolderPath.Text + @"\Pergola Scanning.mp4";
                ScanningVid.Stop();
                ScanningVid.Source = new Uri(filename);
                RestartVideo(ScanningVid);

                //start sound
                SoundComplete(true);
            }
            catch { }
        }

        private void SetScanCompleteVideo(string colour)
        {

            try
            {
                // Open document 
                string filename = txtVideoFolderPath.Text + @"\Pergola Scan " +  colour + ".mp4";
                ScanCompleteVideo.Stop();
                ScanCompleteVideo.Source = new Uri(filename);
                ScanCompleteVideo.Position = TimeSpan.Zero;
                
                Vid.Visibility = Visibility.Hidden;
                ScanCompleteVideo.Play();
                ScanCompleteVideo.Visibility = Visibility.Visible;
                
            }
            catch { }
        }
        
        private void UdpSendMessage(string msg)
        { 
            try
            {
                if (msg.Contains("|"))
                {
                    //send message -randomize
                    string[] messages = msg.Split('|');
                    var random = new Random();
                    var randomIndex = random.Next(0, messages.Length);
                    msg = messages[randomIndex];
                }
                UdpClient udpClient = new UdpClient(txtIpAddress.Text, int.Parse(txtPort.Text));
                Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
                udpClient.Send(sendBytes, sendBytes.Length);

                SetScanCompleteVideo(msg);

                ShowStatusText("Udp Sent: '" + msg + "' to " + address + ":" + port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void OscTransmitMsg(string address, object value)
        {

            if (transmitter != null)
            {
                try
                {
                    OSCMessage OscXMessage = new OSCMessage(address, value);
                    transmitter.Send(OscXMessage);
                }
                catch (Exception ex)
                {
                    string blah = ex.Message;
                }
            }
        }


        private void OscTransmitterConnect()
        {
            try
            {
                address = IPAddress.Parse(txtIpAddress.Text);
                port = int.Parse(txtPort.Text);

                if (transmitter != null) transmitter.Close();

                transmitter = new OSCTransmitter(address.ToString(), port);

                ShowStatusText("Connected: " + address + ":" + port);
                ToggleConnectView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void Vid_MediaEnded(object sender, RoutedEventArgs e)
        {
            RestartVideo(Vid);
        }

        private void RestartVideo(System.Windows.Controls.MediaElement video)
        {
            video.Position = TimeSpan.Zero;
            video.Play();
        }

        private void DepthStreamRangeToggle()
        {
            if (this.sensor != null)
            {
                // will not function on non-Kinect for Windows devices
                try
                {
                    if (this.sensor.DepthStream.Range == DepthRange.Default)
                    {
                        this.sensor.DepthStream.Range = DepthRange.Near;
                        ShowStatusText("DepthStream.Range = Near");
                    }
                    else
                    {
                        this.sensor.DepthStream.Range = DepthRange.Default;
                        ShowStatusText("DepthStream.Range = Default");
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
        private void FullScreenToggle()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            } else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

        private int maxDepthOffset = 1000;
        private void StepMaxDepthOffset(bool up)
        {
            maxDepthOffset = (up) ? maxDepthOffset + 100 : maxDepthOffset - 100;
            ShowStatusText("maxDepthOffset: " + maxDepthOffset);
        }

        //private int maxDepthOffset = 1000;
        private void StepOpacity(bool up)
        {
            var opacity = (up) ? Image.Opacity + 0.05 : Image.Opacity - 0.05;
            if (opacity >= 0 && opacity <= 1) Image.Opacity = opacity;
            ShowStatusText("Image.Opacity: " + Image.Opacity);
        }

        private bool toggleStatusText = true;
        private void ShowStatusText(string text)
        {
            statusBarText.Text = (toggleStatusText)?  text : "";
        }

        private void ToggleStatusBar()
        {
            toggleStatusText = !toggleStatusText;
            statusBarText.Visibility = (toggleStatusText) ? Visibility.Visible : Visibility.Hidden;
            statusBarHelpText.Visibility = (toggleStatusText) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ToggleConnectView()
        {
            ConnectView.Visibility = (ConnectView.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden;
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ShowStatusText("Key pressed: " + e.Key);
            switch (e.Key)
            {
                case System.Windows.Input.Key.D:
                    DepthStreamRangeToggle();
                    break;
                case System.Windows.Input.Key.Enter:
                    //replay video
                    RestartVideo(Vid);
                    break;
                case System.Windows.Input.Key.Space:
                    //replay video
                    RestartVideo(Vid);
                    break;
                case System.Windows.Input.Key.F12:
                    //replay video
                    FullScreenToggle();
                    break;
                case System.Windows.Input.Key.F1:
                    ToggleConnectView();
                    //ToggleStatusBar();
                    break;
                case System.Windows.Input.Key.F2:
                    this.Topmost = !this.Topmost;
                    //ToggleStatusBar();
                    break;
                case System.Windows.Input.Key.Up:
                    StepMaxDepthOffset(true);
                    break;
                case System.Windows.Input.Key.Down:
                    StepMaxDepthOffset(false);
                    break;
                case System.Windows.Input.Key.Right:
                    StepOpacity(true);
                    break;
                case System.Windows.Input.Key.Left:
                    StepOpacity(false);
                    break;
                case System.Windows.Input.Key.Escape:
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to Quit?", "Quit", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                    break;
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            OscTransmitterConnect();
        }

        private void UdpSend_Click(object sender, RoutedEventArgs e)
        {
            UdpSendMessage(txtUdpMessage.Text);
        }

        private void btnVideoBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".mp4";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                try
                {
                    // Open document 
                    string filename = dlg.FileName;
                    txtVideoPath.Text = filename;
                    Vid.Stop();
                    Vid.Source = new Uri(filename);
                    RestartVideo(Vid);
                }
                catch { }
            }
        }

        private void btnVideoFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txtVideoFolderPath.Text = dialog.SelectedPath;
                    //start videos
                    SetMainAndScanningVideos();
                }
            }
            
        }

        private void btnLoadingImageBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gif";
            dlg.Filter = "GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                txtLoadingImagePath.Text = filename;

                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(filename);
                    img.EndInit();
                    //ImageBehavior.SetAnimatedSource(LoadingImage, img);
                }
                catch { }
            }
        }
        private void KinectOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Image.Opacity = KinectOpacity.Value;
        }
        

        private void HexagonOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Hexagon.Opacity = HexagonOpacity.Value;
        }

        private void KinectDepthCull_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            maxDepthOffset = (int)KinectDepthCull.Value;
        }

        private void hexagon_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Shapes.Path hexagon = sender as System.Windows.Shapes.Path;
            try
            {
                Hexagon.Width = int.Parse(txtHexWidth.Text);
                Hexagon.Height = int.Parse(txtHexHeight.Text);
            }
            catch { }
            CreateDataPath(hexagon.Width, hexagon.Height);
        }

        PathFigure figure;
        private void CreateDataPath(double width, double height)
        {
            height -= Hexagon.StrokeThickness;
            width -= Hexagon.StrokeThickness;

            PathGeometry geometry = new PathGeometry();
            figure = new PathFigure();

            //See for figure info http://etc.usf.edu/clipart/50200/50219/50219_area_hexagon_lg.gif
            figure.StartPoint = new Point(0.25 * width, 0);
            AddPoint(0.75 * width, 0);
            AddPoint(width, 0.5 * height);
            AddPoint(0.75 * width, height);
            AddPoint(0.25 * width, height);
            AddPoint(0, 0.5 * height);
            figure.IsClosed = true;
            geometry.Figures.Add(figure);
            Hexagon.Data = geometry;
        }

        private void AddPoint(double x, double y)
        {
            LineSegment segment = new LineSegment();
            segment.Point = new Point(x + 0.5 * Hexagon.StrokeThickness,
                y + 0.5 * Hexagon.StrokeThickness);
            figure.Segments.Add(segment);
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //save
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to Save the settings?", "Save", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveSettings();
            }
        }

        private void HexSize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double size = double.Parse(txtHexSize.Text);
                txtHexHeight.Text = (SystemParameters.PrimaryScreenHeight * (size / 100)).ToString();
                txtHexWidth.Text = (SystemParameters.PrimaryScreenWidth * (size / 100)).ToString();
            }
            catch { }
        }

        private void ScanCompleteVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            ScanCompleteVideo.Stop();
            ScanCompleteVideo.Visibility = Visibility.Hidden;
            Vid.Visibility = Visibility.Visible;
        }

        private void ScanningVid_MediaEnded(object sender, RoutedEventArgs e)
        {
            RestartVideo(ScanningVid);
        }

        private void SoundComplete(bool firstTime = false)
        {
            try
            {
                // Open document 
                string filename = txtVideoFolderPath.Text + @"\music.mp3";
                if (System.IO.File.Exists(filename))
                {
                    soundPlayer.Open(new Uri(filename));
                    soundPlayer.Play();
                    soundPlayer.Volume = SoundVolume.Value;
                    if(firstTime) soundPlayer.MediaEnded += SoundPlayer_MediaEnded;
                }
            }
            catch { }
        }

        private void SoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            SoundComplete();
        }

        private void SoundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            soundPlayer.Volume = SoundVolume.Value;
        }
    }
}