using SnapObjects.Data.Odbc;
using SnapObjects.Data;

namespace MainProject
{
    public class ODBCBionegociosDataContext : OdbcAseDataContext
    {
        public ODBCBionegociosDataContext(string connectionString)
                : this(new OdbcAseDataContextOptions<ODBCBionegociosDataContext>(connectionString))
        {

        }

        public ODBCBionegociosDataContext(IDataContextOptions<ODBCBionegociosDataContext> options)
            : base(options)
        {

        }

        public ODBCBionegociosDataContext(IDataContextOptions options)
            : base(options)
        {

        }
    }
}
