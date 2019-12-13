using Emgu.CV;
using Emgu.CV.Structure;
using Face;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace emgu.cv.face.gui
{
    public partial class MainForm : Form
    {
        private string camera;
        private VideoCapture capture = null;
        private bool captureInProgress;
        private Emgu.CV.Image<Bgr, Byte> frame = null;
        private double threshold = 0.7;

        private Face.Detector detector = null;
        private Face.Aligner aligner = null;
        private Face.Recognizer recognizer = null;

        public MainForm()
        {
            InitializeComponent();

            CvInvoke.UseOpenCL = false;
            try
            {
                this.capture = new VideoCapture();
                this.capture.ImageGrabbed += ProcessFrame;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            //this.frame = new Emgu.CV.Image<Bgr, Byte>();
        }


        private void ProcessFrame(object sender, EventArgs arg)
        {
            if (this.capture != null && this.capture.Ptr != IntPtr.Zero)
            {
                // show frame in image box
                //this.capture.Retrieve(this.frame, 0);    
                this.frame = this.capture.QueryFrame().ToImage<Bgr, byte>();
                this.imageBox.Image = this.frame;

                // detect face
                Bitmap bmp = this.frame.ToBitmap();
                List<Rectangle> faces = this.detector.Detect(bmp);

                for(int i=0;i<faces.Count;i++)
                {
                    this.frame.Draw(faces[i], new Bgr(255,0,0), 1);
                }


                // align face
                List<PointF> points = this.aligner.Align(bmp, faces[0]);

                // recognition
                float similarity = 0;
                int identityIndex = this.recognizer.Identify(bmp, points, ref similarity);
                
                if(similarity > threshold)
                {
                    //CvInvoke.PutText(this.frame, "text", )
                }
                
            }
        }


        private void ReleaseData()
        {
            if(this.capture.IsOpened)
            {
                this.capture.Stop();
            }

            if (this.capture != null)
            {
                this.capture.Dispose();
            }

            if(this.frame != null)
            {
                this.frame.Dispose();
            }

            if(this.detector != null)
            {
                this.detector.Dispose();
            }

            if(this.aligner != null)
            {
                this.aligner.Dispose();
            }

            if(this.recognizer != null)
            {
                this.recognizer.Dispose();
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            this.detector = new Detector(@"models\fd_2_00.dat");
            this.aligner = new Aligner(@"models\pd_2_00_pts5.dat");
            this.recognizer = new Recognizer(@"models\fr_2_10.dat");


            // select a camera
            



            this.capture.Start();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {

        }
    }
}
