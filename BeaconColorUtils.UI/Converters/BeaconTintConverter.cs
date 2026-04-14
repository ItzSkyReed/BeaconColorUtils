using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BeaconColorUtils.UI.Converters;

public class BeaconTintConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values is [Bitmap sourceBitmap, Color tintColor, ..] ? ApplyTint(sourceBitmap, tintColor) : null;
    }

    private static WriteableBitmap ApplyTint(Bitmap source, Color color)
    {
        var rMult = color.R / 255f;
        var gMult = color.G / 255f;
        var bMult = color.B / 255f;

        using var ms = new MemoryStream();
        source.Save(ms);
        ms.Position = 0;
        var writable = WriteableBitmap.Decode(ms);

        using var fb = writable.Lock();
        unsafe
        {
            var ptr = (byte*)fb.Address;
            var length = fb.Size.Height * fb.RowBytes;
            var isBgra = fb.Format == Avalonia.Platform.PixelFormat.Bgra8888;

            for (var i = 0; i < length; i += 4)
            {
                if (isBgra)
                {
                    ptr[i] = (byte)(ptr[i] * bMult);
                    ptr[i + 1] = (byte)(ptr[i + 1] * gMult);
                    ptr[i + 2] = (byte)(ptr[i + 2] * rMult);
                }
                else
                {
                    ptr[i] = (byte)(ptr[i] * rMult);
                    ptr[i + 1] = (byte)(ptr[i + 1] * gMult);
                    ptr[i + 2] = (byte)(ptr[i + 2] * bMult);
                }
            }
        }

        return writable;
    }
}