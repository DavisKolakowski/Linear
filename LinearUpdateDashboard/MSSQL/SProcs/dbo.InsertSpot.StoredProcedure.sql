USE [Linear]
GO
/****** Object:  StoredProcedure [dbo].[InsertSpot]    Script Date: 4/29/2022 3:19:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Davis Kolakowski>
-- Create date: <Create Date,4/19/2022,>
-- Description:	<Description,Checks if rows exist or not,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertSpot] (
    @DistributionServerIdentity nvarchar(max),
    @SpotCode nvarchar(max),
    @FirstAirDate datetime,
	@Name nvarchar(max)
)
AS
BEGIN
    DECLARE @DistributionServerId int
    DECLARE @SpotId int
    DECLARE @LastUpdated datetime = getdate()

    IF(NOT EXISTS(SELECT Id FROM DistributionServers WHERE ServerIdentity = @DistributionServerIdentity))
    BEGIN
        RAISERROR ('INVALID DISTRIBUTION SERVER',15,-1, 'InsertSpot');  
        RETURN
    END

    SELECT @DistributionServerId = Id FROM DistributionServers WHERE ServerIdentity = @DistributionServerIdentity

    IF(NOT EXISTS(SELECT Id FROM Spots WHERE SpotCode = @SpotCode))
    BEGIN
        -- SPOT DOESNT EXIST AT ALL
        -- INSERT SPOT AND DistroServerSpots
        INSERT INTO Spots (SpotCode, LastUpdated, Name) 
        VALUES (@SpotCode, @LastUpdated, @Name)
        
        SELECT @SpotId = SCOPE_IDENTITY()
        INSERT INTO DistributionServerSpots (SpotId, DistributionServerId, FirstAirDate, LastUpdated)
        VALUES (@SpotId, @DistributionServerId, @FirstAirDate, @LastUpdated)
        RETURN
    END
    ELSE
    BEGIN
        -- SPOT EXISTS SOMEWHERE
        -- FIND EXISTING SPOT ID
        SELECT @SpotId = Id FROM Spots WHERE SpotCode = @SpotCode

        --FIND IF WE HAVE AN EXISTING Many-Many LINK FOR THIS SPOT TO THE DISTRO
        IF(NOT EXISTS(SELECT Id FROM DistributionServerSpots WHERE SpotId = @SpotId AND DistributionServerId = @DistributionServerId))
        BEGIN
            -- NO LINK TO THIS DISTRO SO INSERT
            INSERT INTO DistributionServerSpots (SpotId, DistributionServerId, FirstAirDate, LastUpdated)
            VALUES (@SpotId, @DistributionServerId, @FirstAirDate, @LastUpdated)
        END
        
    END

END
GO
