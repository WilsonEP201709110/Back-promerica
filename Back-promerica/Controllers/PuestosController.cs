using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Back_promerica.Models;
using System.Data;

namespace Back_promerica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuestosController : ControllerBase
    {
        private readonly string _connectionString;

        public PuestosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryMultipleAsync("dbo.SP_M_Puestos", 
                new { Accion = "SELECT" }, 
                commandType: CommandType.StoredProcedure);

            var puestos = await result.ReadAsync<PuestoModel>();
            var status = await result.ReadSingleOrDefaultAsync<StoredProcedureResponse>();

            return Ok(new { Data = puestos, Status = status });
        }

        [HttpGet("jerarquia")]
        public async Task<IActionResult> GetJerarquia()
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryMultipleAsync("dbo.SP_M_Puestos", 
                new { Accion = "SELECT" }, 
                commandType: CommandType.StoredProcedure);

            var flatPuestos = (await result.ReadAsync<PuestoModel>()).ToList();
            var status = await result.ReadSingleOrDefaultAsync<StoredProcedureResponse>();

            var hierarchy = BuildHierarchy(flatPuestos);

            return Ok(new { Data = hierarchy, Status = status });
        }

        private List<PuestoJerarquiaModel> BuildHierarchy(List<PuestoModel> flatList)
        {
            var lookup = new Dictionary<int, PuestoJerarquiaModel>();
            var rootNodes = new List<PuestoJerarquiaModel>();

            foreach (var item in flatList)
            {
                lookup[item.Codigo] = new PuestoJerarquiaModel
                {
                    Codigo = item.Codigo,
                    Puesto = item.Puesto,
                    Nombre = item.Nombre,
                    CodigoJefe = item.CodigoJefe,
                    Hijos = new List<PuestoJerarquiaModel>()
                };
            }

            foreach (var item in lookup.Values)
            {
                if (item.CodigoJefe == null)
                {
                    rootNodes.Add(item);
                }
                else if (lookup.ContainsKey(item.CodigoJefe.Value))
                {
                    lookup[item.CodigoJefe.Value].Hijos.Add(item);
                }
            }

            return rootNodes;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PuestoModel puesto)
        {
            using var connection = new SqlConnection(_connectionString);
            var response = await connection.QuerySingleOrDefaultAsync<StoredProcedureResponse>(
                "dbo.SP_M_Puestos",
                new 
                { 
                    Accion = "INSERT", 
                    Puesto = puesto.Puesto,
                    Nombre = puesto.Nombre,
                    CodigoJefe = puesto.CodigoJefe
                },
                commandType: CommandType.StoredProcedure);

            return response?.Status == 1 ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Update(int codigo, PuestoModel puesto)
        {
            using var connection = new SqlConnection(_connectionString);
            var response = await connection.QuerySingleOrDefaultAsync<StoredProcedureResponse>(
                "dbo.SP_M_Puestos",
                new 
                { 
                    Accion = "UPDATE", 
                    Codigo = codigo,
                    Puesto = puesto.Puesto,
                    Nombre = puesto.Nombre,
                    CodigoJefe = puesto.CodigoJefe
                },
                commandType: CommandType.StoredProcedure);

            return response?.Status == 1 ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(int codigo)
        {
            using var connection = new SqlConnection(_connectionString);
            var response = await connection.QuerySingleOrDefaultAsync<StoredProcedureResponse>(
                "dbo.SP_M_Puestos",
                new 
                { 
                    Accion = "DELETE", 
                    Codigo = codigo
                },
                commandType: CommandType.StoredProcedure);

            return response?.Status == 1 ? Ok(response) : BadRequest(response);
        }
    }
}
