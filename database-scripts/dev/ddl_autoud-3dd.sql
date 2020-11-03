/****** Object:  Table [dbo].[ModelAssetConfig]    Script Date: 9/22/2020 2:17:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ModelAssetConfig]') AND type in (N'U'))
DROP TABLE [dbo].[ModelAssetConfig]
GO

/****** Object:  Table [dbo].[ModelAssetConfig]    Script Date: 9/22/2020 2:17:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ModelAssetConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetGroup] [int] NOT NULL,
	[AssetType] [int] NOT NULL,
	[ModelName] [nvarchar](50) NOT NULL,
	[UUID] [uniqueidentifier] NULL,
	[ZLocation] [real] NOT NULL,
	[Scale] [real] NOT NULL,
	[RotationX] [real] NOT NULL,
	[RotationY] [real] NOT NULL,
	[RotationZ] [real] NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Model]    Script Date: 9/22/2020 2:17:40 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Model]') AND type in (N'U'))
DROP TABLE [dbo].[Model]
GO

/****** Object:  Table [dbo].[Model]    Script Date: 9/22/2020 2:17:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Model](
	[ModelName] [nvarchar](500) NOT NULL,
	[X_Scale] [real] NULL,
	[Y_Scale] [real] NULL,
	[Z_Scale] [real] NULL,
	[Z_Position] [real] NULL,
PRIMARY KEY CLUSTERED 
(
	[ModelName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


