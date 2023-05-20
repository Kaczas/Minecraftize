using Hazdryx.Drawing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Minecraftize {
  class Minecraftizer {

    public int Square { get; set; }

    public Minecraftizer() {

    }

    public Bitmap Minecraftize(Bitmap sourceBitmap, int squareSize) {

      var timer = System.Diagnostics.Stopwatch.StartNew();

      List<Color> averageColors = new List<Color>();

      int horizontalSquaresCount = sourceBitmap.Width / squareSize;
      int verticalSquaresCount = sourceBitmap.Height / squareSize;

      var fastSourceBitmap = new FastBitmap(sourceBitmap);

      var icons = new ConcurrentDictionary<Point, FastBitmap>();
      var iconsCache = new ConcurrentDictionary<int, Image>();

      Parallel.For(0, verticalSquaresCount, (y) => {

        for (int x = 0; x < horizontalSquaresCount; x++) {

          var squareBitmap = new FastBitmap(squareSize, squareSize);

          fastSourceBitmap.CopyTo(squareBitmap, x * squareSize, y * squareSize, squareSize, squareSize);

          var color = ColorManager.GetAverageColor(squareBitmap);

          int iconIndex = ColorManager.FindClosestColorIndex(color);

          Image img;
          Bitmap icon;

          if (!iconsCache.ContainsKey(iconIndex)) {
            img = Image.FromFile(ColorManager.Icons[iconIndex]);
            iconsCache.TryAdd(iconIndex, img);
            icon = new Bitmap(img, new Size(squareSize, squareSize));
          }
          else {
            img = iconsCache[iconIndex];
            lock (img) { icon = new Bitmap(img, new Size(squareSize, squareSize)); }
          }

          icons.TryAdd(new Point(x * squareSize, y  * squareSize), new FastBitmap(icon));

        }

      });


      var finalImage = new FastBitmap(horizontalSquaresCount * squareSize, verticalSquaresCount * squareSize);

      foreach (var kvp in icons) {
        kvp.Value.CopyTo(finalImage, kvp.Key.X, kvp.Key.Y, 0, 0, squareSize, squareSize);
      }

      timer.Stop();

#if DEBUG
      System.Windows.MessageBox.Show("Time: " + timer.ElapsedMilliseconds / 1000d + " sec");
#endif

      return finalImage.BaseBitmap;

    }

  }
}
