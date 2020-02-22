using Interop.INTERCEPTACTIVITYLib;
using ncl.app.Loyalty.Aloha.Relay.Interfaces;
using ncl.app.Loyalty.Aloha.Relay.Model;
using NCL.Loyalty;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ncl.app.Loyalty.Aloha.COMIntegration
{
    public class AlohaEventIntercept : IInterceptAlohaActivityVer2_30
    {
        private Configuration InterceptConfiguration { get; set; }
        private ILogWriter LogWriter { get; set; }

        public AlohaEventIntercept()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var config = File.ReadAllText(Path.Combine(location, "appsettings.json"));
            InterceptConfiguration = JsonConvert.DeserializeObject<Configuration>(config);

            this.LogWriter = new LogWriter();
            this.LogWriter.WriteLog($"{nameof(AlohaEventIntercept)}");
        }

        public void Startup(int SourceTermId, int hMainWnd)
        {
            this.LogWriter.WriteLog($"{nameof(Startup)}");
        }

        public void InitializationComplete(int SourceTermId)
        {
            this.LogWriter.WriteLog($"{nameof(InitializationComplete)}");
        }

        public void Shutdown(int SourceTermId)
        {
            this.LogWriter.WriteLog($"{nameof(Shutdown)}");
        }

        public void PostCloseCheck(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            this.LogWriter.WriteLog($"{nameof(PostCloseCheck)} with {nameof(SourceTermId)}:{SourceTermId},{nameof(EmployeeId)}:{EmployeeId},{nameof(QueueId)}:{QueueId},{nameof(TableId)}:{TableId},{nameof(CheckId)}:{CheckId},");
            
            var cardLogPath = InterceptConfiguration.AppSettings.CardLogPath;
            var retryListPath = InterceptConfiguration.AppSettings.RetryListPath;

            var orchestrator = new LoyaltyOrchestrator(this.InterceptConfiguration, this.LogWriter);

            try
            {
                Task.Run(() =>
                {
                    var result = orchestrator.SendTransactionsAsync(CheckId.ToString());
                    var didItWork = result.Result;
                    this.LogWriter.WriteLog($"{nameof(PostCloseCheck)}:: {nameof(SourceTermId)}:{SourceTermId},{nameof(EmployeeId)}:{EmployeeId},{nameof(QueueId)}:{QueueId},{nameof(TableId)}:{TableId},{nameof(CheckId)}:{CheckId}, Result was :: {(didItWork.HasValue ? didItWork.ToString() : "TRANSACTION NOT FOUND IN LOG - MUST BE NO CARD SCAN")}");
                });
            }
            catch (Exception ex)
            {
                this.LogWriter.WriteLog($"Error in {nameof(PostCloseCheck)}:: {nameof(SourceTermId)}:{SourceTermId},{nameof(EmployeeId)}:{EmployeeId},{nameof(QueueId)}:{QueueId},{nameof(TableId)}:{TableId},{nameof(CheckId)}:{CheckId}. Exception was {ex.Message} \r\n {ex.StackTrace}");
            }
        }

        #region Ignored
        public void LogIn(int SourceTermId, int EmployeeId, string Name)
        {
            //SrDebout.WriteDebout($"{nameof(LogIn)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LogOut(int SourceTermId, int EmployeeId, string Name)
        {
            //SrDebout.WriteDebout($"{nameof(LogOut)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ClockIn(int SourceTermId, int EmployeeId, string EmpName, int JobcodeId, string JobName)
        {
            //SrDebout.WriteDebout($"{nameof(ClockIn)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ClockOut(int SourceTermId, int EmployeeId, string EmpName)
        {
            //SrDebout.WriteDebout($"{nameof(ClockOut)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OpenTable(int SourceTermId, int EmployeeId, int QueueId, int TableId, int TableDefId, string Name)
        {
            //SrDebout.WriteDebout($"{nameof(OpenTable)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CloseTable(int SourceTermId, int EmployeeId, int QueueId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(CloseTable)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OpenCheck(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(OpenCheck)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CloseCheck(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(CloseCheck)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void TransferTable(int SourceTermId, int FromEmployeeId, int ToEmployeeId, int TableId, string NewName, int IsGetCheck)
        {
            //SrDebout.WriteDebout($"{nameof(TransferTable)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AcceptTable(int SourceTermId, int EmployeeId, int FromTableId, int ToTableId)
        {
            //SrDebout.WriteDebout($"{nameof(AcceptTable)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SaveTab(int SourceTermId, int EmployeeId, int TableId, string Name)
        {
            //SrDebout.WriteDebout($"{nameof(SaveTab)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AddTab(int SourceTermId, int EmployeeId, int FromTableId, int ToTableId)
        {
            //SrDebout.WriteDebout($"{nameof(AddTab)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void NameOrder(int SourceTermId, int EmployeeId, int QueueId, int TableId, string Name)
        {
            //SrDebout.WriteDebout($"{nameof(NameOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void Bump(int SourceTermId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(Bump)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AddItem(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            //SrDebout.WriteDebout($"{nameof(AddItem)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ModifyItem(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            //SrDebout.WriteDebout($"{nameof(ModifyItem)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OrderItems(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int ModeId)
        {
            //SrDebout.WriteDebout($"{nameof(OrderItems)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void HoldItems(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(HoldItems)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OpenItem(int SourceTermId, int EmployeeId, int EntryId, int ItemId, string Description, double Price)
        {
            //SrDebout.WriteDebout($"{nameof(OpenItem)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SpecialMessage(int SourceTermId, int EmployeeId, int MessageId, string Message)
        {
            //SrDebout.WriteDebout($"{nameof(SpecialMessage)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeleteItems(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            //SrDebout.WriteDebout($"{nameof(DeleteItems)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void UpdateItems(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(UpdateItems)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ApplyPayment(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            //SrDebout.WriteDebout($"{nameof(ApplyPayment)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AdjustPayment(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            //SrDebout.WriteDebout($"{nameof(AdjustPayment)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeletePayment(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int TenderId, int PaymentId)
        {
            //SrDebout.WriteDebout($"{nameof(DeletePayment)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ApplyComp(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {
            //SrDebout.WriteDebout($"{nameof(ApplyComp)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeleteComp(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {
            //SrDebout.WriteDebout($"{nameof(DeleteComp)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ApplyPromo(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId)
        {
            //SrDebout.WriteDebout($"{nameof(ApplyPromo)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeletePromo(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId)
        {
            //SrDebout.WriteDebout($"{nameof(DeletePromo)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void Custom(int SourceTermId, string Name)
        {
            //SrDebout.WriteDebout($"{nameof(Custom)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CarryoverId(int SourceTermId, int Type, int OldId, int NewId)
        {
            //SrDebout.WriteDebout($"{nameof(CarryoverId)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EndOfDay(int SourceTermId, int IsMaster)
        {
            //SrDebout.WriteDebout($"{nameof(EndOfDay)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CombineOrder(int SourceTermId, int EmployeeId, int SrcQueueId, int SrcTableId, int SrcCheckId, int DstQueueId, int DstTableId, int DstCheckId)
        {
            //SrDebout.WriteDebout($"{nameof(CombineOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OnClockTick(int SourceTermId)
        {
            //SrDebout.WriteDebout($"{nameof(OnClockTick)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PreModifyItem(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            //SrDebout.WriteDebout($"{nameof(PreModifyItem)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LockOrder(int SourceTermId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(LockOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void UnlockOrder(int SourceTermId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(UnlockOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SetMasterTerminal(int SourceTermId, int TerminalId)
        {
            //SrDebout.WriteDebout($"{nameof(SetMasterTerminal)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SetQuickComboLevel(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId, int nLevel, int nContext)
        {
            //SrDebout.WriteDebout($"{nameof(SetQuickComboLevel)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void TableToShowOnDispBChanged(int SourceTermId, int nTermID, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(TableToShowOnDispBChanged)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void StartAddItem(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId, int ParentEntryId, int ModCodeId, int ItemId, string ItemName, double ItemPrice)
        {
            //SrDebout.WriteDebout($"{nameof(StartAddItem)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CancelAddItem(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            //SrDebout.WriteDebout($"{nameof(CancelAddItem)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PostDeleteItems(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int ReasonId)
        {
            //SrDebout.WriteDebout($"{nameof(PostDeleteItems)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PostDeleteComp(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int CompTypeId, int CompId)
        {
            //SrDebout.WriteDebout($"{nameof(PostDeleteComp)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PostDeletePromo(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromotionId, int PromoId)
        {
            //SrDebout.WriteDebout($"{nameof(PostDeletePromo)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OrderScreen_TableCheckSeatChanged(int SourceTermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int SeatNum)
        {
            //SrDebout.WriteDebout($"{nameof(OrderScreen_TableCheckSeatChanged)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EventNotification(int SourceTermId, int EmployeeId, int TableId, ALOHA_ACTIVITY_EVENT_NOTIFICATION_TYPES EventNotification)
        {
            //SrDebout.WriteDebout($"{nameof(EventNotification)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void RerouteDisplayBoard(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int DisplayBoardID, int ControllingTerminalID, int DefaultOrderModeOverride, int CurrentOrderOnly)
        {
            //SrDebout.WriteDebout($"{nameof(RerouteDisplayBoard)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ChangeItemSize(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int EntryId)
        {
            //SrDebout.WriteDebout($"{nameof(ChangeItemSize)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AdvanceOrder(int SourceTermId, int EmployeeId, int QueueId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(AdvanceOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EnrollEmployee(int SourceTermId, int EmployeeId, int numTries)
        {
            //SrDebout.WriteDebout($"{nameof(EnrollEmployee)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void MasterDown(int SourceTermId)
        {
            //SrDebout.WriteDebout($"{nameof(MasterDown)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void IAmMaster(int SourceTermId)
        {
            //SrDebout.WriteDebout($"{nameof(IAmMaster)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void FileServerDown(int SourceTermId)
        {
            //SrDebout.WriteDebout($"{nameof(FileServerDown)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void FileServer(int SourceTermId, string serverName)
        {
            //SrDebout.WriteDebout($"{nameof(FileServer)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SettleInfoChanged(int SourceTermId, string SettleInfo)
        {
            //SrDebout.WriteDebout($"{nameof(SettleInfoChanged)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SplitCheck(int SourceTermId, int CheckId, int TableId, int QueueId, int EmployeeNumber, int NumberOfSplits, int SplitType)
        {
            //SrDebout.WriteDebout($"{nameof(SplitCheck)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AuthorizePayment(int SourceTermId, int TableId, int CheckId, int PaymentId, int TransactionType, int TransactionResult)
        {
            //SrDebout.WriteDebout($"{nameof(AuthorizePayment)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CurrentCheckChanged(int SourceTermId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(CurrentCheckChanged)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void FinalBump(int SourceTermId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(FinalBump)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AssignCashDrawer(int SourceTermId, int EmployeeId, int DrawerId, int IsPublic)
        {
            //SrDebout.WriteDebout($"{nameof(AssignCashDrawer)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ReassignCashDrawer(int SourceTermId, int EmployeeId, int DrawerId)
        {
            //SrDebout.WriteDebout($"{nameof(ReassignCashDrawer)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeassignCashDrawer(int SourceTermId, int EmployeeId, int DrawerId, int IsPublic)
        {
            //SrDebout.WriteDebout($"{nameof(DeassignCashDrawer)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ReopenCheck(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(ReopenCheck)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void NameOrder(int SourceTermId, int EmployeeId, int QueueId, int TableId, string Name, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(NameOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EnterIberScreen(int SourceTermId, int ScreenId)
        {
            //SrDebout.WriteDebout($"{nameof(EnterIberScreen)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ExitIberScreen(int SourceTermId, int ScreenId)
        {
            //SrDebout.WriteDebout($"{nameof(ExitIberScreen)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void KitchenOrderStatus(int SourceTermId, string Orders)
        {
            //SrDebout.WriteDebout($"{nameof(KitchenOrderStatus)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void RenameTab(int SourceTermId, int CheckId, string tabName)
        {
            //SrDebout.WriteDebout($"{nameof(RenameTab)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PostLockOrder(int SourceTermId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(PostLockOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PostUnlockOrder(int SourceTermId, int TableId)
        {
            //SrDebout.WriteDebout($"{nameof(PostUnlockOrder)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SpoolMode(int SourceTermId, int SpoolMode)
        {
            //SrDebout.WriteDebout($"{nameof(SpoolMode)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void UpdateDisplayBoard(int SourceTermId, int DisplayBoardID, int TerminalId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(UpdateDisplayBoard)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AddDeferredModifier(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int ParentEntryId, int ModGroupId, string ModGroupName)
        {
            //SrDebout.WriteDebout($"{nameof(AddDeferredModifier)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void RemoveDeferredModifier(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int ParentEntryId, int ModGroupId)
        {
            //SrDebout.WriteDebout($"{nameof(RemoveDeferredModifier)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeleteLoyaltyMember(int SourceTermId, int CheckId, string CardNumber)
        {
            //SrDebout.WriteDebout($"{nameof(DeleteLoyaltyMember)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LogMergedByMaster(int MasterTermId, int MergedTermId, string FileServerName)
        {
            //SrDebout.WriteDebout($"{nameof(LogMergedByMaster)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void BeginLogMergeByMaster(int MasterTermId, int MergedTermId, string FileServerName)
        {
            //SrDebout.WriteDebout($"{nameof(BeginLogMergeByMaster)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void MasterSyncedMyLog(int LocalTermId, int MasterTermId, string FileServerName)
        {
            //SrDebout.WriteDebout($"{nameof(MasterSyncedMyLog)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ReqMasterToSyncMyLog(int LocalTermId, int MasterTermId, string FileServerName)
        {
            //SrDebout.WriteDebout($"{nameof(ReqMasterToSyncMyLog)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ActivatePinPad(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(ActivatePinPad)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ResetPinPad(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(ResetPinPad)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void BeginTransaction(int TermId, string txnKey, int EmployeeId, double amount, double tip)
        {
            //SrDebout.WriteDebout($"{nameof(BeginTransaction)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CancelTransaction(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(CancelTransaction)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DisplayBankResponseMessages(int TermId, string bankResponse)
        {
            //SrDebout.WriteDebout($"{nameof(DisplayBankResponseMessages)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void GenericPinPadCommand(int TermId, string input)
        {
            //SrDebout.WriteDebout($"{nameof(GenericPinPadCommand)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void GiftCardBalance(int TermId, int CardId, ref CardDataInfo CardDetail)
        {
            //SrDebout.WriteDebout($"{nameof(GiftCardBalance)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LiveRefreshStarted(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(LiveRefreshStarted)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void OrderItems(int SourceTermId, int OrderTermId, int EmployeeId, int QueueId, int TableId, int CheckId, int ModeId)
        {
            //SrDebout.WriteDebout($"{nameof(OrderItems)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LiveRefreshDetected(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(LiveRefreshDetected)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LiveRefreshLockoutStart(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(LiveRefreshLockoutStart)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LiveRefreshComplete(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(LiveRefreshComplete)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ActivateFloatingDashboardByTerminal(int TermId, int dashboardId)
        {
            //SrDebout.WriteDebout($"{nameof(ActivateFloatingDashboardByTerminal)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DisableFloatingDashboardByTerminal(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(DisableFloatingDashboardByTerminal)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ItemAvailability(int TermId, int ItemId, int itemCount)
        {
            //SrDebout.WriteDebout($"{nameof(ItemAvailability)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EmployeeMagneticCardEnabled(int TermId, int enabled)
        {
            //SrDebout.WriteDebout($"{nameof(EmployeeMagneticCardEnabled)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SeatNameChanged(int TermId, int TableId, int seatNumber, string seatName)
        {
            //SrDebout.WriteDebout($"{nameof(SeatNameChanged)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LoyaltyMemberItemListUpdated(int TermId, int EmployeeId, int TableId, int QueueId, string[] UpdatedMemberList)
        {
            //SrDebout.WriteDebout($"{nameof(LoyaltyMemberItemListUpdated)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SystemEventCalled(int TermId, int eventType, int idChanged)
        {
            //SrDebout.WriteDebout($"{nameof(SystemEventCalled)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void CarryoverCheck(int SourceTermId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(CarryoverCheck)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void BeforeReopenCheck(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(BeforeReopenCheck)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void MoveLoyaltyCard(int SourceTermId, int EmployeeId, int FromQueueId, int FromTableId, int FromCheckId, int ToQueueId, int ToTableId, int ToCheckId, string[] MemberList)
        {
            //SrDebout.WriteDebout($"{nameof(MoveLoyaltyCard)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void UpdateLoyaltyMemberItemList(int TermId, int EmployeeId, int QueueId, int TableId, int CheckId, string[] MemberList)
        {
            //SrDebout.WriteDebout($"{nameof(UpdateLoyaltyMemberItemList)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SplitEntry(int TermId, int EmployeeId, int QueueId, int TableId, int CheckId, int OriginalEntryId, int NewEntryId)
        {
            //SrDebout.WriteDebout($"{nameof(SplitEntry)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LoyaltyCardAppliedToCheck(int SourceTermId, int EmployeeId, int QueueId, int TableId, int CheckId, ref CardDetail[] CardList)
        {
            //SrDebout.WriteDebout($"{nameof(LoyaltyCardAppliedToCheck)}", SrDebout.DeboutLevel.LogAlways);
        }


        public void TablesCombined(int SourceTermId, int EmployeeId, int PrimaryTableDefId, int SequenceId, int[] TableDefIdList)
        {
            //SrDebout.WriteDebout($"{nameof(TablesCombined)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void TablesDetached(int SourceTermId, int EmployeeId, int OldPrimaryTableDefId, int NewPrimaryTableDefId, int SequenceId, int[] TableDefIdList)
        {
            //SrDebout.WriteDebout($"{nameof(TablesDetached)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void PostEncryptionBlockReady(string RequestId)
        {
            //SrDebout.WriteDebout($"{nameof(PostEncryptionBlockReady)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void NoSaleCashDrawerOpened(int SourceTermId, int EmployeeId, int DrawerId, int ReasonId)
        {
            //SrDebout.WriteDebout($"{nameof(NoSaleCashDrawerOpened)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void AssignEfreqMember(int TermId, int EmployeeId, int TableId, int CheckId, string memberId, string TrackInfo, int mgrIdOverrideMaxChecks, int mgrIdOverrideMagCardOnly, int mgrIdOverrideSeriesUse)
        {
            //SrDebout.WriteDebout($"{nameof(AssignEfreqMember)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void UnassignEfreqMember(int SourceTermId, int EmployeeId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(UnassignEfreqMember)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void ReassignEfreqMember(int SourceTermId, int EmployeeId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(ReassignEfreqMember)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void DeleteEfreqMember(int SourceTermId, int EmployeeId, int TableId, int CheckId)
        {
            //SrDebout.WriteDebout($"{nameof(DeleteEfreqMember)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void QuickRefundCreated(int TermId, int EmployeeId, int TableId, int QueueId, int CheckId, int OriginalCheckId)
        {
            //SrDebout.WriteDebout($"{nameof(QuickRefundCreated)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void LogWasMerged()
        {
            //SrDebout.WriteDebout($"{nameof(LogWasMerged)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void MasterConflictDetected()
        {
            //SrDebout.WriteDebout($"{nameof(MasterConflictDetected)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void BeginPromoSubstitution(int TermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromoId, int PromoEntryId)
        {
            //SrDebout.WriteDebout($"{nameof(BeginPromoSubstitution)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EndPromoSubstitution(int TermId, int ManagerId, int EmployeeId, int QueueId, int TableId, int CheckId, int PromoId, int PromoEntryId)
        {
            //SrDebout.WriteDebout($"{nameof(EndPromoSubstitution)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void TerminalDown(int TermId)
        {
            //SrDebout.WriteDebout($"{nameof(TerminalDown)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void EventNotificationDebug(int SourceTermId, int enabled, string EventText)
        {
            //SrDebout.WriteDebout($"{nameof(EventNotificationDebug)}", SrDebout.DeboutLevel.LogAlways);
        }

        public void SystemEventNotifyAllTerminals(int TermId, int eventType, int idChanged)
        {
            //SrDebout.WriteDebout($"{nameof(SystemEventNotifyAllTerminals)}", SrDebout.DeboutLevel.LogAlways);
        }
        #endregion Ignored
    }
}