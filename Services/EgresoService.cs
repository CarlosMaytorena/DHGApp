﻿using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Identity;
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

        public EgresoService(AppDbContext _ctx)
        {
            context = _ctx;
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

        public int Generar(Egreso egreso)
        {
            try
            {
                Producto producto = context.Productos.FirstOrDefault(p => p.NombreProducto.Equals(egreso.Producto));
                Almacen registro = context.Almacen.FirstOrDefault(x => x.IdProducto.Equals(producto.IdProducto));
                int attempt = registro.Terminados - egreso.Cantidad;
                if (attempt < 0)
                {
                    return -1;
                }
                registro.Terminados -= egreso.Cantidad;
                egreso.Fecha = DateTime.Now;
                context.Egresos.Add(egreso);
                context.SaveChanges();
                egreso.IdEgreso = context.Egresos.OrderByDescending(x => x.IdEgreso).FirstOrDefault().IdEgreso;
                return 1;
            }
            catch
            {
                return -1;
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
            string path;
            try
            {
                if (tipo.Equals("antes")){
                    tipo = "Antes";
                }
                if (tipo.Equals("despues")){
                    tipo = "Despues";
                }
                // Generate a new file name with date and time
                path  = $"Evidencia{tipo}_Id{egreso.IdEgreso}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg";
            }
            catch
            {
                path = null;
            }
            //return blobClient.Uri.ToString(); // Return the URL of the uploaded blob
            return path;
        }

    }
}
