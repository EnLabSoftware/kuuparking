using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using AgentHub.Entities.Utilities;

namespace AgentHub.Web.Controllers
{
    public class BaseController : ApiController
    {
        protected const string ProfileImageFolder = @"/ProviderImages/Profile/";
        protected const string ProviderImageFolder = @"/ProviderImages/";
        protected const string ProviderTempImageFolder = @"/ProviderImages/Temp/";
        protected const string ProviderThumbnailImageFolder = @"/ProviderImages/Thumbnail/";
        protected const string BookingImageFolder = @"/ProviderImages/Booking/";

        protected override ExceptionResult InternalServerError(Exception exception)
        {
            LogHelper.LogException(exception);
            //
            // To return full error message without stack trace to avoid security leak
            var fullErrorMessage = LogHelper.GetFullErrorMessage(exception);

            return base.InternalServerError(new Exception(fullErrorMessage));
        }

        protected BadRequestErrorMessageResult MissingFileBadRequest()
        {
            return BadRequest("Missing image file");
        }

        protected string GetModelStateError()
        {
            var errorMessage = "";

            foreach (var error in ModelState.Values.SelectMany(_ => _.Errors))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    errorMessage += Environment.NewLine;
                errorMessage += error.ErrorMessage;
            }

            return errorMessage;
        }

        protected string GetQueryString(IEnumerable<KeyValuePair<string, string>> queryStrings, string key)
        {
            var match = queryStrings.FirstOrDefault(keyValue => String.Compare(keyValue.Key, key, StringComparison.OrdinalIgnoreCase) == 0);

            return !string.IsNullOrEmpty(match.Value) ? match.Value : string.Empty;
        }

        protected string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => String.Compare(keyValue.Key, key, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return HttpUtility.UrlDecode(match.Value);
        }

        protected string SaveImageFile(HttpPostedFile file, string targetImageFolder, out Image savedImage)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileNameRelative = targetImageFolder + Guid.NewGuid() + extension;
            var newFileNamePhysicalPath = HttpContext.Current.Server.MapPath(fileNameRelative);
//#if DEBUG
//            var compressedImage = ImageHelper.ResizeAndCompressImage(Image.FromStream(file.InputStream), AppSettings.ProviderImageWidth);
//            File.WriteAllBytes(newFileNamePhysicalPath, compressedImage);
//            savedImage = null;
//#else
//            savedImage = ImageHelper.ResizeImage(Image.FromStream(file.InputStream), AppSettings.ProviderImageWidth);
//            savedImage.Save(newFileNamePhysicalPath);
//#endif
            var compressedImage = ImageHelper.ResizeAndCompressImage(Image.FromStream(file.InputStream), AppSettings.ProviderImageWidth);
            File.WriteAllBytes(newFileNamePhysicalPath, compressedImage);
            savedImage = null;

            return fileNameRelative;
        }

        protected string SaveImageFile(HttpPostedFile file, string targetImageFolder, out byte[] imageData)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileNameRelative = targetImageFolder + Guid.NewGuid() + extension;
            var newFileNamePhysicalPath = HttpContext.Current.Server.MapPath(fileNameRelative);
            // Save image
            imageData = new byte[file.InputStream.Length];
            file.InputStream.Read(imageData, 0, (int)file.InputStream.Length);
            var savedImage = ImageHelper.ResizeImage(Image.FromStream(file.InputStream), 800);
            savedImage.Save(newFileNamePhysicalPath);

            return fileNameRelative;
        }

        protected string SaveThumbnailImage(HttpPostedFile file, string imageFileName, string imageFolder, string thumbnailImageFolder)
        {
            var thumbnailFileNameRelativePath = thumbnailImageFolder + Path.GetFileName(imageFileName);
            try
            {
                var thumbnailFileNameFullPath = HttpContext.Current.Server.MapPath(thumbnailFileNameRelativePath);
                // Prepare thumbnail folder if not existing
                var thumbnailFolderFullPath = Path.GetDirectoryName(thumbnailFileNameFullPath);
                if (!string.IsNullOrEmpty(thumbnailFolderFullPath) && !Directory.Exists(thumbnailFolderFullPath))
                    Directory.CreateDirectory(thumbnailFolderFullPath);

                var thumbnailImage = ImageHelper.ResizeImage(Image.FromStream(file.InputStream), AppSettings.ProviderThumbnailImageWidth);
                thumbnailImage.Save(thumbnailFileNameFullPath);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }

            return thumbnailFileNameRelativePath;
        }

        protected string SaveThumbnailImage(byte[] imageData, string imageFileName, string imageFolder, string thumbnailImageFolder)
        {
            var thumbnailFileNameRelativePath = thumbnailImageFolder + Path.GetFileName(imageFileName);
            try
            {
                var thumbnailBytes = ImageHelper.GetCroppedImage(imageData, new Size(AppSettings.ProviderThumbnailImageWidth, AppSettings.ProviderThumbnailImageWidth), ImageFormat.Png);
                var thumbnailFileNameFullPath = HttpContext.Current.Server.MapPath(thumbnailFileNameRelativePath);
                // Prepare thumbnail folder if not existing
                var thumbnailFolderFullPath = Path.GetDirectoryName(thumbnailFileNameFullPath);
                if (!string.IsNullOrEmpty(thumbnailFolderFullPath) && !Directory.Exists(thumbnailFolderFullPath))
                    Directory.CreateDirectory(thumbnailFolderFullPath);

                File.WriteAllBytes(thumbnailFileNameFullPath, thumbnailBytes);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }

            return thumbnailFileNameRelativePath;
        }

        protected void DeleteImageFile(string imageFileName)
        {
            if (!string.IsNullOrEmpty(imageFileName))
            {
                var oldFilePath = HttpContext.Current.Server.MapPath(imageFileName);
                FileHelper.DeleteFile(oldFilePath);
            }
        }
    }
}
