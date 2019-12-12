using Contracts.Group;

namespace DAL
{
    using AutoMapper;
    using CodeFirstStoredProcs;
    using Contracts.Chain.Search;
    using Contracts.Claim.Search;
    using Contracts.Formulary;
    using Contracts.FormularyDrug;
    using Contracts.MacList;
    using Contracts.Provider;
    using DGDean.Models.Paging;
    using Models.DB;
    using ProcParms;
    using ProcResults;
    using Contracts.AmountCalculationSetRule;
    using Contracts.BenefitDesign;
    using Contracts.Bin.Search;
    using Contracts.CopayTier;
    using Contracts.CostCalculationSet;
    using Contracts.CustomNDC;
    using Contracts.DispensingFeeRuleSet;
    using Contracts.Drug;
    using Contracts.InternalBillingGroup;
    using Contracts.Member;
    using Contracts.Network;
    using Contracts.NetworkPricingSet;
    using Contracts.PriceRuleSet;
    using Contracts.PriorAuthorization;
    using Contracts.UserPermissions;
    using Models;
    using Utility.Context;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class ContextDb
    {
        #region dr_Claim_Search

        [StoredProcAttributes.Name("dr_Claim_Search_CardInfo")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(ClaimSearchResult), typeof(ClaimReversalResult))]
        public StoredProc<ClaimSearchCardInfoParms> DrClaimSearchCardInfoProc { get; set; }

        public async Task<PagedList<ClaimSearchResponse>> DrClaimSearchCardInfo(ClaimSearchRequest request)
        {
            string username = GlobalContext.OAuthUserName;
            var parms = Mapper.Map<ClaimSearchCardInfoParms>(request);
            parms.UserName = username;
            var results = await DrClaimSearchCardInfoProc.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<ClaimSearchResponse>
            {
                List = Mapper.Map<List<ClaimSearchResponse>>(results.ToList<ClaimSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            var reversals = results.ToList<ClaimReversalResult>();
            if (list.List.Any() && reversals.Any())
            {
                list.List.ForEach(c => c.ReversalClaimIds = reversals.Where(r => r.ClaimId == c.ClaimId).Select(r => r.ReversalClaimId).ToList());
            }

            return list;
        }

        [StoredProcAttributes.Name("dr_Claim_Search_RxInfo")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(ClaimSearchResult), typeof(ClaimReversalResult))]
        public StoredProc<ClaimSearchRxInfoParms> DrClaimSearchRxInfoProc { get; set; }

        public async Task<PagedList<ClaimSearchResponse>> DrClaimSearchRxInfo(ClaimSearchRequest request)
        {
            string username = GlobalContext.OAuthUserName;
            var parms = Mapper.Map<ClaimSearchRxInfoParms>(request);
            parms.UserName = username;

            var results = await DrClaimSearchRxInfoProc.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<ClaimSearchResponse>
            {
                List = Mapper.Map<List<ClaimSearchResponse>>(results.ToList<ClaimSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            var reversals = results.ToList<ClaimReversalResult>();
            if (list.List.Any() && reversals.Any())
            {
                list.List.ForEach(c => c.ReversalClaimIds = reversals.Where(r => r.ClaimId == c.ClaimId).Select(r => r.ReversalClaimId).ToList());
            }

            return list;
        }

        [StoredProcAttributes.Name("dr_Claim_Search_Member")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MemberClaimSearchResult))]
        public StoredProc<ClaimSearchMemberParms> DrClaimSearchMemberProc { get; set; }

        public async Task<PagedList<MemberClaimSearchResponse>>
            DrClaimSearchMember(MemberClaimSearchRequest request)
        {
            var parms = Mapper.Map<ClaimSearchMemberParms>(request);

            var results = await DrClaimSearchMemberProc
                .CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<MemberClaimSearchResponse>
            {
                List = Mapper.Map<List<MemberClaimSearchResponse>>(results.ToList<MemberClaimSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }

        [StoredProcAttributes.Name("dr_Claim_Search_MemberInfo")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(ClaimSearchResult), typeof(ClaimReversalResult))]
        public StoredProc<ClaimSearchMemberInfoParms> DrClaimSearchMemberInfoProc { get; set; }

        public async Task<PagedList<ClaimSearchResponse>> DrClaimSearchMemberInfo(ClaimSearchRequest request)
        {
            string username = GlobalContext.OAuthUserName;

            var parms = Mapper.Map<ClaimSearchMemberInfoParms>(request);
            parms.UserName = username;

            var results = await DrClaimSearchMemberInfoProc.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<ClaimSearchResponse>
            {
                List = Mapper.Map<List<ClaimSearchResponse>>(results.ToList<ClaimSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            var reversals = results.ToList<ClaimReversalResult>();
            if (list.List.Any() && reversals.Any())
            {
                list.List.ForEach(c => c.ReversalClaimIds = reversals.Where(r => r.ClaimId == c.ClaimId).Select(r => r.ReversalClaimId).ToList());
            }

            return list;
        }

        [StoredProcAttributes.Name("dr_Claim_SearchByClaimId")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(ClaimSearchResult), typeof(ClaimReversalResult))]
        public StoredProc<ClaimSearchByClaimIdParms> DrClaimSearchByClaimIdProc { get; set; }

        public async Task<PagedList<ClaimSearchResponse>> DrClaimSearchByClaimId(ClaimSearchRequest request)
        {
            var parms = Mapper.Map<ClaimSearchByClaimIdParms>(request);

            var results = await DrClaimSearchByClaimIdProc.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<ClaimSearchResponse>
            {
                List = Mapper.Map<List<ClaimSearchResponse>>(results.ToList<ClaimSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            var reversals = results.ToList<ClaimReversalResult>();
            if (list.List.Any() && reversals.Any())
            {
                list.List.ForEach(c => c.ReversalClaimIds = reversals.Where(r => r.ClaimId == c.ClaimId).Select(r => r.ReversalClaimId).ToList());
            }

            return list;
        }
        #endregion

        #region dr_MacListDrug_Search

        [StoredProcAttributes.Name("dr_MacListDrug_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacListDrugModel))]
        public StoredProc<MacListDrugSearchParams> DrMacListDrugSearchProc { get; set; }

        public async Task<PagedList<MacListDrugModel>> DrMacListDrugSearch(MacListDrugSearchRequest request)
        {
            var parms = new MacListDrugSearchParams
            {
                MacListId = request.MacListId,
                Gpi = request.Gpi,
                Ndc = request.Ndc,
                IsActive = request.ActivatedRules != null && request.ActivatedRules != request.DeActivatedRules && request.ActivatedRules.Value,
                PointInTime = request.PointInTime,
                UsePaging = true,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            if (request.ActivatedRules.Value && request.DeActivatedRules.Value)
                parms.IsActive = null;

            if (!request.ActivatedRules.Value && !request.DeActivatedRules.Value)
                return null;

            var results = await DrMacListDrugSearchProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<MacListDrugModel>
            {
                List = results.ToList<MacListDrugModel>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }

        #endregion

        #region dr_MacListDrug_Search_By MacListName

        [StoredProcAttributes.Name("dr_MacListDrug_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacListDrugModel))]
        public StoredProc<MacListDrugSearchParams> DrMacListDrugSearchByMacListNameProc { get; set; }

        public async Task<PagedList<MacListDrugModel>> DrMacListDrugSearchByMacListName(MacListSearchByProduct request)
        {
            var parms = new MacListDrugSearchParams
            {
                MacListName = request.MacListName,
                Gpi = request.GPI,
                Ndc = request.NDC,
                UsePaging = false,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await DrMacListDrugSearchByMacListNameProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<MacListDrugModel>
            {
                List = results.ToList<MacListDrugModel>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }

        #endregion

        #region dr_MacListDrug_Search(by id)

        [StoredProcAttributes.Name("dr_MacListDrug_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacListDrugModel))]
        public StoredProc<MacListDrugSearchParams> DrMacListDrugsHistoryByMacListIDProc { get; set; }

        public async Task<List<MacListDrugModel>> DrMacListDrugsHistoryByMacListID(int macListId)
        {
            var parms = new MacListDrugSearchParams
            {
                MacListId = macListId,
                UsePaging = false
            };
            var results = await DrMacListDrugsHistoryByMacListIDProc.CallStoredProcAsync(parms);
            var list = results.ToList<MacListDrugModel>();
            return list;
        }


        #endregion

        #region dr_MacListDrugs_Current

        [StoredProcAttributes.Name("dr_MacListDrugs_Current")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacListDrug))]
        public StoredProc<MacListDrugSearchCurrentParams> DrMacListDrugsCurrentProc { get; set; }

        public async Task<PagedList<MacListDrug>> DrMacListDrugsCurrent(MacListDrugSearchRequest request)
        {
            var parms = Mapper.Map<MacListDrugSearchCurrentParams>(request);

            var results = await DrMacListDrugsCurrentProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<MacListDrug>
            {
                List = results.ToList<MacListDrug>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }

        #endregion

        #region dr_MacListDrugs_All

        [StoredProcAttributes.Name("dr_MacListDrugs_All")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacListDrug))]
        public StoredProc<MacListDrugSearchCurrentParams> DrMacListDrugsAllProc { get; set; }

        public async Task<PagedList<MacListDrug>> DrMacListDrugsAll(MacListDrugSearchRequest request)
        {
            var parms = Mapper.Map<MacListDrugSearchCurrentParams>(request);

            var results = await DrMacListDrugsAllProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<MacListDrug>
            {
                List = results.ToList<MacListDrug>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }

        #endregion

        #region dr_MacListDrugs_Current(by id)

        [StoredProcAttributes.Name("dr_MacListDrugs_Current")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacListDrug))]
        public StoredProc<MacListDrugSearchCurrentParams> DrMacListDrugsCurrentByMacListIDProc { get; set; }

        public async Task<List<MacListDrug>> DrMacListDrugsCurrentByMacListID(int macListId)
        {
            var parms = new MacListDrugSearchCurrentParams
            {
                MacListId = macListId,
                UsePaging = false
            };

            var results = await DrMacListDrugsCurrentProc.CallStoredProcAsync(parms);
            var list = results.ToList<MacListDrug>();
            return list;
        }

        #endregion

        #region dr_MacList_WithStatistics

        [StoredProcAttributes.Name("dr_MacList_WithStatistics")]
        [StoredProcAttributes.ReturnTypes(typeof(DrugStatistics), typeof(MacListModel))]
        public StoredProc<MacListWithStatisticsParams> DrMacListWithStatisticsProc { get; set; }

        public async Task<MacList> DrMacListWithStatistics(int macListId)
        {
            var parameter = new MacListWithStatisticsParams { MacListId = macListId };
            var results = await DrMacListWithStatisticsProc.CallStoredProcAsync(parameter);
            var stats = results.ToArray<DrugStatistics>()[0];
            var ndcCount = stats.NdcCount;
            var gpiCount = stats.GpiCount;
            if (ndcCount == 0 && gpiCount == 0)
            {
                return null;
            }
            var model = results.ToArray<MacListModel>()[0];
            var macList = Mapper.Map<MacList>(model);
            macList.GpiCount = gpiCount;
            macList.NdcCount = ndcCount;
            macList.DrugsBy = stats.DrugsBy;
            return macList;
        }

        #endregion

        #region dr_ProductListRule_Search
        [StoredProcAttributes.Name("dr_ProductListRule_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrug))]
        public StoredProc<ProductListRuleSearchParams> ProductListRuleSearchProc { get; set; }

        public async Task<PagedList<FormularyDrug>> ProductListRuleSearch(ProductListRuleSearchRequest request)
        {
            var parms = new ProductListRuleSearchParams
            {
                FormularyId = request.FormularyId,
                CheckEffectiveTime = request.CheckEffectiveTime,
                FormularyName = request.FormularyName,
                Gpi = request.Gpi,
                //IsOtcCovered = request.IsOtcCovered, //leaving commented in case requirement is changed to add search for Otc
                Ndc = request.Ndc,
                //Mony = request.Mony, //leaving commented in case requirement is changed to add search for Mony
                PointInTime = request.PointInTime,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await ProductListRuleSearchProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<FormularyDrug>
            {
                List = results.ToList<FormularyDrug>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = request.PageSize > 0 ? (int)Math.Ceiling((decimal)totalItems / request.PageSize) : 1
            };
            return list;
        }
        #endregion

        #region dr_FormularyDrug_Search
        [StoredProcAttributes.Name("dr_FormularyDrug_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrug))]
        public StoredProc<FormularyDrugSearchParams> FormularyDrugSearchProc { get; set; }

        public async Task<PagedList<FormularyDrug>> FormularyDrugSearch(FormularyDrugSearchRequest request)
        {
            var parms = new FormularyDrugSearchParams
            {
                CheckEffectiveTime = request.CheckEffectiveTime,
                FormularyId = request.FormularyId,
                FormularyName = request.FormularyName,
                Gpi = request.Gpi,
                IsOtcCovered = request.IsOtcCovered,
                Ndc = request.Ndc,
                Mony = request.Mony,
                PointInTime = request.PointInTime,
                RoaId = request.RoaId,
                RoaName = request.RoaName,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await FormularyDrugSearchProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<FormularyDrug>
            {
                List = results.ToList<FormularyDrug>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = request.PageSize > 0 ? (int)Math.Ceiling((decimal)totalItems / request.PageSize) : 1
            };
            return list;
        }
        #endregion

        #region dr_FormularyDrug_Search_By_FormularyName
        [StoredProcAttributes.Name("dr_FormularyDrug_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrug))]
        public StoredProc<FormularyDrugSearchParams> FormularyDrugSearchByFormularyNameProc { get; set; }

        public async Task<PagedList<FormularyDrug>> FormularyDrugSearchByFormularyName(FormularySearchByProductRequest request)
        {//TODO: Move to dr_ProductListRule?
            var parms = new FormularyDrugSearchParams
            {
                FormularyName = request.FormularyName,
                Gpi = request.GPI,
                Ndc = request.NDC,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await FormularyDrugSearchByFormularyNameProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<FormularyDrug>
            {
                List = results.ToList<FormularyDrug>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }
        #endregion

        #region dr_FormularyDrug_download
        [StoredProcAttributes.Name("dr_FormularyDrug_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrug))]
        public StoredProc<FormularyDrugSearchParams> FormularyDrugDownloadByFormularyIDProc { get; set; }

        public async Task<List<FormularyDrug>> FormularyDrugDownloadByFormularyID(int formularyId)
        {
            var parms = new FormularyDrugSearchParams
            {
                FormularyId = formularyId
            };
            var results = await FormularyDrugDownloadByFormularyIDProc.CallStoredProcAsync(parms);
            var list = results.ToList<FormularyDrug>();
            return list;
        }
        #endregion

        #region dr_FormularyDrug_Search(by id)
        [StoredProcAttributes.Name("dr_ProductListRule_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrugModel))]
        public StoredProc<FormularyDrugSearchParams> FormularyDrugSearchByFormularyIDProc { get; set; }

        public async Task<List<FormularyDrugModel>> FormularyDrugSearchByFormularyID(int formularyId)
        {
            var parms = new FormularyDrugSearchParams
            {
                FormularyId = formularyId
            };
            var results = await FormularyDrugSearchByFormularyIDProc.CallStoredProcAsync(parms);
            var list = results.ToList<FormularyDrugModel>();
            return list;
        }
        #endregion

        #region dr_Formulary_WithStatistics
        [StoredProcAttributes.Name("dr_Formulary_WithStatistics")]
        [StoredProcAttributes.ReturnTypes(typeof(DrugStatistics), typeof(FormularyModel))]
        public StoredProc<FormularyWithStatisticsParams> FormularyWithStatisticsProc { get; set; }

        public async Task<Formulary> FormularyWithStatistics(int formularyId)
        {
            var parameter = new FormularyWithStatisticsParams { FormularyId = formularyId };
            var results = await FormularyWithStatisticsProc.CallStoredProcAsync(parameter);
            var stats = results.ToArray<DrugStatistics>()[0];
            var ndcCount = stats.NdcCount;
            var gpiCount = stats.GpiCount;
            if (ndcCount == 0 && gpiCount == 0)
            {
                return null;
            }
            var model = results.ToArray<FormularyModel>()[0];
            var formulary = Mapper.Map<Formulary>(model);
            formulary.GpiCount = gpiCount;
            formulary.NdcCount = ndcCount;
            formulary.DrugsBy = stats.DrugsBy;
            return formulary;
        }
        #endregion

        #region dr_FormularyDrug_History
        [StoredProcAttributes.Name("dr_FormularyDrug_History")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrug))]
        public StoredProc<FormularyDrugHistoryParams> FormularyDrugHistoryProc { get; set; }

        public async Task<PagedList<FormularyDrug>> FormularyDrugHistory(FormularyDrugSearchRequest request)
        {
            var parms = Mapper.Map<FormularyDrugHistoryParams>(request);
            var results = await FormularyDrugHistoryProc.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<FormularyDrug>
            {
                List = results.ToList<FormularyDrug>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }
        #endregion

        #region dr_FormularyDrug_History(by id)
        [StoredProcAttributes.Name("dr_FormularyDrug_History")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(FormularyDrugModel))]
        public StoredProc<FormularyDrugSearchParams> FormularyDrugHistoryByFormularyIDProc { get; set; }

        public async Task<List<FormularyDrugModel>> FormularyDrugHistoryByFormularyID(int formularyId)
        {
            var parms = new FormularyDrugSearchParams
            {
                FormularyId = formularyId,
            };
            var results = await FormularyDrugHistoryByFormularyIDProc.CallStoredProcAsync(parms);
            var list = results.ToList<FormularyDrugModel>();
            return list;
        }
        #endregion

        #region dr_Member_Search
        [StoredProcAttributes.Name("dr_Member_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MemberSearchResult))]
        public StoredProc<MemberSearchParams> MemberSearchProc { get; set; }

        public async Task<PagedList<MemberSearchResponse>>
            MemberSearch(MemberSearchRequest request)
        {
            var parms = Mapper.Map<MemberSearchParams>(request);

            var results = (await MemberSearchProc.CallStoredProcAsync(parms));

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var list = new PagedList<MemberSearchResponse>
            {
                List = Mapper.Map<List<MemberSearchResponse>>(results.ToList<MemberSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }
        #endregion

        #region dr_Group_Member_Search
        [StoredProcAttributes.Name("dr_Group_Member_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MemberSearchResult))]
        public StoredProc<GroupMemberSearchParams> GroupMemberSearchProc { get; set; }

        public async Task<PagedList<MemberSearchResponse>>
            GroupMemberSearch(GroupMembersSearchRequest request)
        {
            var parms = Mapper.Map<GroupMemberSearchParams>(request);

            var results = (await GroupMemberSearchProc.CallStoredProcAsync(parms));

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var list = new PagedList<MemberSearchResponse>
            {
                List = Mapper.Map<List<MemberSearchResponse>>(results.ToList<MemberSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }
        #endregion

        #region dr_Provider_Search

        [StoredProcAttributes.Name("dr_Provider_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(ProviderSearchResult))]
        public StoredProc<ProviderSearchParams> ProviderSearchProc { get; set; }

        public async Task<List<ProviderSearchResponse>>
            ProviderSearch(ProviderSearchRequest request)
        {
            var @params = Mapper.Map<ProviderSearchParams>(request);
            var results = (await ProviderSearchProc.CallStoredProcAsync(@params)).ToList<ProviderSearchResult>();

            return Mapper.Map<List<ProviderSearchResponse>>(results);
        }

        [StoredProcAttributes.Name("dr_Provider_Search_Basic")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(ProviderSearchBasicResult))]
        public StoredProc<ProviderSearchBasicParams> ProviderSearchBasicProc { get; set; }

        public async Task<PagedList<ProviderSearchBasicResponse>> ProviderSearchBasic(ProviderSearchBasicRequest request)
        {
            var parms = Mapper.Map<ProviderSearchBasicParams>(request);
            var results = await ProviderSearchBasicProc.CallStoredProcAsync(parms);

            var _pagedResults = results.ToList<ProviderSearchBasicResult>().DistinctBy(x => x.NABP).ToList();

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var list = new PagedList<ProviderSearchBasicResponse>
            {
                List = Mapper.Map<List<ProviderSearchBasicResponse>>(_pagedResults),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }

        [StoredProcAttributes.Name("dr_Attached_Internal_Billing_Pharmacy_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(PharmacySearchBasicResult))]
        public StoredProc<PharmacySearchBasicParams> PharmacySearchBasicProc { get; set; }
        public async Task<PagedList<PharmacySearchResponse>> PharmacySearchBasic(PharmacySearchRequest request)
        {
            var parms = Mapper.Map<PharmacySearchBasicParams>(request);
            var results = await PharmacySearchBasicProc.CallStoredProcAsync(parms);

            var _pagedResults = results.ToList<PharmacySearchBasicResult>().ToList();

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var list = new PagedList<PharmacySearchResponse>
            {
                List = Mapper.Map<List<PharmacySearchResponse>>(_pagedResults),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return list;
        }
        #endregion

        #region dr_Provider_Search_Price

        [StoredProcAttributes.Name("dr_Provider_Search_Member")]
        [StoredProcAttributes.ReturnTypes(typeof(ProviderSearchResult))]
        public StoredProc<MemberProviderSearchParams> ProviderSearchMemberProc { get; set; }

        public async Task<List<ProviderSearchResponse>>
        ProviderMemberSearch(MemberProviderSearchRequest request)
        {
            var @params = Mapper.Map<MemberProviderSearchParams>(request);
            var results = (await ProviderSearchMemberProc.CallStoredProcAsync(@params)).ToList<ProviderSearchResult>();

            return Mapper.Map<List<ProviderSearchResponse>>(results);
        }

        #endregion

        #region BinPcn_Search
        [StoredProcAttributes.Name("dr_BinPcn_Search")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(BinPcnSearchResult))]
        public StoredProc<BinPcnSearchParams> BinPcnSearchProc { get; set; }

        public async Task<PagedList<BinPcnSearchResponse>>
            BinPcnSearch(BinPcnSearchRequest request)
        {
            var parms = Mapper.Map<BinPcnSearchParams>(request);

            var results = await BinPcnSearchProc
                .CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<BinPcnSearchResponse>
            {
                List = Mapper.Map<List<BinPcnSearchResponse>>(results.ToList<BinPcnSearchResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            return list;
        }
        #endregion

        #region dr_Network_Search_For_Providers
        [StoredProcAttributes.Name("dr_Network_Search_Providers")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(NetworkSearchForProvidersResult))]
        public StoredProc<NetworkSearchForProvidersParams> NetworkSearchForProvidersProc { get; set; }

        public async Task<PagedList<NetworkSearchProvidersResult>> NetworkSearchForProviders(NetworkSearchProvidersRequest request)
        {
            var parms = Mapper.Map<NetworkSearchForProvidersParams>(request);

            var results = await NetworkSearchForProvidersProc.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<NetworkSearchProvidersResult>
            {
                List = Mapper.Map<List<NetworkSearchProvidersResult>>(results.ToList<NetworkSearchForProvidersResult>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            return list;
        }
        #endregion

        #region dr_GetClaimHistory_ByMemberCardId
        [StoredProcAttributes.Name("dr_ClaimHistory_ByMemberCardId")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(ClaimResponse))]
        public StoredProc<ClaimHistoryByMemberCardIdParams> ClaimHistoryByMemberCardIdProc { get; set; }
        public async Task<PagedList<ClaimResponse>> GetClaimHistoryByMemberCardId(int memberCardId, int pageNumber, int pageSize)
        {
            var parms = new ClaimHistoryByMemberCardIdParams
            {
                MemberCardId = memberCardId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var results = await ClaimHistoryByMemberCardIdProc.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            var list = new PagedList<ClaimResponse>
            {
                List = Mapper.Map<List<ClaimResponse>>(results.ToList<ClaimResponse>()),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return list;
        }
        #endregion

        #region RetrieveNavigationHierarchy

        [StoredProcAttributes.Name("dr_RetrieveNavigationHierarchy")]
        [StoredProcAttributes.ReturnTypes(typeof(RetrieveNavigationHierarchyResponse))]
        public StoredProc<UserNavigationHierarchyParams> UserNavigationHierarchyParams { get; set; }

        public async Task<RetrieveNavigationHierarchyResponse>
        NavigationHierarchySearch(string userId)
        {
            RetrieveNavigationHierarchyResponse resultHierarchy = null;
            var parms = new UserNavigationHierarchyParams
            {
                UserId = userId
            };
            var results = await UserNavigationHierarchyParams.CallStoredProcAsync(parms);
            if (results != null && results.Count > 0)
            {
                resultHierarchy = results[0].FirstOrDefault() as RetrieveNavigationHierarchyResponse;
                if (resultHierarchy.NavigationHierarchy != null)
                    resultHierarchy.NavigationHierarchy = Newtonsoft.Json.JsonConvert.DeserializeObject(resultHierarchy.NavigationHierarchy.ToString());
            }
            return resultHierarchy;
        }

        #endregion
        #region dr_Medispan_CustomNDC_Update
        [StoredProcAttributes.Name("dr_Medispan_CustomNDC_Update")]
        [StoredProcAttributes.ReturnTypes(typeof(CustomNDCSPResponse))]
        public StoredProc<UpdateMedispanCustomNDCParam> UpdateMedispanCustomNDC { get; set; }
        public async Task<CustomNDCSPResponse> UpdateMedispanCustomNDCByNDC(string NDC, DateTime? InactiveDT, DateTime? Deactivated_DT)
        {
            var parms = new UpdateMedispanCustomNDCParam
            {
                NDC = NDC,
                INACTIVE_DT = InactiveDT,
                DEACTIVATED_DT = Deactivated_DT
            };
            var results = (await UpdateMedispanCustomNDC.CallStoredProcAsync(parms)).ToList<CustomNDCSPResponse>();

            return Mapper.Map<CustomNDCSPResponse>(results[0]);

        }
        #endregion

        #region dr_GetClaimHistory_MultipleGroup_ByMemberCardId
        [StoredProcAttributes.Name("dr_GetClaimHistory_MultipleGroup_ByMemberCardId")]
        [StoredProcAttributes.ReturnTypes(typeof(ClaimModel))]
        public StoredProc<ClaimHistoryMultipleGroupMemberCardIdParams> ClaimHistoryMultipleGroupMemberCardIdParamsProc { get; set; }
        public List<ClaimHistory> GetClaimHistoryMultipleGroupMemberCardId(int memberId, DateTime claimDate)
        {
            List<ClaimHistory> claimHistory = new List<ClaimHistory>();
            var parameters = new ClaimHistoryMultipleGroupMemberCardIdParams
            {
                MemberId = memberId,
                ClaimDate = claimDate

            };
            var results = ClaimHistoryMultipleGroupMemberCardIdParamsProc.CallStoredProc(parameters);

            foreach (var result in results.ToList<ClaimModel>())
            {
                claimHistory.Add(
                    new ClaimHistory
                    {
                        claimModel = result,
                        otcCovered = null
                    }
                );
            }

            return claimHistory;
        }
        #endregion

        #region dr_GetClaimHistory_MultipleGroup_ByMemberId
        [StoredProcAttributes.Name("dr_GetClaimHistory_MultipleGroup_ByMemberId")]
        [StoredProcAttributes.ReturnTypes(typeof(ClaimShortHistory))]
        public StoredProc<ClaimHistoryMultipleGroupMemberIdParams> ClaimHistoryMultipleGroupMemberIdParamsProc { get; set; }
        public List<ClaimShortHistory> GetClaimHistoryMultipleGroupMemberId(int memberId, DateTime startDate, DateTime endDate, bool includeTestClaims)
        {
            var parameters = new ClaimHistoryMultipleGroupMemberIdParams
            {
                MemberId = memberId,
                StartDate = startDate,
                EndDate = endDate,
                IncludeTestClaims = includeTestClaims,
            };
            var results = ClaimHistoryMultipleGroupMemberIdParamsProc.CallStoredProc(parameters);

            var items = results.ToList<ClaimShortHistory>();

            return items;
        }
        #endregion

        #region User Hierarchy for Pricing

        [StoredProcAttributes.Name("dr_UserHierarchy_DispensingFeeTierSet")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(DispensingFeeRuleSet))]
        public StoredProc<PricingParams> UserHierarchyDispensingFeeTierSetParams { get; set; }

        public async Task<PagedList<DispensingFeeRuleSet>> UserHierarchy_DispensingFeeTierSet(string name, int pageNumber, int pageSize)
        {
            PagedList<DispensingFeeRuleSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new PricingParams
            {
                UserName = userName,
                Name = name,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchyDispensingFeeTierSetParams.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            resultHierarchy = new PagedList<DispensingFeeRuleSet>
            {
                List = Mapper.Map<List<DispensingFeeRuleSet>>(results.ToList<DispensingFeeRuleSet>()),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return resultHierarchy;
        }


        [StoredProcAttributes.Name("dr_UserHierarchy_AmountCalculationRuleSet_GetAll")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(AmountCalculationRuleSet))]
        public StoredProc<AmountCalculationRuleSetParams> UserHierarchyAmountCalculationRulSetParams { get; set; }

        public async Task<PagedList<AmountCalculationRuleSet>> UserHierarchy_AmountCalculationRuleSet_GetAll(DispensingFeeTierSetSearchRequest request)
        {
            PagedList<AmountCalculationRuleSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new AmountCalculationRuleSetParams
            {
                UserName = userName,
                Name = request.Name,
                DispensingFeeTierSetId = request.DispensingFeeTierSetId,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await UserHierarchyAmountCalculationRulSetParams.CallStoredProcAsync(parms);

            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            resultHierarchy = new PagedList<AmountCalculationRuleSet>
            {
                List = Mapper.Map<List<AmountCalculationRuleSet>>(results.ToList<AmountCalculationRuleSet>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_AmountCalculationRuleSet")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(AmountCalculationSetRule))]
        public StoredProc<AmountCalculationSetRuleParams> UserHierarchyAmountCalculationSetRuleParams { get; set; }

        public async Task<PagedList<AmountCalculationSetRule>> UserHierarchy_AmountCalculationSetRule_GetAll(string name, int pageNumber, int pageSize)
        {
            PagedList<AmountCalculationSetRule> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;
            var parms = new AmountCalculationSetRuleParams
            {
                UserName = userName,
                Name = name,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchyAmountCalculationSetRuleParams.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            resultHierarchy = new PagedList<AmountCalculationSetRule>
            {
                List = results.ToList<AmountCalculationSetRule>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };
            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_MacList")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacList))]
        public StoredProc<MacListParams> UserHierarchyGetAllMacListParams { get; set; }

        public async Task<PagedList<MacList>> UserHierarchy_GetMacListAll(MacListGetAllRequest request)
        {
            PagedList<MacList> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;
            var parms = new MacListParams
            {
                UserName = userName,
                IsActive = request.IsActive ?? true,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await UserHierarchyGetAllMacListParams.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            resultHierarchy = new PagedList<MacList>
            {
                List = results.ToList<MacList>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_MacList")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(MacList))]
        public StoredProc<MacListParams> UserHierarchySearchMacListParams { get; set; }

        public async Task<PagedList<MacList>> UserHierarchy_SearchMacList(MacListSearchRequest request)
        {

            PagedList<MacList> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;
            var parms = new MacListParams
            {
                UserName = userName,
                DrugName = request.DrugName,
                MacListId = request.MacListId,
                IsActive = request.IsActive ?? true,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await UserHierarchySearchMacListParams.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;
            resultHierarchy = new PagedList<MacList>
            {
                List = results.ToList<MacList>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };
            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_SearchAllNetworkPricingSet")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(NetworkPricingSetModel))]
        public StoredProc<PricingParams> UserHierarchySearchAllNetworkPricingSet { get; set; }

        public async Task<PagedList<NetworkPricingSetModel>> UserHierarchy_SearchAllNetworkPricingSet(NetworkPricingSetSearchAllRequest request)
        {
            PagedList<NetworkPricingSetModel> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new PricingParams
            {
                UserName = userName,
                Name = request.Name,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await UserHierarchySearchAllNetworkPricingSet.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            resultHierarchy = new PagedList<NetworkPricingSetModel>
            {
                List = Mapper.Map<List<NetworkPricingSetModel>>(results.ToList<NetworkPricingSetModel>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_SearchtNetworkPricingSetbyName")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(NetworkPricingSetModel))]
        public StoredProc<SearchtNetworkPricingSetbyNameParams> UserHierarchySearchtNetworkPricingSetbyName { get; set; }

        public async Task<PagedList<NetworkPricingSet>> UserHierarchy_SearchtNetworkPricingSetbyName(NetworkPricingSetSearchRequest request)
        {
            PagedList<NetworkPricingSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new SearchtNetworkPricingSetbyNameParams
            {
                UserName = userName,
                Name = request.Name,
                PriceRuleSetId = request.PriceRuleSetId,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await UserHierarchySearchtNetworkPricingSetbyName.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var resultSet = (results != null && results.Count > 0) ? results[1] : null;

            resultHierarchy = new PagedList<NetworkPricingSet>
            {
                List = Mapper.Map<List<NetworkPricingSet>>(resultSet),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            return resultHierarchy;
        }


        [StoredProcAttributes.Name("dr_UserHierarchy_GetAllNetworkPricingSet")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(NetworkPricingSetModel))]
        public StoredProc<BaseParams> UserHierarchyGetAllNetworkPricingSet { get; set; }
        public async Task<PagedList<NetworkPricingSet>> UserHierarchy_GetAllNetworkPricingSet(int pageNumber, int pageSize)
        {
            PagedList<NetworkPricingSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;
            var parms = new BaseParams
            {
                UserName = userName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchyGetAllNetworkPricingSet.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var resultSet = (results != null && results.Count > 0) ? results[1] : null;
            resultHierarchy = new PagedList<NetworkPricingSet>
            {
                List = Mapper.Map<List<NetworkPricingSet>>(resultSet),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_CostCalculationSet")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(CostCalculationSetModel))]
        public StoredProc<BaseParams> UserHierarchyGetAllCostCalculationSet { get; set; }
        public async Task<PagedList<CostCalculationSet>> UserHierarchy_CostCalculationSet_GetAll(int pageNumber, int pageSize)
        {
            PagedList<CostCalculationSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;
            var parms = new BaseParams
            {
                UserName = userName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchyGetAllCostCalculationSet.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var resultSet = (results != null && results.Count > 0) ? results[1] : null;
            resultHierarchy = new PagedList<CostCalculationSet>
            {
                List = Mapper.Map<List<CostCalculationSet>>(resultSet),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_CostCalculationSet")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(CostCalculationSetModel))]
        public StoredProc<CostCalculationSetParams> UserHierarchySearchCostCalculationSetbyName { get; set; }

        public async Task<PagedList<CostCalculationSet>> UserHierarchy_SearchCostCalculationSetbyName(int pageNumber, int pageSize, string name)
        {
            PagedList<CostCalculationSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new CostCalculationSetParams
            {
                UserName = userName,
                Name = name,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchySearchCostCalculationSetbyName.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var resultSet = (results != null && results.Count > 0) ? results[1] : null;

            resultHierarchy = new PagedList<CostCalculationSet>
            {
                List = Mapper.Map<List<CostCalculationSet>>(resultSet),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_AmountCalculationSetRule")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(AmountCalculationRuleSetModel))]
        public StoredProc<AmountCalculationSetRuleParams> UserHierarchySearchCostBasisSetSetbyName { get; set; }

        public async Task<PagedList<AmountCalculationSetRule>> UserHierarchy_SearchCostBasisSetbyName(int pageNumber, int pageSize, string name)
        {
            PagedList<AmountCalculationSetRule> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new AmountCalculationSetRuleParams
            {
                UserName = userName,
                Name = name,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchySearchCostBasisSetSetbyName.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var resultSet = (results != null && results.Count > 0) ? results[1] : null;

            resultHierarchy = new PagedList<AmountCalculationSetRule>
            {
                List = Mapper.Map<List<AmountCalculationSetRule>>(resultSet),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_CostCalculationSetById")]
        [StoredProcAttributes.ReturnTypes(typeof(CostCalculationModel))]
        public StoredProc<CostCalculationParams> UserHierarchyGetCostCalculationByHierarchyParams { get; set; }

        public async Task<CostCalculationModel> UserHierarchy_GetCostCalculationByHierarchy(int costcalculationid)
        {
            CostCalculationModel resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;
            var parms = new CostCalculationParams
            {
                UserName = userName,
                UsePaging = false,
                CostCalculationId = costcalculationid
            };
            var results = await UserHierarchyGetCostCalculationByHierarchyParams.CallStoredProcAsync(parms);
            resultHierarchy = results.ToList<CostCalculationModel>().FirstOrDefault();
            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_GetPriceRuleSetById")]
        [StoredProcAttributes.ReturnTypes(typeof(PriceRuleSet))]
        public StoredProc<PriceRulSetParams> PriceRulSetParams { get; set; }

        public async Task<PriceRuleSet> UserHierarchy_PriceRuleSetById(int Id)
        {
            PriceRuleSet resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new PriceRulSetParams
            {
                UserName = userName,
                UsePaging = false,
                PriceRuleSetId = Id,
            };
            var result = await PriceRulSetParams.CallStoredProcAsync(parms);
            if (result != null && result.Count > 0)
                resultHierarchy = result.ToList<PriceRuleSet>().FirstOrDefault();
            return resultHierarchy;
        }


        [StoredProcAttributes.Name("dr_UserHierarchy_GetPriceRuleSetByName")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(PriceRuleSetSearchResponse))]
        public StoredProc<PriceRulSetByNameParams> UserHierarchyGetPriceRuleSetByName { get; set; }

        public async Task<PagedList<PriceRuleSetSearchResponse>> UserHierarchy_GetPriceRuleSetbyName(PriceRuleSetSearchRequest request)
        {
            PagedList<PriceRuleSetSearchResponse> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new PriceRulSetByNameParams
            {
                UserName = userName,
                Name = request.Name,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            var results = await UserHierarchyGetPriceRuleSetByName.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            resultHierarchy = new PagedList<PriceRuleSetSearchResponse>
            {
                List = Mapper.Map<List<PriceRuleSetSearchResponse>>(results.ToList<PriceRuleSetSearchResponse>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / request.PageSize)
            };

            return resultHierarchy;
        }

        [StoredProcAttributes.Name("dr_UserHierarchy_CopayTierSetByName")]
        [StoredProcAttributes.ReturnTypes(typeof(PagedCount), typeof(CopayTierSetModel))]
        public StoredProc<CopayTierSetParams> UserHierarchySearchCopayTierSetbyName { get; set; }

        public async Task<PagedList<CopayTierSet>> UserHierarchy_SearchCopayTierSetbyName(int pageNumber, int pageSize, string name)
        {
            PagedList<CopayTierSet> resultHierarchy = null;
            string userName = GlobalContext.OAuthUserName;

            var parms = new CopayTierSetParams
            {
                UserName = userName,
                Name = name,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var results = await UserHierarchySearchCopayTierSetbyName.CallStoredProcAsync(parms);
            var totalItems = results.ToArray<PagedCount>()[0].AllRecordsCount;

            var resultSet = (results != null && results.Count > 0) ? results[1] : null;

            resultHierarchy = new PagedList<CopayTierSet>
            {
                List = Mapper.Map<List<CopayTierSet>>(resultSet),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize)
            };

            return resultHierarchy;
        }

        #endregion

        #region dr_BillingGroupUpdateAfterDataQ
        [StoredProcAttributes.Name("dr_BillingGroupUpdateAfterDataQ")]
        [StoredProcAttributes.ReturnTypes(typeof(BillingGroupUpdateAfterDataQResponse))]
        public StoredProc StoredProc { get; set; }
        public async Task<BillingGroupUpdateAfterDataQResponse> UpdateProvidersAfterDataQExecution()
        {
            var results = ( await StoredProc.CallStoredProcAsync()).ToList<BillingGroupUpdateAfterDataQResponse>();
            return Mapper.Map<BillingGroupUpdateAfterDataQResponse>(results[0]);
        }
        #endregion
    }
}
