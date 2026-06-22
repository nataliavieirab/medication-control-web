using ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra.Sql;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFornecedor.Dominio;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFornecedor.Infra;

public sealed class RepositorioFornecedorEmSql(ISqlConnectionFactory connectionFactory)
    : IRepositorioFornecedor
{
    private const string InserirSql = """
        INSERT INTO dbo.TBFornecedor (Id, Nome, Telefone, Cnpj)
        VALUES (@Id, @Nome, @Telefone, @Cnpj);
    """;

    private const string AtualizarSql = """
        UPDATE dbo.TBFornecedor
        SET
            Nome = @Nome,
            Telefone = @Telefone,
            Cnpj = @Cnpj
        WHERE Id = @Id;
    """;

    private const string ExcluirSql = """
        DELETE FROM dbo.TBFornecedor
        WHERE Id = @Id;
    """;

    private const string SelecionarPorIdSql = """
        SELECT Id, Nome, Telefone, Cnpj
        FROM dbo.TBFornecedor
        WHERE Id = @Id;
    """;

    private const string SelecionarTodosSql = """
        SELECT Id, Nome, Telefone, Cnpj
        FROM dbo.TBFornecedor
        ORDER BY Nome;
    """;

    public void Cadastrar(Fornecedor entidade)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        conexao.Execute(InserirSql, entidade);
    }

    public bool Editar(Guid idSelecionado, Fornecedor entidadeAtualizada)
    {
        entidadeAtualizada.Id = idSelecionado;

        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Execute(AtualizarSql, entidadeAtualizada) == 1;
    }

    public bool Excluir(Guid idSelecionado)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Execute(ExcluirSql, new { Id = idSelecionado }) == 1;
    }

    public Fornecedor? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.QuerySingleOrDefault<Fornecedor>(SelecionarPorIdSql, new { Id = idSelecionado });
    }

    public List<Fornecedor> SelecionarTodos()
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Query<Fornecedor>(SelecionarTodosSql).ToList();
    }

    public List<Fornecedor> Filtrar(Predicate<Fornecedor> filtro)
    {
        return SelecionarTodos().FindAll(filtro);
    }
}
