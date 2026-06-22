IF DB_ID('ControleDeMedicamentosWeb') IS NULL
BEGIN
  CREATE DATABASE [ControleDeMedicamentosWeb];
END;
GO

USE [ControleDeMedicamentosWeb];
GO

CREATE TABLE [dbo].[TBFornecedor]
(
  [Id] uniqueidentifier NOT NULL,
  [Nome] nvarchar(100) NOT NULL,
  [Telefone] nvarchar(15) NOT NULL,
  [CNPJ] nvarchar(18) NOT NULL,
  PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[TBPaciente]
(
  [Id] uniqueidentifier NOT NULL,
  [Nome] nvarchar(100) NOT NULL,
  [Telefone] nvarchar(15) NOT NULL,
  [Cpf] nvarchar(14) NOT NULL,
  [CataoSUS] nvarchar(15) NOT NULL,
  PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[TBFuncionario]
(
  [Id] uniqueidentifier NOT NULL,
  [Nome] nvarchar(100) NOT NULL,
  [Telefone] nvarchar(15) NOT NULL,
  [Cpf] nvarchar(14) NOT NULL,
  PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[TBMedicamento]
(
  [Id] uniqueidentifier NOT NULL,
  [Nome] nvarchar(100) NOT NULL,
  [Descricao] nvarchar(255) NOT NULL,
  [FornecedorId] uniqueidentifier NOT NULL,
  PRIMARY KEY ([Id])
);


ALTER TABLE [dbo].[TBMedicamento]
ADD CONSTRAINT [FK_TBMedicamento_TBFornecedor]
FOREIGN KEY ([FornecedorId])
REFERENCES [dbo].[TBFornecedor]([Id])
ON DELETE NO ACTION
ON UPDATE NO ACTION;



