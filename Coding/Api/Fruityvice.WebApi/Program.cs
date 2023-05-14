namespace Fruityvice.WebApi
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.

            builder.Services.AddControllers();

            string executingAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            FileInfo[] Files = new DirectoryInfo(executingAssemblyPath).GetFiles("Fruityvice*.dll"); //Getting dll files start with Fruityvice

            List<System.Reflection.Assembly> assemblies = new List<System.Reflection.Assembly>();
            foreach (FileInfo file in Files)
            {
                try
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(file.Name.Replace(file.Extension, string.Empty));
                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                    ;
                }
            }

            //// Add NH services to the container.
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration().Configure();
            cfg.CollectionTypeFactory<NHibernate.Type.DefaultCollectionTypeFactory>();
            cfg.LinqToHqlGeneratorsRegistry<NHibernate.Linq.Functions.DefaultLinqToHqlGeneratorsRegistry>();

            NHibernate.Mapping.ByCode.ModelMapper modelMapper = new NHibernate.Mapping.ByCode.ModelMapper();
            List<Type> nhMappings = Base.Utility.GetAllMappings(assemblies);
            modelMapper.AddMappings(nhMappings);

            cfg.AddMapping(modelMapper.CompileMappingForAllExplicitlyAddedEntities());
            NHibernate.ISessionFactory sessionFactory = cfg.BuildSessionFactory();
            NHibernate.ISession session = sessionFactory.OpenSession();

            NHibernate.Tool.hbm2ddl.SchemaExport schemaExport = new NHibernate.Tool.hbm2ddl.SchemaExport(cfg);
            schemaExport.Execute(useStdOut: true, execute: true, justDrop: false);

            builder.Services.AddSingleton(sessionFactory);
            builder.Services.AddScoped<NHibernate.ISession>(s => session);

            //// Add DI services to the container.
            List<Base.DependencyInjection.AppServices> diServices = new List<Base.DependencyInjection.AppServices>();
            diServices.AddRange(Base.DependencyInjection.DIServicesExtensions.GetAllService(assemblies));

            foreach (Base.DependencyInjection.AppServices appService in diServices)
            {
                foreach (System.Type serviceType in appService.ServiceTypes)
                {
                    builder.Services.AddTransient(serviceType, appService.ImplementationType);
                }
            }

            builder.Services.AddTransient(typeof(Base.IDefaultEntityRepository<>), typeof(Base.DefaultEntityRepository<>));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}