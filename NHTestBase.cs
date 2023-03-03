using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (Configuration == null)
            {
                var mapper = new ModelMapper();
                mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
                HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

                Configuration = new NHibernate.Cfg.Configuration();
                string connectionString = NibernateAditionalFields.Properties.Settings.Default.ConnectionString;
                SetupForSQLServer(connectionString);
                Configuration.AddAssembly(Assembly.GetExecutingAssembly());

            }

            //foreach (var mapping in Configuration.ClassMappings)
            //{
            //    foreach (var join in mapping.JoinIterator)
            //    {
            //        var table = join.Table;
            //        if (table != null && table.Name.EndsWith("_Custom"))
            //        {
            //            foreach (var property in join.PropertyIterator)
            //            {
            //                if (property.Name == "Additional")
            //                {
            //                    var component = property.Value as NHibernate.Mapping.Component;
            //                    NHibernate.Mapping.Property field = new NHibernate.Mapping.Property();
            //                    field.Name = "Company";
                                
            //                    component.AddProperty(field);
            //                }
            //            }
            //        }
            //    }
            //}

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

        private void SetupForSQLLite(string dbName)
        {
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), dbName);
            var conString = $"Data Source ={dbPath};Version=3;New=True;Compress=True;";

            Configuration.DataBaseIntegration(x =>
            {
                x.Driver<SQLite20Driver>();
                x.Dialect<SQLiteDialect>();
                x.ConnectionProvider<DriverConnectionProvider>();
                x.ConnectionString = conString;
                x.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                x.Timeout = 255;
                x.BatchSize = 100;
                x.LogFormattedSql = true;
                x.LogSqlInConsole = true;
                x.AutoCommentSql = false;
                x.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
            });
        }
    }
}
