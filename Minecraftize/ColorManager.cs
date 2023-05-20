using Hazdryx.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Minecraftize {
  class ColorManager {

    private static List<Color> _colors;
    public static List<string> Icons;

    public static void CreateLists() {

      _colors = new List<Color>();
      Icons = new List<string>();
      string s = File.ReadAllText("AverageColorsImages.json");
      string[][] str = JsonSerializer.Deserialize<string[][]>(File.ReadAllText("AverageColorsImages.json"));
      for (int i = 0; i < str.Length; i++) {
        Icons.Add(str[i][0]);
        _colors.Add(Color.FromArgb(Convert.ToInt32(str[i][1]), Convert.ToInt32(str[i][2]), Convert.ToInt32(str[i][3])));
      }

    }

    public static Image FindClosestColorIcon(Color targetColor) {

      int index = FindClosestColorIndex(targetColor);
      string iconPath = Icons[index];
      var image = Image.FromFile(iconPath);
      return image;

    }

    public static int FindClosestColorIndex(Color targetColor) {
      Color closestColor = _colors[0];
      int closestDistance = CalculateColorDistance(targetColor, closestColor);

      foreach (Color color in _colors) {
        int distance = CalculateColorDistance(targetColor, color);
        if (distance < closestDistance) {
          closestColor = color;
          closestDistance = distance;
        }
      }
      return _colors.IndexOf(closestColor);
    }

    public static int CalculateColorDistance(Color color1, Color color2) {
      int rDiff = color1.R - color2.R;
      int gDiff = color1.G - color2.G;
      int bDiff = color1.B - color2.B;

      return (rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff);
    }

    public static Color GetAverageColor(FastBitmap bm) {

      int width = bm.Width;
      int height = bm.Height;

      long redSum = 0;
      long greenSum = 0;
      long blueSum = 0;

      var buffer = new int[width * height];

      bm.Read(buffer);

      foreach (var argb in buffer) {
        redSum += (argb & 0x00FF0000L) >> 16;
        greenSum += (argb & 0x0000FF00L) >> 8;
        blueSum += (argb & 0x000000FFL);
      }

      int totalPixels = width * height;

      byte redAverage = (byte)(redSum / totalPixels);
      byte greenAverage = (byte)(greenSum / totalPixels);
      byte blueAverage = (byte)(blueSum / totalPixels);

      Color c = Color.FromArgb(redAverage, greenAverage, blueAverage);
      return c;

    }

  }
}
