﻿using API.Models.Reportes;
using API.Operaciones.OperacionesProveedores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API.Models.DocumentosRecibidos
{
    public class DocumentosRecibidosModel : FechasModel
    {
        public int SucursalId { get; set; }

        public virtual List<API.Operaciones.OperacionesProveedores.DocumentoRecibido> DocumentosRecibidos { get; set; }

        public virtual List<API.Operaciones.OperacionesProveedores.DocumentoRecibido> DocumentosRecibidosAsignados { get; set; }

        public bool isProveedor { get; set; }

    }

}