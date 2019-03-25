using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AgentHub.Entities;
using AgentHub.Entities.Models;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Service;
using AgentHub.Entities.UnitOfWork;
using AgentHub.Entities.Utilities;
using AgentHub.Entities.ViewModels;
using AgentHub.Repository.Repositories;

namespace AgentHub.Service
{
    public class SlotProviderService: BaseService<SlotProvider>, ISlotProviderService
    {
        private readonly IRepositoryAsync<Address> _addressRepositoryAsync;
        private readonly IRepositoryAsync<Slot> _slotRepositoryAsync;
        private readonly IRepositoryAsync<SlotBooking> _slotBookingRepositoryAsync;
        private readonly IRepositoryAsync<SlotPayment> _slotPaymentRepositoryAsync;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        private AgentHubDataContext ParkingSlotContext
        {
            get
            {
                return Repository.Context as AgentHubDataContext;
            }
        }

        public SlotProviderService(IRepositoryAsync<SlotProvider> repository,
            IRepositoryAsync<Address> addressRepository,
            IRepositoryAsync<Slot> slotRepository,
            IRepositoryAsync<SlotBooking> slotBookingRepository,
            IRepositoryAsync<SlotPayment> slotPaymentRepository,
            IUnitOfWorkAsync unitOfWorkAsync) 
            : base(repository)
        {
            _addressRepositoryAsync = addressRepository;
            _slotRepositoryAsync = slotRepository;
            _slotBookingRepositoryAsync = slotBookingRepository;
            _slotPaymentRepositoryAsync = slotPaymentRepository;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        /// <summary>
        /// Gets all possible slot provider locations of the detected city from the provided geolocation address (latitude & longitude)
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        public Task<List<SlotProviderLocation>> GetProviderLocations(double latitude, double longitude)
        {
            var providerLocations = new List<SlotProviderLocation>();
            var webLocation = LocationHelper.GetLocation(latitude, longitude);
            if (webLocation != null && (!string.IsNullOrEmpty(webLocation.State) || !string.IsNullOrEmpty(webLocation.City)))
            {
                var streetsInCity = Queryable()
                        .Where(_ => (_.Address.State != null && _.Address.State.Name.Contains(webLocation.State)) || (_.Address.City != null && _.Address.City.Name.Contains(webLocation.City)) || (_.Address.District != null && _.Address.District.Name.Contains(webLocation.City)))
                        .Select(_ => new SlotProviderLocation()
                        {
                            AddressLine = _.Address.Street + ", " + _.Address.City.Name + ", " + _.Address.State.Name,
                            Street = _.Address.Street,
                            DistrictId = _.Address.DistrictId,
                            District = _.Address.District.Name,
                            CityId = _.Address.CityId,
                            City = _.Address.City.Name,
                            StateId = _.Address.City.StateId,
                            State = _.Address.City.State.Name
                        });
                // Add the selected city
                providerLocations.Add(new SlotProviderLocation() { City = webLocation.City });
                // Add all other streets in the given city
                providerLocations.AddRange(streetsInCity);
            }

            return Task.FromResult(providerLocations);
        }

        /// <summary>
        /// Gets all possible slot provider locations of the detected city from the provided IP address
        /// </summary>
        /// <param name="clientIp">The client ip.</param>
        /// <param name="localProviders">True if to find only provider locations with the detected city of the IP address. False to get all locations of the detected country</param>
        /// <returns></returns>
        public Task<List<SlotProviderLocation>> GetProviderLocationsByIP(string clientIp, bool localProviders)
        {
            var providerLocations = new List<SlotProviderLocation>();
            var webLocation = LocationHelper.GetCityByIP(clientIp);
            if (webLocation != null && !string.IsNullOrEmpty(webLocation.City))
            {
                if (localProviders)
                {
                    var streetsInCity = Queryable()
                        .Where(_ => _.Address.City.Name.Contains(webLocation.City))
                        .Select(_ => new SlotProviderLocation()
                        {
                            AddressLine = _.Address.Street + ", " + _.Address.City.Name + ", " + _.Address.State.Name,
                            CityId = _.Address.CityId,
                            City = _.Address.City.Name,
                            StateId = _.Address.City.StateId,
                            State = _.Address.City.State.Name
                        });
                    // Add the selected city
                    providerLocations.Add(new SlotProviderLocation() { City = webLocation.City });
                    // Add all other streets in the given city
                    providerLocations.AddRange(streetsInCity);
                }
                else
                {
                    var streetsInCountry =
                        Queryable()
                            .Where(_ => _.Address.City.State.Country.Name.Contains(webLocation.Country))
                            .Select(_ => new SlotProviderLocation()
                            {
                                AddressLine = _.Address.Street + ", " + _.Address.City.Name + ", " + _.Address.City.State.Name,
                                CityId = _.Address.CityId,
                                City = _.Address.City.Name,
                                StateId = _.Address.City.StateId,
                                State = _.Address.City.State.Name
                            });
                    // Add all cities
                    var cities = streetsInCountry.Select(item => new
                    {
                        item.CityId,
                        item.City
                    }).Distinct().ToList();
                    providerLocations.AddRange(cities.Select(_ => new SlotProviderLocation()
                    {
                        CityId = _.CityId,
                        City = _.City
                    }));

                    // Add all other streets
                    providerLocations.AddRange(streetsInCountry);
                }
            }

            return Task.FromResult(providerLocations);
        }

        /// <summary>
        /// Searches the slot providers based on given criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public Task<List<SlotProviderViewModel>> SearchSlotProviders(SlotSearchCriteriaViewModel criteria)
        {
            var command = SQLHelper.CreateCommand("park.SearchProviders", CommandType.StoredProcedure);

            command.Parameters.Add(SQLHelper.CreateInParameter("@latitude", criteria.Latitude, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@longitude", criteria.Longitude, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@radius", criteria.Radius, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@priceFrom", criteria.PriceFrom, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@priceTo", criteria.PriceTo, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@isOpen247", criteria.IsOpen247, SqlDbType.Bit));
            command.Parameters.Add(SQLHelper.CreateInParameter("@isWeekendAvailable", criteria.IsWeekendAvailable, SqlDbType.Bit));
            command.Parameters.Add(SQLHelper.CreateInParameter("@isCoveredParking", criteria.IsCoveredParking, SqlDbType.Bit));
            command.Parameters.Add(SQLHelper.CreateInParameter("@isOvernightParking", criteria.IsOvernightParking, SqlDbType.Bit));
            command.Parameters.Add(SQLHelper.CreateInParameter("@isBusParking", criteria.IsBusParking, SqlDbType.Bit));
            command.Parameters.Add(SQLHelper.CreateInParameter("@isCarWashingServiceAvailable", criteria.IsCarWashingServiceAvailable, SqlDbType.Bit));

            var resultTable = SQLHelper.QueryDataTable(command);
            var providers = resultTable.ToList<SlotProviderViewModel>();

            return Task.FromResult(providers);
        }

        public Task<List<SlotProviderViewModel>> SearchSlotProviders(double latitude, double longitude, double radius)
        {
            var command = SQLHelper.CreateCommand("park.SearchProviders", CommandType.StoredProcedure);

            command.Parameters.Add(SQLHelper.CreateInParameter("@latitude", latitude, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@longitude", longitude, SqlDbType.Decimal));
            command.Parameters.Add(SQLHelper.CreateInParameter("@radius", radius, SqlDbType.Decimal));

            var resultTable = SQLHelper.QueryDataTable(command);
            var providers = resultTable.ToList<SlotProviderViewModel>();

            return Task.FromResult(providers);
        }


        public SlotProvider GetSlotProvider(int slotProviderId)
        {
            return Queryable().Include(_ => _.Address).Include(_ => _.Slots).FirstOrDefault(_ => _.ID == slotProviderId);
        }

        public Task<Slot> AddSlot(Slot newSlot)
        {
            _slotRepositoryAsync.Insert(newSlot);
            _unitOfWorkAsync.SaveChanges();

            return Task.FromResult(newSlot);
        }

        public Task<bool> DeleteSlotProvider(int slotProviderId)
        {
            var provider = Repository.Find(slotProviderId);
            provider.IsDeleted = true;
            Repository.Update(provider);
            _unitOfWorkAsync.SaveChanges();

            return Task.FromResult(true);
        }

        public Task<SlotProvider> AddSlotProvider(SlotProvider slotProvider)
        {
            try
            {
                _unitOfWorkAsync.BeginTransaction();
                //
                // Save the adddress first
                _addressRepositoryAsync.Insert(slotProvider.Address);
                _unitOfWorkAsync.SaveChanges();
                slotProvider.AddressId = slotProvider.Address.ID;
                //
                // Keep the Slots
                var slots = slotProvider.Slots;
                slotProvider.Slots = null;
                //
                // Save the provider
                base.Insert(slotProvider);
                _unitOfWorkAsync.SaveChanges();
                //
                // Save the slots
                if (slots != null)
                {
                    for (var index = 0; index < slots.Count; index++)
                    {
                        var slot = slots[index];
                        slot.SlotProviderId = slotProvider.ID;
                        if (string.IsNullOrEmpty(slot.Name))
                            slot.Name = string.Format("{0} {1}", StringTable.Number, index + 1);

                        _slotRepositoryAsync.Insert(slot);
                    }
                }
                _unitOfWorkAsync.SaveChanges();
                //
                // Store Slots
                slotProvider.Slots = slots;
                //
                // Commit all changes at last
                _unitOfWorkAsync.Commit();
            }
            catch (Exception exception)
            {
                _unitOfWorkAsync.Rollback();

                throw; // Make sure to throw exception to upper caller thread for processing
            }

            return Task.FromResult(slotProvider);
        }

        public Task<SlotProvider> UpdateSlotProvider(SlotProvider slotProvider)
        {
            try
            {
                _unitOfWorkAsync.BeginTransaction();
                if (slotProvider.Address != null)
                    _addressRepositoryAsync.Update(slotProvider.Address);


                if (slotProvider.Slots != null)
                {
                    foreach (var slot in slotProvider.Slots)
                    {
                        _slotRepositoryAsync.Update(slot);
                    }
                }

                base.Update(slotProvider);

                _unitOfWorkAsync.SaveChanges();

                _unitOfWorkAsync.Commit();
            }
            catch (Exception exception)
            {
                _unitOfWorkAsync.Rollback();

                throw; // Make sure to throw exception to upper caller thread for processing
            }

            return Task.FromResult(slotProvider);
        }

        public Task<Slot> UpdateSlot(Slot slot)
        {
            _slotRepositoryAsync.Update(slot);
            _unitOfWorkAsync.SaveChanges();

            return Task.FromResult(slot);
        }

        /// <summary>
        /// Books any slot from a given slot provider.
        /// In case a specified slot (in SlotBooking.Slot object) is requested to book, need to check if that slot is available, if not, any other avaialble slot can be book.
        /// </summary>
        /// <param name="slotBooking">The slot booking.</param>
        /// <param name="fromSlotProviderId">From slot provider identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public Task<SlotBooking> BookSlot(SlotBooking slotBooking, int fromSlotProviderId)
        {
            try
            {
                _unitOfWorkAsync.BeginTransaction();
                //
                // First to check if the slot provider still has available slot to book or not. If not, return exception to inform slot not available.
                Slot availableSlot = null;
                var requestedSlotId = (slotBooking.Slot != null && slotBooking.Slot.ID > 0
                    ? slotBooking.Slot.ID : (slotBooking.SlotId > 0 ? slotBooking.SlotId : 0));

                if (requestedSlotId > 0)
                {
                    availableSlot =
                        _slotRepositoryAsync.Queryable()
                            .FirstOrDefault(slot => slot.ID == requestedSlotId && slot.IsAvailable);
                }
                else
                {
                    availableSlot =
                        _slotRepositoryAsync.Queryable()
                            .Where(slot => slot.SlotProviderId == fromSlotProviderId && slot.IsAvailable)
                            .OrderBy(slot => slot.ID)
                            .FirstOrDefault();
                }
                var slotProvider = Find(fromSlotProviderId);
                if (slotProvider == null)
                {
                    throw new ApplicationException(StringTable.ProviderIsNotFound);
                }
                //
                // Throw exception if requested slot is not available.
                if (availableSlot == null)
                {
                    throw new ApplicationException(string.Format(StringTable.NotAvaliableSlotFromProvider, slotProvider.Name));
                }
                slotBooking.Slot = availableSlot;
                slotBooking.SlotId = availableSlot.ID;
                slotBooking.TotalAmount = slotBooking.Price*slotBooking.Duration; // TotalAmount is calculated based on price and slot occupancy duration (1 hour by default)
                slotBooking.BookedByUserId = slotBooking.BookedByUserId;
                _slotBookingRepositoryAsync.Insert(slotBooking);
                //
                // Set status of slot to unavailable
                availableSlot.IsAvailable = false;
                _slotRepositoryAsync.Update(availableSlot);
                //
                // Update slot provider availability if all slots are booked
                slotProvider.IsAvailable = _slotRepositoryAsync.Queryable().Any(_ => _.SlotProviderId == fromSlotProviderId && _.IsAvailable);
                Update(slotProvider);
                
                _unitOfWorkAsync.SaveChanges();
                //
                // Commit all changes at last
                _unitOfWorkAsync.Commit();

                return Task.FromResult(slotBooking);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
                _unitOfWorkAsync.Rollback();

                throw;
            }
        }

        /// <summary>
        /// To reset the slot to be available again for booking
        /// </summary>
        /// <param name="slotBookingId">The slot identifier.</param>
        /// <returns></returns>
        public Task<bool> ReleaseSlot(int slotBookingId)
        {
            try
            {
                _unitOfWorkAsync.BeginTransaction();

                var slotBooking =
                    _slotBookingRepositoryAsync.Queryable()
                        .Where(_ => _.ID == slotBookingId)
                        .Select(_ => new {_.SlotId, _.TotalAmount})
                        .FirstOrDefault();
                if (slotBooking == null)
                    throw new ApplicationException(StringTable.SlotIsNotFound);

                //
                // Create the corresponding payment record
                var payment = new SlotPayment()
                {
                    PaidOn = DateTime.Now,
                    PayAmount = slotBooking.TotalAmount,
                    SlotBookingId = slotBookingId,
                    SlotId = slotBooking.SlotId
                };
                _slotPaymentRepositoryAsync.Insert(payment);
                //
                // Set slot to be available
                var slot = _slotRepositoryAsync.Find(slotBooking.SlotId);
                slot.IsAvailable = true;
                _slotRepositoryAsync.Update(slot);

                _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();

                return Task.FromResult(true);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
                _unitOfWorkAsync.Rollback();
                throw;
            }
        }

        /// <summary>
        /// To query available status of all slots of the specified slot provider.
        /// </summary>
        /// <param name="slotProviderId">The slot provider identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<List<SlotViewModel>> Slots(int slotProviderId)
        {
            var slots = (from slot in ParkingSlotContext.Slots
                join booking in ParkingSlotContext.SlotBookings on slot.ID equals booking.SlotId into bookingGroup
                from bookingInfo in bookingGroup.DefaultIfEmpty()
                         where slot.SlotProviderId == slotProviderId
                select
                    new SlotViewModel()
                    {
                        BookedOn = bookingInfo.BookedOn,
                        IsAvailable = slot.IsAvailable,
                        Description = slot.Description,
                        ID = slot.ID,
                        Name = slot.Name,
                        Price = slot.Price,
                        SlotProviderId = slot.SlotProviderId,
                        VehicleInfo = bookingInfo.VehicleInfo
                    }).ToList();

            return Task.FromResult(slots);
        }

        /// <summary>
        /// Gets all the slot providers of a given user
        /// </summary>
        /// <param name="userProfileId">The provider owwner id.</param>
        /// <returns></returns>
        public Task<List<SlotProvider>> GetSlotProviders(int userProfileId)
        {
            var providers = Queryable().Include(_ => _.Slots).Include(_ => _.Address).Where(provider => provider.SlotOwnerId == userProfileId && provider.IsDeleted == false).OrderBy(_=>_.Address.StateId).ThenBy(_=>_.Address.CityId).ToList();
            foreach (var slot in providers.SelectMany(slotProvider => slotProvider.Slots))
            {
                slot.ActiveBooking = _slotBookingRepositoryAsync.GetActiveBooking(slot.ID);
            }

            return Task.FromResult(providers);
        }

        public Task<List<SlotProvider>> GetInactiveSlotProviders()
        {
            var providers =
                Queryable()
                    .Include(_ => _.Slots)
                    .Include(_ => _.Address)
                    .Where(provider => provider.IsDeleted == false && provider.IsActivated == false)
                    .ToList();
            return Task.FromResult(providers);
        }

        public Task<bool> SetSlotStatus(int slotId, bool isAvailable)
        {
            var slot = _slotRepositoryAsync.Find(slotId);
            if (slot == null) 
                return Task.FromResult(false);

            slot.IsAvailable = isAvailable;
            _slotRepositoryAsync.Update(slot);

            _unitOfWorkAsync.SaveChanges();

            return Task.FromResult(true);
        }

        public Task<bool> DeleteSlot(int slotId)
        {
            try
            {
                _unitOfWorkAsync.BeginTransaction();

                // Check if slot is being used in slot booking yet
                var slotBeingUsed = _slotBookingRepositoryAsync.Queryable().Any(_ => _.SlotId == slotId);
                if (slotBeingUsed)
                {
                    throw new Exception(StringTable.SlotBeingUsed);
                }

                var slot = _slotRepositoryAsync.Find(slotId);
                if (slot == null)
                    throw new ApplicationException(StringTable.SlotIsNotFound);

                var slotProvider = base.Find(slot.SlotProviderId);
                if (slotProvider == null)
                    throw new ApplicationException(StringTable.ProviderIsNotFound);
                //
                // Delete the slot
                _slotRepositoryAsync.Delete(slot);
                _unitOfWorkAsync.SaveChanges();
                //
                // Update number of slots of the slot provider
                slotProvider.MaximumSlots = _slotRepositoryAsync.Queryable().Count(_ => _.SlotProviderId == slotProvider.ID);
                _unitOfWorkAsync.SaveChanges();

                _unitOfWorkAsync.Commit();

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                _unitOfWorkAsync.Rollback();
                throw;
            }
        }
        
        public Task<Slot> AddSlot(int slotProviderId)
        {
            var slotProvider = Queryable().Include(_ => _.Slots).FirstOrDefault(_=>_.ID == slotProviderId);
            if (slotProvider == null)
                throw new ApplicationException(StringTable.ProviderIsNotFound);

            if(slotProvider.Slots == null)
                slotProvider.Slots  = new List<Slot>();

            _unitOfWorkAsync.BeginTransaction();

            var slot = new Slot
            {
                IsAvailable = true,
                Price = slotProvider.Price,
                SlotProviderId = slotProviderId,
                Name = string.Format("{0} {1}", StringTable.Number, slotProvider.Slots.Count + 1)
            };
            _slotRepositoryAsync.Insert(slot);
            _unitOfWorkAsync.SaveChanges();

            slotProvider.Slots.Add(slot);
            slotProvider.MaximumSlots = slotProvider.Slots.Count;

            _unitOfWorkAsync.SaveChanges();
            _unitOfWorkAsync.Commit();

            return Task.FromResult(slot);
        }

        public Task<SlotBooking> GetSlotBooking(int slotBookingId)
        {
            return _slotBookingRepositoryAsync.FindAsync(slotBookingId);
        }

        public Task<bool> UpdateSlotBooking(SlotBooking slotBooking)
        {
            _slotBookingRepositoryAsync.Update(slotBooking);
            _unitOfWorkAsync.SaveChanges();

            return Task.FromResult(true);
        }

        public Task<List<SlotProvider>> GetSlotProviders(int? stateId, int? cityId)
        {
            if (stateId == null && cityId == null)
                return Task.FromResult<List<SlotProvider>>(null);

            var slotProviders = Queryable().Include(_=>_.Address);

            if (stateId != null)
                slotProviders = slotProviders.Where(_ => _.Address.StateId == stateId);

            if (cityId != null)
                slotProviders = slotProviders.Where(_ => _.Address.CityId == cityId);

            return Task.FromResult(slotProviders.Where(_=>!_.IsDeleted && _.IsActivated).ToList());
        }

        public Task<List<ProviderInState>> GetTotalProvidersInState()
        {
            var items = from s in ParkingSlotContext.States
                join a in ParkingSlotContext.Addresses on s.ID equals a.StateId
                join p in ParkingSlotContext.SlotProviders on a.ID equals p.AddressId
                where !p.IsDeleted && p.IsActivated
                group s by new {s.ID, s.Name}
                into grp
                select new ProviderInState
                {
                    StateID = grp.Key.ID,
                    Name = grp.Key.Name,
                    TotalSlotProviders = grp.Count()
                };

            return Task.FromResult(items.ToList());
        }
    }
}
