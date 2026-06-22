using System;
using ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra.Sql;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFornecedor.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloMedicamento.Dominio;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControleDeMedicamentosWeb.WebApp.Modulos.ModuloMedicamento.Infra;

public sealed class RepositorioMedicamentoEmSql(ISqlConnectionFactory connectionFactory)
    : IRepositorioMedicamento
{
    private const string InserirMedicamentoSql = """
        INSERT INTO dbo.TBMedicamento (Id, Nome, Descricao, FornecedorId)
        VALUES (@Id, @Nome, @Descricao, @FornecedorId);
        """;

    private const string AtualizarMedicamentoSql = """
        UPDATE dbo.TBMedicamento
        SET Nome = @Nome,
            Descricao = @Descricao,
            FornecedorId = @FornecedorId
        WHERE Id = @Id;
        """;

    private const string ExcluirMedicamentoSql = """
        DELETE FROM dbo.TBMedicamento
        WHERE Id = @Id;
    """;

    private const string SelecionarTodosMedicamentosSql = """
        SELECT
            m.Id AS MedicamentoId,
            m.Nome AS MedicamentoNome,
            m.Descricao AS MedicamentoDescricao,
            f.Id AS FornecedorId,
            f.Nome AS FornecedorNome,
            f.Telefone AS FornecedorTelefone,
            f.Cnpj AS FornecedorCnpj
        FROM dbo.TBMedicamento AS m
        JOIN dbo.TBFornecedor AS f
            ON f.Id = m.FornecedorId
        ORDER BY m.Nome;
    """;

    private const string SelecionarMedicamentoPorIdSql = """
        SELECT
            m.Id AS MedicamentoId,
            m.Nome AS MedicamentoNome,
            m.Descricao AS MedicamentoDescricao,
            f.Id AS FornecedorId,
            f.Nome AS FornecedorNome,
            f.Telefone AS FornecedorTelefone,
            f.Cnpj AS FornecedorCnpj
        FROM dbo.TBMedicamento AS m
        JOIN dbo.TBFornecedor AS f
            ON f.Id = m.FornecedorId
        WHERE m.Id = @Id;
    """;

    public void Cadastrar(Medicamento entidade)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        conexao.Execute(
            InserirMedicamentoSql,
            new
            {
                Id = entidade.Id,
                Nome = entidade.Nome,
                Descricao = entidade.Descricao,
                FornecedorId = entidade.Fornecedor.Id
            }
        );
    }

    public bool Editar(Guid idSelecionado, Medicamento entidadeAtualizada)
    {
        entidadeAtualizada.Id = idSelecionado;

        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Execute(
            AtualizarMedicamentoSql,
            new
            {
                Id = entidadeAtualizada.Id,
                Nome = entidadeAtualizada.Nome,
                Descricao = entidadeAtualizada.Descricao,
                FornecedorId = entidadeAtualizada.Fornecedor.Id
            }
        ) == 1;
    }

    public bool Excluir(Guid idSelecionado)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Execute(ExcluirMedicamentoSql, new { Id = idSelecionado }) == 1;
    }

    public Medicamento? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        MedicamentoRow? medicamentoRow = conexao.QuerySingleOrDefault<MedicamentoRow>(
            SelecionarMedicamentoPorIdSql,
            new { Id = idSelecionado }
        );

        if (medicamentoRow == null)
            return null;

        return MapearMedicamento(medicamentoRow);
    }

    public List<Medicamento> SelecionarTodos()
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao
            .Query<MedicamentoRow>(SelecionarTodosMedicamentosSql)
            .Select(MapearMedicamento)
            .ToList();
    }

    public List<Medicamento> Filtrar(Predicate<Medicamento> filtro)
    {
        return SelecionarTodos().FindAll(filtro);
    }

    private static Medicamento MapearMedicamento(MedicamentoRow medicamentoRow)
    {
        return new Medicamento
        {
            Id = medicamentoRow.MedicamentoId,
            Nome = medicamentoRow.MedicamentoNome,
            Descricao = medicamentoRow.MedicamentoDescricao,
            Fornecedor = new Fornecedor
            {
                Id = medicamentoRow.FornecedorId,
                Nome = medicamentoRow.FornecedorNome,
                Telefone = medicamentoRow.FornecedorTelefone,
                Cnpj = medicamentoRow.FornecedorCnpj
            }
        };
    }
}

public sealed class MedicamentoRow
{
    public Guid MedicamentoId { get; set; }
    public string MedicamentoNome { get; set; } = string.Empty;
    public string MedicamentoDescricao { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public string FornecedorNome { get; set; } = string.Empty;
    public string FornecedorTelefone { get; set; } = string.Empty;
    public string FornecedorCnpj { get; set; } = string.Empty;
}
