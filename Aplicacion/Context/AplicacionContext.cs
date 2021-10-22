using API.Context;
using MySql.Data.Entity;
using System.Data.Entity;

namespace Aplicacion.Context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class AplicacionContext : DataContext
    {
    }
}