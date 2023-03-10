USE [AdditionalFields]
GO
/****** Object:  Table [dbo].[Contacts]    Script Date: 03/03/2023 4:06:41 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[ContactId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contacts_Custom]    Script Date: 03/03/2023 4:06:41 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts_Custom](
	[ContactId] [int] NOT NULL,
	[Company] [nvarchar](50) NULL,
	[IsPermanent] [bit] NULL,
	[ETagNumber] [int] NULL,
 CONSTRAINT [PK_Contact_Custom] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Contacts] ON 

INSERT [dbo].[Contacts] ([ContactId], [Name], [Email], [Phone]) VALUES (1, N'Dave Smit', N'dave@xyz.tld', N'111-111-1111')
INSERT [dbo].[Contacts] ([ContactId], [Name], [Email], [Phone]) VALUES (2, N'Jane Smith', N'jane@xyz.tld', N'111-111-1234')
SET IDENTITY_INSERT [dbo].[Contacts] OFF
GO
INSERT [dbo].[Contacts_Custom] ([ContactId], [Company], [IsPermanent], [ETagNumber]) VALUES (1, N'ABC Inc', 1, 121234)
INSERT [dbo].[Contacts_Custom] ([ContactId], [Company], [IsPermanent], [ETagNumber]) VALUES (2, N'XYZ LLC', 0, 921837)
GO
ALTER TABLE [dbo].[Contacts_Custom]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Custom_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Contacts_Custom] CHECK CONSTRAINT [FK_Contact_Custom_Contact]
GO
