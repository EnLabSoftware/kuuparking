using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Http;
using System.Web.Http.Description;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.Service;
using System.Threading.Tasks;
using System.Web;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Utilities;
using AgentHub.Entities.ViewModels;
using AgentHub.Web.Identity;

namespace AgentHub.Web.Controllers
{
    [Authorize]
    [ApplicationAuthorization]
    [RoutePrefix("api/ParkingSlot")]
    public class ParkingSlotController : BaseController
    {
        private readonly ISlotProviderService _slotProviderService;
        private readonly ICommonService _commonService;

        public ParkingSlotController(ISlotProviderService slotService, ICommonService commonService)
        {
            _slotProviderService = slotService;
            _commonService = commonService;
        }

        /// <summary>
        /// Query slot provider locations by geolocation
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetProviderLocations")]
        [ResponseType(typeof(SlotProviderLocation))]
        public async Task<IHttpActionResult> GetProviderLocations(double latitude, double longitude)
        {
            try
            {
                var slotProviders = await _slotProviderService.GetProviderLocations(latitude, longitude);

                return Ok(slotProviders);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Query all available slots of given city, geographic location (longitude, latitude), check in/out date time, and price range.
        /// This method should be used to search for slot providers given the condition.
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("SearchProviders")]
        [ResponseType(typeof (SlotProviderViewModel))]
        public async Task<IHttpActionResult> SearchSlotProviders(SlotSearchCriteriaViewModel criteria)
        {
            try
            {
                // Take the search radius from application settings if it's set, else take what the client passes in
                if (AppSettings.SearchSlotsRadius > 0)
                    criteria.Radius = AppSettings.SearchSlotsRadius;

                var slotProviders = await _slotProviderService.SearchSlotProviders(criteria);

                return Ok(slotProviders);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Query a specified slot provider. When user selects a slot in the map, call this API to get the slot detail
        /// </summary>
        /// <param name="userProfileId">The slot provider owner.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSlotProviders")]
        [ResponseType(typeof(SlotProvider))]
        public async Task<IHttpActionResult> GetSlotProviders(int userProfileId)
        {
            try
            {
                var slotProvider = await _slotProviderService.GetSlotProviders(userProfileId);
                
                return Ok(slotProvider);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Get all inactive slot providers to review
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetInactiveSlotProviders")]
        [ResponseType(typeof(SlotProvider))]
        public async Task<IHttpActionResult> GetInactiveSlotProviders()
        {
            try
            {
                var slotProvider = await _slotProviderService.GetInactiveSlotProviders();

                return Ok(slotProvider);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Query a specified slot provider. When user selects a slot in the map, call this API to get the slot detail
        /// </summary>
        /// <param name="slotProviderId">The slot provider identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetSlotProvider")]
        [ResponseType(typeof (SlotProvider))]
        public IHttpActionResult GetSlotProvider(int slotProviderId)
        {
            try
            {
                var slotProvider = _slotProviderService.GetSlotProvider(slotProviderId);

                return Ok(slotProvider);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Add a slot provider.
        /// </summary>
        /// <param name="slotProvider">The slot provider to add.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddSlotProvider")]
        public async Task<IHttpActionResult> AddSlotProvider(SlotProvider slotProvider)
        {
            try
            {
                slotProvider = await _slotProviderService.AddSlotProvider(slotProvider);

                return Ok(slotProvider);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Updates the slot provider.
        /// </summary>
        /// <param name="slotProvider">The slot provider.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateSlotProvider")]
        public async Task<IHttpActionResult> UpdateSlotProvider(SlotProvider slotProvider)
        {
            try
            {
                if (slotProvider.ID <= 0)
                    slotProvider = await _slotProviderService.AddSlotProvider(slotProvider);
                else
                    slotProvider = await _slotProviderService.UpdateSlotProvider(slotProvider);

                return Ok(slotProvider);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Deletes the slot provider.
        /// </summary>
        /// <param name="slotProviderId">The slot provider.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteSlotProvider")]
        public async Task<IHttpActionResult> DeleteSlotProvider(int slotProviderId)
        {
            try
            {
                var isSuccessful = await _slotProviderService.DeleteSlotProvider(slotProviderId);

                return Ok(isSuccessful);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Books the slot. This is in case user 
        /// </summary>
        /// <param name="slotBookingViewModel">The slot booking view model.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("BookSlot")]
        public async Task<IHttpActionResult> BookSlot(SlotBookingViewModel slotBookingViewModel)
        {
            try
            {
                var slotBooking = new SlotBooking
                {
                    SlotId = slotBookingViewModel.SlotId,
                    BookedByUserId = slotBookingViewModel.BookedByUserId,
                    BookedOn = slotBookingViewModel.BookedOn,
                    VehicleImageFileName = slotBookingViewModel.VehicleImageFileName,
                    VehicleInfo = slotBookingViewModel.VehicleInfo,
                    Price = slotBookingViewModel.Price,
                    Duration = slotBookingViewModel.Duration,
                };

                slotBooking = await _slotProviderService.BookSlot(slotBooking, slotBookingViewModel.SlotProviderId);

                return Ok(slotBooking);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Unlock the slot when it is done using
        /// </summary>
        /// <param name="slotBookingId">The slot booking id that is to unlock.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReleaseSlot")]
        public async Task<IHttpActionResult> ReleaseSlot(int slotBookingId)
        {
            try
            {
                var isSuccessful = await _slotProviderService.ReleaseSlot(slotBookingId);

                return Ok(isSuccessful);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Delete old slot booking image and save a new one.
        /// </summary>
        /// <param name="slotBookingId">The slot booking identifier.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("UpdateBookingImage")]
        public async Task<IHttpActionResult> UpdateBookingImage(int slotBookingId)
        {
            try
            {
                var slotBooking = await _slotProviderService.GetSlotBooking(slotBookingId);
                if (slotBooking == null)
                    return InternalServerError(new Exception("SlotBooking is not found"));

                var request = HttpContext.Current.Request;
                var file = request.Files["file0"];
                if (file == null)
                    return BadRequest("Missing image file");

                const string bookingFolder = @"/ProviderImages/Booking/";
                Image savedImage;
                var imageFileNameFullPath = SaveImageFile(file, bookingFolder, out savedImage);
                //
                // Remove old file
                DeleteImageFile(slotBooking.VehicleImageFileName);

                // Update new file path
                slotBooking.VehicleImageFileName = bookingFolder + Path.GetFileName(imageFileNameFullPath);
                await _slotProviderService.UpdateSlotBooking(slotBooking);

                return Ok(imageFileNameFullPath);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("Slots")]
        public async Task<IHttpActionResult> Slots(int slotProviderId)
        {
            try
            {
                var isSuccessful = await _slotProviderService.Slots(slotProviderId);

                return Ok(isSuccessful);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("SetSlotStatus")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> SetSlotStatus(int slotId, bool isAvailable)
        {
            try
            {
                var isSuccessful = await _slotProviderService.SetSlotStatus(slotId, isAvailable);

                return Ok(isSuccessful);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("DeleteSlot")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> DeleteSlot(int slotId)
        {
            try
            {
                var isSuccessful = await _slotProviderService.DeleteSlot(slotId);

                return Ok(isSuccessful);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpPost]
        [Route("AddSlot")]
        [ResponseType(typeof(Slot))]
        public async Task<IHttpActionResult> AddSlot(int slotProviderId)
        {
            try
            {
                var slot = await _slotProviderService.AddSlot(slotProviderId);

                return Ok(slot);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetSlotProviders")]
        [ResponseType(typeof(SlotProvidersInPlace))]
        public async Task<IHttpActionResult> GetSlotProviders(int? stateId, int? cityId)
        {
            try
            {
                var model = new SlotProvidersInPlace
                {
                    SlotProviders = await _slotProviderService.GetSlotProviders(stateId, cityId)
                };

                if (stateId != null)
                    model.State = await _commonService.GetState(stateId.Value);
                else if (cityId != null)
                {
                    model.City = await _commonService.GetCity(cityId.Value);
                    model.State = model.City == null || model.City.State == null ? null : model.City.State;
                }

                if (model.State != null)
                    model.Cities = await _commonService.GetCities(model.State.ID);
                
                model.ProvidersInState = await _slotProviderService.GetTotalProvidersInState();

                return Ok(model);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetSlotProvidersInState")]
        [ResponseType(typeof(List<ProviderInState>))]
        public async Task<IHttpActionResult> GetSlotProvidersInState()
        {
            try
            {
                var providersInState = await _slotProviderService.GetTotalProvidersInState();

                return Ok(providersInState);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
