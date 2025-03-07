using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Win32;
using System.Web.WebPages;

namespace AgricolaDH_GApp.Services.Admin
{
    public class EgresoService
    {
        private readonly AppDbContext context;
        private readonly BlobStorageService _blobStorageService;

        public EgresoService(AppDbContext _ctx, BlobStorageService blobStorageService)
        {
            context = _ctx;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Mostrar datos de tabla principal de Almacen
        /// </summary>
        /// <returns></returns>
        public List<Egreso> SelectEgresos()
        {
            List<Egreso> egresosList;
            try
            {
                egresosList = context.Egresos.ToList();
            }
            catch
            {
                egresosList = new List<Egreso>();
            }
            return egresosList;
        }

        public void Generar(Egreso egreso)
        {
            try
            {
                Producto producto = context.Productos.FirstOrDefault(p => p.NombreProducto.Equals(egreso.Producto));
                Almacen registro = context.Almacen.FirstOrDefault(x => x.IdProducto.Equals(producto.IdProducto));
                int attempt = 0;// registro.Terminados - egreso.Cantidad;
                if (attempt < 0)
                {
                    throw new Exception();
                }
                //registro.Terminados -= egreso.Cantidad;
                egreso.Fecha = DateTime.Now;
                context.Egresos.Add(egreso);
                context.SaveChanges();
                egreso.IdEgreso = context.Egresos.OrderByDescending(x => x.IdEgreso).FirstOrDefault().IdEgreso;
                return;
            }
            catch
            {
                throw new Exception();
            }
        }

        public Egreso SelectEgreso(int IdEgreso)
        {
            Egreso egreso;
            try
            {
                egreso = context.Egresos.Find(IdEgreso);
            }
            catch
            {
                egreso = null;
            }
            return egreso;
        }

        public Producto SelectProductoByName(Egreso egreso)
        {
            Producto producto;
            try
            {
                producto = context.Productos.SingleOrDefault(a => a.NombreProducto == egreso.Producto);

            }
            catch
            {
                producto = null;
            }
            return producto;
        }

        public string UploadFile(Egreso egreso, string tipo)
        {
            string filename;
            filename = $"EgresoId{egreso.IdEgreso}_{DateTime.UtcNow:yyyyMMdd_HHmmss}_Evidencia{tipo}.jpg";
            IFormFile file = null;
            try
            {
                if (tipo.Equals("Antes"))
                {
                    file = egreso.EvidenciaAntesFile;
                }
                if (tipo.Equals("Despues"))
                {
                    file = egreso.EvidenciaDespuesFile;
                }

                _blobStorageService.UploadFileAsync(file, filename);
            }
            catch
            {
                filename = null;
            }
            return filename;
        }
    }
 }

