CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]

CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);

CREATE PROCEDURE MyProcedure
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
END;

CREATE FUNCTION MyFunction(
  @quantity INT,
  @list_price DEC(10,2),
  @discount DEC(4,2)
)
RETURNS DEC(10,2)
AS
BEGIN
    RETURN @quantity * @list_price * (1 - @discount);
END;