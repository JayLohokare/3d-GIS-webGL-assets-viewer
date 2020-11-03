
IF OBJECT_ID('EventServiceMapView', 'V') IS NOT NULL
    DROP VIEW EventServiceMapView;
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventServiceMap]') AND type in (N'U'))
DROP TABLE [dbo].[EventServiceMap]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Event]') AND type in (N'U'))
DROP TABLE [dbo].[Event]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Service]') AND type in (N'U'))
DROP TABLE [dbo].[Service]
GO

CREATE TABLE [dbo].[Event](
	[Id] [nvarchar](50) NOT NULL,
	[OriginationSystem] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Service](
	[Id] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ServiceBusTopicId] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EventServiceMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FK_ServiceId] [nvarchar](50) NOT NULL,
	[FK_EventId] [nvarchar](50) NOT NULL,
	[NotifyCompletion] [bit] NOT NULL,
	[ExecutionOrder] [int] NOT NULL,
 CONSTRAINT [PK_EventServiceMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[EventServiceMapView]
AS
SELECT   dbo.EventServiceMap.Id, dbo.EventServiceMap.FK_ServiceId AS ServiceId, dbo.EventServiceMap.FK_EventId AS EventId, dbo.EventServiceMap.NotifyCompletion, 
                         dbo.Event.OriginationSystem, dbo.Service.Name, dbo.Service.ServiceBusTopicId, dbo.EventServiceMap.ExecutionOrder
FROM         dbo.EventServiceMap INNER JOIN
                         dbo.Event ON dbo.EventServiceMap.FK_EventId = dbo.Event.Id INNER JOIN
                         dbo.Service ON dbo.Service.Id = dbo.EventServiceMap.FK_ServiceId
GO
