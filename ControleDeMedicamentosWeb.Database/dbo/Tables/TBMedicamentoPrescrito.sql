CREATE TABLE [dbo].[TBMedicamentoPrescrito] (
    [RequisicaoSaidaId] UNIQUEIDENTIFIER NOT NULL,
    [MedicamentoId]     UNIQUEIDENTIFIER NOT NULL,
    [Quantidade]        INT              NOT NULL
);
GO

ALTER TABLE [dbo].[TBMedicamentoPrescrito]
    ADD CONSTRAINT [FK_TBMedicamentoPrescrito_TBRequisicaoSsaida] FOREIGN KEY ([RequisicaoSaidaId]) REFERENCES [dbo].[TBRequisicaoSaida] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[TBMedicamentoPrescrito]
    ADD CONSTRAINT [FK_TBMedicamentoPrescrito_TBMedicamento] FOREIGN KEY ([MedicamentoId]) REFERENCES [dbo].[TBMedicamento] ([Id]);
GO

ALTER TABLE [dbo].[TBMedicamentoPrescrito]
    ADD CONSTRAINT [PK_TBMedicamentoPrescrito] PRIMARY KEY CLUSTERED ([RequisicaoSaidaId] ASC, [MedicamentoId] ASC);
GO

