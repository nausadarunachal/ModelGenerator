namespace DB.CodeTemplate
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class TableGenerator
    {

        #region public static members

        private static readonly Dictionary<string, string> CreateDateFieldNames;
        private static readonly Dictionary<string, string> DestNavPropNames;
        private static readonly Dictionary<string, string> EditableColumns;
        private static readonly Dictionary<string, string> StartDateFieldNames;
        private static readonly Dictionary<string, string> EndDateFieldNames;
        private static readonly Dictionary<string, string> DeactivatedDateFieldNames;
        private static readonly Dictionary<string, string> ExcludePopulateDatesInterface;
        private static readonly Dictionary<string, string> ExcludeFieldsFromModelTable;
        private static readonly Dictionary<string, string> ModelsSubclassEffDateExcludedColumns;
        private static readonly Dictionary<string, string> ModifiedDateFieldNames;
        private static readonly Dictionary<string, string> NavPropDisplayNames;
        private static readonly Dictionary<string, string> NavPropsToSkip;
        private static readonly Dictionary<string, string> NewKeywordColumns;
        private static readonly Dictionary<string, string> OverrideColumns;
        private static readonly Dictionary<string, string> SourceNavPropNames;
        private static readonly Dictionary<string, string> TablesToOptOutOfSubClass;
        private static readonly Dictionary<string, string> IsActiveFieldNames;

        #endregion

        static TableGenerator()
        {
            CreateDateFieldNames = TemplateConstants.CreateDateFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            DestNavPropNames = new Dictionary<string, string>();
            TemplateConstants.DestinationNavigationPropertyNames
                .Split(',')
                .ToList()
                .ForEach(a =>
                {
                    var parts = a.Split(':');
                    DestNavPropNames.Add(
                        parts[0].Trim(),
                        parts[1].Trim());
                });
            EditableColumns = TemplateConstants.EditableColumns
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .Select(a => a.Trim())
                .ToDictionary(a => a, a => a);
            EndDateFieldNames = TemplateConstants.EndDateFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            ExcludePopulateDatesInterface = TemplateConstants.ExcludePopulateDatesInterface
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            ExcludeFieldsFromModelTable = TemplateConstants.ExcludeFieldsFromModelTable
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .Select(a => a.Trim())
                .ToDictionary(a => a, a => a);
            ModelsSubclassEffDateExcludedColumns = TemplateConstants.ModelsSubclassEffDateExcludedColumns
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .Select(a => a.Trim())
                .ToDictionary(a => a, a => a);
            ModifiedDateFieldNames = TemplateConstants.ModifiedDateFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            NavPropDisplayNames = new Dictionary<string, string>();
            TemplateConstants.NavigationPropertyDisplayNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(a =>
                {
                    var parts = a.Split(':');
                    NavPropDisplayNames.Add(
                        parts[0].Trim(),
                        parts.Length < 2
                        || string.IsNullOrWhiteSpace(parts[1])
                            ? NamingUtil.GetDisplayName(parts[0].Trim().Split('.')[1])
                            : parts[1].Trim());
                });
            NavPropsToSkip = TemplateConstants.NavigationPropertiesToSkip
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToDictionary(a => a);
            NewKeywordColumns = TemplateConstants.NewKeywordColumns
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToDictionary(a => a);
            OverrideColumns = TemplateConstants.OverrideColumns
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToDictionary(a => a);
            SourceNavPropNames = new Dictionary<string, string>();
            TemplateConstants.SourceNavigationPropertyNames
                .Split(',')
                .ToList()
                .ForEach(a =>
                {
                    var parts = a.Split(':');
                    SourceNavPropNames.Add(
                        parts[0].Trim(),
                        parts[1].Trim());
                });
            StartDateFieldNames = TemplateConstants.StartDateFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            IsActiveFieldNames = TemplateConstants.IsActiveFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            DeactivatedDateFieldNames = TemplateConstants.DeactivatedDateFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(a => a);
            TablesToOptOutOfSubClass = TemplateConstants.TablesToOptOutOfSubClass
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .Select(a => a.Trim())
                .ToDictionary(a => a);
        }

        private static void AddGeneratedDisclaimer(
            StringBuilder sb)
        {
            sb.Append(TemplateConstants.GeneratedDisclaimer);
        }

        private static void AddUsing(
            bool needed,
            StringBuilder sb,
            string usingContent)
        {
            if (needed)
            {
                sb.AppendFormat("using {0};\r\n", usingContent);
            }
        }

        public static IEnumerable<Table> GenerateAll(
            DataSet ds)
        {
            var tables = DbUtil.GetTables(
                ds.Tables["Columns"],
                ds.Tables["IndexColumns"],
                ds.Tables["Indexes"],
                ds.Tables["Schemas"],
                ds.Tables["Tables"]);
            tables
                .ForEach(a =>
                {
                    a.Columns = DbUtil.GetColumnsByTableId(
                        ds.Tables["Columns"],
                        ds.Tables["Types"],
                        ds.Tables["DefaultConstraints"],
                        a.ObjectId,
                        a.TableName);
                    a.ForeignKeys = DbUtil.GetForeignKeysByTableId(
                        ds.Tables["Columns"],
                        ds.Tables["ForeignKeys"],
                        ds.Tables["ForeignKeyColumns"],
                        ds.Tables["Tables"],
                        a.ObjectId,
                        a.TableName);
                    a.PrimaryKeyColumns = DbUtil.GetColumnsByTableIdColumnId(
                        ds.Tables["Columns"],
                        a.ObjectId,
                        DbUtil.GetIndexColumnsByTableIdIndexId(
                            ds.Tables["IndexColumns"],
                            a.ObjectId,
                            DbUtil.GetPrimaryKeyIndexIdByTableId(
                                ds.Tables["Indexes"],
                                a.ObjectId)));
                    a.PrimaryKeyColumns
                        .ForEach(b =>
                        {
                            if (string.IsNullOrWhiteSpace(b)) return;
                            var kcol = a.Columns
                                .FirstOrDefault(c => c.Name == b);
                            if (kcol != null)
                            {
                                kcol.IsKey = true;
                            }
                        });
                    a.ActiveColumnName =
                        DbUtil.HasTableActiveFlag(a)
                        ? DbUtil.GetTableActivityFieldName(a)
                        : null;
                    a.ActiveColumnValue =
                        DbUtil.HasTableActiveFlag(a)
                        && DbUtil.GetTableActivityFieldValue(a);
                });
            tables.ForEach(a =>
            {
                a.NavigationProperties
                    .AddRange(GenerateNavigationProperties(
                    ds.Tables["Columns"],
                    ds.Tables["ForeignKeyColumns"],
                    ds.Tables["ForeignKeys"],
                    ds.Tables["Tables"],
                    a,
                    tables));
            });
            return tables;
        }

        private static IEnumerable<NavigationProperty> GenerateNavigationProperties(
            DataTable columnsTable,
            DataTable foreignKeyColumnsTable,
            DataTable foreignKeysTable,
            DataTable tablesTable,
            Table table,
            IReadOnlyCollection<Table> tables)
        {
            var props = new List<NavigationProperty>();
            // get foreign keys
            var foreignKeys = DbUtil.GetForeignKeysByTableId(
                columnsTable,
                foreignKeysTable,
                foreignKeyColumnsTable,
                tablesTable,
                table.ObjectId,
                table.TableName);
            // for each foreign key,
            foreignKeys.ForEach(a =>
            {
                var isDestinationForeignKeyAllNonNullable =
                    IsDestinationNullable(a, columnsTable);
                var isSourceForeignKeyAlsoPrimaryKey =
                    IsSourcePrimaryKey(table, a);
                var type = EntityDesignUtil.Singularize(
                    NamingUtil.GetTableName(a.DestinationTableName));
                var srcCol = a.Columns[0].SourceColumn;
                var prop = new NavigationProperty
                {
                    DestinationMultiplicity =
                        // Zero: if any source foreign key columns are nullable
                        !isDestinationForeignKeyAllNonNullable
                        ? Multiplicity.Zero
                        // One: not Zero
                        : Multiplicity.One,
                    ForeignKey = a,
                    // *-1, 1-0/1
                    Name = SourceNavPropNames.TryGetValue(
                        a.SourceTableName + "." + string.Join("|", a.Columns.Select(b => b.SourceColumn)),
                        out var sourcePropName)
                        ? sourcePropName
                        : srcCol.ToLower().EndsWith("id") && srcCol.Length > 2
                        ? srcCol.Substring(0, srcCol.Length - 2)
                        : type,
                    SourceMultiplicity = isSourceForeignKeyAlsoPrimaryKey
                        ? Multiplicity.One
                        : Multiplicity.Many,
                    Type = type
                };
                // only add if not in NavigationPropertiesToSkip
                if (!NavPropsToSkip.ContainsKey(table.GeneratedName + "." + prop.Name))
                {
                    props.Add(prop);
                }
                // get destination table
                if (tables == null) return;
                {
                    var destTable = tables
                        .FirstOrDefault(b => b.TableName == a.DestinationTableName);
                    type = EntityDesignUtil.Singularize(
                        NamingUtil.GetTableName(a.SourceTableName));
                    var destProp = new NavigationProperty
                    {
                        DestinationMultiplicity = prop.SourceMultiplicity,
                        ForeignKey = a,
                        InverseProperty = prop.Name,
                        Name = DestNavPropNames.TryGetValue(
                            a.SourceTableName + "." + a.Columns[0].SourceColumn,
                            out var destPropName)
                            ? destPropName
                            : prop.SourceMultiplicity == Multiplicity.Many
                                ? EntityDesignUtil.Pluralize(type)
                                : type,
                        SourceMultiplicity = prop.DestinationMultiplicity,
                        Type = prop.SourceMultiplicity == Multiplicity.Many
                            ? "ICollection<" + type + ">"
                            : type
                    };
                    if (destTable != null
                        && !NavPropsToSkip.ContainsKey(destTable.GeneratedName + "." + destProp.Name))
                    {
                        destTable.NavigationProperties.Add(destProp);
                    }
                }
            });
            // var isManyToMany = AreAllColumnsForeignKeys
            // The relationship is many-to-many only if the
            // relationship table only contains primary keys
            // of both entities, and no other fields.
            // 	0, 1, *: 0-1, 1-0, 1->1, 1<-1, 0-*, 1-*, *-0, *-1, *-*
            // 	if 0-1,
            // 		add single, non-nullable navigation property
            // 		of foreign key destination table class,
            // 		and ensure foreign key destination table class
            // 		has non-required navigation property to current table class
            // 	if 1-0,
            // 		do nothing, as non-required navigation property will be added
            // 		when processing other side of foreign key relationship
            // 	if 1->1,
            // 		add single, non-nullable navigation property
            // 		of foreign key destination table class,
            // 		and ensure foreign key destination table class
            // 		has required navigation property to current table class
            // 	if 1<-1,
            // 		required navigation property will be added
            // 		when processing other side of foreign key relationship
            // 	if 0-*,
            // 		add public virtual ICollection of
            // 		foreign key destination table class,
            // 		and ensure foreign key destination table class
            // 		has non-required navigation property to current table class
            // 	if 1-*,
            // 		add public virtual ICollection
            // 		of foreign key destination table class,
            // 		and ensure foreign key destination table class
            // 		has required navigation property to current table class
            // 	if *-0,
            // 		do nothing, as non-required navigation property will be added
            // 		when processing other side of foreign key relationship
            // 	if *-1,
            // 		do nothing, as required navigation property will be added
            // 		when processing other side of foreign key relationship
            // 	if *-*,
            // 		well, there are no such tables, according to the
            // 		Entity Framework definition, which is that all columns
            // 		in the join table being both in the join table primary key,
            // 		and a foreign key pointing from the join table to one of the
            // 		joined tables.
            return props;
        }

        public static string GenerateTableClassFileContent(
            Table t,
            string modelNamespace)
        {
            var sb = new StringBuilder();
            // TODO: move using statements to AddUsings
            //var hasDateColumns =
            //    t.Columns.Any(a => CreateDateFieldNames.ContainsKey(a.Name))
            //    || t.Columns.Any(a => ModifiedDateFieldNames.ContainsKey(a.Name));
            //var hasDateRangeColumns =
            //    t.Columns.Any(a => StartDateFieldNames.ContainsKey(a.Name))
            //    || t.Columns.Any(a => EndDateFieldNames.ContainsKey(a.Name));
            //var hasIsActiveColumns =
            //    t.Columns.Any(a => IsActiveFieldNames.ContainsKey(a.Name));
            //var hasDeactivatedDateColumns = 
            //    t.Columns.Any(a => DeactivatedDateFieldNames.ContainsKey(a.Name));
            var hasKeyColumns = t.PrimaryKeyColumns != null
                                && t.PrimaryKeyColumns.Count > 0;
            var hasMultipleKeyColumns = t.PrimaryKeyColumns != null
                                        && t.PrimaryKeyColumns.Count > 1;
            var interfaces = new List<string>();
            //if (hasDateColumns && !ExcludePopulateDatesInterface.ContainsKey(t.TableName))
            //    interfaces.Add("IPopulateDates");
            //if (hasDateRangeColumns && hasDeactivatedDateColumns
            //    && TemplateConstants
            //        .ExcludeDateRangeInterface
            //        .Split(',')
            //        .All(x => x != t.TableName))
            //{
            //    interfaces.Add("IDateRange");
            //}
            //if (!hasDateRangeColumns && hasIsActiveColumns
            //    && TemplateConstants
            //        .ExcludeDateRangeInterface
            //        .Split(',')
            //        .All(x => x != t.TableName))
            //{
            //    interfaces.Add("IDeletable");
            //}
            var subClass = interfaces.Contains("IDateRange") ? TemplateConstants.ModelsSubclassEffDate : TemplateConstants.ModelsSubclass;
            var needsColumnAttribute = t.Columns
                .Any(a => a.GeneratedName != a.Name
                        || hasMultipleKeyColumns
                        || a.Type == "varchar");
            var needsKeyAttribute = hasKeyColumns;
            var needsMaxLengthAttribute = t.Columns
                .Any(a => a.ClrType == "string"
                        && a.Type != "text"
                        && a.Length != -1);
            var needsModelTableAttribute = t.Columns
                .Any(a => ExcludeFieldsFromModelTable.ContainsKey(
                $"{t.TableName}.{a.Name}"));
            var needsRequiredAttribute = t.Columns
                .Any(a => a.ClrType == "string"
                        && !a.IsNullable);
            var needsTableAttribute = t.TableName != t.GeneratedName;
            // ReSharper disable once InconsistentNaming
            var needsUsingDGDeanModelsBase = !TablesToOptOutOfSubClass
                .ContainsKey(t.TableName)
                && subClass != TemplateConstants.ModelsSubclassEffDate;
            // ReSharper disable once InconsistentNaming
            var needsUsingDGDeanModelsAttributes =
                !string.IsNullOrWhiteSpace(t.ActiveColumnName)
                && subClass != TemplateConstants.ModelsSubclassEffDate;
            const bool needsUsingNewtonsoftJson = false;
            var needsUsingModelsAttributes = needsModelTableAttribute;
            var needsUsingModelsInterfaces = interfaces.Count > 0;
            var needsUsingSystem = t.Columns
                .Where(a => subClass != TemplateConstants.ModelsSubclassEffDate
                            || !ModelsSubclassEffDateExcludedColumns.ContainsKey(a.Name))
                .Any(a => GetClrType(a.Type) == "DateTime"
                    || GetClrType(a.Type) == "DateTimeOffset"
                     || GetClrType(a.Type) == "Guid"
                    || GetClrType(a.Type) == "Int16"
                    || GetClrType(a.Type) == "Int64");
            var needsUsingSystemCollectionsGeneric =
                t.NavigationProperties
                    .Any(a => a.Type.StartsWith("ICollection<"))
                    // required for metadata properties
                    || true;
            var needsUsingSystemComponentModelDataAnnotations =
                needsKeyAttribute
                || needsMaxLengthAttribute
                || needsRequiredAttribute
                || t.Columns.Any();
            var needsUsingSystemComponentModelDataAnnotationsSchema =
                needsColumnAttribute
                //	|| needsDatabaseGeneratedAttribute
                //	|| needsForeignKeyAttribute
                //	|| needsInversePropertyAttribute
                //	|| needsNotMappedAttribute
                || needsTableAttribute;
            const bool needsUsingSystemRuntimeSerialization = false;
            var needsUsingModelsDbCustom = interfaces.Contains("IDateRange");
            AddGeneratedDisclaimer(sb);
            AddUsing(
                needsUsingDGDeanModelsAttributes,
                sb,
                "DGDean.Models.Attributes");
            AddUsing(
                needsUsingDGDeanModelsBase,
                sb,
                "DGDean.Models.Base");
            AddUsing(
                needsUsingNewtonsoftJson,
                sb,
                "Newtonsoft.Json");
            AddUsing(
                needsUsingModelsAttributes,
                sb,
                "DB.Models.Attributes");
            AddUsing(
                needsUsingModelsInterfaces,
                sb,
                "DB.Models.Interfaces");
            AddUsing(
                needsUsingModelsDbCustom,
                sb,
                "DB.Models.DB.Custom");
            AddUsing(needsUsingSystem, sb, "System");
            AddUsing(
                needsUsingSystemCollectionsGeneric,
                sb,
                "System.Collections.Generic");
            AddUsing(
                needsUsingSystemComponentModelDataAnnotations,
                sb,
                "System.ComponentModel.DataAnnotations");
            AddUsing(
                needsUsingSystemComponentModelDataAnnotationsSchema,
                sb,
                "System.ComponentModel.DataAnnotations.Schema");
            AddUsing(
                needsUsingSystemRuntimeSerialization,
                sb,
                "System.Runtime.Serialization");
            var className = EntityDesignUtil.Singularize(
                NamingUtil.GetTableName(t.TableName));
            //var createDateProperty = hasDateColumns
            //    ? t.Columns
            //          .FirstOrDefault(a => CreateDateFieldNames.ContainsKey(a.Name))
            //          ?.Name ?? ""
            //    : "";
            //var modifiedDateProperty = hasDateColumns
            //    ? t.Columns
            //          .FirstOrDefault(a => ModifiedDateFieldNames.ContainsKey(a.Name))
            //          ?.Name ?? ""
            //    : "";
            //var startDateProperty = hasDateRangeColumns
            //    ? t.Columns
            //          .FirstOrDefault(a => StartDateFieldNames.ContainsKey(a.Name))
            //          ?.Name ?? ""
            //    : "";
            //var endDateProperty = hasDateRangeColumns
            //    ? t.Columns
            //          .FirstOrDefault(a => EndDateFieldNames.ContainsKey(a.Name))
            //          ?.Name ?? ""
            //    : "";
            //var dateRangeModelName =
            //    hasDateRangeColumns
            //    && TemplateConstants
            //        .ExcludeDateRangeInterface
            //        .Split(',')
            //        .All(x => x != t.TableName)
            //    ? Regex.Replace(className, @"Model$", "")
            //    : "";
            sb.Append(
                "\r\n" +
                "namespace " + modelNamespace + "\r\n" +
                "{\r\n" +
                GetClassAttributes(t, className) +
                GetClassStatement(
                    t.TableName,
                    className,
                    subClass,
                    interfaces,
                    //createDateProperty,
                    //modifiedDateProperty,
                    //startDateProperty,
                    //endDateProperty,
                    //dateRangeModelName,
                    GetNormalProperties(t, subClass),
                    GetNavigationProperties(t)) +
                "}\r\n");
            return sb.ToString();
        }

        private static string GetClassAttributes(
            Table t,
            string className)
        {
            // Table (needed if generated class name is different from table name)
            return t.TableName != className
                ? "\t[Table(\"" + t.TableName + "\", Schema = \"" + t.SchemaName + "\")]\r\n"
                : "";
        }

        private static string GetClassStatement(
            string tableName,
            string className,
            string subClass,
            List<string> interfaces,
            //string createDateProperty,
            //string modifiedDateProperty,
            //string startDateProperty,
            //string endDateProperty,
            //string dateRangeModelName,
            string normalProperties,
            string navigationProperties)
        {
            var regionInterfaceMethods = GetRegionInterfaceMethods(
                tableName,
                //createDateProperty,
                //modifiedDateProperty,
                //startDateProperty,
                //endDateProperty,
                //dateRangeModelName,
                subClass);
            regionInterfaceMethods = (string.IsNullOrWhiteSpace(
                regionInterfaceMethods)
                ? ""
                : "\r\n")
                + regionInterfaceMethods;
            var regionNormalProperties = GetRegionNormalProperties(
                normalProperties);
            var regionNavigationProperties =
                GetRegionNavigationProperties(navigationProperties);
            regionNavigationProperties = (string.IsNullOrWhiteSpace(
                regionNavigationProperties)
                ? ""
                : "\r\n")
                + regionNavigationProperties;
            if (interfaces.Contains("IDateRange"))
            {
                subClass = TemplateConstants.ModelsSubclassEffDate;
                //interfaces.Remove("IDateRange");
            }
            return "\tpublic partial class " + className +
                (HasSubClass(subClass, tableName) || HasInterface(interfaces) ? " : " : "") +
                (HasSubClass(subClass, tableName) ? subClass : "") +
                (HasSubClass(subClass, tableName) && HasInterface(interfaces) ? ", " : "") +
                (HasInterface(interfaces) ? string.Join(", ", interfaces) : "") +
                "\r\n" +
                "\t{\r\n" +
                regionNormalProperties +
                // regionMetadataProperties +
                regionNavigationProperties +
                // regionSerializationProperties +
                regionInterfaceMethods +
                "\t}\r\n";
        }

        private static bool HasSubClass(string subClass, string tableName)
        {
            return !(string.IsNullOrWhiteSpace(subClass)
            || TablesToOptOutOfSubClass.ContainsKey(tableName));
        }

        private static bool HasInterface(IReadOnlyCollection<string> interfaces)
        {
            return interfaces.Count > 0;
        }


        public static string GetClrType(string sqlType)
        {
            switch (sqlType)
            {
                case "bigint":
                    return "long";
                case "binary":
                case "rowversion":
                case "varbinary":
                    return "Byte[]";
                case "bit":
                    return "bool";
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                    return "string";
                case "date":
                case "datetime":
                case "datetime2":
                    return "DateTime";
                case "datetimeoffset":
                    return "DateTimeOffset";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "decimal";
                case "float":
                    return "double";
                case "int":
                    return "int";
                case "real":
                    return "Single";
                case "smallint":
                    return "short";
                case "sql_variant":
                    return "Object";
                case "time":
                    return "TimeSpan";
                case "tinyint":
                    return "Byte";
                case "uniqueidentifier":
                    return "Guid";
                default:
                    return "Unknown";
            }
        }

        private static string GetNavigationProperties(
            Table t)
        {
            var sb = new StringBuilder();
            if (t.NavigationProperties.Count > 0)
            {
                t.NavigationProperties
                    .OrderBy(a => a.Name)
                    .ToList()
                    .ForEach(a =>
                    {
                        var attr = GetNavigationPropertyAttributes(t, a);
                        if (!string.IsNullOrWhiteSpace(attr))
                        {
                            sb.Append("\t\t" + attr + "\r\n");
                        }
                        sb.Append("\t\tpublic virtual " + a.Type + " " + a.Name + " { get; set; }\r\n\r\n");
                    });
            }
            return sb.ToString();
        }

        private static string GetNavigationPropertyAttributes(
            Table t,
            NavigationProperty n)
        {
            // need to support the following attributes
            // ForeignKey (needed if foreign key column name doesn't match nav prop name + "Id")
            // Index (needed if index that's not primary key exists)
            // Inverse (needed for multiple relationships between classes)
            // NotMapped (needed if any nav properties)
            // needed if source end of foreign key, 
            // and type + "id" isn't a case-insensitive match to
            // the foreign key source property
            // (only supporting single property foreign keys with the ForeignKey
            // attribute for now) column source column
            var isSource = n.ForeignKey.SourceTableName == t.TableName;
            var isSingleColumnKey = n.ForeignKey.Columns.Count == 1;
            var foreignKeyProperties = string.Join(", ",
                n.ForeignKey.Columns
                .Select(a => NamingUtil.GetColumnName(t.TableName, a.SourceColumn)));
            var isInverse = !string.IsNullOrWhiteSpace(n.InverseProperty);
            var displayNameKey = $"{t.TableName}.{n.Name}";
            var displayNeeded = NavPropDisplayNames
                .ContainsKey(displayNameKey);
            var display = displayNeeded
                ? "Display(Name = \"" + NavPropDisplayNames[displayNameKey] + "\")"
                : "";
            var commaNeeded = displayNeeded;
            var editableNeeded = EditableColumns.ContainsKey($"{t.TableName}.{n.Name}");
            var editable = editableNeeded
                ? (commaNeeded ? ", " : "") + "Editable(true)"
                : "";
            commaNeeded = commaNeeded || editableNeeded;
            var foreignKeyNeeded =
                (isSource
                 || isSingleColumnKey)
                && !isInverse;
            var foreignKey = foreignKeyNeeded
                ? (commaNeeded ? ", " : "")
                  + "ForeignKey(\"" + foreignKeyProperties + "\")"
                : "";
            commaNeeded = commaNeeded || foreignKeyNeeded;
            var inverseNeeded = isInverse;
            var inverse = inverseNeeded
                ? (commaNeeded ? ", " : "") +
                    "InverseProperty(\"" + n.InverseProperty + "\")"
                : "";
            var attrNeeded =
                foreignKeyNeeded
                || inverseNeeded;
            var attr = attrNeeded
                ? "["
                    + display
                    + editable
                    + foreignKey
                    + inverse
                    + "]"
                : "";
            return attr;
        }

        private static string GetNormalProperties(
            Table t,
            string subClass)
        {
            var sb = new StringBuilder();
            t.Columns
                .OrderBy(a => a.Name)
                .Where(a => subClass != TemplateConstants.ModelsSubclassEffDate
                            || !ModelsSubclassEffDateExcludedColumns.ContainsKey(a.Name))
                .ToList()
                .ForEach(a =>
                {
                    var attr = GetNormalPropertyAttributes(t, a);
                    if (!string.IsNullOrWhiteSpace(attr))
                    {
                        sb.Append("\t\t" + attr + "\r\n");
                    }
                    sb.Append(
                        "\t\tpublic "
                        + (NewKeywordColumns.ContainsKey(t.TableName + "." + a.Name)
                            ? "new "
                            : OverrideColumns.ContainsKey(t.TableName + "." + a.Name)
                            ? "override "
                            : "")
                        + a.ClrType
                        + (a.IsNullable && a.ClrType != "string" ? "?" : "")
                        + " "
                        + a.GeneratedName
                        + " { get; set; }"
                        + (!string.IsNullOrWhiteSpace(a.DefaultValue)
                            ? " = "
                              + (a.ClrType == "string"
                                 && !a.DefaultValue.Contains("\"")
                                 && !a.DefaultValue.Contains("'") ? "\"" : "")
                              + a.DefaultValue
                              + (a.ClrType == "string"
                                 && !a.DefaultValue.Contains("\"")
                                 && !a.DefaultValue.Contains("'") ? "\"" : "")
                              + ";"
                            : "")
                        + "\r\n\r\n");
                });
            return sb.ToString();
        }

        private static string GetNormalPropertyAttributes(
            Table t,
            Column c)
        {
            // need to support the following attributes
            //  Display (needed for automatically generated form field labels)
            //  ForeignKey (needed if foreign key column name doesn't match nav prop name + "Id")
            //  Index (needed if index that's not primary key exists)
            //  Inverse (needed for multiple relationships between classes)
            //  NotMapped (needed if any nav properties)

            var keyNeeded = c.IsKey;
            var key = keyNeeded ? "Key" : "";
            var commaNeeded = keyNeeded;
            var activeNeeded = !string.IsNullOrWhiteSpace(t.ActiveColumnName)
                && c.Name == t.ActiveColumnName;
            var active = activeNeeded
                ? (commaNeeded ? ", " : "") + "Active(" +
                    (t.ActiveColumnValue ? "true" : "false") + ")"
                : "";
            commaNeeded = commaNeeded || activeNeeded;
            var columnMultipleKeyCols = c.IsKey
                && t.PrimaryKeyColumns.Count > 1;
            var columnNameMismatch = c.GeneratedName != c.Name;
            var columnVarchar = c.Type == "varchar";
            var columnNeeded = columnMultipleKeyCols
                || columnNameMismatch
                || columnVarchar;
            var column = columnNeeded
                ? (commaNeeded ? ", " : "") + "Column(" +
                // name
                (columnNameMismatch
                ? "\"" + c.Name + "\""
                : "") +
                // order
                (columnMultipleKeyCols
                ? (columnNameMismatch ? ", " : "") +
                    "Order = " + (t.PrimaryKeyColumns.IndexOf(c.Name) + 1)
                : "") +
                // typename
                (columnVarchar
                ? (columnNameMismatch || columnMultipleKeyCols ? ", " : "") +
                    "TypeName = \"varchar\""
                : "") +
                ")"
                : "";
            commaNeeded = commaNeeded || columnNeeded;
            var databaseGeneratedNeeded = c.IsIdentity || c.IsComputed;
            var databaseGenerated = databaseGeneratedNeeded
                ? (commaNeeded ? ", " : "") +
                    "DatabaseGenerated(DatabaseGeneratedOption." +
                    (c.IsIdentity ? "Identity" : "Computed") + ")"
                : "";
            commaNeeded = commaNeeded || databaseGeneratedNeeded;
            // for now, generate the display attribute for every field
            var display = (commaNeeded ? ", " : "") +
                          "Display(Name = \"" + NamingUtil.GetDisplayName(c.Name) + "\")";
            commaNeeded = true;
            var editableNeeded = EditableColumns.ContainsKey($"{t.TableName}.{c.Name}");
            var editable = editableNeeded
                ? ", " + "Editable(true)"
                : "";
            var fk = keyNeeded
                ? t.ForeignKeys
                    .FirstOrDefault(a => a.Columns.Any(b => b.SourceColumn == c.Name))
                : null;
            var foreignKeyNeeded =
                keyNeeded
                && fk != null;
            var srcCol = fk != null
                ? fk.Columns[0].SourceColumn
                : "";
            var type = fk != null
                ? EntityDesignUtil.Singularize(
                    NamingUtil.GetTableName(fk.DestinationTableName))
                : "";
            // TODO: need to support list of source columns
            var foreignKey = foreignKeyNeeded
                ? ", " + "ForeignKey(\"" +
                    (SourceNavPropNames.TryGetValue(
                        fk.SourceTableName + "." + string.Join("|", fk.Columns.Select(a => a.SourceColumn)),
                        out var sourcePropName)
                        ? sourcePropName
                        : srcCol.ToLower().EndsWith("id") && srcCol.Length > 2
                        ? srcCol.Substring(0, srcCol.Length - 2)
                        : type) +
                    "\")"
                : "";
            var indexNeeded = t.Indexes
                .Any(a => a.Columns.Contains(c.Name));
            var index = "";
            t.Indexes
                .Where(a => a.Columns.Contains(c.Name))
                .ToList()
                .ForEach(a =>
                {
                    index +=
                        (string.IsNullOrWhiteSpace(index)
                        ? ""
                        : ", ") +
                        "Index(\"" + a.Name + "\"" +
                        (a.Columns.Count > 1
                        ? ", " + (a.Columns.IndexOf(c.Name) + 1).ToString()
                        : "") +
                        ")";
                });
            index = string.IsNullOrWhiteSpace(index)
                ? ""
                : (commaNeeded ? ", " : "") + index;
            commaNeeded = commaNeeded || indexNeeded;
            var maxLengthNeeded = c.ClrType == "string"
                && c.Type != "text"
                && c.Length > -1;
            var maxLength = maxLengthNeeded
                ? (commaNeeded ? ", " : "") +
                    "MaxLength(" + (c.Type == "varchar" || c.Type == "char" ? c.Length : c.Length / 2) + ")"
                : "";
            commaNeeded = commaNeeded || maxLengthNeeded;
            var modelTableNeeded = ExcludeFieldsFromModelTable.ContainsKey(
                $"{t.TableName}.{c.Name}");
            var modelTable = modelTableNeeded
                ? (commaNeeded ? ", " : "") +
                    "ModelTable(true)"
                : "";
            commaNeeded = commaNeeded || modelTableNeeded;
            var requiredNeeded = c.ClrType == "string" && !c.IsNullable && c.DefaultValue == null;
            var required = requiredNeeded
                ? (commaNeeded ? ", " : "") + "Required(AllowEmptyStrings = true)"
                : "";
            var attrNeeded = commaNeeded || requiredNeeded;
            var attr = attrNeeded
                ? "["
                    + key
                    + active
                    + column
                    + databaseGenerated
                    + display
                    + editable
                    + foreignKey
                    + index
                    + maxLength
                    + modelTable
                    + required
                    + "]"
                : "";
            return attr;
        }

        private static string GetRegionInterfaceMethods(
            string tableName,
            //string createDateProperty,
            //string modifiedDateProperty,
            //string startDateProperty,
            //string endDateProperty,
            //string dateRangeModelName,
            string subClass
            )
        {
            var s = "";

            //if (!ExcludePopulateDatesInterface.ContainsKey(tableName)
            //    //&& (!string.IsNullOrWhiteSpace(createDateProperty)
            //    //|| !string.IsNullOrWhiteSpace(modifiedDateProperty)
            //    //|| !string.IsNullOrWhiteSpace(startDateProperty)
            //    //|| !string.IsNullOrWhiteSpace(endDateProperty))
            //    && subClass != TemplateConstants.ModelsSubclassEffDate)
            //{
              //  s +=
            //        "\t\tpublic void PopulateDates()\r\n" +
            //        "\t\t{\r\n" +
            //        (string.IsNullOrWhiteSpace(createDateProperty)
            //            ? ""
            //            : $"\t\t\t{createDateProperty} = {createDateProperty} == DateTime.MinValue\r\n" +
            //                "\t\t\t\t? DateTime.UtcNow\r\n" +
            //                $"\t\t\t\t: {createDateProperty};\r\n") +
            //        (string.IsNullOrWhiteSpace(modifiedDateProperty)
            //            ? ""
            //            : $"\t\t\t{modifiedDateProperty} = DateTime.UtcNow;\r\n") +
            //        (string.IsNullOrWhiteSpace(startDateProperty)
            //            ? ""
            //            : $"\t\t\t{startDateProperty} = {startDateProperty} == DateTime.MinValue\r\n" +
            //                "\t\t\t\t? new DateTime(1900, 1, 1)\r\n" +
            //                $"\t\t\t\t: {startDateProperty};\r\n") +
            //        (string.IsNullOrWhiteSpace(endDateProperty)
            //            ? ""
            //            : $"\t\t\t{endDateProperty} = {endDateProperty} == DateTime.MinValue\r\n" +
            //                "\t\t\t\t? new DateTime(2500, 1, 1)\r\n" +
            //                $"\t\t\t\t: {endDateProperty};\r\n") +
            //        "\t\t}\r\n\r\n";
            //}

            //if (!string.IsNullOrWhiteSpace(dateRangeModelName) 
            //    && subClass == TemplateConstants.ModelsSubclassEffDate)
            //{
            //    s +=
            //        "\t\tpublic override int IDateRangeId\r\n" +
            //        "\t\t{\r\n" +
            //        $"\t\t\tget {{ return {dateRangeModelName}Id; }}\r\n" +
            //        "\t\t}\r\n\r\n";
            //}

            if (!string.IsNullOrWhiteSpace(s))
            {
                s =
                    "\t\t#region interface methods\r\n\r\n" +
                    s +
                    "\t\t#endregion\r\n";
            }
            return s;
        }

        private static string GetRegionNavigationProperties(
            string navigationProperties)
        {
            return string.IsNullOrWhiteSpace(navigationProperties)
                ? ""
                : "\t\t#region navigation properties\r\n\r\n" +
                navigationProperties +
                "\t\t#endregion\r\n";
        }

        private static string GetRegionNormalProperties(
            string normalProperties)
        {
            return string.IsNullOrWhiteSpace(normalProperties)
                ? ""
                : "\t\t#region normal properties\r\n\r\n" +
                normalProperties +
                "\t\t#endregion\r\n";
        }

        private static bool IsDestinationNullable(
            ForeignKey foreignKey,
            DataTable columnsTable)
        {
            var destinationColumnsList = "";
            foreignKey
                .Columns
                .ForEach(a =>
                {
                    destinationColumnsList +=
                        (destinationColumnsList == ""
                        ? ""
                        : ",") +
                        "'" + a.DestinationColumn + "'";
                });
            return columnsTable
                .Select("name in (" + destinationColumnsList + ")")
                .All(a => Convert.ToBoolean(a["is_nullable"]));
        }

        private static bool IsSourcePrimaryKey(
            Table table,
            ForeignKey foreignKey)
        {
            // all primary key source columns must be in foreign key source columns,
            var allPksInFk = table
                .PrimaryKeyColumns
                .All(a => foreignKey.Columns.Any(b => b.SourceColumn == a));
            // and all foreign key columns must be in primary key columns
            var allFksInPk = foreignKey
                .Columns
                .All(a => table.PrimaryKeyColumns.Any(b => b == a.SourceColumn));
            return allPksInFk
                && allFksInPk;
        }
    }
}
