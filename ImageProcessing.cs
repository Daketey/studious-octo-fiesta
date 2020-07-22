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

namespace Imagecaptureexp
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        private Mat frame;
        Rectangle[] rects;
        public Stopwatch watch { get; set; }
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
                { Console.WriteLine("Failed"); }
            }
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (capture != null && capture.Ptr != IntPtr.Zero)
            {
                capture.Retrieve(frame, 0);
                picbox.Image = frame.Bitmap;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //capture.ImageGrabbed += Device_NewFrame;
            capture.ImageGrabbed += Imageprocess;
        }

        int imagenumber = 0;

        private void Imageprocess(object sender, EventArgs eventArgs)
        {


            capture.Retrieve(frame, 0);
            Image<Gray, byte> capturedimage = frame.ToImage<Gray, byte>();

            //Image<Gray, byte> gray_image = capturedimage.Convert<Gray, byte>();
            Image<Gray, byte> gauss = capturedimage.SmoothGaussian(3, 3, 34.3, 45.3);     //Applying gaussian blue


            Image<Gray, byte> cannyimage = new Image<Gray, byte>(capturedimage.Width, capturedimage.Height, new Gray(0));    //Setting up the parameters for the canny image

            cannyimage = gauss.Canny(90, 20);                      //Applying canny image 

            picbox.Image = cannyimage.Bitmap;                                          //Displaying canny image
            picbox.Paint += new PaintEventHandler(this.drawrect);                      //Drawing the rectangle over the video


            //for (int x = 0; i < capturedimage.Width; i++)                                            //To get the pixel data of the edges fronm the the canny image
            //{
            //    for (int y = 0; j < capturedimage.Height; j++)                                         
            //    {
            //        Color pixel = cannyimage.Bitmap.GetPixel(x, y);

            //        Console.WriteLine(pixel);
            //    }
            //}

            //if (rects.Length > 0)
            //{
            //    string j = "image" + imagenumber + ".jpg";
            //    //capturedimage.Save(string.Format(@"C:\games\image\{0}", j));
            //    imagenumber = imagenumber + 1;
            //}

        }

        private void drawrect(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Red, GetRectangle());                  //Drawing the set rectangle
        }

        Rectangle quadrant;

        private Rectangle GetRectangle()                                         //Setting the size and postion of rectangle selection
        {
            quadrant = new Rectangle();
            quadrant.X = picbox.Width/2;
            quadrant.Y = picbox.Height/2;
            quadrant.Width = 50;
            quadrant.Height = 100;

            return quadrant;
        }

        static readonly CascadeClassifier faceClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private void Device_NewFrame(object sender, EventArgs eventArgs)
        {
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



