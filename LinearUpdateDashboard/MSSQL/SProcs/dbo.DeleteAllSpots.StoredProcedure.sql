USE [Linear]
GO
/****** Object:  StoredProcedure [dbo].[DeleteAllSpots]    Script Date: 4/29/2022 3:19:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,4/19/22>
-- Description:	<Description,,Delete All Spots>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteAllSpots] 
AS 
BEGIN
    DELETE FROM DistributionServerSpots
    DELETE FROM Spots
END
GO
