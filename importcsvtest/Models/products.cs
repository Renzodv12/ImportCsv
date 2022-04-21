using System.ComponentModel.DataAnnotations;

namespace importcsvtest.Models
{
    public class products
    {

            public products()
            {
                Caracteristicas = new List<productscaracteristicas>();
            }
            public string? CodigoProducto { get; set; }

            public string? Nombre { get; set; }
            public string? DescripcionBreve { get; set; }
            public string? DescripcionCompleta { get; set; }
            public double PrecioCatalogo { get; set; }
            public double PrecioContinental { get; set; }
            public int Stock { get; set; }
            public double Peso { get; set; }
            public double Longitud { get; set; }
            public double Anchura { get; set; }
            public double Altura { get; set; }
            public List<productscaracteristicas>? Caracteristicas { get; set; }
            public string? Color1 { get; set; }
            public string? Color2 { get; set; }
            public string? Color3 { get; set; }
            public string? Imagen1 { get; set; }
            public string? Imagen2 { get; set; }
            public string? Imagen3 { get; set; }
        }
        public class productscaracteristicas
            {
                public string? Title { get; set; }
                public string? Value { get; set; }
             }
       public class importproduct
    {
        public string? CodigoProducto { get; set; }
        public string? Nombre { get; set; }
        public string? DescripcionBreve { get; set; }
        public string? DescripcionCompleta { get; set; }
        public string? PrecioCatalogo { get; set; }
        public string? PrecioContinental { get; set; }
        public string? Stock { get; set; }
        public string? Peso { get; set; }
        public string? Longitud { get; set; }
        public string? Anchura { get; set; }
        public string? Altura { get; set; }
        public string? Caracteristicas { get; set; }
        public string? Color1 { get; set; }
        public string? Color2 { get; set; }
        public string? Color3 { get; set; }
        public string? Imagen1 { get; set; }
        public string? Imagen2 { get; set; }
        public string? Imagen3 { get; set; }
    }
    public class importsError
    {
        public int Linea { get; set; }
        public string Message { get; set; }
    }
}


