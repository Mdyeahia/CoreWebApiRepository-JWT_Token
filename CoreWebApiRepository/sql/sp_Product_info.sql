
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON

alter PROCEDURE sp_Product_info @productid int, @categoryid int,@name varchar(50),@price decimal(10,2),@description varchar(200),@type varchar(20)
AS
if @type='insert'
BEGIN
	insert into [dbo].[Product] (productid,categoryid,name,price,description) values(@productid,@categoryid,@name,@price,@description)
END
if @type='update'
begin
update [dbo].Product set categoryid=@categoryid,name=@name,price=@price,description=@description where productid=@productid
end
if @type='delete'
begin
delete [dbo].Product  where productid=@productid
end


