-- Tables

CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]


create trigger MyTrigger
on MyTable
after update
as raiserror ('Notify Customer Relations', 16, 10);

ALTER TABLE [dbo].[MyTable] ENABLE TRIGGER [MyTrigger]