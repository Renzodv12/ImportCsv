using CsvHelper;
using CsvHelper.Configuration;
using importcsvtest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Globalization;

namespace importcsvtest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache; 
        }

        public IActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //public List<csv> Index(IFormFile stream)
        //{
        //    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //    {
        //        HasHeaderRecord = true,
        //        Delimiter = ";",
        //        IgnoreReferences = true,
        //    };
        //    var reader = new StreamReader(stream.OpenReadStream());
        //    var parser = new CsvParser(reader, config);
        //    var csv2 = new CsvReader(parser);
        //    var records = new List<csv>();

        //    // csv2.Read();
        //    //  csv2.ReadHeader();
     
        //    //records = csv2.GetRecords<csv>().ToList();
        //    while (csv2.Read()) {
        //        csv2.ReadHeader();
        //        string[] headerRow = csv2.Context.Reader.HeaderRecord;
        //        csv2.Context.RegisterClassMap<csvMap>();
        //        var record = csv2.GetRecord<csv>();
        //       // var record2 = csv2.GetField(0);
        //        // Do something with the record.
        //        records.Add(record);

        //    }
            

        //    return records;
        //}

        public sealed class csvMap : ClassMap<csv>
        {
            public csvMap()
            {
                Map(m => m.sku).Name("sku");
              /*  Map(m => m.peso).Name("peso");
                Map(m => m.alltura).Name("alltura");
                Map(m => m.ancho).Name("ancho");
                Map(m => m.nombre).Name("nombre");
                Map(m => m.descripcionc).Name("descripcionc");
                Map(m => m.descripcionlarga).Name("descripcionlarga");*/
             
            }
        }

        [HttpPost]
        public IActionResult Index(IFormFile File)
        {
            var Importsproducts = new List<importproduct>();
            var products = new List<products>();
            int linea =0;
            try
            {

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ";",
                    IgnoreReferences = true,
                };
                var reader = new StreamReader(File.OpenReadStream());
                var parser = new CsvParser(reader, config);
                var import = new CsvReader(parser);

                Importsproducts = ImportCsvHlper(import);
                
                 foreach(var item in Importsproducts)
                {
                    bool validar =  Validation(item, linea);
                    linea++;
                    if (validar)
                    {
                        continue;
                    }
                    var aux = new products()
                    {
                        Nombre= item.Nombre,
                        Altura= double.Parse(item.Altura),
                        Anchura = double.Parse( item.Anchura),
                        DescripcionBreve =item.DescripcionBreve,
                        DescripcionCompleta=item.DescripcionCompleta,
                        Longitud = double.Parse(item.Longitud),
                        Peso = double.Parse(item.Peso),
                        PrecioCatalogo = double.Parse(item.PrecioCatalogo),
                        PrecioContinental = double.Parse(item.PrecioContinental),
                        Stock=int.Parse(item.Stock),
                        Color1 = item.Color1,
                        Color2 = item.Color2,
                        Color3 = item.Color3,
                        Imagen1 = item.Imagen1,
                        Imagen2 = item.Imagen2,
                        Imagen3 = item.Imagen3
                    };
                    if (!string.IsNullOrEmpty(item.Caracteristicas))
                    {
                        List<string> crs = item.Caracteristicas.Split("&").ToList();
                        var auxcas = new List<productscaracteristicas>();

                        foreach (var c in crs)
                        {
                            var auxca = new productscaracteristicas();
                            List<string> cr = c.Split(":").ToList();
                            auxca.Title = cr[0];
                            auxca.Value = cr[1];
                            auxcas.Add(auxca);
                        }
                        aux.Caracteristicas = auxcas;
                    }
                    products.Add(aux);
                }


                 return Json(products);

            }
            catch (Exception e)
            {
                throw new Exception("Hubo un error", e);
            }
        }
        public sealed class productMap : ClassMap<importproduct>
        {
            public productMap()
            {
                Map(m => m.CodigoProducto).Name("Codigo Producto");
                Map(m => m.Nombre).Name("Nombre");
                Map(m => m.DescripcionBreve).Name("Descripcion Breve");
                Map(m => m.DescripcionCompleta).Name("Descripcion Completa");
                Map(m => m.PrecioCatalogo).Name("Precio Catalogo");
                Map(m => m.PrecioContinental).Name("Precio Continental");
                Map(m => m.Stock).Name("Stock");
                Map(m => m.Peso).Name("Peso");
                Map(m => m.Longitud).Name("Longitud");
                Map(m => m.Altura).Name("Altura");
                Map(m => m.Anchura).Name("Anchura");
                Map(m => m.Caracteristicas).Name("Caracteristicas");
                Map(m => m.Color1).Name("Color 1");
                Map(m => m.Color2).Name("Color 2");
                Map(m => m.Color3).Name("Color 3");
                Map(m => m.Imagen1).Name("Imagen 1");
                Map(m => m.Imagen2).Name("Imagen 2");
                Map(m => m.Imagen3).Name("Imagen 3");
                

            }
        }
        
        public virtual  List<importproduct> ImportCsvHlper(CsvReader _import)
        {
            var products = new List<importproduct>();
            while (_import.Read())
            {
               // _import.ReadHeader();
                _import.Context.RegisterClassMap<productMap>();
                var record = _import.GetRecord<importproduct>();
                products.Add(record);

            }
            return products;

        }

        public virtual  bool Validation(importproduct Product, int linea)
        {
            var productvalidation =_memoryCache.Get<List<importsError>>("importsErrors");
            var validation = new importsError();
            bool error = false;
             if(productvalidation==null)
            {
                productvalidation = new List<importsError>();
            }

            if (string.IsNullOrEmpty(Product.CodigoProducto))
            {
                validation.Linea = linea;
                validation.Message = validation.Message +  "Producto no puede ser cargado porque falta Codigo de Producto"+ "\n" ; 
                error = true;
            }
            if (string.IsNullOrEmpty(Product.Nombre))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "Producto no puede ser cargado porque falta Codigo de Nombre" + "\n";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.PrecioCatalogo))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Precio Catalogo";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.PrecioContinental))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Precio Continental";
                error = true;
            }

            if (string.IsNullOrEmpty(Product.DescripcionBreve))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta Descripcion Breve";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.DescripcionCompleta))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Descripcion Completa";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.Peso))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Peso";
                error = true;
            }

            if (string.IsNullOrEmpty(Product.Altura))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Altura";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.Anchura))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Anchura";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.Longitud))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Longitud";
                error = true;
            }
            if (string.IsNullOrEmpty(Product.Stock))
            {
                validation.Linea = linea;
                validation.Message = validation.Message + "\n" + "Producto no puede ser cargado porque falta  Stock";
                error = true;
            }
            if(validation != null)
            {
                productvalidation.Add(validation);
                _memoryCache.Set<List<importsError>>("importsErrors", productvalidation);
            }
            return error;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}