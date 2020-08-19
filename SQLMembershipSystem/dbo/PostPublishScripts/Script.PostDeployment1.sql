/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET IDENTITY_INSERT [dbo].[Companies] ON

MERGE INTO Companies AS Target
USING (VALUES
  (1,'Test Company', 'test@company.com', '12346', 1)
 ,(2,'Bows Formula One High Performance Cars', 'support@bowscars.com', '+441234567890', 1)
) AS Source ([Id],[Name],[Email],[PhoneNumber],[isLive])
ON (Target.[Id] = Source.[Id])
 
WHEN MATCHED THEN
 UPDATE SET
 [Name] = Source.[Name],[Email] = Source.[Email], [PhoneNumber] = Source.[PhoneNumber], [isLive] = Source.[isLive]
 
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[Name],[Email], [PhoneNumber], [isLive])
 VALUES(Source.[Id], Source.[Name], Source.[Email], Source.[PhoneNumber], Source.[isLive])
 
--delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;

GO
SET IDENTITY_INSERT [dbo].[Companies] OFF
GO