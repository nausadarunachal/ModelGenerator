namespace DB.CodeTemplate
{
    using EnvDTE;
    using System;
    using System.IO;
    using System.Linq;

    // ReSharper disable once UnusedMember.Global
    public static class Template
    {
        // ReSharper disable once UnusedMember.Global
        public static void Run(IServiceProvider host)
        {
            var statusBar = VsUtil.GetStatusBar(host);
            statusBar.Text = "Getting solution...";
            var solution = VsUtil.GetSolution(host);
            // ConnectionString
            statusBar.Text = "Getting connection string project...";
            var connectionStringProject = VsUtil.GetProjectBySolutionPath(
                solution,
                TemplateConstants.ConnectionStringProject);
            statusBar.Text = "Computing connection string path...";
            var connectionStringPath = Path.Combine(
                Path.GetDirectoryName(connectionStringProject.FileName)
                ?? throw new InvalidOperationException(),
                TemplateConstants.ConnectionStringProjectRelativePath);
            statusBar.Text = "Getting connection string...";
            var connectionStringConfigPath = Path.Combine(connectionStringPath,
                TemplateConstants.ConnectionStringFileName);
            var connStr = VsUtil.GetAppSetting(
                connectionStringConfigPath,
                TemplateConstants.ConnectionStringAppSetting);
            // DAL
            statusBar.Text = "Getting DbContext project...";
            var dbContextProject = VsUtil.GetProjectBySolutionPath(
                solution,
                TemplateConstants.DbContextProject);
            statusBar.Text = "Computing DbContext path...";
            var dbContextPath = Path.Combine(
                Path.GetDirectoryName(dbContextProject.FileName)
                ?? throw new InvalidOperationException(),
                TemplateConstants.DbContextProjectRelativePath,
                TemplateConstants.DbContextClassName + ".cs");
            // Models
            statusBar.Text = "Getting models project...";
            var modelsProject = VsUtil.GetProjectBySolutionPath(
                solution,
                TemplateConstants.ModelsProject);
            statusBar.Text = "Computing models path...";
            var modelsPath = Path.Combine(
                Path.GetDirectoryName(modelsProject.FileName)
                ?? throw new InvalidOperationException(),
                TemplateConstants.ModelsProjectRelativePath);
            statusBar.Text = "Getting schema dataset...";
            var ds = DbUtil.GetSchemaDataSet(connStr);
            statusBar.Text = "Modeling table objects...";
            var tables = TableGenerator.GenerateAll(ds)
                .OrderBy(a => a.GeneratedName)
                .ToList();
            // DAL (DbContext)
            statusBar.Text = "Creating DbContext class...";
            var content = DbContextGenerator.Generate(
                TemplateConstants.DbContextClassName,
                TemplateConstants.DbContextNamespace,
                TemplateConstants.ModelsNamespaceDbContext,
                tables);
            var path = dbContextPath;
            File.WriteAllText(path, content);
            dbContextProject.ProjectItems.AddFromFile(path);
            // Models
            foreach (ProjectItem projectItem in modelsProject.ProjectItems)
            {
                var fileName = projectItem.FileNames[0];
                var dirName = Path.GetDirectoryName(fileName);
                if (dirName != modelsPath) continue;
                foreach (ProjectItem modelsDbItem in projectItem.ProjectItems)
                {
                    var modelsDbItemFileName = modelsDbItem.FileNames[0];
                    var modelsDbItemKind = modelsDbItem.Kind;
                    var modelsDbItemName = modelsDbItem.Name;
                    if (modelsDbItemKind != "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}"
                        || !modelsDbItemFileName
                            .EndsWith(
                                $"{TemplateConstants.EntitySuffix}.cs"))
                    {
                        continue;
                    }
                    statusBar.Text = $"Deleting {modelsDbItemName}";
                    modelsDbItem.Remove();
                    File.Delete(modelsDbItemFileName);
                }
            }
            var tableCount = tables.Count;
            var index = 1;
            tables
                .ForEach(a =>
                {
                    var percent = index * 100 / tableCount;
                    statusBar.Text = "Creating class file for table " +
                                     a.TableName + ", " + index + " of " + tableCount + ", " + percent + "%...";
                    content = TableGenerator.GenerateTableClassFileContent(a,
                        TemplateConstants.ModelsNamespace);
                    path = Path.Combine(modelsPath, a.GeneratedName + ".cs");
                    File.WriteAllText(path, content);
                    modelsProject.ProjectItems.AddFromFile(path);
                    index++;
                });
            statusBar.Text = "Generation complete";
        }
    }
}
