CREATE PROCEDURE [auth].[RegisterUser]
	@login_CreatedDate datetime = null,
	@login_ModifiedDate datetime = null,
	@login_ExpiredDate datetime = null,
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
	@user_Telephone nvarchar(50) = null,
	@user_MobilePhone nvarchar(50) = null,
	@user_UserProfileType nvarchar(50) = null,
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
	@provider_ImageLocation nvarchar(150) = null

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
		DECLARE @userId INT

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
		IF @address_CountryId > 0
			SELECT @addressLine = @addressLine + ', ' + Name FROM common.Countries WHERE ID = @address_CountryId

		SET @addressLine = RTRIM(LTRIM(@addressLine))
	
	
		-- Insert record into common.Addresses table for new user
		INSERT INTO common.Addresses(StreetNumber, Street, ZipCode, AddressLine, DistrictId, CityId, StateId, CountryId)
		SELECT  @address_StreetNumber, @address_Street, @address_ZipCode, @addressLine, @address_DistrictId, @address_CityId, @address_StateId, @address_CountryId

		SELECT @addressId = @@Identity

		-- Insert record into common.Users table for new user
		INSERT INTO common.UserProfiles(FirstName, LastName, Email, Telephone, MobilePhone, UserProfileType, BillingAddressId)
		SELECT  @user_FirstName, @user_LastName, @login_Email, @user_Telephone, @user_MobilePhone, @user_UserProfileType, @addressId

		SELECT @userId = @@Identity
	
		-- Insert record into park.SlotProviders table for new provider
		IF @provider_ProviderName IS NOT NULL AND @provider_ProviderName <> ''
		BEGIN
			INSERT INTO park.SlotProviders(Name, Latitude, Longitude, MaximumSlots, Price, ImageLocation, IsAvailable, SlotOwnerId, AddressId)
			SELECT  @provider_ProviderName, @provider_Latitude, @provider_Longitude, @provider_MaximumSlots, @provider_Price, @provider_ImageLocation, 1, @userId, @addressId

			DECLARE @slotProviderId INT
			SELECT @slotProviderId = @@Identity

			DECLARE @count INT
			SET @count = 1
			DECLARE @slotName NVARCHAR(50)

			WHILE @count <= @provider_MaximumSlots
			BEGIN
				SET @slotName = 'Slot ' + CAST(@count AS NVARCHAR(5))

				INSERT INTO park.Slots(Name, SlotProviderId, Price, IsAvailable)
				SELECT  @slotName, @slotProviderId, @provider_Price, 1

				SET @count = @count + 1
			END
		END

		-- Insert record into auth.Logins table for new user
		INSERT INTO auth.Logins(CreatedDate, ModifiedDate, ExpiredDate, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
								 LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, UserProfileId)
		SELECT  @login_CreatedDate, @login_ModifiedDate, @login_ExpiredDate, @login_Email, @login_EmailConfirmed, @login_PasswordHash, @login_SecurityStamp, @login_PhoneNumber, 
				@login_PhoneNumberConfirmed, @login_TwoFactorEnabled, @login_LockoutEndDateUtc, @login_LockoutEnabled, @login_AccessFailedCount, @login_UserName, @userId
	
	COMMIT TRAN

END TRY
BEGIN CATCH 
	ROLLBACK TRAN

	DECLARE @ERROR_MESSASGE NVARCHAR(800) = ERROR_MESSAGE()
	DECLARE @ERROR_STATE NVARCHAR(800) = ERROR_STATE();

	THROW 112232, @ERROR_MESSASGE, @ERROR_STATE

END CATCH
