namespace DB.CodeTemplate
{
    using EnvDTE;
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public static class VsUtil
    {
        // get app setting from config file
        public static string GetAppSetting(
            string configFilePath,
            string appSettingName)
        {
            var doc = new XmlDocument();
            if (!File.Exists(configFilePath))
            {
                return null;
            }
            doc.Load(configFilePath);
            // if file attribute on appsettings node exists,
            // check file for app setting
            var fileNode = doc.SelectSingleNode("//appSettings/@file");
            if (fileNode != null)
            {
                var value = GetAppSetting(
                    Path.Combine(
                        Path.GetDirectoryName(configFilePath)
                        ?? throw new InvalidOperationException(),
                        fileNode.Value),
                    appSettingName);
                if (value != null)
                {
                    return value;
                }
            }
            var appSettingValueNode =
                doc.SelectSingleNode(
                "//appSettings/add[@key=\""
                + appSettingName + "\"]/@value");
            return appSettingValueNode?.Value;
        }

        private static object GetObjectBySolutionPath(
            Solution solution,
            string solutionPath)
        {
            object item = solution;
            // split solution path
            var parts = solutionPath.Split(new[] { '/' },
                StringSplitOptions.RemoveEmptyEntries);
            // for each path, get matching project item by name
            while (parts.Length > 0)
            {
                switch (item)
                {
                    case Solution _:
                        item = GetSolutionItemByName(
                            item as Solution, parts[0]);
                        break;
                    case Project _:
                        item = GetProjectItemFromProjectByName(
                            item as Project, parts[0]);
                        break;
                    case ProjectItem _:
                        item = GetProjectItemFromProjectItemByName(
                            (ProjectItem) item, parts[0]);
                        break;
                }

                parts = parts.Skip(1).ToArray();
            }
            return item;
        }


        public static Project GetProjectBySolutionPath(
            Solution solution,
            string solutionPath)
        {
            var retval = GetObjectBySolutionPath(
                solution, solutionPath);
            return retval is Project project
                ? project
                : retval is ProjectItem item
                ? item.SubProject
                : null;
        }

        private static ProjectItem GetProjectItemFromProjectByName(
            Project project,
            string itemName)
        {
            return project.ProjectItems
                .Cast<object>()
                .OfType<ProjectItem>()
                .FirstOrDefault(a => string.Equals(
                    a.Name,
                    itemName,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        private static ProjectItem GetProjectItemFromProjectItemByName(
            ProjectItem projectItem,
            string itemName)
        {
            return projectItem.SubProject.ProjectItems
                .Cast<object>()
                .OfType<ProjectItem>()
                .FirstOrDefault(a => string.Equals(
                    a.Name,
                    itemName,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        public static Solution GetSolution(IServiceProvider host)
        {
            var dte = (DTE)host.GetService(typeof(DTE));
            return dte.Solution;
        }

        private static Project GetSolutionItemByName(
            _Solution solution,
            string itemName)
        {
            return solution.Projects
                .Cast<object>()
                .OfType<Project>()
                .FirstOrDefault(a => string.Equals(
                    a.Name,
                    itemName,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        public static StatusBar GetStatusBar(IServiceProvider host)
        {
            var dte = (DTE)host.GetService(typeof(DTE));
            return dte.StatusBar;
        }
    }
}
