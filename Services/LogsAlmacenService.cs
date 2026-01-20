using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace AgricolaDH_GApp.Services
{
    public class LogsAlmacenService
    {
        private readonly AppDbContext context;

        public LogsAlmacenService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public void Entrada(AlmacenVM model)
        {
            try
            {
                LogsAlmacen log = new LogsAlmacen
                {
                    IdProducto = model.almacen.IdProducto,
                    SerialKey = model.almacen.SerialNumber,
                    Fecha = model.almacen.Fecha,
                    IdSolicitante = model.almacen.IdSolicitante,
                    IdAlmacenista = model.almacen.IdAlmacenista,
                    IdMovimiento = model.almacen.IdEstatus,
                };

                context.LogsAlmacen.Add(log);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar los logs de almacén: " + ex.Message, ex);
            }
        }
    }
}
