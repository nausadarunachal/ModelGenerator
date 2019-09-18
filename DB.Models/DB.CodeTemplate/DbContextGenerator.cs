namespace DB.CodeTemplate
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DbContextGenerator
    {
        private static void AddGeneratedDisclaimer(
            StringBuilder sb)
        {
            sb.Append(TemplateConstants.GeneratedDisclaimer);
        }

        public static string Generate(
            string dbContextClassName,
            string dbContextNamespace,
            string modelNamespace,
            List<Table> tables)
        {
            var sb = new StringBuilder();
            AddGeneratedDisclaimer(sb);
            sb.Append(GetNamespace(
                dbContextClassName,
                dbContextNamespace,
                GetUsings(modelNamespace),
                tables));
            return sb.ToString();
        }

        private static string GetClass(
            string dbContextClassName,
            IReadOnlyCollection<Table> tables)
        {
            return
                $"\tpublic partial class {dbContextClassName} : DbContext\r\n" +
                "\t{\r\n" +
                GetRegionStaticProperties() +
                "\r\n" +
                GetRegionStaticConstructor(dbContextClassName) +
                "\r\n" +
                GetRegionInstanceProperties() +
                "\r\n" +
                GetRegionMemberDatasets(tables) +
                "\r\n" +
                GetRegionEfConfig(tables) +
                "\t}\r\n";
        }

        private static string GetNamespace(
            string dbContextClassName,
            string dbContextNamespace,
            string usings,
            IReadOnlyCollection<Table> tables)
        {
            return $"namespace {dbContextNamespace}\r\n" +
                "{\r\n" +
                usings + "\r\n" +
                GetClass(
                    dbContextClassName,
                    tables) +
                "}\r\n";
        }

        private static string GetRegionInstanceProperties()
        {
            return
                "\t\t#region instance properties\r\n\r\n" +
                "\t\tpublic int MetricId { get; set; }\r\n" +
                "\t\tpublic bool SkipLogging { get; set; }\r\n\r\n" +
                "\t\t#endregion\r\n";
        }

        private static string GetRegionEfConfig(
            IEnumerable<Table> tables)
        {
            var precisionColumns = tables.SelectMany(t => t.Columns.Select(c => new { TableGeneratedName = t.GeneratedName, Column = c }))
                .Where(x => x.Column.ClrType == "decimal" && x.Column.Scale > 2)
                .Where(x => x.TableGeneratedName != "MedispanModel")
                .ToList();
            var s =
                "\t\t#region Entity Framework configuration\r\n\r\n" +
                "\t\tprotected override void OnModelCreating(DbModelBuilder modelBuilder)\r\n" +
                "\t\t{\r\n" +
                string.Join("", precisionColumns
                .Select(c => $"\t\t\tmodelBuilder.Entity<{c.TableGeneratedName}>().Property(x => x.{c.Column.Name}).HasPrecision({c.Column.Precision}, {c.Column.Scale});\r\n")) +
                "\t\t\tbase.OnModelCreating(modelBuilder);\r\n" +
                "\t\t}\r\n\r\n" +
                "\t\t#endregion\r\n";
            return s;
        }

        private static string GetRegionMemberDatasets(
            IEnumerable<Table> tables)
        {
            var sb = new StringBuilder();
            sb.Append("\t\t#region member datasets\r\n\r\n");
            sb.Append(
                string.Concat(
                tables
                //TODO (maybe?) custom DbSet to intercept EF DbSet methods
                .Select(a => $"\t\tpublic DbSet<{a.GeneratedName}> {a.GeneratedName} {{ get; set; }}\r\n")));
            sb.Append("\r\n");
            sb.Append("\t\t#endregion\r\n");
            return sb.ToString();
        }

        private static string GetRegionStaticConstructor(
            string dbContextClassName)
        {
            return
                "\t\t#region static constructor\r\n\r\n" +
                $"\t\tstatic {dbContextClassName}()\r\n" +
                "\t\t{\r\n" +
                $"\t\t\tDatabase.SetInitializer<{dbContextClassName}>(null);\r\n" +
                $"\t\t\tLog = LogManager.GetLogger(typeof({dbContextClassName}));\r\n" +
                $"\t\t\tusing (var ctx = new {dbContextClassName}())\r\n" +
                "\t\t\t{\r\n" +
                "\t\t\t\tInteractiveViews\r\n" +
                "\t\t\t\t\t.SetViewCacheFactory(\r\n" +
                "\t\t\t\t\t\tctx,\r\n" +
                "\t\t\t\t\t\tnew SqlServerViewCacheFactory(ctx.Database.Connection.ConnectionString));\r\n" +
                "\t\t\t}\r\n" +
                "\t\t}\r\n\r\n" +
                "\t\t#endregion\r\n";
        }

        private static string GetRegionStaticProperties()
        {
            return
                "\t\t#region static members\r\n\r\n" +
                "\t\tpublic static ILog Log { get; set; }\r\n\r\n" +
                "\t\t#endregion\r\n";
        }

        private static string GetUsings(
            string modelNamespace)
        {
            return
                "\tusing InteractivePreGeneratedViews;\r\n" +
                "\tusing log4net;\r\n" +
                $"\tusing {modelNamespace};\r\n" +
                "\tusing System.Data.Entity;\r\n";
        }
    }
}
