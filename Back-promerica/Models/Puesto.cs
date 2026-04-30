namespace Back_promerica.Models
{
    public class PuestoModel
    {
        public int Codigo { get; set; }
        public string Puesto { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int? CodigoJefe { get; set; }
    }

    public class PuestoJerarquiaModel
    {
        public int Codigo { get; set; }
        public string Puesto { get; set; }
        public string Nombre { get; set; }
        public int? CodigoJefe { get; set; }
        public List<PuestoJerarquiaModel> Hijos { get; set; }
    }

    public class StoredProcedureResponse
    {
        public int Status { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
