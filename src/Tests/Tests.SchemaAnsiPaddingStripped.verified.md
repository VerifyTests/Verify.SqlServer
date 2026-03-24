## Tables

### MultiIndexTable

```sql
CREATE TABLE [dbo].[MultiIndexTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Category] [nvarchar](50) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_MultiIndexTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX [IX_MultiIndexTable_Category_Name] ON [dbo].[MultiIndexTable]
(
	[Category] ASC,
	[Name] ASC
) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_MultiIndexTable_Name] ON [dbo].[MultiIndexTable]
(
	[Name] ASC
) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_MultiIndexTable_Status_Desc] ON [dbo].[MultiIndexTable]
(
	[Status] DESC,
	[Name] ASC
) ON [PRIMARY]
```