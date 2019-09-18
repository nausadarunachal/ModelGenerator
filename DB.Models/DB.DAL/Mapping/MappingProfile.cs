namespace RxSense.DAL.Mapping
{
    using AutoMapper;
    using Models;
    using Models.DB;
    using System;
    using System.Collections.Generic;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Contracts.Claim.Search to DAL.ProcParms

            CreateMap<ClaimSearchRequest, ClaimSearchCardInfoParms>();
            CreateMap<ClaimSearchRequest, ClaimSearchRxInfoParms>();
            CreateMap<ClaimSearchRequest, ClaimSearchMemberInfoParms>();
            CreateMap<ClaimSearchRequest, ClaimSearchByClaimIdParms>();
            CreateMap<ClaimSearchResult, ClaimSearchResponse>()
                .ForMember(x => x.IsDrugInMedicationConsent, o => o.Ignore())
                .ForMember(x => x.ReversalClaimIds, o => o.Ignore())
                .ForMember(x => x.IsTest, o => o.MapFrom(x => (x.ClaimTypeId == (int)ClaimType.TestClaim) ? 1 : 0));

            #endregion

            #region Contracts.Provider to DAL.ProcParams

            CreateMap<ProviderSearchRequest, ProviderSearchParams>()
                .ForMember(x => x.Nabps, o => o.MapFrom(x => string.Join(",", x.Nabps ?? new List<string>())))
                .ForMember(x => x.ChainCodes, o => o.MapFrom(x => string.Join(",", x.ChainCodes ?? new List<string>())))
                .ForMember(x => x.GroupNumbers, o => o.MapFrom(x => string.Join(",", x.GroupNumbers ?? new List<string>())))
                .ForMember(x => x.NetworkIds, o => o.MapFrom(x => string.Join(",", x.NetworkIds ?? new List<int>())));

            CreateMap<ProviderSearchBasicRequest, ProviderSearchBasicParams>();

            CreateMap<MemberProviderSearchRequest, MemberProviderSearchParams>()
              .ForMember(x => x.Nabps, o => o.MapFrom(x => string.Join(",", x.Nabps ?? new List<string>())))
             .ForMember(x => x.ChainCodes, o => o.MapFrom(x => string.Join(",", x.ChainCodes ?? new List<string>())))
             .ForMember(x => x.GroupNumbers, o => o.MapFrom(x => string.Join(",", x.GroupNumbers ?? new List<string>())))
             .ForMember(x => x.NetworkIds, o => o.MapFrom(x => string.Join(",", x.NetworkIds ?? new List<int>())));

            CreateMap<ProviderSearchResult, ProviderSearchResponse>()
                .ForMember(x => x.Address, o => o.Ignore())
                .AfterMap((t, s) =>
                {
                    if (!string.IsNullOrWhiteSpace(t.Address1))
                    {
                        var addr = new Contracts.Address.Address()
                        {
                            Address1 = t.Address1,
                            Address2 = t.Address2,
                            City = t.City,
                            State = t.State,
                            PostalCode = t.ZipCode,
                            Latitude = t.Latitude,
                            Longitude = t.Longitude
                        };
                        s.Address = addr;
                    }

                });

            
            #endregion

            #region Contracts.GroupMembers to DAL.ProcParams

            CreateMap<MemberSearchRequest, MemberSearchParams>()
                .ForMember(x => x.DOB, o => o.Ignore())
                .AfterMap((t, s) =>
                {
                    if (!String.IsNullOrWhiteSpace(t.Dob) && DateTime.TryParse(t.Dob, out DateTime dob))
                    {
                        s.DOB = dob;
                    }
                });

            CreateMap<GroupMembersSearchRequest, GroupMemberSearchParams>()
                .ForMember(x => x.PointInTime, o => o.Ignore())
                .AfterMap((t, s) =>
                {
                    if (!String.IsNullOrWhiteSpace(t.PointInTime) && DateTime.TryParse(t.PointInTime, out DateTime pointInTime))
                    {
                        s.PointInTime = pointInTime;
                    }
                });

            CreateMap<MemberSearchResult, MemberSearchResponse>()
                .ForMember(x => x.MemberId, opt => opt.MapFrom(o => o.MemberId))
                .ForMember(x => x.StartDate, opt => opt.MapFrom(o => o.StartDate))
                .ForMember(x => x.AddressId, opt => opt.MapFrom(o => o.AddressId))
                .ForMember(x => x.GroupNumber, opt => opt.MapFrom(o => o.GroupNumber))
                .ForMember(x => x.MemberNumber, opt => opt.MapFrom(o => o.MemberNumber))
                .ForMember(x => x.MemberRepName, opt => opt.MapFrom(o => o.MemberRepName))
                .ForMember(x => x.MemberId, opt => opt.MapFrom(o => o.MemberId))
                .ForMember(x => x.IsEmployee, opt => opt.MapFrom(o => o.IsEmployee))
                .ForMember(x => x.Address, o => o.Ignore())
                .ForMember(x => x.RelationshipCode, opt => opt.MapFrom(o => o.RelationshipCode))
                .AfterMap((t, s) =>
                {
                    if (!string.IsNullOrWhiteSpace(t.Address1))
                    {
                        var addr = new Contracts.Member.Address()
                        {
                            AddressId = t.AddressId,
                            Address1 = t.Address1,
                            Address2 = t.Address2,
                            City = t.City,
                            State = t.State,
                            Zip = t.PostalCode
                        };
                        s.Address = addr;
                    }

                });
            #endregion

            #region Contracts.Provider to ProviderModel
            CreateMap<ProviderSearchBasicResponse, Contracts.Provider.Provider>()
                .ForMember(x => x.Address, o => o.Ignore())
                .ForMember(x => x.StoreCodeStartDate, opt => opt.MapFrom(o => o.StoreStartDate))
                .ForMember(x => x.StoreCodeEndDate, opt => opt.MapFrom(o => o.StoreEndDate))
                .ForMember(d => d.ChainCodeId, o => o.Ignore())
                .AfterMap((r, p) =>
                {
                    if (r.AddressId > 0)
                    {
                        var addr = new Contracts.Address.Address()
                        {
                            AddressId = r.AddressId,
                            Address1 = r.Address1,
                            Address2 = r.Address2,
                            City = r.City,
                            State = r.State,
                            PostalCode = r.PostalCode,
                        };
                        p.Address = addr;
                    }

                });
            #endregion

            #region Contracts.BINContract.Search to DAL.ProcParams
            CreateMap<BinPcnSearchRequest, BinPcnSearchParams>();

            CreateMap<BinPcnSearchResult, BinPcnSearchResponse>()
                .ForMember(x => x.AdjudicationPermitted, y => y.MapFrom(z => z.IsActive));
            #endregion

            #region Contracts.Network to DAL.ProcParams
            CreateMap<NetworkSearchProvidersRequest, NetworkSearchForProvidersParams>()
                .ForMember(x => x.AddedByChainCodeFilter, y => y.MapFrom(z => z.AddedByChainCodeFilter))
                .ForMember(x => x.NcpdpChainCode, y => y.MapFrom(z => z.ChainCode));

            CreateMap<NetworkSearchForProvidersResult, NetworkSearchProvidersResult>()
                .ForMember(x => x.AddedByChainCode, y => y.MapFrom(z => z.AddedByChainCode))
                .ForMember(x => x.ChainCode, y => y.MapFrom(z => z.NcpdpChainCode))
                .ForMember(x => x.NetworkProviderStartDate, y => y.MapFrom(z => z.StartDate))
                .ForMember(x => x.NetworkProviderEndDate, y => y.MapFrom(z => z.EndDate))
                .ForMember(x => x.DeactivatedDate, y => y.MapFrom(z => z.DeactivatedDate))
                .ForMember(x => x.IsActive, o => o.Ignore());

            #endregion

            #region Contracts.MacList to DAL.ProcParams

            CreateMap<MacListDrugSearchRequest, MacListDrugSearchCurrentParams>()
                .ForMember(x => x.UsePaging, o => o.MapFrom(s => true));

            #endregion

            #region DAL.ProcResults to Contracts.Provider

            CreateMap<ProviderSearchBasicResult, ProviderSearchBasicResponse>();

            #endregion

            #region Models.DB to DAL.ElasticSearch

            CreateMap<FormularyModel, ElasticSearch.Models.FormularyNameSearch>()
                .ForMember(x => x.FormularyName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.FormularyId, o => o.MapFrom(x => x.FormularyId));

            CreateMap<FormularySetModel, ElasticSearch.Models.FormularySetNameSearch>()
                .ForMember(x => x.FormularySetName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.FormularySetId, o => o.MapFrom(x => x.FormularySetId));

            CreateMap<ProviderModel, ElasticSearch.Models.Provider>()
                .ForMember(d => d.Nabp, o => o.MapFrom(x => x.ProviderSet.Nabp))
                .ForMember(d => d.Npi, o => o.MapFrom(x => x.ProviderSet.Npi));

            CreateMap<MedispanModel, ElasticSearch.Models.Product>()
                .ForMember(x => x.GenericName, o => o.MapFrom(x => x.GpiGenericName))
                .ForMember(x => x.Gpi, o => o.MapFrom(x => x.Gpi))
                .ForMember(x => x.MultisourceCode, o => o.MapFrom(x => x.MultisourceCode))
                .ForMember(x => x.Ndc, o => o.MapFrom(x => x.Ndc))
                .ForMember(x => x.RePackaged, o => o.MapFrom(x => x.RepackInd))
                .ForMember(x => x.ItemStatus, o => o.MapFrom(x => x.ItemStatus))
                .ForMember(x => x.RxOtcCode, o => o.MapFrom(x => x.RxOtcCd))
                .ForMember(x => x.ProductName, o => o.MapFrom(x => x.ProductNameTxt))
                .ForMember(x => x.ProductStrength, o => o.MapFrom(x => x.StrengthTxt))
                .ForMember(x => x.ProductForm, o => o.Ignore())
                .ForMember(x => x.DrugName, o => o.Ignore())
                .ForMember(x => x.GroupClassName, o => o.Ignore())
                .ForMember(x => x.GroupName, o => o.Ignore())
                .ForMember(x => x._id, o => o.Ignore())
                .ForMember(x => x.SubClassName, o => o.Ignore());

            CreateMap<PriceRuleSetModel, ElasticSearch.Models.PricingRuleSetNameSearch>()
                .ForMember(x => x.PricingRuleSetName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.PricingTypeId, o => o.MapFrom(x => x.PricingTypeId))
                .ForMember(x => x.PricingRuleSetId, o => o.MapFrom(x => x.PriceRuleSetId));

            CreateMap<NetworkModel, ElasticSearch.Models.PharmacyNetworkNameSearch>()
                .ForMember(x => x.PharmacyNetworkName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.PharmacyNetworkId, o => o.MapFrom(x => x.NetworkId));

            CreateMap<NetworkPricingSetModel, ElasticSearch.Models.NetworkPricingSetNameSearch>()
                .ForMember(x => x.NetworkPricingSetName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.NetworkPricingSetId, o => o.MapFrom(x => x.NetworkPricingSetId));

            CreateMap<MacListModel, ElasticSearch.Models.MacListNameSearch>()
                .ForMember(x => x.MacListName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.MacListId, o => o.MapFrom(x => x.MacListId));

            CreateMap<CostCalculationSetModel, ElasticSearch.Models.CostCalculationSetNameSearch>()
                .ForMember(x => x.CostCalculationSetName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.CostCalculationSetId, o => o.MapFrom(x => x.CostCalculationSetId));

            CreateMap<AmountCalculationRuleSetModel, ElasticSearch.Models.AmountCalculationRuleSetNameSearch>()
                .ForMember(x => x.AmountCalculationRuleSetName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.AmountCalculationRuleSetId, o => o.MapFrom(x => x.AmountCalculationRuleSetId));

            CreateMap<AmountCalculationRuleModel, ElasticSearch.Models.AmountCalculationRuleNameSearch>()
                .ForMember(x => x.AmountCalculationRuleName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.AmountCalculationRuleId, o => o.MapFrom(x => x.AmountCalculationRuleSetId));

            CreateMap<DispensingFeeTierSetModel, ElasticSearch.Models.DispensingFeeTierSetNameSearch>()
                .ForMember(x => x.DispensingFeeTierSetName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.DispensingFeeTierSetId, o => o.MapFrom(x => x.DispensingFeeTierSetId));

            CreateMap<BenefitDesignModel, ElasticSearch.Models.NetworkContractNumberSearch>()
                .ForMember(x => x.NetworkContractNumber, o => o.MapFrom(x => x.PlanNumber))
                .ForMember(x => x.BenefitDesignId, o => o.MapFrom(x => x.BenefitDesignId));

            CreateMap<SuperNetworkModel, ElasticSearch.Models.SuperNetworkNameSearch>()
                .ForMember(x => x.SuperNetworkName, o => o.MapFrom(x => x.Name))
                .ForMember(x => x.SuperNetworkId, o => o.MapFrom(x => x.SuperNetworkId));

            CreateMap<CopayTierSetModel, ElasticSearch.Models.MPPCalcTierSetNameSearch>()
                .ForMember(x => x.CopayTierSetId, o => o.MapFrom(x => x.CopayTierSetId))
                .ForMember(x => x.MPPCalcTierSetName, o => o.MapFrom(x => x.Name));
            CreateMap<AspNetRoleModel, ElasticSearch.Models.RoleNameSearch>()
            .ForMember(x => x.RoleName, o => o.MapFrom(x => x.Name))
            .ForMember(x => x.RoleId, o => o.MapFrom(x => x.AspNetRoleId));

            CreateMap<OrganizationModel, ElasticSearch.Models.UserHierarchyNameSearch>()
            .ForMember(x => x.UserHierarchyName, o => o.MapFrom(x => x.Name))
            .ForMember(x => x.UserHierarchyId, o => o.MapFrom(x => x.OrganizationId));

            #region Contracts.Formulary to DAL.ProcParams 

            CreateMap<FormularyDrugSearchRequest, FormularyDrugHistoryParams>()
                .ForMember(x => x.UsePaging, o => o.MapFrom(s => true));

            #endregion

            CreateMap<Models.DB.SingleCare.ProviderModel, ElasticSearch.Models.Prescriber>()
                .ForMember(x => x.PrescriberId, o => o.MapFrom(y => y.ProviderId))
                .ForMember(x => x.Coordinates, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.IsDeleted, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Coordinates = new ElasticSearch.Models.GeoPoint((double)s.Latitude, (double)s.Longitude);
                });

            CreateMap<Models.DB.SingleCare.SymptomModel, ElasticSearch.Models.Symptom>()
                .ForMember(x => x.Coordinates, o => o.Ignore());

            CreateMap<Models.DB.SingleCare.ConditionModel, ElasticSearch.Models.Condition>()
                .ForMember(x => x.Coordinates, o => o.Ignore())
                .ForMember(x => x.CreateDate, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.IsSynonym, o => o.Ignore());

            CreateMap<Models.DB.SingleCare.SpecialtyModel, ElasticSearch.Models.Specialty>()
                .ForMember(x => x.Coordinates, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.IsDeleted, o => o.Ignore());

            CreateMap<Models.DB.SingleCare.ReasonModel, ElasticSearch.Models.Reason>()
                .ForMember(x => x.Coordinates, o => o.Ignore());

            CreateMap<Models.DB.SingleCare.PrescriptionModel, ElasticSearch.Models.Prescription>()
                .ForMember(x => x.Coordinates, o => o.Ignore());

            CreateMap<Models.DB.SingleCare.LocationModel, ElasticSearch.Models.Location>()
                .ForMember(x => x.Coordinates, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.IsDeleted, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.Coordinates = new ElasticSearch.Models.GeoPoint((double)s.Latitude, (double)s.Longitude);
                });
            #endregion

            #region DAL.ElasticSearch.Models to Contracts.ElasticSearch

            CreateMap<
                ElasticSearch.Models.GeoPoint,
                Contracts.ElasticSearch.GeoPoint>();

            CreateMap<
                ElasticSearch.Models.Prescription,
                Contracts.ElasticSearch.Prescription>();

            #endregion

            #region DAL.ElasticSearch.Models to DAL.ElasticSearch.Models

            CreateMap<ElasticSearch.Models.ProductNameSearch, ElasticSearch.Models.Product>()
                .ForMember(x => x.ProductName, o => o.MapFrom(x => x.product_name))
                .ForMember(x => x.ProductForm, o => o.Ignore())
                .ForMember(x => x.DrugName, o => o.Ignore())
                .ForMember(x => x.GenericName, o => o.Ignore())
                .ForMember(x => x.Gpi, o => o.Ignore())
                .ForMember(x => x.Ndc, o => o.MapFrom(x => x.ndc))               
                .ForMember(x => x.ProductStrength, o => o.Ignore())
                .ForMember(x => x.GroupName, o => o.Ignore())
                .ForMember(x => x._id, o => o.Ignore())
                .ForMember(x => x.GroupClassName, o => o.Ignore())
                .ForMember(x => x.SubClassName, o => o.Ignore())
                .ForMember(x => x.MultisourceCode, o => o.Ignore())
                .ForMember(x => x.RxOtcCode, o => o.Ignore())
                .ForMember(x => x.RePackaged, o => o.Ignore())
                .ForMember(x => x.ItemStatus, o => o.Ignore());

            CreateMap<ElasticSearch.Models.HierarchyNameSearch, ElasticSearch.Models.Product>()
                .ForMember(x => x.GenericName, o => o.MapFrom(x => x.hierarchy_name))
                .ForMember(x => x.ProductForm, o => o.Ignore())
                .ForMember(x => x.DrugName, o => o.Ignore())
                .ForMember(x => x.ProductName, o => o.Ignore())
                .ForMember(x => x.Gpi, o => o.Ignore())
                .ForMember(x => x.Ndc, o => o.Ignore())
                .ForMember(x => x.ProductStrength, o => o.Ignore())
                .ForMember(x => x.GroupName, o => o.Ignore())
                .ForMember(x => x._id, o => o.Ignore())
                .ForMember(x => x.GroupClassName, o => o.Ignore())
                .ForMember(x => x.SubClassName, o => o.Ignore())
                .ForMember(x => x.MultisourceCode, o => o.Ignore())
                .ForMember(x => x.RxOtcCode, o => o.Ignore())
                .ForMember(x => x.RePackaged, o => o.Ignore())
                .ForMember(x => x.ItemStatus, o => o.Ignore());

            CreateMap<ElasticSearch.Models.GpiNumberSearch, ElasticSearch.Models.Product>()
                .ForMember(x => x.Gpi, o => o.MapFrom(x => x.Gpi))
                .ForMember(x => x.GenericName, o => o.Ignore())
                .ForMember(x => x.ProductForm, o => o.Ignore())
                .ForMember(x => x.DrugName, o => o.Ignore())
                .ForMember(x => x.ProductName, o => o.Ignore())
                .ForMember(x => x.Ndc, o => o.Ignore())
                .ForMember(x => x.ProductStrength, o => o.Ignore())
                .ForMember(x => x.GroupName, o => o.Ignore())
                .ForMember(x => x._id, o => o.Ignore())
                .ForMember(x => x.GroupClassName, o => o.Ignore())
                .ForMember(x => x.SubClassName, o => o.Ignore())
                .ForMember(x => x.SubClassName, o => o.Ignore())
                .ForMember(x => x.MultisourceCode, o => o.Ignore())
                .ForMember(x => x.RxOtcCode, o => o.Ignore())
                .ForMember(x => x.RePackaged, o => o.Ignore())
                .ForMember(x => x.ItemStatus, o => o.Ignore());

            CreateMap<ElasticSearch.Models.Product, ElasticSearch.Models.GpiNumberSearch>()
                .ForMember(x => x.Gpi, o => o.MapFrom(x => x.Gpi));

            CreateMap<string, ElasticSearch.Models.GpiNumberSearch>()
                .ForMember(x => x.Gpi, o => o.Ignore());

            CreateMap<string, ElasticSearch.Models.HierarchyNameSearch>()
                .ForMember(x => x.hierarchy_name, o => o.Ignore());

            CreateMap<string, ElasticSearch.Models.ProductNameSearch>()
                .ForMember(x => x.product_name, o => o.Ignore())
             .ForMember(x => x.ndc, o => o.Ignore());

            CreateMap<string, ElasticSearch.Models.FormularySetNameSearch>()
                .ForMember(x => x.FormularySetId, o => o.Ignore())
                .ForMember(x => x.FormularySetName, o => o.Ignore());

            #endregion

            #region Member Claim Search

            CreateMap<MemberClaimSearchRequest, ClaimSearchMemberParms>()
                .ForSourceMember(x => x.MemberNumber, o => o.DoNotValidate());
            CreateMap<MemberClaimSearchResult, MemberClaimSearchResponse>()
                .ForMember(x => x.IsDrugInMedicationConsent, o => o.Ignore());

            #endregion

            #region Model.DB to DependentsListResponse
            CreateMap<MemberModel, DependentsListResponse>()
               .ForMember(x => x.DateOfBirth, o => o.MapFrom(y => y.Dob.ToString("MM/dd/yyyy")))
               .ForMember(x => x.FirstName, o => o.MapFrom(y => y.FirstName))
               .ForMember(x => x.LastName, o => o.MapFrom(y => y.LastName))
               .ForMember(x => x.MemberId, o => o.MapFrom(y => y.MemberId))
               .ForMember(x => x.Gender, o => o.MapFrom(y => y.Gender))
               .ForMember(x => x.MemberCard, o => o.Ignore())
               .ForMember(x => x.PersonCode, o => o.Ignore())
               .ForMember(x => x.RelationshipCode, o => o.Ignore());
            #endregion

            #region  DAL.ElasticSearch.Models to AdminFeeTierSetModel
            CreateMap<AdminFeeTierSetModel, ElasticSearch.Models.AdminFeeTierSetNameSearch>()
             .ForMember(x => x.AdminFeeTierSetName, o => o.MapFrom(x => x.Name))
             .ForMember(x => x.AdminFeeTierSetId, o => o.MapFrom(x => x.AdminFeeTierSetId));
            #endregion

            #region ElasticSearch to Models.DB
            CreateMap<ElasticSearch.Models.PricingRuleSetNameSearch, PriceRuleSetModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.PricingRuleSetName))
                .ForMember(x => x.PricingTypeId, o => o.MapFrom(x => x.PricingTypeId))
                .ForMember(x => x.PriceRuleSetId, o => o.MapFrom(x => x.PricingRuleSetId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.NetworkPricingModels, o => o.Ignore())
                .ForMember(x => x.PlanClaimModels, o => o.Ignore())
                .ForMember(x => x.PriceRuleSetPriceRuleModels, o => o.Ignore())
                .ForMember(x => x.PricingType, o => o.Ignore())
                .ForMember(x => x.StoreClaimModels, o => o.Ignore());

            CreateMap<ElasticSearch.Models.PharmacyNetworkNameSearch, NetworkModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.PharmacyNetworkName))
                .ForMember(x => x.NetworkId, o => o.MapFrom(x => x.PharmacyNetworkId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.AdditionalMessage, o => o.Ignore())
                .ForMember(x => x.ClaimModels, o => o.Ignore())
                .ForMember(x => x.EntityCoverageModels, o => o.Ignore())
                .ForMember(x => x.IsCompoundTransparent, o => o.Ignore())
                .ForMember(x => x.NetworkChainModels, o => o.Ignore())
                .ForMember(x => x.NetworkFormularyModels, o => o.Ignore())
                .ForMember(x => x.NetworkPricingSetModels, o => o.Ignore())
                .ForMember(x => x.NetworkProviderModels, o => o.Ignore())
                .ForMember(x => x.PharmacyNetworkType, o => o.Ignore())
                .ForMember(x => x.ShowOnSavingsFromRetail, o => o.Ignore())
                .ForMember(x => x.PharmacyNetworkTypeId, o => o.Ignore());

            CreateMap<ElasticSearch.Models.NetworkPricingSetNameSearch, NetworkPricingSetModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.NetworkPricingSetName))
                .ForMember(x => x.NetworkPricingSetId, o => o.MapFrom(x => x.NetworkPricingSetId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.OverrideCompoundDispensingFee, o => o.Ignore())
                .ForMember(x => x.BenefitDesignNetworkPricingSetModels, o => o.Ignore())
                .ForMember(x => x.NeedsNetworkPricingCheck, o => o.Ignore())
                .ForMember(x => x.Network, o => o.Ignore())
                .ForMember(x => x.NetworkPricingModels, o => o.Ignore())
                .ForMember(x => x.NetworkReimbursementCode, o => o.Ignore())
                .ForMember(x => x.SuperNetworkPricingSetModels, o => o.Ignore())
                .ForMember(x => x.NetworkId, o => o.Ignore());

            CreateMap<ElasticSearch.Models.MacListNameSearch, MacListModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.MacListName))
                .ForMember(x => x.MacListId, o => o.MapFrom(x => x.MacListId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.CopayTierModels, o => o.Ignore())
                .ForMember(x => x.CostCalculationModels, o => o.Ignore())
                .ForMember(x => x.MacListDrugModels, o => o.Ignore())
                .ForMember(x => x.NetworkPricingModels, o => o.Ignore());

            CreateMap<ElasticSearch.Models.CostCalculationSetNameSearch, CostCalculationSetModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.CostCalculationSetName))
                .ForMember(x => x.CostCalculationSetId, o => o.MapFrom(x => x.CostCalculationSetId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.AmountCalculationRuleModels, o => o.Ignore())
                .ForMember(x => x.CostCalculationModels, o => o.Ignore());

            CreateMap<ElasticSearch.Models.AmountCalculationRuleSetNameSearch, AmountCalculationRuleSetModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.AmountCalculationRuleSetName))
                .ForMember(x => x.AmountCalculationRuleSetId, o => o.MapFrom(x => x.AmountCalculationRuleSetId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.AmountCalculationRuleModels, o => o.Ignore())
                .ForMember(x => x.PriceRuleModels, o => o.Ignore());

            CreateMap<ElasticSearch.Models.AmountCalculationRuleNameSearch, AmountCalculationRuleModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.AmountCalculationRuleName))
                .ForMember(x => x.AmountCalculationRuleId, o => o.MapFrom(x => x.AmountCalculationRuleId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.AmountCalculationRuleSet, o => o.Ignore())
                .ForMember(x => x.CostCalculationSet, o => o.Ignore())
                .ForMember(x => x.DeactivatedDate, o => o.Ignore())
                .ForMember(x => x.DispensingFeeTierSet, o => o.Ignore())
                .ForMember(x => x.Mony, o => o.Ignore())
                .ForMember(x => x.StartDate, o => o.Ignore())
                .ForMember(x => x.DispensingFeeTierSetId, o => o.Ignore())
                .ForMember(x => x.AmountCalculationRuleSetId, o => o.Ignore())
                .ForMember(x => x.AdminFeeTierSet, o => o.Ignore())
                .ForMember(x => x.CostCalculationSetId, o => o.Ignore())
                .ForMember(x => x.EndDate, o => o.Ignore())
                .ForMember(x => x.AdminFeeTierSetId, o => o.Ignore());

            CreateMap<ElasticSearch.Models.DispensingFeeTierSetNameSearch, DispensingFeeTierSetModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.DispensingFeeTierSetName))
                .ForMember(x => x.DispensingFeeTierSetId, o => o.MapFrom(x => x.DispensingFeeTierSetId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.AmountCalculationRuleModels, o => o.Ignore())
                .ForMember(x => x.DispensingFeeTierModels, o => o.Ignore())
                .ForMember(x => x.TierSetValueType, o => o.Ignore())
                .ForMember(x => x.TierSetValueTypeId, o => o.Ignore());

            CreateMap<ElasticSearch.Models.NetworkContractNumberSearch, BenefitDesignModel>()
                .ForMember(x => x.PlanNumber, o => o.MapFrom(x => x.NetworkContractNumber))
                .ForMember(x => x.BenefitDesignId, o => o.MapFrom(x => x.BenefitDesignId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.BenefitDesignNetworkPricingSetModels, o => o.Ignore())
                .ForMember(x => x.ClaimModels, o => o.Ignore())
                .ForMember(x => x.EntityBenefitDesignModels, o => o.Ignore())
                .ForMember(x => x.Name, o => o.Ignore());

            CreateMap<ElasticSearch.Models.SuperNetworkNameSearch, SuperNetworkModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.SuperNetworkName))
                .ForMember(x => x.SuperNetworkId, o => o.MapFrom(x => x.SuperNetworkId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.BenefitDesignNetworkPricingSetModels, o => o.Ignore())
                .ForMember(x => x.NetworkClass, o => o.Ignore())
                .ForMember(x => x.SuperNetworkPricingSetModels, o => o.Ignore())
                .ForMember(x => x.NetworkClassId, o => o.Ignore());

            CreateMap<ElasticSearch.Models.MPPCalcTierSetNameSearch, CopayTierSetModel>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.MPPCalcTierSetName))
                .ForMember(x => x.CopayTierSetId, o => o.MapFrom(x => x.CopayTierSetId))
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.CopayTierModels, o => o.Ignore())
                .ForMember(x => x.IsActive, o => o.Ignore())
                .ForMember(x => x.ModifiedDate, o => o.Ignore())
                .ForMember(x => x.PriceRuleModels, o => o.Ignore())
                .ForMember(x => x.SetDescription, o => o.Ignore())
                .ForMember(x => x.TierSetValueType, o => o.Ignore())
                .ForMember(x => x.TierSetValueTypeId, o => o.Ignore());

            CreateMap<ElasticSearch.Models.FormularyNameSearch, FormularyModel>()
               .ForMember(x => x.Name, o => o.MapFrom(x => x.FormularyName))
               .ForMember(x => x.FormularyId, o => o.MapFrom(x => x.FormularyId))
               .ForMember(x => x.ClinicalRuleModels, o => o.Ignore())
               .ForMember(x => x.CreatedDate, o => o.Ignore())
               .ForMember(x => x.Description, o => o.Ignore())
               .ForMember(x => x.EntityCoverageModels, o => o.Ignore())
               .ForMember(x => x.FormularyDrugModels, o => o.Ignore())
               .ForMember(x => x.FormularySetFormularyModels, o => o.Ignore())
               .ForMember(x => x.FormularyTierRuleModels,o => o.Ignore())
               .ForMember(x => x.IsActive, o => o.Ignore())
               .ForMember(x => x.IsPartiallyLoaded, o => o.Ignore())
               .ForMember(x => x.ModifiedDate, o => o.Ignore())
               .ForMember(x => x.NetworkFormularyModels, o => o.Ignore())
               .ForMember(x => x.PriceRuleModels, o => o.Ignore());

            CreateMap<ElasticSearch.Models.FormularySetNameSearch, FormularySetModel>()
               .ForMember(x => x.Name, o => o.MapFrom(x => x.FormularySetName))
               .ForMember(x => x.FormularySetId, o => o.MapFrom(x => x.FormularySetId))
               .ForMember(x => x.CreatedDate, o => o.Ignore())
               .ForMember(x => x.Description, o => o.Ignore())
               .ForMember(x => x.FormularySetFormularyModels, o => o.Ignore())
               .ForMember(x => x.FormularyType, o => o.Ignore())
               .ForMember(x => x.FormularyTypeId, o => o.Ignore())
               .ForMember(x => x.IsActive, o => o.Ignore())
               .ForMember(x => x.LastModifiedBy, o => o.Ignore())
               .ForMember(x => x.ModifiedDate, o => o.Ignore());

            #endregion
        }
    }
}
