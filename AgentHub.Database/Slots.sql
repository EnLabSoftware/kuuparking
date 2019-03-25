CREATE TABLE [Slots]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(250) NULL, 
    [Size] INT NULL, 
    [Longtitude] BIGINT NULL, 
    [Latitude] BIGINT NULL, 
    [Address] NVARCHAR(1024) NULL, 
    [Price] MONEY NULL, 
    [IsAvailable] BIT NULL, 
    [CityId] INT NULL
)
