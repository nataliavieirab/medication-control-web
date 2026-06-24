using ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra.Arquivos;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFornecedor.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFornecedor.Infra;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFuncionario.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloFuncionario.Infra;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloPaciente.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloPaciente.Infra;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloMedicamento.Dominio;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloMedicamento.Infra;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloEstoque.Infra;
using ControleDeMedicamentosWeb.WebApp.Modulos.ModuloEstoque.Dominio;
using ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra.Sql;

namespace ControleDeMedicamentosWeb.WebApp.Compartilhado.Infra;

public static class InjecaoDependencia
{
    public static void AddInfraRepositories(this IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            ContextoJson contextoJson = new ContextoJson();

            contextoJson.Carregar();

            return contextoJson;
        });

        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IRepositorioFornecedor, RepositorioFornecedorEmSql>();
        services.AddScoped<IRepositorioPaciente, RepositorioPacienteEmSql>();
        services.AddScoped<IRepositorioMedicamento, RepositorioMedicamentoEmSql>();
        services.AddScoped<IRepositorioFuncionario, RepositorioFuncionarioEmSql>();
        services.AddScoped<IRepositorioRequisicao, RepositorioRequisicaoEmSql>();
    }
}
