
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON

alter PROCEDURE sp_user_info @userid int, @username varchar(50),@email varchar(50),@password varchar(50),@type varchar(20)
AS
if @type='insert'
BEGIN
	insert into [dbo].[User] ([UserId],[Username],Email,Password) values(@userid,@username,@email,@password)
END
if @type='update'
begin
update [dbo].[User] set Username=@username,Email=@email,Password=@password where UserId=@userid
end
if @type='delete'
begin
delete [dbo].[User]  where UserId=@userid
end


