using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NibernateAditionalFields
{
    public abstract class NHTestBase
    {
        private ISessionFactory _sessionFactory;

        public ISessionFactory SessionFactory
        {
            get 
            { 
                return _sessionFactory ?? (_sessionFactory = CreateSessionFactory()); 
            }
        }

        public NHibernate.Cfg.Configuration Configuration { get; private set; }


        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }


        private ISessionFactory CreateSessionFactory()
        {
            string connectionString = NibernateAditionalFields.Properties.Settings.Default.ConnectionString;
            if (Configuration == null)
            {
                var mapper = new ModelMapper();
                mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
                HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

                Configuration = new NHibernate.Cfg.Configuration();
                SetupForSQLServer(connectionString);
                Configuration.AddAssembly(Assembly.GetExecutingAssembly());

            }

            foreach (var mapping in Configuration.ClassMappings)
            {
                foreach (var join in mapping.JoinIterator)
                {
                    var table = join.Table;
                    if (table != null && table.Name.EndsWith("_Custom"))
                    {
                        foreach (var property in join.PropertyIterator)
                        {
                            if (property.Name == "Additional")
                            {
                                var component = property.Value as NHibernate.Mapping.Component;
                                var columns = GetColumns(table.Name, connectionString);
                                foreach (string name in columns.Keys)
                                {
                                    SimpleValue value = new SimpleValue(table);
                                    value.AddColumn(new Column(name));
                                    value.TypeName = MapType(columns[name]);
                                    NHibernate.Mapping.Property field = new NHibernate.Mapping.Property(value);
                                    field.Name = name;
                                    component.AddProperty(field);
                                }
                            }
                        }
                    }
                }
            }

            return Configuration.BuildSessionFactory();
        }

        private void SetupForSQLServer(string connectionString) 
        {
            Configuration.DataBaseIntegration(x =>
            {
                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2012Dialect>();
                x.ConnectionProvider<DriverConnectionProvider>();
                x.ConnectionString = connectionString;
                x.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                x.Timeout = 255;
                x.BatchSize = 100;
                x.LogFormattedSql = true;
                x.LogSqlInConsole = true;
                x.AutoCommentSql = false;
                x.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
            });
        }

        private Dictionary<string, string> GetColumns(string tableName, string connectionString) 
        {
            Dictionary<string,string> columns = new Dictionary<string, string>();
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                string query = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}'";
                SqlCommand command = new SqlCommand(query, connection);
                using (var reader = command.ExecuteReader()) 
                {
                    int index = 0;
                    while (reader.Read())
                    {
                        if (index++ == 0)
                            continue;
                        string name = reader["COLUMN_NAME"] as string;
                        string type = reader["DATA_TYPE"] as string;
                        columns.Add(name, type);                    
                    }
                    reader.Close();
                }
            }
            catch
            {
            }
            finally 
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return columns;
        }

        private string MapType(string dbType) 
        {
            // very basic type map as proof of concept
            var typeMap = new Dictionary<string, string>()
            {
                { "nvarchar", NHibernateUtil.String.Name },
                { "bit", NHibernateUtil.Boolean.Name },
                { "int", NHibernateUtil.Int32.Name },
            };

            return typeMap[dbType.ToLower()];
        }
    }
}
