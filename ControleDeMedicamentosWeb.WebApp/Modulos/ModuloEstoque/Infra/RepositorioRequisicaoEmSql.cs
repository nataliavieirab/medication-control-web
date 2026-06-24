using System;
using ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra.Sql;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloEstoque.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloEstoque.Dominio.Base;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFornecedor.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFuncionario.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloMedicamento.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloPaciente.Dominio;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControleDeMedicamentosWeb.WebApp.Modulos.ModuloEstoque.Infra;

public sealed class RepositorioRequisicaoEmSql(ISqlConnectionFactory connectionFactory)
    : IRepositorioRequisicao
{
    private const string InserirRequisicaoSql = """
        INSERT INTO dbo.TBRequisicao (Id, DataCriacao)
        VALUES (@Id, @DataCriacao);
    """;

    private const string InserirRequisicaoEntradaSql = """
        INSERT INTO dbo.TBRequisicaoEntrada (Id, FuncionarioId, MedicamentoId, Quantidade)
        VALUES (@Id, @FuncionarioId, @MedicamentoId, @Quantidade);
    """;

    private const string InserirRequisicaoSaidaSql = """
        INSERT INTO dbo.TBRequisicaoSaida (Id, PacienteId)
        VALUES (@Id, @PacienteId);
    """;

    private const string InserirMedicamentoPrescritoSql = """
        INSERT INTO dbo.TBMedicamentoPrescrito (RequisicaoSaidaId, MedicamentoId, Quantidade)
        VALUES (@RequisicaoSaidaId, @MedicamentoId, @Quantidade);
    """;

    private const string SelecionarRequisicoesEntradaSql = """
        SELECT
            re.Id AS RequisicaoId,
            r.DataCriacao,
            func.Id AS FuncionarioId,
            func.Nome AS FuncionarioNome,
            func.Telefone AS FuncionarioTelefone,
            func.Cpf AS FuncionarioCpf,
            m.Id AS MedicamentoId,
            m.Nome AS MedicamentoNome,
            m.Descricao AS MedicamentoDescricao,
            f.Id AS FornecedorId,
            f.Nome AS FornecedorNome,
            f.Telefone AS FornecedorTelefone,
            f.Cnpj AS FornecedorCnpj,
            re.Quantidade
        FROM dbo.TBRequisicaoEntrada AS re
        JOIN dbo.TBRequisicao AS r
            ON r.Id = re.Id
        JOIN dbo.TBFuncionario AS func
            ON func.Id = re.FuncionarioId
        JOIN dbo.TBMedicamento AS m
            ON m.Id = re.MedicamentoId
        JOIN dbo.TBFornecedor AS f
            ON f.Id = m.FornecedorId
        ORDER BY r.DataCriacao, re.Id;
    """;


    private const string SelecionarRequisicoesSaidaSql = """
        SELECT
            rs.Id AS RequisicaoSaidaId,
            r.DataCriacao,
            p.Id AS PacienteId,
            p.Nome AS PacienteNome,
            p.Telefone AS PacienteTelefone,
            p.Cpf AS PacienteCpf,
            p.CartaoSus AS PacienteCartaoSus,
            mp.MedicamentoId,
            m.Nome AS MedicamentoNome,
            m.Descricao AS MedicamentoDescricao,
            f.Id AS FornecedorId,
            f.Nome AS FornecedorNome,
            f.Telefone AS FornecedorTelefone,
            f.Cnpj AS FornecedorCnpj,
            mp.Quantidade
        FROM dbo.TBRequisicaoSaida AS rs
        JOIN dbo.TBRequisicao AS r
            ON r.Id = rs.Id
        JOIN dbo.TBPaciente AS p
            ON p.Id = rs.PacienteId
        JOIN dbo.TBMedicamentoPrescrito AS mp
            ON mp.RequisicaoSaidaId = rs.Id
        JOIN dbo.TBMedicamento AS m
            ON m.Id = mp.MedicamentoId
        JOIN dbo.TBFornecedor AS f
            ON f.Id = m.FornecedorId
        ORDER BY r.DataCriacao, rs.Id, mp.MedicamentoId;
    """;

    public void Cadastrar(RequisicaoBase requisicao)
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        using SqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            conexao.Execute(InserirRequisicaoSql, requisicao, transacao);

            if (requisicao is RequisicaoEntrada requisicaoEntrada)
            {
                conexao.Execute(
                    InserirRequisicaoEntradaSql,
                    new
                    {
                        Id = requisicaoEntrada.Id,
                        FuncionarioId = requisicaoEntrada.Funcionario.Id,
                        MedicamentoId = requisicaoEntrada.Medicamento.Id,
                        Quantidade = (int)requisicaoEntrada.Quantidade
                    },
                    transacao
                );
            }
            else if (requisicao is RequisicaoSaida requisicaoSaida)
            {
                conexao.Execute(
                    InserirRequisicaoSaidaSql,
                    new
                    {
                        Id = requisicaoSaida.Id,
                        PacienteId = requisicaoSaida.Paciente.Id
                    },
                    transacao
                );

                foreach (MedicamentoPrescrito medicamentoPrescrito in requisicaoSaida.MedicamentosPrescritos)
                {
                    conexao.Execute(
                        InserirMedicamentoPrescritoSql,
                        new
                        {
                            RequisicaoSaidaId = requisicaoSaida.Id,
                            MedicamentoId = medicamentoPrescrito.Medicamento.Id,
                            Quantidade = (int)medicamentoPrescrito.Quantidade
                        },
                        transacao
                    );
                }
            }

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    public List<RequisicaoEntrada> SelecionarRequisicoesEntrada()
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        List<RequisicaoEntradaRow> linhas = conexao
            .Query<RequisicaoEntradaRow>(SelecionarRequisicoesEntradaSql)
            .ToList();

        List<RequisicaoEntrada> requisicoes = linhas.Select(l =>
        {
            return new RequisicaoEntrada
            {
                Id = l.RequisicaoId,
                DataCriacao = l.DataCriacao,
                Funcionario = new Funcionario
                {
                    Id = l.FuncionarioId,
                    Nome = l.FuncionarioNome,
                    Telefone = l.FuncionarioTelefone,
                    CPF = l.FuncionarioCpf
                },
                Medicamento = MapearMedicamento(l),
                Quantidade = (uint)l.Quantidade
            };
        }).ToList();

        return requisicoes;
    }

    public List<RequisicaoSaida> SelecionarRequisicoesSaida()
    {
        using SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        List<RequisicaoSaidaRow> linhas = conexao
            .Query<RequisicaoSaidaRow>(SelecionarRequisicoesSaidaSql)
            .ToList();

        Dictionary<Guid, RequisicaoSaida> requisicoesAgrupadas =
            new Dictionary<Guid, RequisicaoSaida>();

        foreach (RequisicaoSaidaRow linha in linhas)
        {
            if (!requisicoesAgrupadas
                .TryGetValue(linha.RequisicaoSaidaId, out RequisicaoSaida? requisicaoSaida))
            {
                requisicaoSaida = new RequisicaoSaida
                {
                    Id = linha.RequisicaoSaidaId,
                    DataCriacao = linha.DataCriacao,
                    Paciente = new Paciente
                    {
                        Id = linha.PacienteId,
                        Nome = linha.PacienteNome,
                        Telefone = linha.PacienteTelefone,
                        CPF = linha.PacienteCpf,
                        CartaoSUS = linha.PacienteCartaoSus
                    },
                    MedicamentosPrescritos = new List<MedicamentoPrescrito>()
                };

                requisicoesAgrupadas.Add(linha.RequisicaoSaidaId, requisicaoSaida);
            }

            requisicaoSaida.MedicamentosPrescritos.Add(
                new MedicamentoPrescrito
                {
                    Medicamento = MapearMedicamento(linha),
                    Quantidade = (uint)linha.Quantidade
                }
            );
        }

        return requisicoesAgrupadas.Values.ToList();
    }

    private static Medicamento MapearMedicamento(RequisicaoEntradaRow linha)
    {
        return new Medicamento
        {
            Id = linha.MedicamentoId,
            Nome = linha.MedicamentoNome,
            Descricao = linha.MedicamentoDescricao,
            Fornecedor = new Fornecedor
            {
                Id = linha.FornecedorId,
                Nome = linha.FornecedorNome,
                Telefone = linha.FornecedorTelefone,
                Cnpj = linha.FornecedorCnpj
            }
        };
    }

    private static Medicamento MapearMedicamento(RequisicaoSaidaRow linha)
    {
        return new Medicamento
        {
            Id = linha.MedicamentoId,
            Nome = linha.MedicamentoNome,
            Descricao = linha.MedicamentoDescricao,
            Fornecedor = new Fornecedor
            {
                Id = linha.FornecedorId,
                Nome = linha.FornecedorNome,
                Telefone = linha.FornecedorTelefone,
                Cnpj = linha.FornecedorCnpj
            }
        };
    }
}

public sealed class RequisicaoEntradaRow
{
    public Guid RequisicaoId { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid FuncionarioId { get; set; }
    public string FuncionarioNome { get; set; } = string.Empty;
    public string FuncionarioTelefone { get; set; } = string.Empty;
    public string FuncionarioCpf { get; set; } = string.Empty;
    public Guid MedicamentoId { get; set; }
    public string MedicamentoNome { get; set; } = string.Empty;
    public string MedicamentoDescricao { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public string FornecedorNome { get; set; } = string.Empty;
    public string FornecedorTelefone { get; set; } = string.Empty;
    public string FornecedorCnpj { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public sealed class RequisicaoSaidaRow
{
    public Guid RequisicaoSaidaId { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid PacienteId { get; set; }
    public string PacienteNome { get; set; } = string.Empty;
    public string PacienteTelefone { get; set; } = string.Empty;
    public string PacienteCpf { get; set; } = string.Empty;
    public string PacienteCartaoSus { get; set; } = string.Empty;
    public Guid MedicamentoId { get; set; }
    public string MedicamentoNome { get; set; } = string.Empty;
    public string MedicamentoDescricao { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public string FornecedorNome { get; set; } = string.Empty;
    public string FornecedorTelefone { get; set; } = string.Empty;
    public string FornecedorCnpj { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
