namespace CannyEdgeDetector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("��� ������� �� ������ ������ ����� ������� �����������", "C������", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Bitmap inputImage = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "image files (*.jpeg|*.jpeg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                inputImage = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = inputImage;
            }

            // �������������� ����������� � ������� ������
            Bitmap grayscaleImage = CannyEdgeDetection.ConvertToGrayscale(inputImage);

            // ���������� ������� ������ ��� �����������
            Bitmap blurredImage = CannyEdgeDetection.ApplyGaussianBlur(grayscaleImage);

            // ���������� ���������� �� ��� X � Y
            (Bitmap gradientMagnitude, Bitmap gradientDirection) = CannyEdgeDetection.ComputeGradients(blurredImage);

            // ���������� ��-������������ ����������
            Bitmap suppressedImage = CannyEdgeDetection.ApplyNonMaximumSuppression(gradientMagnitude, gradientDirection);

            // ���������� ����������� ��� ���������� ���������
            Bitmap edgesImage = CannyEdgeDetection.ApplyHysteresisThresholding(suppressedImage, lowThreshold: 30, highThreshold: 80);

            // ���������� ��������������� �����������
            Bitmap Canny = edgesImage;
            pictureBox2.Image = Canny;
        }
    }
}