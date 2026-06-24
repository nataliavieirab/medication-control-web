CREATE TABLE [dbo].[TBFornecedor] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Nome]     NVARCHAR (100)   NOT NULL,
    [Telefone] NVARCHAR (15)    NOT NULL,
    [CNPJ]     NVARCHAR (18)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

