CREATE TABLE [dbo].[TBRequisicaoEntrada] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [FuncionarioId] UNIQUEIDENTIFIER NOT NULL,
    [MedicamentoId] UNIQUEIDENTIFIER NOT NULL,
    [Quantidade]    INT              NOT NULL
);
GO

ALTER TABLE [dbo].[TBRequisicaoEntrada]
    ADD CONSTRAINT [FK_RquisicaoEntrada_TBMedicamento] FOREIGN KEY ([MedicamentoId]) REFERENCES [dbo].[TBMedicamento] ([Id]);
GO

ALTER TABLE [dbo].[TBRequisicaoEntrada]
    ADD CONSTRAINT [FK_TBRequisicaoEntrada_TBRequisicao] FOREIGN KEY ([Id]) REFERENCES [dbo].[TBRequisicao] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[TBRequisicaoEntrada]
    ADD CONSTRAINT [FK_TBRequisicaoEntrada_TBFuncionario] FOREIGN KEY ([FuncionarioId]) REFERENCES [dbo].[TBFuncionario] ([Id]);
GO

ALTER TABLE [dbo].[TBRequisicaoEntrada]
    ADD CONSTRAINT [PK_TBRequisicaoEntrada] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

