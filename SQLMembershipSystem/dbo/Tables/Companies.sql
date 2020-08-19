CREATE TABLE [dbo].[Companies] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (320) NOT NULL,
    [Email]       VARCHAR (320) NOT NULL,
    [PhoneNumber] VARCHAR (15)  NOT NULL,
    [isLive]      BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

