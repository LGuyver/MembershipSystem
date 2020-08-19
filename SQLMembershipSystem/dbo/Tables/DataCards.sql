CREATE TABLE [dbo].[DataCards] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [MemberId] INT          NOT NULL,
    [CardId]   VARCHAR (16) NOT NULL,
    [isLive]   BIT          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members] ([Id]),
    UNIQUE NONCLUSTERED ([CardId] ASC)
);

