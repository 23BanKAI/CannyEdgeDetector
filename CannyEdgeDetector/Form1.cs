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
            MessageBox.Show("При нажатии на кнопку запуск нужно выбрать изображение", "Cправка", MessageBoxButtons.OK, MessageBoxIcon.Question);
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

            // Преобразование изображения в оттенки серого
            Bitmap grayscaleImage = CannyEdgeDetection.ConvertToGrayscale(inputImage);

            // Применение фильтра Гаусса для сглаживания
            Bitmap blurredImage = CannyEdgeDetection.ApplyGaussianBlur(grayscaleImage);

            // Вычисление градиентов по оси X и Y
            (Bitmap gradientMagnitude, Bitmap gradientDirection) = CannyEdgeDetection.ComputeGradients(blurredImage);

            // Применение не-максимумного подавления
            Bitmap suppressedImage = CannyEdgeDetection.ApplyNonMaximumSuppression(gradientMagnitude, gradientDirection);

            // Применение гистерезиса для порогового отсечения
            Bitmap edgesImage = CannyEdgeDetection.ApplyHysteresisThresholding(suppressedImage, lowThreshold: 30, highThreshold: 80);

            // Сохранение результирующего изображения
            Bitmap Canny = edgesImage;
            pictureBox2.Image = Canny;
        }
    }
}