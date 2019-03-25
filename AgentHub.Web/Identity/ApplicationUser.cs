using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Security.Claims;
using System.Threading.Tasks;
using AgentHub.Entities;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.UnitOfWork;
using AgentHub.Entities.Utilities;
using AgentHub.Repository.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AgentHub.Web.Identity
{
    /// <summary>
    /// ApplicationUser class.
    /// </summary>
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [NotMapped]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [NotMapped]
        public string Password { get; set; }

        /// <summary>
        /// Date the user is created
        /// </summary>
        public virtual DateTime? CreatedDate { get; set; }
        /// <summary>
        /// Date the user record is modified
        /// </summary>
        public virtual DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// To indidate date the user is expired from subscription
        /// </summary>
        public virtual DateTime? ExpiredDate { get; set; }
        /// <summary>
        /// The international phone number includes country code, region code, and phone number that is ready for dial
        /// </summary>
        [MaxLength(20)]
        public virtual string InternationalPhoneNumber { get; set; }
        /// <summary>
        /// To indicate the associated user profile who owns this user account
        /// </summary>
        public int UserProfileId { get; set; }

        [MaxLength(50)]
        public string ExternalProviderName { get; set; }

        [DefaultValue(false)]
        public bool IsAdministrator { get; set; }

        [NotMapped]
        public UserProfile UserProfile { get; set; }

        #region User profile properties
        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string LastName { get; set; }
        [NotMapped]
        public UserProfileType UserProfileType { get; set; }
        [NotMapped]
        public ApplicationRole UserRole { get; set; }
        #endregion

        #region Slot provider properties
        [NotMapped]
        public string ProviderName { get; set; }
        [NotMapped]
        public string ProviderDescription { get; set; }
        [NotMapped]
        public double Latitude { get; set; }
        [NotMapped]
        public double Longitude { get; set; }
        [NotMapped]
        public int MaximumSlots { get; set; }
        [NotMapped]
        public decimal Price { get; set; }
        [NotMapped]
        public decimal Rating { get; set; }
        [NotMapped]
        public string ImageLocation1 { get; set; }
        [NotMapped]
        public string ImageLocation2 { get; set; }
        [NotMapped]
        public string ImageLocation3 { get; set; }
        [NotMapped]
        public string ThumbnailImageLocation { get; set; }
        [NotMapped]
        public DateTime AvailableFromTime { get; set; }
        [NotMapped]
        public DateTime AvailableToTime { get; set; }
        [NotMapped]
        public bool? IsOpen247 { get; set; }
        [NotMapped]
        public bool? IsWeekendAvailable { get; set; }
        [NotMapped]
        public bool? IsCoveredParking { get; set; }
        [NotMapped]
        public bool? IsOvernightParking { get; set; }
        [NotMapped]
        public bool? IsBusParking { get; set; }
        [NotMapped]
        public bool? IsCarWashingServiceAvailable { get; set; }
        [NotMapped]
        public bool? IsMondayAvailable { get; set; }
        [NotMapped]
        public int? MondayOpenTime { get; set; }
        [NotMapped]
        public int? MondayClosedTime { get; set; }
        [NotMapped]
        public bool? IsTuesdayAvailable { get; set; }
        [NotMapped]
        public int? TuesdayOpenTime { get; set; }
        [NotMapped]
        public int? TuesdayClosedTime { get; set; }
        [NotMapped]
        public bool? IsWednesdayAvailable { get; set; }
        [NotMapped]
        public int? WednedayOpenTime { get; set; }
        [NotMapped]
        public int? WednedayClosedTime { get; set; }
        [NotMapped]
        public bool? IsThursdayAvailable { get; set; }
        [NotMapped]
        public int? ThursdayOpenTime { get; set; }
        [NotMapped]
        public int? ThursdayClosedTime { get; set; }
        [NotMapped]
        public bool? IsFridayAvailable { get; set; }
        [NotMapped]
        public int? FridayOpenTime { get; set; }
        [NotMapped]
        public int? FridayClosedTime { get; set; }
        [NotMapped]
        public bool? IsSaturdayAvailable { get; set; }
        [NotMapped]
        public int? SaturdayOpenTime { get; set; }
        [NotMapped]
        public int? SaturdayClosedTime { get; set; }
        [NotMapped]
        public bool? IsSundayAvailable { get; set; }
        [NotMapped]
        public int? SundayOpenTime { get; set; }
        [NotMapped]
        public int? SundayClosedTime { get; set; }
        #endregion

        #region Address properties
        [NotMapped]
        public string StreetNumber { get; set; }
        [NotMapped]
        public string Street { get; set; }
        [NotMapped]
        public string ZipCode { get; set; }
        [NotMapped]
        public int? DistrictId { get; set; }
        [NotMapped]
        public int? CityId { get; set; }
        [NotMapped]
        public int? StateId { get; set; }
        [NotMapped]
        public int? CountryId { get; set; }
        #endregion

        #region Other not-mapped properties
        /// <summary>
        /// To store API access token in order to return to client when done registering
        /// </summary>
        [NotMapped]
        public string AccessToken { get; set; }
        [NotMapped]
        public bool HasLocalAccount { get; set; }
        [NotMapped]
        public string Provider { get; set; }
        [NotMapped]
        public string ExternalAccessToken { get; set; }

        #endregion

        /// <summary>
        /// Generates the user identity asynchronous.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>
        /// Claims identity
        /// </returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            // Note the authenticationType must match the one defined in
            // CookieAuthenticationOptions.AuthenticationType 
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here 
            return userIdentity;
        }
    }

    /// <summary>
    /// ApplicationUserStore class.
    /// </summary>
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ApplicationUserStore(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// To overwrite the Create new user login in order to create user profile (Users.Users table) when registering new account
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async override Task CreateAsync(ApplicationUser user)
        {
            await CreateUserUsingStoredProcedure(user);
        }

        private static async Task<string> CreateUserUsingStoredProcedure(ApplicationUser user)
        {
            try
            {
                var command = SQLHelper.CreateCommand("auth.RegisterUser", CommandType.StoredProcedure);
                //
                // User login info
                command.Parameters.Add(SQLHelper.CreateOutParameter("@login_id", SqlDbType.NVarChar, 128));

                command.Parameters.Add(SQLHelper.CreateInParameter("@login_CreatedDate", user.CreatedDate, SqlDbType.DateTime));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_ModifiedDate", user.ModifiedDate, SqlDbType.DateTime));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_ExpiredDate", user.ExpiredDate, SqlDbType.DateTime));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_InternationalPhoneNumber", user.InternationalPhoneNumber, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_Email", user.Email, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_EmailConfirmed", user.EmailConfirmed, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_PasswordHash", user.PasswordHash, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_SecurityStamp", user.SecurityStamp, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_PhoneNumber", user.PhoneNumber, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_PhoneNumberConfirmed", user.PhoneNumberConfirmed, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_TwoFactorEnabled", user.TwoFactorEnabled, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_LockoutEndDateUtc", user.LockoutEndDateUtc, SqlDbType.DateTime));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_LockoutEnabled", user.LockoutEnabled, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_AccessFailedCount", user.AccessFailedCount, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_UserName", user.UserName, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@login_ExternalProviderName", user.ExternalProviderName, SqlDbType.NVarChar));
                //
                // User profile info
                command.Parameters.Add(SQLHelper.CreateInParameter("@user_FirstName", user.FirstName, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@user_LastName", user.LastName, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@user_UserProfileType", (int)user.UserProfileType, SqlDbType.Int));
                //
                // Slot provider info
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ProviderName", user.ProviderName, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ProviderDescription", user.ProviderDescription, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_Latitude", user.Latitude, SqlDbType.Float));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_Longitude", user.Longitude, SqlDbType.Float));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_MaximumSlots", user.MaximumSlots, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_Price", user.Price, SqlDbType.Decimal));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ImageLocation1", user.ImageLocation1, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ImageLocation2", user.ImageLocation2, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ImageLocation3", user.ImageLocation3, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ThumbnailImageLocation", user.ThumbnailImageLocation, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsOpen247", user.IsOpen247, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsWeekendAvailable", user.IsWeekendAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsCoveredParking", user.IsCoveredParking, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsOvernightParking", user.IsOvernightParking, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsBusParking", user.IsBusParking, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsCarWashingServiceAvailable", user.IsCarWashingServiceAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsMondayAvailable", user.IsMondayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_MondayOpenTime", user.MondayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_MondayClosedTime", user.MondayClosedTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsTuesdayAvailable", user.IsTuesdayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_TuesdayOpenTime", user.TuesdayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_TuesdayClosedTime", user.TuesdayClosedTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsWednesdayAvailable", user.IsWednesdayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_WednedayOpenTime", user.WednedayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_WednedayClosedTime", user.WednedayClosedTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsThursdayAvailable", user.IsThursdayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ThursdayOpenTime", user.ThursdayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_ThursdayClosedTime", user.ThursdayClosedTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsFridayAvailable", user.IsFridayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_FridayOpenTime", user.FridayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_FridayClosedTime", user.FridayClosedTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsSaturdayAvailable", user.IsSaturdayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_SaturdayOpenTime", user.SaturdayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_SaturdayClosedTime", user.SaturdayClosedTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_IsSundayAvailable", user.IsSundayAvailable, SqlDbType.Bit));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_SundayOpenTime", user.SundayOpenTime, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@provider_SundayClosedTime", user.SundayClosedTime, SqlDbType.Int));
                //
                // Slot provider address info
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_StreetNumber", user.StreetNumber, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_Street", user.Street, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_ZipCode", user.ZipCode, SqlDbType.NVarChar));
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_DistrictId", user.DistrictId, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_CityId", user.CityId, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_StateId", user.StateId, SqlDbType.Int));
                command.Parameters.Add(SQLHelper.CreateInParameter("@address_CountryId", user.CountryId, SqlDbType.Int));

                await SQLHelper.ExecuteNonQuery(command);

                user.Id = command.Parameters[0].Value.ToString();
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }

            return user.Id;
        }

        #region "Register user profile and provider"
        private async Task<SlotProvider> CreateSlotProvider(int userProfileId, int billingAddressId, ApplicationUser user, IUnitOfWorkAsync unitOfWork)
        {
            if (string.IsNullOrEmpty(user.ProviderName))
                return null;

            var provider = new SlotProvider()
            {
                Name = user.ProviderName,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                MaximumSlots = user.MaximumSlots,
                Price = user.Price,
                ImageLocation1 = user.ImageLocation1,
                ImageLocation2 = user.ImageLocation2,
                ImageLocation3 = user.ImageLocation3,
                IsAvailable = true,
                SlotOwnerId = userProfileId,
                AddressId = billingAddressId
            };
            var providerRepository = ObjectFactory.GetInstance<IRepositoryAsync<SlotProvider>>();
            providerRepository.Insert(provider);
            await unitOfWork.SaveChangesAsync();
            //
            // Create slots
            var slots = new List<Slot>();
            for (var slotIndex = 1; slotIndex <= user.MaximumSlots; slotIndex++)
            {
                slots.Add(new Slot()
                {
                    Name = StringTable.Number + " " + slotIndex,
                    IsAvailable = true,
                    Price = user.Price,
                    SlotProviderId = provider.ID
                });
            }
            var slotRepository = ObjectFactory.GetInstance<IRepositoryAsync<Slot>>();
            slotRepository.InsertRange(slots);
            await unitOfWork.SaveChangesAsync();

            return provider;
        }

        private UserProfile CreateUserProfile(int? billingAddressId, ApplicationUser user, IUnitOfWorkAsync unitOfWork)
        {
            var userProfile = new UserProfile()
            {
                BillingAddressId = billingAddressId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserProfileType = user.UserProfileType
            };
            var userProfileRepository = ObjectFactory.GetInstance<IRepositoryAsync<UserProfile>>();
            userProfileRepository.Insert(userProfile);
            unitOfWork.SaveChanges();

            return userProfile;
        }

        private Address CreateUserAddress(ApplicationUser user, IUnitOfWorkAsync unitOfWork)
        {
            var addressLine = string.Empty;
            if (!string.IsNullOrEmpty(user.StreetNumber))
                addressLine = user.StreetNumber;
            if (!string.IsNullOrEmpty(user.Street))
            {
                if (!string.IsNullOrEmpty(addressLine))
                    addressLine = addressLine + " - " + user.Street;
                else
                    addressLine = user.Street;
            }
            if (user.DistrictId.HasValue)
            {
                var repository = ObjectFactory.GetInstance<IRepositoryAsync<District>>();
                var viewModel = repository.GetAddressViewModelByDistrictId(user.DistrictId.Value);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.DistrictName);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.CityName);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.StateName);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.CountryName);
            }
            else if (user.CityId.HasValue)
            {
                var repository = ObjectFactory.GetInstance<IRepositoryAsync<City>>();
                var viewModel = repository.GetAddressViewModelByCityId(user.CityId.Value);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.CityName);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.StateName);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.CountryName);
            }
            else if (user.StateId.HasValue)
            {
                var repository = ObjectFactory.GetInstance<IRepositoryAsync<State>>();
                var viewModel = repository.GetAddressViewModelByStateId(user.StateId.Value);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.StateName);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.CountryName);
            }
            else if (user.CountryId.HasValue)
            {
                var repository = ObjectFactory.GetInstance<IRepositoryAsync<Country>>();
                var viewModel = repository.GetAddressViewModelByCountryId(user.CountryId.Value);
                addressLine = string.Format("{0}, {1}", addressLine, viewModel.CountryName);
            }

            var addressRepository = ObjectFactory.GetInstance<IRepositoryAsync<Address>>();
            var newAddress = new Address()
            {
                StreetNumber = user.StreetNumber,
                Street = user.Street,
                ZipCode = user.ZipCode,
                AddressLine = addressLine,
                DistrictId = user.DistrictId,
                CityId = user.CityId,
                StateId = user.StateId,
                CountryId = user.CountryId
            };
            addressRepository.Insert(newAddress);
            unitOfWork.SaveChanges();

            return newAddress;
        }
        #endregion

    }
}
