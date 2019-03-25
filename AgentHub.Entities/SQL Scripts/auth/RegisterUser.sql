-- =============================================
-- Author:       Vinh Tran
-- Create date:  May 25, 2016
-- Description:  To register a new user login and create associated user profile for that user. If the user is the slot provider, it will create a slot provider profile as well.
-- =============================================
CREATE PROCEDURE [auth].[RegisterUser]
	-- Output parameters
	@login_id nvarchar(128) output,

	@login_CreatedDate datetime = null,
	@login_ModifiedDate datetime = null,
	@login_ExpiredDate datetime = null,
	@login_InternationalPhoneNumber nvarchar(max) = null,
	@login_Email nvarchar(max) = null,
	@login_EmailConfirmed bit,
	@login_PasswordHash nvarchar(max) = null,
	@login_SecurityStamp nvarchar(max) = null,
	@login_PhoneNumber nvarchar(max) = null,
	@login_PhoneNumberConfirmed bit,
	@login_TwoFactorEnabled bit,
	@login_LockoutEndDateUtc datetime = null,
	@login_LockoutEnabled bit,
	@login_AccessFailedCount int,
	@login_UserName nvarchar(max) = null,
	-- User info
	@user_FirstName nvarchar(50) = null, 
	@user_LastName nvarchar(50) = null,
	@user_UserProfileType INT = null,
	-- Address info
	@address_StreetNumber nvarchar(20) = null, 
	@address_Street nvarchar(80) = null, 
	@address_ZipCode nvarchar(10) = null, 
	@address_DistrictId int = null, 
	@address_CityId int = null, 
	@address_StateId int = null, 
	@address_CountryId int = null,
	-- Provider info
	@provider_ProviderName nvarchar(250) = null, 
	@provider_Latitude float = 0,
	@provider_Longitude float = 0,
	@provider_MaximumSlots int = 0,
	@provider_Price decimal(18,2) = 0,
	@provider_ImageLocation1 nvarchar(150) = null,
	@provider_ImageLocation2 nvarchar(150) = null,
	@provider_ImageLocation3 nvarchar(150) = null,
	@provider_IsOpen247 bit,
	@provider_IsWeekendAvailable bit,
	@provider_IsCoveredParking bit,
	@provider_IsOvernightParking bit,
	@provider_IsBusParking bit,
	@provider_IsCarWashingServiceAvailable bit,
	@provider_IsMondayAvailable bit,
	@provider_MondayOpenTime int = null,
	@provider_MondayClosedTime int = null,
	@provider_IsTuesdayAvailable bit,
	@provider_TuesdayOpenTime int = null,
	@provider_TuesdayClosedTime int = null,
	@provider_IsWednesdayAvailable bit,
	@provider_WednedayOpenTime int = null,
	@provider_WednedayClosedTime int = null,
	@provider_IsThursdayAvailable bit,
	@provider_ThursdayOpenTime int = null,
	@provider_ThursdayClosedTime int = null,
	@provider_IsFridayAvailable bit,
	@provider_FridayOpenTime int = null,
	@provider_FridayClosedTime int = null,
	@provider_IsSaturdayAvailable bit,
	@provider_SaturdayOpenTime int = null,
	@provider_SaturdayClosedTime int = null,
	@provider_IsSundayAvailable bit,
	@provider_SundayOpenTime int = null,
	@provider_SundayClosedTime int = null

AS
SET NOCOUNT ON
/*******************************************************************************
Date :-  13rd May 2016
Purpose:- Single procedure to perform insert of auth.Logins, common.Addresses, park.SlotProviders, park.Slots, and common.UserProfiles data
		 Batch processing requries calling the procedure for each user registration details being added
*******************************************************************************/

BEGIN TRY
	BEGIN TRAN
		DECLARE @addressLine NVARCHAR(250)
		DECLARE @addressId INT
		DECLARE @userProfileId INT

		SET @addressLine = ''

		IF @address_StreetNumber IS NOT NULL
			SET @addressLine = @address_StreetNumber
		IF @address_Street IS NOT NULL
		BEGIN
			IF @addressLine <> ''
				SET @addressLine = @addressLine + ' - ' + @address_Street
			ELSE
				SET @addressLine = @address_Street
		END
		IF @address_DistrictId > 0
			SELECT @addressLine = @addressLine + ', ' + Name FROM common.Districts WHERE ID = @address_DistrictId
		IF @address_CityId > 0
			SELECT @addressLine = @addressLine + ', ' + Name FROM common.Cities WHERE ID = @address_CityId
		IF @address_StateId > 0
			SELECT @addressLine = @addressLine + ', ' + Name FROM common.States WHERE ID = @address_StateId
		--IF @address_CountryId > 0
			--SELECT @addressLine = @addressLine + ', ' + Name FROM common.Countries WHERE ID = @address_CountryId

		SET @addressLine = RTRIM(LTRIM(@addressLine))	
	
		-- Insert record into common.Addresses table for new user
		INSERT INTO common.Addresses(StreetNumber, Street, ZipCode, AddressLine, DistrictId, CityId, StateId, CountryId)
		SELECT  @address_StreetNumber, @address_Street, @address_ZipCode, @addressLine, @address_DistrictId, @address_CityId, @address_StateId, @address_CountryId

		SELECT @addressId = @@Identity

		-- Insert record into common.Users table for new user
		INSERT INTO common.UserProfiles(FirstName, LastName, Email, PhoneNumber, UserProfileType, BillingAddressId)
		SELECT  @user_FirstName, @user_LastName, @login_Email, @login_PhoneNumber, @user_UserProfileType, @addressId

		SELECT @userProfileId = @@Identity
	
		-- Insert record into park.SlotProviders table for new provider
		IF @provider_ProviderName IS NOT NULL AND @provider_ProviderName <> ''
		BEGIN
			INSERT INTO park.SlotProviders(Name, Latitude, Longitude, MaximumSlots, Price, IsAvailable, SlotOwnerId, AddressId, ImageLocation1, ImageLocation2, ImageLocation3, IsOpen247, IsWeekendAvailable, IsCoveredParking, IsOvernightParking, IsBusParking, IsCarWashingServiceAvailable, IsMondayAvailable, MondayOpenTime, MondayClosedTime, IsTuesdayAvailable, TuesdayOpenTime, TuesdayClosedTime, IsWednesdayAvailable, WednedayOpenTime, WednedayClosedTime, IsThursdayAvailable, ThursdayOpenTime, ThursdayClosedTime, IsFridayAvailable, FridayOpenTime, FridayClosedTime, IsSaturdayAvailable, SaturdayOpenTime, SaturdayClosedTime, IsSundayAvailable, SundayOpenTime, SundayClosedTime)
			SELECT  @provider_ProviderName, @provider_Latitude, @provider_Longitude, @provider_MaximumSlots, @provider_Price, 1, @userProfileId, @addressId, @provider_ImageLocation1, @provider_ImageLocation2, @provider_ImageLocation3, @provider_IsOpen247, @provider_IsWeekendAvailable, @provider_IsCoveredParking, @provider_IsOvernightParking, @provider_IsBusParking, @provider_IsCarWashingServiceAvailable, @provider_IsMondayAvailable, @provider_MondayOpenTime, @provider_MondayClosedTime, @provider_IsTuesdayAvailable, @provider_TuesdayOpenTime, @provider_TuesdayClosedTime, @provider_IsWednesdayAvailable, @provider_WednedayOpenTime, @provider_WednedayClosedTime, @provider_IsThursdayAvailable, @provider_ThursdayOpenTime, @provider_ThursdayClosedTime, @provider_IsFridayAvailable, @provider_FridayOpenTime, @provider_FridayClosedTime, @provider_IsSaturdayAvailable, @provider_SaturdayOpenTime, @provider_SaturdayClosedTime, @provider_IsSundayAvailable, @provider_SundayOpenTime, @provider_SundayClosedTime

			DECLARE @slotProviderId INT
			SELECT @slotProviderId = @@Identity

			DECLARE @count INT
			SET @count = 1
			DECLARE @slotName NVARCHAR(50)

			WHILE @count <= @provider_MaximumSlots
			BEGIN
				SET @slotName = N'Slot ' + CAST(@count AS NVARCHAR(5))

				INSERT INTO park.Slots(Name, SlotProviderId, Price, IsAvailable)
				SELECT  @slotName, @slotProviderId, @provider_Price, 1

				SET @count = @count + 1
			END
		END

		-- Insert record into auth.Logins table for new user
		SELECT @login_id =  NEWID()
		INSERT INTO auth.ApplicationUsers(ID, CreatedDate, ModifiedDate, ExpiredDate, InternationalPhoneNumber, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, 
				PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, UserProfileId)
		SELECT  @login_id, @login_CreatedDate, @login_ModifiedDate, @login_ExpiredDate, @login_InternationalPhoneNumber, @login_Email, @login_EmailConfirmed, @login_PasswordHash, @login_SecurityStamp, @login_PhoneNumber, 
				@login_PhoneNumberConfirmed, @login_TwoFactorEnabled, @login_LockoutEndDateUtc, @login_LockoutEnabled, @login_AccessFailedCount, @login_UserName, @userProfileId
	
	COMMIT TRAN

END TRY
BEGIN CATCH 
	ROLLBACK TRAN

	DECLARE @ERROR_MESSASGE NVARCHAR(800) = ERROR_MESSAGE()
	DECLARE @ERROR_STATE NVARCHAR(800) = ERROR_STATE();

	THROW 112232, @ERROR_MESSASGE, @ERROR_STATE

END CATCH