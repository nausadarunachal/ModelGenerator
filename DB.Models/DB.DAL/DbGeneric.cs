using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using DGDean.Models.Base;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations.Schema;
using DGDean.Utils.Linq;
using System.Data.SqlClient;
using RxSense.Models;

namespace RxSense.DAL
{

    public class DBGeneric
    {
        private static readonly Dictionary<string, string[]> EffectiveDateMatchingCriteria = new Dictionary<string, string[]>
        {
            {"dr_AccumulatorRule", new[] {"AccumulatorSetId", "AccumulatorTypeId","AccumulatorCoverageTypeId"}},
            {"dr_AccumulatorPatientLevelRuleLimit", new[] { "MemberCardIdentityId","AccumulatorTypeId","AccumulatorBenefitPeriodId"}},
            {"dr_Account", new[] {"AccountNumber", "CarrierId"}},
            {"dr_BenefitDesignNetworkPricingSet", new[] {"BenefitDesignId", "NetworkPricingSetId","SuperNetworkId"}},
            {"dr_Bin", new[] {"Bin"}},
            {"dr_BaseControlledSubstanceRule", new[] { "BaseControlledSubstanceRuleTypeId", "ClinicalRuleSetId", "IsExcluded","DrugClass"}},
            {"dr_Carrier", new[] {"CarrierNumber", "OrganizationId"}},
            {"dr_ChainCodeProvider", new[] {"ChainCodeId", "ProviderId", "NcpdpChainCode"}},
            {"dr_ChainProvider", new[] {"ChainId", "ProviderId", "NcpdpChainCode", "Nabp"}},
            {"dr_ClinicalRule", new[] {"ClinicalRuleTypeId", "ClinicalRuleSetId", "Gpi", "Ndc", "FormularyId", "Mony", "RxCovered", "OtcCovered"}},
            {"dr_CopaymentRule", new[] {"CopaymentRuleSetId", "DaysSupply"}},//, "FormularyTier", "PharmacyNetworkTypeId"}},            
            {"dr_CopayTier", new[] {"CopayTierSetId","MaxAmount","IsPlanPay"}},
            {"dr_CostCalculation", new[] {"CostCalculationSetId", "CostCompare"}},
            {"dr_DispensingFeeTier", new[] {"DispensingFeeTierSetId", "MaxAmount"}},
            {"dr_AdminFeeTier", new[] { "AdminFeeTierSetId"}},
            {"dr_EntityCoverage", new [] {  "EntityId" , "EntityTypeId" ,"NetworkId" , "FormularyId" } },
            {"dr_EntityConfiguration", new[] {"EntityConfigurationTypeId", "EntityTypeId", "EntityId", "State", "Gpi"}},
            {"dr_EntityCopaymentRuleSet", new[] {"EntityTypeId", "EntityId", "CopaymentRuleSetId"}},
            {"dr_EntityPrescriberNetwork", new[] {"EntityTypeId", "EntityId", "PrescriberNetworkId"}},
            {"dr_FormularyDrug", new[] { "FormularyId", "Gpi", "Ndc", "Mony", "OtcCovered", "RoaId"}},
            {"dr_FormularyTierRule" , new [] { "Name" } },
            {"dr_Group", new[] {"GroupNumber", "AccountId", "PcnId"}},
            {"dr_MacListDrug", new[] {"MacListId", "Gpi", "Ndc"}},
            {"dr_MemberCard", new[] {"MemberCardIdentityId", "MemberId", "CardId", "PersonCode", "RelationshipCodeTypeId", "MemberCode"}},
            {"dr_NetworkChain", new[] {"NetworkId", "ChainId"}},
            {"dr_NetworkPricing", new[] {"NetworkPricingSetId", "PriceRuleSetId", "MacListId"}},
            //{"dr_NetworkProvider", new[] {"NetworkId", "ProviderId", "NcpdpChainCode", "Nabp"}},
            {"dr_NetworkProvider", new[] {"NetworkId", "ProviderSetId","ChainCodeId"}},
            {"dr_Organization", new[] {"OrganizationNumber"}},
            {"dr_PaymentCenter", new[] {"CodeNumber", "Nabp", "NcpdpChainCode"}},
            {"dr_Pcn", new[] {"Pcn", "BinId"}},
            {"dr_PharmacyNetworkType", new[] {"Name", "Description"}},
            {"dr_Prescriber", new[] {"Hcid"}},
            {"dr_PrescriberDeaDrugSchedule", new[] {"PrescriberDeaNumberId"}},
            {"dr_PrescriberDeaNumber", new[] {"Hcid", "DeaNumber"}},
            {"dr_PrescriberLocation", new[] {"Hcid", "AddressId", "PhoneNumber", "FaxNumber"}},
            {"dr_PrescriberNetworkPrescriber", new[] {"PrescriberNetworkId", "Hcid"}},
            {"dr_PrescriberNpiNumber", new[] {"Hcid"}},
            {"dr_PrescriberSpecialty", new[] {"Hcid"}},
            {"dr_PrescriberStateLicense", new[] {"Hcid"}},
            {"dr_PrescriberTaxonomy", new[] {"Hcid"}},
            {"dr_PriceRule", new[] {"FormularyId", "Mony"}},
            //{"dr_PriceRule", new[] {"FormularyId", "Mony", "CostCalculationSetId", "CopayTierSetId", "DispensingFeeTierSetId"}},
            {"dr_PriorAuthorizationHistory", new[] {"Ndc", "Gpi"}},
            {"dr_AdminOverride", new[] {"OverrideTypeId", "Ndc", "Gpi", "MemberCardId"}},
            {"dr_AmountCalculationRule",new [] { "AmountCalculationRuleSetId"} },
            {"dr_EntityDawConfiguration", new[] {"EntityTypeId", "EntityId"}},
            {"dr_SuperNetworkPricingSet",new[] { "SuperNetworkId", "NetworkPricingSetId"}},
            {"dr_EntityBenefitDesign", new[] {"EntityTypeId", "EntityId", "BenefitDesignId", "MemberCode"}},
            {"dr_EntityTradeScheduleConfiguration", new[] {"EntityTypeId", "EntityId", "MacListId"}},
            {"dr_ProviderSetChainCodeInternalBillingGroup", new[] {"ProviderSetId", "ChainCodeId", "NcpdpChainCode"}}
        };

        /// <summary>
        /// Given an EffDated model, finds all DB records
        /// that satisfy matching criteria, per EffectiveDateMatchingCriteria
        /// (i.e. finda all "versions" of this record). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="filter"></param>
        /// <param name="useWriteDb"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetEffectiveMatches<T>(T model,
            bool useWriteDb = false,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes)
            where T : BaseDateRangeModel
        {
            var type = typeof(T); //model.GetType();
            var tablettribute = (TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute));
            var matchingCriteria = EffectiveDateMatchingCriteria[tablettribute.Name];

            var predicate = PredicateBuilder.True<T>();
            foreach (var matchingCriterion in matchingCriteria)
            {
                var prop = type.GetProperty(matchingCriterion);

                if (prop == null) { throw new Exception("EffectiveDateMatchingCriteria lookup failed."); }

                var propVal = prop.GetValue(model);
                var param = Expression.Parameter(typeof(T), "x");           //"x =>"
                var expr = Expression.Lambda<Func<T, bool>>(                 //x.param
                    Expression.Equal(                                       // == 
                        Expression.Property(param, matchingCriterion),      //propVal
                        Expression.Convert(Expression.Constant(propVal),
                            prop.PropertyType)),
                    param);

                predicate = predicate.And(expr);
            }

            if (filter != null)
            {
                predicate = predicate.And(filter);
            }

            using (var db = new RxSenseDb(useWriteDb))
            {
                var query = db.Set<T>()
                    .AsQueryable();

                foreach (var include in includes)
                {
                    query.Include(include);
                }

                return query
                    .Where(predicate)
                    .ToList();  //concrete the results since this db context will be disposed
            }
        }

        /// <summary>
        /// Given an EffDated model, finds all daterange overlaps in DB
        /// that also satisfy matching criteria, per EffectiveDateMatchingCriteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetConflicts<T>(T model,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes)
            where T : BaseDateRangeModel
        {
            var matches = GetEffectiveMatches(model, useWriteDb: false, filter: filter, includes: includes);
            return matches.GetConflictingItems(model);
        }



        /// <summary>
        /// Given a list of effDated models find all daterange overlaps in DB,
        /// that also satisfy matching criteria per EffectiveDateMatchingCriteria,
        /// for each model, and exclude the argument models themselves  by IDateRangeId
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetConflicts<T>(IEnumerable<T> models,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes)
            where T : BaseDateRangeModel
        {
            var modelsList = models.ToList();
            var conflicts = new List<T>();
            foreach (var model in modelsList)
            {
                conflicts.AddRange(GetConflicts(model, filter: filter, includes: includes)
                    .Where(x => !modelsList
                        .Select(y => y.IDateRangeId)
                        .Contains(x.IDateRangeId)));
            }

            return conflicts;
        }

        /// <summary>
        /// Given a T model, find Db record based on EffectiveDateMatchingCriteria
        /// and IDateRangeId (PK Id), and set record to inactive if not already so.
        /// Also record DeactivatedDate as UtcNow.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="useWriteDb"></param>
        /// <returns></returns>
        public static T Deactivate<T>(T model, bool useWriteDb = false) where T : BaseDateRangeModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                var matches = GetEffectiveMatches(model);
                var dbModel = matches.FirstOrDefault(x => x.IDateRangeId == model.IDateRangeId);
                if (dbModel != null)
                {
                    db.Set<T>().Attach(dbModel);    //because GetEffectiveMatches db context is out of scope
                    dbModel.IsActive = false;
                    dbModel.DeactivatedDate = DateTime.UtcNow;

                    db.SaveChanges();
                }

                return dbModel;
            }
        }

        public static IEnumerable<T> Deactivate<T>(IEnumerable<T> models, bool useWriteDb = false) where T : BaseDateRangeModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                foreach (var model in models)
                {
                    model.IsActive = false;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.DeactivatedDate = model.ModifiedDate;                   

                    db.Entry(model).State = EntityState.Modified;
                } 

                db.SaveChanges();
            }               

            return models;
        }

        public static T DeactivateConflicts<T>(T model, IEnumerable<T> conflicts, bool useWriteDb = false) where T : BaseDateRangeModel
        {
            var newModel = Add(model);
            Deactivate(conflicts);

            return newModel;
        }

        public static IEnumerable<T> EndDateConflicts<T>(T model, IEnumerable<T> conflicts, bool useWriteDb = false) where T : BaseDateRangeModel
        {
            var replacements = new List<T>() { model };
            
            foreach (var conflict in conflicts)
            {
                if (conflict.StartDate < model.StartDate)
                {
                    var leftOverlap = conflict;
                    leftOverlap.IsActive = true;
                    leftOverlap.EndDate = model.StartDate;
                    leftOverlap.ModifiedDate = DateTime.UtcNow;
                    leftOverlap.DeactivatedDate = null;
                    replacements.Add(leftOverlap);
                }
                if (conflict.EndDate > model.EndDate)
                {
                    var rightOverlap = conflict;
                    rightOverlap.IsActive = true;
                    rightOverlap.StartDate = model.EndDate;
                    rightOverlap.ModifiedDate = DateTime.UtcNow;
                    rightOverlap.DeactivatedDate = null;
                    replacements.Add(rightOverlap);
                }
            }

            AddRange(replacements);
            Deactivate(conflicts);

            return replacements;
        }

        /// <summary>
        /// Mechanism to upbiquitiously end date conflicting records,
        /// i.e. inactivate conflict and add new record with properly tweaked daterange.
        /// Also saves the model we are end-date resolving conflicts for.
        /// TODO WIP
        /// TODO discern proper edge/failure cases, test all cases.
        /// TODO check performance impact this has (especially if used in bulk, like via loaders).
        /// TODO leave as private/unused until above all "TODO"s are addressed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static /*IEnumerable<T>*/void TerminateConflicts<T>(T model,
            bool createLeftProtrusion = true,
            bool createRightProtrusion = false,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes)
            where T : BaseDateRangeModel
        {
            var matches = GetEffectiveMatches(model, useWriteDb: false, filter: filter, includes: includes);
            var conflictsToModel = matches
                .GetConflictingItems(model)
                .OrderByDescending(x => x.IDateRangeId)
                .ToList();

            /*
             * TODO maybe recursively(recursion is scary, especially when talking to DB) pull in additional conflicts
             * that perhapes model doesnt conflict with, but other items in conflicts list do?
             * I hesitate/question doing this because it complicates checking the no protrusion case,
             * as it could add gaps in the conflicts list. Currenttly, they all atleast conflict with model,
             * thus making a continous body of timeline overlaps
             **/
            //var conflictsToConflicts = conflictsToModel.AsEnumerable();
            //foreach (var conflict in conflictsToModel)
            //{
            //    conflictsToConflicts = conflictsToConflicts.Union(matches.GetConflictingItems(conflict));
            //}

            var replacements = new List<T> { model };
            foreach (var conflict in conflictsToModel)
            {
                //TODO check no conflict records with higher IDateRangeId fully cover the span of this conflict's daterange.
                //Maybe two or more records that combined complete the full covering/hiding, so check start/end separately.
                if (replacements.Any(x => x.StartDate < conflict.StartDate) &&
                    replacements.Any(x => x.EndDate > conflict.EndDate))
                    continue;

                //TODO maybe check for left/right protrusion against all items in replacements instead of just model?
                if (createLeftProtrusion && conflict.StartDate < model.StartDate) //left protrusion
                {
                    //TODO since conflict already has IDateRangeId (already exists in DB), need to
                    //attach replacement as new to dbset, and inactivate db record this is a replacement for.
                    //Method SaveConflictResolution may handle this as needed.
                    var replacement = conflict;

                    // Only contract endDate if it extends past any in replacements so far.
                    // The IDateRangeId greater than check is handled by ordering conflicts list desc before loop
                    if (replacements //.Where(x => x.IDateRangeId > replacement.IDateRangeId)
                        .Any(x => x.StartDate < replacement.EndDate))
                    {
                        replacement.EndDate = replacements.Min(x => x.StartDate); //model.StartDate on first iteration
                    }

                    replacements.Add(replacement);
                }
                if (createRightProtrusion && conflict.EndDate > model.EndDate) //right protrusion
                {
                    var replacement = conflict;
                    if (replacements //.Where(x => x.IDateRangeId > replacement.IDateRangeId)
                        .Any(x => x.EndDate > replacement.StartDate))
                    {
                        replacement.StartDate = replacements.Max(x => x.EndDate); //model.EndDate on first iteration
                    }
                    replacements.Add(replacement);
                }
            }

            var getMatches = GetEffectiveMatches(model, useWriteDb: false, filter: filter, includes: includes);
            var conflictsToDeactivate = getMatches
                .GetConflictingItems(model)
                .OrderByDescending(x => x.IDateRangeId)
                .ToList();
            //this will add the new model with IDateRangeId of 0 as well as deactivate
            //the ones that have a nonzero IDateRangeId and add their new "replacement"
            SaveConflictResolution(replacements);

            //Directly deactivate all conflicts (old records) because the ones that were fully hidden may not
            //have had a counterpart in replacements to be deactivated from via SaveConflictResolution call.
            //Must do this after applying SaveConflictResolution on replacements otherwise the lack of finding 
            //a DB record with that (nonzero) IDateRangeId will cause the save of the replacement to skip.
            //Commented out this logic as it is deactivating the active records as well.
            foreach (var conflict in conflictsToDeactivate)
            {
                //TODO adjust Deactivate and SaveConflictResolution methods to not bother deactivating
                //already deactivated records (may save incorrect deactivatedDate on repeats).
                //TODO maybe tweak saveConflicts call to allow adding a record with nonzro IDateRangeId
                //even if an existing active record is not found in DB.
                Deactivate(conflict);
            }
        }

        /// <summary>
        /// Deactivate all models that have been "resolved", i.e. date range adjusted, or inactivated.
        /// Add new records for "resolved" rows. Add the new record that hit all the conflicts to begin with.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <param name="useWriteDb"></param>
        /// <param name="isUpdateOnly"></param>
        /// <returns></returns>
        public static IEnumerable<T> SaveConflictResolution<T>(IEnumerable<T> models, bool useWriteDb = false, bool isUpdateOnly = false) where T : BaseDateRangeModel
        {
            var modelsList = models.ToList();

            // Can't premptive check here because some BO's have extended matching criteria that would get ignored.
            // Do it in BO layer. See CopayRuleBO.GetConflicts for example.
            //var hasPendingConflicts = GetConflicts<T>(modelsList).Any();
            //if (hasPendingConflicts)
            //{
            //    throw new Exception("Cannot save this resolution because some conflicts remain.");
            //}

            using (var db = new RxSenseDb(useWriteDb))
            {
                foreach (var model in modelsList)
                {
                    if (!isUpdateOnly && model.IDateRangeId == 0) //add new model only if isUpdateOnly is false.
                    {
                        db.Set<T>().Add(model);
                        continue;
                    }
                    var dbModel = db.Set<T>().Find(model.IDateRangeId);
                    if (dbModel != null                             //found a dbModel
                        && (dbModel.StartDate != model.StartDate    //and there is some edit to this dbModel
                            || dbModel.EndDate != model.EndDate
                            || dbModel.IsActive != model.IsActive))
                    {
                        //deactivate, regardless of if this is a deactivation or a date adjustment
                        //inactives stay inactive and keep deactivated date as is
                        dbModel.IsActive = false;
                        dbModel.DeactivatedDate = dbModel.DeactivatedDate ?? DateTime.UtcNow;
                        if (model.IsActive &&                       //not a deactivate; maybe a "reactivate", still yields new record
                            (dbModel.StartDate != model.StartDate   //or is an effDateRange adjustment
                             || dbModel.EndDate != model.EndDate))
                        {
                            db.Set<T>().Add(model);
                        }
                    }
                }

                db.SaveChanges();
            }
            //Set DeactivatedDate and ModifiedDate for return 
            foreach (var mdl in modelsList.Where(x => x.IsActive == false))
            {
                mdl.DeactivatedDate = mdl.DeactivatedDate ?? DateTime.UtcNow;
                mdl.ModifiedDate = DateTime.UtcNow;
            }
            return modelsList;
        }

        /// <summary>
        /// True indicates no resolution conflicts amongst the argument records.
        /// Should not hit DB, just check among the models given.
        /// </summary>
        /// <param name="models"></param>
        /// <param name="useWriteDb"></param>
        /// <returns></returns>
        public static bool CheckConflictResolution<T>(IEnumerable<T> models) where T : BaseDateRangeModel
        {
            var list = models.ToList();

            //check no records conflict amongst themselves
            for (var i = 0; i < list.Count - 1; i++)
            {
                var item1 = list[i];
                var type = typeof(T); //item1.GetType();
                var tablettribute = (TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute));
                var matchingCriteria = EffectiveDateMatchingCriteria[tablettribute.Name];

                for (var j = i + 1; j < list.Count; j++)
                {
                    var item2 = list[j];
                    var matchingCriteriaAccumulation = true;
                    foreach (var matchingCriterion in matchingCriteria)
                    {
                        var prop = type.GetProperty(matchingCriterion);

                        if (prop == null) { throw new Exception("EffectiveDateMatchingCriteria lookup failed."); }

                        var propVal1 = prop.GetValue(item1);
                        var propVal2 = prop.GetValue(item2);
                        matchingCriteriaAccumulation =
                            matchingCriteriaAccumulation && object.Equals(propVal1, propVal2);
                    }

                    if (matchingCriteriaAccumulation && item1.IsDateRangeOverlap(item2))
                    {
                        return false;   //if any pair conflicts, fail
                    }
                }
            }
            return true;    //if no pairs conflict, pass
        }

        public static T Add<T>(
            T model,
            bool useWriteDb = false)
            where T : BaseModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                db.Set<T>().Add(model);

                db.SaveChanges();

                return model;
            }
        }

        public static async Task<T> AddAsync<T>(
            T model) where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                db.Set<T>().Add(model);
                await db.SaveChangesAsync();
                return model;
            }
        }

        public static List<T> AddRange<T>(List<T> models) where T : BaseModel
        {
            if (models?.Count > 0)
            {
                using (var db = new RxSenseDb())
                {
                    db.Set<T>().AddRange(models);
                    db.SaveChanges();
                }
            }
            return models;
        }

        public static T Update<T>(
            T model,
            bool useWriteDb = false)
            where T : BaseModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                db.Set<T>().Attach(model);
                db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();

                return model;
            }
        }

        public static async Task<T> UpdateAsync<T>(T model) where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                db.Set<T>().Attach(model);
                db.Entry(model).State = EntityState.Modified;

                await db.SaveChangesAsync();

                return model;
            }
        }

        public static T AddOrUpdate<T>
            (T model,
            bool useWriteDb = false)
            where T : BaseModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                db.Set<T>().AddOrUpdate(model);

                db.SaveChanges();

                return model;
            }
        }

        public static async Task<T> AddOrUpdateAsync<T>(T model, bool useWritedb = true) where T : BaseModel
        {
            using (var db = new RxSenseDb(useWritedb))
            {
                db.Set<T>().AddOrUpdate(model);

                await db.SaveChangesAsync();

                return model;
            }
        }

        public static T Get<T>(int id) where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                var item = db.Set<T>().Find(id);

                return item;
            }
        }

        public static async Task<T> GetAsync<T>(int id) where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                var item = await db.Set<T>().FindAsync(id);

                return item;
            }
        }

        public static T Get<T>(
            Expression<Func<T, bool>> predicate,
            bool useWriteDb = false,
            params Expression<Func<T, object>>[] includes)
            where T : BaseModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                var dbSet = db.Set<T>().AsQueryable();

                foreach (var property in includes)
                {
                    dbSet = dbSet.Include(property);
                }

                return dbSet.Where(predicate).SingleOrDefault();
            }
        }

        public static async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                var dbSet = db.Set<T>().AsQueryable();

                foreach (var property in includes)
                {
                    dbSet = dbSet.Include(property);
                }

                var result = await dbSet.SingleOrDefaultAsync(predicate);

                return result;
            }
        }

        public static List<T> GetAll<T>() where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                var results = db.Set<T>().ToList();

                return results;
            }
        }

        public static List<T> Query<T>(
            Expression<Func<T, bool>> predicate,
            bool useWriteDb = false,
            params Expression<Func<T, object>>[] includes)
            where T : BaseModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                var dbSet = db.Set<T>().AsQueryable();

                foreach (var property in includes)
                {
                    dbSet = dbSet.Include(property);
                }

                return dbSet.Where(predicate).ToList();
            }
        }

        public static int Count<T>(
            Expression<Func<T, bool>> predicate,
            bool useWriteDb = false)
            where T : BaseModel
        {
            using (var db = new RxSenseDb(useWriteDb))
            {
                var dbSet = db.Set<T>().AsQueryable();
                return dbSet.Where(predicate).Count();
            }
        }

        public static bool Contains<T>(Func<T, string> property, string value) where T : BaseModel
        {
            using (var db = new RxSenseDb())
            {
                var items = db.Set<T>().Select(property);

                return items.Contains(value, StringComparer.OrdinalIgnoreCase);
            }
        }

        public static bool Contains(string fieldName, string idName, string tableName, string valueToCheck, int idValue)
        {
            using (var db = new RxSenseDb())
            {
                var exists = db.Database.SqlQuery<int>(@"CheckUniqueness @FieldName,
                                                                         @IdField,
                                                                         @TableName,
                                                                         @ValueToCheck,
                                                                         @IdBeingUpdated",
                                   new SqlParameter("@FieldName", fieldName),
                                   new SqlParameter("@IdField", idName),
                                   new SqlParameter("@TableName", tableName),
                                   new SqlParameter("@ValueToCheck", valueToCheck),
                                   new SqlParameter("@IdBeingUpdated", idValue)).Single();

                return exists == 1;
            }
        }

    }
}