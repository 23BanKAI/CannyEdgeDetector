using System;
using System.Drawing;

class CannyEdgeDetection
{

    public static Bitmap ConvertToGrayscale(Bitmap inputImage)
    {
        // Создаем новое изображение с таким же размером, но в оттенках серого
        Bitmap grayscaleImage = new Bitmap(inputImage.Width, inputImage.Height);

        // Проходимся по каждому пикселю в исходном изображении
        for (int y = 0; y < inputImage.Height; y++)
        {
            for (int x = 0; x < inputImage.Width; x++)
            {
                // Получаем цветовое значение пикселя
                Color pixel = inputImage.GetPixel(x, y);

                // Вычисляем яркость пикселя по формуле (0.299 * R + 0.587 * G + 0.114 * B)
                int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

                // Создаем новый пиксель с оттенком серого, используя вычисленное значение
                Color grayPixel = Color.FromArgb(grayValue, grayValue, grayValue);

                // Устанавливаем новый пиксель в изображении в оттенках серого
                grayscaleImage.SetPixel(x, y, grayPixel);
            }
        }

        // Возвращаем новое изображение в оттенках серого
        return grayscaleImage;
    }

    public static Bitmap ApplyGaussianBlur(Bitmap inputImage)
    {
        // Определение ядра для фильтра Гаусса
        int[,] kernel = {
        { 1, 2, 1 },
        { 2, 4, 2 },
        { 1, 2, 1 }
        };

        int kernelSize = 3;
        int kernelSum = 16;

        // Создание нового изображения с таким же размером
        Bitmap blurredImage = new Bitmap(inputImage.Width, inputImage.Height);

        // Проход по каждому пикселю внутри границ изображения
        for (int y = 1; y < inputImage.Height - 1; y++)
        {
            for (int x = 1; x < inputImage.Width - 1; x++)
            {
                int redSum = 0;
                int greenSum = 0;
                int blueSum = 0;

                // Применение ядра фильтра Гаусса к окружению текущего пикселя
                for (int i = 0; i < kernelSize; i++)
                {
                    for (int j = 0; j < kernelSize; j++)
                    {
                        // Получение цветового значения соседнего пикселя
                        Color pixelColor = inputImage.GetPixel(x + i - 1, y + j - 1);

                        // Умножение цветового значения на соответствующий коэффициент ядра и суммирование
                        redSum += pixelColor.R * kernel[i, j];
                        greenSum += pixelColor.G * kernel[i, j];
                        blueSum += pixelColor.B * kernel[i, j];
                    }
                }

                // Вычисление окончательного цветового значения пикселя после применения фильтра
                int redValue = redSum / kernelSum;
                int greenValue = greenSum / kernelSum;
                int blueValue = blueSum / kernelSum;

                // Создание нового пикселя с окончательными цветовыми значениями
                Color blurredColor = Color.FromArgb(redValue, greenValue, blueValue);

                // Установка нового пикселя в изображении
                blurredImage.SetPixel(x, y, blurredColor);
            }
        }

        // Возвращение изображения с примененным размытием Гаусса
        return blurredImage;
    }

    public static (Bitmap gradientMagnitude, Bitmap gradientDirection) ComputeGradients(Bitmap inputImage)
    {
        // Создаем новые битмапы для хранения градиента по амплитуде и направлению
        Bitmap gradientMagnitude = new Bitmap(inputImage.Width, inputImage.Height);
        Bitmap gradientDirection = new Bitmap(inputImage.Width, inputImage.Height);

        // Создаем массивы для хранения градиентов по оси X и Y
        double[,] gx = new double[inputImage.Width, inputImage.Height];
        double[,] gy = new double[inputImage.Width, inputImage.Height];

        // Вычисление градиентов по оси X и Y
        for (int y = 1; y < inputImage.Height - 1; y++)
        {
            for (int x = 1; x < inputImage.Width - 1; x++)
            {
                double gxSum = 0;
                double gySum = 0;

                // Операторы Собеля для вычисления градиентов
                gxSum += inputImage.GetPixel(x - 1, y - 1).R * -1;
                gxSum += inputImage.GetPixel(x - 1, y).R * -2;
                gxSum += inputImage.GetPixel(x - 1, y + 1).R * -1;
                gxSum += inputImage.GetPixel(x + 1, y - 1).R * 1;
                gxSum += inputImage.GetPixel(x + 1, y).R * 2;
                gxSum += inputImage.GetPixel(x + 1, y + 1).R * 1;

                gySum += inputImage.GetPixel(x - 1, y - 1).R * -1;
                gySum += inputImage.GetPixel(x, y - 1).R * -2;
                gySum += inputImage.GetPixel(x + 1, y - 1).R * -1;
                gySum += inputImage.GetPixel(x - 1, y + 1).R * 1;
                gySum += inputImage.GetPixel(x, y + 1).R * 2;
                gySum += inputImage.GetPixel(x + 1, y + 1).R * 1;

                // Сохраняем значения градиентов
                gx[x, y] = gxSum;
                gy[x, y] = gySum;
            }
        }

        // Находим максимальное значение градиента
        double maxMagnitude = double.MinValue;

        // Вычисление амплитуды и направления градиента для каждого пикселя
        for (int y = 1; y < inputImage.Height - 1; y++)
        {
            for (int x = 1; x < inputImage.Width - 1; x++)
            {
                // Вычисление амплитуды градиента
                double magnitude = Math.Sqrt(gx[x, y] * gx[x, y] + gy[x, y] * gy[x, y]);
                maxMagnitude = Math.Max(maxMagnitude, magnitude);

                // Вычисление направления градиента
                double direction = Math.Atan2(gy[x, y], gx[x, y]) * 180 / Math.PI;
                direction += 180; // Приведение направления градиента к положительному значению
                direction %= 180; // Ограничение направления градиента в диапазоне 0-180 градусов
                                  // Округляем амплитуду и направление градиента до целых чисел
                int magnitudeInt = (int)Math.Round(magnitude);
                int directionInt = (int)Math.Round(direction);

                // Ограничиваем значения амплитуды и направления градиента в диапазоне от 0 до 255
                magnitudeInt = Math.Max(0, Math.Min(255, magnitudeInt));
                directionInt = Math.Max(0, Math.Min(255, directionInt));

                // Устанавливаем значения пикселей в изображениях градиента по амплитуде и направлению
                gradientMagnitude.SetPixel(x, y, Color.FromArgb(magnitudeInt, magnitudeInt, magnitudeInt));
                gradientDirection.SetPixel(x, y, Color.FromArgb(directionInt, directionInt, directionInt));
            }
        }

        // Возвращаем изображения градиента по амплитуде и направлению
        return (gradientMagnitude, gradientDirection);
    }

    public static Bitmap ApplyNonMaximumSuppression(Bitmap gradientMagnitude, Bitmap gradientDirection)
    {
        // Создание нового изображения с таким же размером, как и градиентная амплитуда
        Bitmap suppressedImage = new Bitmap(gradientMagnitude.Width, gradientMagnitude.Height);

        // Проход по каждому пикселю внутри границ изображения
        for (int y = 1; y < gradientMagnitude.Height - 1; y++)
        {
            for (int x = 1; x < gradientMagnitude.Width - 1; x++)
            {
                // Получение направления градиента текущего пикселя
                int direction = gradientDirection.GetPixel(x, y).R;

                // Инициализация переменных для хранения значений соседних пикселей
                int pixelA = 0;
                int pixelB = 0;

                // Определение соседних пикселей в зависимости от направления градиента текущего пикселя
                if ((direction >= -22.5 && direction < 22.5) || (direction >= 157.5 && direction <= 180) || (direction >= -180 && direction < -157.5))
                {
                    // Горизонтальное направление
                    pixelA = gradientMagnitude.GetPixel(x - 1, y).R;
                    pixelB = gradientMagnitude.GetPixel(x + 1, y).R;
                }
                else if ((direction >= 22.5 && direction < 67.5) || (direction >= -157.5 && direction < -112.5))
                {
                    // Диагональное направление 1
                    pixelA = gradientMagnitude.GetPixel(x - 1, y - 1).R;
                    pixelB = gradientMagnitude.GetPixel(x + 1, y + 1).R;
                }
                else if ((direction >= 67.5 && direction < 112.5) || (direction >= -112.5 && direction < -67.5))
                {
                    // Вертикальное направление
                    pixelA = gradientMagnitude.GetPixel(x, y - 1).R;
                    pixelB = gradientMagnitude.GetPixel(x, y + 1).R;
                }
                else if ((direction >= 112.5 && direction < 157.5) || (direction >= -67.5 && direction < -22.5))
                {
                    // Диагональное направление 2
                    pixelA = gradientMagnitude.GetPixel(x - 1, y + 1).R;
                    pixelB = gradientMagnitude.GetPixel(x + 1, y - 1).R;
                }

                // Получение амплитуды градиента текущего пикселя
                int currentPixel = gradientMagnitude.GetPixel(x, y).R;

                // Подавление немаксимальных значений градиента
                if (currentPixel >= pixelA && currentPixel >= pixelB)
                {
                    // Установка текущего пикселя с его амплитудой в новом изображении
                    suppressedImage.SetPixel(x, y, Color.FromArgb(currentPixel, currentPixel, currentPixel));
                }
                else
                {
                    // Установка черного цвета для немаксимальных значений градиента
                    suppressedImage.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }
        }
        // Возвращение изображения с примененным подавлением немаксимальных значений
        return suppressedImage;
    }

    public static Bitmap ApplyHysteresisThresholding(Bitmap suppressedImage, int lowThreshold, int highThreshold)
    {
        // Создание нового изображения с таким же размером, как и подавленное изображение
        Bitmap edgesImage = new Bitmap(suppressedImage.Width, suppressedImage.Height);

        // Проход по каждому пикселю внутри подавленного изображения
        for (int y = 0; y < suppressedImage.Height; y++)
        {
            for (int x = 0; x < suppressedImage.Width; x++)
            {
                // Получение цвета пикселя и его интенсивности
                Color pixelColor = suppressedImage.GetPixel(x, y);
                int intensity = pixelColor.R;

                // Применение порогового значения для определения границы
                if (intensity >= highThreshold)
                {
                    edgesImage.SetPixel(x, y, Color.White); // Пиксель является граничным
                }
                else if (intensity < lowThreshold)
                {
                    edgesImage.SetPixel(x, y, Color.Black); // Пиксель не является граничным
                }
                else
                {
                    bool isStrongEdge = false;

                    // Проверка окружающих пикселей на наличие сильных граничных пикселей
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            // Проверка, находится ли окружающий пиксель внутри изображения
                            if (x + i >= 0 && x + i < suppressedImage.Width && y + j >= 0 && y + j < suppressedImage.Height)
                            {
                                Color surroundingPixelColor = suppressedImage.GetPixel(x + i, y + j);
                                int surroundingIntensity = surroundingPixelColor.R;

                                if (surroundingIntensity >= highThreshold)
                                {
                                    isStrongEdge = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (isStrongEdge)
                    {
                        edgesImage.SetPixel(x, y, Color.White); // Пиксель является граничным
                    }
                    else
                    {
                        edgesImage.SetPixel(x, y, Color.Black); // Пиксель не является граничным
                    }
                }
            }
        }

        // Возвращение изображения с примененным гистерезисным пороговым значением
        return edgesImage;
    }

}