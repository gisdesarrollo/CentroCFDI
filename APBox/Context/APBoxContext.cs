using API.Context;
using MySql.Data.EntityFramework;
using System.Data.Entity;

namespace APBox.Context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class APBoxContext : DataContext
    {
    }
}