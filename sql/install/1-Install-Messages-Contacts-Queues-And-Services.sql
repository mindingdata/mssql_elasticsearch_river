CREATE MESSAGE TYPE [RiverUpdateMessage]
VALIDATION = NONE
GO 

CREATE CONTRACT RiverUpdateContract
(RiverUpdateMessage SENT BY INITIATOR)
GO

CREATE QUEUE RiverUpdateSendQueue
GO

CREATE SERVICE RiverUpdateSendService
ON QUEUE RiverUpdateSendQueue (RiverUpdateContract)
GO

CREATE QUEUE RiverUpdateReceiveQueue
GO

CREATE SERVICE RiverUpdateReceiveService
ON QUEUE RiverUpdateReceiveQueue (RiverUpdateContract)
GO