using Hazdryx.Drawing;
using System.Drawing;

namespace Minecraftize {
  public class Minecraftizer {

    private readonly ColorManager _colorManager;

    public Minecraftizer(ColorManager colorManager) {
      _colorManager = colorManager;
    }

    public Bitmap Minecraftize(Bitmap sourceBitmap, int squareSize) {

      var timer = System.Diagnostics.Stopwatch.StartNew();

      int horizontalSquaresCount = sourceBitmap.Width / squareSize;
      int verticalSquaresCount = sourceBitmap.Height / squareSize;

      var fastSourceBitmap = new FastBitmap(sourceBitmap);

      var finalImage = new FastBitmap(sourceBitmap.Width, sourceBitmap.Height);

      for (int i = 0; i < verticalSquaresCount * horizontalSquaresCount; i++) {
        int y = (i / horizontalSquaresCount) * squareSize;
        int x = (i % horizontalSquaresCount) * squareSize;
        var squareBitmap = new FastBitmap(squareSize, squareSize);
        fastSourceBitmap.CopyTo(squareBitmap, x, y, squareSize, squareSize);
        var iconBitmap = new FastBitmap(_colorManager.GetBitmapFromAverageColor(squareBitmap));
        iconBitmap.CopyTo(finalImage, x, y, 0, 0, squareSize, squareSize);
      }

      timer.Stop();

#if DEBUG
      System.Windows.MessageBox.Show("Time: " + timer.ElapsedMilliseconds / 1000d + " sec");
#endif

      return finalImage.BaseBitmap;

    }

  }
}
