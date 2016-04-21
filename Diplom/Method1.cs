﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Diplom
{
	class Method1
	{
		string path;
		string resultstring="";
		public string ImagePath
		{
			set
			{
				path = value;
				Cv2.DestroyAllWindows();
				//Cv2.DestroyWindow("Результат первого метода");

				//Step ё. Получаем исходное изображение
				Mat image = new Mat(path, ImreadModes.Unchanged);
				Mat res = new Mat();
				Mat result = new Mat();

				//Step 2. Обрезаем нерабочую область кадра
				Rect part = new Rect(0,0,1100,800);
				res = image.Clone(part);
				result = res.Clone();
				//Step 3. Размываем изображение
				//Cv2.MedianBlur(result, result, 3);
				//Cv2.GaussianBlur(result, result, new Size(5, 5), 0);
				

				//Step 3. Обрезаем нерабочую область кадра
				Cv2.CvtColor(res, res, ColorConversionCodes.RGB2GRAY);
//				Cv2.MedianBlur(res, result, 5);
								Cv2.BilateralFilter(res, result, 3, 50, 100);

				//				Cv2.GaussianBlur(result, result, new Size(5, 5), 0);
				//				Cv2.AddWeighted(result, 1.5, result, -0.5, 0, result);

				//Step 4. Применяем фильтр Канни
				Cv2.Canny(result, result, 75, 200, 3);
				new Window("Канни", result);
				//				Cv2.ImWrite("result12.jpg", result);

				//Step 5. Ищем прямоугольники
				FindContours(ref result, ref image, ref resultstring);
				new Window("Результат первого метода", image);
			}

			get
			{
				return resultstring;
			}
		}


		static void FindContours(ref Mat result, ref Mat image, ref string resultstring)
		{
			Point[][] contours;
			HierarchyIndex[] hierarchyIndexes;
			Cv2.FindContours(result, out contours, out hierarchyIndexes, RetrievalModes.List, ContourApproximationModes.ApproxNone);

			RotatedRect[] rectangles = new RotatedRect[contours.Length];

			Point2f[] centerOfBigRectangle = new Point2f[contours.Length];
			Point2f[] centerOfSmallRectangle = new Point2f[contours.Length];
			int bigRectangles = 0, smallRectangles = 0;
			for (int i = 0; i < contours.Length; i++)
			{
				rectangles[i] = Cv2.MinAreaRect(contours[i]);
				int boxLength = (int) Math.Max(rectangles[i].Size.Height, rectangles[i].Size.Width);
				int boxWidth = (int) Math.Min(rectangles[i].Size.Height, rectangles[i].Size.Width);
				
				//Ищем контур NXT блока
				if (boxLength > 30 && boxLength < 45 && boxWidth > 25 && boxWidth < 40)
				{
					centerOfBigRectangle[bigRectangles] = rectangles[i].Center;
//					Cv2.Circle(image, centerOfBigRectangle[bigRectangles], 10, Scalar.Green, 1);
					bigRectangles++;
				}

				//Ищем контур экрана NXT блока
				if (boxLength > 5 && boxLength < 20 && boxWidth > 5 && boxWidth < 15)
				{
					centerOfSmallRectangle[smallRectangles] = rectangles[i].Center;
//					Cv2.Circle(image, centerOfSmallRectangle[smallRectangles], 5, Scalar.Yellow, 1);
					smallRectangles++;
				}

			}

			int numOfRobots = 0;
			double prevxBigRectangle = 0;
			double prevyBigRectangle = 0;

			//Определяем правильность и угол
			for (int i=0; i<bigRectangles; i++)
			{


				for (int j=0; j<smallRectangles; j++)
				{
					//Проверяем расстояние между центрами прямоугольников
					double xDistance = centerOfBigRectangle[i].X - centerOfSmallRectangle[j].X;
					double yDistance = centerOfBigRectangle[i].Y - centerOfSmallRectangle[j].Y;
					double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
					double angle;
					if ( distance < 10)
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

//						Cv2.Line(image, (int) centerOfBigRectangle[i].X, (int) centerOfBigRectangle[i].Y, (int) centerOfSmallRectangle[j].X, (int) centerOfSmallRectangle[j].Y, Scalar.Red, 2);
//						Scalar color = Scalar.FromRgb(50 * numOfRobots, 50 * numOfRobots, 50 * numOfRobots);
//						Cv2.Circle(image, centerOfBigRectangle[bigRectangles], 10, color, 2);

						int thickness = 2;
						int robotLength = 150;
						int robotWeigth = 90;
						int fromNXTCentertoForward = 95;

						//Определяем угловые точки прямоугольника, описывающего робот
						Point2f[] corners = new Point2f[4];
						corners[0].X = (float) (centerOfBigRectangle[i].X - fromNXTCentertoForward * Math.Sin(angle) + robotWeigth * Math.Cos(angle)/2);
						corners[0].Y = (float) (centerOfBigRectangle[i].Y - fromNXTCentertoForward * Math.Cos(angle) - robotWeigth * Math.Sin(angle)/2);
						corners[1].X = (float) (corners[0].X - robotWeigth * Math.Cos(angle));
						corners[1].Y = (float) (corners[0].Y + robotWeigth * Math.Sin(angle));
						corners[2].X = (float) (corners[1].X + robotLength * Math.Sin(angle));
						corners[2].Y = (float) (corners[1].Y + robotLength * Math.Cos(angle));
						corners[3].X = (float) (corners[2].X + robotWeigth * Math.Cos(angle));
						corners[3].Y = (float) (corners[2].Y - robotWeigth * Math.Sin(angle));

						//Находим центр робота
						Point2f centerOfRobot;
						centerOfRobot.X = (float) (centerOfBigRectangle[i].X - (robotLength - fromNXTCentertoForward)/4 * Math.Sin(angle));
						centerOfRobot.Y = (float) (centerOfBigRectangle[i].Y - (robotLength - fromNXTCentertoForward)/4 * Math.Cos(angle));
//						Cv2.Circle(image, centerOfRobot, 75, Scalar.Red);

						//Cv2.Circle(image, new Point(10, 10), 5, Scalar.Green);
						Cv2.Line(image, corners[0], corners[1], Scalar.Red, thickness);
						Cv2.Line(image, corners[1], corners[2], Scalar.Yellow, thickness);
						Cv2.Line(image, corners[2], corners[3], Scalar.Green, thickness);
						Cv2.Line(image, corners[3], corners[0], Scalar.Gold, thickness);

						//Формируем строку для вывода
						resultstring += String.Format("Координаты {0}-го робота: ({1:F0}; {2:F0})\r\nУгол поворота: {3:F2} рад ({4:F0} град)\r\n",
														numOfRobots+1, centerOfRobot.X, centerOfRobot.Y, angle, angle*180/Math.PI);
						numOfRobots++;
					}
				}
			}

			resultstring += String.Format("Обнаружено роботов:{0}\r\n", numOfRobots);

		}
	}
}