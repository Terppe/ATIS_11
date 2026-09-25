using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Models;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace ATIS.DesignSystem.Converters;

public partial class ImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        //if (value == null)
        //{
        //    return new BitmapImage();
        //}

        //try
        //{
        //    var strmImg = new MemoryStream((byte[])value);
        //    var myBitmapImage = new BitmapImage();
        //    myBitmapImage.BeginInit();
        //    myBitmapImage.StreamSource = strmImg;
        //    myBitmapImage.EndInit();
        //    return myBitmapImage;
        //}
        //catch (Exception ex)
        //{
        //    var message = ex.Message;
        //}

        //return new BitmapImage();
        //-------------------------------------------
        //if (value == null || !(value is byte[]))
        //    return null;

        //using (var ms = new InMemoryRandomAccessStream())
        //{
        //    // Writes the image byte array in an InMemoryRandomAccessStream
        //    // that is needed to set the source of BitmapImage.
        //    using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
        //    {
        //        writer.WriteBytes((byte[])value);

        //        // The GetResults here forces to wait until the operation completes
        //        // (i.e., it is executed synchronously), so this call can block the UI.
        //        writer.StoreAsync().GetResults();
        //    }
        //    var image = new BitmapImage();
        //    image.SetSource(ms);

        //    return image;
        //}
        //-----------------------------------------------
        //if (value != null)
        //{

        //    var bytes = (byte[])value;
        //    var myBitmapImage = new BitmapImage();
        //    if (bytes.Any())
        //    {


        //        var stream = new InMemoryRandomAccessStream();
        //        var writer = new DataWriter(stream.GetOutputStreamAt(0));

        //        writer.WriteBytes(bytes);
        //        writer.StoreAsync().GetResults();
        //        myBitmapImage.SetSource(stream);
        //    }
        //    else
        //    {
        //        myBitmapImage.UriSource = new Uri("ms-appx:///Assets/Nilpferd.jpg");
        //    }

        //    return myBitmapImage;
        //}
        //else
        //{
        //    return new BitmapImage();
        //}
        //------------------------------------------
        //if (value == null || !(value is byte[]))
        //    return new BitmapImage();

        //try
        //{
        //    var bytes = (byte[])value;
        //    var myBitmapImage = new BitmapImage();
        //    if (bytes.Any())
        //    {
        //        var stream = new InMemoryRandomAccessStream();
        //        var writer = new DataWriter(stream.GetOutputStreamAt(0));

        //        writer.WriteBytes(bytes);
        //        writer.StoreAsync().GetResults();
        //        myBitmapImage.SetSource(stream);
        //        return myBitmapImage;
        //    }
        //    else
        //    {
        //        myBitmapImage.UriSource = new Uri("ms-appx:///Assets/Nilpferd.jpg");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    var message = ex.Message;
        //}

        //return new BitmapImage();
        //--------------------------------------
        //if (value != null && value is byte[])
        //{
        //    var randomAccessStream = new InMemoryRandomAccessStream();
        //    var writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
        //    writer.WriteBytes((byte[])value);
        //    var x = writer.StoreAsync().AsTask().Result;
        //    var image = new BitmapImage();
        //    image.SetSource(randomAccessStream);

        //    return image;
        //}

        //return null;
        //-------------------------------------
        //if (value != null && value is byte[])
        //{
        //    try
        //    {
        //        var randomAccessStream = new InMemoryRandomAccessStream();
        //        var writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
        //        writer.WriteBytes(value as byte[]);
        //        var x = writer.StoreAsync().AsTask().Result;
        //        var image = new BitmapImage();
        //        image.SetSource(randomAccessStream);

        //        return image;
        //    }
        //    catch (Exception e)
        //    {
        //        var message = e.Message;
        //    }
        //}
        //else
        //{
        //    BitmapImage myBitmapImage = null;
        //    myBitmapImage.UriSource = new Uri("ms-appx:///Assets/splash.jpg");
        //}

        //return null;
        //-----------------hier orginal---------------------
        if (value is not byte[] bytes)
            return new BitmapImage();
        try
        {
            using var ms = new InMemoryRandomAccessStream();
            using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(bytes);
                writer.StoreAsync().GetResults();
            }
            var image = new BitmapImage();
            image.SetSource(ms);
            return image;
        }
        catch (Exception e)
        {
            SimpleLog.Log(e);

            //  var message = e.Message;
        }
        return new BitmapImage();

        //-----------------------------------------------------------
        //-----------------------------------------------------------

        //if (value is not byte[])
        //    return null;
        //{
        //    var randomAccessStream = new InMemoryRandomAccessStream();
        //    var writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
        //    writer.WriteBytes((byte[])value);
        //    var x = writer.StoreAsync().AsTask().Result;
        //    var image = new BitmapImage();
        //    image.SetSource(randomAccessStream);

        //    return image;
        //}
        //--------------------------------------------
        //if (value is not byte[])
        //    return new BitmapImage();

        //try
        //{
        //    var randomAccessStream = new InMemoryRandomAccessStream();
        //    var writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
        //    writer.WriteBytes((byte[])value);
        //    var x = writer.StoreAsync().AsTask().Result;
        //    var image = new BitmapImage();
        //    image.SetSource(randomAccessStream);

        //    return image;
        //}
        //catch (Exception e)
        //{
        //    var message = e.Message;
        //}

        //return new BitmapImage();
    }


    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return null!;
    }
}
