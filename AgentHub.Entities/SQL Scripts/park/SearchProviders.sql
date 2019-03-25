-- =============================================
-- Author:       Vinh Tran
-- Create date:  May 25, 2016
-- Description:  To search and return all slot providers that are in a given radius of a specified location
-- =============================================
CREATE PROCEDURE [park].[SearchProviders](@latitude DECIMAL(18, 2), @longitude DECIMAL(18, 2), @radius DECIMAL(18, 2),
	@priceFrom DECIMAL(18, 2) = NULL, @priceTo DECIMAL(18, 2) = NULL, 
	@isOpen247 BIT = NULL, @isWeekendAvailable BIT = NULL, @isCoveredParking BIT = NULL, 
	@isOvernightParking BIT = NULL, @isBusParking BIT = NULL, @isCarWashingServiceAvailable BIT = NULL)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @source geography = geography::Point(@latitude, @longitude, 4326);

	SELECT provider.*, addresses.AddressLine, @source.STDistance(geography::Point(Latitude, Longitude, 4326)) AS Distance
	FROM park.SlotProviders provider
		INNER JOIN common.Addresses addresses ON provider.AddressId = addresses.ID
	WHERE @source.STDistance(geography::Point(provider.Latitude, provider.Longitude, 4326)) <= @radius 
		AND (@priceFrom IS NULL OR provider.Price >= @priceFrom) AND (@priceTo IS NULL OR provider.Price <= @priceTo) 
		AND (@isOpen247 IS NULL OR provider.IsOpen247 = @isOpen247) AND (@isWeekendAvailable IS NULL OR provider.IsWeekendAvailable = @isWeekendAvailable) 
		AND (@isCoveredParking IS NULL OR provider.IsCoveredParking = @isCoveredParking) AND (@isOvernightParking IS NULL OR provider.IsOvernightParking = @isOvernightParking) 
		AND (@isBusParking IS NULL OR provider.IsBusParking = @isBusParking) AND (@isOvernightParking IS NULL OR provider.IsCarWashingServiceAvailable = @isCarWashingServiceAvailable) 
END
