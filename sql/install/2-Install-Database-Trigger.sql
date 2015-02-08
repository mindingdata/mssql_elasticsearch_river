CREATE TRIGGER RiverUpdateTrigger ON YOURDATABASEHERE
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @RiverUpdateDialog uniqueidentifier
	BEGIN DIALOG CONVERSATION @RiverUpdateDialog
	FROM SERVICE RiverUpdateSendService
	TO SERVICE 'RiverUpdateReceiveService'
	ON CONTRACT RiverUpdateContract
	WITH ENCRYPTION = OFF
		
	
	DECLARE @ID int
	SELECT @ID = CONVERT(int, ID) from inserted;
	if(@ID IS NULL)
	BEGIN
		SELECT @ID = CONVERT(int, ID) FROM deleted;
	END;
	
	DECLARE @Message XML;
	
	SELECT @Message = (SELECT @ID as Id, --YOURDATABASE HERE AS STRING-- as DatabaseTable
	FOR XML PATH (''), ROOT('RiverMessage'));
	
					
	SEND ON CONVERSATION @RiverUpdateDialog
	MESSAGE TYPE RiverUpdateMessage (@Message)
					
END;
GO

