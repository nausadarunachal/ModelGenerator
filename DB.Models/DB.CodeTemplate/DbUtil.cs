namespace DB.CodeTemplate
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class DbUtil
    {
        private static readonly Dictionary<string, bool> ActivityColumnNames;
        private static readonly Dictionary<string, string> DefaultValuesToBeExcluded;
        private static readonly string[] DataSetTables;

        static DbUtil()
        {
            ActivityColumnNames = TemplateConstants
                .DbColumnActiveMappings
                .Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(a => new
                {
                    Key = a.Split(':')[0].Trim(),
                    Value = a.Split(':')[1].Trim()
                })
                .ToDictionary(a => a.Key, a => a.Value == "1");
            DataSetTables = TemplateConstants
                .DataSetTables
                .Split(',')
                .Select(a => a.Trim())
                .ToArray();
            DefaultValuesToBeExcluded = TemplateConstants.DefaultValuesToBeExcluded
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().ToLower())
                .ToDictionary(a => a);
        }

        private static void AddTableToDataSet(
            SqlConnection cnn,
            string dataTableName,
            string tableName,
            DataSet ds)
        {
            var adapter = new SqlDataAdapter();
            adapter.TableMappings.Add("Table", dataTableName);
            var cmd = new SqlCommand(
                "SELECT * FROM " + tableName,
                cnn) {CommandType = CommandType.Text};
            adapter.SelectCommand = cmd;
            adapter.Fill(ds);
        }

        private static string GetColumnNameByTableIdColumnId(
            DataTable columnsTable,
            int tableId,
            int columnId)
        {
            return columnsTable
                .Select("object_id = " + tableId +
                    " AND column_id = " + columnId)[0]["name"]
                .ToString();
        }

        public static List<ForeignKey> GetForeignKeysByTableId(
            DataTable columnsTable,
            DataTable foreignKeysTable,
            DataTable foreignKeyColumnsTable,
            DataTable tablesTable,
            int tableId,
            string tableName)
        {
            var foreignKeys = new List<ForeignKey>();
            var fksToIgnore = TemplateConstants
                .ForeignKeysToIgnore
                .Split(',')
                .ToDictionary(a => a.Trim());
            var keys = foreignKeysTable
                .Select("parent_object_id = " + tableId);
            if (keys.Length == 0)
            {
                return foreignKeys;
            }
            var fkeycols = foreignKeyColumnsTable
                .Select(
                    "parent_object_id = " + tableId,
                    "constraint_object_id, constraint_column_id");
            var sourceTableName = GetTableNameById(
                tablesTable,
                tableId);
            foreach (var fk in keys)
            {
                if (fksToIgnore.ContainsKey(tableName + "." + fk["name"])) continue;
                var columns = new List<ForeignKeyColumn>();
                fkeycols
                    .Where(a => Convert.ToInt32(a["constraint_object_id"])
                                == Convert.ToInt32(fk["object_id"]))
                    .OrderBy(a => Convert.ToInt32(a["constraint_column_id"]))
                    .ToList()
                    .ForEach(a => {
                        var column = new ForeignKeyColumn
                        {
                            DestinationColumn =
                                GetColumnNameByTableIdColumnId(
                                    columnsTable,
                                    Convert.ToInt32(a["referenced_object_id"]),
                                    Convert.ToInt32(a["referenced_column_id"])),
                            SourceColumn =
                                GetColumnNameByTableIdColumnId(
                                    columnsTable,
                                    Convert.ToInt32(a["parent_object_id"]),
                                    Convert.ToInt32(a["parent_column_id"]))
                        };
                        columns.Add(column);
                    });
                var destinationTableName = GetTableNameById(
                    tablesTable,
                    Convert.ToInt32(fk["referenced_object_id"]));
                var foreignKey = new ForeignKey
                {
                    Columns = columns,
                    DestinationTableName = destinationTableName,
                    SourceTableName = sourceTableName
                };
                foreignKeys.Add(foreignKey);
            }
            return foreignKeys;
        }

        public static IEnumerable<DataRow> GetIndexColumnsByTableIdIndexId(
            DataTable indexColumnsTable,
            int tableId,
            int indexId)
        {
            return indexColumnsTable
                .Select("object_id = " + tableId +
                    " AND index_id = " + indexId,
                    "key_ordinal ASC");
        }

        public static DataSet GetSchemaDataSet(
            string connStr)
        {
            var ds = new DataSet();
            using (var cnn = new SqlConnection(connStr))
            {
                cnn.Open();
                for (var i = 0; i < DataSetTables.Length; i += 2)
                {
                    AddTableToDataSet(
                        cnn,
                        DataSetTables[i],
                        DataSetTables[i + 1],
                        ds);
                }
            }
            return ds;
        }

        public static int GetPrimaryKeyIndexIdByTableId(
            DataTable indexTable,
            int tableId)
        {
            var indexId = 0;
            var indexes = indexTable
                .Select("object_id = " + tableId + " AND is_primary_key = 1");
            if (indexes.Length > 0)
            {
                indexId = Convert.ToInt32(indexes[0]["index_id"]);
            }
            return indexId;
        }

        private static string GetSchemaNameById(
            DataTable schemas,
            int id)
        {
            var rows = schemas.Select("schema_id = " + id);
            return rows.Length > 0
                ? rows[0]["name"].ToString()
                : "";
        }

        private static string GetTableNameById(
            DataTable tablesTable,
            int id)
        {
            return tablesTable
                .Select("object_id = " + id)[0]["name"]
                .ToString();
        }

        public static List<Table> GetTables(
            DataTable columns,
            DataTable indexColumns,
            DataTable indexes,
            DataTable schemas,
            DataTable systables)
        {
            var tables = new List<Table>();
            var skipList = TemplateConstants.TablesToSkip
                .Split(',')
                .Select(a => a.Trim().ToLower())
                .ToList();
            // get tables
            systables.Rows
                .OfType<DataRow>()
                .Where(a => !skipList.Any(s => Regex.IsMatch(a["name"].ToString(), @"^" + s + "$", RegexOptions.IgnoreCase)))
                .ToList()
                .ForEach(a =>
                {
                    var t = new Table
                    {
                        GeneratedName = EntityDesignUtil.Singularize(
                            NamingUtil.GetTableName(a["name"].ToString())),
                        ObjectId = (int)a["object_id"],
                        SchemaName = GetSchemaNameById(schemas, (int)a["schema_id"]),
                        TableName = a["name"].ToString()
                    };
                    tables.Add(t);
                });
            // get indexes for each table
            tables.ForEach(a =>
            {
                // get indexes for table
                // for each index, create index
                indexes.Select("object_id = " + a.ObjectId +
                    " AND type IN(1, 2)" +
                    " AND is_primary_key = 0",
                    "index_id ASC")
                    .ToList()
                    .ForEach(b =>
                    {
                        var index = new Index
                        {
                            IndexId = Convert.ToInt32(b["index_id"]),
                            Name = b["name"].ToString()
                        };
                        indexColumns
                            .Select("object_id = " + a.ObjectId +
                                " AND index_id = " + index.IndexId,
                                "index_column_id ASC")
                            .ToList()
                            .ForEach(c =>
                            {
                                index.Columns.Add(
                                    columns
                                    .Select("object_id = " + a.ObjectId +
                                        " AND column_id = " + c["column_id"].ToString())
                                    .Select(d => d["name"].ToString())
                                    .FirstOrDefault());
                            });
                        a.Indexes.Add(index);
                    });
                a.Indexes =
                    a.Indexes
                        .OrderBy(b => b.Name)
                        .ToList();
            });
            return tables
                .OrderBy(a => a.TableName)
                .ToList();
        }

        private static string GetTypeByUserTypeId(
            DataTable types,
            int id)
        {
            return types
                .Select("user_type_id = " + id)[0]["name"]
                .ToString();
        }

        public static string GetTableActivityFieldName(
            Table table)
        {
            return table.Columns
                .First(a => ActivityColumnNames.ContainsKey(a.Name))
                .Name;
        }

        public static bool GetTableActivityFieldValue(
            Table table)
        {
            return ActivityColumnNames[
                table.Columns
                .First(a => ActivityColumnNames.ContainsKey(a.Name))
                .Name];
        }

        public static bool HasTableActiveFlag(Table table)
        {
            return table.Columns
                .Any(a => ActivityColumnNames.ContainsKey(a.Name));
        }

        public static List<Column> GetColumnsByTableId(
            DataTable syscolumns,
            DataTable systypes,
            DataTable defaultConstraints,
            int id,
            string tableName)
        {
            var columns = new List<Column>();
            var skipList = TemplateConstants.FieldsToSkip
                .Split(',')
                .Select(a => a.Trim().ToLower())
                .ToList();
            var selectedColumns = syscolumns.Select(
                "object_id = " + id,
                "column_id asc")
                .Where(a => skipList
                    .All(s => tableName.ToLower() + "." + a["name"].ToString().ToLower() != s))
                .ToList();
            selectedColumns
                .ToList()
                .ForEach(a =>
                {
                    var type = GetTypeByUserTypeId(
                        systypes,
                        (int)a["user_type_id"]);
                    var column = new Column
                    {
                        ClrType = TableGenerator.GetClrType(type),
                        GeneratedName = NamingUtil.GetColumnName(tableName, a["name"].ToString()),
                        IsComputed = Convert.ToBoolean(a["is_computed"]),
                        IsIdentity = Convert.ToBoolean(a["is_identity"]),
                        IsNullable = Convert.ToBoolean(a["is_nullable"]),
                        DefaultValue = a["default_object_id"].ToString() != "0" ?
                            defaultConstraints
                                .Select(
                                    "object_id = "
                                    + a["default_object_id"].ToString())
                                .Select(dc => dc["definition"].ToString())
                                .SingleOrDefault()
                            : null,
                        Length = Convert.ToInt32(a["max_length"]),
                        Name = a["name"].ToString(),
                        Precision = Convert.ToInt32(a["precision"]),
                        Scale = Convert.ToInt32(a["scale"]),
                        Type = type
                    };
                    if (column.DefaultValue != null
                        && DefaultValuesToBeExcluded.ContainsKey(column.DefaultValue.ToLower()))
                    {
                        column.DefaultValue = null;
                    }
                    if (column.DefaultValue != null)
                    {
                        column.DefaultValue = Regex.Replace(column.DefaultValue, @"[\(\)]", "");
                        column.DefaultValue = Regex.Replace(column.DefaultValue, @"\'", "\"");
                        column.DefaultValue = column.DefaultValue.Trim();
                        switch (column.ClrType)
                        {
                            case "bool":
                                column.DefaultValue = column.DefaultValue == "1" ? "true" : "false";
                                break;
                            case "Guid":
                                column.DefaultValue = "Guid.NewGuid()";
                                break;
                            case "DateTime" when column.DefaultValue.ToLower() == "getutcdate":
                                column.DefaultValue = "DateTime.UtcNow";
                                break;
                            case "DateTime":
                                column.DefaultValue = "DateTime.Parse(" + column.DefaultValue + ")";
                                break;
                            // ReSharper disable once RedundantEmptySwitchSection
                            default:
                                break;
                        }
                    }
                    columns.Add(column);
                });
            return columns;
        }

        public static List<string> GetColumnsByTableIdColumnId(
            DataTable columnsTable,
            int tableId,
            IEnumerable<DataRow> indexColumns)
        {
            var columnIdDelimitedList = "";
            indexColumns
                .Select(a => a["column_id"])
                .ToList()
                .ForEach(a =>
                {
                    columnIdDelimitedList +=
                        string.IsNullOrWhiteSpace(columnIdDelimitedList)
                            ? a.ToString()
                            : "," + a.ToString();
                });
            if (columnIdDelimitedList == "")
            {
                return new List<string>();
            }
            var unorderedList = columnsTable.Select(
                "object_id = " + tableId +
                " AND column_id IN(" + columnIdDelimitedList + ")");
            return columnIdDelimitedList.Split(',')
                .Select(columnId => unorderedList.Where(a => a["column_id"].ToString() == columnId)
                    .Select(a => a["name"].ToString())
                    .FirstOrDefault())
                .ToList();
        }
    }
}
