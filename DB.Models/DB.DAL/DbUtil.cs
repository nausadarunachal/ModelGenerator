namespace DB.DAL
{
    using CodeFirstStoredProcs;
    using DGDean.EfUtils;
    using Models.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class ContextDb
    {
        #region private static members

        private static readonly Lazy<bool> LogDbConnections =
            new Lazy<bool>(() => ConfigurationManager.AppSettings[
            "DB.DAL.ContextDb.LogDbConnections"] == "1");

        private static readonly Lazy<bool>
            PopulateOAuthUsernameInDbSessionContext =
            new Lazy<bool>(() => ConfigurationManager.AppSettings[
            "DB.DAL.ContextDb.PopulateOAuthUsernameInDbSessionContext"]
            == "1");

        private static readonly Lazy<string>
            DbSessionContextKeyOauthUserName =
            new Lazy<string>(() => ConfigurationManager.AppSettings[
            "DB.DAL.ContextDb.DbSessionContextKeyOauthUserName"]);

        #endregion

        #region private instance members

        private readonly string _oauthUserName;

        #endregion

        #region constructor
        public ContextDb(
            bool useWriteDb = false,
            int? commandTimeout = null)
            : base(useWriteDb
                  ? ConfigurationManager
                  .ConnectionStrings["WriteContext"]
                  .ConnectionString
                  : ConfigurationManager
                  .AppSettings["MSSQLConnectionString"])
        {
            if (ConfigurationManager.AppSettings["LogEFSQL"] == "1")
            {
                Database.Log = a => Log.Debug(a);
            }

            if (commandTimeout.HasValue)
            {
                Database.CommandTimeout = commandTimeout;
            }

            if (LogDbConnections.Value
                || PopulateOAuthUsernameInDbSessionContext.Value)
            {
                Database.Connection.StateChange += Connection_StateChange;
            }
           // _oauthUserName = GlobalContext.OAuthUserName;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            this.InitializeStoredProcs();
        }

        #endregion

        #region automatic population and error handling

        private void PopulateDates()
        {
            ChangeTracker.Entries()
                .Where(a => (a.State == EntityState.Added
                             || a.State == EntityState.Modified))
                .ToList()
                .ForEach(a =>
                {
                    if (a.Entity is IPopulateDates dates)
                    {
                        dates.PopulateDates();
                    }
                });
        }

        public override int SaveChanges()
        {
            try
            {
                PopulateDates();
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(EfUtil.GetFullEntityException(ex));
            }
        }

      
        public int SaveChanges(bool skipLogging)
        {
            try
            {
                var originalSkipLoggingValue = SkipLogging;
                SkipLogging = skipLogging;
                PopulateDates();
                var retVal = base.SaveChanges();
                SkipLogging = originalSkipLoggingValue;
                return retVal;
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(EfUtil.GetFullEntityException(ex));
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                PopulateDates();
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                Log.Error("DbEntityValidation Error from ContextDb.SaveChangesAsync()", ex);
                throw;
            }
        }

        #endregion

        #region context helper methods

        private static void SetSessionContext(
            DbConnection cnn,
            string key,
            string value,
            bool readOnly)
        {
            var cmd = cnn.CreateCommand();
            cmd.CommandText = "sp_set_session_context";
            cmd.CommandType = CommandType.StoredProcedure;
            // @key
            var param = cmd.CreateParameter();
            param.DbType = DbType.String;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@key";
            param.Size = 128;
            param.Value = key;
            cmd.Parameters.Add(param);
            // @value
            param = cmd.CreateParameter();
            param.DbType = DbType.Object;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@value";
            param.Size = 8000;
            param.Value = value;
            cmd.Parameters.Add(param);
            // @read_only
            param = cmd.CreateParameter();
            param.DbType = DbType.Boolean;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@read_only";
            param.Value = readOnly;
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region event handlers

        private void Connection_StateChange(
            object sender,
            StateChangeEventArgs e)
        {
            if (LogDbConnections.Value)
            {
                var action = e.CurrentState == ConnectionState.Open
                    ? "opening"
                    : e.CurrentState == ConnectionState.Closed
                        ? "closing"
                        : null;
                if (!string.IsNullOrEmpty(action))
                {
                    Log.Debug($"{action} database connection "
                              + $"{Database.Connection.ConnectionString}");
                }
            }
            if (!PopulateOAuthUsernameInDbSessionContext.Value
                || e.CurrentState != ConnectionState.Open
                || string.IsNullOrWhiteSpace(_oauthUserName)) return;
            // call sp_set_session_context
            if (!(sender is DbConnection db)) return;
            SetSessionContext(
                db,
                DbSessionContextKeyOauthUserName.Value,
                _oauthUserName,
                true);
        }

        #endregion

        #region navigation helper methods

        public IEnumerable<NavigationProperty> GetNavigationProperties<T>(
            T entity = default(T)) where T : class
        {
            var oc = ((IObjectContextAdapter)this).ObjectContext;
            var type = typeof(T);
            if (type.Name.ToLower() == "object")
                type = entity?.GetType();
            var entityType = oc.MetadataWorkspace
                .GetItems(DataSpace.OSpace).OfType<EntityType>()
                .FirstOrDefault(et => et.Name == type?.Name);
            return entityType != null
                ? entityType.NavigationProperties
                : Enumerable.Empty<NavigationProperty>();
        }

        public Dictionary<string, object> RemoveNavigationProperties<T>(
            List<NavigationProperty> navProps,
            T entity = default(T)) where T : class
        {
            // get navigation properties' values
            var navPropsValues = navProps
                .Select(a => new KeyValuePair<string, object>(
                    a.Name,
                    entity?.GetType().GetProperty(a.Name)?.GetValue(entity)))
                .ToList()
                .ToDictionary(a => a.Key, a => a.Value);
            // clear entity's navigation properties
            navProps.ForEach(a =>
            {
                entity?.GetType()
                    .GetProperty(a.Name)
                    ?.SetValue(entity, null);
            });
            return navPropsValues;
        }

        public T RestoreNavigationProperties<T>(
            List<NavigationProperty> navProps,
            Dictionary<string, object> navPropsValues,
            T entity = default(T)) where T : class
        {
            navProps.ForEach(a =>
            {
                entity?.GetType()
                    .GetProperty(a.Name)
                    ?.SetValue(entity, navPropsValues[a.Name]);
            });
            return entity;
        }

        public T SaveEntityWithoutNavigationProperties<T>(
            T entity = default(T)) where T : class
        {
            // store entity's navigation properties
            var navProps = GetNavigationProperties(entity).ToList();
            var navPropsValues = RemoveNavigationProperties(navProps, entity);
            // add entity
            Entry(entity).State = EntityState.Added;
            SaveChanges();
            // restore entity's navigation properties
            return RestoreNavigationProperties(navProps, navPropsValues, entity);
        }

        public async Task<T> SaveEntityWithoutNavigationPropertiesAsync<T>(
            T entity = default(T)) where T : class
        {
            // store entity's navigation properties
            var navProps = GetNavigationProperties(entity).ToList();
            var navPropsValues = RemoveNavigationProperties(navProps, entity);
            // add entity
            Entry(entity).State = EntityState.Added;
            await SaveChangesAsync();
            // restore entity's navigation properties
            return RestoreNavigationProperties(navProps, navPropsValues, entity);
        }

        #endregion
    }
}
