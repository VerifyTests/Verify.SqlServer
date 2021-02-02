-- Tables

CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]


create trigger MyTrigger
on MyTable
after update
as raiserror ('Notify Customer Relations', 16, 10);

ALTER TABLE [dbo].[MyTable] ENABLE TRIGGER [MyTrigger]


-- Views

create view MyView
as
  select Value
  from MyTable
  where (Value > 10);


-- StoredProcedures

create procedure MyProcedure
as
begin
  set nocount on;
  select Value
  from MyTable
  where (Value > 10);
end;


-- UserDefinedFunctions

create function MyFunction(
  @quantity int,
  @list_price dec(10,2),
  @discount dec(4,2)
)
returns dec(10,2)
as
begin
    return @quantity * @list_price * (1 - @discount);
end;


-- Synonyms

CREATE SYNONYM [dbo].[synonym1] FOR [MyTable]