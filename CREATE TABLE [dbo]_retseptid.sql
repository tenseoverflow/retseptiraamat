CREATE TABLE [dbo].[retseptid]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [nimi] VARCHAR (MAX) NOT NULL,
    [desc] TEXT NOT NULL,
    CONSTRAINT [PK_retseptid] PRIMARY KEY CLUSTERED ([Id] ASC)
);

