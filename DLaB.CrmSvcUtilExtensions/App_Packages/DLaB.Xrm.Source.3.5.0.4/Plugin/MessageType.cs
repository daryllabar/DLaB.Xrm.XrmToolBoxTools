#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Defines all Default CRM Message Types.  Utilizes a TypeSafeEnumBase to allow for custom Actions to be added
    /// </summary>
    public class MessageType : TypeSafeEnumBase<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageType" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public MessageType(string name, string value) : base(name, value) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageType" /> class.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        public MessageType(string nameValue) : base(nameValue, nameValue) { }

        #region Default OOB CRM Message Types

        /// <summary>
        /// Add Item Message.
        /// </summary>
        public static MessageType AddItem = new MessageType("AddItem");
        /// <summary>
        /// Add List Memebers Message.
        /// </summary>
        public static MessageType AddListMembers = new MessageType("AddListMembers");
        /// <summary>
        /// Add Member Message.
        /// </summary>
        public static MessageType AddMember = new MessageType("AddMember");
        /// <summary>
        /// Add Members Message.
        /// </summary>
        public static MessageType AddMembers = new MessageType("AddMembers");
        /// <summary>
        /// Add Principal To Queue Message.
        /// </summary>
        public static MessageType AddPrincipalToQueue = new MessageType("AddPrincipalToQueue");
        /// <summary>
        /// Add Privileges Message.
        /// </summary>
        public static MessageType AddPrivileges = new MessageType("AddPrivileges");
        /// <summary>
        /// Add Product To Kit Message.
        /// </summary>
        public static MessageType AddProductToKit = new MessageType("AddProductToKit");
        /// <summary>
        /// Add Recurrence Message.
        /// </summary>
        public static MessageType AddRecurrence = new MessageType("AddRecurrence");
        /// <summary>
        /// Add To Queue Message.
        /// </summary>
        public static MessageType AddToQueue = new MessageType("AddToQueue");
        /// <summary>
        /// Add User To Record Team Message.
        /// </summary>
        public static MessageType AddUserToRecordTeam = new MessageType("AddUserToRecordTeam");
        /// <summary>
        /// Assign Message.
        /// </summary>
        public static MessageType Assign = new MessageType("Assign");
        /// <summary>
        /// Assign User Roles Message.
        /// </summary>
        public static MessageType AssignUserRoles = new MessageType("AssignUserRoles");
        /// <summary>
        /// Associate Message.
        /// </summary>
        public static MessageType Associate = new MessageType("Associate");
        /// <summary>
        /// Background Send Message.
        /// </summary>
        public static MessageType BackgroundSend = new MessageType("BackgroundSend");
        /// <summary>
        /// Book Message.
        /// </summary>
        public static MessageType Book = new MessageType("Book");
        /// <summary>
        /// Cancel Message.
        /// </summary>
        public static MessageType Cancel = new MessageType("Cancel");
        /// <summary>
        /// Check Incoming Message.
        /// </summary>
        public static MessageType CheckIncoming = new MessageType("CheckIncoming");
        /// <summary>
        /// Check Promote Message.
        /// </summary>
        public static MessageType CheckPromote = new MessageType("CheckPromote");
        /// <summary>
        /// Clone Message.
        /// </summary>
        public static MessageType Clone = new MessageType("Clone");
        /// <summary>
        /// Close Message.
        /// </summary>
        public static MessageType Close = new MessageType("Close");
        /// <summary>
        /// Copy Dynamic List To Static Message.
        /// </summary>
        public static MessageType CopyDynamicListToStatic = new MessageType("CopyDynamicListToStatic");
        /// <summary>
        /// Copy System Form Message.
        /// </summary>
        public static MessageType CopySystemForm = new MessageType("CopySystemForm");
        /// <summary>
        /// Create Message.
        /// </summary>
        public static MessageType Create = new MessageType("Create");
        /// <summary>
        /// Create Exception Message.
        /// </summary>
        public static MessageType CreateException = new MessageType("CreateException");
        /// <summary>
        /// Create Instance Message.
        /// </summary>
        public static MessageType CreateInstance = new MessageType("CreateInstance");
        /// <summary>
        /// Delete Message.
        /// </summary>
        public static MessageType Delete = new MessageType("Delete");
        /// <summary>
        /// Delete Open Instances Message.
        /// </summary>
        public static MessageType DeleteOpenInstances = new MessageType("DeleteOpenInstances");
        /// <summary>
        /// Deliver Incoming Message.
        /// </summary>
        public static MessageType DeliverIncoming = new MessageType("DeliverIncoming");
        /// <summary>
        /// Deliver Promote Message.
        /// </summary>
        public static MessageType DeliverPromote = new MessageType("DeliverPromote");
        /// <summary>
        /// Detach From Queue Message.
        /// </summary>
        public static MessageType DetachFromQueue = new MessageType("DetachFromQueue");
        /// <summary>
        /// Disassociate Message.
        /// </summary>
        public static MessageType Disassociate = new MessageType("Disassociate");
        /// <summary>
        /// Execute Message.
        /// </summary>
        public static MessageType Execute = new MessageType("Execute");
        /// <summary>
        /// Execute By Id Message.
        /// </summary>
        public static MessageType ExecuteById = new MessageType("ExecuteById");
        /// <summary>
        /// Export Message.
        /// </summary>
        public static MessageType Export = new MessageType("Export");
        /// <summary>
        /// Export All Message.
        /// </summary>
        public static MessageType ExportAll = new MessageType("ExportAll");
        /// <summary>
        /// Export Compressed Message.
        /// </summary>
        public static MessageType ExportCompressed = new MessageType("ExportCompressed");
        /// <summary>
        /// Export Compressed All Message.
        /// </summary>
        public static MessageType ExportCompressedAll = new MessageType("ExportCompressedAll");
        /// <summary>
        /// Generate Social Profile Message.
        /// </summary>
        public static MessageType GenerateSocialProfile = new MessageType("GenerateSocialProfile");
        /// <summary>
        /// Grant Access Message.
        /// </summary>
        public static MessageType GrantAccess = new MessageType("GrantAccess");
        /// <summary>
        /// Handle Message.
        /// </summary>
        public static MessageType Handle = new MessageType("Handle");
        /// <summary>
        /// Import Message.
        /// </summary>
        public static MessageType Import = new MessageType("Import");
        /// <summary>
        /// Import All Message.
        /// </summary>
        public static MessageType ImportAll = new MessageType("ImportAll");
        /// <summary>
        /// Import Compressed All Message.
        /// </summary>
        public static MessageType ImportCompressedAll = new MessageType("ImportCompressedAll");
        /// <summary>
        /// Import Compressed With Progress Message.
        /// </summary>
        public static MessageType ImportCompressedWithProgress = new MessageType("ImportCompressedWithProgress");
        /// <summary>
        /// Import With Progress Message.
        /// </summary>
        public static MessageType ImportWithProgress = new MessageType("ImportWithProgress");
        /// <summary>
        /// Lock Invoice Pricing Message.
        /// </summary>
        public static MessageType LockInvoicePricing = new MessageType("LockInvoicePricing");
        /// <summary>
        /// Lock Sales Order Pricing Message.
        /// </summary>
        public static MessageType LockSalesOrderPricing = new MessageType("LockSalesOrderPricing");
        /// <summary>
        /// Lose Message.
        /// </summary>
        public static MessageType Lose = new MessageType("Lose");
        /// <summary>
        /// Merge Message.
        /// </summary>
        public static MessageType Merge = new MessageType("Merge");
        /// <summary>
        /// Modify Access Message.
        /// </summary>
        public static MessageType ModifyAccess = new MessageType("ModifyAccess");
        /// <summary>
        /// Pick From Queue Message.
        /// </summary>
        public static MessageType PickFromQueue = new MessageType("PickFromQueue");
        /// <summary>
        /// Publish Message.
        /// </summary>
        public static MessageType Publish = new MessageType("Publish");
        /// <summary>
        /// Publish All Message.
        /// </summary>
        public static MessageType PublishAll = new MessageType("PublishAll");
        /// <summary>
        /// Qualify Lead Message.
        /// </summary>
        public static MessageType QualifyLead = new MessageType("QualifyLead");
        /// <summary>
        /// Recalculate Message.
        /// </summary>
        public static MessageType Recalculate = new MessageType("Recalculate");
        /// <summary>
        /// Release To Queue Message.
        /// </summary>
        public static MessageType ReleaseToQueue = new MessageType("ReleaseToQueue");
        /// <summary>
        /// Remove From Queue Message.
        /// </summary>
        public static MessageType RemoveFromQueue = new MessageType("RemoveFromQueue");
        /// <summary>
        /// Remove Item Message.
        /// </summary>
        public static MessageType RemoveItem = new MessageType("RemoveItem");
        /// <summary>
        /// Remove Member Message.
        /// </summary>
        public static MessageType RemoveMember = new MessageType("RemoveMember");
        /// <summary>
        /// Remove Members Message.
        /// </summary>
        public static MessageType RemoveMembers = new MessageType("RemoveMembers");
        /// <summary>
        /// Remove Privilege Message.
        /// </summary>
        public static MessageType RemovePrivilege = new MessageType("RemovePrivilege");
        /// <summary>
        /// Remove Product From Kit Message.
        /// </summary>
        public static MessageType RemoveProductFromKit = new MessageType("RemoveProductFromKit");
        /// <summary>
        /// Remove Related Message.
        /// </summary>
        public static MessageType RemoveRelated = new MessageType("RemoveRelated");
        /// <summary>
        /// Remove User From Record Team Message.
        /// </summary>
        public static MessageType RemoveUserFromRecordTeam = new MessageType("RemoveUserFromRecordTeam");
        /// <summary>
        /// Remove User Roles Message.
        /// </summary>
        public static MessageType RemoveUserRoles = new MessageType("RemoveUserRoles");
        /// <summary>
        /// Replace Privileges Message.
        /// </summary>
        public static MessageType ReplacePrivileges = new MessageType("ReplacePrivileges");
        /// <summary>
        /// Reschedule Message.
        /// </summary>
        public static MessageType Reschedule = new MessageType("Reschedule");
        /// <summary>
        /// Retrieve Message.
        /// </summary>
        public static MessageType Retrieve = new MessageType("Retrieve");
        /// <summary>
        /// Retrieve Exchange Rate Message.
        /// </summary>
        public static MessageType RetrieveExchangeRate = new MessageType("RetrieveExchangeRate");
        /// <summary>
        /// Retrieve Filtered Forms Message.
        /// </summary>
        public static MessageType RetrieveFilteredForms = new MessageType("RetrieveFilteredForms");
        /// <summary>
        /// Retrieve Multiple Message.
        /// </summary>
        public static MessageType RetrieveMultiple = new MessageType("RetrieveMultiple");
        /// <summary>
        /// Retrieve Personal Wall Message.
        /// </summary>
        public static MessageType RetrievePersonalWall = new MessageType("RetrievePersonalWall");
        /// <summary>
        /// Retrieve Principal Access Message.
        /// </summary>
        public static MessageType RetrievePrincipalAccess = new MessageType("RetrievePrincipalAccess");
        /// <summary>
        /// Retrieve Record Wall Message.
        /// </summary>
        public static MessageType RetrieveRecordWall = new MessageType("RetrieveRecordWall");
        /// <summary>
        /// Retrieve Shared Principals And Access Message.
        /// </summary>
        public static MessageType RetrieveSharedPrincipalsAndAccess = new MessageType("RetrieveSharedPrincipalsAndAccess");
        /// <summary>
        /// Retrieve Unpublished Message.
        /// </summary>
        public static MessageType RetrieveUnpublished = new MessageType("RetrieveUnpublished");
        /// <summary>
        /// Retrieve Unpublished Multiple Message.
        /// </summary>
        public static MessageType RetrieveUnpublishedMultiple = new MessageType("RetrieveUnpublishedMultiple");
        /// <summary>
        /// Retrieve User Queues Message.
        /// </summary>
        public static MessageType RetrieveUserQueues = new MessageType("RetrieveUserQueues");
        /// <summary>
        /// Revoke Access Message.
        /// </summary>
        public static MessageType RevokeAccess = new MessageType("RevokeAccess");
        /// <summary>
        /// Route Message.
        /// </summary>
        public static MessageType Route = new MessageType("Route");
        /// <summary>
        /// Route To Message.
        /// </summary>
        public static MessageType RouteTo = new MessageType("RouteTo");
        /// <summary>
        /// Send Message.
        /// </summary>
        public static MessageType Send = new MessageType("Send");
        /// <summary>
        /// Send From Template Message.
        /// </summary>
        public static MessageType SendFromTemplate = new MessageType("SendFromTemplate");
        /// <summary>
        /// Set Related Message.
        /// </summary>
        public static MessageType SetRelated = new MessageType("SetRelated");
        /// <summary>
        /// Set State Message.
        /// </summary>
        public static MessageType SetState = new MessageType("SetState");
        /// <summary>
        /// Set State Dynamic Entity Message.
        /// </summary>
        public static MessageType SetStateDynamicEntity = new MessageType("SetStateDynamicEntity");
        /// <summary>
        /// Trigger Service Endpoint Check Message.
        /// </summary>
        public static MessageType TriggerServiceEndpointCheck = new MessageType("TriggerServiceEndpointCheck");
        /// <summary>
        /// Unlock Invoice Pricing Message.
        /// </summary>
        public static MessageType UnlockInvoicePricing = new MessageType("UnlockInvoicePricing");
        /// <summary>
        /// Unlock Sales Order Pricing Message.
        /// </summary>
        public static MessageType UnlockSalesOrderPricing = new MessageType("UnlockSalesOrderPricing");
        /// <summary>
        /// Update Message.
        /// </summary>
        public static MessageType Update = new MessageType("Update");
        /// <summary>
        /// Validate Recurrence Rule Message.
        /// </summary>
        public static MessageType ValidateRecurrenceRule = new MessageType("ValidateRecurrenceRule");
        /// <summary>
        /// Win Message.
        /// </summary>
        public static MessageType Win = new MessageType("Win");

        #endregion Default OOB CRM Message Types
        
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// Since this is open (the constructors are public, not private) to allow for Custom Actions to be added, overriding the ToString to allow for value equality
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            return obj != null && Equals(obj as MessageType);

        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// Since this is open (the constructors are public, not private) to allow for Custom Actions to be added, overriding the ToString to allow for value equality
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Equals(MessageType message)
        {
            // If parameter is null return false:
            if (message == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Value == message.Value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Value ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Implements the operator == to allow for standard comparisons just like enums.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MessageType a, MessageType b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return !(a is null) && a.Equals(b);
        }

        /// <summary>
        /// Implements the operator != to allow for standard comparisons just like enums.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MessageType a, MessageType b)
        {
            return !(a == b);
        }
    }
}
