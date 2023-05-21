using Hazdryx.Drawing;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraftize {
  public class Minecraftizer {

    private readonly ColorManager _colorManager;

    private int _squareSize;
    private int _horizontalSquaresCount;
    private int _verticalSquaresCount;
    private FastBitmap _fastSourceBitmap;
    private FastBitmap _finalImage;

    public Minecraftizer(ColorManager colorManager) {

      _colorManager = colorManager;

    }

    public async Task<Bitmap> Minecraftize(Bitmap sourceBitmap, int squareSize) {

#if DEBUG
      var timer = System.Diagnostics.Stopwatch.StartNew();
#endif

      _squareSize = squareSize;
      _horizontalSquaresCount = sourceBitmap.Width / squareSize;
      _verticalSquaresCount = sourceBitmap.Height / squareSize;

      _fastSourceBitmap = new FastBitmap(sourceBitmap);
      _finalImage = new FastBitmap(sourceBitmap.Width, sourceBitmap.Height);

      await Parallel.ForEachAsync(
        Partitioner.Create(0, _verticalSquaresCount * _horizontalSquaresCount, 1_000_000).GetDynamicPartitions(),
        MinecraftizeAsync
        );

#if DEBUG
      timer.Stop();
      Debug.WriteLine($"Minecraftized image in {timer.Elapsed.TotalSeconds} seconds");
#endif

      return _finalImage.BaseBitmap;

    }

    private ValueTask MinecraftizeAsync(Tuple<int, int> range, CancellationToken ct) {

      for (int i = range.Item1; i < range.Item2; i++) {

        int x = (i % _horizontalSquaresCount) * _squareSize;
        int y = (i / _horizontalSquaresCount) * _squareSize;

        var squareBitmap = new FastBitmap(_squareSize, _squareSize);

        _fastSourceBitmap.CopyTo(squareBitmap, x, y, _squareSize, _squareSize);

        var avgBitmap = _colorManager.MinecraftizeBitmap(squareBitmap);

        squareBitmap.Dispose();

        var icon = new FastBitmap(avgBitmap);

        icon.CopyTo(_finalImage, x, y, 0, 0, _squareSize, _squareSize);

        icon.Dispose();

      }

      return ValueTask.CompletedTask;

    }

  }
}
