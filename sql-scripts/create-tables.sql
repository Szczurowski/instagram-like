CREATE TABLE [dbo].[T_Photo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[VisionAnalysis] [varchar](max) NULL,
	[OriginalContent] [varbinary](max) NOT NULL,
	[ThumbnailContent] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
