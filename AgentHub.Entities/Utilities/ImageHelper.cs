using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AgentHub.Entities.Utilities
{
    public static class ImageHelper
    {
        public static Bitmap CropImage(Image image, Rectangle cropRect)
        {
            var bitmap = new Bitmap(cropRect.Width, cropRect.Height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            var graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(image, 0, 0, cropRect, GraphicsUnit.Pixel);

            return bitmap;
        }

        public static Image LoadImage(string photoFileName)
        {
            Image imageResult;
            using (var fStream = new FileStream(photoFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] imgData = null;
                imgData = new byte[fStream.Length + 1];
                fStream.Read(imgData, 0, imgData.Length);
                fStream.Close();
                fStream.Dispose();
                imageResult = Image.FromStream(new MemoryStream(imgData));
            }
            GC.Collect();

            return imageResult;
        }

        public static Image ResizeImage(Image sourceImage, int targetWidth)
        {
            var targetHeight = (int)((Convert.ToSingle(targetWidth) / sourceImage.Width) * sourceImage.Height);

            var newImage = new Bitmap(targetWidth, targetHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourceImage, 0, 0, targetWidth, targetHeight);
            }

            return newImage;
        }

        public static byte[] ResizeAndCompressImage(Image sourceImage, int targetWidth)
        {
            var targetHeight = (int) ((Convert.ToSingle(targetWidth)/sourceImage.Width)*sourceImage.Height);

            var newImage = new Bitmap(targetWidth, targetHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourceImage, 0, 0, targetWidth, targetHeight);
            }

            using (var ms = new MemoryStream())
            {
                // Get the codec needed
                var imgCodec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);

                // Make a paramater to adjust quality
                var codecParams = new EncoderParameters(1);
                // Compress image using CompressionCCITT4
                codecParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);

                //save to the memorystream - convert it to an array and send it back as a byte[]
                newImage.Save(ms, imgCodec, codecParams);

                return ms.ToArray();
            }
        }

        public static void ResizeImage(string sourceFile, string targetFile, int width)
        {
            using (var sourceImage = LoadImage(sourceFile))
            {
                dynamic newImage = ResizeImage(sourceImage, width);
                //
                // Save to target path.
                newImage.Save(targetFile, sourceImage.RawFormat);
            }
        }

        public static Image CropImageForThumbnail(Image sourceImage, int targetWidth, int targetHeight)
        {
            var isHorImage = (sourceImage.Width > sourceImage.Height);
            Rectangle newImageRect;

            if (isHorImage)
            {
                targetWidth = (int) ((Convert.ToSingle(targetHeight) / sourceImage.Height) * sourceImage.Width);
                newImageRect = new Rectangle((targetWidth - targetWidth) / 2, 0, targetWidth, targetHeight);
            }
            else
            {
                targetHeight = (int) ((Convert.ToSingle(targetWidth) / sourceImage.Width) * sourceImage.Height);
                newImageRect = new Rectangle(0, (targetHeight - targetHeight) / 2, targetWidth, targetHeight);
            }

            sourceImage = ResizeImage(sourceImage, targetWidth);

            dynamic targetImage = new Bitmap(targetWidth, targetHeight);
            using (var grp = Graphics.FromImage(targetImage))
            {
                grp.DrawImage(sourceImage, newImageRect, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), GraphicsUnit.Pixel);

                sourceImage.Dispose();
            }

            return targetImage;
        }

        public static byte[] CropThumbnailImage(byte[] originalBytes, Size size, ImageFormat format)
        {
            var streamOriginal = new MemoryStream(originalBytes);
            var imgOriginal = Image.FromStream(streamOriginal);
            //get original width and height of the incoming image
            var originalWidth = imgOriginal.Width; // 1000
            var originalHeight = imgOriginal.Height; // 800

            //get the percentage difference in size of the dimension that will change the least
            var percWidth = (size.Width / (float)originalWidth); // 0.2
            var percHeight = (size.Height / (float)originalHeight); // 0.25
            var percentage = Math.Max(percHeight, percWidth); // 0.25

            //get the ideal width and height for the resize (to the next whole number)
            var width = (int)Math.Max(originalWidth * percentage, size.Width); // 250
            var height = (int)Math.Max(originalHeight * percentage, size.Height); // 200

            var croppedImage = CropImageForThumbnail(imgOriginal, width, height);
            using (var ms = new MemoryStream())
            {
                //get the codec needed
                var imgCodec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);

                //make a paramater to adjust quality
                var codecParams = new EncoderParameters(1);

                //reduce to quality of 80 (from range of 0 (max compression) to 100 (no compression))
                codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 65L);

                //save to the memorystream - convert it to an array and send it back as a byte[]
                croppedImage.Save(ms, imgCodec, codecParams);

                return ms.ToArray();
            }
        }

        public static byte[] GetCroppedImage(byte[] originalBytes, Size size, ImageFormat format)
        {
            using (var streamOriginal = new MemoryStream(originalBytes))
            {
                using (var imgOriginal = Image.FromStream(streamOriginal))
                {
                    //get original width and height of the incoming image
                    var originalWidth = imgOriginal.Width; // 1000
                    var originalHeight = imgOriginal.Height; // 800

                    //get the percentage difference in size of the dimension that will change the least
                    var percWidth = ((float) size.Width/(float) originalWidth); // 0.2
                    var percHeight = ((float) size.Height/(float) originalHeight); // 0.25
                    var percentage = Math.Max(percHeight, percWidth); // 0.25

                    //get the ideal width and height for the resize (to the next whole number)
                    var width = (int) Math.Max(originalWidth*percentage, size.Width); // 250
                    var height = (int) Math.Max(originalHeight*percentage, size.Height); // 200

                    //actually resize it
                    using (var resizedBmp = new Bitmap(width, height))
                    {
                        using (var graphics = Graphics.FromImage((Image) resizedBmp))
                        {
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.DrawImage(imgOriginal, 0, 0, width, height);
                        }

                        //work out the coordinates of the top left pixel for cropping
                        var x = (width - size.Width)/2; // 25
                        var y = (height - size.Height)/2; // 0

                        //create the cropping rectangle
                        var rectangle = new Rectangle(x, y, size.Width, size.Height); // 25, 0, 200, 200

                        //crop
                        using (var croppedBmp = resizedBmp.Clone(rectangle, resizedBmp.PixelFormat))
                        {
                            using (var ms = new MemoryStream())
                            {
                                //get the codec needed
                                var imgCodec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);

                                //make a paramater to adjust quality
                                var codecParams = new EncoderParameters(1);

                                //reduce to quality of 80 (from range of 0 (max compression) to 100 (no compression))
                                codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 80L);

                                //save to the memorystream - convert it to an array and send it back as a byte[]
                                croppedBmp.Save(ms, imgCodec, codecParams);
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
}
