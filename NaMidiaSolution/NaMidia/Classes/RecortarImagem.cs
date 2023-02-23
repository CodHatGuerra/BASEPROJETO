using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NaMidia.Classes
{
    public class RecortarImagem
    {
        #region [Propriedades]

        private MediaPlayer mediaPlayer = new MediaPlayer();

        #endregion

        #region [Metodos]

        public byte[] RecuperarImagemRecortada(byte[] imagem, int largura, int altura)
        {
            try
            {
                return RedimensionarImagem(imagem, largura, altura);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private byte[] ConvertBitmapFrameToArray(BitmapFrame bfResize)
        {
            using (MemoryStream msStream = new MemoryStream())
            {
                PngBitmapEncoder pbdDecoder = new PngBitmapEncoder();
                pbdDecoder.Frames.Add(bfResize);
                pbdDecoder.Save(msStream);
                return msStream.ToArray();
            }
        }

        private byte[] RedimensionarImagem(byte[] image, int largura, int altura)
        {
            using (System.Drawing.Image oldImage = System.Drawing.Image.FromStream(new MemoryStream(image)))
            {
                System.Drawing.Size newSize = CalcularDimensoes(oldImage.Size, largura, altura);
                using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    using (Graphics canvas = Graphics.FromImage(newImage))
                    {
                        canvas.SmoothingMode = SmoothingMode.AntiAlias;
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        canvas.DrawImage(oldImage, new Rectangle(new System.Drawing.Point(0, 0), newSize));
                        MemoryStream m = new MemoryStream();
                        newImage.Save(m, ImageFormat.Png);
                        return m.GetBuffer(); 
                        
                        //Se quiser cortar a imagem basta chamar o metodo abaixo
                        //this.RecortarImagemRedimensionada(m.GetBuffer(), new System.Drawing.Size(largura, altura), ImageFormat.Png);
                    }
                }
            }
        }

        private System.Drawing.Size CalcularDimensoes(System.Drawing.Size oldSize, int largura, int altura)
        {
            System.Drawing.Size newSize = new System.Drawing.Size();

            if (oldSize.Height > oldSize.Width)
            {
                newSize.Width = (int)(oldSize.Width * ((float)altura / (float)oldSize.Height));
                newSize.Height = altura;

                if (newSize.Width < largura)
                {
                    newSize.Width = largura;
                    newSize.Height = (int)(oldSize.Height * ((float)largura / (float)oldSize.Width)); ;
                }
            }
            else
            {
                newSize.Width = largura;
                newSize.Height = (int)(oldSize.Height * ((float)largura / (float)oldSize.Width));

                if (newSize.Height < altura)
                {
                    newSize.Width = (int)(oldSize.Width * ((float)altura / (float)oldSize.Height));
                    newSize.Height = altura;
                }
            }
            return newSize;
        }

        private byte[] RecortarImagemRedimensionada(byte[] originalBytes, System.Drawing.Size size, ImageFormat format)
        {
            using (var streamOriginal = new MemoryStream(originalBytes))
            using (var imgOriginal = System.Drawing.Image.FromStream(streamOriginal))
            {
                var originalWidth = imgOriginal.Width;
                var originalHeight = imgOriginal.Height;

                var percWidth = ((float)size.Width / (float)originalWidth);
                var percHeight = ((float)size.Height / (float)originalHeight);
                var percentage = Math.Max(percHeight, percWidth);

                var width = (int)Math.Max(originalWidth * percentage, size.Width);
                var height = (int)Math.Max(originalHeight * percentage, size.Height);

                using (var resizedBmp = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage((System.Drawing.Image)resizedBmp))
                    {
                        graphics.InterpolationMode = InterpolationMode.Default;
                        graphics.DrawImage(imgOriginal, 0, 0, width, height);
                    }

                    var x = (width - size.Width) / 2;
                    var y = (height - size.Height) / 2;

                    var rectangle = new Rectangle(x, y, size.Width, size.Height);

                    using (var croppedBmp = resizedBmp.Clone(rectangle, resizedBmp.PixelFormat))
                    using (var ms = new MemoryStream())
                    {
                        var imgCodec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);

                        var codecParams = new EncoderParameters(1);

                        codecParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);

                        croppedBmp.Save(ms, imgCodec, codecParams);
                        return ms.ToArray();
                    }
                }
            }
        }

        #endregion
    }
}
