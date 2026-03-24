## Tables

### ChildTable

```sql
CREATE TABLE [dbo].[ChildTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NOT NULL,
	[Value] [int] NOT NULL,
 CONSTRAINT [PK_ChildTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
```

### ColumnTypes

```sql
CREATE TABLE [dbo].[ColumnTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Code] [varchar](20) NULL,
	[Amount] [decimal](18, 4) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Price] [money] NULL,
	[Ratio] [float] NULL,
	[SmallNum] [smallint] NULL,
	[TinyNum] [tinyint] NULL,
	[BigNum] [bigint] NULL,
	[UniqueCode] [uniqueidentifier] NULL,
	[Notes] [text] NULL,
	[BinaryData] [varbinary](500) NULL,
	[MaxBinary] [varbinary](max) NULL,
	[FixedChar] [char](10) NULL,
	[FixedBinary] [binary](16) NULL,
	[TimeOnly] [time](7) NULL,
	[DateOnly] [date] NULL,
	[NText] [ntext] NULL,
	[Xml] [xml] NULL,
 CONSTRAINT [PK_ColumnTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

### CompositeKeyTable

```sql
CREATE TABLE [dbo].[CompositeKeyTable](
	[Key1] [int] NOT NULL,
	[Key2] [int] NOT NULL,
	[Data] [nvarchar](200) NULL,
 CONSTRAINT [PK_CompositeKeyTable] PRIMARY KEY CLUSTERED 
(
	[Key1] ASC,
	[Key2] ASC
) ON [PRIMARY]
) ON [PRIMARY]
```

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

SET ANSI_PADDING ON

CREATE UNIQUE NONCLUSTERED INDEX [IX_MultiIndexTable_Category_Name] ON [dbo].[MultiIndexTable]
(
	[Category] ASC,
	[Name] ASC
) ON [PRIMARY]
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_MultiIndexTable_Name] ON [dbo].[MultiIndexTable]
(
	[Name] ASC
) ON [PRIMARY]
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [IX_MultiIndexTable_Status_Desc] ON [dbo].[MultiIndexTable]
(
	[Status] DESC,
	[Name] ASC
) ON [PRIMARY]
```

### MultiTriggerTable

```sql
CREATE TABLE [dbo].[MultiTriggerTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_MultiTriggerTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]


CREATE TRIGGER TR_MultiTrigger_Delete
ON MultiTriggerTable
AFTER DELETE
AS RAISERROR ('Delete detected', 16, 10);

ALTER TABLE [dbo].[MultiTriggerTable] ENABLE TRIGGER [TR_MultiTrigger_Delete]

CREATE TRIGGER TR_MultiTrigger_Insert
ON MultiTriggerTable
AFTER INSERT
AS RAISERROR ('Insert detected', 16, 10);

ALTER TABLE [dbo].[MultiTriggerTable] ENABLE TRIGGER [TR_MultiTrigger_Insert]

CREATE TRIGGER TR_MultiTrigger_Update
ON MultiTriggerTable
AFTER UPDATE
AS RAISERROR ('Update detected', 16, 10);

ALTER TABLE [dbo].[MultiTriggerTable] ENABLE TRIGGER [TR_MultiTrigger_Update]
```

### MyOtherTable

```sql
CREATE TABLE [dbo].[MyOtherTable](
	[Value] [int] NULL
) ON [PRIMARY]
```

### MyTable

```sql
CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [MyIndex] ON [dbo].[MyTable]
(
	[Value] ASC
) ON [PRIMARY]

CREATE TRIGGER MyTrigger
ON MyTable
AFTER UPDATE
AS RAISERROR ('Notify Customer Relations', 16, 10);

ALTER TABLE [dbo].[MyTable] ENABLE TRIGGER [MyTrigger]
```

### ParentTable

```sql
CREATE TABLE [dbo].[ParentTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ParentTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY],
 CONSTRAINT [UQ_ParentTable_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
) ON [PRIMARY]
) ON [PRIMARY]
```

### SchemaTable

```sql
CREATE TABLE [TestSchema].[SchemaTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SchemaTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
```

### WithComputed

```sql
CREATE TABLE [dbo].[WithComputed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[FullName]  AS (([FirstName]+' ')+[LastName]),
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[TotalPrice]  AS ([Quantity]*[UnitPrice]) PERSISTED,
 CONSTRAINT [PK_WithComputed] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
```

### WithDefaults

```sql
CREATE TABLE [dbo].[WithDefaults](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Score] [int] NOT NULL,
 CONSTRAINT [PK_WithDefaults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
```

## Views

### MyView

```sql
CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
```

### MyViewWithJoin

```sql
CREATE VIEW MyViewWithJoin
AS
  SELECT c.Id, c.Value, p.Name AS ParentName
  FROM ChildTable c
  INNER JOIN ParentTable p ON c.ParentId = p.Id;
```

### MyViewWithSchemabinding

```sql
CREATE VIEW MyViewWithSchemabinding
WITH SCHEMABINDING
AS
  SELECT Id, Name
  FROM dbo.ParentTable;
```

### SchemaView

```sql
CREATE VIEW TestSchema.SchemaView
AS
  SELECT Id, Name
  FROM TestSchema.SchemaTable;
```
