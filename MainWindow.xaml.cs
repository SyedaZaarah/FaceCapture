<<<<<<< HEAD
﻿// WPF + Emgu.CV + Newtonsoft.Json

=======
﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.VisualBasic;  
using Newtonsoft.Json;
>>>>>>> aaf8317ad038444b84024b5d6db0e75331acf3ce
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Newtonsoft.Json;

namespace FaceRecognition
{
    public partial class MainWindow : Window
    {
        private VideoCapture _capture;
        private CascadeClassifier _faceCascade;
        private CascadeClassifier _eyeCascade;
        private CascadeClassifier _mouthCascade;
        private LBPHFaceRecognizer _faceRecognizer;

        private Dictionary<int, string> _labelNameMap = new();
        private Dictionary<string, int> _nameLabelMap = new();
        private int _nextLabel = 0;

        private Dictionary<string, string> _appointments = new();

        private bool _isProcessing = false;
        private bool _isWaitingForStart = true;

        public MainWindow()
        {
            InitializeComponent();

            LoadAppointments();

            _faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml.xml");
            _eyeCascade = new CascadeClassifier("haarcascade_eye.xml");
            _mouthCascade = new CascadeClassifier("haarcascade_mcs_mouth.xml");

            _faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);

            LoadKnownFaces();

            _capture = new VideoCapture();
            _capture.ImageGrabbed += Capture_ImageGrabbed;
            _capture.Start();

            StatusLabel.Content = "Waiting for you to say 'Hi' or click Start...";
        }

        private void LoadAppointments()
        {
            var path = "appointments.json";
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                _appointments = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
            }
            else
            {
                _appointments = new Dictionary<string, string>();
            }
        }

        private void SaveAppointments()
        {
            File.WriteAllText("appointments.json", JsonConvert.SerializeObject(_appointments, Formatting.Indented));
        }

        private void LoadKnownFaces()
        {
            var dir = "SavedFaces";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var images = new List<Image<Gray, byte>>();
            var labels = new List<int>();

            foreach (var file in Directory.GetFiles(dir, "*.jpg"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                var img = new Image<Gray, byte>(file).Resize(100, 100, Inter.Cubic);

                if (!_nameLabelMap.ContainsKey(name))
                {
                    _nameLabelMap[name] = _nextLabel;
                    _labelNameMap[_nextLabel] = name;
                    _nextLabel++;
                }

                images.Add(img);
                labels.Add(_nameLabelMap[name]);
            }

            if (images.Count > 0)
            {
                Mat[] mats = images.Select(img => img.Mat).ToArray();
                _faceRecognizer.Train(mats, labels.ToArray());
            }

        }

        private void Capture_ImageGrabbed(object? sender, EventArgs e)
        {
            if (_isProcessing || _isWaitingForStart)
                return;

            _isProcessing = true;
            try
            {
                Mat frame = new();
                _capture.Retrieve(frame);
                if (frame.IsEmpty)
                {
                    _isProcessing = false;
                    return;
                }

                var img = frame.ToImage<Bgr, byte>();
                var gray = img.Convert<Gray, byte>();

                var faces = _faceCascade.DetectMultiScale(gray, 1.1, 4);

                Dispatcher.Invoke(() =>
                {
                    CameraPreview.Source = ToBitmapSource(img);
                });

                if (faces.Length > 0)
                {
                    // Face detected
                    Dispatcher.Invoke(() =>
                    {
                        StatusLabel.Content = "Face detected! Please click 'Say Hi / Start'";
                    });
                }
                else
                {
<<<<<<< HEAD
                    Dispatcher.Invoke(() =>
=======
                    
                    _timer.Stop();
                    _capture.Pause();

                    var hasAppointment = MessageBox.Show("Do you have an appointment?", "Appointment Check", MessageBoxButton.YesNo);

                    if (hasAppointment == MessageBoxResult.No)
>>>>>>> aaf8317ad038444b84024b5d6db0e75331acf3ce
                    {
                        StatusLabel.Content = "Waiting for face...";
                    });
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

<<<<<<< HEAD
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isWaitingForStart)
            {
                _isWaitingForStart = false;
                StatusLabel.Content = "Detecting face, please wait...";
                await RunUserFlow();
                _isWaitingForStart = true;
                StatusLabel.Content = "Waiting for you to say 'Hi' or click Start...";
            }
        }

        private async Task RunUserFlow()
        {
            // Grab current frame and detect face
            Mat frame = new();
            _capture.Retrieve(frame);
            var img = frame.ToImage<Bgr, byte>();
            var gray = img.Convert<Gray, byte>();

            var faces = _faceCascade.DetectMultiScale(gray, 1.1, 4);

            if (faces.Length == 0)
            {
                MessageBox.Show("No face detected. Please try again.");
                return;
            }

            var faceRect = faces[0];

            // Validate eyes open and mouth closed and basic lighting
            if (!ValidateFaceParameters(img, faceRect))
            {
                MessageBox.Show("Face parameters not valid. Please adjust your face, open your eyes and close your mouth.");
                return;
            }

            // Ask appointment question
            var answer = MessageBox.Show("Do you have an appointment?", "Appointment", MessageBoxButton.YesNoCancel);
            if (answer == MessageBoxResult.Yes)
            {
                // Get name
                string name = Microsoft.VisualBasic.Interaction.InputBox("Enter your name:", "Name");
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Name cannot be empty.");
                    return;
=======
                        bool nameFound = _appointments.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                        if (!nameFound)
                        {
                            MessageBox.Show("No appointment found for this name.");
                        }
                        else
                        {
                           
                            bool faceMatch = VerifyFaceMatch(faceImg, name);
                            if (faceMatch)
                            {
                                MessageBox.Show("Access Granted");
                                label = name;
                            }
                            else
                            {
                                MessageBox.Show("Access Denied - Face does not match appointment");
                                label = "Access Denied";
                            }
                        }
                    }

                    
                    _capture.Start();
                    _timer.Start();
>>>>>>> aaf8317ad038444b84024b5d6db0e75331acf3ce
                }

                // Check if name in appointments
                if (!_appointments.ContainsKey(name))
                {
                    MessageBox.Show("No appointment found for this name.");
                    return;
                }

                // Capture photo (passport size, white background)
                var passportImg = CapturePassportPhoto(img, faceRect);

                // Save captured photo temporarily to compare
                var tempPath = Path.Combine("Temp", "capture.jpg");
                Directory.CreateDirectory("Temp");
                passportImg.Save(tempPath);

                // Compare with saved image
                var savedPath = Path.Combine("SavedFaces", name + ".jpg");
                if (!File.Exists(savedPath))
                {
                    MessageBox.Show("No saved image found for this name. Saving current photo as reference.");
                    passportImg.Save(savedPath);
                    LoadKnownFaces(); // reload faces
                    MessageBox.Show("Reference photo saved. Access granted.");
                    return;
                }

                bool match = await Task.Run(() => CompareFaces(passportImg, savedPath));

                if (match)
                {
                    MessageBox.Show("Face matched! Access granted.");
                }
                else
                {
                    MessageBox.Show("Face does not match saved image. Access denied.");
                }
            }
            else if (answer == MessageBoxResult.No)
            {
                // Visitor flow
                var reason = MessageBox.Show("Are you here to make an appointment?\nYes = Make Appointment\nNo = Just Visiting", "Visitor", MessageBoxButton.YesNoCancel);
                if (reason == MessageBoxResult.Yes)
                {
                    // Make appointment
                    string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter your name to make an appointment:", "Make Appointment");
                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        MessageBox.Show("Name cannot be empty.");
                        return;
                    }

                    _appointments[newName] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    SaveAppointments();
                    MessageBox.Show("Appointment saved.");
                }
                else if (reason == MessageBoxResult.No)
                {
                    string details = Microsoft.VisualBasic.Interaction.InputBox("Please provide your details:", "Visitor Details");
                    File.AppendAllText("visitor_reasons.log", $"{DateTime.Now}: {details}\n");
                    MessageBox.Show("Directions: Room 102 on the right. 🗺");
                }
            }
        }

        private Image<Bgr, byte> CapturePassportPhoto(Image<Bgr, byte> img, System.Drawing.Rectangle faceRect)
        {
            // Expand rect to include forehead etc
            var cropRect = new System.Drawing.Rectangle(
                Math.Max(faceRect.X - faceRect.Width / 2, 0),
                Math.Max(faceRect.Y - faceRect.Height / 2, 0),
                Math.Min(faceRect.Width * 2, img.Width - faceRect.X + faceRect.Width / 2),
                Math.Min(faceRect.Height * 2, img.Height - faceRect.Y + faceRect.Height / 2)
            );

            var faceImg = img.Copy(cropRect);

            // Create white background image
            var whiteBg = new Image<Bgr, byte>(cropRect.Width, cropRect.Height, new Bgr(255, 255, 255));

            // Overlay face on white background (basic background removal)
            CvInvoke.Resize(faceImg, faceImg, whiteBg.Size);
            faceImg.CopyTo(whiteBg);

            return whiteBg;
        }

        private bool ValidateFaceParameters(Image<Bgr, byte> img, System.Drawing.Rectangle faceRect)
        {
            var faceImgGray = img.Copy(faceRect).Convert<Gray, byte>();

            var eyes = _eyeCascade.DetectMultiScale(faceImgGray, 1.1, 4);
            var mouths = _mouthCascade.DetectMultiScale(faceImgGray, 1.1, 4);

            bool eyesOpen = eyes.Length >= 1;
            bool mouthClosed = mouths.Length == 0;

            // Additional lighting check (simple brightness average)
            var brightness = faceImgGray.GetAverage().Intensity;
            bool goodLighting = brightness > 50; // tweak threshold

            return eyesOpen && mouthClosed && goodLighting;
        }

        private bool CompareFaces(Image<Bgr, byte> liveImg, string savedImagePath)
        {
            try
            {
                var savedImgGray = new Image<Gray, byte>(savedImagePath).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
                var liveImgGray = liveImg.Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);

                // Simple pixel difference method
                var diff = liveImgGray.AbsDiff(savedImgGray);
                double diffSum = CvInvoke.Sum(diff).V0;

                double threshold = 1000000; // tune this threshold

                return diffSum < threshold;
            }
            catch
            {
                return false;
            }
        }

        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
        }

        private BitmapSource ToBitmapSource(Image<Bgr, byte> image)
        {
            using var bitmap = image.ToBitmap();
            var hBitmap = bitmap.GetHbitmap();

            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap); // Clean up native resource
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _capture?.Dispose();
            _faceCascade?.Dispose();
            _eyeCascade?.Dispose();
            _mouthCascade?.Dispose();
            _faceRecognizer?.Dispose();
        }
    }
}

