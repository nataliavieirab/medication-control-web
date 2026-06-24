CREATE TABLE [dbo].[TBPaciente] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Nome]     NVARCHAR (100)   NOT NULL,
    [Telefone] NVARCHAR (15)    NOT NULL,
    [Cpf]      NVARCHAR (14)    NOT NULL,
    [CataoSUS] NVARCHAR (15)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

