CREATE TABLE [dbo].[TBMedicamento] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Nome]         NVARCHAR (100)   NOT NULL,
    [Descricao]    NVARCHAR (255)   NOT NULL,
    [FornecedorId] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[TBMedicamento]
    ADD CONSTRAINT [FK_TBMedicamento_TBFornecedor] FOREIGN KEY ([FornecedorId]) REFERENCES [dbo].[TBFornecedor] ([Id]);
GO

