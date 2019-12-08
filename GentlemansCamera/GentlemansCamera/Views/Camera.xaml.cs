using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.IO;

namespace GentlemansCamera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Camera : ContentPage
    {
    public Camera()
        {
            InitializeComponent();
            OpenImageButton.IsVisible = true;
            ImplementMessage.IsVisible = false;
            EncryptionKey.IsVisible = false;
            EncodedMessage.IsVisible = false;
            // ImageSource.FromResource("template.png");
        }
        string filepath = "";
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (filepath != null || filepath != "")
            {
                SKBitmap bitmap = SKBitmap.Decode(filepath);
                if (bitmap != null)
                {
                    args.Surface.Canvas.DrawBitmap(bitmap, 10, 10); // = new SKCanvas(bitmap);
                    Console.WriteLine("Loading image: " + filepath);
                    OpenImageButton.IsVisible = false;
                    ImplementMessage.IsVisible = true;
                //    EncryptionKey.IsVisible = true; // no point in spending another hour working on something unrelated to xamarin/skiasharp
                    EncodedMessage.IsVisible = true;
                }
            }
        }

        async private void OpenImage(object sender, EventArgs args)
        {
            FileData photo = await CrossFilePicker.Current.PickFile();
            filepath = photo.FilePath;
            Console.WriteLine("Opening image: " + photo.FilePath);
        }

        private void Implement(object sender, EventArgs args)
        {
            SKBitmap replacement = SKBitmap.Decode(filepath);
            int m = 0;
            for (int y = 0; y < replacement.Height; y++)
            {
                for (int x = 0; x < replacement.Width; x++)
                {
                    SKColor newPixel = replacement.GetPixel(x, y);
                    if (m < EncodedMessage.Text.Length)
                    {
                        SKColor replacementPixel = new SKColor(newPixel.Red, newPixel.Blue, newPixel.Green, Convert.ToByte(255 - Convert.ToInt32(EncodedMessage.Text[m])));
                        replacement.SetPixel(x, y, replacementPixel);
                        m++;
                    }
                    else
                    {
                        replacement.SetPixel(x, y, newPixel);
                    }
                }
            }
            SKData bytes = SKImage.FromBitmap(replacement).Encode(SKEncodedImageFormat.Png, 100);
            System.IO.File.WriteAllBytes(Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "encoded.png"), replacement.Bytes);
            ImplementMessage.Text = "Message implemented!"; // not stored in gallery, stored in appdata folder. this is because android permissions SUCK
        }
    }
}