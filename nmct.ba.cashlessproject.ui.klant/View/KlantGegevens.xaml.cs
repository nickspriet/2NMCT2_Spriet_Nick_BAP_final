using AForge.Video;
using nmct.ba.cashlessproject.helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using ZXing;

namespace nmct.ba.cashlessproject.ui.klant.View
{
    /// <summary>
    /// Interaction logic for KlantGegevens.xaml
    /// </summary>
    public partial class KlantGegevens : UserControl
    {

        public KlantGegevens()
        {
            InitializeComponent();
        } 
        
        
        /*
        private struct Device
        {
            public int Index;
            public string Name;
            public override string ToString()
            {
                return Name;
            }
        }


        private readonly CameraDevices camDevices;
        private Bitmap currentBitmapForDecoding;
        private readonly Thread decodingThread;
        private Result currentResult;
        //private readonly System.Windows.Media.Pen resultRectPen;


        public KlantGegevens()
        {
            InitializeComponent();

            camDevices = new CameraDevices();

            decodingThread = new Thread(DecodeBarcode);
            decodingThread.Start();
            LoadDevicesToCombobox();
            //pictureBox1.Paint += pictureBox1_Paint;
            //resultRectPen = new Pen(Color.Green, 10);
        }

        /*
        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (currentResult == null)
                return;

            if (currentResult.ResultPoints != null && currentResult.ResultPoints.Length > 0)
            {
                var resultPoints = currentResult.ResultPoints;
                var rect = new System.Drawing.Rectangle((int)resultPoints[0].X, (int)resultPoints[0].Y, 1, 1);
                foreach (var point in resultPoints)
                {
                    if (point.X < rect.Left)
                        rect = new System.Drawing.Rectangle((int)point.X, rect.Y, rect.Width + rect.X - (int)point.X, rect.Height);
                    if (point.X > rect.Right)
                        rect = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width + (int)point.X - rect.X, rect.Height);
                    if (point.Y < rect.Top)
                        rect = new System.Drawing.Rectangle(rect.X, (int)point.Y, rect.Width, rect.Height + rect.Y - (int)point.Y);
                    if (point.Y > rect.Bottom)
                        rect = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height + (int)point.Y - rect.Y);
                }
                using (var g = pictureBox1.CreateGraphics())
                {
                    g.DrawRectangle(resultRectPen, rect);
                }
            }
        }*/



        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    this.OnClosing(e);
        //    if (!e.Cancel)
        //    {
        //        decodingThread.Abort();
        //        if (camDevices.Current != null)
        //        {
        //            camDevices.Current.NewFrame -= Current_NewFrame;
        //            if (camDevices.Current.IsRunning)
        //            {
        //                camDevices.Current.SignalToStop();
        //            }
        //        }
        //    }
        //}
        /*
        private void LoadDevicesToCombobox()
        {
            cmbDevice.Items.Clear();
            for (var index = 0; index < camDevices.Devices.Count; index++)
            {
                cmbDevice.Items.Add(new Device { Index = index, Name = camDevices.Devices[index].Name });
            }
        }

        private void Current_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //if (IsDisposed) return;

            try
            {
                if (currentBitmapForDecoding == null)
                {
                    currentBitmapForDecoding = (Bitmap)eventArgs.Frame.Clone();
                }
                Dispatcher.Invoke(new Action<Bitmap>(ShowFrame), eventArgs.Frame.Clone());
            }
            catch (ObjectDisposedException)
            {
                // not sure, why....
            }
        }

        private void ShowFrame(Bitmap frame)
        {
            if (pictureBox1.Width < frame.Width)
            {
                pictureBox1.Width = frame.Width;
            }
            if (pictureBox1.Height < frame.Height)
            {
                pictureBox1.Height = frame.Height;
            }

            pictureBox1.Source = Bitmap2BitmapImage(frame);
        }

        private void DecodeBarcode()
        {
            var reader = new BarcodeReader();
            while (true)
            {
                if (currentBitmapForDecoding != null)
                {
                    var result = reader.Decode(currentBitmapForDecoding);
                    if (result != null)
                    {
                        Dispatcher.Invoke(new Action<Result>(ShowResult), result);
                    }
                    currentBitmapForDecoding.Dispose();
                    currentBitmapForDecoding = null;
                }
                Thread.Sleep(200);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        private void ShowResult(Result result)
        {
            currentResult = result;
            txtBarcodeFormat.Text = result.BarcodeFormat.ToString();

            lstMoney.Items.Add(result.Text);
            txtContent.Text = result.Text;
        }

        private void cmbDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (camDevices.Current != null)
            {
                camDevices.Current.NewFrame -= Current_NewFrame;
                if (camDevices.Current.IsRunning)
                {
                    camDevices.Current.SignalToStop();
                }
            }

            camDevices.SelectCamera(((Device)(cmbDevice.SelectedItem)).Index);
            camDevices.Current.NewFrame += Current_NewFrame;
            camDevices.Current.Start();
        }*/
    }
}
