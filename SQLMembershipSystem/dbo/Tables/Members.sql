CREATE TABLE [dbo].[Members] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeId]  VARCHAR (25)  NOT NULL,
    [FirstName]   VARCHAR (25)  NULL,
    [LastName]    VARCHAR (25)  NULL,
    [Email]       VARCHAR (320) NOT NULL,
    [PhoneNumber] VARCHAR (15)  NOT NULL,
    [SecurityPin] INT           NOT NULL,
    [CompanyId]   INT           NOT NULL,
    [isLive]      BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([Id]),
    UNIQUE NONCLUSTERED ([EmployeeId] ASC)
);

