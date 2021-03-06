USE [Linear]
GO
/****** Object:  StoredProcedure [dbo].[DeleteSpotsByDistributionServer]    Script Date: 4/29/2022 3:19:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,4/19/22>
-- Description:	<Description,,Delete Spots by distribution server>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSpotsByDistributionServer] (
    @DistributionServerIdentity nvarchar(max)
)
AS 
BEGIN
    DECLARE @DistributionServerId int    

    IF(NOT EXISTS(SELECT Id FROM DistributionServers WHERE ServerIdentity = @DistributionServerIdentity))
    BEGIN
        RAISERROR ('INVALID DISTRIBUTION SERVER',15,-1, 'DeleteSpotByDistributionServer');  
        RETURN
    END

    SELECT @DistributionServerId = Id FROM DistributionServers WHERE ServerIdentity = @DistributionServerIdentity
    
    -- Clear out the M-M table for the specific Distro Server
    DELETE FROM DistributionServerSpots
    WHERE DistributionServerId = @DistributionServerId

    -- Clear out the spots which no longer have any joins in the Distribution Server Spots
    DELETE FROM Spots
    FROM Spots 
    LEFT JOIN DistributionServerSpots ON (spots.Id = DistributionServerSpots.SpotId)
    WHERE DistributionServerSpots.Id IS NULL
END
GO
