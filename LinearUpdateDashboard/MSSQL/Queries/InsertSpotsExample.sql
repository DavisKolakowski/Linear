exec dbo.InsertSpot 
    @DistributionServerIdentity = 'vaacdpit02',
    @SpotCode = 'ABCD1234',
    @FirstAirDate = '2022-04-19 21:15:22'

exec dbo.InsertSpot 
    @DistributionServerIdentity = 'vaacdphl01',
    @SpotCode = 'ABCD1234',
    @FirstAirDate = '2022-04-19 21:15:22'

exec dbo.InsertSpot 
    @DistributionServerIdentity = 'vaacdphl01',
    @SpotCode = 'ABCD5432',
    @FirstAirDate = '2022-04-19 21:15:22'

exec dbo.InsertSpot 
    @DistributionServerIdentity = 'imaserver',
    @SpotCode = 'XYZ123',
    @FirstAirDate = '2022-04-19 21:15:22'