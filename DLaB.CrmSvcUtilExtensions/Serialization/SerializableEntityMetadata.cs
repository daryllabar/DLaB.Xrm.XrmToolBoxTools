using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.Serialization
{
    [MetadataName(LogicalCollectionName = "EntityDefinitions", LogicalName = "EntityMetadata")]
    [DataContract(Name = "EntityMetadata", Namespace = "http://schemas.microsoft.com/xrm/2011/Metadata")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    public class SerializableEntityMetadata
    {
        public SerializableEntityMetadata(EntityMetadata entity)
        {
            
        }

        private int? _activityTypeMask;
        private AttributeMetadata[] _attributes;
        private bool? _canTriggerWorkflow;
        private Label _description;
        private Label _displayCollectionName;
        private Label _displayName;
        private bool? _isDocumentMangementEnabled;
        private bool? _isOneNoteIntegrationEnabled;
        private bool? _isInteractionCentricEnabled;
        private bool? _isKnowledgeManagementEnabled;
        private bool? _isActivity;
        private bool? _isActivityParty;
        private BooleanManagedProperty _isAuditEnabled;
        private bool? _isAvailableOffline;
        private bool? _isChildEntity;
        private bool? _isAIRUpdated;
        private bool? _autoCreateAccessTeams;
        private BooleanManagedProperty _isValidForQueue;
        private BooleanManagedProperty _isConnectionsEnabled;
        private string _iconLargeName;
        private string _iconMediumName;
        private string _iconSmallName;
        private bool? _isCustomEntity;
        private bool? _isBusinessProcessEnabled;
        private BooleanManagedProperty _isCustomizable;
        private BooleanManagedProperty _isRenameable;
        private BooleanManagedProperty _isMappable;
        private BooleanManagedProperty _isDuplicateDetectionEnabled;
        private bool? _isImportable;
        private bool? _isIntersect;
        private BooleanManagedProperty _isMailMergeEnabled;
        private bool? autoRouteToOwnerQueue;
        private bool? _isEnabledForCharts;
        private bool? _isEnabledForTrace;
        private bool? _isValidForAdvancedFind;
        private string _entityHelpUrl;
        private bool? _entityHelpUrlEnabled;
        private BooleanManagedProperty _isVisibleInMobile;
        private BooleanManagedProperty _isVisibleInMobileClient;
        private BooleanManagedProperty _isReadOnlyInMobileClient;
        private string _logicalName;
        private ManyToManyRelationshipMetadata[] _manyToManyRelationships;
        private OneToManyRelationshipMetadata[] _manyToOneRelationships;
        private int? _objectTypeCode;
        private OneToManyRelationshipMetadata[] _oneToManyRelationships;
        private OwnershipTypes? _ownershipType;
        private string _primaryNameAttribute;
        private string _primaryImageAttribute;
        private string _primaryIdAttribute;
        private SecurityPrivilegeMetadata[] _privileges;
        private string _recurrenceBaseEntityLogicalName;
        private string _reportViewName;
        private string _schemaName;
        private string _physicalName;
        private int? _workflowSupport;
        private bool? _isManaged;
        private bool? _isReadingPaneEnabled;
        private bool? _isQuickCreateEnabled;
        private string _introducedVersion;
        private bool? _isStateModelAware;
        private bool? _enforceStateTransitions;
        private BooleanManagedProperty _canCreateAttributes;
        private BooleanManagedProperty _canCreateForms;
        private BooleanManagedProperty _canCreateCharts;
        private BooleanManagedProperty _canCreateViews;
        private BooleanManagedProperty _canBeRelatedEntityInRelationship;
        private BooleanManagedProperty _canBePrimaryEntityInRelationship;
        private BooleanManagedProperty _canBeInManyToMany;
        private BooleanManagedProperty _canModifyAdditionalSettings;
        private BooleanManagedProperty _canChangeHierarchicalRelationship;
        private BooleanManagedProperty _canChangeTrackingBeEnabled;
        private BooleanManagedProperty _canEnableSyncToExternalSearchIndex;
        private bool? _syncToExternalSearchIndex;
        private bool? _changeTrackingEnabled;
        private bool? _isOptimisticConcurrencyEnabled;
        private string _entityColor;
        private EntityKeyMetadata[] _keys;
        private string _logicalCollectionName;
        private string _collectionSchemaName;
        private BooleanManagedProperty _isOfflineInMobileClient;
        private int? _daysSinceRecordLastModified;
        private string _entitySetName;
        private bool? _isEnabledForExternalChannels;
        private bool? _isPrivate;

        [DataMember]
        public int? ActivityTypeMask
        {
            get
            {
                return this._activityTypeMask;
            }
            set
            {
                this._activityTypeMask = value;
            }
        }

        [DataMember]
        public AttributeMetadata[] Attributes
        {
            get
            {
                return this._attributes;
            }
            internal set
            {
                this._attributes = value;
            }
        }

        [DataMember]
        public bool? AutoRouteToOwnerQueue
        {
            get
            {
                return this.autoRouteToOwnerQueue;
            }
            set
            {
                this.autoRouteToOwnerQueue = value;
            }
        }

        [DataMember]
        public bool? CanTriggerWorkflow
        {
            get
            {
                return this._canTriggerWorkflow;
            }
            internal set
            {
                this._canTriggerWorkflow = value;
            }
        }

        [DataMember]
        public Label Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        [DataMember]
        public Label DisplayCollectionName
        {
            get
            {
                return this._displayCollectionName;
            }
            set
            {
                this._displayCollectionName = value;
            }
        }

        [DataMember]
        public Label DisplayName
        {
            get
            {
                return this._displayName;
            }
            set
            {
                this._displayName = value;
            }
        }

        [DataMember(Order = 70)]
        public bool? EntityHelpUrlEnabled
        {
            get
            {
                return this._entityHelpUrlEnabled;
            }
            set
            {
                this._entityHelpUrlEnabled = value;
            }
        }

        [DataMember(Order = 70)]
        public string EntityHelpUrl
        {
            get
            {
                return this._entityHelpUrl;
            }
            set
            {
                this._entityHelpUrl = value;
            }
        }

        [DataMember]
        public bool? IsDocumentManagementEnabled
        {
            get
            {
                return this._isDocumentMangementEnabled;
            }
            set
            {
                this._isDocumentMangementEnabled = value;
            }
        }

        [DataMember]
        public bool? IsOneNoteIntegrationEnabled
        {
            get
            {
                return this._isOneNoteIntegrationEnabled;
            }
            set
            {
                this._isOneNoteIntegrationEnabled = value;
            }
        }

        [DataMember]
        public bool? IsInteractionCentricEnabled
        {
            get
            {
                return this._isInteractionCentricEnabled;
            }
            set
            {
                this._isInteractionCentricEnabled = value;
            }
        }

        [DataMember]
        public bool? IsKnowledgeManagementEnabled
        {
            get
            {
                return this._isKnowledgeManagementEnabled;
            }
            set
            {
                this._isKnowledgeManagementEnabled = value;
            }
        }

        [DataMember]
        public bool? AutoCreateAccessTeams
        {
            get
            {
                return this._autoCreateAccessTeams;
            }
            set
            {
                this._autoCreateAccessTeams = value;
            }
        }

        [DataMember]
        public bool? IsActivity
        {
            get
            {
                return this._isActivity;
            }
            set
            {
                this._isActivity = value;
            }
        }

        [DataMember]
        public bool? IsActivityParty
        {
            get
            {
                return this._isActivityParty;
            }
            set
            {
                this._isActivityParty = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsAuditEnabled
        {
            get
            {
                return this._isAuditEnabled;
            }
            set
            {
                this._isAuditEnabled = value;
            }
        }

        [DataMember]
        public bool? IsAvailableOffline
        {
            get
            {
                return this._isAvailableOffline;
            }
            set
            {
                this._isAvailableOffline = value;
            }
        }

        [DataMember]
        public bool? IsChildEntity
        {
            get
            {
                return this._isChildEntity;
            }
            internal set
            {
                this._isChildEntity = value;
            }
        }

        [DataMember]
        public bool? IsAIRUpdated
        {
            get
            {
                return this._isAIRUpdated;
            }
            internal set
            {
                this._isAIRUpdated = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsValidForQueue
        {
            get
            {
                return this._isValidForQueue;
            }
            set
            {
                this._isValidForQueue = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsConnectionsEnabled
        {
            get
            {
                return this._isConnectionsEnabled;
            }
            set
            {
                this._isConnectionsEnabled = value;
            }
        }

        [DataMember]
        public string IconLargeName
        {
            get
            {
                return this._iconLargeName;
            }
            set
            {
                this._iconLargeName = value;
            }
        }

        [DataMember]
        public string IconMediumName
        {
            get
            {
                return this._iconMediumName;
            }
            set
            {
                this._iconMediumName = value;
            }
        }

        [DataMember]
        public string IconSmallName
        {
            get
            {
                return this._iconSmallName;
            }
            set
            {
                this._iconSmallName = value;
            }
        }

        [DataMember]
        public bool? IsCustomEntity
        {
            get
            {
                return this._isCustomEntity;
            }
            internal set
            {
                this._isCustomEntity = value;
            }
        }

        [DataMember]
        public bool? IsBusinessProcessEnabled
        {
            get
            {
                return this._isBusinessProcessEnabled;
            }
            set
            {
                this._isBusinessProcessEnabled = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsCustomizable
        {
            get
            {
                return this._isCustomizable;
            }
            set
            {
                this._isCustomizable = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsRenameable
        {
            get
            {
                return this._isRenameable;
            }
            set
            {
                this._isRenameable = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsMappable
        {
            get
            {
                return this._isMappable;
            }
            set
            {
                this._isMappable = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsDuplicateDetectionEnabled
        {
            get
            {
                return this._isDuplicateDetectionEnabled;
            }
            set
            {
                this._isDuplicateDetectionEnabled = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanCreateAttributes
        {
            get
            {
                return this._canCreateAttributes;
            }
            set
            {
                this._canCreateAttributes = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanCreateForms
        {
            get
            {
                return this._canCreateForms;
            }
            set
            {
                this._canCreateForms = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanCreateViews
        {
            get
            {
                return this._canCreateViews;
            }
            set
            {
                this._canCreateViews = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanCreateCharts
        {
            get
            {
                return this._canCreateCharts;
            }
            set
            {
                this._canCreateCharts = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanBeRelatedEntityInRelationship
        {
            get
            {
                return this._canBeRelatedEntityInRelationship;
            }
            internal set
            {
                this._canBeRelatedEntityInRelationship = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanBePrimaryEntityInRelationship
        {
            get
            {
                return this._canBePrimaryEntityInRelationship;
            }
            internal set
            {
                this._canBePrimaryEntityInRelationship = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanBeInManyToMany
        {
            get
            {
                return this._canBeInManyToMany;
            }
            internal set
            {
                this._canBeInManyToMany = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanEnableSyncToExternalSearchIndex
        {
            get
            {
                return this._canEnableSyncToExternalSearchIndex;
            }
            set
            {
                this._canEnableSyncToExternalSearchIndex = value;
            }
        }

        [DataMember]
        public bool? SyncToExternalSearchIndex
        {
            get
            {
                return this._syncToExternalSearchIndex;
            }
            set
            {
                this._syncToExternalSearchIndex = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanModifyAdditionalSettings
        {
            get
            {
                return this._canModifyAdditionalSettings;
            }
            set
            {
                this._canModifyAdditionalSettings = value;
            }
        }

        [DataMember(Order = 70)]
        public BooleanManagedProperty CanChangeHierarchicalRelationship
        {
            get
            {
                return this._canChangeHierarchicalRelationship;
            }
            set
            {
                this._canChangeHierarchicalRelationship = value;
            }
        }

        [DataMember(Order = 71)]
        public bool? IsOptimisticConcurrencyEnabled
        {
            get
            {
                return this._isOptimisticConcurrencyEnabled;
            }
            internal set
            {
                this._isOptimisticConcurrencyEnabled = value;
            }
        }

        [DataMember]
        public bool? ChangeTrackingEnabled
        {
            get
            {
                return this._changeTrackingEnabled;
            }
            set
            {
                this._changeTrackingEnabled = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty CanChangeTrackingBeEnabled
        {
            get
            {
                return this._canChangeTrackingBeEnabled;
            }
            set
            {
                this._canChangeTrackingBeEnabled = value;
            }
        }

        [DataMember]
        public bool? IsImportable
        {
            get
            {
                return this._isImportable;
            }
            internal set
            {
                this._isImportable = value;
            }
        }

        [DataMember]
        public bool? IsIntersect
        {
            get
            {
                return this._isIntersect;
            }
            internal set
            {
                this._isIntersect = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsMailMergeEnabled
        {
            get
            {
                return this._isMailMergeEnabled;
            }
            set
            {
                this._isMailMergeEnabled = value;
            }
        }

        [DataMember]
        public bool? IsManaged
        {
            get
            {
                return this._isManaged;
            }
            internal set
            {
                this._isManaged = value;
            }
        }

        [DataMember]
        public bool? IsEnabledForCharts
        {
            get
            {
                return this._isEnabledForCharts;
            }
            internal set
            {
                this._isEnabledForCharts = value;
            }
        }

        [DataMember]
        public bool? IsEnabledForTrace
        {
            get
            {
                return this._isEnabledForTrace;
            }
            internal set
            {
                this._isEnabledForTrace = value;
            }
        }

        [DataMember]
        public bool? IsValidForAdvancedFind
        {
            get
            {
                return this._isValidForAdvancedFind;
            }
            internal set
            {
                this._isValidForAdvancedFind = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsVisibleInMobile
        {
            get
            {
                return this._isVisibleInMobile;
            }
            set
            {
                this._isVisibleInMobile = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsVisibleInMobileClient
        {
            get
            {
                return this._isVisibleInMobileClient;
            }
            set
            {
                this._isVisibleInMobileClient = value;
            }
        }

        [DataMember]
        public BooleanManagedProperty IsReadOnlyInMobileClient
        {
            get
            {
                return this._isReadOnlyInMobileClient;
            }
            set
            {
                this._isReadOnlyInMobileClient = value;
            }
        }

        [DataMember(Order = 72)]
        public BooleanManagedProperty IsOfflineInMobileClient
        {
            get
            {
                return this._isOfflineInMobileClient;
            }
            set
            {
                this._isOfflineInMobileClient = value;
            }
        }

        [DataMember(Order = 72)]
        public int? DaysSinceRecordLastModified
        {
            get
            {
                return this._daysSinceRecordLastModified;
            }
            set
            {
                this._daysSinceRecordLastModified = value;
            }
        }

        [DataMember]
        public bool? IsReadingPaneEnabled
        {
            get
            {
                return this._isReadingPaneEnabled;
            }
            set
            {
                this._isReadingPaneEnabled = value;
            }
        }

        [DataMember]
        public bool? IsQuickCreateEnabled
        {
            get
            {
                return this._isQuickCreateEnabled;
            }
            set
            {
                this._isQuickCreateEnabled = value;
            }
        }

        [DataMember]
        public string LogicalName
        {
            get
            {
                return this._logicalName;
            }
            set
            {
                this._logicalName = value;
            }
        }

        [DataMember]
        public ManyToManyRelationshipMetadata[] ManyToManyRelationships
        {
            get
            {
                return this._manyToManyRelationships;
            }
            internal set
            {
                this._manyToManyRelationships = value;
            }
        }

        [DataMember]
        public OneToManyRelationshipMetadata[] ManyToOneRelationships
        {
            get
            {
                return this._manyToOneRelationships;
            }
            internal set
            {
                this._manyToOneRelationships = value;
            }
        }

        [DataMember]
        public OneToManyRelationshipMetadata[] OneToManyRelationships
        {
            get
            {
                return this._oneToManyRelationships;
            }
            internal set
            {
                this._oneToManyRelationships = value;
            }
        }

        [DataMember]
        public int? ObjectTypeCode
        {
            get
            {
                return this._objectTypeCode;
            }
            internal set
            {
                this._objectTypeCode = value;
            }
        }

        [DataMember]
        public OwnershipTypes? OwnershipType
        {
            get
            {
                return this._ownershipType;
            }
            set
            {
                this._ownershipType = value;
            }
        }

        [DataMember]
        public string PrimaryNameAttribute
        {
            get
            {
                return this._primaryNameAttribute;
            }
            internal set
            {
                this._primaryNameAttribute = value;
            }
        }

        [DataMember(Order = 60)]
        public string PrimaryImageAttribute
        {
            get
            {
                return this._primaryImageAttribute;
            }
            internal set
            {
                this._primaryImageAttribute = value;
            }
        }

        [DataMember]
        public string PrimaryIdAttribute
        {
            get
            {
                return this._primaryIdAttribute;
            }
            internal set
            {
                this._primaryIdAttribute = value;
            }
        }

        [DataMember]
        public SecurityPrivilegeMetadata[] Privileges
        {
            get
            {
                return this._privileges;
            }
            internal set
            {
                this._privileges = value;
            }
        }

        [DataMember]
        public string RecurrenceBaseEntityLogicalName
        {
            get
            {
                return this._recurrenceBaseEntityLogicalName;
            }
            internal set
            {
                this._recurrenceBaseEntityLogicalName = value;
            }
        }

        [DataMember]
        public string ReportViewName
        {
            get
            {
                return this._reportViewName;
            }
            internal set
            {
                this._reportViewName = value;
            }
        }

        [DataMember]
        public string SchemaName
        {
            get
            {
                return this._schemaName;
            }
            set
            {
                this._schemaName = value;
            }
        }

        [DataMember(Order = 60)]
        public string IntroducedVersion
        {
            get
            {
                return this._introducedVersion;
            }
            internal set
            {
                this._introducedVersion = value;
            }
        }

        [DataMember]
        public bool? IsStateModelAware
        {
            get
            {
                return this._isStateModelAware;
            }
            internal set
            {
                this._isStateModelAware = value;
            }
        }

        [DataMember]
        public bool? EnforceStateTransitions
        {
            get
            {
                return this._enforceStateTransitions;
            }
            internal set
            {
                this._enforceStateTransitions = value;
            }
        }

        internal int? WorkflowSupport
        {
            get
            {
                return this._workflowSupport;
            }
            set
            {
                this._workflowSupport = value;
            }
        }

        internal string PhysicalName
        {
            get
            {
                return this._physicalName;
            }
            set
            {
                this._physicalName = value;
            }
        }

        [DataMember(Order = 71)]
        public string EntityColor
        {
            get
            {
                return this._entityColor;
            }
            set
            {
                this._entityColor = value;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [DataMember(Order = 71)]
        public EntityKeyMetadata[] Keys
        {
            get
            {
                return this._keys;
            }
            internal set
            {
                this._keys = value;
            }
        }

        [DataMember(Order = 71)]
        public string LogicalCollectionName
        {
            get
            {
                return this._logicalCollectionName;
            }
            internal set
            {
                this._logicalCollectionName = value;
            }
        }

        [DataMember(Order = 71)]
        public string CollectionSchemaName
        {
            get
            {
                return this._collectionSchemaName;
            }
            internal set
            {
                this._collectionSchemaName = value;
            }
        }

        [DataMember(Order = 72)]
        public string EntitySetName
        {
            get
            {
                return this._entitySetName;
            }
            set
            {
                this._entitySetName = value;
            }
        }

        [DataMember(Order = 72)]
        public bool? IsEnabledForExternalChannels
        {
            get
            {
                return this._isEnabledForExternalChannels;
            }
            set
            {
                this._isEnabledForExternalChannels = value;
            }
        }

        [DataMember(Order = 72)]
        public bool? IsPrivate
        {
            get
            {
                return this._isPrivate;
            }
            internal set
            {
                this._isPrivate = value;
            }
        }
    }
}
