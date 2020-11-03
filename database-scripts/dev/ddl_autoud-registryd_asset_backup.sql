CREATE TABLE [dbo].[Asset](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedTime] [datetime] NULL,
	[ModifiedTime] [datetime] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[CreatedBySystem] [nvarchar](50) NULL,
	[ModifiedBySystem] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[Fid] [int] NULL,
	[FeatureClassName] [varchar](50) NULL,
	[ModelGroup] [varchar](50) NULL,
	[ModelName] [varchar](50) NULL,
	[ModelDescription] [varchar](250) NULL,
	[WorkOrderStatus] [varchar](50) NULL,
	[Notes] [varchar](250) NULL,
	[DesignDrawingName] [varchar](50) NULL,
	[WorkOrderId] [varchar](50) NULL,
	[DesignVersion] [varchar](50) NULL,
	[Make] [nvarchar](50) NULL,
	[SerialNumber] [nvarchar](50) NULL,
	[AssetType] [nvarchar](50) NULL,
	[Longitude] [decimal](18, 0) NULL,
	[Latitude] [decimal](18, 0) NULL,
 CONSTRAINT [PK_Asset] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


