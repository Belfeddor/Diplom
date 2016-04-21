using OpenCvSharp;

namespace Diplom
{
	class SourceImage
	{
		private string path;
		public string ImagePath
		{
			set
			{
				path = value;
				Cv2.DestroyAllWindows();
//				Cv2.DestroyWindow("Исходный кадр");
				Mat srcImage = new Mat(path);
				new Window("Исходный кадр", srcImage);
			}
		}
	}
}
