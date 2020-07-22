using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.Util;
using Emgu.CV.Ocl;
using System.Diagnostics;
using AForge.Imaging;
using AForge;

namespace Imagecaptureexp
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        private Mat frame;
        Rectangle[] rects;
        public Form1()
        {
            InitializeComponent();
            capture = new VideoCapture(0);
            //capture.ImageGrabbed += ProcessFrame;
            frame = new Mat();
            //capture.ImageGrabbed += Device_NewFrame;
            //Capture.Image += takesnap;
            Console.WriteLine(111);
            if (capture != null)
            {

                try
                { capture.Start(); }

                catch (Exception e)
                { }

            }
        }

       // FilterInfoCollection filter;
        
        int imagenumber = 0;

        private void ProcessFrame(object sender, EventArgs e)
        { 
          if(capture!=null && capture.Ptr != IntPtr.Zero)
            {
                capture.Retrieve(frame, 0);
                picbox.Image = frame.Bitmap;

            }
        }

            private void Form1_Load(object sender, EventArgs e)
        {
            //capture.ImageGrabbed += Device_NewFrame;
            capture.ImageGrabbed += takesnap;

        }

        public Stopwatch watch { get; set; }

        private void takesnap(object sender, EventArgs eventArgs)
        {
            

            capture.Retrieve(frame, 0);
            Image<Gray, byte> capturedimage = frame.ToImage<Gray, byte>();

            //Image<Gray, byte> gray_image = capturedimage.Convert<Gray, byte>();

            Image<Gray, byte> cannyimage = new Image<Gray, byte>(capturedimage.Width, capturedimage.Height, new Gray(0));

            cannyimage = capturedimage.Canny(90, 20);

            //MoravecCornersDetector mcd = new MoravecCornersDetector();
            //List<IntPoint> edges = mcd.ProcessImage(cannyimage.Bitmap);

            //foreach (IntPoint edge in edges)
            //{
            //    Console.WriteLine(edge);
            //}


            picbox.Image = cannyimage.Bitmap;



            for (int i = 0; i < capturedimage.Width; i++)
            {
                for (int j = 0; j < capturedimage.Height; j++)
                {
                    Color pixel = cannyimage.Bitmap.GetPixel(i, j);

                    Console.WriteLine(pixel);
                }
            }


            //if (rects.Length > 0)
            //{
            //    string j = "image" + imagenumber + ".jpg";
            //    //capturedimage.Save(string.Format(@"C:\games\image\{0}", j));
            //    imagenumber = imagenumber + 1;
            //}

        }

        static readonly CascadeClassifier faceClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private void Device_NewFrame(object sender, EventArgs eventArgs)
        {
            //capture.Retrieve(frame);

            //Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            //try
            //{
            //    var filter = new Mirror(false, true);
            //    filter.ApplyInPlace(bitmap);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            capture.Retrieve(frame, 0);
            Image<Bgr, byte> capturedimage = frame.ToImage<Bgr, byte>();

            Image<Gray, byte> image = capturedimage.Convert<Gray, byte>().Clone();
            Rectangle[] facerectangles = faceClassifier.DetectMultiScale(image, 1.2, 1);

            if (facerectangles.Length > 0)
            {
                int NewpointX = 472 - facerectangles[0].X;
                int NewpointY = 240 - facerectangles[0].Y;
                Console.WriteLine(facerectangles[0].X);
                Console.WriteLine(facerectangles[0].Y);
                textBox1.Invoke(new Action(() => textBox1.Text = facerectangles[0].X.ToString()));
                textBox2.Invoke(new Action(() => textBox2.Text = NewpointY.ToString()));
            }

            rects = facerectangles;

            foreach (Rectangle rectangle in facerectangles)
            {

                capturedimage.Draw(rectangle, new Bgr(Color.Green), 2);


            }

           // picbox.Image = capturedimage.Bitmap;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
          
                Application.Exit();
                capture.Stop();
            
        }
    }
}
      


