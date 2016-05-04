using System;
using OpenCvSharp;
using System.Xml.Serialization;
using System.IO;

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

				//Step 1. Получаем исходное изображение
				Mat image = new Mat(path, ImreadModes.Unchanged);
				Mat result = new Mat();

				//Step 2. Преобразуем изображение в оттенки серого
				Cv2.CvtColor(image, result, ColorConversionCodes.RGB2GRAY);

				//Шаг 3. Применяем медианный фильтр
				int ksize = 5;
				if (image.Width <= 640)
				{
					ksize = 3;
				}
				Cv2.MedianBlur(result, result, ksize);

				//Шаг 4. Применяем фильтр Канни
				Cv2.Canny(result, result, 75, 200, 3);


				//Step 5. Ищем прямоугольники
				FindContours(ref result, ref image, ref resultstring);
				new Window("Результат первого метода", WindowMode.Normal, image);
			}

			get
			{
				return resultstring;
			}
		}

		static void FindContours(ref Mat result, ref Mat image, ref string resultstring)
		{
			//Подгружаем параметры робота из соответствующего xml файла
			NXTProperties nxt = new NXTProperties();
			XmlSerializer xmlNXTSerializer = new XmlSerializer(typeof(NXTProperties));
			FileStream xmlNXT = new FileStream("NXT.xml", FileMode.Open);
			nxt = (NXTProperties) xmlNXTSerializer.Deserialize(xmlNXT);
			xmlNXT.Close();

			Point[][] contours;
			HierarchyIndex[] hierarchyIndexes;
			Cv2.FindContours(result, out contours, out hierarchyIndexes, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
			RotatedRect[,] rectangles = new RotatedRect[contours.Length, 2];
			
			//Определяем соотношение площадей NXT блока и его экрана
			float squaresProportion = (nxt.NXTlength * nxt.NXTwidth) / (nxt.LCDlength*nxt.LCDwidth);
			int numOfRobots = 0;

			//В данный массив заносим номера вложенного прямоугольника и описывающего его прямоугольника
			int[,] childsAndParents = new int[contours.Length,2];

			/*Ищем вложенные прямоугольники (родитель родителя необходим, чтобы определить не внешнюю сторону внутреннего прямоугольника, 
			 *а внутреннюю сторону следующего по иерархии)
			*/
			for (int i=0, childnum=0; i < contours.Length; i++)
			{
				if ((hierarchyIndexes[i].Child == -1) && (hierarchyIndexes[i].Parent != -1))
				{
					//Определяем наличие прямоугольника, описанного вокруг вложенного  
					int parentnum = hierarchyIndexes[hierarchyIndexes[i].Parent].Parent;
					if ( parentnum != -1)
					{
						childsAndParents[childnum, 0] = i;
						childsAndParents[childnum, 1] = parentnum;
						childnum++;
					}
				}
			}

			//Нашли хотя бы один вложенный
			for(int i=0; i<childsAndParents.Length; i++)
			{
				int childnum = childsAndParents[i, 0];
				int parentnum = childsAndParents[i, 1];

				if (childnum>0)
				{
					rectangles[i, 0] = Cv2.MinAreaRect(contours[childnum]);
					rectangles[i, 1] = Cv2.MinAreaRect(contours[parentnum]);

					//Определяем коэффициент соотношения между размером робота и его снимком
					float coef = Math.Max(rectangles[i, 0].Size.Height, rectangles[i, 0].Size.Width) / nxt.LCDlength;
					
					//Проверяем соотношение площадей
					float rectanglesProportion = (rectangles[i, 1].Size.Height * rectangles[i, 1].Size.Width)
												/ (rectangles[i, 0].Size.Height * rectangles[i, 0].Size.Width);
										
					if (rectanglesProportion >= 0.5 * squaresProportion && rectanglesProportion <= 1.5 * squaresProportion)
					{
						double xDistance = rectangles[i, 0].Center.X - rectangles[i, 1].Center.X;
						double yDistance = rectangles[i, 1].Center.Y - rectangles[i, 0].Center.Y;
						
						//Определяем расстояние между центрами прямоугольников 
						double centersDistance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

						//Проверяем соответствие расстояние между центрами прямоугольника реальным
						double distance = coef * (nxt.NXTlength / 2 - nxt.fromFrontNXTtoLCD - nxt.LCDwidth / 2);
						if (centersDistance >= 0.5 * distance && centersDistance <= 1.5 * distance)
						{
							//Определяем угол
							double angle = Math.Atan2(yDistance, xDistance);

							//Определяем центр робота
							Point2f centerOfRobot;
							centerOfRobot.X = (float) (rectangles[i, 0].Center.X + (nxt.LCDwidth / 2 + nxt.fromFrontToNXT + nxt.fromFrontNXTtoLCD - nxt.length / 2) * coef * Math.Cos(angle));
							centerOfRobot.Y = (float) (rectangles[i, 0].Center.Y - (nxt.LCDlength / 2 + nxt.fromLeftToNXT + nxt.fromLeftNXTtoLCD - nxt.width / 2) * coef * Math.Sin(angle));
							
							//Определяем угловые точки прямоугольника, описывающего робот
							Point2f[] corners = new Point2f[4];
							corners[0].X = (float) (centerOfRobot.X + nxt.length / 2 * coef * Math.Cos(angle) + nxt.width / 2 * coef * Math.Sin(angle));
							corners[0].Y = (float) (centerOfRobot.Y - nxt.length / 2 * coef * Math.Sin(angle) + nxt.width / 2 * coef * Math.Cos(angle));
							
							corners[1].X = (float) (corners[0].X - nxt.length * coef * Math.Cos(angle));
							corners[1].Y = (float) (corners[0].Y + nxt.length * coef * Math.Sin(angle));
							corners[2].X = (float) (corners[1].X - nxt.width * coef * Math.Sin(angle));
							corners[2].Y = (float) (corners[1].Y - nxt.width * coef * Math.Cos(angle));
							corners[3].X = (float) (corners[2].X + nxt.length * coef * Math.Cos(angle));
							corners[3].Y = (float) (corners[2].Y - nxt.length * coef * Math.Sin(angle));

							//И рисуем его
							int thickness = 2;
							Cv2.Line(image, corners[0], corners[1], Scalar.Red, thickness);
							Cv2.Line(image, corners[1], corners[2], Scalar.Red, thickness);
							Cv2.Line(image, corners[2], corners[3], Scalar.Red, thickness);
							Cv2.Line(image, corners[3], corners[0], Scalar.Red, thickness);

							//Формируем строку для вывода
							resultstring += String.Format("Координаты {0}-го робота: ({1:F0}; {2:F0}) \r\nУгол поворота: {3:F2} рад ({4:F0} град)\r\n",
															numOfRobots + 1, rectangles[i, 0].Center.X, rectangles[i, 0].Center.Y, angle, angle * 180 / Math.PI);
							numOfRobots++;
						}						
					}
				}
				else
				{
					break;
				}
			}
			resultstring += String.Format("Обнаружено роботов:{0}\r\n", numOfRobots);
		}
	}
}