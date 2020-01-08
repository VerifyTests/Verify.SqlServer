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

