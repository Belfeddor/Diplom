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
				Mat srcImage = new Mat(path);
				new Window("Исходный кадр", srcImage);
			}
		}
	}
}
