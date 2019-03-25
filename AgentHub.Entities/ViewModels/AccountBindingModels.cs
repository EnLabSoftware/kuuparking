using System;
using System.ComponentModel.DataAnnotations;
using AgentHub.Entities.Models.Common;

namespace AgentHub.Entities.ViewModels
{
    // Models used as parameters to AccountController actions.
#pragma warning disable 1591

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = @"External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ResetPasswordBindingModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = @"New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirm new password")]
        [Compare("NewPassword", ErrorMessage = @"The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        public string Provider { get; set; }
        public string ExternalAccessToken { get; set; }

        [Display(Name = @"Email")]
        public string Email { get; set; }

        [Display(Name = @"UserName")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = @"The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = @"Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = @"FullName")]
        public string FullName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirm password")]
        [Compare("Password", ErrorMessage = @"The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required]
        [Display(Name = @"Expiry Date")]
        public DateTime? ExpiredDate { get; set; }

        #region User profile properties
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public UserProfileType UserProfileType { get; set; }
        #endregion

        #region Slot provider properties
        [Required]
        public string ProviderName { get; set; }
        public string ProviderDescription { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public int MaximumSlots { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public string ImageLocation1 { get; set; }
        public string ImageLocation2 { get; set; }
        public string ImageLocation3 { get; set; }
        public string ThumbnailImageLocation { get; set; }
        public DateTime AvailableFromTime { get; set; }
        public DateTime AvailableToTime { get; set; }
        public bool? IsOpen247 { get; set; }
        public bool? IsWeekendAvailable { get; set; }
        public bool? IsCoveredParking { get; set; }
        public bool? IsOvernightParking { get; set; }
        public bool? IsBusParking { get; set; }
        public bool? IsCarWashingServiceAvailable { get; set; }
        public bool? IsMondayAvailable { get; set; }
        public int? MondayOpenTime { get; set; }
        public int? MondayClosedTime { get; set; }
        public bool? IsTuesdayAvailable { get; set; }
        public int? TuesdayOpenTime { get; set; }
        public int? TuesdayClosedTime { get; set; }
        public bool? IsWednesdayAvailable { get; set; }
        public int? WednedayOpenTime { get; set; }
        public int? WednedayClosedTime { get; set; }
        public bool? IsThursdayAvailable { get; set; }
        public int? ThursdayOpenTime { get; set; }
        public int? ThursdayClosedTime { get; set; }
        public bool? IsFridayAvailable { get; set; }
        public int? FridayOpenTime { get; set; }
        public int? FridayClosedTime { get; set; }
        public bool? IsSaturdayAvailable { get; set; }
        public int? SaturdayOpenTime { get; set; }
        public int? SaturdayClosedTime { get; set; }
        public bool? IsSundayAvailable { get; set; }
        public int? SundayOpenTime { get; set; }
        public int? SundayClosedTime { get; set; }
        #endregion

        #region Address properties
        public string StreetNumber { get; set; }
        [Required]
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        #endregion
    }

    public class LoginBindingModel
    {
        [Required]
        [Display(Name = @"UserName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = @"Password")]
        public string Password { get; set; }

        [Display(Name = @"IsKeepSignedIn")]
        public bool IsKeepSignedIn { get; set; }

        public UserProfileType UserProfileType { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = @"Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = @"Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = @"The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = @"New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Confirm new password")]
        [Compare("NewPassword", ErrorMessage = @"The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = @"Token")]
        public string Token { get; set; }

        [Required]
        [Display(Name = @"User Id")]
        public string UserId { get; set; }
    }

    public class ConfirmEmailBindingModel
    {
        [Display(Name = @"UserId")]
        public string UserId { get; set; }

        public string EmailConfirmationToken { get; set; }

        public string AccessToken { get; set; }

        public bool Succeeded { get; set; }
    }

    public class ForgotPasswordBindingModel
    {
        [Required]
        [Display(Name = @"EmailAddress")]
        public string EmailAddress { get; set; }
    }
}
