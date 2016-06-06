using System;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Diplom
{
	class Method2
	{
		string path;
		string modelpath;
		string resultstring = "";
		public string ModelPath
		{
			set
			{
				modelpath = value;
			}
			get
			{
				return modelpath;
			}
		}
		public string ImagePath
		{
			set
			{
				//path = value;
				//Cv2.DestroyAllWindows();
				////Cv2.DestroyWindow("Результат первого метода");

				//Step 1. Получаем исходное изображение
				//Mat image = new Mat(path, ImreadModes.Unchanged);
				//Mat model = new Mat(modelpath);
				Mat model = new Mat("box.png");
				Mat image = new Mat("box_in_scene.png");
				Mat result = new Mat();
				//new Window("Модель", WindowMode.AutoSize, model);

				var modelDescriptors = new Mat();
				var imageDescriptors = new Mat();
				var matcher = new BFMatcher(NormTypes.Hamming);
				var akaze = AKAZE.Create();
				KeyPoint[] modelKeyPoints = null, imageKeyPoints = null;
				akaze.DetectAndCompute(model, null, out modelKeyPoints, modelDescriptors);
				akaze.DetectAndCompute(image, null, out imageKeyPoints, imageDescriptors);

				DMatch[][] matches = matcher.KnnMatch(modelDescriptors, imageDescriptors, 2);

				Mat mask = new Mat(matches.Length, 1, MatType.CV_8U);
				mask.SetTo(new Scalar(255));
				int nonZero = Cv2.CountNonZero(mask);

				VoteForUniqueness(matches, mask);
				nonZero = Cv2.CountNonZero(mask);
				nonZero = VoteForSizeAndOrientation(imageKeyPoints, modelKeyPoints, matches, mask, 1.5f, 20);


				//Cv2.FindHomography
				List<Point2f> obj = new List<Point2f>();
				List<Point2f> scene = new List<Point2f>();
				List<DMatch> goodMatchesList = new List<DMatch>();

				//iterate through the mask only pulling out nonzero items because they're matches
				for (int i = 0; i < mask.Rows; i++)
				{
					MatIndexer<byte> maskIndexer = mask.GetGenericIndexer<byte>();
					if (maskIndexer[i] > 0)
					{
						obj.Add(modelKeyPoints[matches[i][0].QueryIdx].Pt);
						scene.Add(imageKeyPoints[matches[i][0].TrainIdx].Pt);
						goodMatchesList.Add(matches[i][0]);
					}
				}

				List<Point2d> objPts = obj.ConvertAll(Point2fToPoint2d);
				List<Point2d> scenePts = scene.ConvertAll(Point2fToPoint2d);
				if (nonZero >= 4)
				{
					Mat homography = Cv2.FindHomography(objPts, scenePts, HomographyMethods.Ransac, 1.5, mask);
					nonZero = Cv2.CountNonZero(mask);

					if (homography != null)
					{
						Point2f[] objCorners = { new Point2f(0, 0),
									  new Point2f(model.Cols, 0),
									  new Point2f(model.Cols, model.Rows),
									  new Point2f(0, model.Rows) };

						Point2d[] sceneCorners = MyPerspectiveTransform3(objCorners, homography);

						Mat img3 = new Mat(Math.Max(model.Height, image.Height), image.Width + model.Width, MatType.CV_8UC3);
						Mat left = new Mat(img3, new Rect(0, 0, model.Width, model.Height));
						Mat right = new Mat(img3, new Rect(model.Width, 0, image.Width, image.Height));
						model.CopyTo(left);
						image.CopyTo(right);

						byte[] maskBytes = new byte[mask.Rows * mask.Cols];
						mask.GetArray(0, 0, maskBytes);
						Cv2.DrawMatches(model, modelKeyPoints, image, imageKeyPoints, goodMatchesList, img3, Scalar.All(-1), Scalar.All(-1), maskBytes, DrawMatchesFlags.NotDrawSinglePoints);

						List<List<Point>> listOfListOfPoint2D = new List<List<Point>>();
						List<Point> listOfPoint2D = new List<Point>();
						listOfPoint2D.Add(new Point(sceneCorners[0].X + model.Cols, sceneCorners[0].Y));
						listOfPoint2D.Add(new Point(sceneCorners[1].X + model.Cols, sceneCorners[1].Y));
						listOfPoint2D.Add(new Point(sceneCorners[2].X + model.Cols, sceneCorners[2].Y));
						listOfPoint2D.Add(new Point(sceneCorners[3].X + model.Cols, sceneCorners[3].Y));
						listOfListOfPoint2D.Add(listOfPoint2D);
						img3.Polylines(listOfListOfPoint2D, true, Scalar.Red, 4);

						//This works too
						//Cv2.Line(img3, scene_corners[0] + new Point2d(img1.Cols, 0), scene_corners[1] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);
						//Cv2.Line(img3, scene_corners[1] + new Point2d(img1.Cols, 0), scene_corners[2] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);
						//Cv2.Line(img3, scene_corners[2] + new Point2d(img1.Cols, 0), scene_corners[3] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);
						//		//Cv2.Line(img3, scene_corners[3] + new Point2d(img1.Cols, 0), scene_corners[0] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);

						//		img3.SaveImage("Kaze_Output.png");
								Window.ShowImages(img3);
					}
				}

				//Cv2.CvtColor(image, result, ColorConversionCodes.RGB2GRAY);
				//Cv2.CvtColor(model, model, ColorConversionCodes.RGB2GRAY);

				//var akaze = AKAZE.Create();
				//var modelDescriptors = new Mat();
				//var imageDescriptors = new Mat();
				//KeyPoint[] modelKeyPoints = null, imageKeyPoints = null;
				//akaze.DetectAndCompute(model, null, out modelKeyPoints, modelDescriptors);
				//akaze.DetectAndCompute(result, null, out imageKeyPoints, imageDescriptors);
				//akaze.DetectAndCompute(model, null, out modelKeyPoints, modelDescriptors);
				//var matcher = new BFMatcher(NormTypes.Hamming);
				//var matches = matcher.KnnMatch(modelDescriptors, imageDescriptors, 2);
				//var imgMatches = new Mat();
				//Cv2.DrawMatches(image, imageKeyPoints, model, modelKeyPoints, matches, imgMatches, Scalar.Red);
				//Cv2.ImShow("Matches", imgMatches);



				//var akaze = AKAZE.Create();

				//KeyPoint[] modelKeyPoints;
				//Mat modelDescriptor = new Mat();
				//akaze.DetectAndCompute(model, null, out modelKeyPoints, modelDescriptor);

				//KeyPoint[] imageKeyPoints;
				//Mat imageDescriptor = new Mat();
				//akaze.DetectAndCompute(result, null, out imageKeyPoints, imageDescriptor);


				//BFMatcher matcher = new BFMatcher(NormTypes.Hamming, true);
				//DMatch[] matches = matcher.Match(modelDescriptor, imageDescriptor, null);

				//double min_dist = double.MaxValue;
				//for (int i = 0; i < (int)matches.Length; i++)
				//{
				//	double dist = matches[i].Distance;
				//	if (dist < min_dist)
				//	{
				//		min_dist = dist;
				//	}
				//}

				//DMatch[] good_matches0 = new DMatch[(int)matches.Length];
				//int x = 1;
				//for (int i = 0; i < (int)matches.Length; i++)
				//{
				//	if (matches[i].Distance < 3.0 * min_dist)
				//	{
				//		good_matches0[x] = matches[i];
				//		x++;
				//	}
				//}

				//DMatch[] good_matches = new DMatch[x];

				//Array.Copy(good_matches0, good_matches, x);
				//Mat img_matches = new Mat();
				//Cv2.DrawMatches(model, modelKeyPoints, result, imageKeyPoints, good_matches, img_matches, Scalar.All(-1), Scalar.All(-1), null, DrawMatchesFlags.NotDrawSinglePoints);
				//Cv2.ImShow("test", img_matches);


			}

			get
			{
				return resultstring;
			}
		}


		private static void VoteForUniqueness(DMatch[][] matches, Mat mask, float uniqnessThreshold = 0.80f)
		{
			byte[] maskData = new byte[matches.Length];
			GCHandle maskHandle = GCHandle.Alloc(maskData, GCHandleType.Pinned);
			using (Mat m = new Mat(matches.Length, 1, MatType.CV_8U, maskHandle.AddrOfPinnedObject()))
			{
				mask.CopyTo(m);
				for (int i = 0; i < matches.Length; i++)
				{
					//This is also known as NNDR Nearest Neighbor Distance Ratio
					if ((matches[i][0].Distance / matches[i][1].Distance) <= uniqnessThreshold)
						maskData[i] = 255;
					else
						maskData[i] = 0;
				}
				m.CopyTo(mask);
			}
			maskHandle.Free();
		}

		static int VoteForSizeAndOrientation(KeyPoint[] modelKeyPoints, KeyPoint[] observedKeyPoints, DMatch[][] matches, Mat mask, float scaleIncrement, int rotationBins)
		{
			int idx = 0;
			int nonZeroCount = 0;
			byte[] maskMat = new byte[mask.Rows];
			GCHandle maskHandle = GCHandle.Alloc(maskMat, GCHandleType.Pinned);
			using (Mat m = new Mat(mask.Rows, 1, MatType.CV_8U, maskHandle.AddrOfPinnedObject()))
			{
				mask.CopyTo(m);
				List<float> logScale = new List<float>();
				List<float> rotations = new List<float>();
				double s, maxS, minS, r;
				maxS = -1.0e-10f; minS = 1.0e10f;

				//if you get an exception here, it's because you're passing in the model and observed keypoints backwards.  Just switch the order.
				for (int i = 0; i < maskMat.Length; i++)
				{
					if (maskMat[i] > 0)
					{
						KeyPoint observedKeyPoint = observedKeyPoints[i];
						KeyPoint modelKeyPoint = modelKeyPoints[matches[i][0].TrainIdx];
						s = Math.Log10(observedKeyPoint.Size / modelKeyPoint.Size);
						logScale.Add((float)s);
						maxS = s > maxS ? s : maxS;
						minS = s < minS ? s : minS;

						r = observedKeyPoint.Angle - modelKeyPoint.Angle;
						r = r < 0.0f ? r + 360.0f : r;
						rotations.Add((float)r);
					}
				}

				int scaleBinSize = (int)Math.Ceiling((maxS - minS) / Math.Log10(scaleIncrement));
				if (scaleBinSize < 2)
					scaleBinSize = 2;
				float[] scaleRanges = { (float)minS, (float)(minS + scaleBinSize + Math.Log10(scaleIncrement)) };

				using (MatOfFloat scalesMat = new MatOfFloat(rows: logScale.Count, cols: 1, data: logScale.ToArray()))
				using (MatOfFloat rotationsMat = new MatOfFloat(rows: rotations.Count, cols: 1, data: rotations.ToArray()))
				using (MatOfFloat flagsMat = new MatOfFloat(logScale.Count, 1))
				using (Mat hist = new Mat())
				{
					flagsMat.SetTo(new Scalar(0.0f));
					float[] flagsMatFloat1 = flagsMat.ToArray();

					int[] histSize = { scaleBinSize, rotationBins };
					float[] rotationRanges = { 0.0f, 360.0f };
					int[] channels = { 0, 1 };
					Rangef[] ranges = { new Rangef(scaleRanges[0], scaleRanges[1]), new Rangef(0.0f, 360.0f) };
					double minVal, maxVal;

					Mat[] arrs = { scalesMat, rotationsMat };
					Cv2.CalcHist(arrs, channels, null, hist, 2, histSize, ranges);
					Cv2.MinMaxLoc(hist, out minVal, out maxVal);

					Cv2.Threshold(hist, hist, maxVal * 0.5, 0, ThresholdTypes.Tozero);
					Cv2.CalcBackProject(arrs, channels, hist, flagsMat, ranges);

					MatIndexer<float> flagsMatIndexer = flagsMat.GetIndexer();

					for (int i = 0; i < maskMat.Length; i++)
					{
						if (maskMat[i] > 0)
						{
							if (flagsMatIndexer[idx++] != 0.0f)
							{
								nonZeroCount++;
							}
							else
								maskMat[i] = 0;
						}
					}
					m.CopyTo(mask);
				}
			}
			maskHandle.Free();

			return nonZeroCount;
		}

		public static Point2d Point2fToPoint2d(Point2f pf)
		{
			return new Point2d(((int)pf.X), ((int)pf.Y));
		}

		// to avoid opencvsharp's bug
		static Point2d[] MyPerspectiveTransform1(Point2f[] yourData, Mat transformationMatrix)
		{
			using (Mat src = new Mat(yourData.Length, 1, MatType.CV_32FC2, yourData))
			using (Mat dst = new Mat())
			{
				Cv2.PerspectiveTransform(src, dst, transformationMatrix);
				Point2f[] dstArray = new Point2f[dst.Rows * dst.Cols];
				dst.GetArray(0, 0, dstArray);
				Point2d[] result = Array.ConvertAll(dstArray, Point2fToPoint2d);
				return result;
			}
		}

		// fixed FromArray behavior
		static Point2d[] MyPerspectiveTransform2(Point2f[] yourData, Mat transformationMatrix)
		{
			using (MatOfPoint2f s = MatOfPoint2f.FromArray(yourData))
			using (MatOfPoint2f d = new MatOfPoint2f())
			{
				Cv2.PerspectiveTransform(s, d, transformationMatrix);
				Point2f[] f = d.ToArray();
				return f.Select(Point2fToPoint2d).ToArray();
			}
		}

		// new API
		static Point2d[] MyPerspectiveTransform3(Point2f[] yourData, Mat transformationMatrix)
		{
			Point2f[] ret = Cv2.PerspectiveTransform(yourData, transformationMatrix);
			return ret.Select(Point2fToPoint2d).ToArray();
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
