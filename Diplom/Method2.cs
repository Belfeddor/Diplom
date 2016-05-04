using System;
using OpenCvSharp;
using System.Xml.Serialization;
using System.IO;

namespace Diplom
{
	class Method2
	{
		string path;
		string resultstring = "";
		public string ImagePath
		{
			set
			{
				path = value;
				Cv2.DestroyAllWindows();
				//Cv2.DestroyWindow("Результат первого метода");

				//Step 1. Получаем исходное изображение
				Mat image = new Mat(path, ImreadModes.Unchanged);
//				Mat res = new Mat();
				Mat result = new Mat();

				//Step 2. Преобразуем изображение в оттенки серого
				Cv2.CvtColor(image, result, ColorConversionCodes.RGB2GRAY);
				Cv2.MedianBlur(result, result, 5);
				Cv2.Canny(result, result, 75, 200, 3);
				LineSegmentPoint[] lines;
				lines = Cv2.HoughLinesP(result, 1, Cv2.PI / 180, 50, 50, 10);
				//for(int i=0; i<lines.Length; i++)
				//{
				//	Vec4i v = lines[i];
				//	float rho = lines[i].Rho;
				//	float theta = lines[i].Theta;
				//	double a = Math.Cos(theta);
				//	double b = Math.Sin(theta);
				//	double x0 = a * rho;
				//	double y0 = b * rho;
				//	Point pt1 = new Point { X = (int)Math.Round(x0 + 1000 * (-b)), Y = (int)Math.Round(y0 + 1000 * (a)) };
				//	Point pt2 = new Point { X = (int)Math.Round(x0 - 1000 * (-b)), Y = (int)Math.Round(y0 - 1000 * (a)) };
				//	image.Line(pt1, pt2, Scalar.Red, 3, LineTypes.AntiAlias, 0);
				//}
				new Window("Hough_line_standard", WindowMode.AutoSize, image);






				////Step 3. Размываем изображение
				////				Cv2.MedianBlur(res, result, 5);
				//Cv2.BilateralFilter(res, result, 3, 50, 100);
				////Cv2.MedianBlur(result, result, 3);
				////Cv2.GaussianBlur(result, result, new Size(5, 5), 0);
				////				Cv2.GaussianBlur(result, result, new Size(5, 5), 0);
				////				Cv2.AddWeighted(result, 1.5, result, -0.5, 0, result);

				////Step 4. Применяем фильтр Канни
				//Cv2.Canny(result, result, 75, 200, 3);
				//new Window("Канни", result);
				////				Cv2.ImWrite("result12.jpg", result);

				////Step 5. Ищем прямоугольники
				//FindContours(ref result, ref image, ref resultstring);
				//new Window("Результат первого метода", image);
			}

			get
			{
				return resultstring;
			}
		}


		static void FindContours(ref Mat result, ref Mat image, ref string resultstring)
		{
			//Подгружаем размеры робота из соответствующего xml файла
			NXTProperties nxt = new NXTProperties();
			XmlSerializer xmlNXTSerializer = new XmlSerializer(typeof(NXTProperties));
			FileStream xmlNXT = new FileStream("NXT.xml", FileMode.Open);
			nxt = (NXTProperties)xmlNXTSerializer.Deserialize(xmlNXT);
			xmlNXT.Close();

			//resultstring += String.Format("{0}*{1}", robodrom.length, nxt.LCDlength);

			Point[][] contours;
			HierarchyIndex[] hierarchyIndexes;
			Cv2.FindContours(result, out contours, out hierarchyIndexes, RetrievalModes.Tree, ContourApproximationModes.ApproxNone);

			RotatedRect[] rectangles = new RotatedRect[contours.Length];

			Point2f[] centerOfBigRectangle = new Point2f[contours.Length];
			Point2f[] centerOfSmallRectangle = new Point2f[contours.Length];
			int bigRectangles = 0, smallRectangles = 0;
			for (int i = 0; i < contours.Length; i++)
			{
				rectangles[i] = Cv2.MinAreaRect(contours[i]);
				int boxLength = (int)Math.Max(rectangles[i].Size.Height, rectangles[i].Size.Width);
				int boxWidth = (int)Math.Min(rectangles[i].Size.Height, rectangles[i].Size.Width);

				//Ищем контур NXT блока
				if (boxLength > 30 && boxLength < 45 && boxWidth > 25 && boxWidth < 40)
				{
					centerOfBigRectangle[bigRectangles] = rectangles[i].Center;
					bigRectangles++;
				}

				//Ищем контур экрана NXT блока
				if (boxLength > 5 && boxLength < 20 && boxWidth > 5 && boxWidth < 15)
				{
					centerOfSmallRectangle[smallRectangles] = rectangles[i].Center;
					smallRectangles++;
				}

			}

			int numOfRobots = 0;
			double prevxBigRectangle = 0;
			double prevyBigRectangle = 0;

			//Определяем правильность и угол
			for (int i = 0; i < bigRectangles; i++)
			{


				for (int j = 0; j < smallRectangles; j++)
				{
					//Проверяем расстояние между центрами прямоугольников
					double xDistance = centerOfBigRectangle[i].X - centerOfSmallRectangle[j].X;
					double yDistance = centerOfBigRectangle[i].Y - centerOfSmallRectangle[j].Y;
					double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
					double angle;
					if (distance < 10)
					{
						//Избегаем повторного детектирования
						if (Math.Abs(prevxBigRectangle - centerOfBigRectangle[i].X) <= 5 && Math.Abs(prevyBigRectangle - centerOfBigRectangle[i].Y) <= 5)
						{
							continue;
						}
						prevxBigRectangle = centerOfBigRectangle[i].X;
						prevyBigRectangle = centerOfBigRectangle[i].Y;

						//Определяем углы и рисуем
						angle = Math.Atan2(xDistance, yDistance);

						int thickness = 2;
						int robotLength = 150;
						int robotWeigth = 90;
						int fromNXTCentertoForward = 95;

						//Определяем угловые точки прямоугольника, описывающего робот
						Point2f[] corners = new Point2f[4];
						corners[0].X = (float)(centerOfBigRectangle[i].X - fromNXTCentertoForward * Math.Sin(angle) + robotWeigth * Math.Cos(angle) / 2);
						corners[0].Y = (float)(centerOfBigRectangle[i].Y - fromNXTCentertoForward * Math.Cos(angle) - robotWeigth * Math.Sin(angle) / 2);
						corners[1].X = (float)(corners[0].X - robotWeigth * Math.Cos(angle));
						corners[1].Y = (float)(corners[0].Y + robotWeigth * Math.Sin(angle));
						corners[2].X = (float)(corners[1].X + robotLength * Math.Sin(angle));
						corners[2].Y = (float)(corners[1].Y + robotLength * Math.Cos(angle));
						corners[3].X = (float)(corners[2].X + robotWeigth * Math.Cos(angle));
						corners[3].Y = (float)(corners[2].Y - robotWeigth * Math.Sin(angle));

						//Находим центр робота
						Point2f centerOfRobot;
						centerOfRobot.X = (float)(centerOfBigRectangle[i].X - (robotLength - fromNXTCentertoForward) / 4 * Math.Sin(angle));
						centerOfRobot.Y = (float)(centerOfBigRectangle[i].Y - (robotLength - fromNXTCentertoForward) / 4 * Math.Cos(angle));
						//						Cv2.Circle(image, centerOfRobot, 75, Scalar.Red);

						//Cv2.Circle(image, new Point(10, 10), 5, Scalar.Green);
						Cv2.Line(image, corners[0], corners[1], Scalar.Red, thickness);
						Cv2.Line(image, corners[1], corners[2], Scalar.Yellow, thickness);
						Cv2.Line(image, corners[2], corners[3], Scalar.Green, thickness);
						Cv2.Line(image, corners[3], corners[0], Scalar.Gold, thickness);



						//Формируем строку для вывода
						resultstring += String.Format("Координаты {0}-го робота: ({1:F0}; {2:F0})[{7}] и ({5:F0}; {6:F0})[{8}]\r\nУгол поворота: {3:F2} рад ({4:F0} град)\r\n",
														numOfRobots + 1,
														//centerOfRobot.X, centerOfRobot.Y, 
														centerOfBigRectangle[i].X, centerOfBigRectangle[i].Y, angle, angle * 180 / Math.PI, centerOfSmallRectangle[j].X, centerOfSmallRectangle[j].Y, i, j);
						numOfRobots++;
					}
				}
			}

			resultstring += String.Format("Обнаружено роботов:{0}\r\n", numOfRobots);


			//resultstring += String.Format("{0}*{1}*{2}", robodrom.height, robodrom.length, robodrom.width);
		}
	}

}
