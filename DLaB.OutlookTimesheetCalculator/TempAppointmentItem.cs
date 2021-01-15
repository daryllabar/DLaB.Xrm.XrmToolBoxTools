using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;
using TimeZone = Microsoft.Office.Interop.Outlook.TimeZone;

namespace DLaB.OutlookTimesheetCalculator
{
    public class NonOutlookAppointmentItem : AppointmentItem
    {
        public DateTime CreationTime { get; set; }

        public NonOutlookAppointmentItem(AppointmentItem appointment)
        {
            AllDayEvent = appointment.AllDayEvent;
            BillingInformation = appointment.BillingInformation;
            Body = appointment.Body;
            BusyStatus = appointment.BusyStatus;
            Categories = appointment.Categories;
            Companies = appointment.Companies;
            ConferenceServerAllowExternal = appointment.ConferenceServerAllowExternal;
            ConferenceServerPassword = appointment.ConferenceServerPassword;
            Duration = appointment.Duration;
            End = appointment.End;
            EndInEndTimeZone = appointment.EndInEndTimeZone;
            EndTimeZone = appointment.EndTimeZone;
            EndUTC = appointment.EndUTC;
            ForceUpdateToAllAttendees = appointment.ForceUpdateToAllAttendees;
            Importance = appointment.Importance;
            InternetCodepage = appointment.InternetCodepage;
            IsOnlineMeeting = appointment.IsOnlineMeeting;
            Location = appointment.Location;
            MarkForDownload = appointment.MarkForDownload;
            MeetingStatus = appointment.MeetingStatus;
            MessageClass = appointment.MessageClass;
            Mileage = appointment.Mileage;
            NetMeetingAutoStart = appointment.NetMeetingAutoStart;
            NetMeetingDocPathName = appointment.NetMeetingDocPathName;
            NetMeetingOrganizerAlias = appointment.NetMeetingOrganizerAlias;
            NetMeetingServer = appointment.NetMeetingServer;
            NetMeetingType = appointment.NetMeetingType;
            NetShowURL = appointment.NetShowURL;
            NoAging = appointment.NoAging;
            OptionalAttendees = appointment.OptionalAttendees;
            ReminderMinutesBeforeStart = appointment.ReminderMinutesBeforeStart;
            ReminderOverrideDefault = appointment.ReminderOverrideDefault;
            ReminderPlaySound = appointment.ReminderPlaySound;
            ReminderSet = appointment.ReminderSet;
            ReminderSoundFile = appointment.ReminderSoundFile;
            ReplyTime = appointment.ReplyTime;
            RequiredAttendees = appointment.RequiredAttendees;
            Resources = appointment.Resources;
            ResponseRequested = appointment.ResponseRequested;
            SendUsingAccount = appointment.SendUsingAccount;
            Sensitivity = appointment.Sensitivity;
            Start = appointment.Start;
            StartInStartTimeZone = appointment.StartInStartTimeZone;
            StartTimeZone = appointment.StartTimeZone;
            StartUTC = appointment.StartUTC;
            Subject = appointment.Subject;
            UnRead = appointment.UnRead;
            CreationTime = appointment.CreationTime;
        }

        #region _AppointmentItem Members

        public Actions Actions
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllDayEvent { get; set; }

        public Application Application
        {
            get { throw new NotImplementedException(); }
        }

        public Attachments Attachments
        {
            get { throw new NotImplementedException(); }
        }

        public bool AutoResolvedWinner
        {
            get { throw new NotImplementedException(); }
        }

        public string BillingInformation { get; set; }

        public string Body { get; set; }

        public OlBusyStatus BusyStatus { get; set; }

        public string Categories { get; set; }

        public OlObjectClass Class
        {
            get { throw new NotImplementedException(); }
        }

        public void ClearRecurrencePattern()
        {
            throw new NotImplementedException();
        }

        public void Close(OlInspectorClose saveMode)
        {
            throw new NotImplementedException();
        }

        public string Companies { get; set; }

        public bool ConferenceServerAllowExternal { get; set; }

        public string ConferenceServerPassword { get; set; }

        public Conflicts Conflicts
        {
            get { throw new NotImplementedException(); }
        }

        public string ConversationID
        {
            get { throw new NotImplementedException(); }
        }

        public string ConversationIndex
        {
            get { throw new NotImplementedException(); }
        }

        public string ConversationTopic
        {
            get { throw new NotImplementedException(); }
        }

        public dynamic Copy()
        {
            throw new NotImplementedException();
        }

        public AppointmentItem CopyTo(MAPIFolder destinationFolder, OlAppointmentCopyOptions copyOptions)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Display([Optional]object modal)
        {
            throw new NotImplementedException();
        }

        public OlDownloadState DownloadState
        {
            get { throw new NotImplementedException(); }
        }

        public int Duration { get; set; }

        public DateTime End { get; set; }

        public DateTime EndInEndTimeZone { get; set; }

        public TimeZone EndTimeZone { get; set; }

        public DateTime EndUTC { get; set; }

        public string EntryID
        {
            get { throw new NotImplementedException(); }
        }

        public bool ForceUpdateToAllAttendees { get; set; }

        public FormDescription FormDescription
        {
            get { throw new NotImplementedException(); }
        }

        public MailItem ForwardAsVcal()
        {
            throw new NotImplementedException();
        }

        public Conversation GetConversation()
        {
            throw new NotImplementedException();
        }

        public Inspector GetInspector
        {
            get { throw new NotImplementedException(); }
        }

        public AddressEntry GetOrganizer()
        {
            throw new NotImplementedException();
        }

        public RecurrencePattern GetRecurrencePattern()
        {
            throw new NotImplementedException();
        }

        public string GlobalAppointmentID
        {
            get { throw new NotImplementedException(); }
        }

        public OlImportance Importance { get; set; }

        public int InternetCodepage { get; set; }

        public bool IsConflict
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsOnlineMeeting { get; set; }

        public bool IsRecurring
        {
            get { throw new NotImplementedException(); }
        }

        public ItemProperties ItemProperties
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public Links Links
        {
            get { throw new NotImplementedException(); }
        }

        public string Location { get; set; }

        public dynamic MAPIOBJECT
        {
            get { throw new NotImplementedException(); }
        }

        public OlRemoteStatus MarkForDownload { get; set; }

        public OlMeetingStatus MeetingStatus { get; set; }

        public string MeetingWorkspaceURL
        {
            get { throw new NotImplementedException(); }
        }

        public string MessageClass { get; set; }

        public string Mileage { get; set; }

        public dynamic Move(MAPIFolder destFldr)
        {
            throw new NotImplementedException();
        }

        public bool NetMeetingAutoStart { get; set; }

        public string NetMeetingDocPathName { get; set; }

        public string NetMeetingOrganizerAlias { get; set; }

        public string NetMeetingServer { get; set; }

        public OlNetMeetingType NetMeetingType { get; set; }

        public string NetShowURL { get; set; }

        public bool NoAging { get; set; }

        public string OptionalAttendees { get; set; }

        public string Organizer
        {
            get { throw new NotImplementedException(); }
        }

        public int OutlookInternalVersion
        {
            get { throw new NotImplementedException(); }
        }

        public string OutlookVersion
        {
            get { throw new NotImplementedException(); }
        }

        public dynamic Parent
        {
            get { throw new NotImplementedException(); }
        }

        public void PrintOut()
        {
            throw new NotImplementedException();
        }

        public PropertyAccessor PropertyAccessor
        {
            get { throw new NotImplementedException(); }
        }

        public dynamic RTFBody { get; set; }

        public Recipients Recipients
        {
            get { throw new NotImplementedException(); }
        }

        public OlRecurrenceState RecurrenceState
        {
            get { throw new NotImplementedException(); }
        }

        public int ReminderMinutesBeforeStart { get; set; }

        public bool ReminderOverrideDefault { get; set; }

        public bool ReminderPlaySound { get; set; }

        public bool ReminderSet { get; set; }

        public string ReminderSoundFile { get; set; }

        public DateTime ReplyTime { get; set; }

        public string RequiredAttendees { get; set; }

        public string Resources { get; set; }

        public MeetingItem Respond(OlMeetingResponse response, [Optional]object fNoUi, [Optional]object fAdditionalTextDialog)
        {
            throw new NotImplementedException();
        }

        public bool ResponseRequested { get; set; }

        public OlResponseStatus ResponseStatus
        {
            get { throw new NotImplementedException(); }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string path, [Optional]object type)
        {
            throw new NotImplementedException();
        }

        public bool Saved
        {
            get { throw new NotImplementedException(); }
        }

        public void Send()
        {
            throw new NotImplementedException();
        }

        public Account SendUsingAccount { get; set; }

        public OlSensitivity Sensitivity { get; set; }

        public NameSpace Session
        {
            get { throw new NotImplementedException(); }
        }

        public void ShowCategoriesDialog()
        {
            throw new NotImplementedException();
        }

        public int Size
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime Start { get; set; }

        public DateTime StartInStartTimeZone { get; set; }

        public TimeZone StartTimeZone { get; set; }

        public DateTime StartUTC { get; set; }

        public string Subject { get; set; }

        public bool UnRead { get; set; }

        public UserProperties UserProperties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ItemEvents_10_Event Members

#pragma warning disable 67

        public event ItemEvents_10_AfterWriteEventHandler AfterWrite;

        public event ItemEvents_10_AttachmentAddEventHandler AttachmentAdd;

        public event ItemEvents_10_AttachmentReadEventHandler AttachmentRead;

        public event ItemEvents_10_AttachmentRemoveEventHandler AttachmentRemove;

        public event ItemEvents_10_BeforeAttachmentAddEventHandler BeforeAttachmentAdd;

        public event ItemEvents_10_BeforeAttachmentPreviewEventHandler BeforeAttachmentPreview;

        public event ItemEvents_10_BeforeAttachmentReadEventHandler BeforeAttachmentRead;

        public event ItemEvents_10_BeforeAttachmentSaveEventHandler BeforeAttachmentSave;

        public event ItemEvents_10_BeforeAttachmentWriteToTempFileEventHandler BeforeAttachmentWriteToTempFile;

        public event ItemEvents_10_BeforeAutoSaveEventHandler BeforeAutoSave;

        public event ItemEvents_10_BeforeCheckNamesEventHandler BeforeCheckNames;

        public event ItemEvents_10_BeforeDeleteEventHandler BeforeDelete;

        public event ItemEvents_10_BeforeReadEventHandler BeforeRead;

        event ItemEvents_10_CloseEventHandler ItemEvents_10_Event.Close
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public event ItemEvents_10_CustomActionEventHandler CustomAction;

        public event ItemEvents_10_CustomPropertyChangeEventHandler CustomPropertyChange;

        public event ItemEvents_10_ForwardEventHandler Forward;

        public event ItemEvents_10_OpenEventHandler Open;

        public event ItemEvents_10_PropertyChangeEventHandler PropertyChange;

        public event ItemEvents_10_ReadEventHandler Read;

        public event ItemEvents_10_ReplyEventHandler Reply;

        public event ItemEvents_10_ReplyAllEventHandler ReplyAll;

        event ItemEvents_10_SendEventHandler ItemEvents_10_Event.Send
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public event ItemEvents_10_UnloadEventHandler Unload;

        public event ItemEvents_10_WriteEventHandler Write;
        public event ItemEvents_10_ReadCompleteEventHandler ReadComplete;

#pragma warning restore 67

        #endregion
    }
}
