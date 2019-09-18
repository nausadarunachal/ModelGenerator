namespace DB.CodeTemplate
{
    public static class TemplateConstants
    {
        public const string ConnectionStringFileName = "app.config";
        public const string ConnectionStringAppSetting = "MSSQLConnectionString";
        public const string ConnectionStringProject = "DB.Models";
        public const string ConnectionStringProjectRelativePath = "";
        public const string CreateDateFieldNames = "Created,CreateDate,CreatedDate,DateCreated";
        public const string DataSetTables =
            @"Columns, sys.columns,
			DefaultConstraints, sys.default_constraints,
			ForeignKeyColumns, sys.foreign_key_columns,
			ForeignKeys, sys.foreign_keys,
			IdentityColumns, sys.identity_columns,
			IndexColumns, sys.index_columns,
			Indexes, sys.indexes,
			KeyConstraints, sys.key_constraints,
			Objects, sys.objects,
			Parameters, sys.parameters,
			Procs, sys.procedures,
			Schemas, sys.schemas,
			Tables, sys.tables,
			Types, sys.types,
			Views, sys.views";
        public const string DbColumnActiveMappings =
            @"Active:1,
			Inactive:0,
			is_active:1,
			IsActive:1,
			IsDeleted:0";
        public const string DbColumnNameMappings =
            @"cm_BrokerFee.BrokerFee:BrokerFeeAmount,
			SR_MelissaCode.CodeType:CodeTypeId";
        public const string DbContextClassName = "RxSenseDb";
        public const string DbContextNamespace = "DB.DAL";
        public const string DbContextProject = "DB.DAL";
        public const string DbContextProjectRelativePath = "";
        public const string DefaultValuesToBeExcluded =
            @"(app_name()),
			(suser_sname()),
			(sysutcdatetime()),
			(host_name()),
			(@@spid)";
        public const string DestinationNavigationPropertyNames =
            @"dr_Claim.StoreCopayTierId:StoreClaimModels,
			dr_Claim.StoreCostCalculationId:StoreClaimModels,
			dr_Claim.StoreDispensingFeeTierId:StoreClaimModels,
			dr_Claim.StoreMacListDrugId:StoreClaimModels,
			dr_Claim.StoreNetworkPricingId:StoreClaimModels,
			dr_Claim.StorePriceRuleId:StoreClaimModels,
			dr_Claim.StorePriceRuleSetId:StoreClaimModels,
			dr_Claim.PlanCopayTierId:PlanClaimModels,
			dr_Claim.PlanCostCalculationId:PlanClaimModels,
			dr_Claim.PlanDispensingFeeTierId:PlanClaimModels,
			dr_Claim.PlanMacListDrugId:PlanClaimModels,
			dr_Claim.PlanNetworkPricingId:PlanClaimModels,
			dr_Claim.PlanPriceRuleId:PlanClaimModels,
			dr_Claim.PlanPriceRuleSetId:PlanClaimModels,
			dr_Claim.ReversedClaimId:ReversalClaimModels,
            dr_MemberCoverageTypeGroup.MemberCoverageTypeId:MemberCoverageTypeGroupModels2,
			sc_Appointment.MemberUserId:MemberAppointments,
			sc_Appointment.PatientUserId:PatientAppointments,
			sc_Dependent.DependentUserId:Dependents,
			sc_Dependent.ParentUserId:Parents,
			sc_Estimate.CreatedBy:CreatedEstimates,
			sc_Estimate.UserId:UserEstimates,
			sc_PaymentMethod.CreatedBy:CreatedPaymentMethods,
			sc_PaymentMethod.UserId:UserPaymentMethods,
			sc_PaymentMethodLog.CreatedBy:CreatedPaymentMethodLogs,
			sc_PaymentMethodLog.UserId:UserPaymentMethodLogs,
			sc_Person.ActivationContactId:ActivationPersons,
			sc_Person.ContactId:Persons,
			sc_Person.MarketingContactId:MarketingPersons,
			sc_Transaction.FromUserId:FromTransactions,
			sc_Transaction.ToUserId:ToTransactions,
			sc_Transaction.ReferenceTransactionId:ReferencingTransactions,
			sc_Transaction.VisitId:Transactions,
			sc_Visit.CancelledBy:CancelledVisits,
			sc_Visit.CreatedBy:CreatedVisits,
			sc_Visit.MemberUserId:MemberVisits,
			sc_Visit.PatientUserId:PatientVisits,
			sc_WorkflowItem.LockedBy:LockedWorkflowItems,
			sc_WorkflowItem.ModifiedBy:ModifiedWorkflowItems,
			sc_WorkflowItem.OwnerId:OwnedWorkflowItems,
			sc_WorkflowItemContact.ModifiedBy:ModifiedWorkflowItemContacts,
			sc_WorkflowItemContact.UserId:UserWorkflowItemContacts,
			sr_ContactSource.LetterCodeAnnualID:LetterCodeAnnualContactSources,
			sr_ContactSource.LetterCodeRemailerID:LetterCodeRemailerContactSources";
        public const string DisplayNameMappings =
            @"PluralName,Plural Form";
        public const string DisplayNameWordMappings =
            @"Calc,Calculated,
			Desc,Description,
			Pkg,Package";
        public const string EditableColumns =
            @"sc_MedispanDrug.BypassDisplayQuantity,
				sc_MedispanDrug.CalcDosage,
				sc_MedispanDrug.CalcForm,
				sc_MedispanDrug.MarketingName,
				sc_MedispanDrug.MarketingForm,
				sc_Tenant.AllowMembership,
				sc_Tenant.ContactSource,
				sc_Tenant.HasFundedPBM,
				sc_Tenant.HostName,
				sc_Tenant.IsActive,
				sc_Tenant.LogoPath,
				sc_Tenant.MarketingName,
				sc_Tenant.Name,
				sc_Tenant.StylePath,
				sc_Tenant.TenantFolder,
				sc_Tenant.TenantModuleModels,
				sc_Tenant.TenantNetworkModels,
				sc_Tenant.URL";
        public const string EntitySuffix = "Model";
        public const string ExcludeDateRangeInterface =
            @"dr_ClaimMessage,
            dr_ForgotPassword,
            sc_Promo";
        public const string ExcludeFieldsFromModelTable =
            @"";
        public const string ExcludePopulateDatesInterface = "sc_SrDrug";
        public const string FieldsToSkip =
            @"cm_Broker.BountyPartyTypeId,
			cm_FMO.BountyPartyTypeId,
			cm_Partner.BountyPartyTypeId,
			cm_Producer.BountyPartyTypeId";
        public const string ForeignKeysToIgnore =
            @"";
        public const string GeneratedDisclaimer =
                "/* *** THIS FILE IS GENERATED BY A REALLY SMART ROBOT.      ***\r\n" +
                "   *** IF YOU EDIT THIS FILE, THE ROBOT WILL EAT YOUR CODE, ***\r\n" +
                "   *** AND REPLACE YOUR CODE WITH HIS, BECAUSE THAT'S HOW   ***\r\n" +
                "   *** HE ROLLS. YOU HAVE BEEN WARNED.                      ***\r\n\r\n" +
                "   *** Seriously, if you want to make changes to this file, ***\r\n" +
                "   *** either run the template that created it again,       ***\r\n" +
                "   *** or change the template code, at the project path     ***\r\n" +
                "   *** below.                                               ***\r\n\r\n" +
                "   *** DB.Models/Code Templates/RxSenseDb.tt     ***\r\n" +
                "*/\r\n";
        public const string ModelsNamespace = "DB.Models.DB";
        public const string ModelsNamespaceDbContext = "Models.DB";
        public const string ModelsProjectRelativePath = "DB";
        public const string ModelsProject = "DB.Models";
        public const string ModelsSubclass = "BaseModel";
        public const string ModelsSubclassEffDate = "BaseDateRangeModel";
        public const string ModelsSubclassEffDateExcludedColumns = "CreatedDate,DeactivatedDate,EndDate,IsActive,ModifiedDate,StartDate";
        public const string ModifiedDateFieldNames = "LastModified,ModifedDate,ModifiedDate";
        public const string NavigationPropertyDisplayNames =
            @"sc_Tenant.TenantContactSourceModels:ContactSources,
			sc_Tenant.TenantModuleModels:Modules,
			sc_Tenant.TenantNetworkModels:Networks";
        public const string NavigationPropertiesToSkip =
            @"BountyPartyModel.BrokerModels,
			BountyPartyModel.Fmoes,
			BountyPartyModel.PartnerModels,
			BountyPartyModel.ProducerModels";
        public const string NewKeywordColumns =
            @"AspNetRole.Name";
        public const string OverrideColumns =
            @"AspNetUser.AccessFailedCount,
			AspNetUser.Email,
			AspNetUser.EmailConfirmed,
			AspNetUser.LockoutEnabled,
			AspNetUser.LockoutEndDateUtc,
			AspNetUser.PasswordHash,
			AspNetUser.PhoneNumber,
			AspNetUser.PhoneNumberConfirmed,
			AspNetUser.SecurityStamp,
			AspNetUser.TwoFactorEnabled,
			AspNetUser.UserName,
			AspNetUserClaim.ClaimType,
			AspNetUserClaim.ClaimValue,
			AspNetUserLogin.LoginProvider,
			AspNetUserLogin.ProviderKey";
        public const string PluralizationMappings =
            @"Diagnosis,Diagnoses,
			TransactionStatus,TransactionStatuses,
			VisitServiceItemDiagnosis,VisitServiceItemDiagnoses,
			WorkflowStatus,WorkflowStatuses,
			WorkflowStatusTransactionStatus,WorkflowStatusTransactionStatuses,
			WorkflowTypeStatus,WorkflowTypeStatuses";
        public const string SourceNavigationPropertyNames =
            @"cm_Producer.Id|BountyPartyTypeId:Producer,
			dr_CopayTier.
			sc_Estimate.CreatedBy:CreatingUser,
			sc_PaymentMethod.CreatedBy:CreatingUser,
			sc_PaymentMethodLog.CreatedBy:CreatingUser,
			sc_ServiceAreaGeoZone.GeoZoneGuid:GeoZone,
			sc_Visit.CancelledBy:CancellingUser,
			sc_Visit.CreatedBy:CreatingUser,
			sc_WorkflowItem.LockedBy:Locker,
			sc_WorkflowItem.ModifiedBy:Modifier,
			sc_WorkflowItemContact.ModifiedBy:Modifier";
        public const string StartDateFieldNames = "StartDate";
        public const string EndDateFieldNames = "EndDate";
        public const string IsActiveFieldNames = "IsActive";
        public const string DeactivatedDateFieldNames = "DeactivatedDate";
        public const string TableNameMappings =
            @"a_ETL_Segments,AEtlSegment,
			cm_FMO,Fmo,
			cm_FMOHistory,FmoHistory,
			ETL_ReturnFile,ReturnFile,
			MYzipcode,MyZipCode,
			sc_ETL_fulfillment,EtlFulfillment,
			sc_ETL_GeoAddress,EtlGeoAddress,
			sc_Supression,Suppression,
			SR_CodeType,CodeType,
			SR_InvalidContact,InvalidContact,
			sr_IPBlocks,IpBlock,
			sr_IPLocation,IpLocation,
			SR_MelissaCode,MelissaCode,
			SR_Segment_Member_Tracking,SegmentMemberTracking,
			sr_SMSTemplate,SmsTemplate";
        public const string TableNamePrefixesToRemove =
            @"dr_,sc_,mp_,sr_";
        public const string TablesToOptOutOfSubClass =
            @"AspNetRole,
			AspNetUser,
			AspNetUserClaim,
			AspNetUserLogin,
			AspNetUserRole";
        public const string TablesToSkip =
            @"__.+,
			'DMX.+,
			a_.+,
			cm_Admin_TableSize,
			cm_BountyHistory,
			cm_BTB.+,
			cm_Export_MemberEmail,
			cm_FMOHistory,
			cm_ProducerPartnerTest,
			cm_Segment_.*,
			DenteMax.*,
			DMX.*,
			dr_AuditTable,
			dr_ClaimAuthsProffee,
			dw_SavingsPercent,
			etl_.+,
			Excel.*,
			import.+,
			medispan_old,
			medispan_qa.*,
			medispan_Restored.*,
			micromedix1,
			mp_UsersSalesRep,
			MYGeo,
			MYzipcode,
            my_jan15,
            my_jan31,
            my_tempMyRx,
            ncpdp_chain,
			ncpdp_provider,
			nppes_.+,
            PaymentCenters,
            ProviderRelationships,
			prescriber,
			Program,
			SalesRepDataImport,
            sam_Claim,
            sam_ClaimMessage,
			sc_Address_bak,
			sc_AddressBak,
			sc_BTB.+,
			sc_DeltaDrugMicromedex,
			sc_DeltaZipCode,
			sc_DrugInformation,
			sc_etl_.+,
			sc_GeoAddress_QA,
			sc_GeoAddress_QA1,
			sc_GeoAddress1,
			sc_medispandrug_Bak,
			sc_MedispanIm.+,
			sc_MemberPin,
			sc_Micromedex_Drug,
			sc_Micromedex_DrugInformation,
			sc_Micromedex_TypeSection,
			sc_Micromedex_TypeSectionSub,
			sc_Segment,
			sc_WorkflowDispositionArchive,
			sc_WorkflowHierarchyArchive,
			sc_WorkFlowHierarchyBackup,
			sc_WorkFlowItemArchive,
			sc_WorkflowItemArchiveFirstHealthExit,
			sc_WorkFlowItemArchive_old,
			sc_WorkFlowItemBackUp,
			sc_WorkflowItemContactArchive,
			sc_WorkflowItemContactArchiveFirstHealthExit,
			sc_WorkflowItemQuestionArchive,
			sc_WorkflowItemQuestionArchiveFirstHealthExit,
			sc_WorkflowMemberArchive,
			sc_WorkflowProviderArchive,
			sc_WorkflowStatusArchive,
			sc_WorkflowTypeArchive,
			sc_WorkflowTypeStatusArchive,
			.*scrub.*,
			stg_.*,
			DRXNewVisionProvider,
			ScriptsRun,
			ScriptsRunErrors,
			Version,
			SR_CodeTyp,
			SR_MelissaCode,
			sr_ScriptsByDrug,
			sr_ScriptsByDrugQty,
			sysdiagrams,
			temp.+,
			test,
			tmp.+,
			VisionFeeSchedule,
			WalmartVisionProviders,
			workflowImport,
			workflowImport_bak,
			workflowRep,
			workflowSUP,
			xmp_.+";
    }
}
