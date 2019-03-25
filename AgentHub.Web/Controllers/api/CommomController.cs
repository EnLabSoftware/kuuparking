using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Service;
using AgentHub.Entities.Utilities;
using AgentHub.Web.Identity;

namespace AgentHub.Web.Controllers
{
    [ApplicationAuthorization]
    [RoutePrefix("api/Commom")]
    public class CommomController : BaseController
    {
        private readonly ICommonService _commonService;

        public CommomController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet]
        [Route("GetCountries")]
        public async Task<IHttpActionResult> GetCountries()
        {
            try
            {
                var countries = await _commonService.GetCountries();

                return Ok(countries);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }


        [HttpGet]
        [Route("GetCountriesAndStates")]
        public async Task<IHttpActionResult> GetCountriesAndStates()
        {
            try
            {
                var countries = await _commonService.GetCountries();
                var firstCountry = countries.FirstOrDefault();
                var countryId = firstCountry == null ? 1 : firstCountry.ID;
                var states = await _commonService.GetStates(countryId);

                return Ok(new { countries, states });
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("GetStates")]
        public async Task<IHttpActionResult> GetStates(int countryId)
        {
            try
            {
                var states = await _commonService.GetStates(countryId);

                return Ok(states);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("GetCities")]
        public async Task<IHttpActionResult> GetCities(int stateId)
        {
            try
            {
                var cities = await _commonService.GetCities(stateId);

                return Ok(cities);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("GetDistricts")]
        public async Task<IHttpActionResult> GetDistricts(int cityId)
        {
            try
            {
                var districts = await _commonService.GetDistricts(cityId);

                return Ok(districts);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("GetAddressInformations")]
        public async Task<IHttpActionResult> GetAddressInformations(string country, string state, string city, string district)
        {
            try
            {
                int? countryId = null, stateId = null, cityId = null;


                var countryResult = await _commonService.GetCountry(country);
                if (countryResult != null)
                    countryId = countryResult.ID;

                var stateResult = await _commonService.GetState(state, countryId);
                if (stateResult != null)
                    stateId = stateResult.ID;

                var cityResult = await _commonService.GetCity(city, stateId);
                if (cityResult != null)
                    cityId = cityResult.ID;

                var districtResult = await _commonService.GetDistrict(district, cityId);

                var cities = await _commonService.GetCities(stateResult != null ? stateResult.ID : 0);
                var districts = await _commonService.GetDistricts(cityResult != null ? cityResult.ID : 0);
                if (cityResult == null && districtResult != null && districtResult.CityId != null)
                {
                    districts = await _commonService.GetDistricts((int)districtResult.CityId);
                    cityResult = await _commonService.GetCity((int)districtResult.CityId);
                }

                return Ok(new { countryResult, stateResult, cityResult, districtResult, cities, districts });
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("UpdateAddress")]
        public IHttpActionResult UpdateAddress(Address address)
        {
            try
            {
                _commonService.UpdateAddress(address);

                return Ok();
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpPost]
        [Route("UploadProviderImages")]
        public IHttpActionResult UploadProviderImages(string imageFile1, string imageFile2, string imageFile3)
        {
            try
            {
                var request = HttpContext.Current.Request;
                // Save file on server with Guid ID
                var newFileName = new List<string>();
                var thumbnailFileNameRelativePath = string.Empty;

                for (var imageIndex = 0; imageIndex < request.Files.Count; imageIndex++)
                {
                    var file = request.Files["file" + imageIndex];
                    if (file == null)
                        return MissingFileBadRequest();

                    Image savedImage;
                    var imageFileNameRelativePath = SaveImageFile(file, ProviderImageFolder, out savedImage);
                    // Delete old images if existing
                    if (imageIndex == 0 && !string.IsNullOrEmpty(imageFile1))
                    {
                        DeleteImageFile(ProviderImageFolder + imageFile1);
                    }
                    else if (imageIndex == 1 && !string.IsNullOrEmpty(imageFile2))
                    {
                        DeleteImageFile(ProviderImageFolder + imageFile2);
                    }
                    else if (imageIndex == 2 && !string.IsNullOrEmpty(imageFile3))
                    {
                        DeleteImageFile(ProviderImageFolder + imageFile3);
                    }
                    if (imageIndex == 0) // Crop the first image to be the thumbnail image of the provider
                    {
                        thumbnailFileNameRelativePath = SaveThumbnailImage(file, imageFileNameRelativePath, ProviderImageFolder, ProviderThumbnailImageFolder);
                    }

                    newFileName.Add(imageFileNameRelativePath);
                }
                // Add the thumbnail image to the end of the list
                newFileName.Add(thumbnailFileNameRelativePath);

                return Ok(newFileName);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("UploadProfileImage")]
        public IHttpActionResult UploadProfileImage()
        {
            try
            {
                // Save file on server with Guid ID
                var file = HttpContext.Current.Request.Files["file0"];
                if (file == null)
                    return MissingFileBadRequest();

                Image savedImage;
                var newFileNameFullPath = SaveImageFile(file, ProfileImageFolder, out savedImage);

                return Ok(ProfileImageFolder + Path.GetFileName(newFileNameFullPath));
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("UploadBookingImage")]
        public IHttpActionResult UploadBookingImage()
        {
            try
            {
                var request = HttpContext.Current.Request;
                // Save file on server with Guid ID
                var postedFiles = request.Files;
                var file = postedFiles["file0"];
                if (file == null)
                    return MissingFileBadRequest();

                Image savedImage;
                var newFileNameFullPath = SaveImageFile(file, BookingImageFolder, out savedImage);

                return Ok(BookingImageFolder + Path.GetFileName(newFileNameFullPath));
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Uploads the image during registration.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadProviderTempImage")]
        public IHttpActionResult UploadProviderTempImage()
        {
            try
            {
                // Save file on server with Guid ID
                var file = HttpContext.Current.Request.Files["file0"];
                if (file == null)
                    return MissingFileBadRequest();
                
                Image savedImage;
                var newFileNameFullPath = SaveImageFile(file, ProviderTempImageFolder, out savedImage);

                return Ok(ProviderTempImageFolder + Path.GetFileName(newFileNameFullPath));
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Uploads single image of slot provider.
        /// </summary>
        /// <param name="oldImageFile">The old image file.</param>
        /// <param name="saveThumbnailImage">Save thumbnail image or not</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("UploadProviderImage")]
        public IHttpActionResult UploadProviderImage(string oldImageFile, bool saveThumbnailImage)
        {
            try
            {
                var newFileName = new List<string>();
                var file = HttpContext.Current.Request.Files["file0"];
                if (file == null)
                    return MissingFileBadRequest();

                // Delete old file
                if (!string.IsNullOrEmpty(oldImageFile))
                    DeleteImageFile(ProviderImageFolder + oldImageFile);

                Image savedImage;
                var newFileNameFullPath = SaveImageFile(file, ProviderImageFolder, out savedImage);
                newFileName.Add(ProviderImageFolder + Path.GetFileName(newFileNameFullPath));

                if (!saveThumbnailImage) 
                    return Ok(newFileName);

                // Delete old thumbnail file
                if (!string.IsNullOrEmpty(oldImageFile))
                    DeleteImageFile(ProviderThumbnailImageFolder + oldImageFile);

                var thumbnail = SaveThumbnailImage(file, newFileNameFullPath, ProviderImageFolder, ProviderThumbnailImageFolder);
                newFileName.Add(ProviderThumbnailImageFolder + Path.GetFileName(thumbnail));

                return Ok(newFileName);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
