using ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra.Sql;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFuncionario.Dominio;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFuncionario.Infra;

public sealed class RepositorioFuncionarioEmSql(ISqlConnectionFactory connectionFactory)
    : IRepositorioFuncionario
{
    private const string InserirSql = """
        INSERT INTO dbo.TBFuncionario (Id, Nome, Telefone, Cpf)
        VALUES (@Id, @Nome, @Telefone, @Cpf);
        """;

    private const string AtualizarSql = """
        UPDATE dbo.TBFuncionario
        SET Nome = @Nome,
            Telefone = @Telefone,
            Cpf = @Cpf
        WHERE Id = @Id;
        """;

    private const string ExcluirSql = """
        DELETE FROM dbo.TBFuncionario
        WHERE Id = @Id;
        """;

    private const string SelecionarTodosSql = """
        SELECT Id, Nome, Telefone, Cpf
        FROM dbo.TBFuncionario
        ORDER BY Nome;
        """;

    private const string SelecionarPorIdSql = """
        SELECT Id, Nome, Telefone, Cpf
        FROM dbo.TBFuncionario
        WHERE Id = @Id;
        """;

    public void Cadastrar(Funcionario entidade)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        conexao.Execute(InserirSql, entidade);
    }

    public bool Editar(Guid idSelecionado, Funcionario entidadeAtualizada)
    {
        entidadeAtualizada.Id = idSelecionado;

        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Execute(AtualizarSql, entidadeAtualizada) > 0;
    }

    public bool Excluir(Guid idSelecionado)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Execute(ExcluirSql, new { Id = idSelecionado }) > 0;
    }

    public Funcionario? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.QuerySingleOrDefault<Funcionario>(
            SelecionarPorIdSql,
            new { Id = idSelecionado }
        );
    }

    public List<Funcionario> SelecionarTodos()
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao.Query<Funcionario>(SelecionarTodosSql).ToList();
    }

    public List<Funcionario> Filtrar(Predicate<Funcionario> filtro)
    {
        return SelecionarTodos().FindAll(filtro);
    }
}
