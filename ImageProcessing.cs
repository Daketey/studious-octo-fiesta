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
using System.Threading;

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
        Thread thread = new Thread(Newprocess);

        private static void Newprocess()
        {
           
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (capture != null)
            {
                capture.Read(frame);
                picbox.Image = frame.Bitmap;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //capture.ImageGrabbed += Device_NewFrame;
            capture.ImageGrabbed += Imageprocess;
            capture.ImageGrabbed += processes;
        }

        private void processes(object sender , EventArgs eventArgs)
        {
            for (int x = 0; x < Rightrect.Width; x++)                                            //To get the pixel data of the edges fronm the the canny image
            {
                for (int y = 0; y < Rightrect.Height; y++)
                {
                    Color pixel1 = Rightrect.Bitmap.GetPixel(x, y);
                    Color pixel2 = Leftrect.Bitmap.GetPixel(x, y);


                }
            }
        }

        Image<Gray, byte> Rightrect = null;
        Image<Gray, byte> Leftrect = null;
        Image<Gray, byte> CenterRed = null;
        Image<Gray, byte> CenterYellow = null;
        Image<Gray, byte> CenterGreen = null;
        private void Imageprocess(object sender, EventArgs eventArgs)
        {


            capture.Read(frame);
            Image<Gray, byte> capturedimage = frame.ToImage<Gray, byte>();

            //Image<Gray, byte> gray_image = capturedimage.Convert<Gray, byte>();
            Image<Gray, byte> gauss = capturedimage.SmoothGaussian(3, 3, 34.3, 45.3);     //Applying gaussian blue


            Image<Gray, byte> cannyimage = new Image<Gray, byte>(capturedimage.Width, capturedimage.Height, new Gray(0));    //Setting up the parameters for the canny image


            Image<Gray, byte> Binary = gauss.ThresholdBinary(new Gray(110), new Gray(90));

            cannyimage = Binary.Canny(90, 20);                      //Applying canny image 

            try
            {
                Rightrect = cannyimage.Copy(GetRectangle(picbox.Width - (picbox.Width / 6) - 60, 100, 120, 200)).Resize(120, 200, Emgu.CV.CvEnum.Inter.Cubic);
                Rightrect.Save(string.Format(@"C:\games\Image\test.jpg"));
                Leftrect = cannyimage.Copy(GetRectangle((picbox.Width / 6) - 60, 100, 120, 200)).Resize(120, 200, Emgu.CV.CvEnum.Inter.Cubic);
                Leftrect.Save(string.Format(@"C:\games\Image\test1.jpg"));
                CenterRed = cannyimage.Copy(GetRectangle((picbox.Width / 2) - 37, 210, 75, 100)).Resize(75, 100, Emgu.CV.CvEnum.Inter.Cubic);
                CenterRed.Save(string.Format(@"C:\games\Image\test3.jpg"));
                CenterYellow = cannyimage.Copy(GetRectangle((picbox.Width / 2) - 75, 170, 150, 150)).Resize(150, 150, Emgu.CV.CvEnum.Inter.Cubic);
                CenterYellow.Save(string.Format(@"C:\games\Image\test4.jpg"));
                CenterGreen = cannyimage.Copy(GetRectangle((picbox.Width / 2) - 100, 100, 200, 225)).Resize(200, 225, Emgu.CV.CvEnum.Inter.Cubic);
                CenterGreen.Save(string.Format(@"C:\games\Image\test5.jpg"));
            }
            catch(Exception e)
            {

            }

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
        }

        private void drawrect(object sender, PaintEventArgs e)                                        //Drawing the rectangles 
        {
            e.Graphics.DrawRectangle(Pens.Red, GetRectangle((picbox.Width/2)-37,210,75,100));         //Center Rectangle       Picbox-Center-(Width of rect/2)
            e.Graphics.DrawRectangle(Pens.Yellow, GetRectangle((picbox.Width / 2) - 75, 170,150, 150));      //Center Rectangle   Picbox-Center-(Width of rect/2)
            e.Graphics.DrawRectangle(Pens.Green, GetRectangle((picbox.Width / 2) - 100, 100, 200, 225));
            e.Graphics.DrawRectangle(Pens.Blue, GetRectangle(picbox.Width-(picbox.Width /6)-60 , 100, 120, 200));                 //Side rectangle
            e.Graphics.DrawRectangle(Pens.Blue, GetRectangle((picbox.Width / 6)-60, 100, 120, 200));                 //Side rectangle

        }

        Rectangle quadrant;

        private Rectangle GetRectangle(int x, int y ,int w , int h)                                         //Setting the size and postion of rectangle selection
        {
            quadrant = new Rectangle();
            quadrant.X = x;
            quadrant.Y = y;
            quadrant.Width = w;
            quadrant.Height = h;

            return quadrant;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            capture.Stop();
        }
    }
}



