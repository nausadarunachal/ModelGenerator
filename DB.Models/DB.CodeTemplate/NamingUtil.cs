namespace DB.CodeTemplate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class NamingUtil
    {
        private static readonly Dictionary<string, string> ColumnNameMappings;
        private static readonly Dictionary<string, string> DisplayNameMappings;
        private static readonly Dictionary<string, string> DisplayNameWordMappings;
        private static readonly Dictionary<string, string> TableNameMappings;

        static NamingUtil()
        {
            ColumnNameMappings = new Dictionary<string, string>();
            DisplayNameMappings = new Dictionary<string, string>();
            DisplayNameWordMappings = new Dictionary<string, string>();
            TableNameMappings = new Dictionary<string, string>();
            TemplateConstants.DbColumnNameMappings
                .Split(',')
                .ToList()
                .ForEach(a =>
                {
                    var parts = a.Split(':');
                    ColumnNameMappings.Add(
                        parts[0].Trim(),
                        parts[1].Trim());
                });
            var mappings = TemplateConstants
                .DisplayNameMappings
                .Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < mappings.Length; i += 2)
            {
                DisplayNameMappings.Add(
                    mappings[i].Trim(),
                    mappings[i + 1].Trim());
            }
            mappings = TemplateConstants
                .DisplayNameWordMappings
                .Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < mappings.Length; i += 2)
            {
                DisplayNameWordMappings.Add(
                    mappings[i].Trim(),
                    mappings[i + 1].Trim());
            }
            mappings = TemplateConstants
                .TableNameMappings
                .Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < mappings.Length; i += 2)
            {
                TableNameMappings.Add(
                    mappings[i].Trim(),
                    mappings[i + 1].Trim());
            }
        }

        public static string GetColumnName(
            string tableName,
            string originalColumnName)
        {
            if (ColumnNameMappings.TryGetValue(
                tableName + "." + originalColumnName,
                out var mappedName))
            {
                return mappedName;
            }
            // remove prefix if in list
            var newName = Regex.Replace(
                originalColumnName,
                "^("
                + TemplateConstants
                    .TableNamePrefixesToRemove
                    .Replace(",", "|")
                + ")",
                "");
            // remove every space
            newName = Regex.Replace(
                newName,
                "\\s",
                "");
            // change every initial lower case character to
            // an upper case character
            newName = Regex.Replace(
                newName,
                "^[a-z]",
                a => a.Groups[0].Value.ToUpper());
            // change every _+([a-zA-Z]) to ($1).ToUpper()
            newName = Regex.Replace(
                newName,
                "_+([a-zA-Z])",
                a => a.Groups[1].Value.ToUpper());
            // normal suffix "Id"
            newName = newName.ToLower().EndsWith("id")
                && !newName.ToLower().EndsWith("guid")
                && !newName.ToLower().EndsWith("paid")
                ? newName.Substring(0, newName.Length - 2) + "Id"
                : newName;
            return newName;
        }

        public static string GetDisplayName(string originalName)
        {
            if (DisplayNameMappings.TryGetValue(originalName, out var mappedName))
            {
                return mappedName;
            }
            // add spaces between word boundaries
            var newName = Regex.Replace(
                originalName,
                "([a-z])([A-Z])",
                "$1 $2");
            // split newName into words
            var words = newName.Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            // for each word, replace with mapping if present
            for (var i = 0; i < words.Length; i++)
            {
                if (DisplayNameWordMappings.TryGetValue(words[i], out mappedName))
                {
                    words[i] = mappedName;
                }
            }
            newName = string.Join(" ", words);
            return newName;
        }

        public static string GetTableName(string originalName)
        {
            if (TableNameMappings.TryGetValue(originalName, out var mappedName))
            {
                return mappedName;
            }
            // remove prefix if in list
            var newName = Regex.Replace(
                originalName,
                "^("
                + TemplateConstants
                    .TableNamePrefixesToRemove
                    .Replace(",", "|")
                + ")",
                "");
            // remove every space
            newName = Regex.Replace(
                newName,
                "\\s",
                "");
            // change every initial lower case character to
            // an upper case character
            newName = Regex.Replace(
                newName,
                "^[a-z]",
                a => a.Groups[0].Value.ToUpper());
            // change every _+([a-zA-Z]) to ($1).ToUpper()
            newName = Regex.Replace(
                newName,
                "_+([a-zA-Z])",
                a => a.Groups[1].Value.ToUpper());
            // normal suffix "Id"
            newName = newName.ToLower().EndsWith("id")
                && !newName.ToLower().EndsWith("guid")
                && !newName.ToLower().EndsWith("paid")
                ? newName.Substring(0, newName.Length - 2) + "Id"
                : newName;
            return newName + TemplateConstants.EntitySuffix;
        }
    }
}
