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

                //Extra Fields
                foreach(Egreso item in egresosList)
                {
                    item.Producto = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(item.IdProducto)).NombreProducto;
                    item.Solicitante = context.Usuarios.FirstOrDefault(x => x.IdUsuario.Equals(item.IdSolicitante)).Username;
                    //item.SerialNumber = context.Almacen.FirstOrDefault(x => x.IdAlmacen.Equals(item.IdAlmacen)).SerialNumber;
                    item.PathAntes = context.Evidencia.FirstOrDefault(x => x.IdEvidencia.Equals(item.IdEvidencia)).PathAntes;
                    item.PathDespues = context.Evidencia.FirstOrDefault(x => x.IdEvidencia.Equals(item.IdEvidencia)).PathDespues;
                }
            }
            catch
            {
                egresosList = new List<Egreso>();
            }
            return egresosList;
        }

        public void Generar(EgresosVM model, int IdLogsEgresos)
        {
            List<Egreso>egresos = new List<Egreso>();
            var serialNumbers = model.almacenLista.Select(a => a.SerialNumber).ToList();
            foreach (Almacen a in model.almacenLista)
            {
                egresos.Add(new Egreso
                {
                    IdAlmacen = a.IdAlmacen,
                    IdProducto = a.IdProducto,
                    Fecha = DateTime.Now,
                    SerialNumber = a.SerialNumber,
                    IdEvidencia = model.egreso.IdEvidencia,
                    IdSolicitante = model.egreso.IdSolicitante,
                    PathAntes = model.egreso.PathAntes,
                    PathDespues = model.egreso.PathDespues,
                    IdLogsEgresos = IdLogsEgresos

                });
            }
            context.Egresos.AddRange(egresos);

            // Eliminar todos los registros de Almacen en bloque
            var almacenes = context.Almacen.Where(x => serialNumbers.Contains(x.SerialNumber)).ToList();
            context.Almacen.RemoveRange(almacenes);

            // Actualizar evidencia
            var evidencia = context.Evidencia.FirstOrDefault(x => x.IdEvidencia == model.egreso.IdEvidencia);
            evidencia.PathAntes = model.egreso.PathAntes;
            evidencia.PathDespues = model.egreso.PathDespues;
            context.Evidencia.Update(evidencia);
            context.SaveChanges();
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

        public Producto SelectProductoByName(string nombre)
        {
            Producto producto;
            try
            {
                producto = context.Productos.SingleOrDefault(a => a.NombreProducto == nombre);

            }
            catch
            {
                producto = null;
            }
            return new Producto();
        }

        public string UploadFile(EgresosVM model, string tipo)
        {
            string filename;
            IFormFile file = null;
            try
            {
                Evidencia e = context.Evidencia.SingleOrDefault(x => x.IdEvidencia.Equals(model.egreso.IdEvidencia));

                if (tipo.Equals("Antes"))
                    file = model.egreso.FileAntes;
                if (tipo.Equals("Despues"))
                    file = model.egreso.FileDespues;

                filename = $"{e.IdEvidencia}_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{tipo}{Path.GetExtension(file.FileName)}";
                _blobStorageService.UploadFileAsync(file, filename);
                
                return filename;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
 }

