using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Gbase;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Xml;
using ExpenseProcessingSystem.ViewModels.Reports;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace ExpenseProcessingSystem.Services
{
    public class HomeService
    {
        private readonly string defaultPW = "Mizuho2019";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private readonly GWriteContext _gWriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ModalService _modalservice;
        private FilterQueryService _filterservice;
        XElement xelemAcc = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
        XElement xelemLiq = XElement.Load("wwwroot/xml/LiquidationValue.xml");
        XElement xelemReport = XElement.Load("wwwroot/xml/ReportHeader.xml");
        private ModelStateDictionary _modelState;
        private NumberToText _class;
        public HomeService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext goContext, GWriteContext gWriteContext, ModelStateDictionary modelState, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = goContext;
            _gWriteContext = gWriteContext;
            _modelState = modelState;
            _hostingEnvironment = hostingEnvironment;
            _modalservice = new ModalService(_httpContextAccessor, _context, _gWriteContext);
            _filterservice = new FilterQueryService();
            _class = new NumberToText();
        }
        public string getUserRole(string id)
        {
            var data = _context.User.Where(x => x.User_ID == int.Parse(id))
                .Select(x => x.User_Role).FirstOrDefault() ?? "";
            return data;
        }
        private int[] status = { GlobalSystemValues.STATUS_POSTED, GlobalSystemValues.STATUS_FOR_CLOSING,
                            GlobalSystemValues.STATUS_FOR_PRINTING };
        private int[] statusTrans = { GlobalSystemValues.STATUS_APPROVED, GlobalSystemValues.STATUS_RESENDING_COMPLETE,
                            GlobalSystemValues.STATUS_REVERSING_COMPLETE };

        //-----------------------------------Populate-------------------------------------//
        //[ Home ]
        //[Notification]
        public List<HomeNotifViewModel> populateNotif(FiltersViewModel filters, int loggedUID)
        {
            var mList = (from notifs in (from n in _context.HomeNotif
                                             //for currently logged in User
                                         join user in _context.User
                                         on loggedUID
                                         equals user.User_ID
                                         into c
                                         from user in c.DefaultIfEmpty()
                                         where n.Notif_UserFor_ID == loggedUID ||
                                         (n.Notif_UserFor_ID == 0 && n.Notif_Application_Maker_ID != loggedUID)
                                         select new { n, user })
                         join creator in _context.User
                         on notifs.n.Notif_Application_Maker_ID
                         equals creator.User_ID
                         into c
                         from creator in c.DefaultIfEmpty()
                         join stat in _context.StatusList
                         on notifs.n.Notif_Status_ID
                         equals stat.Status_ID
                         into s
                         from stat in s.DefaultIfEmpty()
                         select new
                         {
                             notifs.n.Notif_ID,
                             notifs.n.Notif_Application_Type_ID,
                             notifs.n.Notif_Application_Maker_ID,
                             CreatorName = creator.User_LName + ", " + creator.User_FName,
                             notifs.n.Notif_UserFor_ID,
                             notifs.n.Notif_Message,
                             notifs.n.Notif_Date,
                             stat.Status_ID,
                             stat.Status_Name
                         }).ToList();


            //assign values
            List<HomeNotifViewModel> vmList = new List<HomeNotifViewModel>();
            foreach (var m in mList)
            {
                HomeNotifViewModel vm = new HomeNotifViewModel
                {
                    Notif_ID = m.Notif_ID,
                    Notif_Application_Type_ID = m.Notif_Application_Type_ID,
                    Notif_Application_Type_Name = NotificationMessageValues.TYTIONARY[m.Notif_Application_Type_ID],
                    Notif_Application_Maker_ID = m.Notif_Application_Maker_ID,
                    Notif_Application_Maker_Name = m.CreatorName,
                    Notif_UserFor_ID = m.Notif_UserFor_ID,
                    Notif_Message = m.Notif_Message,
                    Notif_Date = m.Notif_Date,
                    Notif_Status_ID = m.Status_ID,
                    Notif_Status_Name = m.Status_Name
                };
                vmList.Add(vm);
            }

            PropertyInfo[] properties = filters.NotifFil.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.NotifFil).ToString();
                DateTime dt;
                DateTime.TryParseExact(toStr, "yyyy-dd-MM h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                if (toStr != "" && toStr != "0")
                {
                    if (subStr == "Date")
                    {
                        if ((dt.ToShortDateString() != new DateTime().ToShortDateString()))
                        {
                            var filterDate = DateTime.Parse(toStr).ToShortDateString();
                            vmList = vmList.AsQueryable()
                               .Where(x => (x.Notif_Date.ToShortDateString() == filterDate))
                              .Select(e => e).ToList();
                        }
                    }
                    else if (subStr == "Application_Type_Select")
                    {
                        //there is no filter for this field
                    }
                    else // IF STRING VALUE
                    {
                        vmList = vmList.AsQueryable().Where("Notif_" + subStr + ".Contains(@0)", toStr)
                            .Select(e => e).ToList();
                    }
                }
            }

            return vmList;
        }
        //Insert new records in Notif
        public bool insertIntoNotif(int UserID, int TypeID, int StatusID, int Maker_UserID = 0)
        {
            string uName = getName(UserID);
            string strMessPers = "You " + NotificationMessageValues.action[StatusID] + NotificationMessageValues.commonstr + NotificationMessageValues.TYTIONARY[TypeID];
            string strMessGen = uName + " " + NotificationMessageValues.action[StatusID] + NotificationMessageValues.commonstr + NotificationMessageValues.TYTIONARY[TypeID];

            List<HomeNotifModel> model = new List<HomeNotifModel>()
            {
                new HomeNotifModel()
                {
                    Notif_Application_Type_ID = TypeID,
                    Notif_Application_Maker_ID = UserID,
                    Notif_UserFor_ID = UserID,
                    Notif_Message = strMessPers,
                    Notif_Date = DateTime.Now,
                    Notif_Status_ID = StatusID
                },
                new HomeNotifModel()
                {
                    Notif_Application_Type_ID = TypeID,
                    Notif_Application_Maker_ID = UserID,
                    Notif_UserFor_ID = Maker_UserID,
                    Notif_Message = strMessGen,
                    Notif_Date = DateTime.Now,
                    Notif_Status_ID = StatusID

                }
            };
            _context.HomeNotif.AddRange(model);
            _context.SaveChanges();
            return true;
        }
        //Insert new records in Notif for Data Maintenance
        public bool insertIntoNotifDM(int UserID, int TypeID, int StatusID, int Maker_UserID = 0)
        {
            string uName = getName(UserID);
            string strMessPers = "You " + NotificationMessageValues.action[StatusID] + NotificationMessageValues.commonstr + "Data Maintenance: " + NotificationMessageValues.DMTIONARY[TypeID];
            string strMessGen = uName + " " + NotificationMessageValues.action[StatusID] + NotificationMessageValues.commonstr + "Data Maintenance: " + NotificationMessageValues.DMTIONARY[TypeID];

            List<HomeNotifModel> model = new List<HomeNotifModel>()
            {
                new HomeNotifModel()
                {
                    Notif_Application_Type_ID = GlobalSystemValues.TYPE_DM,
                    Notif_Application_Maker_ID = UserID,
                    Notif_UserFor_ID = UserID,
                    Notif_Message = strMessPers,
                    Notif_Date = DateTime.Now,
                    Notif_Status_ID = StatusID
                },
                new HomeNotifModel()
                {
                    Notif_Application_Type_ID = GlobalSystemValues.TYPE_DM,
                    Notif_Application_Maker_ID = UserID,
                    Notif_UserFor_ID = Maker_UserID,
                    Notif_Message = strMessGen,
                    Notif_Date = DateTime.Now,
                    Notif_Status_ID = StatusID

                }
            };
            _context.HomeNotif.AddRange(model);
            _context.SaveChanges();
            return true;
        }
        //Pending
        public List<ApplicationsViewModel> getPending(int userID, FiltersViewModel filters)
        {
            var linktionary = new Dictionary<int, string>();
            string mkr = GlobalSystemValues.ROLE_MAKER;
            var vfr = GlobalSystemValues.ROLE_VERIFIER;
            var appr = GlobalSystemValues.ROLE_APPROVER;
            // New Linktionary for Expense Transactions
            linktionary = new Dictionary<int, string>
                    {
                        {0,"Data Maintenance" },
                        {GlobalSystemValues.TYPE_CV,"View_CV"},
                        {GlobalSystemValues.TYPE_DDV,"View_DDV"},
                        {GlobalSystemValues.TYPE_NC,"View_NC"},
                        {GlobalSystemValues.TYPE_PC,"View_PCV"},
                        {GlobalSystemValues.TYPE_SS,"View_SS"},
                    };
            List<ApplicationsViewModel> dbPending = new List<ApplicationsViewModel>();
            var userInfo = _context.User.Where(x => x.User_ID == userID).FirstOrDefault();
            if (userInfo != null)
            {
                switch (userInfo.User_Role)
                {
                    case "maker":
                        dbPending = (from exp in _context.ExpenseEntry
                                     join us in _context.User on exp.Expense_Creator_ID equals us.User_ID
                                     where (
                                         (exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING ||
                                         exp.Expense_Status == GlobalSystemValues.STATUS_REJECTED)
                                         && exp.Expense_Creator_ID == userID
                                     )
                                     select new ApplicationsViewModel
                                     {
                                         App_ID = exp.Expense_ID,
                                         App_Type = GlobalSystemValues.getApplicationType(exp.Expense_Type),
                                         App_Amount = exp.Expense_Debit_Total,
                                         App_Payee = exp.Expense_Payee + "," + exp.Expense_Payee_Type,
                                         App_Maker = exp.Expense_Creator_ID + "",
                                         App_Verifier_ID_List = new List<string> { exp.Expense_Verifier_1 == 0 ? null : exp.Expense_Verifier_1 + "", exp.Expense_Verifier_2 == 0 ? null : exp.Expense_Verifier_2 + "" },
                                         App_Date = exp.Expense_Date,
                                         App_Last_Updated = exp.Expense_Last_Updated,
                                         App_Status = exp.Expense_Status + "",
                                         App_Link = linktionary[exp.Expense_Type]
                                     }).ToList();
                        dbPending = dbPending.Concat((from liq in _context.LiquidationEntryDetails
                                                      join us in _context.User on liq.Liq_Created_UserID equals us.User_ID
                                                      join exp in _context.ExpenseEntry on liq.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                                      where (
                                                          (liq.Liq_Status == GlobalSystemValues.STATUS_FOR_PRINTING ||
                                                          liq.Liq_Status == GlobalSystemValues.STATUS_REJECTED)
                                                          && liq.Liq_Created_UserID == userID
                                                      )
                                                      select new ApplicationsViewModel
                                                      {
                                                          App_ID = liq.ExpenseEntryModel.Expense_ID,
                                                          App_Type = "Liquidation",
                                                          App_Amount = exp.Expense_Debit_Total,
                                                          App_Payee = "",
                                                          App_Maker = liq.Liq_Created_UserID + "",
                                                          App_Verifier_ID_List = new List<string> { liq.Liq_Verifier1 == 0 ? null : liq.Liq_Verifier1 + "", liq.Liq_Verifier2 == 0 ? null : liq.Liq_Verifier2 + "" },
                                                          App_Date = liq.Liq_Created_Date,
                                                          App_Last_Updated = liq.Liq_LastUpdated_Date,
                                                          App_Status = liq.Liq_Status + "",
                                                          App_Link = "View_Liquidation_SS"
                                                      })).ToList();
                        break;
                    case "verifier":
                        dbPending = (from exp in _context.ExpenseEntry
                                     join us in _context.User on exp.Expense_Creator_ID equals us.User_ID
                                     where (
                                             (exp.Expense_Status == GlobalSystemValues.STATUS_PENDING
                                             && exp.Expense_Creator_ID != userID
                                             && us.User_DeptID == userInfo.User_DeptID)
                                         ||
                                             (exp.Expense_Status == GlobalSystemValues.STATUS_VERIFIED
                                             && exp.Expense_Creator_ID != userID
                                             && exp.Expense_Verifier_1 != userID
                                             && exp.Expense_Verifier_2 != userID
                                             && (exp.Expense_Verifier_1 == 0 || exp.Expense_Verifier_2 == 0)
                                             && us.User_DeptID == userInfo.User_DeptID)
                                         ||
                                            ((exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING) &&
                                            (exp.Expense_Creator_ID == userID
                                            || exp.Expense_Verifier_1 == userID
                                            || exp.Expense_Verifier_2 == userID
                                            || exp.Expense_Approver == userID))
                                         ||
                                            (exp.Expense_Status == GlobalSystemValues.STATUS_REJECTED
                                             && exp.Expense_Creator_ID == userID)
                                         )
                                     select new ApplicationsViewModel
                                     {
                                         App_ID = exp.Expense_ID,
                                         App_Type = GlobalSystemValues.getApplicationType(exp.Expense_Type),
                                         App_Amount = exp.Expense_Debit_Total,
                                         App_Payee = exp.Expense_Payee + "," + exp.Expense_Payee_Type,
                                         App_Maker = exp.Expense_Creator_ID + "",
                                         App_Verifier_ID_List = new List<string> { exp.Expense_Verifier_1 == 0 ? null : exp.Expense_Verifier_1 + "", exp.Expense_Verifier_2 == 0 ? null : exp.Expense_Verifier_2 + "" },
                                         App_Date = exp.Expense_Date,
                                         App_Last_Updated = exp.Expense_Last_Updated,
                                         App_Status = exp.Expense_Status + "",
                                         App_Link = linktionary[exp.Expense_Type]
                                     }).ToList();
                        dbPending = dbPending.Concat((from liq in _context.LiquidationEntryDetails
                                                      join us in _context.User on liq.Liq_Created_UserID equals us.User_ID
                                                      join exp in _context.ExpenseEntry on liq.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                                      where (
                                                         (liq.Liq_Status == GlobalSystemValues.STATUS_PENDING
                                                         && liq.Liq_Created_UserID != userID
                                                         && us.User_DeptID == userInfo.User_DeptID)
                                                     ||
                                                         (liq.Liq_Status == GlobalSystemValues.STATUS_VERIFIED
                                                         && liq.Liq_Created_UserID != userID
                                                         && liq.Liq_Verifier1 != userID
                                                         && liq.Liq_Verifier2 != userID
                                                         && (liq.Liq_Verifier1 == 0 || liq.Liq_Verifier2 == 0)
                                                         && us.User_DeptID == userInfo.User_DeptID)
                                                     ||
                                                        ((liq.Liq_Status == GlobalSystemValues.STATUS_FOR_PRINTING) &&
                                                        (liq.Liq_Created_UserID == userID
                                                        || liq.Liq_Verifier1 == userID
                                                        || liq.Liq_Verifier2 == userID
                                                        || liq.Liq_Approver == userID))
                                                     ||
                                                        (liq.Liq_Status == GlobalSystemValues.STATUS_REJECTED
                                                         && liq.Liq_Created_UserID == userID)
                                                     )
                                                      select new ApplicationsViewModel
                                                      {
                                                          App_ID = liq.ExpenseEntryModel.Expense_ID,
                                                          App_Type = "Liquidation",
                                                          App_Amount = exp.Expense_Debit_Total,
                                                          App_Payee = "",
                                                          App_Maker = liq.Liq_Created_UserID + "",
                                                          App_Verifier_ID_List = new List<string> { liq.Liq_Verifier1 == 0 ? null : liq.Liq_Verifier1 + "", liq.Liq_Verifier2 == 0 ? null : liq.Liq_Verifier2 + "" },
                                                          App_Date = liq.Liq_Created_Date,
                                                          App_Last_Updated = liq.Liq_LastUpdated_Date,
                                                          App_Status = liq.Liq_Status + "",
                                                          App_Link = "View_Liquidation_SS"
                                                      })).ToList();
                        break;
                    case "approver":
                        dbPending = (from exp in _context.ExpenseEntry
                                     join us in _context.User on exp.Expense_Creator_ID equals us.User_ID
                                     where (
                                             (exp.Expense_Status == GlobalSystemValues.STATUS_PENDING
                                             && exp.Expense_Creator_ID != userID
                                             && us.User_DeptID == userInfo.User_DeptID)
                                         ||
                                             (exp.Expense_Status == GlobalSystemValues.STATUS_VERIFIED
                                             && exp.Expense_Creator_ID != userID
                                             && exp.Expense_Verifier_1 != userID
                                             && exp.Expense_Verifier_2 != userID
                                             && us.User_DeptID == userInfo.User_DeptID)
                                         ||
                                            ((exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING) &&
                                            (exp.Expense_Creator_ID == userID
                                            || exp.Expense_Verifier_1 == userID
                                            || exp.Expense_Verifier_2 == userID
                                            || exp.Expense_Approver == userID))
                                         ||
                                            (exp.Expense_Status == GlobalSystemValues.STATUS_REJECTED
                                             && exp.Expense_Creator_ID == userID))
                                     select new ApplicationsViewModel
                                     {
                                         App_ID = exp.Expense_ID,
                                         App_Type = GlobalSystemValues.getApplicationType(exp.Expense_Type),
                                         App_Amount = exp.Expense_Debit_Total,
                                         App_Payee = exp.Expense_Payee + "," + exp.Expense_Payee_Type,
                                         App_Maker = exp.Expense_Creator_ID + "",
                                         App_Verifier_ID_List = new List<string> { exp.Expense_Verifier_1 == 0 ? null : exp.Expense_Verifier_1 + "", exp.Expense_Verifier_2 == 0 ? null : exp.Expense_Verifier_2 + "" },
                                         App_Date = exp.Expense_Date,
                                         App_Last_Updated = exp.Expense_Last_Updated,
                                         App_Status = exp.Expense_Status + "",
                                         App_Link = linktionary[exp.Expense_Type]
                                     }).ToList();
                        dbPending = dbPending.Concat((from liq in _context.LiquidationEntryDetails
                                                      join us in _context.User on liq.Liq_Created_UserID equals us.User_ID
                                                      join exp in _context.ExpenseEntry on liq.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                                      where (
                                                         (liq.Liq_Status == GlobalSystemValues.STATUS_PENDING
                                                         && liq.Liq_Created_UserID != userID
                                                         && us.User_DeptID == userInfo.User_DeptID)
                                                     ||
                                                         (liq.Liq_Status == GlobalSystemValues.STATUS_VERIFIED
                                                         && liq.Liq_Created_UserID != userID
                                                         && liq.Liq_Verifier1 != userID
                                                         && liq.Liq_Verifier2 != userID
                                                         && us.User_DeptID == userInfo.User_DeptID)
                                                     ||
                                                        ((liq.Liq_Status == GlobalSystemValues.STATUS_FOR_PRINTING) &&
                                                        (liq.Liq_Created_UserID == userID
                                                        || liq.Liq_Verifier1 == userID
                                                        || liq.Liq_Verifier2 == userID
                                                        || liq.Liq_Approver == userID))
                                                     ||
                                                        (liq.Liq_Status == GlobalSystemValues.STATUS_REJECTED
                                                         && liq.Liq_Created_UserID == userID)
                                                     )
                                                      select new ApplicationsViewModel
                                                      {
                                                          App_ID = liq.ExpenseEntryModel.Expense_ID,
                                                          App_Type = "Liquidation",
                                                          App_Amount = exp.Expense_Debit_Total,
                                                          App_Payee = "",
                                                          App_Maker = liq.Liq_Created_UserID + "",
                                                          App_Verifier_ID_List = new List<string> { liq.Liq_Verifier1 == 0 ? null : liq.Liq_Verifier1 + "", liq.Liq_Verifier2 == 0 ? null : liq.Liq_Verifier2 + "" },
                                                          App_Date = liq.Liq_Created_Date,
                                                          App_Last_Updated = liq.Liq_LastUpdated_Date,
                                                          App_Status = liq.Liq_Status + "",
                                                          App_Link = "View_Liquidation_SS"
                                                      })).ToList();
                        break;
                    default:
                        break;
                }
            }
            //Get Name of Maker, Verifier and Status.
            dbPending.ForEach(pen =>
            {
                if (pen.App_Payee.Length > 0)
                {
                    var split = pen.App_Payee.Split(",");
                    pen.App_Payee = getVendorName(int.Parse(split[0]), int.Parse(split[1])) ?? "";
                }
                pen.App_Maker = getName(int.Parse(pen.App_Maker));
                pen.App_Verifier_ID_List[0] = pen.App_Verifier_ID_List[0] != null ? getName(int.Parse(pen.App_Verifier_ID_List[0])) : null;
                pen.App_Verifier_ID_List[1] = pen.App_Verifier_ID_List[1] != null ? getName(int.Parse(pen.App_Verifier_ID_List[1])) : null;
                pen.App_Status = getStatus(int.Parse(pen.App_Status));
            });

            //FILTER
            var properties = filters.GenPendFil.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.GenPendFil).ToString();
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Created_Date" || subStr == "Updated_Date")
                        {
                            if (toStr != new DateTime().ToString())
                            {
                                if (subStr == "Created_Date")
                                {
                                    var filterDate = DateTime.Parse(toStr).ToShortDateString();
                                    dbPending = dbPending.Where(x => (x.App_Date.ToShortDateString() == filterDate))
                                                .Select(e => e).ToList();
                                }
                                else
                                {
                                    var filterDate = DateTime.Parse(toStr).ToShortDateString();
                                    dbPending = dbPending.Where(x => (x.App_Last_Updated.ToShortDateString() == filterDate))
                                                .Select(e => e).ToList();
                                }
                            }
                        }
                        else if (subStr == "Type_Select")
                        {

                        }
                        else if (subStr == "Amount")
                        {
                            dbPending = dbPending.Where(x => (x.App_Amount.ToString().Contains(toStr)))
                                                .Select(e => e).ToList();
                        }
                        else // IF STRING VALUE
                        {
                            dbPending = dbPending.AsQueryable().Where("App_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                                    .Select(e => e).ToList();
                        }
                    }
                }
            }
            return dbPending;
        }

        //History
        public PaginatedList<AppHistoryViewModel> getHistory(int userID, FiltersViewModel filters)
        {
            List<AppHistoryViewModel> historyList = new List<AppHistoryViewModel>();
            var linktionary = new Dictionary<int, string>
            {
                {0,"Data Maintenance" },
                {GlobalSystemValues.TYPE_CV,"View_CV"},
                {GlobalSystemValues.TYPE_DDV,"View_DDV"},
                {GlobalSystemValues.TYPE_NC,"View_NC"},
                {GlobalSystemValues.TYPE_PC,"View_PCV"},
                {GlobalSystemValues.TYPE_SS,"View_SS"},
            };
            var properties = filters.HistoryFil.GetType().GetProperties();
            var userInfo = _context.User.Where(x => x.User_ID == userID).FirstOrDefault();
            if (userInfo != null)
            {
                historyList = (from exp in _context.ExpenseEntry
                               join us in _context.User on exp.Expense_Creator_ID equals us.User_ID
                               where ((exp.Expense_Creator_ID == userID
                                       || exp.Expense_Verifier_1 == userID
                                       || exp.Expense_Verifier_2 == userID
                                       || exp.Expense_Approver == userID)
                                       && us.User_DeptID == userInfo.User_DeptID)
                               select new AppHistoryViewModel
                               {
                                   App_Entry_ID = exp.Expense_ID,
                                   App_Voucher_No = GlobalSystemValues.getApplicationCode(exp.Expense_Type) + "-" + GetSelectedYearMonthOfTerm(exp.Expense_Date.Month, exp.Expense_Date.Year).Year + "-" + exp.Expense_Number.ToString().PadLeft(5, '0'),
                                   App_Maker_ID = exp.Expense_Creator_ID,
                                   App_Approver_ID = exp.Expense_Approver,
                                   App_Verifier_Name_List = new List<string> { exp.Expense_Verifier_1 == 0 ? null : exp.Expense_Verifier_1 + "", exp.Expense_Verifier_2 == 0 ? null : exp.Expense_Verifier_2 + "" },
                                   App_Date = exp.Expense_Date,
                                   App_Last_Updated = exp.Expense_Last_Updated,
                                   App_Status = exp.Expense_Status + "",
                                   App_Link = linktionary[exp.Expense_Type]
                               }).ToList();
                historyList = historyList.Concat((from liq in _context.LiquidationEntryDetails
                                                  join us in _context.User on liq.Liq_Created_UserID equals us.User_ID
                                                  join exp in _context.ExpenseEntry on liq.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                                  where ((liq.Liq_Created_UserID == userID
                                                      || liq.Liq_Verifier1 == userID
                                                      || liq.Liq_Verifier2 == userID
                                                      || liq.Liq_Approver == userID)
                                                      && us.User_DeptID == userInfo.User_DeptID)
                                                  select new AppHistoryViewModel
                                                  {
                                                      App_Entry_ID = exp.Expense_ID,
                                                      //AppCode 6 == LIQ
                                                      App_Voucher_No = GlobalSystemValues.getApplicationCode(6) + "-" + GetSelectedYearMonthOfTerm(liq.Liq_Created_Date.Month, liq.Liq_Created_Date.Year).Year + "-" + exp.Expense_Number.ToString().PadLeft(5, '0'),
                                                      App_Maker_ID = liq.Liq_Created_UserID,
                                                      App_Approver_ID = liq.Liq_Approver,
                                                      App_Verifier_Name_List = new List<string> { liq.Liq_Verifier1 == 0 ? null : liq.Liq_Verifier1 + "", liq.Liq_Verifier2 == 0 ? null : liq.Liq_Verifier2 + "" },
                                                      App_Date = liq.Liq_Created_Date.Date,
                                                      App_Last_Updated = liq.Liq_LastUpdated_Date,
                                                      App_Status = liq.Liq_Status + "",
                                                      App_Link = "View_Liquidation_SS"
                                                  })).ToList();
            }
            //Get Name of Maker, Verifier, Approver and Status.
            historyList.ForEach(hist =>
            {
                hist.App_Maker_Name = getName(hist.App_Maker_ID);
                hist.App_Verifier_Name_List[0] = hist.App_Verifier_Name_List[0] != null ? getName(int.Parse(hist.App_Verifier_Name_List[0])) : null;
                hist.App_Verifier_Name_List[1] = hist.App_Verifier_Name_List[1] != null ? getName(int.Parse(hist.App_Verifier_Name_List[1])) : null;
                hist.App_Approver_Name = hist.App_Approver_ID != 0 ? getName(hist.App_Approver_ID) : null;
                hist.App_Status = getStatus(int.Parse(hist.App_Status));
            });
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                if (propertyName != "Hist_YearList" && propertyName != "Hist_Type_Select")
                {
                    var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                    var toStr = property.GetValue(filters.HistoryFil).ToString();
                    if (toStr != "")
                    {
                        if (toStr != "0")
                        {
                            if (subStr == "Maker" || subStr == "Approver" || subStr == "Status")
                            {
                                if (subStr == "Approver")
                                {
                                    historyList = historyList.Where(x => x.App_Approver_Name != null)
                                             .Select(e => e).ToList();
                                    historyList = historyList.Where(x => x.App_Approver_Name.Contains(toStr))
                                             .Select(e => e).ToList();
                                }
                                else if (subStr == "Maker")
                                {
                                    historyList = historyList.Where(x => x.App_Maker_Name.Contains(toStr))
                                             .Select(e => e).ToList();
                                }
                                else if (subStr == "Status")
                                {
                                    historyList = historyList.Where(x => x.App_Status.Contains(toStr))
                                             .Select(e => e).ToList();
                                }
                            }
                            else if (subStr == "Created_Date" || subStr == "Updated_Date")
                            {
                                if (toStr != new DateTime().ToString())
                                {
                                    if (subStr == "Created_Date")
                                    {
                                        var filterDate = DateTime.Parse(toStr).ToShortDateString();
                                        historyList = historyList.Where(x => (x.App_Date.ToShortDateString() == filterDate))
                                                 .Select(e => e).ToList();
                                    }
                                    else
                                    {
                                        var filterDate = DateTime.Parse(toStr).ToShortDateString();
                                        historyList = historyList.Where(x => (x.App_Last_Updated.ToShortDateString() == filterDate))
                                                 .Select(e => e).ToList();
                                    }
                                }
                            }
                            else if (subStr == "Voucher_Type" || subStr == "Voucher_Year" || subStr == "Voucher_No")
                            {
                                historyList = historyList.Where(x => x.App_Voucher_No.Contains(toStr))
                                        .Select(e => e).ToList();
                            }
                            else // IF STRING VALUE
                            {
                                historyList = historyList.AsQueryable().Where("App_" + subStr + ".Contains(@0)", toStr)
                                        .Select(e => e).ToList();
                            }
                        }
                    }
                }
            }
            PaginatedList<AppHistoryViewModel> pgHistoryList = new PaginatedList<AppHistoryViewModel>(historyList, historyList.Count, 1, 10);

            return pgHistoryList;
        }
        //[ User Maintenance ]
        public UserManagementViewModel2 populateUM()
        {
            List<UserViewModel> vmList = new List<UserViewModel>();
            //get all accounts
            var accs = (from a in _context.User
                        join d in _context.DMDept on a.User_DeptID equals d.Dept_ID
                        select new
                        {
                            a.User_ID,
                            a.User_EmpCode,
                            a.User_UserName,
                            a.User_FName,
                            a.User_LName,
                            d.Dept_Name,
                            a.User_DeptID,
                            a.User_Email,
                            a.User_Role,
                            a.User_Comment,
                            a.User_InUse,
                            a.User_Creator_ID,
                            a.User_Created_Date,
                            a.User_Approver_ID,
                            a.User_Last_Updated,
                            a.User_Status
                        }).ToList();
            //get account creator/approver IDs and dates, not all accounts have this
            var creatr = (from a in accs
                          join c in _context.User on a.User_Creator_ID equals c.User_ID
                          let CreatorName = c.User_LName + ", " + c.User_FName
                          select new
                          { a.User_ID, CreatorName }).ToList();
            var apprv = (from a in accs
                         join c in _context.User on a.User_Approver_ID equals c.User_ID
                         let ApproverName = c.User_LName + ", " + c.User_FName
                         select new
                         { a.User_ID, ApproverName }).ToList();

            accs.ForEach(x => {
                var creator = creatr.Where(a => a.User_ID == x.User_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprv.Where(a => a.User_ID == x.User_ID).Select(a => a.ApproverName).FirstOrDefault();
                UserViewModel vm = new UserViewModel
                {
                    User_ID = x.User_ID,
                    User_UserName = x.User_UserName,
                    User_EmpCode = x.User_EmpCode,
                    User_FName = x.User_FName,
                    User_LName = x.User_LName,
                    User_Dept_ID = x.User_DeptID,
                    User_Dept_Name = x.Dept_Name,
                    User_Email = x.User_Email,
                    User_Role = x.User_Role,
                    User_InUse = x.User_InUse,
                    User_Comment = x.User_Comment,
                    User_Creator_Name = creator ?? "N/A",
                    User_Approver_Name = approver ?? "",
                    User_Created_Date = x.User_Created_Date,
                    User_Last_Updated = x.User_Last_Updated,
                    User_Status = x.User_Status
                };
                vmList.Add(vm);
            });
            List<DMDeptViewModel> deptList = new List<DMDeptViewModel>();

            DMDeptViewModel optionLbl = new DMDeptViewModel
            {
                Dept_ID = 0,
                Dept_Name = "--Select Department--",
                Dept_Code = "0000"
            };
            deptList.Add(optionLbl);

            _context.DMDept.Where(x => x.Dept_isDeleted == false).ToList().ForEach(x => {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = x.Dept_ID,
                    Dept_Name = x.Dept_Name,
                    Dept_Code = x.Dept_Code
                };
                deptList.Add(vm);
            });

            UserManagementViewModel2 mod = new UserManagementViewModel2
            {
                NewAcc = new User2ViewModel(),
                AccList = vmList,
                DeptList = deptList
            };
            return mod;
        }
        //Add / Edit User
        public bool addUser(UserManagementViewModel model, string userId)
        {
            UserModel mod = _context.User.Where(x => model.NewAcc.User_ID == x.User_ID).FirstOrDefault();
            if (mod == null)
            {
                UserModel duplicate = _context.User.Where(x => x.User_UserName == model.NewAcc.User_UserName).FirstOrDefault();

                if (duplicate != null)
                    return false;

                mod = new UserModel
                {
                    User_UserName = model.NewAcc.User_UserName,
                    User_EmpCode = model.NewAcc.User_EmpCode,
                    User_FName = model.NewAcc.User_FName,
                    User_LName = model.NewAcc.User_LName,
                    User_DeptID = model.NewAcc.User_DeptID,
                    User_Email = model.NewAcc.User_Email,
                    User_Role = model.NewAcc.User_Role,
                    User_Password = (CryptoTools.getHashPasswd("PLACEHOLDER", model.NewAcc.User_UserName, model.NewAcc.User_Password ?? defaultPW)),
                    User_Comment = model.NewAcc.User_Comment,
                    User_InUse = model.NewAcc.User_InUse,
                    User_Creator_ID = int.Parse(userId),
                    User_Created_Date = DateTime.Now,
                    User_Status = "Is Created"
                };
                if (_modelState.IsValid)
                {
                    _context.User.Add(mod);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (model.NewAcc.User_ID == mod.User_ID)
                {
                    mod.User_EmpCode = model.NewAcc.User_EmpCode;
                    mod.User_FName = model.NewAcc.User_FName;
                    mod.User_LName = model.NewAcc.User_LName;
                    mod.User_DeptID = model.NewAcc.User_DeptID;
                    mod.User_Email = model.NewAcc.User_Email;
                    mod.User_Role = model.NewAcc.User_Role;
                    mod.User_Password = model.NewAcc.User_Password != null ? (CryptoTools.getHashPasswd("PLACEHOLDER", mod.User_UserName, model.NewAcc.User_Password)) : mod.User_Password;
                    mod.User_Comment = model.NewAcc.User_Comment;
                    mod.User_InUse = model.NewAcc.User_InUse;
                    mod.User_Approver_ID = int.Parse(userId);
                    mod.User_Last_Updated = DateTime.Now;
                    mod.User_Status = "Is Updated";
                    if (_modelState.IsValid)
                    {
                        _context.SaveChanges();
                    }
                }
            }
            return true;
        }

        //---------------------------DM - ADMIN---------------------------
        //[ PAYEE ]
        public bool approveVendor(List<DMVendorViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Vendor_MasterID).ToList();

            var allPending = (from pp in _context.DMVendor_Pending
                              from pm in _context.DMVendor.Where(x => x.Vendor_MasterID == pp.Pending_Vendor_MasterID
                              && x.Vendor_isActive == true
                              && x.Vendor_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_Vendor_MasterID,
                                  pp.Pending_Vendor_Name,
                                  pp.Pending_Vendor_TIN,
                                  pp.Pending_Vendor_Address,
                                  pp.Pending_Vendor_IsDeleted,
                                  pp.Pending_Vendor_Creator_ID,
                                  pmCreatorID = pm.Vendor_Creator_ID.ToString(),
                                  pmCreateDate = pm.Vendor_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_Vendor_MasterID)).Distinct().ToList();
            var allVTVPending = (from pp in _context.DMVendorTRVAT_Pending
                                 from pm in _context.DMVendorTRVAT.Where(x => x.VTV_ID == pp.Pending_VTV_ID).DefaultIfEmpty()
                                 select new
                                 {
                                     pp.Pending_VTV_ID,
                                     pp.Pending_VTV_Vendor_ID,
                                     pp.Pending_VTV_TR_ID,
                                     pp.Pending_VTV_VAT_ID
                                 }).Where(x => intList.Contains(x.Pending_VTV_Vendor_ID)).ToList();

            List<DMVendorModel_Pending> toDelete = _context.DMVendor_Pending.Where(x => intList.Contains(x.Pending_Vendor_MasterID)).ToList();
            List<DMVendorTRVATModel_Pending> toVTVDelete = _context.DMVendorTRVAT_Pending.Where(x => intList.Contains(x.Pending_VTV_Vendor_ID)).ToList();

            //get all records that currently exists in Master Data
            List<DMVendorModel> vmList = _context.DMVendor.
                Where(x => intList.Contains(x.Vendor_MasterID) && x.Vendor_isActive == true).ToList();
            List<DMVendorTRVATModel> vmVTVList = _context.DMVendorTRVAT.
                Where(x => intList.Contains(x.VTV_Vendor_ID)).ToList();

            //list for formatted records to be added
            List<DMVendorModel> addList = new List<DMVendorModel>();
            List<DMVendorTRVATModel> addVTVList = new List<DMVendorTRVATModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMVendorModel m = new DMVendorModel
                {
                    Vendor_Name = pending.Pending_Vendor_Name,
                    Vendor_MasterID = pending.Pending_Vendor_MasterID,
                    Vendor_TIN = pending.Pending_Vendor_TIN,
                    Vendor_Address = pending.Pending_Vendor_Address,
                    Vendor_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Vendor_Creator_ID : int.Parse(pending.pmCreatorID),
                    Vendor_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Vendor_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Vendor_Last_Updated = DateTime.Now,
                    Vendor_Status_ID = 3,
                    Vendor_isDeleted = pending.Pending_Vendor_IsDeleted,
                    Vendor_isActive = true
                };
                addList.Add(m);
            });
            allVTVPending.ForEach(pending =>
            {
                DMVendorTRVATModel m = new DMVendorTRVATModel
                {
                    VTV_TR_ID = pending.Pending_VTV_TR_ID,
                    VTV_VAT_ID = pending.Pending_VTV_VAT_ID,
                    VTV_Vendor_ID = pending.Pending_VTV_Vendor_ID
                };
                addVTVList.Add(m);
            });
            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Vendor_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMVendor.AddRange(addList);
                _context.DMVendor_Pending.RemoveRange(toDelete);
                _context.DMVendorTRVAT.RemoveRange(vmVTVList);
                _context.DMVendorTRVAT.AddRange(addVTVList);
                _context.DMVendorTRVAT_Pending.RemoveRange(toVTVDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejVendor(List<DMVendorViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Vendor_MasterID).ToList();
            List<DMVendorModel_Pending> allPending = _context.DMVendor_Pending.Where(x => intList.Contains(x.Pending_Vendor_MasterID)).ToList();
            List<DMVendorTRVATModel_Pending> allPendingTRVAT = _context.DMVendorTRVAT_Pending.Where(x => intList.Contains(x.Pending_VTV_Vendor_ID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.RemoveRange(allPending);
                _context.DMVendorTRVAT_Pending.RemoveRange(allPendingTRVAT);
                _context.SaveChanges();
            }
            return true;
        }
        //[ DEPARTMENT ]
        public bool approveDept(List<DMDeptViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Dept_MasterID).ToList();

            var allPending = ((from pp in _context.DMDept_Pending
                               from pm in _context.DMDept.Where(x => x.Dept_MasterID == pp.Pending_Dept_MasterID
                              && x.Dept_isActive == true
                              && x.Dept_isDeleted == false).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Dept_MasterID,
                                   pp.Pending_Dept_Name,
                                   pp.Pending_Dept_Code,
                                   pp.Pending_Dept_Budget_Unit,
                                   pp.Pending_Dept_isDeleted,
                                   pp.Pending_Dept_Creator_ID,
                                   pmCreatorID = pm.Dept_Creator_ID.ToString(),
                                   pmCreateDate = pm.Dept_Created_Date.ToString(),
                                   pp.Pending_Dept_isActive
                               })).Where(x => intList.Contains(x.Pending_Dept_MasterID)).Distinct().ToList();

            List<DMDeptModel_Pending> toDelete = _context.DMDept_Pending.Where(x => intList.Contains(x.Pending_Dept_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMDeptModel> vmList = _context.DMDept.
                Where(x => intList.Contains(x.Dept_MasterID) && x.Dept_isActive == true).ToList();

            //list for formatted records to be added
            List<DMDeptModel> addList = new List<DMDeptModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMDeptModel m = new DMDeptModel
                {
                    Dept_Name = pending.Pending_Dept_Name,
                    Dept_MasterID = pending.Pending_Dept_MasterID,
                    Dept_Code = pending.Pending_Dept_Code,
                    Dept_Budget_Unit = pending.Pending_Dept_Budget_Unit,
                    Dept_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Dept_Creator_ID : int.Parse(pending.pmCreatorID),
                    Dept_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Dept_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Dept_Last_Updated = DateTime.Now,
                    Dept_Status_ID = 3,
                    Dept_isDeleted = pending.Pending_Dept_isDeleted,
                    Dept_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Dept_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMDept.AddRange(addList);
                _context.DMDept_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejDept(List<DMDeptViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Dept_MasterID).ToList();
            List<DMDeptModel_Pending> allPending = _context.DMDept_Pending.Where(x => intList.Contains(x.Pending_Dept_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ CHECK ]
        public bool approveCheck(List<DMCheckViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Check_MasterID).ToList();

            var allPending = ((from pp in _context.DMCheck_Pending
                               from pm in _context.DMCheck.Where(x => x.Check_MasterID == pp.Pending_Check_MasterID
                              && x.Check_isActive == true
                              && x.Check_isDeleted == false).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Check_MasterID,
                                   pp.Pending_Check_Input_Date,
                                   pp.Pending_Check_Series_From,
                                   pp.Pending_Check_Series_To,
                                   pp.Pending_Check_Bank_Info,
                                   pp.Pending_Check_isDeleted,
                                   pp.Pending_Check_Creator_ID,
                                   pmCreatorID = pm.Check_Creator_ID.ToString(),
                                   pmCreateDate = pm.Check_Created_Date.ToString(),
                                   pp.Pending_Check_isActive
                               })).Where(x => intList.Contains(x.Pending_Check_MasterID)).Distinct().ToList();

            List<DMCheckModel_Pending> toDelete = _context.DMCheck_Pending.Where(x => intList.Contains(x.Pending_Check_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMCheckModel> vmList = _context.DMCheck.
                Where(x => intList.Contains(x.Check_MasterID) && x.Check_isActive == true).ToList();

            //list for formatted records to be added
            List<DMCheckModel> addList = new List<DMCheckModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMCheckModel m = new DMCheckModel
                {
                    Check_Input_Date = pending.Pending_Check_Input_Date,
                    Check_MasterID = pending.Pending_Check_MasterID,
                    Check_Bank_Info = pending.Pending_Check_Bank_Info,
                    Check_Series_From = pending.Pending_Check_Series_From,
                    Check_Series_To = pending.Pending_Check_Series_To,
                    Check_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Check_Creator_ID : int.Parse(pending.pmCreatorID),
                    Check_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Check_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Check_Last_Updated = DateTime.Now,
                    Check_Status_ID = 3,
                    Check_isDeleted = pending.Pending_Check_isDeleted,
                    Check_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Check_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMCheck.AddRange(addList);
                _context.DMCheck_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejCheck(List<DMCheckViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Check_MasterID).ToList();
            List<DMCheckModel_Pending> allPending = _context.DMCheck_Pending.Where(x => intList.Contains(x.Pending_Check_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ ACCOUNT ]
        public bool approveAccount(List<DMAccountViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Account_MasterID).ToList();

            var allPending = (from pp in _context.DMAccount_Pending
                              from pm in _context.DMAccount.Where(x => x.Account_MasterID == pp.Pending_Account_MasterID
                              && x.Account_isActive == true
                              && x.Account_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_Account_MasterID,
                                  pp.Pending_Account_Name,
                                  pp.Pending_Account_FBT_MasterID,
                                  pp.Pending_Account_Group_MasterID,
                                  pp.Pending_Account_Currency_MasterID,
                                  pp.Pending_Account_Code,
                                  pp.Pending_Account_Budget_Code,
                                  pp.Pending_Account_No,
                                  pp.Pending_Account_Cust,
                                  pp.Pending_Account_Div,
                                  pp.Pending_Account_Fund,
                                  pp.Pending_Account_isDeleted,
                                  pp.Pending_Account_Creator_ID,
                                  pmCreatorID = pm.Account_Creator_ID.ToString(),
                                  pmCreateDate = pm.Account_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_Account_MasterID)).ToList();

            List<DMAccountModel_Pending> toDelete = _context.DMAccount_Pending.Where(x => intList.Contains(x.Pending_Account_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMAccountModel> vmList = _context.DMAccount.
                Where(x => intList.Contains(x.Account_MasterID) && x.Account_isActive == true).ToList();

            //list for formatted records to be added
            List<DMAccountModel> addList = new List<DMAccountModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMAccountModel m = new DMAccountModel
                {
                    Account_Name = pending.Pending_Account_Name,
                    Account_MasterID = pending.Pending_Account_MasterID,
                    Account_FBT_MasterID = pending.Pending_Account_FBT_MasterID,
                    Account_Group_MasterID = pending.Pending_Account_Group_MasterID,
                    Account_Currency_MasterID = pending.Pending_Account_Currency_MasterID,
                    Account_Code = pending.Pending_Account_Code,
                    Account_Budget_Code = pending.Pending_Account_Budget_Code,
                    Account_Cust = pending.Pending_Account_Cust,
                    Account_Div = pending.Pending_Account_Div,
                    Account_Fund = pending.Pending_Account_Fund,
                    Account_No = pending.Pending_Account_No,
                    Account_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Account_Creator_ID : int.Parse(pending.pmCreatorID),
                    Account_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Account_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Account_Last_Updated = DateTime.Now,
                    Account_Status_ID = 3,
                    Account_isDeleted = pending.Pending_Account_isDeleted,
                    Account_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Account_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMAccount.AddRange(addList);
                _context.DMAccount_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejAccount(List<DMAccountViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Account_MasterID).ToList();
            List<DMAccountModel_Pending> allPending = _context.DMAccount_Pending.Where(x => intList.Contains(x.Pending_Account_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ ACCOUNT GROUP ]
        public bool approveAccountGroup(List<DMAccountGroupViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.AccountGroup_MasterID).ToList();

            var allPending = (from pp in _context.DMAccountGroup_Pending
                              from pm in _context.DMAccountGroup.Where(x => x.AccountGroup_MasterID == pp.Pending_AccountGroup_MasterID
                              && x.AccountGroup_isActive == true
                              && x.AccountGroup_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_AccountGroup_MasterID,
                                  pp.Pending_AccountGroup_Name,
                                  pp.Pending_AccountGroup_Code,
                                  pp.Pending_AccountGroup_isDeleted,
                                  pp.Pending_AccountGroup_Creator_ID,
                                  pmCreatorID = pm.AccountGroup_Creator_ID.ToString(),
                                  pmCreateDate = pm.AccountGroup_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_AccountGroup_MasterID)).ToList();

            List<DMAccountGroupModel_Pending> toDelete = _context.DMAccountGroup_Pending.Where(x => intList.Contains(x.Pending_AccountGroup_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMAccountGroupModel> vmList = _context.DMAccountGroup.
                Where(x => intList.Contains(x.AccountGroup_MasterID) && x.AccountGroup_isActive == true).ToList();

            //list for formatted records to be added
            List<DMAccountGroupModel> addList = new List<DMAccountGroupModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMAccountGroupModel m = new DMAccountGroupModel
                {
                    AccountGroup_Name = pending.Pending_AccountGroup_Name,
                    AccountGroup_MasterID = pending.Pending_AccountGroup_MasterID,
                    AccountGroup_Code = pending.Pending_AccountGroup_Code,
                    AccountGroup_Creator_ID = pending.pmCreatorID == null ? pending.Pending_AccountGroup_Creator_ID : int.Parse(pending.pmCreatorID),
                    AccountGroup_Approver_ID = int.Parse(_session.GetString("UserID")),
                    AccountGroup_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    AccountGroup_Last_Updated = DateTime.Now,
                    AccountGroup_Status_ID = 3,
                    AccountGroup_isDeleted = pending.Pending_AccountGroup_isDeleted,
                    AccountGroup_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.AccountGroup_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup.AddRange(addList);
                _context.DMAccountGroup_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejAccountGroup(List<DMAccountGroupViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.AccountGroup_MasterID).ToList();
            List<DMAccountGroupModel_Pending> allPending = _context.DMAccountGroup_Pending.Where(x => intList.Contains(x.Pending_AccountGroup_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ VAT ]
        public bool approveVAT(List<DMVATViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.VAT_MasterID).ToList();

            var allPending = (from pp in _context.DMVAT_Pending
                              from pm in _context.DMVAT.Where(x => x.VAT_MasterID == pp.Pending_VAT_MasterID
                              && x.VAT_isActive == true
                              && x.VAT_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_VAT_MasterID,
                                  pp.Pending_VAT_Name,
                                  pp.Pending_VAT_Rate,
                                  pp.Pending_VAT_isDeleted,
                                  pp.Pending_VAT_Creator_ID,
                                  pmCreatorID = pm.VAT_Creator_ID.ToString(),
                                  pmCreateDate = pm.VAT_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_VAT_MasterID)).ToList();

            List<DMVATModel_Pending> toDelete = _context.DMVAT_Pending.Where(x => intList.Contains(x.Pending_VAT_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMVATModel> vmList = _context.DMVAT.
                Where(x => intList.Contains(x.VAT_MasterID) && x.VAT_isActive == true).ToList();

            //list for formatted records to be added
            List<DMVATModel> addList = new List<DMVATModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMVATModel m = new DMVATModel
                {
                    VAT_Name = pending.Pending_VAT_Name,
                    VAT_MasterID = pending.Pending_VAT_MasterID,
                    VAT_Rate = pending.Pending_VAT_Rate,
                    VAT_Creator_ID = pending.pmCreatorID == null ? pending.Pending_VAT_Creator_ID : int.Parse(pending.pmCreatorID),
                    VAT_Approver_ID = int.Parse(_session.GetString("UserID")),
                    VAT_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    VAT_Last_Updated = DateTime.Now,
                    VAT_Status_ID = 3,
                    VAT_isDeleted = pending.Pending_VAT_isDeleted,
                    VAT_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.VAT_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMVAT.AddRange(addList);
                _context.DMVAT_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejVAT(List<DMVATViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.VAT_MasterID).ToList();
            List<DMVATModel_Pending> allPending = _context.DMVAT_Pending.Where(x => intList.Contains(x.Pending_VAT_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ FBT ]
        public bool approveFBT(List<DMFBTViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.FBT_MasterID).ToList();

            var allPending = (from pp in _context.DMFBT_Pending
                              from pm in _context.DMFBT.Where(x => x.FBT_MasterID == pp.Pending_FBT_MasterID
                              && x.FBT_isActive == true
                              && x.FBT_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_FBT_MasterID,
                                  pp.Pending_FBT_Name,
                                  pp.Pending_FBT_Formula,
                                  pp.Pending_FBT_Tax_Rate,
                                  pp.Pending_FBT_isDeleted,
                                  pp.Pending_FBT_Creator_ID,
                                  pmCreatorID = pm.FBT_Creator_ID.ToString(),
                                  pmCreateDate = pm.FBT_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_FBT_MasterID)).Distinct().ToList();

            List<DMFBTModel_Pending> toDelete = _context.DMFBT_Pending.Where(x => intList.Contains(x.Pending_FBT_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMFBTModel> vmList = _context.DMFBT.
                Where(x => intList.Contains(x.FBT_MasterID) && x.FBT_isActive == true).ToList();

            //list for formatted records to be added
            List<DMFBTModel> addList = new List<DMFBTModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMFBTModel m = new DMFBTModel
                {
                    FBT_Name = pending.Pending_FBT_Name,
                    FBT_MasterID = pending.Pending_FBT_MasterID,
                    FBT_Formula = pending.Pending_FBT_Formula,
                    FBT_Tax_Rate = pending.Pending_FBT_Tax_Rate,
                    FBT_Creator_ID = pending.pmCreatorID == null ? pending.Pending_FBT_Creator_ID : int.Parse(pending.pmCreatorID),
                    FBT_Approver_ID = int.Parse(_session.GetString("UserID")),
                    FBT_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    FBT_Last_Updated = DateTime.Now,
                    FBT_Status_ID = 3,
                    FBT_isDeleted = pending.Pending_FBT_isDeleted,
                    FBT_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.FBT_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMFBT.AddRange(addList);
                _context.DMFBT_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejFBT(List<DMFBTViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.FBT_MasterID).ToList();
            List<DMFBTModel_Pending> allPending = _context.DMFBT_Pending.Where(x => intList.Contains(x.Pending_FBT_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ TR ]
        public bool approveTR(List<DMTRViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.TR_MasterID).ToList();

            var allPending = (from pp in _context.DMTR_Pending
                              from pm in _context.DMTR.Where(x => x.TR_MasterID == pp.Pending_TR_MasterID
                              && x.TR_isActive == true
                              && x.TR_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_TR_WT_Title,
                                  pp.Pending_TR_MasterID,
                                  pp.Pending_TR_Nature,
                                  pp.Pending_TR_Tax_Rate,
                                  pp.Pending_TR_ATC,
                                  pp.Pending_TR_Nature_Income_Payment,
                                  pp.Pending_TR_isDeleted,
                                  pp.Pending_TR_Creator_ID,
                                  pmCreatorID = pm.TR_Creator_ID.ToString(),
                                  pmCreateDate = pm.TR_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_TR_MasterID)).Distinct().ToList();

            List<DMTRModel_Pending> toDelete = _context.DMTR_Pending.Where(x => intList.Contains(x.Pending_TR_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMTRModel> vmList = _context.DMTR.
                Where(x => intList.Contains(x.TR_MasterID) && x.TR_isActive == true).ToList();

            //list for formatted records to be added
            List<DMTRModel> addList = new List<DMTRModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMTRModel m = new DMTRModel
                {
                    TR_WT_Title = pending.Pending_TR_WT_Title,
                    TR_Nature = pending.Pending_TR_Nature,
                    TR_MasterID = pending.Pending_TR_MasterID,
                    TR_Tax_Rate = pending.Pending_TR_Tax_Rate,
                    TR_ATC = pending.Pending_TR_ATC,
                    TR_Nature_Income_Payment = pending.Pending_TR_Nature_Income_Payment,
                    TR_Creator_ID = pending.pmCreatorID == null ? pending.Pending_TR_Creator_ID : int.Parse(pending.pmCreatorID),
                    TR_Approver_ID = int.Parse(_session.GetString("UserID")),
                    TR_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    TR_Last_Updated = DateTime.Now,
                    TR_Status_ID = 3,
                    TR_isDeleted = pending.Pending_TR_isDeleted,
                    TR_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.TR_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMTR.AddRange(addList);
                _context.DMTR_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejTR(List<DMTRViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.TR_MasterID).ToList();
            List<DMTRModel_Pending> allPending = _context.DMTR_Pending.Where(x => intList.Contains(x.Pending_TR_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Curr ]
        public bool approveCurr(List<DMCurrencyViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Curr_MasterID).ToList();

            var allPending = (from pp in _context.DMCurrency_Pending
                              from pm in _context.DMCurrency.Where(x => x.Curr_MasterID == pp.Pending_Curr_MasterID
                              && x.Curr_isActive == true
                              && x.Curr_isDeleted == false).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_Curr_MasterID,
                                  pp.Pending_Curr_Name,
                                  pp.Pending_Curr_CCY_ABBR,
                                  pp.Pending_Curr_isDeleted,
                                  pp.Pending_Curr_Creator_ID,
                                  pmCreatorID = pm.Curr_Creator_ID.ToString(),
                                  pmCreateDate = pm.Curr_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_Curr_MasterID)).Distinct().ToList();

            List<DMCurrencyModel_Pending> toDelete = _context.DMCurrency_Pending.Where(x => intList.Contains(x.Pending_Curr_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMCurrencyModel> vmList = _context.DMCurrency.
                Where(x => intList.Contains(x.Curr_MasterID) && x.Curr_isActive == true).ToList();

            //list for formatted records to be added
            List<DMCurrencyModel> addList = new List<DMCurrencyModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMCurrencyModel m = new DMCurrencyModel
                {
                    Curr_Name = pending.Pending_Curr_Name,
                    Curr_MasterID = pending.Pending_Curr_MasterID,
                    Curr_CCY_ABBR = pending.Pending_Curr_CCY_ABBR,
                    Curr_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Curr_Creator_ID : int.Parse(pending.pmCreatorID),
                    Curr_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Curr_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Curr_Last_Updated = DateTime.Now,
                    Curr_Status_ID = 3,
                    Curr_isDeleted = pending.Pending_Curr_isDeleted,
                    Curr_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Curr_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMCurrency.AddRange(addList);
                _context.DMCurrency_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejCurr(List<DMCurrencyViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Curr_MasterID).ToList();
            List<DMCurrencyModel_Pending> allPending = _context.DMCurrency_Pending.Where(x => intList.Contains(x.Pending_Curr_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Employee ]
        public bool approveEmp(List<DMEmpViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Emp_MasterID).ToList();

            var allPending = ((from pp in _context.DMEmp_Pending
                               from pm in _context.DMEmp.Where(x => x.Emp_MasterID == pp.Pending_Emp_MasterID
                              && x.Emp_isActive == true
                              && x.Emp_isDeleted == false).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Emp_MasterID,
                                   pp.Pending_Emp_Name,
                                   pp.Pending_Emp_Acc_No,
                                   pp.Pending_Emp_Type,
                                   pp.Pending_Emp_Category_ID,
                                   pp.Pending_Emp_FBT_MasterID,
                                   pp.Pending_Emp_isDeleted,
                                   pp.Pending_Emp_Creator_ID,
                                   pmCreatorID = pm.Emp_Creator_ID.ToString(),
                                   pmCreateDate = pm.Emp_Created_Date.ToString(),
                                   pp.Pending_Emp_isActive
                               })).Where(x => intList.Contains(x.Pending_Emp_MasterID)).Distinct().ToList();

            List<DMEmpModel_Pending> toDelete = _context.DMEmp_Pending.Where(x => intList.Contains(x.Pending_Emp_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMEmpModel> vmList = _context.DMEmp.
                Where(x => intList.Contains(x.Emp_MasterID) && x.Emp_isActive == true).ToList();

            //list for formatted records to be added
            List<DMEmpModel> addList = new List<DMEmpModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMEmpModel m = new DMEmpModel
                {
                    Emp_Name = pending.Pending_Emp_Name,
                    Emp_MasterID = pending.Pending_Emp_MasterID,
                    Emp_Acc_No = pending.Pending_Emp_Acc_No,
                    Emp_FBT_MasterID = pending.Pending_Emp_FBT_MasterID,
                    Emp_Category_ID = pending.Pending_Emp_Category_ID,
                    Emp_Type = pending.Pending_Emp_Acc_No.Length <= 0 ? "Temporary" : "Regular",
                    Emp_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Emp_Creator_ID : int.Parse(pending.pmCreatorID),
                    Emp_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Emp_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Emp_Last_Updated = DateTime.Now,
                    Emp_Status_ID = 3,
                    Emp_isDeleted = pending.Pending_Emp_isDeleted,
                    Emp_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Emp_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMEmp.AddRange(addList);
                _context.DMEmp_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejEmp(List<DMEmpViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Emp_MasterID).ToList();
            List<DMEmpModel_Pending> allPending = _context.DMEmp_Pending.Where(x => intList.Contains(x.Pending_Emp_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Customer ]
        public bool approveCust(List<DMCustViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Cust_MasterID).ToList();

            var allPending = ((from pp in _context.DMCust_Pending
                               from pm in _context.DMCust.Where(x => x.Cust_MasterID == pp.Pending_Cust_MasterID
                              && x.Cust_isActive == true
                              && x.Cust_isDeleted == false).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Cust_MasterID,
                                   pp.Pending_Cust_Name,
                                   pp.Pending_Cust_Abbr,
                                   pp.Pending_Cust_No,
                                   pp.Pending_Cust_isDeleted,
                                   pp.Pending_Cust_Creator_ID,
                                   pmCreatorID = pm.Cust_Creator_ID.ToString(),
                                   pmCreateDate = pm.Cust_Created_Date.ToString(),
                                   pp.Pending_Cust_isActive
                               })).Where(x => intList.Contains(x.Pending_Cust_MasterID)).Distinct().ToList();

            List<DMCustModel_Pending> toDelete = _context.DMCust_Pending.Where(x => intList.Contains(x.Pending_Cust_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMCustModel> vmList = _context.DMCust.
                Where(x => intList.Contains(x.Cust_MasterID) && x.Cust_isActive == true).ToList();

            //list for formatted records to be added
            List<DMCustModel> addList = new List<DMCustModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMCustModel m = new DMCustModel
                {
                    Cust_Name = pending.Pending_Cust_Name,
                    Cust_MasterID = pending.Pending_Cust_MasterID,
                    Cust_Abbr = pending.Pending_Cust_Abbr,
                    Cust_No = pending.Pending_Cust_No,
                    Cust_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Cust_Creator_ID : int.Parse(pending.pmCreatorID),
                    Cust_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Cust_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Cust_Last_Updated = DateTime.Now,
                    Cust_Status_ID = 3,
                    Cust_isDeleted = pending.Pending_Cust_isDeleted,
                    Cust_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Cust_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMCust.AddRange(addList);
                _context.DMCust_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejCust(List<DMCustViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Cust_MasterID).ToList();
            List<DMCustModel_Pending> allPending = _context.DMCust_Pending.Where(x => intList.Contains(x.Pending_Cust_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ BIR Cert Signatory ]
        public bool approveBCS(List<DMBCSViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.BCS_MasterID).ToList();

            var allPending = ((from pp in _context.DMBCS_Pending
                               from pm in _context.DMBCS.Where(x => x.BCS_MasterID == pp.Pending_BCS_MasterID
                              && x.BCS_isActive == true
                              && x.BCS_isDeleted == false).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_BCS_MasterID,
                                   pp.Pending_BCS_User_ID,
                                   pp.Pending_BCS_TIN,
                                   pp.Pending_BCS_Position,
                                   pp.Pending_BCS_Signatures,
                                   pp.Pending_BCS_isDeleted,
                                   pp.Pending_BCS_Creator_ID,
                                   pmCreatorID = pm.BCS_Creator_ID.ToString(),
                                   pmCreateDate = pm.BCS_Created_Date.ToString(),
                                   pp.Pending_BCS_isActive
                               })).Where(x => intList.Contains(x.Pending_BCS_MasterID)).Distinct().ToList();

            List<DMBIRCertSignModel_Pending> toDelete = _context.DMBCS_Pending.Where(x => intList.Contains(x.Pending_BCS_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMBIRCertSignModel> vmList = _context.DMBCS.
                Where(x => intList.Contains(x.BCS_MasterID) && x.BCS_isActive == true).ToList();

            //list for formatted records to be added
            List<DMBIRCertSignModel> addList = new List<DMBIRCertSignModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMBIRCertSignModel m = new DMBIRCertSignModel
                {
                    BCS_User_ID = pending.Pending_BCS_User_ID,
                    BCS_MasterID = pending.Pending_BCS_MasterID,
                    BCS_TIN = pending.Pending_BCS_TIN,
                    BCS_Position = pending.Pending_BCS_Position,
                    BCS_Signatures = pending.Pending_BCS_Signatures,
                    BCS_Creator_ID = pending.pmCreatorID == null ? pending.Pending_BCS_Creator_ID : int.Parse(pending.pmCreatorID),
                    BCS_Approver_ID = int.Parse(_session.GetString("UserID")),
                    BCS_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    BCS_Last_Updated = DateTime.Now,
                    BCS_Status_ID = 3,
                    BCS_isDeleted = pending.Pending_BCS_isDeleted,
                    BCS_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.BCS_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMBCS.AddRange(addList);
                _context.DMBCS_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejBCS(List<DMBCSViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.BCS_MasterID).ToList();
            List<DMBIRCertSignModel_Pending> allPending = _context.DMBCS_Pending.Where(x => intList.Contains(x.Pending_BCS_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //------------------------------For Approval------------------------------
        //[ PAYEE ]
        public bool addVendor_Pending(NewVendorListViewModel model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();

            var payeeMax = _context.DMVendor.Select(x => x.Vendor_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMVendor_Pending.Select(x => x.Pending_Vendor_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewVendorViewModel dm in model.NewVendorVM)
            {
                DMVendorModel_Pending m = new DMVendorModel_Pending
                {
                    Pending_Vendor_Name = dm.Vendor_Name,
                    Pending_Vendor_MasterID = ++masterIDMax,
                    Pending_Vendor_TIN = dm.Vendor_TIN,
                    Pending_Vendor_Address = dm.Vendor_Address,
                    Pending_Vendor_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Vendor_Filed_Date = DateTime.Now,
                    Pending_Vendor_IsDeleted = false,
                    Pending_Vendor_Status_ID = GlobalSystemValues.STATUS_NEW
                };
                vmList.Add(m);
            }
            DMVendorTRVATModel_Pending tmp = new DMVendorTRVATModel_Pending();
            List<DMVendorTRVATModel_Pending> tmpList = new List<DMVendorTRVATModel_Pending>();
            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.SaveChanges();
                var i = 0;
                vmList.Select(p => p.Pending_Vendor_MasterID)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (model.NewVendorVM[i].Vendor_Tax_Rates_ID != null)
                        {

                            List<string> trIds = model.NewVendorVM[i].Vendor_Tax_Rates_ID.Split(',').ToList();
                            trIds.ForEach(tr =>
                            {
                                if (tr != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_TR_ID = int.Parse(tr)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        if (model.NewVendorVM[i].Vendor_VAT_ID != null)
                        {
                            List<string> vatIds = model.NewVendorVM[i].Vendor_VAT_ID.Split(',').ToList();
                            vatIds.ForEach(vat =>
                            {
                                if (vat != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_VAT_ID = int.Parse(vat)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        i++;
                    });

                _context.DMVendorTRVAT_Pending.AddRange(tmpList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editVendor_Pending(List<DMVendorViewModel> model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();
            foreach (DMVendorViewModel dm in model)
            {
                DMVendorModel_Pending m = new DMVendorModel_Pending
                {
                    Pending_Vendor_Name = dm.Vendor_Name,
                    Pending_Vendor_MasterID = dm.Vendor_MasterID,
                    Pending_Vendor_TIN = dm.Vendor_TIN,
                    Pending_Vendor_Address = dm.Vendor_Address,
                    Pending_Vendor_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Vendor_Filed_Date = DateTime.Now,
                    Pending_Vendor_IsDeleted = false,
                    Pending_Vendor_Status_ID = 8
                };
                vmList.Add(m);
            }

            DMVendorTRVATModel_Pending tmp = new DMVendorTRVATModel_Pending();
            List<DMVendorTRVATModel_Pending> tmpList = new List<DMVendorTRVATModel_Pending>();
            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.SaveChanges();
                var i = 0;
                vmList.Select(p => p.Pending_Vendor_MasterID)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (model[i].Vendor_Tax_Rates_ID != null)
                        {

                            List<string> trIds = model[i].Vendor_Tax_Rates_ID.Split(',').ToList();
                            trIds.ForEach(tr =>
                            {
                                if (tr != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_TR_ID = int.Parse(tr)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        if (model[i].Vendor_VAT_ID != null)
                        {
                            List<string> vatIds = model[i].Vendor_VAT_ID.Split(',').ToList();
                            vatIds.ForEach(vat =>
                            {
                                if (vat != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_VAT_ID = int.Parse(vat)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        i++;
                    });

                _context.DMVendorTRVAT_Pending.AddRange(tmpList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteVendor_Pending(List<DMVendorViewModel> model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();
            List<DMVendorTRVATModel_Pending> vtvList = new List<DMVendorTRVATModel_Pending>();
            foreach (DMVendorViewModel dm in model)
            {
                DMVendorModel_Pending m = new DMVendorModel_Pending
                {
                    Pending_Vendor_Name = dm.Vendor_Name,
                    Pending_Vendor_MasterID = dm.Vendor_MasterID,
                    Pending_Vendor_TIN = dm.Vendor_TIN,
                    Pending_Vendor_Address = dm.Vendor_Address,
                    Pending_Vendor_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Vendor_Filed_Date = DateTime.Now,
                    Pending_Vendor_IsDeleted = true,
                    Pending_Vendor_Status_ID = 9
                };
                vmList.Add(m);
            }
            DMVendorTRVATModel_Pending tmp = new DMVendorTRVATModel_Pending();
            model.Select(x => x.Vendor_MasterID)
                .ToList()
                .ForEach(x => {
                    _context.DMVendorTRVAT.Where(tr => tr.VTV_Vendor_ID == x).ToList()
                   .ForEach(val => {
                       if (val.VTV_TR_ID != 0)
                       {
                           tmp = new DMVendorTRVATModel_Pending()
                           {
                               Pending_VTV_Vendor_ID = x,
                               Pending_VTV_TR_ID = val.VTV_TR_ID
                           };
                           vtvList.Add(tmp);
                       }
                       if (val.VTV_VAT_ID != 0)
                       {
                           tmp = new DMVendorTRVATModel_Pending()
                           {
                               Pending_VTV_Vendor_ID = x,
                               Pending_VTV_VAT_ID = val.VTV_VAT_ID
                           };
                           vtvList.Add(tmp);
                       }
                   });
                });

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.DMVendorTRVAT_Pending.AddRange(vtvList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ DEPARTMENT ]
        public bool addDept_Pending(NewDeptListViewModel model, string userId)
        {
            List<DMDeptModel_Pending> vmList = new List<DMDeptModel_Pending>();

            var deptMax = _context.DMDept.Select(x => x.Dept_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMDept_Pending.Select(x => x.Pending_Dept_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            foreach (NewDeptViewModel dm in model.NewDeptVM)
            {
                DMDeptModel_Pending m = new DMDeptModel_Pending
                {
                    Pending_Dept_Name = dm.Dept_Name,
                    Pending_Dept_MasterID = ++masterIDMax,
                    Pending_Dept_Code = dm.Dept_Code,
                    Pending_Dept_Budget_Unit = dm.Dept_Budget_Unit,
                    Pending_Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Dept_Filed_Date = DateTime.Now,
                    Pending_Dept_isDeleted = false,
                    Pending_Dept_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editDept_Pending(List<DMDeptViewModel> model, string userId)
        {
            List<DMDeptModel_Pending> vmList = new List<DMDeptModel_Pending>();
            var deptMax = _context.DMDept.Select(x => x.Dept_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMDept_Pending.Select(x => x.Pending_Dept_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMDeptViewModel dm in model)
            {
                DMDeptModel_Pending m = new DMDeptModel_Pending
                {
                    Pending_Dept_Name = dm.Dept_Name,
                    Pending_Dept_MasterID = dm.Dept_MasterID,
                    Pending_Dept_Code = dm.Dept_Code,
                    Pending_Dept_Budget_Unit = dm.Dept_Budget_Unit,
                    Pending_Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Dept_Filed_Date = DateTime.Now,
                    Pending_Dept_isDeleted = false,
                    Pending_Dept_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteDept_Pending(List<DMDeptViewModel> model, string userId)
        {
            List<DMDeptModel_Pending> vmList = new List<DMDeptModel_Pending>();
            var deptMax = _context.DMDept.Select(x => x.Dept_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMDept_Pending.Select(x => x.Pending_Dept_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMDeptViewModel dm in model)
            {
                DMDeptModel_Pending m = new DMDeptModel_Pending
                {
                    Pending_Dept_Name = dm.Dept_Name,
                    Pending_Dept_MasterID = dm.Dept_MasterID,
                    Pending_Dept_Code = dm.Dept_Code,
                    Pending_Dept_Budget_Unit = dm.Dept_Budget_Unit,
                    Pending_Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Dept_Filed_Date = DateTime.Now,
                    Pending_Dept_isDeleted = true,
                    Pending_Dept_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Check ]
        public bool addCheck_Pending(NewCheckListViewModel model, string userId)
        {
            List<DMCheckModel_Pending> vmList = new List<DMCheckModel_Pending>();

            var checkMax = _context.DMCheck.Select(x => x.Check_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCheck_Pending.Select(x => x.Pending_Check_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = checkMax > pendingMax ? checkMax : pendingMax;

            foreach (NewCheckViewModel dm in model.NewCheckVM)
            {
                DMCheckModel_Pending m = new DMCheckModel_Pending
                {
                    Pending_Check_Input_Date = DateTime.Now,
                    Pending_Check_MasterID = ++masterIDMax,
                    Pending_Check_Series_From = dm.Check_Series_From,
                    Pending_Check_Series_To = dm.Check_Series_To,
                    Pending_Check_Bank_Info = dm.Check_Bank_Info,
                    Pending_Check_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Check_Filed_Date = DateTime.Now,
                    Pending_Check_isDeleted = false,
                    Pending_Check_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editCheck_Pending(List<DMCheckViewModel> model, string userId)
        {
            List<DMCheckModel_Pending> vmList = new List<DMCheckModel_Pending>();
            var checkMax = _context.DMCheck.Select(x => x.Check_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCheck_Pending.Select(x => x.Pending_Check_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = checkMax > pendingMax ? checkMax : pendingMax;
            foreach (DMCheckViewModel dm in model)
            {
                DMCheckModel_Pending m = new DMCheckModel_Pending
                {
                    Pending_Check_Input_Date = dm.Check_Input_Date,
                    Pending_Check_MasterID = dm.Check_MasterID,
                    Pending_Check_Series_From = dm.Check_Series_From,
                    Pending_Check_Series_To = dm.Check_Series_To,
                    Pending_Check_Bank_Info = dm.Check_Bank_Info,
                    Pending_Check_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Check_Filed_Date = DateTime.Now,
                    Pending_Check_isDeleted = false,
                    Pending_Check_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteCheck_Pending(List<DMCheckViewModel> model, string userId)
        {
            List<DMCheckModel_Pending> vmList = new List<DMCheckModel_Pending>();
            var checkMax = _context.DMCheck.Select(x => x.Check_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCheck_Pending.Select(x => x.Pending_Check_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = checkMax > pendingMax ? checkMax : pendingMax;
            foreach (DMCheckViewModel dm in model)
            {
                DMCheckModel_Pending m = new DMCheckModel_Pending
                {
                    Pending_Check_Input_Date = DateTime.Now,
                    Pending_Check_MasterID = dm.Check_MasterID,
                    Pending_Check_Series_From = dm.Check_Series_From,
                    Pending_Check_Series_To = dm.Check_Series_To,
                    Pending_Check_Bank_Info = dm.Check_Bank_Info,
                    Pending_Check_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Check_Filed_Date = DateTime.Now,
                    Pending_Check_isDeleted = true,
                    Pending_Check_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ ACCOUNT ]
        public bool addAccount_Pending(NewAccountListViewModel model, string userId)
        {
            List<DMAccountModel_Pending> vmList = new List<DMAccountModel_Pending>();

            var payeeMax = _context.DMAccount.Select(x => x.Account_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMAccount_Pending.Select(x => x.Pending_Account_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewAccountViewModel dm in model.NewAccountVM)
            {
                DMAccountModel_Pending m = new DMAccountModel_Pending
                {
                    Pending_Account_Name = dm.Account_Name,
                    Pending_Account_MasterID = ++masterIDMax,
                    Pending_Account_FBT_MasterID = dm.Account_FBT_MasterID,
                    Pending_Account_Group_MasterID = dm.Account_Group_MasterID > 0 ? dm.Account_Group_MasterID : 0,
                    Pending_Account_Currency_MasterID = dm.Account_Currency_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Budget_Code = dm.Account_Budget_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = false,
                    Pending_Account_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editAccount_Pending(List<DMAccountViewModel> model, string userId)
        {
            List<DMAccountModel_Pending> vmList = new List<DMAccountModel_Pending>();
            foreach (DMAccountViewModel dm in model)
            {
                DMAccountModel_Pending m = new DMAccountModel_Pending
                {
                    Pending_Account_Name = dm.Account_Name,
                    Pending_Account_MasterID = dm.Account_MasterID,
                    Pending_Account_FBT_MasterID = dm.Account_FBT_MasterID,
                    Pending_Account_Group_MasterID = dm.Account_Group_MasterID,
                    Pending_Account_Currency_MasterID = dm.Account_Currency_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Budget_Code = dm.Account_Budget_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = false,
                    Pending_Account_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteAccount_Pending(List<DMAccountViewModel> model, string userId)
        {
            List<DMAccountModel_Pending> vmList = new List<DMAccountModel_Pending>();
            foreach (DMAccountViewModel dm in model)
            {
                DMAccountModel_Pending m = new DMAccountModel_Pending
                {
                    Pending_Account_Name = dm.Account_Name,
                    Pending_Account_MasterID = dm.Account_MasterID,
                    Pending_Account_FBT_MasterID = dm.Account_FBT_MasterID,
                    Pending_Account_Group_MasterID = dm.Account_Group_MasterID,
                    Pending_Account_Currency_MasterID = dm.Account_Currency_MasterID,
                    Pending_Account_Budget_Code = dm.Account_Budget_Code,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = true,
                    Pending_Account_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ ACCOUNT GROUP ]
        public bool addAccountGroup_Pending(NewAccountGroupListViewModel model, string userId)
        {
            List<DMAccountGroupModel_Pending> vmList = new List<DMAccountGroupModel_Pending>();

            var payeeMax = _context.DMAccountGroup.Select(x => x.AccountGroup_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMAccountGroup_Pending.Select(x => x.Pending_AccountGroup_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewAccountGroupViewModel dm in model.NewAccountGroupVM)
            {
                DMAccountGroupModel_Pending m = new DMAccountGroupModel_Pending
                {
                    Pending_AccountGroup_Name = dm.AccountGroup_Name,
                    Pending_AccountGroup_MasterID = ++masterIDMax,
                    Pending_AccountGroup_Code = dm.AccountGroup_Code,
                    Pending_AccountGroup_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_AccountGroup_Filed_Date = DateTime.Now,
                    Pending_AccountGroup_isDeleted = false,
                    Pending_AccountGroup_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editAccountGroup_Pending(List<DMAccountGroupViewModel> model, string userId)
        {
            List<DMAccountGroupModel_Pending> vmList = new List<DMAccountGroupModel_Pending>();
            foreach (DMAccountGroupViewModel dm in model)
            {
                DMAccountGroupModel_Pending m = new DMAccountGroupModel_Pending
                {
                    Pending_AccountGroup_Name = dm.AccountGroup_Name,
                    Pending_AccountGroup_MasterID = dm.AccountGroup_MasterID,
                    Pending_AccountGroup_Code = dm.AccountGroup_Code,
                    Pending_AccountGroup_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_AccountGroup_Filed_Date = DateTime.Now,
                    Pending_AccountGroup_isDeleted = false,
                    Pending_AccountGroup_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteAccountGroup_Pending(List<DMAccountGroupViewModel> model, string userId)
        {
            List<DMAccountGroupModel_Pending> vmList = new List<DMAccountGroupModel_Pending>();
            foreach (DMAccountGroupViewModel dm in model)
            {
                DMAccountGroupModel_Pending m = new DMAccountGroupModel_Pending
                {
                    Pending_AccountGroup_Name = dm.AccountGroup_Name,
                    Pending_AccountGroup_MasterID = dm.AccountGroup_MasterID,
                    Pending_AccountGroup_Code = dm.AccountGroup_Code,
                    Pending_AccountGroup_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_AccountGroup_Filed_Date = DateTime.Now,
                    Pending_AccountGroup_isDeleted = true,
                    Pending_AccountGroup_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ VAT ]
        public bool addVAT_Pending(NewVATListViewModel model, string userId)
        {
            List<DMVATModel_Pending> vmList = new List<DMVATModel_Pending>();

            var VATMax = _context.DMVAT.Select(x => x.VAT_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMVAT_Pending.Select(x => x.Pending_VAT_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = VATMax > pendingMax ? VATMax : pendingMax;

            foreach (NewVATViewModel dm in model.NewVATVM)
            {
                DMVATModel_Pending m = new DMVATModel_Pending
                {
                    Pending_VAT_Name = dm.VAT_Name,
                    Pending_VAT_MasterID = ++masterIDMax,
                    Pending_VAT_Rate = dm.VAT_Rate,
                    Pending_VAT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_VAT_Filed_Date = DateTime.Now,
                    Pending_VAT_isDeleted = false,
                    Pending_VAT_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editVAT_Pending(List<DMVATViewModel> model, string userId)
        {
            List<DMVATModel_Pending> vmList = new List<DMVATModel_Pending>();
            foreach (DMVATViewModel dm in model)
            {
                DMVATModel_Pending m = new DMVATModel_Pending
                {
                    Pending_VAT_Name = dm.VAT_Name,
                    Pending_VAT_MasterID = dm.VAT_MasterID,
                    Pending_VAT_Rate = dm.VAT_Rate,
                    Pending_VAT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_VAT_Filed_Date = DateTime.Now,
                    Pending_VAT_isDeleted = false,
                    Pending_VAT_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteVAT_Pending(List<DMVATViewModel> model, string userId)
        {
            List<DMVATModel_Pending> vmList = new List<DMVATModel_Pending>();
            foreach (DMVATViewModel dm in model)
            {
                DMVATModel_Pending m = new DMVATModel_Pending
                {
                    Pending_VAT_Name = dm.VAT_Name,
                    Pending_VAT_MasterID = dm.VAT_MasterID,
                    Pending_VAT_Rate = dm.VAT_Rate,
                    Pending_VAT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_VAT_Filed_Date = DateTime.Now,
                    Pending_VAT_isDeleted = true,
                    Pending_VAT_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ FBT ]
        public bool addFBT_Pending(NewFBTListViewModel model, string userId)
        {
            List<DMFBTModel_Pending> vmList = new List<DMFBTModel_Pending>();

            var payeeMax = _context.DMFBT.Select(x => x.FBT_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMFBT_Pending.Select(x => x.Pending_FBT_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewFBTViewModel dm in model.NewFBTVM)
            {
                DMFBTModel_Pending m = new DMFBTModel_Pending
                {
                    Pending_FBT_Name = dm.FBT_Name,
                    Pending_FBT_MasterID = ++masterIDMax,
                    Pending_FBT_Formula = dm.FBT_Formula,
                    Pending_FBT_Tax_Rate = dm.FBT_Tax_Rate,
                    Pending_FBT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_FBT_Filed_Date = DateTime.Now,
                    Pending_FBT_isDeleted = false,
                    Pending_FBT_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editFBT_Pending(List<DMFBTViewModel> model, string userId)
        {
            List<DMFBTModel_Pending> vmList = new List<DMFBTModel_Pending>();
            foreach (DMFBTViewModel dm in model)
            {
                DMFBTModel_Pending m = new DMFBTModel_Pending
                {
                    Pending_FBT_Name = dm.FBT_Name,
                    Pending_FBT_MasterID = dm.FBT_MasterID,
                    Pending_FBT_Formula = dm.FBT_Formula,
                    Pending_FBT_Tax_Rate = dm.FBT_Tax_Rate,
                    Pending_FBT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_FBT_Filed_Date = DateTime.Now,
                    Pending_FBT_isDeleted = false,
                    Pending_FBT_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteFBT_Pending(List<DMFBTViewModel> model, string userId)
        {
            List<DMFBTModel_Pending> vmList = new List<DMFBTModel_Pending>();
            foreach (DMFBTViewModel dm in model)
            {
                DMFBTModel_Pending m = new DMFBTModel_Pending
                {
                    Pending_FBT_Name = dm.FBT_Name,
                    Pending_FBT_MasterID = dm.FBT_MasterID,
                    Pending_FBT_Formula = dm.FBT_Formula,
                    Pending_FBT_Tax_Rate = dm.FBT_Tax_Rate,
                    Pending_FBT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_FBT_Filed_Date = DateTime.Now,
                    Pending_FBT_isDeleted = true,
                    Pending_FBT_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ TR ]
        public bool addTR_Pending(NewTRListViewModel model, string userId)
        {
            List<DMTRModel_Pending> vmList = new List<DMTRModel_Pending>();

            var payeeMax = _context.DMTR.Select(x => x.TR_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMTR_Pending.Select(x => x.Pending_TR_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewTRViewModel dm in model.NewTRVM)
            {
                DMTRModel_Pending m = new DMTRModel_Pending
                {
                    Pending_TR_Nature = dm.TR_Nature,
                    Pending_TR_MasterID = ++masterIDMax,
                    Pending_TR_Tax_Rate = dm.TR_Tax_Rate,
                    Pending_TR_ATC = dm.TR_ATC,
                    Pending_TR_WT_Title = dm.TR_WT_Title,
                    Pending_TR_Nature_Income_Payment = dm.TR_Nature_Income_Payment,
                    Pending_TR_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_TR_Filed_Date = DateTime.Now,
                    Pending_TR_isDeleted = false,
                    Pending_TR_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editTR_Pending(List<DMTRViewModel> model, string userId)
        {
            List<DMTRModel_Pending> vmList = new List<DMTRModel_Pending>();
            foreach (DMTRViewModel dm in model)
            {
                DMTRModel_Pending m = new DMTRModel_Pending
                {
                    Pending_TR_WT_Title = dm.TR_WT_Title,
                    Pending_TR_Nature = dm.TR_Nature,
                    Pending_TR_MasterID = dm.TR_MasterID,
                    Pending_TR_Tax_Rate = dm.TR_Tax_Rate,
                    Pending_TR_ATC = dm.TR_ATC,
                    Pending_TR_Nature_Income_Payment = dm.TR_Nature_Income_Payment,
                    Pending_TR_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_TR_Filed_Date = DateTime.Now,
                    Pending_TR_isDeleted = false,
                    Pending_TR_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteTR_Pending(List<DMTRViewModel> model, string userId)
        {
            List<DMTRModel_Pending> vmList = new List<DMTRModel_Pending>();
            foreach (DMTRViewModel dm in model)
            {
                DMTRModel_Pending m = new DMTRModel_Pending
                {
                    Pending_TR_WT_Title = dm.TR_WT_Title,
                    Pending_TR_Nature = dm.TR_Nature,
                    Pending_TR_MasterID = dm.TR_MasterID,
                    Pending_TR_Tax_Rate = dm.TR_Tax_Rate,
                    Pending_TR_ATC = dm.TR_ATC,
                    Pending_TR_Nature_Income_Payment = dm.TR_Nature_Income_Payment,
                    Pending_TR_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_TR_Filed_Date = DateTime.Now,
                    Pending_TR_isDeleted = true,
                    Pending_TR_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Curr ]
        public bool addCurr_Pending(NewCurrListViewModel model, string userId)
        {
            List<DMCurrencyModel_Pending> vmList = new List<DMCurrencyModel_Pending>();

            var payeeMax = _context.DMCurrency.Select(x => x.Curr_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCurrency_Pending.Select(x => x.Pending_Curr_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewCurrViewModel dm in model.NewCurrVM)
            {
                DMCurrencyModel_Pending m = new DMCurrencyModel_Pending
                {
                    Pending_Curr_Name = dm.Curr_Name,
                    Pending_Curr_MasterID = ++masterIDMax,
                    Pending_Curr_CCY_ABBR = dm.Curr_CCY_ABBR,
                    Pending_Curr_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Curr_Filed_Date = DateTime.Now,
                    Pending_Curr_isDeleted = false,
                    Pending_Curr_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editCurr_Pending(List<DMCurrencyViewModel> model, string userId)
        {
            List<DMCurrencyModel_Pending> vmList = new List<DMCurrencyModel_Pending>();
            foreach (DMCurrencyViewModel dm in model)
            {
                DMCurrencyModel_Pending m = new DMCurrencyModel_Pending
                {
                    Pending_Curr_Name = dm.Curr_Name,
                    Pending_Curr_MasterID = dm.Curr_MasterID,
                    Pending_Curr_CCY_ABBR = dm.Curr_CCY_ABBR,
                    Pending_Curr_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Curr_Filed_Date = DateTime.Now,
                    Pending_Curr_isDeleted = false,
                    Pending_Curr_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteCurr_Pending(List<DMCurrencyViewModel> model, string userId)
        {
            List<DMCurrencyModel_Pending> vmList = new List<DMCurrencyModel_Pending>();
            foreach (DMCurrencyViewModel dm in model)
            {
                DMCurrencyModel_Pending m = new DMCurrencyModel_Pending
                {
                    Pending_Curr_Name = dm.Curr_Name,
                    Pending_Curr_MasterID = dm.Curr_MasterID,
                    Pending_Curr_CCY_ABBR = dm.Curr_CCY_ABBR,
                    Pending_Curr_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Curr_Filed_Date = DateTime.Now,
                    Pending_Curr_isDeleted = true,
                    Pending_Curr_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Employee ]
        public bool addEmp_Pending(NewEmpListViewModel model, string userId)
        {
            List<DMEmpModel_Pending> vmList = new List<DMEmpModel_Pending>();
            var deptMax = _context.DMEmp.Select(x => x.Emp_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEmp_Pending.Select(x => x.Pending_Emp_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            foreach (NewEmpViewModel dm in model.NewEmpVM)
            {
                var tempAccNo = dm.Emp_Acc_No ?? "";
                DMEmpModel_Pending m = new DMEmpModel_Pending
                {
                    Pending_Emp_Name = dm.Emp_Name,
                    Pending_Emp_MasterID = ++masterIDMax,
                    Pending_Emp_Acc_No = tempAccNo,
                    Pending_Emp_Category_ID = dm.Emp_Category_ID,
                    Pending_Emp_FBT_MasterID = dm.Emp_FBT_MasterID > 0 ? dm.Emp_FBT_MasterID : 0,
                    Pending_Emp_Type = tempAccNo.Length <= 0 ? "Temporary" : "Regular",
                    Pending_Emp_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Emp_Filed_Date = DateTime.Now,
                    Pending_Emp_isDeleted = false,
                    Pending_Emp_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editEmp_Pending(List<DMEmpViewModel> model, string userId)
        {
            List<DMEmpModel_Pending> vmList = new List<DMEmpModel_Pending>();
            var deptMax = _context.DMEmp.Select(x => x.Emp_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEmp_Pending.Select(x => x.Pending_Emp_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMEmpViewModel dm in model)
            {
                var tempAccNo = dm.Emp_Acc_No ?? "";
                DMEmpModel_Pending m = new DMEmpModel_Pending
                {
                    Pending_Emp_Name = dm.Emp_Name,
                    Pending_Emp_MasterID = dm.Emp_MasterID,
                    Pending_Emp_Acc_No = tempAccNo,
                    Pending_Emp_Category_ID = dm.Emp_Category_ID,
                    Pending_Emp_FBT_MasterID = dm.Emp_FBT_MasterID,
                    Pending_Emp_Type = tempAccNo.Length <= 0 ? "Temporary" : "Regular",
                    Pending_Emp_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Emp_Filed_Date = DateTime.Now,
                    Pending_Emp_isDeleted = false,
                    Pending_Emp_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteEmp_Pending(List<DMEmpViewModel> model, string userId)
        {
            List<DMEmpModel_Pending> vmList = new List<DMEmpModel_Pending>();
            var deptMax = _context.DMEmp.Select(x => x.Emp_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEmp_Pending.Select(x => x.Pending_Emp_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMEmpViewModel dm in model)
            {
                var tempAccNo = dm.Emp_Acc_No ?? "";
                DMEmpModel_Pending m = new DMEmpModel_Pending
                {
                    Pending_Emp_Name = dm.Emp_Name,
                    Pending_Emp_MasterID = dm.Emp_MasterID,
                    Pending_Emp_Acc_No = tempAccNo,
                    Pending_Emp_Category_ID = dm.Emp_Category_ID,
                    Pending_Emp_FBT_MasterID = dm.Emp_FBT_MasterID,
                    Pending_Emp_Type = tempAccNo.Length <= 0 ? "Temporary" : "Regular",
                    Pending_Emp_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Emp_Filed_Date = DateTime.Now,
                    Pending_Emp_isDeleted = true,
                    Pending_Emp_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Customer ]
        public bool addCust_Pending(NewCustListViewModel model, string userId)
        {
            List<DMCustModel_Pending> vmList = new List<DMCustModel_Pending>();

            var deptMax = _context.DMCust.Select(x => x.Cust_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCust_Pending.Select(x => x.Pending_Cust_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            foreach (NewCustViewModel dm in model.NewCustVM)
            {
                DMCustModel_Pending m = new DMCustModel_Pending
                {
                    Pending_Cust_Name = dm.Cust_Name,
                    Pending_Cust_MasterID = ++masterIDMax,
                    Pending_Cust_Abbr = dm.Cust_Abbr,
                    Pending_Cust_No = dm.Cust_No,
                    Pending_Cust_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Cust_Filed_Date = DateTime.Now,
                    Pending_Cust_isDeleted = false,
                    Pending_Cust_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editCust_Pending(List<DMCustViewModel> model, string userId)
        {
            List<DMCustModel_Pending> vmList = new List<DMCustModel_Pending>();
            var deptMax = _context.DMCust.Select(x => x.Cust_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCust_Pending.Select(x => x.Pending_Cust_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMCustViewModel dm in model)
            {
                DMCustModel_Pending m = new DMCustModel_Pending
                {
                    Pending_Cust_Name = dm.Cust_Name,
                    Pending_Cust_MasterID = dm.Cust_MasterID,
                    Pending_Cust_Abbr = dm.Cust_Abbr,
                    Pending_Cust_No = dm.Cust_No,
                    Pending_Cust_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Cust_Filed_Date = DateTime.Now,
                    Pending_Cust_isDeleted = false,
                    Pending_Cust_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteCust_Pending(List<DMCustViewModel> model, string userId)
        {
            List<DMCustModel_Pending> vmList = new List<DMCustModel_Pending>();
            var deptMax = _context.DMCust.Select(x => x.Cust_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCust_Pending.Select(x => x.Pending_Cust_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMCustViewModel dm in model)
            {
                DMCustModel_Pending m = new DMCustModel_Pending
                {
                    Pending_Cust_Name = dm.Cust_Name,
                    Pending_Cust_MasterID = dm.Cust_MasterID,
                    Pending_Cust_Abbr = dm.Cust_Abbr,
                    Pending_Cust_No = dm.Cust_No,
                    Pending_Cust_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Cust_Filed_Date = DateTime.Now,
                    Pending_Cust_isDeleted = true,
                    Pending_Cust_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ BIR Cert Signatory ]
        public List<SelectListItem> getEmpSelectList()
        {
            List<SelectListItem> empList = new List<SelectListItem>();
            _context.User.Where(x => x.User_InUse == true && (x.User_Role != "admin" || x.User_Role != "maker")).ToList().ForEach(x => {
                empList.Add(new SelectListItem() { Text = x.User_LName + ", " + x.User_FName, Value = x.User_ID + "" });
            });
            return empList;
        }
        public bool addBCS_Pending(NewBCSViewModel model, string userId)
        {
            List<DMBIRCertSignModel_Pending> vmList = new List<DMBIRCertSignModel_Pending>();

            var deptMax = _context.DMBCS.Select(x => x.BCS_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMBCS_Pending.Select(x => x.Pending_BCS_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, _context.FileLocation.Where(x => x.FL_Type == "BCS").Select(x => x.FL_Location).FirstOrDefault());
            //save file to uploads folder
            FileService objFile = new FileService();
            string empName = getEmpSelectList().Where(x => x.Value == model.BCS_User_ID + "").FirstOrDefault().Value;
            string strFilePath = "";

            if (model.BCS_Signatures != null)
            {
                strFilePath = objFile.SaveFile(model.BCS_Signatures, uploads, empName);
            }
            DMBIRCertSignModel_Pending m = new DMBIRCertSignModel_Pending
            {
                Pending_BCS_User_ID = model.BCS_User_ID,
                Pending_BCS_MasterID = ++masterIDMax,
                Pending_BCS_TIN = model.BCS_TIN,
                Pending_BCS_Position = model.BCS_Position,
                Pending_BCS_Signatures = strFilePath,
                Pending_BCS_Creator_ID = int.Parse(_session.GetString("UserID")),
                Pending_BCS_Filed_Date = DateTime.Now,
                Pending_BCS_isDeleted = false,
                Pending_BCS_Status_ID = 7
            };
            vmList.Add(m);

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editBCS_Pending(DMBCS2ViewModel model, string userId)
        {
            List<DMBIRCertSignModel_Pending> vmList = new List<DMBIRCertSignModel_Pending>();
            var deptMax = _context.DMBCS.Select(x => x.BCS_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMBCS_Pending.Select(x => x.Pending_BCS_MasterID).
                DefaultIfEmpty(0).Max();
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, _context.FileLocation.Where(x => x.FL_Type == "BCS").Select(x => x.FL_Location).FirstOrDefault());
            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            string strFilePath = _context.DMBCS.Where(x => x.BCS_MasterID == model.BCS_MasterID).Select(x => x.BCS_Signatures).FirstOrDefault();
            //if there is image uploaded
            if (model.BCS_Signatures != null)
            {

                FileService objFile = new FileService();
                string empName = getEmpSelectList().Where(x => x.Value == model.BCS_User_ID + "").FirstOrDefault().Value;
                strFilePath = objFile.SaveFile(model.BCS_Signatures, uploads, empName);
            }
            DMBIRCertSignModel_Pending m = new DMBIRCertSignModel_Pending
            {
                Pending_BCS_User_ID = model.BCS_User_ID,
                Pending_BCS_TIN = model.BCS_TIN,
                Pending_BCS_Position = model.BCS_Position,
                Pending_BCS_MasterID = model.BCS_MasterID,
                Pending_BCS_Signatures = strFilePath,
                Pending_BCS_Creator_ID = int.Parse(_session.GetString("UserID")),
                Pending_BCS_Filed_Date = DateTime.Now,
                Pending_BCS_isDeleted = false,
                Pending_BCS_Status_ID = 8
            };
            vmList.Add(m);

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteBCS_Pending(List<DMBCSViewModel> model, string userId)
        {
            List<DMBIRCertSignModel_Pending> vmList = new List<DMBIRCertSignModel_Pending>();
            var deptMax = _context.DMBCS.Select(x => x.BCS_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMBCS_Pending.Select(x => x.Pending_BCS_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMBCSViewModel dm in model)
            {
                DMBIRCertSignModel_Pending m = new DMBIRCertSignModel_Pending
                {
                    Pending_BCS_User_ID = dm.BCS_User_ID,
                    Pending_BCS_TIN = dm.BCS_TIN,
                    Pending_BCS_Position = dm.BCS_Position,
                    Pending_BCS_MasterID = dm.BCS_MasterID,
                    Pending_BCS_Signatures = dm.BCS_Signatures,
                    Pending_BCS_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_BCS_Filed_Date = DateTime.Now,
                    Pending_BCS_isDeleted = true,
                    Pending_BCS_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }

        // [Budget Monitoring]
        public List<BMViewModel> PopulateBM()
        {
            var accList = _context.DMAccount.Where(x => x.Account_isActive == true && x.Account_isDeleted == false
                                                    && x.Account_Fund == true).ToList();

            List<BMViewModel> bmvmList = new List<BMViewModel>();

            var dbBudget = (from bud in _context.Budget
                            join acc in _context.DMAccount on bud.Budget_Account_MasterID equals acc.Account_MasterID
                            join accGrp in _context.DMAccountGroup on acc.Account_Group_MasterID equals accGrp.AccountGroup_MasterID
                            where bud.Budget_IsActive == true && bud.Budget_isDeleted == false &&
                            acc.Account_isActive == true && acc.Account_isDeleted == false &&
                            accGrp.AccountGroup_isActive == true && accGrp.AccountGroup_isDeleted == false
                            orderby acc.Account_Budget_Code
                            select new
                            {
                                bud.Budget_ID,
                                acc.Account_ID,
                                acc.Account_MasterID,
                                accGrp.AccountGroup_Name,
                                bud.Budget_Amount,
                                bud.Budget_Date_Registered,
                                bud.Budget_New_Amount,
                                bud.Budget_GWrite_Status
                            }).ToList();

            foreach (var i in dbBudget)
            {
                //Get latest account information of saved account ID.
                var accInfo = accList.Where(x => x.Account_MasterID == i.Account_MasterID && x.Account_isActive == true
                                            && x.Account_isDeleted == false).FirstOrDefault();
                if (accInfo == null) continue;

                bmvmList.Add(new BMViewModel()
                {
                    BM_Budget_ID = i.Budget_ID,
                    BM_Acc_Group_Name = i.AccountGroup_Name,
                    BM_Acc_Name = accInfo.Account_Name,
                    BM_GBase_Code = accInfo.Account_Budget_Code,
                    BM_Acc_Num = accInfo.Account_No,
                    BM_Budget_Current = i.Budget_Amount,
                    BM_Budget_Amount = i.Budget_New_Amount,
                    BM_GWrite_StatusID = i.Budget_GWrite_Status,
                    BM_GWrite_Status = GlobalSystemValues.getStatus(i.Budget_GWrite_Status),
                    BM_Date_Registered = i.Budget_Date_Registered,
                    BM_GWrite_Msg = GetGWriteErrorMsgForBM(i.Budget_ID, "budget")
                });
            };

            return bmvmList;
        }

        public bool UpdateBMIinfoByDMAccountEdit(int AccMasterID, int newAccID, int newAccGrpMasterID, int action)
        {
            BudgetModel budget = _context.Budget.Where(x => x.Budget_Account_MasterID == AccMasterID &&
                                                x.Budget_IsActive == true && x.Budget_isDeleted == false).FirstOrDefault();
            if (budget == null)
            {
                return false;
            }

            if (action == GlobalSystemValues.STATUS_EDIT)
            {
                budget.Budget_Account_ID = newAccID;
                _context.Entry(budget).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else if (action == GlobalSystemValues.STATUS_DELETE)
            {
                budget.Budget_IsActive = false;
                budget.Budget_isDeleted = true;
                _context.Entry(budget).State = EntityState.Modified;
                _context.SaveChanges();
            }
            return true;
        }

        public string GetGWriteErrorMsgForBM(int GWTransID, string GWType)
        {
            var gwriteTrans = _context.GwriteTransLists.Where(x => x.GW_TransID == GWTransID).LastOrDefault();
            if (gwriteTrans == null)
                return "GWrite Message Not Available.";
            var gwriteDtl = _gWriteContext.TblRequestDetails.Where(x => x.RequestId == gwriteTrans.GW_GWrite_ID).FirstOrDefault();
            if (gwriteDtl == null)
                return "GWrite Message Not Available.";

            if (String.IsNullOrEmpty(gwriteDtl.ReturnMessage))
                return "Waiting for G-Write to be processed.";
            return gwriteDtl.ReturnMessage;
        }

        public bool CancelBudgetRegistration(int budgetID)
        {
            var budget = _context.Budget.Where(x => x.Budget_ID == budgetID).FirstOrDefault();

            if (budget == null) return false;
            if (budget.Budget_GWrite_Status != GlobalSystemValues.STATUS_ERROR) return false;

            budget.Budget_GWrite_Status = GlobalSystemValues.STATUS_APPROVED;
            budget.Budget_Date_Registered = DateTime.Now;
            budget.Budget_New_Amount = 0.00M;
            _context.Entry(budget).State = EntityState.Modified;
            _context.SaveChanges();

            return true;
        }
        // [Report]
        public IEnumerable<HomeReportOutputAPSWT_MModel> GetAPSWT_MData(int month, int year)
        {
            List<HomeReportOutputAPSWT_MModel> dbAPSWT_M = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> dbAPSWT_M_Check = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> dbAPSWT_M_LIQ = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> dbAPSWT_M_NC = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> finalOutput = new List<HomeReportOutputAPSWT_MModel>();
            var vendorMasterIDList = _context.DMVendor.OrderBy(x => x.Vendor_MasterID).Select(x => x.Vendor_MasterID).Distinct();
            var taxRatesList = _context.DMTR.OrderBy(x => x.TR_Tax_Rate).Select(x => x.TR_Tax_Rate).Distinct();
            var vatList = _context.DMVAT.ToList();

            //Get data from Taxable expense table except cash advance(SS) and Check Payment
            dbAPSWT_M = (from expEntryDetl in _context.ExpenseEntryDetails
                         join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                         join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                         join vend in _context.DMVendor on expEntryDetl.ExpDtl_Ewt_Payor_Name_ID equals vend.Vendor_ID
                         where status.Contains(expense.Expense_Status)
                         && expense.Expense_Last_Updated.Month == month
                         && expense.Expense_Last_Updated.Year == year
                         && expense.Expense_Type != GlobalSystemValues.TYPE_SS
                         && expense.Expense_Type != GlobalSystemValues.TYPE_CV
                         orderby expense.Expense_Last_Updated
                         select new HomeReportOutputAPSWT_MModel
                         {
                             Payee = vend.Vendor_Name,
                             Tin = vend.Vendor_TIN,
                             ATC = tr.TR_ATC,
                             NOIP = tr.TR_Nature_Income_Payment,
                             //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                             //Example: 45,000 / 1.12 = 40,178.57
                             AOIP = (expEntryDetl.ExpDtl_Vat != 0) ? (expEntryDetl.ExpDtl_Debit / (decimal)(1 + vatList
                                    .Where(x => x.VAT_ID == expEntryDetl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : expEntryDetl.ExpDtl_Debit,
                             RateOfTax = tr.TR_Tax_Rate,
                             AOTW = expEntryDetl.ExpDtl_Credit_Ewt,
                             Last_Update_Date = expense.Expense_Last_Updated,
                             Vendor_masterID = vend.Vendor_MasterID
                         }).ToList();

            //Get data from Taxable expense table for Check Payment
            dbAPSWT_M_Check = (from expEntryDetl in _context.ExpenseEntryDetails
                               join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                               join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                               join vend in _context.DMVendor on expense.Expense_Payee equals vend.Vendor_ID
                               where status.Contains(expense.Expense_Status)
                               && expense.Expense_Last_Updated.Month == month
                               && expense.Expense_Last_Updated.Year == year
                               && expense.Expense_Type == GlobalSystemValues.TYPE_CV
                               && expense.Expense_Payee_Type == GlobalSystemValues.PAYEETYPE_VENDOR
                               orderby expense.Expense_Last_Updated
                               select new HomeReportOutputAPSWT_MModel
                               {
                                   Payee = vend.Vendor_Name,
                                   Tin = vend.Vendor_TIN,
                                   ATC = tr.TR_ATC,
                                   NOIP = tr.TR_Nature_Income_Payment,
                                   //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                                   //Example: 45,000 / 1.12 = 40,178.57
                                   AOIP = (expEntryDetl.ExpDtl_Vat != 0) ? (expEntryDetl.ExpDtl_Debit / (decimal)(1 + vatList
                                          .Where(x => x.VAT_ID == expEntryDetl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : expEntryDetl.ExpDtl_Debit,
                                   RateOfTax = tr.TR_Tax_Rate,
                                   AOTW = expEntryDetl.ExpDtl_Credit_Ewt,
                                   Last_Update_Date = expense.Expense_Last_Updated,
                                   Vendor_masterID = vend.Vendor_MasterID
                               }).ToList();

            //Get data from Taxable liquidation table.
            dbAPSWT_M_LIQ = (from ie in _context.LiquidationInterEntity
                             join expDtl in _context.ExpenseEntryDetails on ie.ExpenseEntryDetailModel.ExpDtl_ID equals expDtl.ExpDtl_ID
                             join liqDtl in _context.LiquidationEntryDetails on expDtl.ExpenseEntryModel.Expense_ID equals liqDtl.ExpenseEntryModel.Expense_ID
                             join tr in _context.DMTR on ie.Liq_TaxRate equals tr.TR_ID
                             join vend in _context.DMVendor on ie.Liq_VendorID equals vend.Vendor_ID
                             where status.Contains(liqDtl.Liq_Status)
                             && liqDtl.Liq_LastUpdated_Date.Month == month
                             && liqDtl.Liq_LastUpdated_Date.Year == year
                             select new HomeReportOutputAPSWT_MModel
                             {
                                 Payee = vend.Vendor_Name,
                                 Tin = vend.Vendor_TIN,
                                 ATC = tr.TR_ATC,
                                 NOIP = tr.TR_Nature_Income_Payment,
                                 //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                                 //Example: 45,000 / 1.12 = 40,178.57
                                 AOIP = (expDtl.ExpDtl_Vat != 0) ? ((ie.Liq_Amount_2_1 + ie.Liq_Amount_2_2 + ie.Liq_Amount_3_1) / (decimal)(1 + vatList
                                    .Where(x => x.VAT_ID == expDtl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : (ie.Liq_Amount_2_1 + ie.Liq_Amount_2_2 + ie.Liq_Amount_3_1),
                                 RateOfTax = tr.TR_Tax_Rate,
                                 AOTW = ie.Liq_Amount_2_2,
                                 Last_Update_Date = liqDtl.Liq_LastUpdated_Date,
                                 Vendor_masterID = vend.Vendor_MasterID
                             }).ToList();

            //Get data from Taxable Non-cash  table.
            dbAPSWT_M_NC = (from ncDtl in _context.ExpenseEntryNonCashDetails
                            join ncAcc in _context.ExpenseEntryNonCashDetailAccounts on ncDtl.ExpNCDtl_ID equals ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID
                            join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
                            join exp in _context.ExpenseEntry on nc.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                            join tr in _context.DMTR on ncDtl.ExpNCDtl_TR_ID equals tr.TR_ID
                            join vend in _context.DMVendor on ncDtl.ExpNCDtl_Vendor_ID equals vend.Vendor_ID
                            join ncAcc2 in (from ncDtl2 in _context.ExpenseEntryNonCashDetails
                                            join ncAcc2 in _context.ExpenseEntryNonCashDetailAccounts on ncDtl2.ExpNCDtl_ID equals ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID
                                            where ncAcc2.ExpNCDtlAcc_Type_ID == 3
                                            select new { ncAcc2.ExpNCDtlAcc_Amount, ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID }) on ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID equals ncAcc2.ExpNCDtl_ID
                            where status.Contains(exp.Expense_Status)
                            && ncAcc.ExpNCDtlAcc_Type_ID == 3
                            && exp.Expense_Last_Updated.Month == month
                            && exp.Expense_Last_Updated.Year == year
                            select new HomeReportOutputAPSWT_MModel
                            {
                                Payee = vend.Vendor_Name,
                                Tin = vend.Vendor_TIN,
                                RateOfTax = tr.TR_Tax_Rate,
                                ATC = tr.TR_ATC,
                                NOIP = tr.TR_Nature_Income_Payment,
                                AOIP = (ncDtl.ExpNCDtl_TaxBasedAmt != 0) ? ncDtl.ExpNCDtl_TaxBasedAmt : ncAcc.ExpNCDtlAcc_Amount,
                                AOTW = ncAcc2.ExpNCDtlAcc_Amount,
                                Last_Update_Date = exp.Expense_Last_Updated,
                                Vendor_masterID = vend.Vendor_MasterID
                            }).Where(x => x.AOTW > 0).ToList();
            //dbAPSWT_M_NC = (from ncDtl in _context.ExpenseEntryNonCashDetails
            //                join ncAcc in _context.ExpenseEntryNonCashDetailAccounts on ncDtl.ExpNCDtl_ID equals ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID
            //                join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
            //                join exp in _context.ExpenseEntry on nc.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
            //                join tr in _context.DMTR on ncDtl.ExpNCDtl_TR_ID equals tr.TR_ID
            //                join vend in _context.DMVendor on ncDtl.ExpNCDtl_Vendor_ID equals vend.Vendor_ID
            //                join ncAcc2 in (from ncDtl2 in _context.ExpenseEntryNonCashDetails
            //                                join ncAcc2 in _context.ExpenseEntryNonCashDetailAccounts on ncDtl2.ExpNCDtl_ID equals ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID
            //                                where ncAcc2.ExpNCDtlAcc_Type_ID == 3
            //                                select new { ncAcc2.ExpNCDtlAcc_Amount, ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID }) on ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID equals ncAcc2.ExpNCDtl_ID
            //                where status.Contains(exp.Expense_Status)
            //                && ncAcc.ExpNCDtlAcc_Type_ID == 1
            //                && exp.Expense_Last_Updated.Month == month
            //                && exp.Expense_Last_Updated.Year == year
            //                select new HomeReportOutputAPSWT_MModel
            //                {
            //                    Payee = vend.Vendor_Name,
            //                    Tin = vend.Vendor_TIN,
            //                    RateOfTax = tr.TR_Tax_Rate,
            //                    ATC = tr.TR_ATC,
            //                    NOIP = tr.TR_Nature_Income_Payment,
            //                    AOIP = (ncDtl.ExpNCDtl_TaxBasedAmt != 0 ) ? ncDtl.ExpNCDtl_TaxBasedAmt : ncAcc.ExpNCDtlAcc_Amount,
            //                    AOTW = ncAcc2.ExpNCDtlAcc_Amount,
            //                    Last_Update_Date = exp.Expense_Last_Updated,
            //                    Vendor_masterID = vend.Vendor_MasterID
            //                }).Where(x => x.AOTW > 0).ToList();

            var dbAPSWT_Conc = dbAPSWT_M.Concat(dbAPSWT_M_Check).Concat(dbAPSWT_M_LIQ).Concat(dbAPSWT_M_NC).OrderBy(x => x.Payee);


            foreach (var i in vendorMasterIDList)
            {
                string payee = "";
                string tin = "";
                string atc = "";
                string noip = "";

                foreach (var j in taxRatesList)
                {
                    //NATURE OF INCOME PAYMENT
                    decimal aoipTotal = 0;
                    //AMOUNT OF TAX WITHHELD
                    decimal aotwTotal = 0;
                    foreach (var k in dbAPSWT_Conc.Where(x => x.Vendor_masterID == i && x.RateOfTax == j))
                    {
                        aoipTotal = aoipTotal + k.AOIP;
                        aotwTotal = aotwTotal + k.AOTW;

                        payee = k.Payee;
                        tin = k.Tin;
                        atc = k.ATC;
                        noip = k.NOIP;
                    }
                    if (aoipTotal != 0 && aotwTotal != 0)
                    {
                        finalOutput.Add(new HomeReportOutputAPSWT_MModel
                        {
                            Payee = payee,
                            Tin = tin,
                            ATC = atc,
                            NOIP = noip,
                            AOIP = aoipTotal,
                            RateOfTax = j,
                            AOTW = aotwTotal
                        });
                    }
                }
            }

            return finalOutput.OrderBy(x => x.Payee).ThenBy(x => x.RateOfTax);
        }

        public IEnumerable<HomeReportOutputAST1000Model> GetAST1000_Data(HomeReportViewModel model)
        {
            string format = "yyyy-M";
            DateTime startDT = new DateTime();
            DateTime endDT = new DateTime();
            if (model.PeriodOption == 1)
            {
                startDT = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
                endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }
            else
            {
                startDT = model.PeriodFrom;
                endDT = model.PeriodTo;
            }

            List<HomeReportOutputAST1000Model> dbAST1000 = new List<HomeReportOutputAST1000Model>();
            List<HomeReportOutputAST1000Model> dbAST1000_Check = new List<HomeReportOutputAST1000Model>();
            List<HomeReportOutputAST1000Model> dbAST1000_LIQ = new List<HomeReportOutputAST1000Model>();
            List<HomeReportOutputAST1000Model> dbAST1000_NC = new List<HomeReportOutputAST1000Model>();
            List<HomeReportOutputAST1000Model> finalOutput = new List<HomeReportOutputAST1000Model>();

            var vendorMasterIDList = _context.DMVendor.OrderBy(x => x.Vendor_MasterID).Select(x => x.Vendor_MasterID).Distinct();
            var taxRatesList = _context.DMTR.OrderBy(x => x.TR_Tax_Rate).Select(x => x.TR_Tax_Rate).Distinct();
            var vatList = _context.DMVAT.ToList();

            //Get data from Taxable expense table except cash advance(SS) and Check Payment.
            dbAST1000 = (from expEntryDetl in _context.ExpenseEntryDetails
                         join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                         join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                         join vend in _context.DMVendor on expEntryDetl.ExpDtl_Ewt_Payor_Name_ID equals vend.Vendor_ID
                         where status.Contains(expense.Expense_Status)
                         && model.TaxRateList.Contains(tr.TR_Tax_Rate)
                         && startDT.Date <= expense.Expense_Last_Updated.Date
                         && expense.Expense_Last_Updated.Date <= endDT.Date
                         && expense.Expense_Type != GlobalSystemValues.TYPE_SS
                         && expense.Expense_Type != GlobalSystemValues.TYPE_CV
                         orderby expense.Expense_Last_Updated
                         select new HomeReportOutputAST1000Model
                         {
                             SupplierName = vend.Vendor_Name,
                             Tin = vend.Vendor_TIN,
                             ATC = tr.TR_ATC,
                             NOIP = tr.TR_Nature_Income_Payment,
                             //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                             //Example: 45,000 / 1.12 = 40,178.57
                             TaxBase = (expEntryDetl.ExpDtl_Vat != 0) ? (expEntryDetl.ExpDtl_Debit / (decimal)(1 + vatList
                                    .Where(x => x.VAT_ID == expEntryDetl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : expEntryDetl.ExpDtl_Debit,
                             RateOfTax = tr.TR_Tax_Rate,
                             AOTW = expEntryDetl.ExpDtl_Credit_Ewt,
                             Vendor_masterID = vend.Vendor_MasterID
                         }).ToList();

            //Get data from Taxable expense table for Check Payment.
            dbAST1000_Check = (from expEntryDetl in _context.ExpenseEntryDetails
                               join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                               join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                               join vend in _context.DMVendor on expense.Expense_Payee equals vend.Vendor_ID
                               where status.Contains(expense.Expense_Status)
                               && model.TaxRateList.Contains(tr.TR_Tax_Rate)
                               && startDT.Date <= expense.Expense_Last_Updated.Date
                               && expense.Expense_Last_Updated.Date <= endDT.Date
                               && expense.Expense_Type == GlobalSystemValues.TYPE_CV
                               && expense.Expense_Payee_Type == GlobalSystemValues.PAYEETYPE_VENDOR
                               orderby expense.Expense_Last_Updated
                               select new HomeReportOutputAST1000Model
                               {
                                   SupplierName = vend.Vendor_Name,
                                   Tin = vend.Vendor_TIN,
                                   ATC = tr.TR_ATC,
                                   NOIP = tr.TR_Nature_Income_Payment,
                                   //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                                   //Example: 45,000 / 1.12 = 40,178.57
                                   TaxBase = (expEntryDetl.ExpDtl_Vat != 0) ? (expEntryDetl.ExpDtl_Debit / (decimal)(1 + vatList
                                          .Where(x => x.VAT_ID == expEntryDetl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : expEntryDetl.ExpDtl_Debit,
                                   RateOfTax = tr.TR_Tax_Rate,
                                   AOTW = expEntryDetl.ExpDtl_Credit_Ewt,
                                   Vendor_masterID = vend.Vendor_MasterID
                               }).ToList();

            //Get data from Taxable liquidation table.
            dbAST1000_LIQ = (from ie in _context.LiquidationInterEntity
                             join expDtl in _context.ExpenseEntryDetails on ie.ExpenseEntryDetailModel.ExpDtl_ID equals expDtl.ExpDtl_ID
                             join liqDtl in _context.LiquidationEntryDetails on expDtl.ExpenseEntryModel.Expense_ID equals liqDtl.ExpenseEntryModel.Expense_ID
                             join tr in _context.DMTR on ie.Liq_TaxRate equals tr.TR_ID
                             join vend in _context.DMVendor on ie.Liq_VendorID equals vend.Vendor_ID
                             where status.Contains(liqDtl.Liq_Status)
                             && model.TaxRateList.Contains(tr.TR_Tax_Rate)
                             && startDT.Date <= liqDtl.Liq_LastUpdated_Date
                             && liqDtl.Liq_LastUpdated_Date <= endDT.Date
                             select new HomeReportOutputAST1000Model
                             {
                                 SupplierName = vend.Vendor_Name,
                                 Tin = vend.Vendor_TIN,
                                 ATC = tr.TR_ATC,
                                 NOIP = tr.TR_Nature_Income_Payment,
                                 //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                                 //Example: 45,000 / 1.12 = 40,178.57
                                 TaxBase = (expDtl.ExpDtl_Vat != 0) ? ((ie.Liq_Amount_2_1 + ie.Liq_Amount_2_2 + ie.Liq_Amount_3_1) / (decimal)(1 + vatList
                                    .Where(x => x.VAT_ID == expDtl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : (ie.Liq_Amount_2_1 + ie.Liq_Amount_2_2 + ie.Liq_Amount_3_1),
                                 RateOfTax = tr.TR_Tax_Rate,
                                 AOTW = ie.Liq_Amount_2_2,
                                 Last_Update_Date = liqDtl.Liq_LastUpdated_Date,
                                 Vendor_masterID = vend.Vendor_MasterID
                             }).ToList();

            //Get data from Taxable Non-cash  table.
            dbAST1000_NC = (from ncDtl in _context.ExpenseEntryNonCashDetails
                            join ncAcc in _context.ExpenseEntryNonCashDetailAccounts on ncDtl.ExpNCDtl_ID equals ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID
                            join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
                            join exp in _context.ExpenseEntry on nc.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                            join tr in _context.DMTR on ncDtl.ExpNCDtl_TR_ID equals tr.TR_ID
                            join vend in _context.DMVendor on ncDtl.ExpNCDtl_Vendor_ID equals vend.Vendor_ID
                            join ncAcc2 in (from ncDtl2 in _context.ExpenseEntryNonCashDetails
                                            join ncAcc2 in _context.ExpenseEntryNonCashDetailAccounts on ncDtl2.ExpNCDtl_ID equals ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID
                                            where ncAcc2.ExpNCDtlAcc_Type_ID == 3
                                            select new { ncAcc2.ExpNCDtlAcc_Amount, ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID }) on ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID equals ncAcc2.ExpNCDtl_ID
                            where status.Contains(exp.Expense_Status)
                            && ncAcc.ExpNCDtlAcc_Type_ID == 3
                            && model.TaxRateList.Contains(tr.TR_Tax_Rate)
                            && startDT.Date <= exp.Expense_Last_Updated.Date
                            && exp.Expense_Last_Updated.Date <= endDT.Date
                            select new HomeReportOutputAST1000Model
                            {
                                SupplierName = vend.Vendor_Name,
                                Tin = vend.Vendor_TIN,
                                RateOfTax = tr.TR_Tax_Rate,
                                ATC = tr.TR_ATC,
                                NOIP = tr.TR_Nature_Income_Payment,
                                TaxBase = (ncDtl.ExpNCDtl_TaxBasedAmt != 0) ? ncDtl.ExpNCDtl_TaxBasedAmt : ncAcc.ExpNCDtlAcc_Amount,
                                AOTW = ncAcc2.ExpNCDtlAcc_Amount,
                                Last_Update_Date = exp.Expense_Last_Updated,
                                Vendor_masterID = vend.Vendor_MasterID
                            }).Where(x => x.AOTW > 0).ToList();

            var dbAPSWT_Conc = dbAST1000.Concat(dbAST1000_Check).Concat(dbAST1000_LIQ).Concat(dbAST1000_NC).OrderBy(x => x.SupplierName);

            foreach (var i in vendorMasterIDList)
            {
                string payee = "";
                string tin = "";
                string atc = "";
                string noip = "";

                foreach (var j in taxRatesList)
                {
                    //NATURE OF INCOME PAYMENT
                    decimal aoipTotal = 0;
                    //AMOUNT OF TAX WITHHELD
                    decimal aotwTotal = 0;
                    foreach (var k in dbAPSWT_Conc.Where(x => x.Vendor_masterID == i && x.RateOfTax == j))
                    {
                        aoipTotal = aoipTotal + k.TaxBase;
                        aotwTotal = aotwTotal + k.AOTW;

                        payee = k.SupplierName;
                        tin = k.Tin;
                        atc = k.ATC;
                        noip = k.NOIP;
                    }
                    if (aoipTotal != 0 && aotwTotal != 0)
                    {
                        finalOutput.Add(new HomeReportOutputAST1000Model
                        {
                            SupplierName = payee,
                            Tin = tin,
                            ATC = atc,
                            NOIP = noip,
                            TaxBase = aoipTotal,
                            RateOfTax = j,
                            AOTW = aotwTotal
                        });
                    }
                }
            }

            return finalOutput.OrderBy(x => x.SupplierName).ThenBy(x => x.RateOfTax);
        }

        public ReportLOIViewModel GetLOIData(HomeReportViewModel model)
        {
            List<LOIAccount> accs = new List<LOIAccount>();
            decimal totalAmount = 0;
            string stringNum = "";
            List<string> voucherNoList = new List<string>();
            List<int> entryIDs = new List<int>();
            if (model.VoucherArray != null)
            {
                voucherNoList = model.VoucherArray.Split(',').ToList();
                entryIDs = voucherNoList.Select(x => int.Parse(x)).ToList();
                //sort list
                entryIDs.Sort();
                model.VoucherNoList = PopulateVoucherNo(entryIDs);

                //get BDO MNL Master ID from XML
                var bdoAcc = getAccountByMasterID(int.Parse(xelemAcc.Element("C_DDV2").Value));

                var entryList = (from e in _context.ExpenseEntry
                                 join emp in _context.DMEmp on e.Expense_Payee equals emp.Emp_ID
                                 where entryIDs.Contains(e.Expense_ID)
                                 select new
                                 {
                                     e,
                                     emp,
                                     ExpenseEntryDetails = from d
                                                           in _context.ExpenseEntryDetails
                                                           where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                           select new
                                                           {
                                                               d,
                                                               ExpenseEntryGbaseDtls = from g
                                                                                       in _context.ExpenseEntryGbaseDtls
                                                                                       where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                       select g,
                                                               ExpenseEntryInterEntity = from a
                                                                                           in _context.ExpenseEntryInterEntity
                                                                                         where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                         select new
                                                                                         {
                                                                                             a,
                                                                                             ExpenseEntryInterEntityParticular = from p
                                                                                                                                 in _context.ExpenseEntryInterEntityParticular
                                                                                                                                 where p.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == a.ExpDtl_DDVInter_ID
                                                                                                                                 select new
                                                                                                                                 {
                                                                                                                                     p,
                                                                                                                                     ExpenseEntryEntityAccounts = from acc
                                                                                                                                                                  in _context.ExpenseEntryInterEntityAccs
                                                                                                                                                                  where acc.ExpenseEntryInterEntityParticular.InterPart_ID == p.InterPart_ID
                                                                                                                                                                  select acc
                                                                                                                                 }
                                                                                         }
                                                           }
                                 }).ToList();
                foreach (var ent in entryList)
                {
                    decimal amt = 0;
                    foreach (var dtl in ent.ExpenseEntryDetails)
                    {
                        if (dtl.d.ExpDtl_Inter_Entity)
                        {
                            foreach (var inter in dtl.ExpenseEntryInterEntity)
                            {
                                foreach (var part in inter.ExpenseEntryInterEntityParticular)
                                {
                                    foreach (var acc in part.ExpenseEntryEntityAccounts)
                                    {
                                        if (bdoAcc.Account_ID == acc.InterAcc_Acc_ID)
                                        {
                                            amt += acc.InterAcc_Amount;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //gets Amount when Gbase Remarks and w/ EWT
                            amt += dtl.d.ExpDtl_Credit_Cash;
                        }
                    }
                    accs.Add(
                       new LOIAccount
                       {
                           loi_Emp_Name = ent.emp.Emp_Name,
                           loi_Acc_Type = ent.emp.Emp_Acc_No.Substring(0, 2),
                           loi_Acc_No = ent.emp.Emp_Acc_No.Substring(2, ent.emp.Emp_Acc_No.Length - 2),
                           loi_Amount = amt
                       }
                    );
                    totalAmount += amt;
                }
                stringNum = _class.decimalNumberToWords(totalAmount);
            }
            return new ReportLOIViewModel()
            {
                Rep_DDVNoList = model.VoucherNoList != null ? model.VoucherNoList.Select(x => x.vchr_No).ToList() : voucherNoList,
                Rep_Amount = (decimal)totalAmount,
                Rep_AmountInString = stringNum,
                Rep_LOIAccList = accs,
                Rep_LOIEntryIDList = entryIDs,
                Rep_Approver_Name = (model.SignatoryID > 0) ? getBCSName(model.SignatoryID) : "",
                Rep_Verifier1_Name = (model.SignatoryIDVerifier > 0) ? getBCSName(model.SignatoryIDVerifier) : "",
                //Rep_Verifier2_Name = "",
                Rep_String1 = "This authority to debit and credit is issued pursuant to and subject to the terms and conditions of the Company's",
                Rep_String2 = "Regular Payroll Agreement with the Bank.",
                Rep_String3 = "Thank you and best regards.",
                Rep_String4 = "Very truly yours, "
            };
        }

        public List<RepAmortViewModel> GetPrepaidAmortSchedule(HomeReportViewModel model)
        {
            List<RepAmortViewModel> repAmortVMs = new List<RepAmortViewModel>();
            List<AmortSched> amorts = new List<AmortSched>();

            if (model.VoucherNo != "null")
            {
                var entryList = (from ent in _context.ExpenseEntry
                                 from dtls in _context.ExpenseEntryDetails
                                 where ent.Expense_ID == dtls.ExpenseEntryModel.Expense_ID
                                 && dtls.ExpenseEntryAmortizations.Count > 0
                                 && ent.Expense_ID == int.Parse(model.VoucherNo)
                                 select new
                                 {
                                     ent,
                                     ExpenseEntryDetails = from d
                                                          in _context.ExpenseEntryDetails
                                                           where d.ExpenseEntryModel.Expense_ID == ent.Expense_ID
                                                           select new
                                                           {
                                                               d,
                                                               ExpenseEntryAmortizations = from a
                                                                                           in _context.ExpenseEntryAmortizations
                                                                                           where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                           select a
                                                           }

                                 }).ToList();
                if (entryList.Count > 0)
                {
                    foreach (var list in entryList)
                    {
                        foreach (var dtl in list.ExpenseEntryDetails)
                        {
                            amorts = new List<AmortSched>();

                            foreach (var i in dtl.ExpenseEntryAmortizations)
                            {
                                amorts.Add(new AmortSched
                                {
                                    as_Amort_Name = i.Amor_Sched_Date.ToShortDateString(),
                                    as_Amount = i.Amor_Price
                                });
                            }

                            repAmortVMs.Add(new RepAmortViewModel()
                            {
                                PA_AmortScheds = amorts.OrderBy(x => DateTime.Parse(x.as_Amort_Name)).ToList(),
                                PA_VoucherNo = GlobalSystemValues.getApplicationCode(list.ent.Expense_Type) + "-" + list.ent.Expense_Date.Year + "-" + list.ent.Expense_Number.ToString().PadLeft(5, '0'),
                                PA_CheckNo = list.ent.Expense_CheckNo,
                                PA_RefNo = "",
                                PA_Value_Date = list.ent.Expense_Last_Updated,
                                PA_Section = "10",
                                PA_Remarks = dtl.d.ExpDtl_Gbase_Remarks,
                                PA_Vendor_Name = getVendorName(list.ent.Expense_Payee, list.ent.Expense_Payee_Type),
                                PA_Total_Amt = dtl.d.ExpDtl_Debit,
                                PA_Day = dtl.d.ExpDtl_Amor_Day,
                                PA_Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dtl.d.ExpDtl_Amor_Month + 1),
                                PA_No_Of_Months = dtl.d.ExpDtl_Amor_Duration
                            });
                        }
                    };
                }
            }
            else
            {
                repAmortVMs.Add(new RepAmortViewModel()
                {
                    PA_AmortScheds = amorts,
                    PA_VoucherNo = "",
                    PA_CheckNo = "",
                    PA_RefNo = "",
                    PA_Value_Date = DateTime.Now,
                    PA_Section = "10",
                    PA_Remarks = "",
                    PA_Vendor_Name = "",
                    PA_Total_Amt = 0,
                    PA_Day = 0,
                    PA_Month = "",
                    PA_No_Of_Months = 0
                });
            }
            return repAmortVMs;
        }

        public IEnumerable<HomeReportActualBudgetModel> GetActualReportData(int filterMonth, int filterYear)
        {
            List<HomeReportActualBudgetModel> actualBudgetData = new List<HomeReportActualBudgetModel>();
            List<AccGroupBudgetModel> accountCategory = new List<AccGroupBudgetModel>();
            DateTime startOfTerm = GetSelectedYearMonthOfTerm(filterMonth, filterYear);
            DateTime startDT;
            DateTime endDT;
            DateTime StartFiscal = GetStartOfFiscal(filterMonth, filterYear, true);
            DateTime EndFiscal = GetStartOfFiscal(filterMonth, filterYear, false);
            int termYear = startOfTerm.Year;
            int termMonth = startOfTerm.Month;
            decimal budgetBalance;
            decimal totalExpenseThisTermToPrevMonthend;
            decimal subTotal;
            string format = "yyyy-M";

            endDT = DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);

            //Get latest Budget of fiscal year of selected month/year of each account
            var accountList = _context.DMAccount.Where(x => x.Account_isActive == true && x.Account_isDeleted == false
                                                        && x.Account_Fund == true && x.Account_Group_MasterID != 0).OrderBy(x => x.Account_Group_MasterID);
            var accountGrpList = _context.DMAccountGroup.Where(x => x.AccountGroup_isActive == true && x.AccountGroup_isDeleted == false);
            var budgetList = _context.Budget.Where(x => x.Budget_Date_Registered.Date <= EndFiscal.Date)
                                                        .OrderByDescending(x => x.Budget_Date_Registered);
            int currGroup = accountList.First().Account_Group_MasterID;
            decimal budgetAmount = 0.0M;
            var transList = _context.ExpenseTransLists.ToList();

            if (budgetList.Count() == 0M)
            {
                actualBudgetData.Add(new HomeReportActualBudgetModel
                {
                    BudgetBalance = 0.0M,
                    ExpenseAmount = 0.0M,
                    Remarks = "NO_RECORD",
                    ValueDate = DateTime.Parse("1991/01/01 12:00:00")
                });
                return actualBudgetData;
            }
            int lastCnt = accountList.Count();
            int cnt = 0;
            foreach (var i in accountList)
            {
                cnt++;
                var budget = budgetList.Where(x => x.Budget_Account_MasterID == i.Account_MasterID
                                            && x.Budget_IsActive == true && x.Budget_isDeleted == false)
                    .DefaultIfEmpty(new BudgetModel
                    {
                        Budget_Account_MasterID = i.Account_MasterID,
                        Budget_Amount = 0.0M
                    }).OrderByDescending(x => x.Budget_Date_Registered).First();

                var accgrp = accountGrpList.Where(x => x.AccountGroup_MasterID == currGroup).FirstOrDefault();

                if (currGroup == i.Account_Group_MasterID)
                {
                    budgetAmount += budget.Budget_Amount;
                }
                else
                {
                    if (accgrp == null)
                    {
                        currGroup = i.Account_Group_MasterID;
                        continue;
                    }
                    accountCategory.Add(new AccGroupBudgetModel
                    {
                        StartOfTerm = startOfTerm,
                        AccountGroupName = accgrp.AccountGroup_Name,
                        AccountGroupMasterID = currGroup,
                        Remarks = "Budget Amount - This Term",
                        Budget = budgetAmount
                    });

                    budgetAmount = budget.Budget_Amount;
                    currGroup = i.Account_Group_MasterID;
                }
                //Add last account category
                if (cnt == lastCnt)
                {
                    accgrp = accountGrpList.Where(x => x.AccountGroup_MasterID == currGroup).FirstOrDefault();
                    accountCategory.Add(new AccGroupBudgetModel
                    {
                        StartOfTerm = startOfTerm,
                        AccountGroupName = accgrp.AccountGroup_Name,
                        AccountGroupMasterID = currGroup,
                        Remarks = "Budget Amount - This Term",
                        Budget = budgetAmount
                    });
                }
            }

            //Get all expenses with in the term of selected month/year
            var GOExpHist = _context.GOExpressHist.Where(x => startOfTerm <= DateTime.Parse(x.GOExpHist_ValueDate.Substring(0, 2) + "/" + x.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(x.GOExpHist_ValueDate.Substring(4, 2))))
                            && DateTime.Parse(x.GOExpHist_ValueDate.Substring(0, 2) + "/" + x.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(x.GOExpHist_ValueDate.Substring(4, 2)))) <= endDT).ToList();

            //Get total expenses of each Account GROUP
            //Flow:
            //1. Loop account filtered by account group
            //1.1. Get all expenses from GOExpressHist table from start of term until Prev monthend of selected month/year
            //1.2. Loop #1 section until all account.
            //1.3. Subtract the total expenses amount of start of term until Prev monthend of selected month/year then add to the List()
            //2. Loop account filtered by account group for expenses of selected month/year
            //2.1. Get all expenses from GOExpressHist table of selected month/year
            //2.2. Subtract the expenses amount of each budget account. at the same time, add to sub-total.
            //2.3. Loop #2 section until all account.
            //3. Add sub-total to the list.
            //4. Add break to the list.
            //5. Loop Flow until all Account Group.

            foreach (var category in accountCategory)
            {
                startDT = DateTime.ParseExact(termYear + "-" + termMonth, format, CultureInfo.InvariantCulture);
                endDT = DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture);
                subTotal = 0.00M;
                bool subTotalFlag = false;
                totalExpenseThisTermToPrevMonthend = 0.00M;

                budgetBalance = category.Budget;

                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = category.AccountGroupName,
                    BudgetBalance = budgetBalance,
                    Remarks = category.Remarks,
                    ValueDate = category.StartOfTerm
                });

                //#1
                //Get total expenses of each account from start of term to Prev monthend of selected month, year
                foreach (var acc in accountList.Where(x => x.Account_Group_MasterID == category.AccountGroupMasterID))
                {
                    foreach (var hist in GOExpHist.Where(x => startOfTerm <= DateTime.Parse(x.GOExpHist_ValueDate.Substring(0, 2) + "/" + x.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(x.GOExpHist_ValueDate.Substring(4, 2))))
                             && DateTime.Parse(x.GOExpHist_ValueDate.Substring(0, 2) + "/" + x.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(x.GOExpHist_ValueDate.Substring(4, 2)))) <= endDT.AddDays(-1).Date))
                    {
                        var transData = transList.Where(x => x.TL_GoExpHist_ID == hist.GOExpHist_Id).FirstOrDefault();
                        if (transList != null)
                        {
                            if (transData.TL_StatusID != GlobalSystemValues.STATUS_RESENDING_COMPLETE
                                && transData.TL_StatusID != GlobalSystemValues.STATUS_APPROVED
                                && transData.TL_StatusID != GlobalSystemValues.STATUS_REVERSING_COMPLETE) continue;
                        }

                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry11ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry11ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry11ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry11Actcde)
                        {
                            if (hist.GOExpHist_Entry11Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry11Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry11Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry12ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry12ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry12ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry12Actcde)
                        {
                            if (hist.GOExpHist_Entry12Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry12Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry12Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry21ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry21ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry21ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry21Actcde)
                        {
                            if (hist.GOExpHist_Entry21Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry21Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry21Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry22ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry22ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry22ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry22Actcde)
                        {
                            if (hist.GOExpHist_Entry22Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry22Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry22Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry31ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry31ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry31ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry31Actcde)
                        {
                            if (hist.GOExpHist_Entry31Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry31Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry31Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry32ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry32ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry32ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry32Actcde)
                        {
                            if (hist.GOExpHist_Entry32Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry32Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry32Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry41ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry41ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry41ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry41Actcde)
                        {
                            if (hist.GOExpHist_Entry41Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry41Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry41Amt);
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry42ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry42ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry42ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry42Actcde)
                        {
                            if (hist.GOExpHist_Entry42Type == "D")
                            {
                                totalExpenseThisTermToPrevMonthend += Decimal.Parse(hist.GOExpHist_Entry42Amt);
                            }
                            else
                            {
                                totalExpenseThisTermToPrevMonthend -= Decimal.Parse(hist.GOExpHist_Entry42Amt);
                            }
                        }
                    }
                }
                budgetBalance -= totalExpenseThisTermToPrevMonthend;

                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    BudgetBalance = budgetBalance,
                    ExpenseAmount = totalExpenseThisTermToPrevMonthend,
                    Remarks = "Total Expenses - This Term to Prev Monthend",
                    ValueDate = endDT.AddDays(-1)
                });

                var deptInfo = (from dtl in _context.ExpenseEntryDetails
                                join exp in _context.ExpenseEntry on dtl.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                join dept in _context.DMDept on dtl.ExpDtl_Dept equals dept.Dept_ID
                                where exp.Expense_Last_Updated.Month == filterMonth
                                   && exp.Expense_Last_Updated.Year == filterYear
                                select new
                                {
                                    exp.Expense_ID,
                                    dtl.ExpDtl_ID,
                                    dept.Dept_ID,
                                    dept.Dept_Name
                                });


                //#2
                foreach (var acc in accountList.Where(x => x.Account_Group_MasterID == category.AccountGroupMasterID))
                {
                    foreach (var hist in GOExpHist.Where(x => x.GOExpHist_ValueDate.Substring(0, 2) == filterMonth.ToString().ToString().PadLeft(2, '0')
                                                        && (2000 + int.Parse(x.GOExpHist_ValueDate.Substring(4, 2))) == filterYear))
                    {
                        var transData = transList.Where(x => x.TL_GoExpHist_ID == hist.GOExpHist_Id).FirstOrDefault();
                        if (transList != null)
                        {
                            if (transData.TL_StatusID != GlobalSystemValues.STATUS_RESENDING_COMPLETE
                                && transData.TL_StatusID != GlobalSystemValues.STATUS_APPROVED
                                && transData.TL_StatusID != GlobalSystemValues.STATUS_REVERSING_COMPLETE) continue;
                        }

                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry11ActNo)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry11ActType)
                            && acc.Account_No.Contains(hist.GOExpHist_Entry11ActNo)
                            && acc.Account_Code == hist.GOExpHist_Entry11Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry11Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry11Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry11Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry11Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry11Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry11Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry11Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry12ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry12ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry12ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry12Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry12Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry12Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry12Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry12Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry12Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry12Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry12Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry21ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry21ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry21ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry21Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry21Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry21Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry21Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry21Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry21Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry21Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry21Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry22ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry22ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry22ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry22Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry22Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry22Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry22Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry22Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry22Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry22Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry22Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry31ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry31ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry31ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry31Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry31Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry31Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry31Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry31Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry31Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry31Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry31Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry32ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry32ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry32ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry32Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry32Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry32Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry32Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry32Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry32Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry32Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry32Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry41ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry41ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry41ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry41Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry41Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry41Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry41Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry41Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry41Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry41Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry41Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                        if (!String.IsNullOrEmpty(hist.GOExpHist_Entry42ActNo)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry42ActType)
                        && acc.Account_No.Contains(hist.GOExpHist_Entry42ActNo)
                        && acc.Account_Code == hist.GOExpHist_Entry42Actcde)
                        {
                            subTotalFlag = true;
                            if (hist.GOExpHist_Entry42Type == "D")
                            {
                                budgetBalance -= Decimal.Parse(hist.GOExpHist_Entry42Amt);
                                subTotal += Decimal.Parse(hist.GOExpHist_Entry42Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry42Amt),
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = (deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).Count() > 0) ? deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).FirstOrDefault().Dept_Name : "",
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                            else
                            {
                                budgetBalance += Decimal.Parse(hist.GOExpHist_Entry42Amt);
                                subTotal -= Decimal.Parse(hist.GOExpHist_Entry42Amt);

                                actualBudgetData.Add(new HomeReportActualBudgetModel()
                                {
                                    BudgetBalance = budgetBalance,
                                    ExpenseAmount = Decimal.Parse(hist.GOExpHist_Entry42Amt) * -1, //Credit convert to negative value
                                    Remarks = hist.GOExpHist_Remarks,
                                    Department = deptInfo.Where(x => x.ExpDtl_ID == hist.ExpenseDetailID).DefaultIfEmpty("").FirstOrDefault().Dept_Name,
                                    ValueDate = DateTime.Parse(hist.GOExpHist_ValueDate.Substring(0, 2) + "/" + hist.GOExpHist_ValueDate.Substring(2, 2) + "/" + (2000 + int.Parse(hist.GOExpHist_ValueDate.Substring(4, 2))))
                                });
                            }
                        }
                    }
                }

                //#3
                //Add Sub-Total to List
                if (subTotalFlag == true)
                {
                    actualBudgetData.Add(new HomeReportActualBudgetModel()
                    {
                        BudgetBalance = budgetBalance,
                        ExpenseAmount = subTotal,
                        Remarks = "Sub-total",
                        ValueDate = endDT.AddMonths(1).AddDays(-1)
                    });
                }

                //#4
                //Insert break or seperation of row
                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = "BREAK"
                });

            }

            return actualBudgetData;
        }

        public IEnumerable<HomeReportTransactionListViewModel> GetTransactionData(HomeReportViewModel model)
        {
            string whereQuery = "";
            string whereQuery1 = "";
            string whereQuery2 = "";
            string whereQuery3 = "";

            int[] expType1 = { GlobalSystemValues.TYPE_CV, GlobalSystemValues.TYPE_PC,
                        GlobalSystemValues.TYPE_DDV, GlobalSystemValues.TYPE_SS };
            int[] expType2 = { GlobalSystemValues.NC_LS_PAYROLL,
                                GlobalSystemValues.NC_TAX_REMITTANCE,
                                GlobalSystemValues.NC_MONTHLY_ROSS_BILL,
                                GlobalSystemValues.NC_PSSC,
                                GlobalSystemValues.NC_PCHC,
                                GlobalSystemValues.NC_DEPRECIATION,
                                GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT,
                                GlobalSystemValues.NC_JS_PAYROLL,
                                GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL,
                                GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES};
            int[] expType3 = { GlobalSystemValues.TYPE_SS };
            List<HomeReportTransactionListViewModel> list1 = new List<HomeReportTransactionListViewModel>();
            List<HomeReportTransactionListViewModel> list2 = new List<HomeReportTransactionListViewModel>();
            List<HomeReportTransactionListViewModel> list3 = new List<HomeReportTransactionListViewModel>();

            List<DMAccountModel> accList = getAccountListIncHist();

            if (model.CheckNoFrom > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@5 <= int.Parse(Expense_CheckNo)";
            }
            if (model.CheckNoTo > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "int.Parse(Expense_CheckNo) <= @6";
            }
            if (model.VoucherNoFrom > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@7 <= Expense_Number";
            }
            if (model.VoucherNoTo > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "Expense_Number <= @8";
            }
            if (model.TransNoFrom > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@9 <= TL_TransID";
            }
            if (model.TransNoTo > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "TL_TransID <= @10";
            }
            if (!String.IsNullOrEmpty(model.SubjName))
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "GOExpHist_Remarks.Contains(@11)";
            }

            if (model.ReportSubType == 0 || model.ReportSubType == GlobalSystemValues.TYPE_CV ||
                model.ReportSubType == GlobalSystemValues.TYPE_PC || model.ReportSubType == GlobalSystemValues.TYPE_DDV ||
                model.ReportSubType == GlobalSystemValues.TYPE_SS || model.ReportSubType == HomeReportConstantValue.REP_LIQUIDATION)
            {
                DateTime startDT = DateTime.ParseExact(model.Year + "-" + model.Month, "yyyy-M", CultureInfo.InvariantCulture);
                DateTime endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, "yyyy-M", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
                int subType = 0;

                if (model.ReportSubType == HomeReportConstantValue.REP_LIQUIDATION)
                {
                    subType = GlobalSystemValues.TYPE_SS;
                }
                else
                {
                    subType = model.ReportSubType;
                }
                //Get DDV entry detail list. include inter entity
                List<EntryDDVViewModel> ddvDetails = GetEntryDetailsListForDDV();

                if (model.ReportSubType != 0)
                {
                    if (!String.IsNullOrEmpty(whereQuery))
                    {
                        whereQuery1 = "Expense_Type = @0 && " + whereQuery;
                    }
                    else
                    {
                        whereQuery1 = "Expense_Type = @0";
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(whereQuery))
                    {
                        whereQuery1 = "@12.Contains(Expense_Type) && " + whereQuery;
                    }
                    else
                    {
                        whereQuery1 = "@12.Contains(Expense_Type)";
                    }
                }

                if (model.PeriodOption == 1)
                {
                    if (whereQuery1 != "")
                        whereQuery1 += " && ";
                    whereQuery1 += "@1 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @2";
                }
                else if (model.PeriodOption == 3)
                {
                    if (whereQuery1 != "")
                        whereQuery1 += " && ";
                    whereQuery1 += "@3 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @4";
                }

                var db1 = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           join expDtl in _context.ExpenseEntryDetails on hist.ExpenseDetailID equals expDtl.ExpDtl_ID
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               expDtl.ExpDtl_Account,
                               expDtl.ExpDtl_CreditAccount1,
                               expDtl.ExpDtl_CreditAccount2,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID,
                               hist.GOExpHist_Id,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_ReferenceNo,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Section,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Ccy,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Cust,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry11ExchRate,
                               hist.GOExpHist_Entry11ExchCcy,
                               hist.GOExpHist_Entry11Fund,
                               hist.GOExpHist_Entry11AdvcPrnt,
                               hist.GOExpHist_Entry11Details,
                               hist.GOExpHist_Entry11Entity,
                               hist.GOExpHist_Entry11Division,
                               hist.GOExpHist_Entry11InterAmt,
                               hist.GOExpHist_Entry11InterRate,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Ccy,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Cust,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry12ExchRate,
                               hist.GOExpHist_Entry12ExchCcy,
                               hist.GOExpHist_Entry12Fund,
                               hist.GOExpHist_Entry12AdvcPrnt,
                               hist.GOExpHist_Entry12Details,
                               hist.GOExpHist_Entry12Entity,
                               hist.GOExpHist_Entry12Division,
                               hist.GOExpHist_Entry12InterAmt,
                               hist.GOExpHist_Entry12InterRate,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Ccy,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Cust,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry21ExchRate,
                               hist.GOExpHist_Entry21ExchCcy,
                               hist.GOExpHist_Entry21Fund,
                               hist.GOExpHist_Entry21AdvcPrnt,
                               hist.GOExpHist_Entry21Details,
                               hist.GOExpHist_Entry21Entity,
                               hist.GOExpHist_Entry21Division,
                               hist.GOExpHist_Entry21InterAmt,
                               hist.GOExpHist_Entry21InterRate,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Ccy,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Cust,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry22ExchRate,
                               hist.GOExpHist_Entry22ExchCcy,
                               hist.GOExpHist_Entry22Fund,
                               hist.GOExpHist_Entry22AdvcPrnt,
                               hist.GOExpHist_Entry22Details,
                               hist.GOExpHist_Entry22Entity,
                               hist.GOExpHist_Entry22Division,
                               hist.GOExpHist_Entry22InterAmt,
                               hist.GOExpHist_Entry22InterRate,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Ccy,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Cust,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry31ExchRate,
                               hist.GOExpHist_Entry31ExchCcy,
                               hist.GOExpHist_Entry31Fund,
                               hist.GOExpHist_Entry31AdvcPrnt,
                               hist.GOExpHist_Entry31Details,
                               hist.GOExpHist_Entry31Entity,
                               hist.GOExpHist_Entry31Division,
                               hist.GOExpHist_Entry31InterAmt,
                               hist.GOExpHist_Entry31InterRate,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Ccy,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Cust,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry32ExchRate,
                               hist.GOExpHist_Entry32ExchCcy,
                               hist.GOExpHist_Entry32Fund,
                               hist.GOExpHist_Entry32AdvcPrnt,
                               hist.GOExpHist_Entry32Details,
                               hist.GOExpHist_Entry32Entity,
                               hist.GOExpHist_Entry32Division,
                               hist.GOExpHist_Entry32InterAmt,
                               hist.GOExpHist_Entry32InterRate,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Ccy,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Cust,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry41ExchRate,
                               hist.GOExpHist_Entry41ExchCcy,
                               hist.GOExpHist_Entry41Fund,
                               hist.GOExpHist_Entry41AdvcPrnt,
                               hist.GOExpHist_Entry41Details,
                               hist.GOExpHist_Entry41Entity,
                               hist.GOExpHist_Entry41Division,
                               hist.GOExpHist_Entry41InterAmt,
                               hist.GOExpHist_Entry41InterRate,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Ccy,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Cust,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               hist.GOExpHist_Entry42ExchRate,
                               hist.GOExpHist_Entry42ExchCcy,
                               hist.GOExpHist_Entry42Fund,
                               hist.GOExpHist_Entry42AdvcPrnt,
                               hist.GOExpHist_Entry42Details,
                               hist.GOExpHist_Entry42Entity,
                               hist.GOExpHist_Entry42Division,
                               hist.GOExpHist_Entry42InterAmt,
                               hist.GOExpHist_Entry42InterRate,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_StatusID
                           }).Where("@13.Contains(TL_StatusID) && " + whereQuery1, subType, startDT.Date, endDT.Date, model.PeriodFrom.Date,
                                   model.PeriodTo.Date, model.CheckNoFrom, model.CheckNoTo, model.VoucherNoFrom, model.VoucherNoTo,
                                   model.TransNoFrom, model.TransNoTo, model.SubjName, expType1, statusTrans).ToList();

                //Convert to List object.
                foreach (var i in db1)
                {
                    //Ignore Liquidation record if Filter is Cash Advance only
                    if (model.ReportSubType == GlobalSystemValues.TYPE_SS)
                    {
                        if (i.GOExpHist_Remarks == "S" + _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.ExpenseDetailID).FirstOrDefault().ExpDtl_Gbase_Remarks)
                        {
                            continue;
                        }
                    }
                    //Ignore Cash advance record if Filter is Liquidation only
                    if (model.ReportSubType == HomeReportConstantValue.REP_LIQUIDATION)
                    {
                        if (i.GOExpHist_Remarks != "S" + _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.ExpenseDetailID).FirstOrDefault().ExpDtl_Gbase_Remarks)
                        {
                            continue;
                        }
                    }

                    //Check record if liquidation or not.
                    bool boolLiq = _context.ExpenseTransLists.Where(x => x.TL_ID == i.TL_ID).Select(x => x.TL_Liquidation).FirstOrDefault();

                    if (boolLiq)
                    {
                        continue;
                    }

                    list1.Add(new HomeReportTransactionListViewModel
                    {
                        ExpExpense_ID = i.Expense_ID,
                        ExpExpense_Type = i.Expense_Type,
                        Trans_Last_Updated_Date = i.Expense_Last_Updated,
                        ExpExpense_Date = i.Expense_Date.ToString(),
                        Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, boolLiq),
                        Trans_Check_Number = i.Expense_CheckNo,
                        HistExpenseEntryID = i.ExpenseEntryID,
                        HistExpenseDetailID = i.ExpenseDetailID,
                        HistGOExpHist_Id = i.GOExpHist_Id,
                        Trans_Value_Date = i.GOExpHist_ValueDate,
                        Trans_Reference_No = i.GOExpHist_ReferenceNo,
                        Trans_Section = i.GOExpHist_Section,
                        Trans_Remarks = i.GOExpHist_Remarks,
                        Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                        Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                        Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                        Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                        Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                        Trans_Account_Name1_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, i.GOExpHist_Entry11Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                        Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                        Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                        Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                        Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                        Trans_Details1_1 = i.GOExpHist_Entry11Details,
                        Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                        Trans_Division1_1 = i.GOExpHist_Entry11Division,
                        Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                        Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                        Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                        Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                        Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                        Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                        Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                        Trans_Account_Name1_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, i.GOExpHist_Entry12Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                        Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                        Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                        Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                        Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                        Trans_Details1_2 = i.GOExpHist_Entry12Details,
                        Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                        Trans_Division1_2 = i.GOExpHist_Entry12Division,
                        Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                        Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                        Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                        Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                        Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                        Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                        Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                        Trans_Account_Name2_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, i.GOExpHist_Entry21Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                        Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                        Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                        Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                        Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                        Trans_Details2_1 = i.GOExpHist_Entry21Details,
                        Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                        Trans_Division2_1 = i.GOExpHist_Entry21Division,
                        Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                        Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                        Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                        Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                        Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                        Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                        Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                        Trans_Account_Name2_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, i.GOExpHist_Entry22Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                        Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                        Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                        Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                        Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                        Trans_Details2_2 = i.GOExpHist_Entry22Details,
                        Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                        Trans_Division2_2 = i.GOExpHist_Entry22Division,
                        Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                        Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                        Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                        Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                        Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                        Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                        Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                        Trans_Account_Name3_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, i.GOExpHist_Entry31Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                        Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                        Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                        Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                        Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                        Trans_Details3_1 = i.GOExpHist_Entry31Details,
                        Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                        Trans_Division3_1 = i.GOExpHist_Entry31Division,
                        Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                        Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                        Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                        Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                        Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                        Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                        Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                        Trans_Account_Name3_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, i.GOExpHist_Entry32Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                        Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                        Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                        Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                        Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                        Trans_Details3_2 = i.GOExpHist_Entry32Details,
                        Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                        Trans_Division3_2 = i.GOExpHist_Entry32Division,
                        Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                        Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                        Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                        Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                        Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                        Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                        Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                        Trans_Account_Name4_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, i.GOExpHist_Entry41Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                        Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                        Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                        Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                        Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                        Trans_Details4_1 = i.GOExpHist_Entry41Details,
                        Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                        Trans_Division4_1 = i.GOExpHist_Entry41Division,
                        Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                        Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                        Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                        Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                        Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                        Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                        Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                        Trans_Account_Name4_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, i.GOExpHist_Entry42Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                        Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                        Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                        Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                        Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                        Trans_Details4_2 = i.GOExpHist_Entry42Details,
                        Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                        Trans_Division4_2 = i.GOExpHist_Entry42Division,
                        Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                        Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                        TransTL_ID = i.TL_ID,
                        TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                        TransTL_TransID = i.TL_TransID

                    });
                }
            }

            if (model.ReportSubType == 0 || model.ReportSubType == HomeReportConstantValue.REP_NC_LS_PAYROLL ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_TAX_REMITTANCE ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_MONTHLY_ROSS_BILL ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_PSSC ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_PCHC ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_DEPRECIATION ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_PETTY_CASH_REPLENISHMENT ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_JS_PAYROLL ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_RETURN_OF_JS_PAYROLL ||
                model.ReportSubType == HomeReportConstantValue.REP_NC_MISCELLANEOUS_ENTRIES
                )
            {
                DateTime startDT = DateTime.ParseExact(model.Year + "-" + model.Month, "yyyy-M", CultureInfo.InvariantCulture);
                DateTime endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, "yyyy-M", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
                List<ExpenseEntryNCDtlViewModel> ncDtlList = GetEntryDetailAccountListForNonCash();

                if (model.ReportSubType != 0)
                {
                    if (!String.IsNullOrEmpty(whereQuery))
                    {
                        whereQuery2 = "ExpNC_Category_ID = (@0 - 50) && " + whereQuery;
                    }
                    else
                    {
                        whereQuery2 = "ExpNC_Category_ID = (@0 - 50)";
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(whereQuery))
                    {
                        whereQuery2 = "@12.Contains(ExpNC_Category_ID) && " + whereQuery;
                    }
                    else
                    {
                        whereQuery2 = "@12.Contains(ExpNC_Category_ID)";
                    }
                }
                if (model.PeriodOption == 1)
                {
                    if (whereQuery2 != "")
                        whereQuery2 += " && ";
                    whereQuery2 += "@1 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @2";
                }
                else if (model.PeriodOption == 3)
                {
                    if (whereQuery2 != "")
                        whereQuery2 += " && ";
                    whereQuery2 += "@3 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @4";
                }
                var db2 = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join ncDtl in _context.ExpenseEntryNonCashDetails on hist.ExpenseDetailID equals ncDtl.ExpNCDtl_ID
                           join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID,
                               hist.GOExpHist_Id,
                               nc.ExpNC_Category_ID,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_ReferenceNo,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Section,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Ccy,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Cust,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry11ExchRate,
                               hist.GOExpHist_Entry11ExchCcy,
                               hist.GOExpHist_Entry11Fund,
                               hist.GOExpHist_Entry11AdvcPrnt,
                               hist.GOExpHist_Entry11Details,
                               hist.GOExpHist_Entry11Entity,
                               hist.GOExpHist_Entry11Division,
                               hist.GOExpHist_Entry11InterAmt,
                               hist.GOExpHist_Entry11InterRate,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Ccy,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Cust,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry12ExchRate,
                               hist.GOExpHist_Entry12ExchCcy,
                               hist.GOExpHist_Entry12Fund,
                               hist.GOExpHist_Entry12AdvcPrnt,
                               hist.GOExpHist_Entry12Details,
                               hist.GOExpHist_Entry12Entity,
                               hist.GOExpHist_Entry12Division,
                               hist.GOExpHist_Entry12InterAmt,
                               hist.GOExpHist_Entry12InterRate,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Ccy,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Cust,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry21ExchRate,
                               hist.GOExpHist_Entry21ExchCcy,
                               hist.GOExpHist_Entry21Fund,
                               hist.GOExpHist_Entry21AdvcPrnt,
                               hist.GOExpHist_Entry21Details,
                               hist.GOExpHist_Entry21Entity,
                               hist.GOExpHist_Entry21Division,
                               hist.GOExpHist_Entry21InterAmt,
                               hist.GOExpHist_Entry21InterRate,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Ccy,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Cust,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry22ExchRate,
                               hist.GOExpHist_Entry22ExchCcy,
                               hist.GOExpHist_Entry22Fund,
                               hist.GOExpHist_Entry22AdvcPrnt,
                               hist.GOExpHist_Entry22Details,
                               hist.GOExpHist_Entry22Entity,
                               hist.GOExpHist_Entry22Division,
                               hist.GOExpHist_Entry22InterAmt,
                               hist.GOExpHist_Entry22InterRate,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Ccy,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Cust,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry31ExchRate,
                               hist.GOExpHist_Entry31ExchCcy,
                               hist.GOExpHist_Entry31Fund,
                               hist.GOExpHist_Entry31AdvcPrnt,
                               hist.GOExpHist_Entry31Details,
                               hist.GOExpHist_Entry31Entity,
                               hist.GOExpHist_Entry31Division,
                               hist.GOExpHist_Entry31InterAmt,
                               hist.GOExpHist_Entry31InterRate,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Ccy,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Cust,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry32ExchRate,
                               hist.GOExpHist_Entry32ExchCcy,
                               hist.GOExpHist_Entry32Fund,
                               hist.GOExpHist_Entry32AdvcPrnt,
                               hist.GOExpHist_Entry32Details,
                               hist.GOExpHist_Entry32Entity,
                               hist.GOExpHist_Entry32Division,
                               hist.GOExpHist_Entry32InterAmt,
                               hist.GOExpHist_Entry32InterRate,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Ccy,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Cust,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry41ExchRate,
                               hist.GOExpHist_Entry41ExchCcy,
                               hist.GOExpHist_Entry41Fund,
                               hist.GOExpHist_Entry41AdvcPrnt,
                               hist.GOExpHist_Entry41Details,
                               hist.GOExpHist_Entry41Entity,
                               hist.GOExpHist_Entry41Division,
                               hist.GOExpHist_Entry41InterAmt,
                               hist.GOExpHist_Entry41InterRate,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Ccy,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Cust,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               hist.GOExpHist_Entry42ExchRate,
                               hist.GOExpHist_Entry42ExchCcy,
                               hist.GOExpHist_Entry42Fund,
                               hist.GOExpHist_Entry42AdvcPrnt,
                               hist.GOExpHist_Entry42Details,
                               hist.GOExpHist_Entry42Entity,
                               hist.GOExpHist_Entry42Division,
                               hist.GOExpHist_Entry42InterAmt,
                               hist.GOExpHist_Entry42InterRate,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_StatusID
                           }).Where("@14.Contains(TL_StatusID) && " + whereQuery2 + " && Expense_Type = @13", model.ReportSubType, startDT.Date, endDT.Date, model.PeriodFrom.Date,
            model.PeriodTo.Date, model.CheckNoFrom, model.CheckNoTo, model.VoucherNoFrom, model.VoucherNoTo,
            model.TransNoFrom, model.TransNoTo, model.SubjName, expType2, GlobalSystemValues.TYPE_NC, statusTrans).ToList();

                //Convert to List object.
                foreach (var i in db2)
                {
                    list2.Add(new HomeReportTransactionListViewModel
                    {
                        ExpExpense_ID = i.Expense_ID,
                        ExpExpense_Type = i.Expense_Type,
                        Trans_Last_Updated_Date = i.Expense_Last_Updated,
                        ExpExpense_Date = i.Expense_Date.ToString(),
                        Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, false),
                        Trans_Check_Number = i.Expense_CheckNo,
                        HistExpenseEntryID = i.ExpenseEntryID,
                        HistExpenseDetailID = i.ExpenseDetailID,
                        HistGOExpHist_Id = i.GOExpHist_Id,
                        NCExpNC_Category_ID = i.ExpNC_Category_ID,
                        Trans_Value_Date = i.GOExpHist_ValueDate,
                        Trans_Reference_No = i.GOExpHist_ReferenceNo,
                        Trans_Section = i.GOExpHist_Section,
                        Trans_Remarks = i.GOExpHist_Remarks,
                        Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                        Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                        Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                        Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                        Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                        Trans_Account_Name1_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                        Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                        Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                        Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                        Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                        Trans_Details1_1 = i.GOExpHist_Entry11Details,
                        Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                        Trans_Division1_1 = i.GOExpHist_Entry11Division,
                        Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                        Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                        Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                        Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                        Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                        Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                        Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                        Trans_Account_Name1_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                        Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                        Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                        Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                        Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                        Trans_Details1_2 = i.GOExpHist_Entry12Details,
                        Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                        Trans_Division1_2 = i.GOExpHist_Entry12Division,
                        Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                        Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                        Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                        Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                        Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                        Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                        Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                        Trans_Account_Name2_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                        Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                        Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                        Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                        Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                        Trans_Details2_1 = i.GOExpHist_Entry21Details,
                        Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                        Trans_Division2_1 = i.GOExpHist_Entry21Division,
                        Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                        Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                        Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                        Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                        Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                        Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                        Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                        Trans_Account_Name2_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                        Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                        Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                        Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                        Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                        Trans_Details2_2 = i.GOExpHist_Entry22Details,
                        Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                        Trans_Division2_2 = i.GOExpHist_Entry22Division,
                        Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                        Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                        Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                        Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                        Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                        Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                        Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                        Trans_Account_Name3_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                        Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                        Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                        Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                        Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                        Trans_Details3_1 = i.GOExpHist_Entry31Details,
                        Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                        Trans_Division3_1 = i.GOExpHist_Entry31Division,
                        Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                        Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                        Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                        Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                        Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                        Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                        Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                        Trans_Account_Name3_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                        Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                        Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                        Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                        Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                        Trans_Details3_2 = i.GOExpHist_Entry32Details,
                        Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                        Trans_Division3_2 = i.GOExpHist_Entry32Division,
                        Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                        Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                        Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                        Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                        Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                        Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                        Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                        Trans_Account_Name4_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                        Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                        Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                        Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                        Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                        Trans_Details4_1 = i.GOExpHist_Entry41Details,
                        Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                        Trans_Division4_1 = i.GOExpHist_Entry41Division,
                        Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                        Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                        Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                        Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                        Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                        Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                        Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                        Trans_Account_Name4_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, ncDtlList, i.ExpenseDetailID),
                        Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                        Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                        Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                        Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                        Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                        Trans_Details4_2 = i.GOExpHist_Entry42Details,
                        Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                        Trans_Division4_2 = i.GOExpHist_Entry42Division,
                        Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                        Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                        TransTL_ID = i.TL_ID,
                        TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                        TransTL_TransID = i.TL_TransID

                    });
                }
            }

            if (model.ReportSubType == 0 || model.ReportSubType == HomeReportConstantValue.REP_LIQUIDATION)
            {
                DateTime startDT = DateTime.ParseExact(model.Year + "-" + model.Month, "yyyy-M", CultureInfo.InvariantCulture);
                DateTime endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, "yyyy-M", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
                int subType = 0;

                if (model.ReportSubType == HomeReportConstantValue.REP_LIQUIDATION)
                {
                    subType = GlobalSystemValues.TYPE_SS;
                }
                else
                {
                    subType = model.ReportSubType;
                }
                //Get DDV entry detail list. include inter entity
                List<EntryDDVViewModel> ddvDetails = GetEntryDetailsListForDDV();

                if (model.ReportSubType != 0)
                {
                    if (!String.IsNullOrEmpty(whereQuery))
                    {
                        whereQuery3 = "Expense_Type = @0 && " + whereQuery;
                    }
                    else
                    {
                        whereQuery3 = "Expense_Type = @0";
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(whereQuery))
                    {
                        whereQuery3 = "@12.Contains(Expense_Type) && " + whereQuery;
                    }
                    else
                    {
                        whereQuery3 = "@12.Contains(Expense_Type)";
                    }
                }
                if (model.PeriodOption == 1)
                {
                    if (whereQuery3 != "")
                        whereQuery3 += " && ";
                    whereQuery3 += "@1 <= Liq_LastUpdated_Date.Date && Liq_LastUpdated_Date.Date <= @2";
                }
                else if (model.PeriodOption == 3)
                {
                    if (whereQuery3 != "")
                        whereQuery3 += " && ";
                    whereQuery3 += "@3 <= Liq_LastUpdated_Date.Date && Liq_LastUpdated_Date.Date <= @4";
                }
                var db3 = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join liqDtl in _context.LiquidationEntryDetails on hist.ExpenseEntryID equals liqDtl.ExpenseEntryModel.Expense_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           join expDtl in _context.ExpenseEntryDetails on hist.ExpenseDetailID equals expDtl.ExpDtl_ID
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               expDtl.ExpDtl_Account,
                               expDtl.ExpDtl_CreditAccount1,
                               expDtl.ExpDtl_CreditAccount2,
                               liqDtl.Liq_LastUpdated_Date,
                               liqDtl.Liq_Status,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID,
                               hist.GOExpHist_Id,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_ReferenceNo,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Section,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Ccy,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Cust,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry11ExchRate,
                               hist.GOExpHist_Entry11ExchCcy,
                               hist.GOExpHist_Entry11Fund,
                               hist.GOExpHist_Entry11AdvcPrnt,
                               hist.GOExpHist_Entry11Details,
                               hist.GOExpHist_Entry11Entity,
                               hist.GOExpHist_Entry11Division,
                               hist.GOExpHist_Entry11InterAmt,
                               hist.GOExpHist_Entry11InterRate,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Ccy,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Cust,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry12ExchRate,
                               hist.GOExpHist_Entry12ExchCcy,
                               hist.GOExpHist_Entry12Fund,
                               hist.GOExpHist_Entry12AdvcPrnt,
                               hist.GOExpHist_Entry12Details,
                               hist.GOExpHist_Entry12Entity,
                               hist.GOExpHist_Entry12Division,
                               hist.GOExpHist_Entry12InterAmt,
                               hist.GOExpHist_Entry12InterRate,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Ccy,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Cust,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry21ExchRate,
                               hist.GOExpHist_Entry21ExchCcy,
                               hist.GOExpHist_Entry21Fund,
                               hist.GOExpHist_Entry21AdvcPrnt,
                               hist.GOExpHist_Entry21Details,
                               hist.GOExpHist_Entry21Entity,
                               hist.GOExpHist_Entry21Division,
                               hist.GOExpHist_Entry21InterAmt,
                               hist.GOExpHist_Entry21InterRate,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Ccy,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Cust,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry22ExchRate,
                               hist.GOExpHist_Entry22ExchCcy,
                               hist.GOExpHist_Entry22Fund,
                               hist.GOExpHist_Entry22AdvcPrnt,
                               hist.GOExpHist_Entry22Details,
                               hist.GOExpHist_Entry22Entity,
                               hist.GOExpHist_Entry22Division,
                               hist.GOExpHist_Entry22InterAmt,
                               hist.GOExpHist_Entry22InterRate,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Ccy,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Cust,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry31ExchRate,
                               hist.GOExpHist_Entry31ExchCcy,
                               hist.GOExpHist_Entry31Fund,
                               hist.GOExpHist_Entry31AdvcPrnt,
                               hist.GOExpHist_Entry31Details,
                               hist.GOExpHist_Entry31Entity,
                               hist.GOExpHist_Entry31Division,
                               hist.GOExpHist_Entry31InterAmt,
                               hist.GOExpHist_Entry31InterRate,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Ccy,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Cust,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry32ExchRate,
                               hist.GOExpHist_Entry32ExchCcy,
                               hist.GOExpHist_Entry32Fund,
                               hist.GOExpHist_Entry32AdvcPrnt,
                               hist.GOExpHist_Entry32Details,
                               hist.GOExpHist_Entry32Entity,
                               hist.GOExpHist_Entry32Division,
                               hist.GOExpHist_Entry32InterAmt,
                               hist.GOExpHist_Entry32InterRate,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Ccy,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Cust,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry41ExchRate,
                               hist.GOExpHist_Entry41ExchCcy,
                               hist.GOExpHist_Entry41Fund,
                               hist.GOExpHist_Entry41AdvcPrnt,
                               hist.GOExpHist_Entry41Details,
                               hist.GOExpHist_Entry41Entity,
                               hist.GOExpHist_Entry41Division,
                               hist.GOExpHist_Entry41InterAmt,
                               hist.GOExpHist_Entry41InterRate,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Ccy,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Cust,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               hist.GOExpHist_Entry42ExchRate,
                               hist.GOExpHist_Entry42ExchCcy,
                               hist.GOExpHist_Entry42Fund,
                               hist.GOExpHist_Entry42AdvcPrnt,
                               hist.GOExpHist_Entry42Details,
                               hist.GOExpHist_Entry42Entity,
                               hist.GOExpHist_Entry42Division,
                               hist.GOExpHist_Entry42InterAmt,
                               hist.GOExpHist_Entry42InterRate,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_StatusID
                           }).Where("@13.Contains(TL_StatusID) && " + whereQuery3, subType, startDT.Date, endDT.Date, model.PeriodFrom.Date,
                                   model.PeriodTo.Date, model.CheckNoFrom, model.CheckNoTo, model.VoucherNoFrom, model.VoucherNoTo,
                                   model.TransNoFrom, model.TransNoTo, model.SubjName, expType3, statusTrans).ToList();

                //Convert to List object.
                foreach (var i in db3)
                {
                    //Ignore Liquidation record if Filter is Cash Advance only
                    if (model.ReportSubType == GlobalSystemValues.TYPE_SS)
                    {
                        if (i.GOExpHist_Remarks == "S" + _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.ExpenseDetailID).FirstOrDefault().ExpDtl_Gbase_Remarks)
                        {
                            continue;
                        }
                    }
                    //Ignore Cash advance record if Filter is Liquidation only
                    if (model.ReportSubType == HomeReportConstantValue.REP_LIQUIDATION)
                    {
                        if (i.GOExpHist_Remarks != "S" + _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.ExpenseDetailID).FirstOrDefault().ExpDtl_Gbase_Remarks)
                        {
                            continue;
                        }
                    }

                    //Check record if liquidation or not.
                    bool boolLiq = _context.ExpenseTransLists.Where(x => x.TL_ID == i.TL_ID).Select(x => x.TL_Liquidation).FirstOrDefault();

                    if (!boolLiq)
                    {
                        continue;
                    }

                    list3.Add(new HomeReportTransactionListViewModel
                    {
                        ExpExpense_ID = i.Expense_ID,
                        ExpExpense_Type = i.Expense_Type,
                        Trans_Last_Updated_Date = i.Expense_Last_Updated,
                        ExpExpense_Date = i.Expense_Date.ToString(),
                        Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, boolLiq),
                        Trans_Check_Number = i.Expense_CheckNo,
                        HistExpenseEntryID = i.ExpenseEntryID,
                        HistExpenseDetailID = i.ExpenseDetailID,
                        HistGOExpHist_Id = i.GOExpHist_Id,
                        Trans_Value_Date = i.GOExpHist_ValueDate,
                        Trans_Reference_No = i.GOExpHist_ReferenceNo,
                        Trans_Section = i.GOExpHist_Section,
                        Trans_Remarks = i.GOExpHist_Remarks,
                        Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                        Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                        Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                        Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                        Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                        Trans_Account_Name1_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, i.GOExpHist_Entry11Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                        Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                        Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                        Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                        Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                        Trans_Details1_1 = i.GOExpHist_Entry11Details,
                        Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                        Trans_Division1_1 = i.GOExpHist_Entry11Division,
                        Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                        Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                        Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                        Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                        Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                        Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                        Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                        Trans_Account_Name1_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, i.GOExpHist_Entry12Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                        Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                        Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                        Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                        Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                        Trans_Details1_2 = i.GOExpHist_Entry12Details,
                        Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                        Trans_Division1_2 = i.GOExpHist_Entry12Division,
                        Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                        Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                        Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                        Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                        Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                        Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                        Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                        Trans_Account_Name2_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, i.GOExpHist_Entry21Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                        Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                        Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                        Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                        Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                        Trans_Details2_1 = i.GOExpHist_Entry21Details,
                        Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                        Trans_Division2_1 = i.GOExpHist_Entry21Division,
                        Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                        Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                        Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                        Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                        Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                        Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                        Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                        Trans_Account_Name2_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, i.GOExpHist_Entry22Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                        Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                        Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                        Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                        Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                        Trans_Details2_2 = i.GOExpHist_Entry22Details,
                        Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                        Trans_Division2_2 = i.GOExpHist_Entry22Division,
                        Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                        Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                        Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                        Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                        Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                        Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                        Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                        Trans_Account_Name3_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, i.GOExpHist_Entry31Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                        Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                        Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                        Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                        Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                        Trans_Details3_1 = i.GOExpHist_Entry31Details,
                        Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                        Trans_Division3_1 = i.GOExpHist_Entry31Division,
                        Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                        Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                        Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                        Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                        Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                        Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                        Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                        Trans_Account_Name3_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, i.GOExpHist_Entry32Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                        Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                        Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                        Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                        Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                        Trans_Details3_2 = i.GOExpHist_Entry32Details,
                        Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                        Trans_Division3_2 = i.GOExpHist_Entry32Division,
                        Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                        Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                        Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                        Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                        Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                        Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                        Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                        Trans_Account_Name4_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, i.GOExpHist_Entry41Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                        Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                        Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                        Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                        Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                        Trans_Details4_1 = i.GOExpHist_Entry41Details,
                        Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                        Trans_Division4_1 = i.GOExpHist_Entry41Division,
                        Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                        Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                        Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                        Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                        Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                        Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                        Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                        Trans_Account_Name4_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, i.GOExpHist_Entry42Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                        Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                        Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                        Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                        Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                        Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                        Trans_Details4_2 = i.GOExpHist_Entry42Details,
                        Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                        Trans_Division4_2 = i.GOExpHist_Entry42Division,
                        Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                        Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                        TransTL_ID = i.TL_ID,
                        TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                        TransTL_TransID = i.TL_TransID

                    });
                }
            }

            return list1.Concat(list2).Concat(list3).OrderBy(x => x.Trans_Last_Updated_Date);
        }

        public IEnumerable<HomeReportAccountSummaryViewModel> GetAccountSummaryData(HomeReportViewModel model)
        {
            string whereQuery = "";
            string whereQuery1 = "";
            string whereQuery2 = "";
            string whereQuery3 = "";
            DateTime startDT = new DateTime();
            DateTime endDT = new DateTime();
            if (model.ReportType != HomeReportConstantValue.OutstandingAdvances)
            {
                startDT = DateTime.ParseExact(model.Year + "-" + model.Month, "yyyy-M", CultureInfo.InvariantCulture);
                endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, "yyyy-M", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }
            //int[] expType1 = { GlobalSystemValues.TYPE_CV, GlobalSystemValues.TYPE_PC,
            //    GlobalSystemValues.TYPE_DDV, GlobalSystemValues.TYPE_SS };
            //int[] expType2 = { HomeReportConstantValue.REP_NC_LS_PAYROLL - 50, HomeReportConstantValue.REP_NC_JS_PAYROLL - 50,
            //    HomeReportConstantValue.REP_NC_TAX_REMITTANCE - 50};

            List<HomeReportTransactionListViewModel> list1 = new List<HomeReportTransactionListViewModel>();
            List<HomeReportTransactionListViewModel> list2 = new List<HomeReportTransactionListViewModel>();
            List<HomeReportTransactionListViewModel> list3 = new List<HomeReportTransactionListViewModel>();

            List<DMAccountModel> accList = getAccountListIncHist();
            DMAccountModel selectedAccount = new DMAccountModel();

            if (model.ReportSubType != 0)
            {
                selectedAccount = accList.Where(x => x.Account_ID == model.ReportSubType).FirstOrDefault();
            }

            if (model.PeriodOption == 1)
            {
                whereQuery = "@0 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @1";
            }
            else if (model.PeriodOption == 3)
            {
                whereQuery = "@2 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @3";
            }
            if (!String.IsNullOrEmpty(whereQuery))
            {
                whereQuery1 = "Expense_Type != @4 && " + whereQuery;
            }
            else
            {
                whereQuery1 = "Expense_Type != @4";
            }

            //Get DDV entry detail list. include inter entity
            List<EntryDDVViewModel> ddvDetails = GetEntryDetailsListForDDV();

            var db1 = (from hist in _context.GOExpressHist
                       join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                       join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                       join expDtl in _context.ExpenseEntryDetails on hist.ExpenseDetailID equals expDtl.ExpDtl_ID
                       select new
                       {
                           exp.Expense_ID,
                           exp.Expense_Type,
                           exp.Expense_Last_Updated,
                           exp.Expense_Date,
                           exp.Expense_Number,
                           exp.Expense_CheckNo,
                           expDtl.ExpDtl_Account,
                           expDtl.ExpDtl_CreditAccount1,
                           expDtl.ExpDtl_CreditAccount2,
                           hist.ExpenseEntryID,
                           hist.ExpenseDetailID,
                           hist.GOExpHist_Id,
                           hist.GOExpHist_ValueDate,
                           hist.GOExpHist_ReferenceNo,
                           hist.GOExpHist_Branchno,
                           hist.GOExpHist_Section,
                           hist.GOExpHist_Remarks,
                           hist.GOExpHist_Entry11Type,
                           hist.GOExpHist_Entry11Ccy,
                           hist.GOExpHist_Entry11Amt,
                           hist.GOExpHist_Entry11Cust,
                           hist.GOExpHist_Entry11Actcde,
                           hist.GOExpHist_Entry11ActType,
                           hist.GOExpHist_Entry11ActNo,
                           hist.GOExpHist_Entry11ExchRate,
                           hist.GOExpHist_Entry11ExchCcy,
                           hist.GOExpHist_Entry11Fund,
                           hist.GOExpHist_Entry11AdvcPrnt,
                           hist.GOExpHist_Entry11Details,
                           hist.GOExpHist_Entry11Entity,
                           hist.GOExpHist_Entry11Division,
                           hist.GOExpHist_Entry11InterAmt,
                           hist.GOExpHist_Entry11InterRate,
                           hist.GOExpHist_Entry12Type,
                           hist.GOExpHist_Entry12Ccy,
                           hist.GOExpHist_Entry12Amt,
                           hist.GOExpHist_Entry12Cust,
                           hist.GOExpHist_Entry12Actcde,
                           hist.GOExpHist_Entry12ActType,
                           hist.GOExpHist_Entry12ActNo,
                           hist.GOExpHist_Entry12ExchRate,
                           hist.GOExpHist_Entry12ExchCcy,
                           hist.GOExpHist_Entry12Fund,
                           hist.GOExpHist_Entry12AdvcPrnt,
                           hist.GOExpHist_Entry12Details,
                           hist.GOExpHist_Entry12Entity,
                           hist.GOExpHist_Entry12Division,
                           hist.GOExpHist_Entry12InterAmt,
                           hist.GOExpHist_Entry12InterRate,
                           hist.GOExpHist_Entry21Type,
                           hist.GOExpHist_Entry21Ccy,
                           hist.GOExpHist_Entry21Amt,
                           hist.GOExpHist_Entry21Cust,
                           hist.GOExpHist_Entry21Actcde,
                           hist.GOExpHist_Entry21ActType,
                           hist.GOExpHist_Entry21ActNo,
                           hist.GOExpHist_Entry21ExchRate,
                           hist.GOExpHist_Entry21ExchCcy,
                           hist.GOExpHist_Entry21Fund,
                           hist.GOExpHist_Entry21AdvcPrnt,
                           hist.GOExpHist_Entry21Details,
                           hist.GOExpHist_Entry21Entity,
                           hist.GOExpHist_Entry21Division,
                           hist.GOExpHist_Entry21InterAmt,
                           hist.GOExpHist_Entry21InterRate,
                           hist.GOExpHist_Entry22Type,
                           hist.GOExpHist_Entry22Ccy,
                           hist.GOExpHist_Entry22Amt,
                           hist.GOExpHist_Entry22Cust,
                           hist.GOExpHist_Entry22Actcde,
                           hist.GOExpHist_Entry22ActType,
                           hist.GOExpHist_Entry22ActNo,
                           hist.GOExpHist_Entry22ExchRate,
                           hist.GOExpHist_Entry22ExchCcy,
                           hist.GOExpHist_Entry22Fund,
                           hist.GOExpHist_Entry22AdvcPrnt,
                           hist.GOExpHist_Entry22Details,
                           hist.GOExpHist_Entry22Entity,
                           hist.GOExpHist_Entry22Division,
                           hist.GOExpHist_Entry22InterAmt,
                           hist.GOExpHist_Entry22InterRate,
                           hist.GOExpHist_Entry31Type,
                           hist.GOExpHist_Entry31Ccy,
                           hist.GOExpHist_Entry31Amt,
                           hist.GOExpHist_Entry31Cust,
                           hist.GOExpHist_Entry31Actcde,
                           hist.GOExpHist_Entry31ActType,
                           hist.GOExpHist_Entry31ActNo,
                           hist.GOExpHist_Entry31ExchRate,
                           hist.GOExpHist_Entry31ExchCcy,
                           hist.GOExpHist_Entry31Fund,
                           hist.GOExpHist_Entry31AdvcPrnt,
                           hist.GOExpHist_Entry31Details,
                           hist.GOExpHist_Entry31Entity,
                           hist.GOExpHist_Entry31Division,
                           hist.GOExpHist_Entry31InterAmt,
                           hist.GOExpHist_Entry31InterRate,
                           hist.GOExpHist_Entry32Type,
                           hist.GOExpHist_Entry32Ccy,
                           hist.GOExpHist_Entry32Amt,
                           hist.GOExpHist_Entry32Cust,
                           hist.GOExpHist_Entry32Actcde,
                           hist.GOExpHist_Entry32ActType,
                           hist.GOExpHist_Entry32ActNo,
                           hist.GOExpHist_Entry32ExchRate,
                           hist.GOExpHist_Entry32ExchCcy,
                           hist.GOExpHist_Entry32Fund,
                           hist.GOExpHist_Entry32AdvcPrnt,
                           hist.GOExpHist_Entry32Details,
                           hist.GOExpHist_Entry32Entity,
                           hist.GOExpHist_Entry32Division,
                           hist.GOExpHist_Entry32InterAmt,
                           hist.GOExpHist_Entry32InterRate,
                           hist.GOExpHist_Entry41Type,
                           hist.GOExpHist_Entry41Ccy,
                           hist.GOExpHist_Entry41Amt,
                           hist.GOExpHist_Entry41Cust,
                           hist.GOExpHist_Entry41Actcde,
                           hist.GOExpHist_Entry41ActType,
                           hist.GOExpHist_Entry41ActNo,
                           hist.GOExpHist_Entry41ExchRate,
                           hist.GOExpHist_Entry41ExchCcy,
                           hist.GOExpHist_Entry41Fund,
                           hist.GOExpHist_Entry41AdvcPrnt,
                           hist.GOExpHist_Entry41Details,
                           hist.GOExpHist_Entry41Entity,
                           hist.GOExpHist_Entry41Division,
                           hist.GOExpHist_Entry41InterAmt,
                           hist.GOExpHist_Entry41InterRate,
                           hist.GOExpHist_Entry42Type,
                           hist.GOExpHist_Entry42Ccy,
                           hist.GOExpHist_Entry42Amt,
                           hist.GOExpHist_Entry42Cust,
                           hist.GOExpHist_Entry42Actcde,
                           hist.GOExpHist_Entry42ActType,
                           hist.GOExpHist_Entry42ActNo,
                           hist.GOExpHist_Entry42ExchRate,
                           hist.GOExpHist_Entry42ExchCcy,
                           hist.GOExpHist_Entry42Fund,
                           hist.GOExpHist_Entry42AdvcPrnt,
                           hist.GOExpHist_Entry42Details,
                           hist.GOExpHist_Entry42Entity,
                           hist.GOExpHist_Entry42Division,
                           hist.GOExpHist_Entry42InterAmt,
                           hist.GOExpHist_Entry42InterRate,
                           trans.TL_ID,
                           trans.TL_GoExpress_ID,
                           trans.TL_TransID,
                           trans.TL_StatusID
                       }).Where("@5.Contains(TL_StatusID) && " + whereQuery1, startDT.Date, endDT.Date, model.PeriodFrom.Date,
                               model.PeriodTo.Date, GlobalSystemValues.TYPE_NC, statusTrans).ToList();

            //Convert to List object.
            foreach (var i in db1)
            {
                //Check record if liquidation or not.
                bool boolLiq = _context.ExpenseTransLists.Where(x => x.TL_ID == i.TL_ID).Select(x => x.TL_Liquidation).FirstOrDefault();
                if (boolLiq)
                {
                    continue;
                }
                list1.Add(new HomeReportTransactionListViewModel
                {
                    ExpExpense_ID = i.Expense_ID,
                    ExpExpense_Type = i.Expense_Type,
                    Trans_Last_Updated_Date = i.Expense_Last_Updated,
                    ExpExpense_Date = i.Expense_Date.ToString(),
                    Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, boolLiq),
                    Trans_Check_Number = i.Expense_CheckNo,
                    HistExpenseEntryID = i.ExpenseEntryID,
                    HistExpenseDetailID = i.ExpenseDetailID,
                    HistGOExpHist_Id = i.GOExpHist_Id,
                    Trans_Value_Date = i.GOExpHist_ValueDate,
                    Trans_Reference_No = i.GOExpHist_ReferenceNo,
                    Trans_Section = i.GOExpHist_Section,
                    Trans_Remarks = i.GOExpHist_Remarks,
                    Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                    Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                    Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                    Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                    Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                    Trans_Account_Name1_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, i.GOExpHist_Entry11Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                    Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                    Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                    Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details1_1 = i.GOExpHist_Entry11Details,
                    Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                    Trans_Division1_1 = i.GOExpHist_Entry11Division,
                    Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                    Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                    Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                    Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                    Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                    Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                    Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                    Trans_Account_Name1_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, i.GOExpHist_Entry12Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                    Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                    Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                    Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details1_2 = i.GOExpHist_Entry12Details,
                    Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                    Trans_Division1_2 = i.GOExpHist_Entry12Division,
                    Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                    Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                    Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                    Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                    Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                    Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                    Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                    Trans_Account_Name2_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, i.GOExpHist_Entry21Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                    Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                    Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                    Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details2_1 = i.GOExpHist_Entry21Details,
                    Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                    Trans_Division2_1 = i.GOExpHist_Entry21Division,
                    Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                    Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                    Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                    Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                    Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                    Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                    Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                    Trans_Account_Name2_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, i.GOExpHist_Entry22Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                    Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                    Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                    Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details2_2 = i.GOExpHist_Entry22Details,
                    Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                    Trans_Division2_2 = i.GOExpHist_Entry22Division,
                    Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                    Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                    Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                    Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                    Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                    Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                    Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                    Trans_Account_Name3_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, i.GOExpHist_Entry31Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                    Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                    Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                    Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details3_1 = i.GOExpHist_Entry31Details,
                    Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                    Trans_Division3_1 = i.GOExpHist_Entry31Division,
                    Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                    Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                    Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                    Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                    Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                    Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                    Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                    Trans_Account_Name3_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, i.GOExpHist_Entry32Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                    Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                    Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                    Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details3_2 = i.GOExpHist_Entry32Details,
                    Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                    Trans_Division3_2 = i.GOExpHist_Entry32Division,
                    Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                    Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                    Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                    Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                    Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                    Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                    Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                    Trans_Account_Name4_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, i.GOExpHist_Entry41Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                    Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                    Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                    Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details4_1 = i.GOExpHist_Entry41Details,
                    Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                    Trans_Division4_1 = i.GOExpHist_Entry41Division,
                    Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                    Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                    Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                    Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                    Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                    Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                    Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                    Trans_Account_Name4_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, i.GOExpHist_Entry42Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                    Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                    Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                    Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details4_2 = i.GOExpHist_Entry42Details,
                    Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                    Trans_Division4_2 = i.GOExpHist_Entry42Division,
                    Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                    Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                    TransTL_ID = i.TL_ID,
                    TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                    TransTL_TransID = i.TL_TransID

                });
            }

            List<ExpenseEntryNCDtlViewModel> ncDtlList = GetEntryDetailAccountListForNonCash();

            if (!String.IsNullOrEmpty(whereQuery))
            {
                whereQuery2 = "Expense_Type == @4 && " + whereQuery;
            }
            else
            {
                whereQuery2 = "Expense_Type == @4";
            }

            var db2 = (from hist in _context.GOExpressHist
                       join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                       join ncDtl in _context.ExpenseEntryNonCashDetails on hist.ExpenseDetailID equals ncDtl.ExpNCDtl_ID
                       join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
                       join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                       select new
                       {
                           exp.Expense_ID,
                           exp.Expense_Type,
                           exp.Expense_Last_Updated,
                           exp.Expense_Date,
                           exp.Expense_Number,
                           exp.Expense_CheckNo,
                           hist.ExpenseEntryID,
                           hist.ExpenseDetailID,
                           hist.GOExpHist_Id,
                           nc.ExpNC_Category_ID,
                           hist.GOExpHist_ValueDate,
                           hist.GOExpHist_ReferenceNo,
                           hist.GOExpHist_Branchno,
                           hist.GOExpHist_Section,
                           hist.GOExpHist_Remarks,
                           hist.GOExpHist_Entry11Type,
                           hist.GOExpHist_Entry11Ccy,
                           hist.GOExpHist_Entry11Amt,
                           hist.GOExpHist_Entry11Cust,
                           hist.GOExpHist_Entry11Actcde,
                           hist.GOExpHist_Entry11ActType,
                           hist.GOExpHist_Entry11ActNo,
                           hist.GOExpHist_Entry11ExchRate,
                           hist.GOExpHist_Entry11ExchCcy,
                           hist.GOExpHist_Entry11Fund,
                           hist.GOExpHist_Entry11AdvcPrnt,
                           hist.GOExpHist_Entry11Details,
                           hist.GOExpHist_Entry11Entity,
                           hist.GOExpHist_Entry11Division,
                           hist.GOExpHist_Entry11InterAmt,
                           hist.GOExpHist_Entry11InterRate,
                           hist.GOExpHist_Entry12Type,
                           hist.GOExpHist_Entry12Ccy,
                           hist.GOExpHist_Entry12Amt,
                           hist.GOExpHist_Entry12Cust,
                           hist.GOExpHist_Entry12Actcde,
                           hist.GOExpHist_Entry12ActType,
                           hist.GOExpHist_Entry12ActNo,
                           hist.GOExpHist_Entry12ExchRate,
                           hist.GOExpHist_Entry12ExchCcy,
                           hist.GOExpHist_Entry12Fund,
                           hist.GOExpHist_Entry12AdvcPrnt,
                           hist.GOExpHist_Entry12Details,
                           hist.GOExpHist_Entry12Entity,
                           hist.GOExpHist_Entry12Division,
                           hist.GOExpHist_Entry12InterAmt,
                           hist.GOExpHist_Entry12InterRate,
                           hist.GOExpHist_Entry21Type,
                           hist.GOExpHist_Entry21Ccy,
                           hist.GOExpHist_Entry21Amt,
                           hist.GOExpHist_Entry21Cust,
                           hist.GOExpHist_Entry21Actcde,
                           hist.GOExpHist_Entry21ActType,
                           hist.GOExpHist_Entry21ActNo,
                           hist.GOExpHist_Entry21ExchRate,
                           hist.GOExpHist_Entry21ExchCcy,
                           hist.GOExpHist_Entry21Fund,
                           hist.GOExpHist_Entry21AdvcPrnt,
                           hist.GOExpHist_Entry21Details,
                           hist.GOExpHist_Entry21Entity,
                           hist.GOExpHist_Entry21Division,
                           hist.GOExpHist_Entry21InterAmt,
                           hist.GOExpHist_Entry21InterRate,
                           hist.GOExpHist_Entry22Type,
                           hist.GOExpHist_Entry22Ccy,
                           hist.GOExpHist_Entry22Amt,
                           hist.GOExpHist_Entry22Cust,
                           hist.GOExpHist_Entry22Actcde,
                           hist.GOExpHist_Entry22ActType,
                           hist.GOExpHist_Entry22ActNo,
                           hist.GOExpHist_Entry22ExchRate,
                           hist.GOExpHist_Entry22ExchCcy,
                           hist.GOExpHist_Entry22Fund,
                           hist.GOExpHist_Entry22AdvcPrnt,
                           hist.GOExpHist_Entry22Details,
                           hist.GOExpHist_Entry22Entity,
                           hist.GOExpHist_Entry22Division,
                           hist.GOExpHist_Entry22InterAmt,
                           hist.GOExpHist_Entry22InterRate,
                           hist.GOExpHist_Entry31Type,
                           hist.GOExpHist_Entry31Ccy,
                           hist.GOExpHist_Entry31Amt,
                           hist.GOExpHist_Entry31Cust,
                           hist.GOExpHist_Entry31Actcde,
                           hist.GOExpHist_Entry31ActType,
                           hist.GOExpHist_Entry31ActNo,
                           hist.GOExpHist_Entry31ExchRate,
                           hist.GOExpHist_Entry31ExchCcy,
                           hist.GOExpHist_Entry31Fund,
                           hist.GOExpHist_Entry31AdvcPrnt,
                           hist.GOExpHist_Entry31Details,
                           hist.GOExpHist_Entry31Entity,
                           hist.GOExpHist_Entry31Division,
                           hist.GOExpHist_Entry31InterAmt,
                           hist.GOExpHist_Entry31InterRate,
                           hist.GOExpHist_Entry32Type,
                           hist.GOExpHist_Entry32Ccy,
                           hist.GOExpHist_Entry32Amt,
                           hist.GOExpHist_Entry32Cust,
                           hist.GOExpHist_Entry32Actcde,
                           hist.GOExpHist_Entry32ActType,
                           hist.GOExpHist_Entry32ActNo,
                           hist.GOExpHist_Entry32ExchRate,
                           hist.GOExpHist_Entry32ExchCcy,
                           hist.GOExpHist_Entry32Fund,
                           hist.GOExpHist_Entry32AdvcPrnt,
                           hist.GOExpHist_Entry32Details,
                           hist.GOExpHist_Entry32Entity,
                           hist.GOExpHist_Entry32Division,
                           hist.GOExpHist_Entry32InterAmt,
                           hist.GOExpHist_Entry32InterRate,
                           hist.GOExpHist_Entry41Type,
                           hist.GOExpHist_Entry41Ccy,
                           hist.GOExpHist_Entry41Amt,
                           hist.GOExpHist_Entry41Cust,
                           hist.GOExpHist_Entry41Actcde,
                           hist.GOExpHist_Entry41ActType,
                           hist.GOExpHist_Entry41ActNo,
                           hist.GOExpHist_Entry41ExchRate,
                           hist.GOExpHist_Entry41ExchCcy,
                           hist.GOExpHist_Entry41Fund,
                           hist.GOExpHist_Entry41AdvcPrnt,
                           hist.GOExpHist_Entry41Details,
                           hist.GOExpHist_Entry41Entity,
                           hist.GOExpHist_Entry41Division,
                           hist.GOExpHist_Entry41InterAmt,
                           hist.GOExpHist_Entry41InterRate,
                           hist.GOExpHist_Entry42Type,
                           hist.GOExpHist_Entry42Ccy,
                           hist.GOExpHist_Entry42Amt,
                           hist.GOExpHist_Entry42Cust,
                           hist.GOExpHist_Entry42Actcde,
                           hist.GOExpHist_Entry42ActType,
                           hist.GOExpHist_Entry42ActNo,
                           hist.GOExpHist_Entry42ExchRate,
                           hist.GOExpHist_Entry42ExchCcy,
                           hist.GOExpHist_Entry42Fund,
                           hist.GOExpHist_Entry42AdvcPrnt,
                           hist.GOExpHist_Entry42Details,
                           hist.GOExpHist_Entry42Entity,
                           hist.GOExpHist_Entry42Division,
                           hist.GOExpHist_Entry42InterAmt,
                           hist.GOExpHist_Entry42InterRate,
                           trans.TL_ID,
                           trans.TL_GoExpress_ID,
                           trans.TL_TransID,
                           trans.TL_StatusID
                       }).Where("@5.Contains(TL_StatusID) && " + whereQuery2, startDT.Date, endDT.Date, model.PeriodFrom.Date,
                                model.PeriodTo.Date, GlobalSystemValues.TYPE_NC, statusTrans).ToList();

            //Convert to List object.
            foreach (var i in db2)
            {
                list2.Add(new HomeReportTransactionListViewModel
                {
                    ExpExpense_ID = i.Expense_ID,
                    ExpExpense_Type = i.Expense_Type,
                    Trans_Last_Updated_Date = i.Expense_Last_Updated,
                    ExpExpense_Date = i.Expense_Date.ToString(),
                    Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, false),
                    Trans_Check_Number = i.Expense_CheckNo,
                    HistExpenseEntryID = i.ExpenseEntryID,
                    HistExpenseDetailID = i.ExpenseDetailID,
                    HistGOExpHist_Id = i.GOExpHist_Id,
                    NCExpNC_Category_ID = i.ExpNC_Category_ID,
                    Trans_Value_Date = i.GOExpHist_ValueDate,
                    Trans_Reference_No = i.GOExpHist_ReferenceNo,
                    Trans_Section = i.GOExpHist_Section,
                    Trans_Remarks = i.GOExpHist_Remarks,
                    Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                    Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                    Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                    Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                    Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                    Trans_Account_Name1_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                    Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                    Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                    Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details1_1 = i.GOExpHist_Entry11Details,
                    Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                    Trans_Division1_1 = i.GOExpHist_Entry11Division,
                    Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                    Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                    Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                    Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                    Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                    Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                    Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                    Trans_Account_Name1_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                    Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                    Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                    Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details1_2 = i.GOExpHist_Entry12Details,
                    Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                    Trans_Division1_2 = i.GOExpHist_Entry12Division,
                    Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                    Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                    Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                    Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                    Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                    Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                    Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                    Trans_Account_Name2_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                    Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                    Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                    Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details2_1 = i.GOExpHist_Entry21Details,
                    Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                    Trans_Division2_1 = i.GOExpHist_Entry21Division,
                    Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                    Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                    Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                    Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                    Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                    Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                    Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                    Trans_Account_Name2_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                    Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                    Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                    Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details2_2 = i.GOExpHist_Entry22Details,
                    Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                    Trans_Division2_2 = i.GOExpHist_Entry22Division,
                    Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                    Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                    Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                    Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                    Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                    Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                    Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                    Trans_Account_Name3_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                    Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                    Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                    Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details3_1 = i.GOExpHist_Entry31Details,
                    Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                    Trans_Division3_1 = i.GOExpHist_Entry31Division,
                    Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                    Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                    Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                    Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                    Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                    Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                    Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                    Trans_Account_Name3_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                    Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                    Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                    Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details3_2 = i.GOExpHist_Entry32Details,
                    Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                    Trans_Division3_2 = i.GOExpHist_Entry32Division,
                    Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                    Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                    Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                    Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                    Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                    Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                    Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                    Trans_Account_Name4_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                    Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                    Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                    Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details4_1 = i.GOExpHist_Entry41Details,
                    Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                    Trans_Division4_1 = i.GOExpHist_Entry41Division,
                    Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                    Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                    Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                    Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                    Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                    Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                    Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                    Trans_Account_Name4_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                    Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                    Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                    Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details4_2 = i.GOExpHist_Entry42Details,
                    Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                    Trans_Division4_2 = i.GOExpHist_Entry42Division,
                    Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                    Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                    TransTL_ID = i.TL_ID,
                    TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                    TransTL_TransID = i.TL_TransID

                });
            }

            whereQuery = "";

            if (model.PeriodOption == 1)
            {
                whereQuery = "@0 <= Liq_LastUpdated_Date.Date && Liq_LastUpdated_Date.Date <= @1";
            }
            else if (model.PeriodOption == 3)
            {
                whereQuery = "@2 <= Liq_LastUpdated_Date.Date && Liq_LastUpdated_Date.Date <= @3";
            }
            if (!String.IsNullOrEmpty(whereQuery))
            {
                whereQuery3 = "Expense_Type == @4 && " + whereQuery;
            }
            else
            {
                whereQuery3 = "Expense_Type == @4";
            }

            var db3 = (from hist in _context.GOExpressHist
                       join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                       join liqDtl in _context.LiquidationEntryDetails on hist.ExpenseEntryID equals liqDtl.ExpenseEntryModel.Expense_ID
                       join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                       join expDtl in _context.ExpenseEntryDetails on hist.ExpenseDetailID equals expDtl.ExpDtl_ID
                       select new
                       {
                           exp.Expense_ID,
                           exp.Expense_Type,
                           exp.Expense_Last_Updated,
                           exp.Expense_Date,
                           exp.Expense_Number,
                           exp.Expense_CheckNo,
                           expDtl.ExpDtl_Account,
                           expDtl.ExpDtl_CreditAccount1,
                           expDtl.ExpDtl_CreditAccount2,
                           liqDtl.Liq_LastUpdated_Date,
                           liqDtl.Liq_Status,
                           hist.ExpenseEntryID,
                           hist.ExpenseDetailID,
                           hist.GOExpHist_Id,
                           hist.GOExpHist_ValueDate,
                           hist.GOExpHist_ReferenceNo,
                           hist.GOExpHist_Branchno,
                           hist.GOExpHist_Section,
                           hist.GOExpHist_Remarks,
                           hist.GOExpHist_Entry11Type,
                           hist.GOExpHist_Entry11Ccy,
                           hist.GOExpHist_Entry11Amt,
                           hist.GOExpHist_Entry11Cust,
                           hist.GOExpHist_Entry11Actcde,
                           hist.GOExpHist_Entry11ActType,
                           hist.GOExpHist_Entry11ActNo,
                           hist.GOExpHist_Entry11ExchRate,
                           hist.GOExpHist_Entry11ExchCcy,
                           hist.GOExpHist_Entry11Fund,
                           hist.GOExpHist_Entry11AdvcPrnt,
                           hist.GOExpHist_Entry11Details,
                           hist.GOExpHist_Entry11Entity,
                           hist.GOExpHist_Entry11Division,
                           hist.GOExpHist_Entry11InterAmt,
                           hist.GOExpHist_Entry11InterRate,
                           hist.GOExpHist_Entry12Type,
                           hist.GOExpHist_Entry12Ccy,
                           hist.GOExpHist_Entry12Amt,
                           hist.GOExpHist_Entry12Cust,
                           hist.GOExpHist_Entry12Actcde,
                           hist.GOExpHist_Entry12ActType,
                           hist.GOExpHist_Entry12ActNo,
                           hist.GOExpHist_Entry12ExchRate,
                           hist.GOExpHist_Entry12ExchCcy,
                           hist.GOExpHist_Entry12Fund,
                           hist.GOExpHist_Entry12AdvcPrnt,
                           hist.GOExpHist_Entry12Details,
                           hist.GOExpHist_Entry12Entity,
                           hist.GOExpHist_Entry12Division,
                           hist.GOExpHist_Entry12InterAmt,
                           hist.GOExpHist_Entry12InterRate,
                           hist.GOExpHist_Entry21Type,
                           hist.GOExpHist_Entry21Ccy,
                           hist.GOExpHist_Entry21Amt,
                           hist.GOExpHist_Entry21Cust,
                           hist.GOExpHist_Entry21Actcde,
                           hist.GOExpHist_Entry21ActType,
                           hist.GOExpHist_Entry21ActNo,
                           hist.GOExpHist_Entry21ExchRate,
                           hist.GOExpHist_Entry21ExchCcy,
                           hist.GOExpHist_Entry21Fund,
                           hist.GOExpHist_Entry21AdvcPrnt,
                           hist.GOExpHist_Entry21Details,
                           hist.GOExpHist_Entry21Entity,
                           hist.GOExpHist_Entry21Division,
                           hist.GOExpHist_Entry21InterAmt,
                           hist.GOExpHist_Entry21InterRate,
                           hist.GOExpHist_Entry22Type,
                           hist.GOExpHist_Entry22Ccy,
                           hist.GOExpHist_Entry22Amt,
                           hist.GOExpHist_Entry22Cust,
                           hist.GOExpHist_Entry22Actcde,
                           hist.GOExpHist_Entry22ActType,
                           hist.GOExpHist_Entry22ActNo,
                           hist.GOExpHist_Entry22ExchRate,
                           hist.GOExpHist_Entry22ExchCcy,
                           hist.GOExpHist_Entry22Fund,
                           hist.GOExpHist_Entry22AdvcPrnt,
                           hist.GOExpHist_Entry22Details,
                           hist.GOExpHist_Entry22Entity,
                           hist.GOExpHist_Entry22Division,
                           hist.GOExpHist_Entry22InterAmt,
                           hist.GOExpHist_Entry22InterRate,
                           hist.GOExpHist_Entry31Type,
                           hist.GOExpHist_Entry31Ccy,
                           hist.GOExpHist_Entry31Amt,
                           hist.GOExpHist_Entry31Cust,
                           hist.GOExpHist_Entry31Actcde,
                           hist.GOExpHist_Entry31ActType,
                           hist.GOExpHist_Entry31ActNo,
                           hist.GOExpHist_Entry31ExchRate,
                           hist.GOExpHist_Entry31ExchCcy,
                           hist.GOExpHist_Entry31Fund,
                           hist.GOExpHist_Entry31AdvcPrnt,
                           hist.GOExpHist_Entry31Details,
                           hist.GOExpHist_Entry31Entity,
                           hist.GOExpHist_Entry31Division,
                           hist.GOExpHist_Entry31InterAmt,
                           hist.GOExpHist_Entry31InterRate,
                           hist.GOExpHist_Entry32Type,
                           hist.GOExpHist_Entry32Ccy,
                           hist.GOExpHist_Entry32Amt,
                           hist.GOExpHist_Entry32Cust,
                           hist.GOExpHist_Entry32Actcde,
                           hist.GOExpHist_Entry32ActType,
                           hist.GOExpHist_Entry32ActNo,
                           hist.GOExpHist_Entry32ExchRate,
                           hist.GOExpHist_Entry32ExchCcy,
                           hist.GOExpHist_Entry32Fund,
                           hist.GOExpHist_Entry32AdvcPrnt,
                           hist.GOExpHist_Entry32Details,
                           hist.GOExpHist_Entry32Entity,
                           hist.GOExpHist_Entry32Division,
                           hist.GOExpHist_Entry32InterAmt,
                           hist.GOExpHist_Entry32InterRate,
                           hist.GOExpHist_Entry41Type,
                           hist.GOExpHist_Entry41Ccy,
                           hist.GOExpHist_Entry41Amt,
                           hist.GOExpHist_Entry41Cust,
                           hist.GOExpHist_Entry41Actcde,
                           hist.GOExpHist_Entry41ActType,
                           hist.GOExpHist_Entry41ActNo,
                           hist.GOExpHist_Entry41ExchRate,
                           hist.GOExpHist_Entry41ExchCcy,
                           hist.GOExpHist_Entry41Fund,
                           hist.GOExpHist_Entry41AdvcPrnt,
                           hist.GOExpHist_Entry41Details,
                           hist.GOExpHist_Entry41Entity,
                           hist.GOExpHist_Entry41Division,
                           hist.GOExpHist_Entry41InterAmt,
                           hist.GOExpHist_Entry41InterRate,
                           hist.GOExpHist_Entry42Type,
                           hist.GOExpHist_Entry42Ccy,
                           hist.GOExpHist_Entry42Amt,
                           hist.GOExpHist_Entry42Cust,
                           hist.GOExpHist_Entry42Actcde,
                           hist.GOExpHist_Entry42ActType,
                           hist.GOExpHist_Entry42ActNo,
                           hist.GOExpHist_Entry42ExchRate,
                           hist.GOExpHist_Entry42ExchCcy,
                           hist.GOExpHist_Entry42Fund,
                           hist.GOExpHist_Entry42AdvcPrnt,
                           hist.GOExpHist_Entry42Details,
                           hist.GOExpHist_Entry42Entity,
                           hist.GOExpHist_Entry42Division,
                           hist.GOExpHist_Entry42InterAmt,
                           hist.GOExpHist_Entry42InterRate,
                           trans.TL_ID,
                           trans.TL_GoExpress_ID,
                           trans.TL_TransID,
                           trans.TL_StatusID
                       }).Where("@5.Contains(TL_StatusID) && " + whereQuery3, startDT.Date, endDT.Date, model.PeriodFrom.Date,
                               model.PeriodTo.Date, GlobalSystemValues.TYPE_SS, statusTrans).ToList();

            //Convert to List object.
            foreach (var i in db3)
            {
                //Check record if liquidation or not.
                bool boolLiq = _context.ExpenseTransLists.Where(x => x.TL_ID == i.TL_ID).Select(x => x.TL_Liquidation).FirstOrDefault();
                if (!boolLiq)
                {
                    continue;
                }
                list3.Add(new HomeReportTransactionListViewModel
                {
                    ExpExpense_ID = i.Expense_ID,
                    ExpExpense_Type = i.Expense_Type,
                    Trans_Last_Updated_Date = i.Expense_Last_Updated,
                    ExpExpense_Date = i.Expense_Date.ToString(),
                    Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, boolLiq),
                    Trans_Check_Number = i.Expense_CheckNo,
                    HistExpenseEntryID = i.ExpenseEntryID,
                    HistExpenseDetailID = i.ExpenseDetailID,
                    HistGOExpHist_Id = i.GOExpHist_Id,
                    Trans_Value_Date = i.GOExpHist_ValueDate,
                    Trans_Reference_No = i.GOExpHist_ReferenceNo,
                    Trans_Section = i.GOExpHist_Section,
                    Trans_Remarks = i.GOExpHist_Remarks,
                    Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                    Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                    Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                    Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                    Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                    Trans_Account_Name1_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, i.GOExpHist_Entry11Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                    Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                    Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                    Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details1_1 = i.GOExpHist_Entry11Details,
                    Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                    Trans_Division1_1 = i.GOExpHist_Entry11Division,
                    Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                    Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                    Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                    Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                    Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                    Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                    Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                    Trans_Account_Name1_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, i.GOExpHist_Entry12Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                    Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                    Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                    Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details1_2 = i.GOExpHist_Entry12Details,
                    Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                    Trans_Division1_2 = i.GOExpHist_Entry12Division,
                    Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                    Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                    Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                    Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                    Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                    Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                    Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                    Trans_Account_Name2_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, i.GOExpHist_Entry21Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                    Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                    Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                    Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details2_1 = i.GOExpHist_Entry21Details,
                    Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                    Trans_Division2_1 = i.GOExpHist_Entry21Division,
                    Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                    Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                    Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                    Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                    Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                    Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                    Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                    Trans_Account_Name2_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, i.GOExpHist_Entry22Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                    Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                    Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                    Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details2_2 = i.GOExpHist_Entry22Details,
                    Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                    Trans_Division2_2 = i.GOExpHist_Entry22Division,
                    Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                    Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                    Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                    Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                    Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                    Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                    Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                    Trans_Account_Name3_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, i.GOExpHist_Entry31Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                    Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                    Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                    Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details3_1 = i.GOExpHist_Entry31Details,
                    Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                    Trans_Division3_1 = i.GOExpHist_Entry31Division,
                    Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                    Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                    Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                    Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                    Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                    Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                    Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                    Trans_Account_Name3_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, i.GOExpHist_Entry32Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                    Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                    Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                    Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details3_2 = i.GOExpHist_Entry32Details,
                    Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                    Trans_Division3_2 = i.GOExpHist_Entry32Division,
                    Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                    Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                    Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                    Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                    Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                    Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                    Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                    Trans_Account_Name4_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, i.GOExpHist_Entry41Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                    Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                    Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                    Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details4_1 = i.GOExpHist_Entry41Details,
                    Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                    Trans_Division4_1 = i.GOExpHist_Entry41Division,
                    Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                    Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                    Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                    Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                    Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                    Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                    Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                    Trans_Account_Name4_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, i.GOExpHist_Entry42Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                    Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                    Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                    Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details4_2 = i.GOExpHist_Entry42Details,
                    Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                    Trans_Division4_2 = i.GOExpHist_Entry42Division,
                    Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                    Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                    TransTL_ID = i.TL_ID,
                    TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                    TransTL_TransID = i.TL_TransID

                });
            }

            var newList = list1.Concat(list2).Concat(list3);

            List<HomeReportAccountSummaryViewModel> list = new List<HomeReportAccountSummaryViewModel>();

            foreach (var i in newList)
            {
                if (!String.IsNullOrEmpty(i.Trans_Account_Number1_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit1_1,
                        Trans_Currency = i.Trans_Currency1_1,
                        Trans_Amount = i.Trans_Amount1_1,
                        Trans_Customer = i.Trans_Customer1_1,
                        Trans_Account_Code = i.Trans_Account_Code1_1,
                        Trans_Account_Number = i.Trans_Account_Number1_1,
                        Trans_Account_Name = i.Trans_Account_Name1_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate1_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency1_1,
                        Trans_Fund = i.Trans_Fund1_1,
                        Trans_Advice_Print = i.Trans_Advice_Print1_1,
                        Trans_Details = i.Trans_Details1_1,
                        Trans_Entity = i.Trans_Entity1_1,
                        Trans_Division = i.Trans_Division1_1,
                        Trans_InterAmount = i.Trans_InterAmount1_1,
                        Trans_InterRate = i.Trans_InterRate1_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number1_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit1_2,
                        Trans_Currency = i.Trans_Currency1_2,
                        Trans_Amount = i.Trans_Amount1_2,
                        Trans_Customer = i.Trans_Customer1_2,
                        Trans_Account_Code = i.Trans_Account_Code1_2,
                        Trans_Account_Number = i.Trans_Account_Number1_2,
                        Trans_Account_Name = i.Trans_Account_Name1_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate1_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency1_2,
                        Trans_Fund = i.Trans_Fund1_2,
                        Trans_Advice_Print = i.Trans_Advice_Print1_2,
                        Trans_Details = i.Trans_Details1_2,
                        Trans_Entity = i.Trans_Entity1_2,
                        Trans_Division = i.Trans_Division1_2,
                        Trans_InterAmount = i.Trans_InterAmount1_2,
                        Trans_InterRate = i.Trans_InterRate1_2
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number2_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit2_1,
                        Trans_Currency = i.Trans_Currency2_1,
                        Trans_Amount = i.Trans_Amount2_1,
                        Trans_Customer = i.Trans_Customer2_1,
                        Trans_Account_Code = i.Trans_Account_Code2_1,
                        Trans_Account_Number = i.Trans_Account_Number2_1,
                        Trans_Account_Name = i.Trans_Account_Name2_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate2_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency2_1,
                        Trans_Fund = i.Trans_Fund2_1,
                        Trans_Advice_Print = i.Trans_Advice_Print2_1,
                        Trans_Details = i.Trans_Details2_1,
                        Trans_Entity = i.Trans_Entity2_1,
                        Trans_Division = i.Trans_Division2_1,
                        Trans_InterAmount = i.Trans_InterAmount2_1,
                        Trans_InterRate = i.Trans_InterRate2_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number2_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit2_2,
                        Trans_Currency = i.Trans_Currency2_2,
                        Trans_Amount = i.Trans_Amount2_2,
                        Trans_Customer = i.Trans_Customer2_2,
                        Trans_Account_Code = i.Trans_Account_Code2_2,
                        Trans_Account_Number = i.Trans_Account_Number2_2,
                        Trans_Account_Name = i.Trans_Account_Name2_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate2_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency2_2,
                        Trans_Fund = i.Trans_Fund2_2,
                        Trans_Advice_Print = i.Trans_Advice_Print2_2,
                        Trans_Details = i.Trans_Details2_2,
                        Trans_Entity = i.Trans_Entity2_2,
                        Trans_Division = i.Trans_Division2_2,
                        Trans_InterAmount = i.Trans_InterAmount2_2,
                        Trans_InterRate = i.Trans_InterRate2_2
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number3_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit3_1,
                        Trans_Currency = i.Trans_Currency3_1,
                        Trans_Amount = i.Trans_Amount3_1,
                        Trans_Customer = i.Trans_Customer3_1,
                        Trans_Account_Code = i.Trans_Account_Code3_1,
                        Trans_Account_Number = i.Trans_Account_Number3_1,
                        Trans_Account_Name = i.Trans_Account_Name3_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate3_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency3_1,
                        Trans_Fund = i.Trans_Fund3_1,
                        Trans_Advice_Print = i.Trans_Advice_Print3_1,
                        Trans_Details = i.Trans_Details3_1,
                        Trans_Entity = i.Trans_Entity3_1,
                        Trans_Division = i.Trans_Division3_1,
                        Trans_InterAmount = i.Trans_InterAmount3_1,
                        Trans_InterRate = i.Trans_InterRate3_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number3_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit3_2,
                        Trans_Currency = i.Trans_Currency3_2,
                        Trans_Amount = i.Trans_Amount3_2,
                        Trans_Customer = i.Trans_Customer3_2,
                        Trans_Account_Code = i.Trans_Account_Code3_2,
                        Trans_Account_Number = i.Trans_Account_Number3_2,
                        Trans_Account_Name = i.Trans_Account_Name3_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate3_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency3_2,
                        Trans_Fund = i.Trans_Fund3_2,
                        Trans_Advice_Print = i.Trans_Advice_Print3_2,
                        Trans_Details = i.Trans_Details3_2,
                        Trans_Entity = i.Trans_Entity3_2,
                        Trans_Division = i.Trans_Division3_2,
                        Trans_InterAmount = i.Trans_InterAmount3_2,
                        Trans_InterRate = i.Trans_InterRate3_2
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number4_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit4_1,
                        Trans_Currency = i.Trans_Currency4_1,
                        Trans_Amount = i.Trans_Amount4_1,
                        Trans_Customer = i.Trans_Customer4_1,
                        Trans_Account_Code = i.Trans_Account_Code4_1,
                        Trans_Account_Number = i.Trans_Account_Number4_1,
                        Trans_Account_Name = i.Trans_Account_Name4_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate4_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency4_1,
                        Trans_Fund = i.Trans_Fund4_1,
                        Trans_Advice_Print = i.Trans_Advice_Print4_1,
                        Trans_Details = i.Trans_Details4_1,
                        Trans_Entity = i.Trans_Entity4_1,
                        Trans_Division = i.Trans_Division4_1,
                        Trans_InterAmount = i.Trans_InterAmount4_1,
                        Trans_InterRate = i.Trans_InterRate4_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number4_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit4_2,
                        Trans_Currency = i.Trans_Currency4_2,
                        Trans_Amount = i.Trans_Amount4_2,
                        Trans_Customer = i.Trans_Customer4_2,
                        Trans_Account_Code = i.Trans_Account_Code4_2,
                        Trans_Account_Number = i.Trans_Account_Number4_2,
                        Trans_Account_Name = i.Trans_Account_Name4_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate4_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency4_2,
                        Trans_Fund = i.Trans_Fund4_2,
                        Trans_Advice_Print = i.Trans_Advice_Print4_2,
                        Trans_Details = i.Trans_Details4_2,
                        Trans_Entity = i.Trans_Entity4_2,
                        Trans_Division = i.Trans_Division4_2,
                        Trans_InterAmount = i.Trans_InterAmount4_2,
                        Trans_InterRate = i.Trans_InterRate4_2
                    });
                }
            }
            if (!String.IsNullOrEmpty(selectedAccount.Account_No))
            {
                return list.Where(x => x.Trans_Account_Number == selectedAccount.Account_No
                    && x.Trans_Account_Code == selectedAccount.Account_Code).OrderBy(x => x.Trans_Value_Date);
            }
            return list.OrderBy(x => x.Trans_Value_Date);
        }

        public IEnumerable<HomeReportOutputAPSWT_MModel> GetBIRWTCSVData(HomeReportViewModel model)
        {
            string format = "yyyy-M";
            DateTime startDT = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
            DateTime endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);

            List<HomeReportOutputAPSWT_MModel> dbBIRCSV = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> dbBIRCSV_Check = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> dbBIRCSV_LIQ = new List<HomeReportOutputAPSWT_MModel>();
            List<HomeReportOutputAPSWT_MModel> dbBIRCSV_NC = new List<HomeReportOutputAPSWT_MModel>();

            var vatList = _context.DMVAT.ToList();

            //Get data from Taxable expense table except Cash Advance(SS) and Check payment.
            dbBIRCSV = (from expEntryDetl in _context.ExpenseEntryDetails
                        join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                        join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                        join vend in _context.DMVendor on expEntryDetl.ExpDtl_Ewt_Payor_Name_ID equals vend.Vendor_ID
                        where status.Contains(expense.Expense_Status)
                               && startDT.Date <= expense.Expense_Last_Updated.Date
                               && expense.Expense_Last_Updated.Date <= endDT.Date
                               && expense.Expense_Type != GlobalSystemValues.TYPE_SS
                               && expense.Expense_Type != GlobalSystemValues.TYPE_CV
                        orderby expense.Expense_Last_Updated
                        select new HomeReportOutputAPSWT_MModel
                        {
                            Payee = vend.Vendor_Name,
                            Tin = vend.Vendor_TIN,
                            ATC = tr.TR_ATC,
                            NOIP = tr.TR_Nature_Income_Payment,
                            //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                            //Example: 45,000 / 1.12 = 40,178.57
                            AOIP = (expEntryDetl.ExpDtl_Vat != 0) ? (expEntryDetl.ExpDtl_Debit / (decimal)(1 + vatList
                                    .Where(x => x.VAT_ID == expEntryDetl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : expEntryDetl.ExpDtl_Debit,
                            RateOfTax = tr.TR_Tax_Rate,
                            AOTW = expEntryDetl.ExpDtl_Credit_Ewt,
                            Last_Update_Date = expense.Expense_Last_Updated
                        }).ToList();

            //Get data from Taxable expense table for Check payment.
            dbBIRCSV_Check = (from expEntryDetl in _context.ExpenseEntryDetails
                              join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                              join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                              join vend in _context.DMVendor on expense.Expense_Payee equals vend.Vendor_ID
                              where status.Contains(expense.Expense_Status)
                                     && startDT.Date <= expense.Expense_Last_Updated.Date
                                     && expense.Expense_Last_Updated.Date <= endDT.Date
                                     && expense.Expense_Type == GlobalSystemValues.TYPE_CV
                                     && expense.Expense_Payee_Type == GlobalSystemValues.PAYEETYPE_VENDOR
                              orderby expense.Expense_Last_Updated
                              select new HomeReportOutputAPSWT_MModel
                              {
                                  Payee = vend.Vendor_Name,
                                  Tin = vend.Vendor_TIN,
                                  ATC = tr.TR_ATC,
                                  NOIP = tr.TR_Nature_Income_Payment,
                                  //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                                  //Example: 45,000 / 1.12 = 40,178.57
                                  AOIP = (expEntryDetl.ExpDtl_Vat != 0) ? (expEntryDetl.ExpDtl_Debit / (decimal)(1 + vatList
                                          .Where(x => x.VAT_ID == expEntryDetl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : expEntryDetl.ExpDtl_Debit,
                                  RateOfTax = tr.TR_Tax_Rate,
                                  AOTW = expEntryDetl.ExpDtl_Credit_Ewt,
                                  Last_Update_Date = expense.Expense_Last_Updated
                              }).ToList();

            //Get data from Taxable liquidation table.
            dbBIRCSV_LIQ = (from ie in _context.LiquidationInterEntity
                            join expDtl in _context.ExpenseEntryDetails on ie.ExpenseEntryDetailModel.ExpDtl_ID equals expDtl.ExpDtl_ID
                            join liqDtl in _context.LiquidationEntryDetails on expDtl.ExpenseEntryModel.Expense_ID equals liqDtl.ExpenseEntryModel.Expense_ID
                            join tr in _context.DMTR on ie.Liq_TaxRate equals tr.TR_ID
                            join vend in _context.DMVendor on ie.Liq_VendorID equals vend.Vendor_ID
                            where status.Contains(liqDtl.Liq_Status)
                                && startDT.Date <= liqDtl.Liq_LastUpdated_Date.Date
                                && liqDtl.Liq_LastUpdated_Date.Date <= endDT.Date
                            select new HomeReportOutputAPSWT_MModel
                            {
                                Payee = vend.Vendor_Name,
                                Tin = vend.Vendor_TIN,
                                ATC = tr.TR_ATC,
                                NOIP = tr.TR_Nature_Income_Payment,
                                //B. AMOUNT NET OF VAT = (GROSS AMOUNT/( 1 + VAT RATE))
                                //Example: 45,000 / 1.12 = 40,178.57
                                AOIP = (expDtl.ExpDtl_Vat != 0) ? ((ie.Liq_Amount_2_1 + ie.Liq_Amount_2_2 + ie.Liq_Amount_3_1) / (decimal)(1 + vatList
                                    .Where(x => x.VAT_ID == expDtl.ExpDtl_Vat).FirstOrDefault().VAT_Rate)) : (ie.Liq_Amount_2_1 + ie.Liq_Amount_2_2 + ie.Liq_Amount_3_1),
                                RateOfTax = tr.TR_Tax_Rate,
                                AOTW = ie.Liq_Amount_2_2,
                                Last_Update_Date = liqDtl.Liq_LastUpdated_Date
                            }).ToList();

            //Get data from Taxable Non-cash  table.
            dbBIRCSV_NC = (from ncDtl in _context.ExpenseEntryNonCashDetails
                           join ncAcc in _context.ExpenseEntryNonCashDetailAccounts on ncDtl.ExpNCDtl_ID equals ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID
                           join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
                           join exp in _context.ExpenseEntry on nc.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                           join tr in _context.DMTR on ncDtl.ExpNCDtl_TR_ID equals tr.TR_ID
                           join vend in _context.DMVendor on ncDtl.ExpNCDtl_Vendor_ID equals vend.Vendor_ID
                           join ncAcc2 in (from ncDtl2 in _context.ExpenseEntryNonCashDetails
                                           join ncAcc2 in _context.ExpenseEntryNonCashDetailAccounts on ncDtl2.ExpNCDtl_ID equals ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID
                                           where ncAcc2.ExpNCDtlAcc_Type_ID == 3
                                           select new { ncAcc2.ExpNCDtlAcc_Amount, ncAcc2.ExpenseEntryNCDtlModel.ExpNCDtl_ID }) on ncAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID equals ncAcc2.ExpNCDtl_ID
                           where status.Contains(exp.Expense_Status)
                           && ncAcc.ExpNCDtlAcc_Type_ID == 3
                              && startDT.Date <= exp.Expense_Last_Updated.Date
                              && exp.Expense_Last_Updated.Date <= endDT.Date
                           select new HomeReportOutputAPSWT_MModel
                           {
                               Payee = vend.Vendor_Name,
                               Tin = vend.Vendor_TIN,
                               RateOfTax = tr.TR_Tax_Rate,
                               ATC = tr.TR_ATC,
                               NOIP = tr.TR_Nature_Income_Payment,
                               AOIP = (ncDtl.ExpNCDtl_TaxBasedAmt != 0) ? ncDtl.ExpNCDtl_TaxBasedAmt : ncAcc.ExpNCDtlAcc_Amount,
                               AOTW = ncAcc2.ExpNCDtlAcc_Amount,
                               Last_Update_Date = exp.Expense_Last_Updated,
                               Vendor_masterID = vend.Vendor_MasterID
                           }).Where(x => x.AOTW > 0).ToList();

            return dbBIRCSV.Concat(dbBIRCSV_Check).Concat(dbBIRCSV_LIQ).Concat(dbBIRCSV_NC).OrderBy(x => x.Payee).ThenBy(x => x.RateOfTax);
        }

        public IEnumerable<HomeReportAccountSummaryViewModel> GetWithHoldingSummaryData(HomeReportViewModel model)
        {
            DateTime startDT = new DateTime();
            DateTime endDT = new DateTime();
            if (model.PeriodOption == 1)
            {
                startDT = DateTime.ParseExact(model.Year + "-" + model.Month, "yyyy-M", CultureInfo.InvariantCulture);
                endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, "yyyy-M", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }
            else
            {
                startDT = model.PeriodFrom;
                endDT = model.PeriodTo;
            }
            int[] TRMasterIDs = null;
            if (model.ReportSubType == 0)
            {
                TRMasterIDs = _context.DMTR.Select(x => x.TR_MasterID).Distinct().ToArray();
            }
            else
            {
                TRMasterIDs = new int[] { model.ReportSubType };
            }
            //Get DDV entry detail list. include inter entity
            List<EntryDDVViewModel> ddvDetails = GetEntryDetailsListForDDV();

            List<HomeReportTransactionListViewModel> list1 = new List<HomeReportTransactionListViewModel>();
            List<HomeReportTransactionListViewModel> list2 = new List<HomeReportTransactionListViewModel>();
            List<HomeReportTransactionListViewModel> list3 = new List<HomeReportTransactionListViewModel>();

            List<DMAccountModel> accList = getAccountListIncHist();
            List<int> ewtTaxes = new List<int> { int.Parse(xelemAcc.Element("C_CV1").Value), int.Parse(xelemAcc.Element("C_DDV1").Value),
                                                int.Parse(xelemAcc.Element("C_PC1").Value), int.Parse(xelemLiq.Element("ACCOUNT1_PHP").Value) };

            var WHTAccount = accList.Where(x => x.Account_MasterID == int.Parse(xelemReport.Element("WHT_ACCOUNT").Value)
                                        && x.Account_isActive == true && x.Account_isDeleted == false).FirstOrDefault();

            var db1 = (from hist in _context.GOExpressHist
                       join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                       join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                       join expDtl in _context.ExpenseEntryDetails on hist.ExpenseDetailID equals expDtl.ExpDtl_ID
                       join tr in _context.DMTR on expDtl.ExpDtl_Ewt equals tr.TR_ID
                       join acc in _context.DMAccount on expDtl.ExpDtl_CreditAccount1 equals acc.Account_ID
                       where exp.Expense_Type != GlobalSystemValues.TYPE_NC
                            && exp.Expense_Type != GlobalSystemValues.TYPE_SS
                            && startDT.Date <= exp.Expense_Last_Updated.Date
                            && exp.Expense_Last_Updated.Date <= endDT.Date
                            && ewtTaxes.Contains(acc.Account_MasterID)
                            && TRMasterIDs.Contains(tr.TR_MasterID)
                            && status.Contains(exp.Expense_Status)
                       select new
                       {
                           exp.Expense_ID,
                           exp.Expense_Type,
                           exp.Expense_Last_Updated,
                           exp.Expense_Date,
                           exp.Expense_Number,
                           exp.Expense_CheckNo,
                           expDtl.ExpDtl_Account,
                           expDtl.ExpDtl_CreditAccount1,
                           expDtl.ExpDtl_CreditAccount2,
                           hist.ExpenseEntryID,
                           hist.ExpenseDetailID,
                           hist.GOExpHist_Id,
                           hist.GOExpHist_ValueDate,
                           hist.GOExpHist_ReferenceNo,
                           hist.GOExpHist_Branchno,
                           hist.GOExpHist_Section,
                           hist.GOExpHist_Remarks,
                           hist.GOExpHist_Entry11Type,
                           hist.GOExpHist_Entry11Ccy,
                           hist.GOExpHist_Entry11Amt,
                           hist.GOExpHist_Entry11Cust,
                           hist.GOExpHist_Entry11Actcde,
                           hist.GOExpHist_Entry11ActType,
                           hist.GOExpHist_Entry11ActNo,
                           hist.GOExpHist_Entry11ExchRate,
                           hist.GOExpHist_Entry11ExchCcy,
                           hist.GOExpHist_Entry11Fund,
                           hist.GOExpHist_Entry11AdvcPrnt,
                           hist.GOExpHist_Entry11Details,
                           hist.GOExpHist_Entry11Entity,
                           hist.GOExpHist_Entry11Division,
                           hist.GOExpHist_Entry11InterAmt,
                           hist.GOExpHist_Entry11InterRate,
                           hist.GOExpHist_Entry12Type,
                           hist.GOExpHist_Entry12Ccy,
                           hist.GOExpHist_Entry12Amt,
                           hist.GOExpHist_Entry12Cust,
                           hist.GOExpHist_Entry12Actcde,
                           hist.GOExpHist_Entry12ActType,
                           hist.GOExpHist_Entry12ActNo,
                           hist.GOExpHist_Entry12ExchRate,
                           hist.GOExpHist_Entry12ExchCcy,
                           hist.GOExpHist_Entry12Fund,
                           hist.GOExpHist_Entry12AdvcPrnt,
                           hist.GOExpHist_Entry12Details,
                           hist.GOExpHist_Entry12Entity,
                           hist.GOExpHist_Entry12Division,
                           hist.GOExpHist_Entry12InterAmt,
                           hist.GOExpHist_Entry12InterRate,
                           hist.GOExpHist_Entry21Type,
                           hist.GOExpHist_Entry21Ccy,
                           hist.GOExpHist_Entry21Amt,
                           hist.GOExpHist_Entry21Cust,
                           hist.GOExpHist_Entry21Actcde,
                           hist.GOExpHist_Entry21ActType,
                           hist.GOExpHist_Entry21ActNo,
                           hist.GOExpHist_Entry21ExchRate,
                           hist.GOExpHist_Entry21ExchCcy,
                           hist.GOExpHist_Entry21Fund,
                           hist.GOExpHist_Entry21AdvcPrnt,
                           hist.GOExpHist_Entry21Details,
                           hist.GOExpHist_Entry21Entity,
                           hist.GOExpHist_Entry21Division,
                           hist.GOExpHist_Entry21InterAmt,
                           hist.GOExpHist_Entry21InterRate,
                           hist.GOExpHist_Entry22Type,
                           hist.GOExpHist_Entry22Ccy,
                           hist.GOExpHist_Entry22Amt,
                           hist.GOExpHist_Entry22Cust,
                           hist.GOExpHist_Entry22Actcde,
                           hist.GOExpHist_Entry22ActType,
                           hist.GOExpHist_Entry22ActNo,
                           hist.GOExpHist_Entry22ExchRate,
                           hist.GOExpHist_Entry22ExchCcy,
                           hist.GOExpHist_Entry22Fund,
                           hist.GOExpHist_Entry22AdvcPrnt,
                           hist.GOExpHist_Entry22Details,
                           hist.GOExpHist_Entry22Entity,
                           hist.GOExpHist_Entry22Division,
                           hist.GOExpHist_Entry22InterAmt,
                           hist.GOExpHist_Entry22InterRate,
                           hist.GOExpHist_Entry31Type,
                           hist.GOExpHist_Entry31Ccy,
                           hist.GOExpHist_Entry31Amt,
                           hist.GOExpHist_Entry31Cust,
                           hist.GOExpHist_Entry31Actcde,
                           hist.GOExpHist_Entry31ActType,
                           hist.GOExpHist_Entry31ActNo,
                           hist.GOExpHist_Entry31ExchRate,
                           hist.GOExpHist_Entry31ExchCcy,
                           hist.GOExpHist_Entry31Fund,
                           hist.GOExpHist_Entry31AdvcPrnt,
                           hist.GOExpHist_Entry31Details,
                           hist.GOExpHist_Entry31Entity,
                           hist.GOExpHist_Entry31Division,
                           hist.GOExpHist_Entry31InterAmt,
                           hist.GOExpHist_Entry31InterRate,
                           hist.GOExpHist_Entry32Type,
                           hist.GOExpHist_Entry32Ccy,
                           hist.GOExpHist_Entry32Amt,
                           hist.GOExpHist_Entry32Cust,
                           hist.GOExpHist_Entry32Actcde,
                           hist.GOExpHist_Entry32ActType,
                           hist.GOExpHist_Entry32ActNo,
                           hist.GOExpHist_Entry32ExchRate,
                           hist.GOExpHist_Entry32ExchCcy,
                           hist.GOExpHist_Entry32Fund,
                           hist.GOExpHist_Entry32AdvcPrnt,
                           hist.GOExpHist_Entry32Details,
                           hist.GOExpHist_Entry32Entity,
                           hist.GOExpHist_Entry32Division,
                           hist.GOExpHist_Entry32InterAmt,
                           hist.GOExpHist_Entry32InterRate,
                           hist.GOExpHist_Entry41Type,
                           hist.GOExpHist_Entry41Ccy,
                           hist.GOExpHist_Entry41Amt,
                           hist.GOExpHist_Entry41Cust,
                           hist.GOExpHist_Entry41Actcde,
                           hist.GOExpHist_Entry41ActType,
                           hist.GOExpHist_Entry41ActNo,
                           hist.GOExpHist_Entry41ExchRate,
                           hist.GOExpHist_Entry41ExchCcy,
                           hist.GOExpHist_Entry41Fund,
                           hist.GOExpHist_Entry41AdvcPrnt,
                           hist.GOExpHist_Entry41Details,
                           hist.GOExpHist_Entry41Entity,
                           hist.GOExpHist_Entry41Division,
                           hist.GOExpHist_Entry41InterAmt,
                           hist.GOExpHist_Entry41InterRate,
                           hist.GOExpHist_Entry42Type,
                           hist.GOExpHist_Entry42Ccy,
                           hist.GOExpHist_Entry42Amt,
                           hist.GOExpHist_Entry42Cust,
                           hist.GOExpHist_Entry42Actcde,
                           hist.GOExpHist_Entry42ActType,
                           hist.GOExpHist_Entry42ActNo,
                           hist.GOExpHist_Entry42ExchRate,
                           hist.GOExpHist_Entry42ExchCcy,
                           hist.GOExpHist_Entry42Fund,
                           hist.GOExpHist_Entry42AdvcPrnt,
                           hist.GOExpHist_Entry42Details,
                           hist.GOExpHist_Entry42Entity,
                           hist.GOExpHist_Entry42Division,
                           hist.GOExpHist_Entry42InterAmt,
                           hist.GOExpHist_Entry42InterRate,
                           trans.TL_ID,
                           trans.TL_GoExpress_ID,
                           trans.TL_TransID
                       }).ToList();

            //Convert to List object.
            foreach (var i in db1)
            {
                //Check record if liquidation or not.
                bool boolLiq = _context.ExpenseTransLists.Where(x => x.TL_ID == i.TL_ID).Select(x => x.TL_Liquidation).FirstOrDefault();
                list1.Add(new HomeReportTransactionListViewModel
                {
                    ExpExpense_ID = i.Expense_ID,
                    ExpExpense_Type = i.Expense_Type,
                    Trans_Last_Updated_Date = i.Expense_Last_Updated,
                    ExpExpense_Date = i.Expense_Date.ToString(),
                    Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, boolLiq),
                    Trans_Check_Number = i.Expense_CheckNo,
                    HistExpenseEntryID = i.ExpenseEntryID,
                    HistExpenseDetailID = i.ExpenseDetailID,
                    HistGOExpHist_Id = i.GOExpHist_Id,
                    Trans_Value_Date = i.GOExpHist_ValueDate,
                    Trans_Reference_No = i.GOExpHist_ReferenceNo,
                    Trans_Section = i.GOExpHist_Section,
                    Trans_Remarks = i.GOExpHist_Remarks,
                    Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                    Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                    Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                    Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                    Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                    Trans_Account_Name1_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, i.GOExpHist_Entry11Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                    Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                    Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                    Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details1_1 = i.GOExpHist_Entry11Details,
                    Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                    Trans_Division1_1 = i.GOExpHist_Entry11Division,
                    Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                    Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                    Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                    Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                    Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                    Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                    Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                    Trans_Account_Name1_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, i.GOExpHist_Entry12Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                    Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                    Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                    Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details1_2 = i.GOExpHist_Entry12Details,
                    Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                    Trans_Division1_2 = i.GOExpHist_Entry12Division,
                    Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                    Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                    Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                    Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                    Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                    Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                    Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                    Trans_Account_Name2_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, i.GOExpHist_Entry21Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                    Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                    Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                    Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details2_1 = i.GOExpHist_Entry21Details,
                    Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                    Trans_Division2_1 = i.GOExpHist_Entry21Division,
                    Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                    Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                    Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                    Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                    Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                    Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                    Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                    Trans_Account_Name2_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, i.GOExpHist_Entry22Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                    Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                    Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                    Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details2_2 = i.GOExpHist_Entry22Details,
                    Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                    Trans_Division2_2 = i.GOExpHist_Entry22Division,
                    Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                    Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                    Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                    Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                    Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                    Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                    Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                    Trans_Account_Name3_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, i.GOExpHist_Entry31Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                    Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                    Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                    Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details3_1 = i.GOExpHist_Entry31Details,
                    Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                    Trans_Division3_1 = i.GOExpHist_Entry31Division,
                    Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                    Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                    Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                    Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                    Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                    Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                    Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                    Trans_Account_Name3_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, i.GOExpHist_Entry32Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                    Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                    Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                    Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details3_2 = i.GOExpHist_Entry32Details,
                    Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                    Trans_Division3_2 = i.GOExpHist_Entry32Division,
                    Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                    Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                    Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                    Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                    Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                    Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                    Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                    Trans_Account_Name4_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, i.GOExpHist_Entry41Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                    Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                    Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                    Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details4_1 = i.GOExpHist_Entry41Details,
                    Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                    Trans_Division4_1 = i.GOExpHist_Entry41Division,
                    Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                    Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                    Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                    Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                    Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                    Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                    Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                    Trans_Account_Name4_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, i.GOExpHist_Entry42Ccy, i.ExpDtl_Account, i.ExpDtl_CreditAccount1, i.ExpDtl_CreditAccount2, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                    Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                    Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                    Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details4_2 = i.GOExpHist_Entry42Details,
                    Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                    Trans_Division4_2 = i.GOExpHist_Entry42Division,
                    Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                    Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                    TransTL_ID = i.TL_ID,
                    TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                    TransTL_TransID = i.TL_TransID

                });
            }

            //Liquidation
            var db2 = (from hist in _context.GOExpressHist
                       join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                       join liqDtl in _context.LiquidationEntryDetails on hist.ExpenseEntryID equals liqDtl.ExpenseEntryModel.Expense_ID
                       join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                       join ie in _context.LiquidationInterEntity on hist.ExpenseDetailID equals ie.ExpenseEntryDetailModel.ExpDtl_ID
                       join tr in _context.DMTR on ie.Liq_TaxRate equals tr.TR_ID
                       join acc in _context.DMAccount on ie.Liq_AccountID_2_2 equals acc.Account_ID
                       where exp.Expense_Type == GlobalSystemValues.TYPE_SS
                            && startDT.Date <= liqDtl.Liq_LastUpdated_Date.Date
                            && liqDtl.Liq_LastUpdated_Date.Date <= endDT.Date
                            && ewtTaxes.Contains(acc.Account_MasterID)
                            && TRMasterIDs.Contains(tr.TR_MasterID)
                            && status.Contains(liqDtl.Liq_Status)
                       select new
                       {
                           exp.Expense_ID,
                           exp.Expense_Type,
                           exp.Expense_Last_Updated,
                           exp.Expense_Date,
                           exp.Expense_Number,
                           exp.Expense_CheckNo,
                           hist.ExpenseEntryID,
                           hist.ExpenseDetailID,
                           hist.GOExpHist_Id,
                           hist.GOExpHist_ValueDate,
                           hist.GOExpHist_ReferenceNo,
                           hist.GOExpHist_Branchno,
                           hist.GOExpHist_Section,
                           hist.GOExpHist_Remarks,
                           hist.GOExpHist_Entry11Type,
                           hist.GOExpHist_Entry11Ccy,
                           hist.GOExpHist_Entry11Amt,
                           hist.GOExpHist_Entry11Cust,
                           hist.GOExpHist_Entry11Actcde,
                           hist.GOExpHist_Entry11ActType,
                           hist.GOExpHist_Entry11ActNo,
                           hist.GOExpHist_Entry11ExchRate,
                           hist.GOExpHist_Entry11ExchCcy,
                           hist.GOExpHist_Entry11Fund,
                           hist.GOExpHist_Entry11AdvcPrnt,
                           hist.GOExpHist_Entry11Details,
                           hist.GOExpHist_Entry11Entity,
                           hist.GOExpHist_Entry11Division,
                           hist.GOExpHist_Entry11InterAmt,
                           hist.GOExpHist_Entry11InterRate,
                           hist.GOExpHist_Entry12Type,
                           hist.GOExpHist_Entry12Ccy,
                           hist.GOExpHist_Entry12Amt,
                           hist.GOExpHist_Entry12Cust,
                           hist.GOExpHist_Entry12Actcde,
                           hist.GOExpHist_Entry12ActType,
                           hist.GOExpHist_Entry12ActNo,
                           hist.GOExpHist_Entry12ExchRate,
                           hist.GOExpHist_Entry12ExchCcy,
                           hist.GOExpHist_Entry12Fund,
                           hist.GOExpHist_Entry12AdvcPrnt,
                           hist.GOExpHist_Entry12Details,
                           hist.GOExpHist_Entry12Entity,
                           hist.GOExpHist_Entry12Division,
                           hist.GOExpHist_Entry12InterAmt,
                           hist.GOExpHist_Entry12InterRate,
                           hist.GOExpHist_Entry21Type,
                           hist.GOExpHist_Entry21Ccy,
                           hist.GOExpHist_Entry21Amt,
                           hist.GOExpHist_Entry21Cust,
                           hist.GOExpHist_Entry21Actcde,
                           hist.GOExpHist_Entry21ActType,
                           hist.GOExpHist_Entry21ActNo,
                           hist.GOExpHist_Entry21ExchRate,
                           hist.GOExpHist_Entry21ExchCcy,
                           hist.GOExpHist_Entry21Fund,
                           hist.GOExpHist_Entry21AdvcPrnt,
                           hist.GOExpHist_Entry21Details,
                           hist.GOExpHist_Entry21Entity,
                           hist.GOExpHist_Entry21Division,
                           hist.GOExpHist_Entry21InterAmt,
                           hist.GOExpHist_Entry21InterRate,
                           hist.GOExpHist_Entry22Type,
                           hist.GOExpHist_Entry22Ccy,
                           hist.GOExpHist_Entry22Amt,
                           hist.GOExpHist_Entry22Cust,
                           hist.GOExpHist_Entry22Actcde,
                           hist.GOExpHist_Entry22ActType,
                           hist.GOExpHist_Entry22ActNo,
                           hist.GOExpHist_Entry22ExchRate,
                           hist.GOExpHist_Entry22ExchCcy,
                           hist.GOExpHist_Entry22Fund,
                           hist.GOExpHist_Entry22AdvcPrnt,
                           hist.GOExpHist_Entry22Details,
                           hist.GOExpHist_Entry22Entity,
                           hist.GOExpHist_Entry22Division,
                           hist.GOExpHist_Entry22InterAmt,
                           hist.GOExpHist_Entry22InterRate,
                           hist.GOExpHist_Entry31Type,
                           hist.GOExpHist_Entry31Ccy,
                           hist.GOExpHist_Entry31Amt,
                           hist.GOExpHist_Entry31Cust,
                           hist.GOExpHist_Entry31Actcde,
                           hist.GOExpHist_Entry31ActType,
                           hist.GOExpHist_Entry31ActNo,
                           hist.GOExpHist_Entry31ExchRate,
                           hist.GOExpHist_Entry31ExchCcy,
                           hist.GOExpHist_Entry31Fund,
                           hist.GOExpHist_Entry31AdvcPrnt,
                           hist.GOExpHist_Entry31Details,
                           hist.GOExpHist_Entry31Entity,
                           hist.GOExpHist_Entry31Division,
                           hist.GOExpHist_Entry31InterAmt,
                           hist.GOExpHist_Entry31InterRate,
                           hist.GOExpHist_Entry32Type,
                           hist.GOExpHist_Entry32Ccy,
                           hist.GOExpHist_Entry32Amt,
                           hist.GOExpHist_Entry32Cust,
                           hist.GOExpHist_Entry32Actcde,
                           hist.GOExpHist_Entry32ActType,
                           hist.GOExpHist_Entry32ActNo,
                           hist.GOExpHist_Entry32ExchRate,
                           hist.GOExpHist_Entry32ExchCcy,
                           hist.GOExpHist_Entry32Fund,
                           hist.GOExpHist_Entry32AdvcPrnt,
                           hist.GOExpHist_Entry32Details,
                           hist.GOExpHist_Entry32Entity,
                           hist.GOExpHist_Entry32Division,
                           hist.GOExpHist_Entry32InterAmt,
                           hist.GOExpHist_Entry32InterRate,
                           hist.GOExpHist_Entry41Type,
                           hist.GOExpHist_Entry41Ccy,
                           hist.GOExpHist_Entry41Amt,
                           hist.GOExpHist_Entry41Cust,
                           hist.GOExpHist_Entry41Actcde,
                           hist.GOExpHist_Entry41ActType,
                           hist.GOExpHist_Entry41ActNo,
                           hist.GOExpHist_Entry41ExchRate,
                           hist.GOExpHist_Entry41ExchCcy,
                           hist.GOExpHist_Entry41Fund,
                           hist.GOExpHist_Entry41AdvcPrnt,
                           hist.GOExpHist_Entry41Details,
                           hist.GOExpHist_Entry41Entity,
                           hist.GOExpHist_Entry41Division,
                           hist.GOExpHist_Entry41InterAmt,
                           hist.GOExpHist_Entry41InterRate,
                           hist.GOExpHist_Entry42Type,
                           hist.GOExpHist_Entry42Ccy,
                           hist.GOExpHist_Entry42Amt,
                           hist.GOExpHist_Entry42Cust,
                           hist.GOExpHist_Entry42Actcde,
                           hist.GOExpHist_Entry42ActType,
                           hist.GOExpHist_Entry42ActNo,
                           hist.GOExpHist_Entry42ExchRate,
                           hist.GOExpHist_Entry42ExchCcy,
                           hist.GOExpHist_Entry42Fund,
                           hist.GOExpHist_Entry42AdvcPrnt,
                           hist.GOExpHist_Entry42Details,
                           hist.GOExpHist_Entry42Entity,
                           hist.GOExpHist_Entry42Division,
                           hist.GOExpHist_Entry42InterAmt,
                           hist.GOExpHist_Entry42InterRate,
                           trans.TL_ID,
                           trans.TL_GoExpress_ID,
                           trans.TL_TransID
                       }).ToList();

            //Convert to List object.
            foreach (var i in db2)
            {
                if (i.GOExpHist_Remarks != "S" + _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.ExpenseDetailID).FirstOrDefault().ExpDtl_Gbase_Remarks)
                {
                    continue;
                }

                list2.Add(new HomeReportTransactionListViewModel
                {
                    ExpExpense_ID = i.Expense_ID,
                    ExpExpense_Type = i.Expense_Type,
                    Trans_Last_Updated_Date = i.Expense_Last_Updated,
                    ExpExpense_Date = i.Expense_Date.ToString(),
                    Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, false),
                    Trans_Check_Number = i.Expense_CheckNo,
                    HistExpenseEntryID = i.ExpenseEntryID,
                    HistExpenseDetailID = i.ExpenseDetailID,
                    HistGOExpHist_Id = i.GOExpHist_Id,
                    Trans_Value_Date = i.GOExpHist_ValueDate,
                    Trans_Reference_No = i.GOExpHist_ReferenceNo,
                    Trans_Section = i.GOExpHist_Section,
                    Trans_Remarks = i.GOExpHist_Remarks,
                    Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                    Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                    Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                    Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                    Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                    Trans_Account_Name1_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, i.GOExpHist_Entry11Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                    Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                    Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                    Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details1_1 = i.GOExpHist_Entry11Details,
                    Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                    Trans_Division1_1 = i.GOExpHist_Entry11Division,
                    Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                    Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                    Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                    Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                    Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                    Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                    Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                    Trans_Account_Name1_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, i.GOExpHist_Entry12Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                    Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                    Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                    Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details1_2 = i.GOExpHist_Entry12Details,
                    Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                    Trans_Division1_2 = i.GOExpHist_Entry12Division,
                    Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                    Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                    Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                    Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                    Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                    Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                    Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                    Trans_Account_Name2_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, i.GOExpHist_Entry21Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                    Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                    Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                    Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details2_1 = i.GOExpHist_Entry21Details,
                    Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                    Trans_Division2_1 = i.GOExpHist_Entry21Division,
                    Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                    Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                    Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                    Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                    Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                    Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                    Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                    Trans_Account_Name2_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, i.GOExpHist_Entry22Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                    Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                    Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                    Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details2_2 = i.GOExpHist_Entry22Details,
                    Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                    Trans_Division2_2 = i.GOExpHist_Entry22Division,
                    Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                    Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                    Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                    Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                    Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                    Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                    Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                    Trans_Account_Name3_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, i.GOExpHist_Entry31Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                    Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                    Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                    Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details3_1 = i.GOExpHist_Entry31Details,
                    Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                    Trans_Division3_1 = i.GOExpHist_Entry31Division,
                    Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                    Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                    Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                    Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                    Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                    Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                    Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                    Trans_Account_Name3_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, i.GOExpHist_Entry32Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                    Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                    Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                    Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details3_2 = i.GOExpHist_Entry32Details,
                    Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                    Trans_Division3_2 = i.GOExpHist_Entry32Division,
                    Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                    Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                    Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                    Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                    Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                    Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                    Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                    Trans_Account_Name4_1 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, i.GOExpHist_Entry41Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                    Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                    Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                    Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details4_1 = i.GOExpHist_Entry41Details,
                    Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                    Trans_Division4_1 = i.GOExpHist_Entry41Division,
                    Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                    Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                    Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                    Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                    Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                    Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                    Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                    Trans_Account_Name4_2 = GetAccountNameForCADDVPCSS(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, i.GOExpHist_Entry42Ccy, 0, 0, 0, i.Expense_Type, ddvDetails, i.ExpenseEntryID, i.ExpenseDetailID),
                    Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                    Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                    Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                    Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details4_2 = i.GOExpHist_Entry42Details,
                    Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                    Trans_Division4_2 = i.GOExpHist_Entry42Division,
                    Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                    Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                    TransTL_ID = i.TL_ID,
                    TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                    TransTL_TransID = i.TL_TransID

                });
            }

            //Non-cash
            var db3 = (from hist in _context.GOExpressHist
                       join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                       join ncDtl in _context.ExpenseEntryNonCashDetails on hist.ExpenseDetailID equals ncDtl.ExpNCDtl_ID
                       join nc in _context.ExpenseEntryNonCash on ncDtl.ExpenseEntryNCModel.ExpNC_ID equals nc.ExpNC_ID
                       join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                       join tr in _context.DMTR on ncDtl.ExpNCDtl_TR_ID equals tr.TR_ID
                       where exp.Expense_Type == GlobalSystemValues.TYPE_NC
                            && startDT.Date <= exp.Expense_Last_Updated.Date
                            && exp.Expense_Last_Updated.Date <= endDT.Date
                            && TRMasterIDs.Contains(tr.TR_MasterID)
                            && status.Contains(exp.Expense_Status)
                       select new
                       {
                           exp.Expense_ID,
                           exp.Expense_Type,
                           exp.Expense_Last_Updated,
                           exp.Expense_Date,
                           exp.Expense_Number,
                           exp.Expense_CheckNo,
                           hist.ExpenseEntryID,
                           hist.ExpenseDetailID,
                           hist.GOExpHist_Id,
                           nc.ExpNC_Category_ID,
                           hist.GOExpHist_ValueDate,
                           hist.GOExpHist_ReferenceNo,
                           hist.GOExpHist_Branchno,
                           hist.GOExpHist_Section,
                           hist.GOExpHist_Remarks,
                           hist.GOExpHist_Entry11Type,
                           hist.GOExpHist_Entry11Ccy,
                           hist.GOExpHist_Entry11Amt,
                           hist.GOExpHist_Entry11Cust,
                           hist.GOExpHist_Entry11Actcde,
                           hist.GOExpHist_Entry11ActType,
                           hist.GOExpHist_Entry11ActNo,
                           hist.GOExpHist_Entry11ExchRate,
                           hist.GOExpHist_Entry11ExchCcy,
                           hist.GOExpHist_Entry11Fund,
                           hist.GOExpHist_Entry11AdvcPrnt,
                           hist.GOExpHist_Entry11Details,
                           hist.GOExpHist_Entry11Entity,
                           hist.GOExpHist_Entry11Division,
                           hist.GOExpHist_Entry11InterAmt,
                           hist.GOExpHist_Entry11InterRate,
                           hist.GOExpHist_Entry12Type,
                           hist.GOExpHist_Entry12Ccy,
                           hist.GOExpHist_Entry12Amt,
                           hist.GOExpHist_Entry12Cust,
                           hist.GOExpHist_Entry12Actcde,
                           hist.GOExpHist_Entry12ActType,
                           hist.GOExpHist_Entry12ActNo,
                           hist.GOExpHist_Entry12ExchRate,
                           hist.GOExpHist_Entry12ExchCcy,
                           hist.GOExpHist_Entry12Fund,
                           hist.GOExpHist_Entry12AdvcPrnt,
                           hist.GOExpHist_Entry12Details,
                           hist.GOExpHist_Entry12Entity,
                           hist.GOExpHist_Entry12Division,
                           hist.GOExpHist_Entry12InterAmt,
                           hist.GOExpHist_Entry12InterRate,
                           hist.GOExpHist_Entry21Type,
                           hist.GOExpHist_Entry21Ccy,
                           hist.GOExpHist_Entry21Amt,
                           hist.GOExpHist_Entry21Cust,
                           hist.GOExpHist_Entry21Actcde,
                           hist.GOExpHist_Entry21ActType,
                           hist.GOExpHist_Entry21ActNo,
                           hist.GOExpHist_Entry21ExchRate,
                           hist.GOExpHist_Entry21ExchCcy,
                           hist.GOExpHist_Entry21Fund,
                           hist.GOExpHist_Entry21AdvcPrnt,
                           hist.GOExpHist_Entry21Details,
                           hist.GOExpHist_Entry21Entity,
                           hist.GOExpHist_Entry21Division,
                           hist.GOExpHist_Entry21InterAmt,
                           hist.GOExpHist_Entry21InterRate,
                           hist.GOExpHist_Entry22Type,
                           hist.GOExpHist_Entry22Ccy,
                           hist.GOExpHist_Entry22Amt,
                           hist.GOExpHist_Entry22Cust,
                           hist.GOExpHist_Entry22Actcde,
                           hist.GOExpHist_Entry22ActType,
                           hist.GOExpHist_Entry22ActNo,
                           hist.GOExpHist_Entry22ExchRate,
                           hist.GOExpHist_Entry22ExchCcy,
                           hist.GOExpHist_Entry22Fund,
                           hist.GOExpHist_Entry22AdvcPrnt,
                           hist.GOExpHist_Entry22Details,
                           hist.GOExpHist_Entry22Entity,
                           hist.GOExpHist_Entry22Division,
                           hist.GOExpHist_Entry22InterAmt,
                           hist.GOExpHist_Entry22InterRate,
                           hist.GOExpHist_Entry31Type,
                           hist.GOExpHist_Entry31Ccy,
                           hist.GOExpHist_Entry31Amt,
                           hist.GOExpHist_Entry31Cust,
                           hist.GOExpHist_Entry31Actcde,
                           hist.GOExpHist_Entry31ActType,
                           hist.GOExpHist_Entry31ActNo,
                           hist.GOExpHist_Entry31ExchRate,
                           hist.GOExpHist_Entry31ExchCcy,
                           hist.GOExpHist_Entry31Fund,
                           hist.GOExpHist_Entry31AdvcPrnt,
                           hist.GOExpHist_Entry31Details,
                           hist.GOExpHist_Entry31Entity,
                           hist.GOExpHist_Entry31Division,
                           hist.GOExpHist_Entry31InterAmt,
                           hist.GOExpHist_Entry31InterRate,
                           hist.GOExpHist_Entry32Type,
                           hist.GOExpHist_Entry32Ccy,
                           hist.GOExpHist_Entry32Amt,
                           hist.GOExpHist_Entry32Cust,
                           hist.GOExpHist_Entry32Actcde,
                           hist.GOExpHist_Entry32ActType,
                           hist.GOExpHist_Entry32ActNo,
                           hist.GOExpHist_Entry32ExchRate,
                           hist.GOExpHist_Entry32ExchCcy,
                           hist.GOExpHist_Entry32Fund,
                           hist.GOExpHist_Entry32AdvcPrnt,
                           hist.GOExpHist_Entry32Details,
                           hist.GOExpHist_Entry32Entity,
                           hist.GOExpHist_Entry32Division,
                           hist.GOExpHist_Entry32InterAmt,
                           hist.GOExpHist_Entry32InterRate,
                           hist.GOExpHist_Entry41Type,
                           hist.GOExpHist_Entry41Ccy,
                           hist.GOExpHist_Entry41Amt,
                           hist.GOExpHist_Entry41Cust,
                           hist.GOExpHist_Entry41Actcde,
                           hist.GOExpHist_Entry41ActType,
                           hist.GOExpHist_Entry41ActNo,
                           hist.GOExpHist_Entry41ExchRate,
                           hist.GOExpHist_Entry41ExchCcy,
                           hist.GOExpHist_Entry41Fund,
                           hist.GOExpHist_Entry41AdvcPrnt,
                           hist.GOExpHist_Entry41Details,
                           hist.GOExpHist_Entry41Entity,
                           hist.GOExpHist_Entry41Division,
                           hist.GOExpHist_Entry41InterAmt,
                           hist.GOExpHist_Entry41InterRate,
                           hist.GOExpHist_Entry42Type,
                           hist.GOExpHist_Entry42Ccy,
                           hist.GOExpHist_Entry42Amt,
                           hist.GOExpHist_Entry42Cust,
                           hist.GOExpHist_Entry42Actcde,
                           hist.GOExpHist_Entry42ActType,
                           hist.GOExpHist_Entry42ActNo,
                           hist.GOExpHist_Entry42ExchRate,
                           hist.GOExpHist_Entry42ExchCcy,
                           hist.GOExpHist_Entry42Fund,
                           hist.GOExpHist_Entry42AdvcPrnt,
                           hist.GOExpHist_Entry42Details,
                           hist.GOExpHist_Entry42Entity,
                           hist.GOExpHist_Entry42Division,
                           hist.GOExpHist_Entry42InterAmt,
                           hist.GOExpHist_Entry42InterRate,
                           trans.TL_ID,
                           trans.TL_GoExpress_ID,
                           trans.TL_TransID
                       }).ToList();

            List<ExpenseEntryNCDtlViewModel> ncDtlList = GetEntryDetailAccountListForNonCash();

            //Convert to List object.
            foreach (var i in db3)
            {
                list3.Add(new HomeReportTransactionListViewModel
                {
                    ExpExpense_ID = i.Expense_ID,
                    ExpExpense_Type = i.Expense_Type,
                    Trans_Last_Updated_Date = i.Expense_Last_Updated,
                    ExpExpense_Date = i.Expense_Date.ToString(),
                    Trans_Voucher_Number = getVoucherNo(i.Expense_Type, i.Expense_Date, i.Expense_Number, false),
                    Trans_Check_Number = i.Expense_CheckNo,
                    HistExpenseEntryID = i.ExpenseEntryID,
                    HistExpenseDetailID = i.ExpenseDetailID,
                    HistGOExpHist_Id = i.GOExpHist_Id,
                    NCExpNC_Category_ID = i.ExpNC_Category_ID,
                    Trans_Value_Date = i.GOExpHist_ValueDate,
                    Trans_Reference_No = i.GOExpHist_ReferenceNo,
                    Trans_Section = i.GOExpHist_Section,
                    Trans_Remarks = i.GOExpHist_Remarks,
                    Trans_DebitCredit1_1 = i.GOExpHist_Entry11Type,
                    Trans_Currency1_1 = i.GOExpHist_Entry11Ccy,
                    Trans_Amount1_1 = i.GOExpHist_Entry11Amt,
                    Trans_Customer1_1 = i.GOExpHist_Entry11Cust,
                    Trans_Account_Code1_1 = i.GOExpHist_Entry11Actcde,
                    Trans_Account_Name1_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry11ActType, i.GOExpHist_Entry11ActNo, i.GOExpHist_Entry11Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number1_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry11ActType)) ? i.GOExpHist_Entry11ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry11ActNo : "",
                    Trans_Exchange_Rate1_1 = i.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency1_1 = i.GOExpHist_Entry11ExchCcy,
                    Trans_Fund1_1 = i.GOExpHist_Entry11Fund,
                    Trans_Advice_Print1_1 = i.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details1_1 = i.GOExpHist_Entry11Details,
                    Trans_Entity1_1 = i.GOExpHist_Entry11Entity,
                    Trans_Division1_1 = i.GOExpHist_Entry11Division,
                    Trans_InterAmount1_1 = i.GOExpHist_Entry11InterAmt,
                    Trans_InterRate1_1 = i.GOExpHist_Entry11InterRate,
                    Trans_DebitCredit1_2 = i.GOExpHist_Entry12Type,
                    Trans_Currency1_2 = i.GOExpHist_Entry12Ccy,
                    Trans_Amount1_2 = i.GOExpHist_Entry12Amt,
                    Trans_Customer1_2 = i.GOExpHist_Entry12Cust,
                    Trans_Account_Code1_2 = i.GOExpHist_Entry12Actcde,
                    Trans_Account_Name1_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry12ActType, i.GOExpHist_Entry12ActNo, i.GOExpHist_Entry12Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number1_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry12ActType)) ? i.GOExpHist_Entry12ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry12ActNo : "",
                    Trans_Exchange_Rate1_2 = i.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency1_2 = i.GOExpHist_Entry12ExchCcy,
                    Trans_Fund1_2 = i.GOExpHist_Entry12Fund,
                    Trans_Advice_Print1_2 = i.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details1_2 = i.GOExpHist_Entry12Details,
                    Trans_Entity1_2 = i.GOExpHist_Entry12Entity,
                    Trans_Division1_2 = i.GOExpHist_Entry12Division,
                    Trans_InterAmount1_2 = i.GOExpHist_Entry12InterAmt,
                    Trans_InterRate1_2 = i.GOExpHist_Entry12InterRate,
                    Trans_DebitCredit2_1 = i.GOExpHist_Entry21Type,
                    Trans_Currency2_1 = i.GOExpHist_Entry21Ccy,
                    Trans_Amount2_1 = i.GOExpHist_Entry21Amt,
                    Trans_Customer2_1 = i.GOExpHist_Entry21Cust,
                    Trans_Account_Code2_1 = i.GOExpHist_Entry21Actcde,
                    Trans_Account_Name2_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry21ActType, i.GOExpHist_Entry21ActNo, i.GOExpHist_Entry21Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number2_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry21ActType)) ? i.GOExpHist_Entry21ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry21ActNo : "",
                    Trans_Exchange_Rate2_1 = i.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency2_1 = i.GOExpHist_Entry21ExchCcy,
                    Trans_Fund2_1 = i.GOExpHist_Entry21Fund,
                    Trans_Advice_Print2_1 = i.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details2_1 = i.GOExpHist_Entry21Details,
                    Trans_Entity2_1 = i.GOExpHist_Entry21Entity,
                    Trans_Division2_1 = i.GOExpHist_Entry21Division,
                    Trans_InterAmount2_1 = i.GOExpHist_Entry21InterAmt,
                    Trans_InterRate2_1 = i.GOExpHist_Entry21InterRate,
                    Trans_DebitCredit2_2 = i.GOExpHist_Entry22Type,
                    Trans_Currency2_2 = i.GOExpHist_Entry22Ccy,
                    Trans_Amount2_2 = i.GOExpHist_Entry22Amt,
                    Trans_Customer2_2 = i.GOExpHist_Entry22Cust,
                    Trans_Account_Code2_2 = i.GOExpHist_Entry22Actcde,
                    Trans_Account_Name2_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry22ActType, i.GOExpHist_Entry22ActNo, i.GOExpHist_Entry22Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number2_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry22ActType)) ? i.GOExpHist_Entry22ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry22ActNo : "",
                    Trans_Exchange_Rate2_2 = i.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency2_2 = i.GOExpHist_Entry22ExchCcy,
                    Trans_Fund2_2 = i.GOExpHist_Entry22Fund,
                    Trans_Advice_Print2_2 = i.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details2_2 = i.GOExpHist_Entry22Details,
                    Trans_Entity2_2 = i.GOExpHist_Entry22Entity,
                    Trans_Division2_2 = i.GOExpHist_Entry22Division,
                    Trans_InterAmount2_2 = i.GOExpHist_Entry22InterAmt,
                    Trans_InterRate2_2 = i.GOExpHist_Entry22InterRate,
                    Trans_DebitCredit3_1 = i.GOExpHist_Entry31Type,
                    Trans_Currency3_1 = i.GOExpHist_Entry31Ccy,
                    Trans_Amount3_1 = i.GOExpHist_Entry31Amt,
                    Trans_Customer3_1 = i.GOExpHist_Entry31Cust,
                    Trans_Account_Code3_1 = i.GOExpHist_Entry31Actcde,
                    Trans_Account_Name3_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry31ActType, i.GOExpHist_Entry31ActNo, i.GOExpHist_Entry31Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number3_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry31ActType)) ? i.GOExpHist_Entry31ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry31ActNo : "",
                    Trans_Exchange_Rate3_1 = i.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency3_1 = i.GOExpHist_Entry31ExchCcy,
                    Trans_Fund3_1 = i.GOExpHist_Entry31Fund,
                    Trans_Advice_Print3_1 = i.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details3_1 = i.GOExpHist_Entry31Details,
                    Trans_Entity3_1 = i.GOExpHist_Entry31Entity,
                    Trans_Division3_1 = i.GOExpHist_Entry31Division,
                    Trans_InterAmount3_1 = i.GOExpHist_Entry31InterAmt,
                    Trans_InterRate3_1 = i.GOExpHist_Entry31InterRate,
                    Trans_DebitCredit3_2 = i.GOExpHist_Entry32Type,
                    Trans_Currency3_2 = i.GOExpHist_Entry32Ccy,
                    Trans_Amount3_2 = i.GOExpHist_Entry32Amt,
                    Trans_Customer3_2 = i.GOExpHist_Entry32Cust,
                    Trans_Account_Code3_2 = i.GOExpHist_Entry32Actcde,
                    Trans_Account_Name3_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry32ActType, i.GOExpHist_Entry32ActNo, i.GOExpHist_Entry32Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number3_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry32ActType)) ? i.GOExpHist_Entry32ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry32ActNo : "",
                    Trans_Exchange_Rate3_2 = i.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency3_2 = i.GOExpHist_Entry32ExchCcy,
                    Trans_Fund3_2 = i.GOExpHist_Entry32Fund,
                    Trans_Advice_Print3_2 = i.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details3_2 = i.GOExpHist_Entry32Details,
                    Trans_Entity3_2 = i.GOExpHist_Entry32Entity,
                    Trans_Division3_2 = i.GOExpHist_Entry32Division,
                    Trans_InterAmount3_2 = i.GOExpHist_Entry32InterAmt,
                    Trans_InterRate3_2 = i.GOExpHist_Entry32InterRate,
                    Trans_DebitCredit4_1 = i.GOExpHist_Entry41Type,
                    Trans_Currency4_1 = i.GOExpHist_Entry41Ccy,
                    Trans_Amount4_1 = i.GOExpHist_Entry41Amt,
                    Trans_Customer4_1 = i.GOExpHist_Entry41Cust,
                    Trans_Account_Code4_1 = i.GOExpHist_Entry41Actcde,
                    Trans_Account_Name4_1 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry41ActType, i.GOExpHist_Entry41ActNo, i.GOExpHist_Entry41Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number4_1 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry41ActType)) ? i.GOExpHist_Entry41ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry41ActNo : "",
                    Trans_Exchange_Rate4_1 = i.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency4_1 = i.GOExpHist_Entry41ExchCcy,
                    Trans_Fund4_1 = i.GOExpHist_Entry41Fund,
                    Trans_Advice_Print4_1 = i.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details4_1 = i.GOExpHist_Entry41Details,
                    Trans_Entity4_1 = i.GOExpHist_Entry41Entity,
                    Trans_Division4_1 = i.GOExpHist_Entry41Division,
                    Trans_InterAmount4_1 = i.GOExpHist_Entry41InterAmt,
                    Trans_InterRate4_1 = i.GOExpHist_Entry41InterRate,
                    Trans_DebitCredit4_2 = i.GOExpHist_Entry42Type,
                    Trans_Currency4_2 = i.GOExpHist_Entry42Ccy,
                    Trans_Amount4_2 = i.GOExpHist_Entry42Amt,
                    Trans_Customer4_2 = i.GOExpHist_Entry42Cust,
                    Trans_Account_Code4_2 = i.GOExpHist_Entry42Actcde,
                    Trans_Account_Name4_2 = GetAccountNameForNonCash(accList, i.GOExpHist_Entry42ActType, i.GOExpHist_Entry42ActNo, i.GOExpHist_Entry42Actcde, ncDtlList, i.ExpenseDetailID),
                    Trans_Account_Number4_2 = (!String.IsNullOrEmpty(i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo) && !String.IsNullOrEmpty(i.GOExpHist_Entry42ActType)) ? i.GOExpHist_Entry42ActType + "-" + i.GOExpHist_Branchno + "-" + i.GOExpHist_Entry42ActNo : "",
                    Trans_Exchange_Rate4_2 = i.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency4_2 = i.GOExpHist_Entry42ExchCcy,
                    Trans_Fund4_2 = i.GOExpHist_Entry42Fund,
                    Trans_Advice_Print4_2 = i.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details4_2 = i.GOExpHist_Entry42Details,
                    Trans_Entity4_2 = i.GOExpHist_Entry42Entity,
                    Trans_Division4_2 = i.GOExpHist_Entry42Division,
                    Trans_InterAmount4_2 = i.GOExpHist_Entry42InterAmt,
                    Trans_InterRate4_2 = i.GOExpHist_Entry42InterRate,
                    TransTL_ID = i.TL_ID,
                    TransTL_GoExpress_ID = i.TL_GoExpress_ID,
                    TransTL_TransID = i.TL_TransID

                });
            }

            var newList = list1.Concat(list2).Concat(list3);

            List<HomeReportAccountSummaryViewModel> list = new List<HomeReportAccountSummaryViewModel>();

            foreach (var i in newList)
            {
                if (!String.IsNullOrEmpty(i.Trans_Account_Number1_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit1_1,
                        Trans_Currency = i.Trans_Currency1_1,
                        Trans_Amount = i.Trans_Amount1_1,
                        Trans_Customer = i.Trans_Customer1_1,
                        Trans_Account_Code = i.Trans_Account_Code1_1,
                        Trans_Account_Number = i.Trans_Account_Number1_1,
                        Trans_Account_Name = i.Trans_Account_Name1_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate1_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency1_1,
                        Trans_Fund = i.Trans_Fund1_1,
                        Trans_Advice_Print = i.Trans_Advice_Print1_1,
                        Trans_Details = i.Trans_Details1_1,
                        Trans_Entity = i.Trans_Entity1_1,
                        Trans_Division = i.Trans_Division1_1,
                        Trans_InterAmount = i.Trans_InterAmount1_1,
                        Trans_InterRate = i.Trans_InterRate1_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number1_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit1_2,
                        Trans_Currency = i.Trans_Currency1_2,
                        Trans_Amount = i.Trans_Amount1_2,
                        Trans_Customer = i.Trans_Customer1_2,
                        Trans_Account_Code = i.Trans_Account_Code1_2,
                        Trans_Account_Number = i.Trans_Account_Number1_2,
                        Trans_Account_Name = i.Trans_Account_Name1_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate1_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency1_2,
                        Trans_Fund = i.Trans_Fund1_2,
                        Trans_Advice_Print = i.Trans_Advice_Print1_2,
                        Trans_Details = i.Trans_Details1_2,
                        Trans_Entity = i.Trans_Entity1_2,
                        Trans_Division = i.Trans_Division1_2,
                        Trans_InterAmount = i.Trans_InterAmount1_2,
                        Trans_InterRate = i.Trans_InterRate1_2
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number2_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit2_1,
                        Trans_Currency = i.Trans_Currency2_1,
                        Trans_Amount = i.Trans_Amount2_1,
                        Trans_Customer = i.Trans_Customer2_1,
                        Trans_Account_Code = i.Trans_Account_Code2_1,
                        Trans_Account_Number = i.Trans_Account_Number2_1,
                        Trans_Account_Name = i.Trans_Account_Name2_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate2_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency2_1,
                        Trans_Fund = i.Trans_Fund2_1,
                        Trans_Advice_Print = i.Trans_Advice_Print2_1,
                        Trans_Details = i.Trans_Details2_1,
                        Trans_Entity = i.Trans_Entity2_1,
                        Trans_Division = i.Trans_Division2_1,
                        Trans_InterAmount = i.Trans_InterAmount2_1,
                        Trans_InterRate = i.Trans_InterRate2_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number2_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit2_2,
                        Trans_Currency = i.Trans_Currency2_2,
                        Trans_Amount = i.Trans_Amount2_2,
                        Trans_Customer = i.Trans_Customer2_2,
                        Trans_Account_Code = i.Trans_Account_Code2_2,
                        Trans_Account_Number = i.Trans_Account_Number2_2,
                        Trans_Account_Name = i.Trans_Account_Name2_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate2_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency2_2,
                        Trans_Fund = i.Trans_Fund2_2,
                        Trans_Advice_Print = i.Trans_Advice_Print2_2,
                        Trans_Details = i.Trans_Details2_2,
                        Trans_Entity = i.Trans_Entity2_2,
                        Trans_Division = i.Trans_Division2_2,
                        Trans_InterAmount = i.Trans_InterAmount2_2,
                        Trans_InterRate = i.Trans_InterRate2_2
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number3_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit3_1,
                        Trans_Currency = i.Trans_Currency3_1,
                        Trans_Amount = i.Trans_Amount3_1,
                        Trans_Customer = i.Trans_Customer3_1,
                        Trans_Account_Code = i.Trans_Account_Code3_1,
                        Trans_Account_Number = i.Trans_Account_Number3_1,
                        Trans_Account_Name = i.Trans_Account_Name3_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate3_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency3_1,
                        Trans_Fund = i.Trans_Fund3_1,
                        Trans_Advice_Print = i.Trans_Advice_Print3_1,
                        Trans_Details = i.Trans_Details3_1,
                        Trans_Entity = i.Trans_Entity3_1,
                        Trans_Division = i.Trans_Division3_1,
                        Trans_InterAmount = i.Trans_InterAmount3_1,
                        Trans_InterRate = i.Trans_InterRate3_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number3_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit3_2,
                        Trans_Currency = i.Trans_Currency3_2,
                        Trans_Amount = i.Trans_Amount3_2,
                        Trans_Customer = i.Trans_Customer3_2,
                        Trans_Account_Code = i.Trans_Account_Code3_2,
                        Trans_Account_Number = i.Trans_Account_Number3_2,
                        Trans_Account_Name = i.Trans_Account_Name3_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate3_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency3_2,
                        Trans_Fund = i.Trans_Fund3_2,
                        Trans_Advice_Print = i.Trans_Advice_Print3_2,
                        Trans_Details = i.Trans_Details3_2,
                        Trans_Entity = i.Trans_Entity3_2,
                        Trans_Division = i.Trans_Division3_2,
                        Trans_InterAmount = i.Trans_InterAmount3_2,
                        Trans_InterRate = i.Trans_InterRate3_2
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number4_1))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit4_1,
                        Trans_Currency = i.Trans_Currency4_1,
                        Trans_Amount = i.Trans_Amount4_1,
                        Trans_Customer = i.Trans_Customer4_1,
                        Trans_Account_Code = i.Trans_Account_Code4_1,
                        Trans_Account_Number = i.Trans_Account_Number4_1,
                        Trans_Account_Name = i.Trans_Account_Name4_1,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate4_1,
                        Trans_Contra_Currency = i.Trans_Contra_Currency4_1,
                        Trans_Fund = i.Trans_Fund4_1,
                        Trans_Advice_Print = i.Trans_Advice_Print4_1,
                        Trans_Details = i.Trans_Details4_1,
                        Trans_Entity = i.Trans_Entity4_1,
                        Trans_Division = i.Trans_Division4_1,
                        Trans_InterAmount = i.Trans_InterAmount4_1,
                        Trans_InterRate = i.Trans_InterRate4_1
                    });
                }

                if (!String.IsNullOrEmpty(i.Trans_Account_Number4_2))
                {
                    list.Add(new HomeReportAccountSummaryViewModel
                    {
                        Trans_Voucher_Number = i.Trans_Voucher_Number,
                        Trans_Check_Number = i.Trans_Check_Number,
                        Trans_Value_Date = i.Trans_Value_Date,
                        Trans_Reference_No = i.Trans_Reference_No,
                        Trans_Section = i.Trans_Section,
                        Trans_Remarks = i.Trans_Remarks,
                        Trans_DebitCredit = i.Trans_DebitCredit4_2,
                        Trans_Currency = i.Trans_Currency4_2,
                        Trans_Amount = i.Trans_Amount4_2,
                        Trans_Customer = i.Trans_Customer4_2,
                        Trans_Account_Code = i.Trans_Account_Code4_2,
                        Trans_Account_Number = i.Trans_Account_Number4_2,
                        Trans_Account_Name = i.Trans_Account_Name4_2,
                        Trans_Exchange_Rate = i.Trans_Exchange_Rate4_2,
                        Trans_Contra_Currency = i.Trans_Contra_Currency4_2,
                        Trans_Fund = i.Trans_Fund4_2,
                        Trans_Advice_Print = i.Trans_Advice_Print4_2,
                        Trans_Details = i.Trans_Details4_2,
                        Trans_Entity = i.Trans_Entity4_2,
                        Trans_Division = i.Trans_Division4_2,
                        Trans_InterAmount = i.Trans_InterAmount4_2,
                        Trans_InterRate = i.Trans_InterRate4_2
                    });
                }
            }

            return list.Where(x => x.Trans_Account_Number == WHTAccount.Account_No
                                && x.Trans_Account_Code == WHTAccount.Account_Code).OrderBy(x => x.Trans_Value_Date);
        }

        public IEnumerable<HomeReportESAMSViewModel> GetESAMSData(HomeReportViewModel model)
        {
            List<HomeReportESAMSViewModel> esamsData = new List<HomeReportESAMSViewModel>();
            DateTime startDT = model.PeriodFrom;
            DateTime endDT = model.PeriodTo;
            DMAccountModel selectedAccount = _context.DMAccount.Where(x => x.Account_ID == model.ReportSubType).FirstOrDefault();
            List<DMAccountModel> accList = getAccountListIncHist();
            List<UserModel> userList = getAllUsers();
            List<int> usedGOExpHistID = new List<int>();
            var getAllReversalEntry = _context.ReversalEntry.ToList();

            //Get all debit transaction of selected suspense account transactions in GOExpress
            var dbDebit = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           where trans.TL_Liquidation == false &&
                                (exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING ||
                                 exp.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING ||
                                 (exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED &&
                                  !getAllReversalEntry.Select(x => x.Reversal_GOExpressHistID).Contains(hist.GOExpHist_Id))) &&
                                   ((hist.GOExpHist_Entry11Type == "D" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                                    (hist.GOExpHist_Entry12Type == "D" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                                    (hist.GOExpHist_Entry21Type == "D" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                                    (hist.GOExpHist_Entry22Type == "D" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                                    (hist.GOExpHist_Entry31Type == "D" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                                    (hist.GOExpHist_Entry32Type == "D" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                                    (hist.GOExpHist_Entry41Type == "D" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                                    (hist.GOExpHist_Entry42Type == "D" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               exp.Expense_Creator_ID,
                               exp.Expense_Approver,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID,
                               exp.Expense_Status, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                               hist.GOExpHist_Id,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_Liquidation
                           }).ToList();

            //Get all liquidated credit transaction of selected suspense account transactions in GOExpress
            var dbLiq = (from hist in _context.GOExpressHist
                         join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                         join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                         join liqDtl in _context.LiquidationEntryDetails on hist.ExpenseEntryID equals liqDtl.ExpenseEntryModel.Expense_ID
                         where status.Contains(liqDtl.Liq_Status) &&
                                 ((hist.GOExpHist_Entry11Type == "C" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                                  (hist.GOExpHist_Entry12Type == "C" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                                  (hist.GOExpHist_Entry21Type == "C" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                                  (hist.GOExpHist_Entry22Type == "C" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                                  (hist.GOExpHist_Entry31Type == "C" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                                  (hist.GOExpHist_Entry32Type == "C" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                                  (hist.GOExpHist_Entry41Type == "C" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                                  (hist.GOExpHist_Entry42Type == "C" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                         select new
                         {
                             exp.Expense_ID,
                             exp.Expense_Type,
                             exp.Expense_Last_Updated,
                             exp.Expense_Date,
                             exp.Expense_Number,
                             exp.Expense_CheckNo,
                             exp.Expense_Creator_ID,
                             exp.Expense_Approver,
                             hist.ExpenseEntryID,
                             hist.ExpenseDetailID, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                             hist.GOExpHist_Id,
                             hist.GOExpHist_ValueDate,
                             hist.GOExpHist_Remarks,
                             hist.GOExpHist_Branchno,
                             hist.GOExpHist_Entry11Type,
                             hist.GOExpHist_Entry11Amt,
                             hist.GOExpHist_Entry11Actcde,
                             hist.GOExpHist_Entry11ActType,
                             hist.GOExpHist_Entry11ActNo,
                             hist.GOExpHist_Entry12Type,
                             hist.GOExpHist_Entry12Amt,
                             hist.GOExpHist_Entry12Actcde,
                             hist.GOExpHist_Entry12ActType,
                             hist.GOExpHist_Entry12ActNo,
                             hist.GOExpHist_Entry21Type,
                             hist.GOExpHist_Entry21Amt,
                             hist.GOExpHist_Entry21Actcde,
                             hist.GOExpHist_Entry21ActType,
                             hist.GOExpHist_Entry21ActNo,
                             hist.GOExpHist_Entry22Type,
                             hist.GOExpHist_Entry22Amt,
                             hist.GOExpHist_Entry22Actcde,
                             hist.GOExpHist_Entry22ActType,
                             hist.GOExpHist_Entry22ActNo,
                             hist.GOExpHist_Entry31Type,
                             hist.GOExpHist_Entry31Amt,
                             hist.GOExpHist_Entry31Actcde,
                             hist.GOExpHist_Entry31ActType,
                             hist.GOExpHist_Entry31ActNo,
                             hist.GOExpHist_Entry32Type,
                             hist.GOExpHist_Entry32Amt,
                             hist.GOExpHist_Entry32Actcde,
                             hist.GOExpHist_Entry32ActType,
                             hist.GOExpHist_Entry32ActNo,
                             hist.GOExpHist_Entry41Type,
                             hist.GOExpHist_Entry41Amt,
                             hist.GOExpHist_Entry41Actcde,
                             hist.GOExpHist_Entry41ActType,
                             hist.GOExpHist_Entry41ActNo,
                             hist.GOExpHist_Entry42Type,
                             hist.GOExpHist_Entry42Amt,
                             hist.GOExpHist_Entry42Actcde,
                             hist.GOExpHist_Entry42ActType,
                             hist.GOExpHist_Entry42ActNo,
                             trans.TL_ID,
                             trans.TL_GoExpress_ID,
                             trans.TL_TransID,
                             trans.TL_Liquidation
                         }).ToList();

            //Get all reversed(CA) credit transaction of selected suspense account transactions in GOExpress
            var dbCARev = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           join rev in _context.ReversalEntry on hist.GOExpHist_Id equals rev.Reversal_GOExpressHistID
                           where exp.Expense_Type == GlobalSystemValues.TYPE_SS &&
                                (exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED ||
                                 exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR) &&
                                ((hist.GOExpHist_Entry11Type == "C" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                                 (hist.GOExpHist_Entry12Type == "C" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                                 (hist.GOExpHist_Entry21Type == "C" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                                 (hist.GOExpHist_Entry22Type == "C" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                                 (hist.GOExpHist_Entry31Type == "C" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                                 (hist.GOExpHist_Entry32Type == "C" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                                 (hist.GOExpHist_Entry41Type == "C" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                                 (hist.GOExpHist_Entry42Type == "C" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               exp.Expense_Creator_ID,
                               exp.Expense_Approver,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                               hist.GOExpHist_Id,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_Liquidation
                           }).ToList();

            //Get all Non-cash credit transaction of selected suspense account transactions in GOExpress
            var dbCANC = (from hist in _context.GOExpressHist
                          join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                          join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                          where trans.TL_Liquidation == false &&
                                (exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING ||
                                 exp.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING ||
                                (exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED &&
                                 getAllReversalEntry.Select(x => x.Reversal_GOExpressHistID).Contains(hist.GOExpHist_Id))) &&
                              ((hist.GOExpHist_Entry11Type == "C" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                               (hist.GOExpHist_Entry12Type == "C" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                               (hist.GOExpHist_Entry21Type == "C" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                               (hist.GOExpHist_Entry22Type == "C" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                               (hist.GOExpHist_Entry31Type == "C" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                               (hist.GOExpHist_Entry32Type == "C" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                               (hist.GOExpHist_Entry41Type == "C" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                               (hist.GOExpHist_Entry42Type == "C" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                          select new
                          {
                              exp.Expense_ID,
                              exp.Expense_Type,
                              exp.Expense_Last_Updated,
                              exp.Expense_Date,
                              exp.Expense_Number,
                              exp.Expense_CheckNo,
                              exp.Expense_Creator_ID,
                              exp.Expense_Approver,
                              hist.ExpenseEntryID,
                              hist.ExpenseDetailID, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                              hist.GOExpHist_Id,
                              hist.GOExpHist_ValueDate,
                              hist.GOExpHist_Remarks,
                              hist.GOExpHist_Branchno,
                              hist.GOExpHist_Entry11Type,
                              hist.GOExpHist_Entry11Amt,
                              hist.GOExpHist_Entry11Actcde,
                              hist.GOExpHist_Entry11ActType,
                              hist.GOExpHist_Entry11ActNo,
                              hist.GOExpHist_Entry12Type,
                              hist.GOExpHist_Entry12Amt,
                              hist.GOExpHist_Entry12Actcde,
                              hist.GOExpHist_Entry12ActType,
                              hist.GOExpHist_Entry12ActNo,
                              hist.GOExpHist_Entry21Type,
                              hist.GOExpHist_Entry21Amt,
                              hist.GOExpHist_Entry21Actcde,
                              hist.GOExpHist_Entry21ActType,
                              hist.GOExpHist_Entry21ActNo,
                              hist.GOExpHist_Entry22Type,
                              hist.GOExpHist_Entry22Amt,
                              hist.GOExpHist_Entry22Actcde,
                              hist.GOExpHist_Entry22ActType,
                              hist.GOExpHist_Entry22ActNo,
                              hist.GOExpHist_Entry31Type,
                              hist.GOExpHist_Entry31Amt,
                              hist.GOExpHist_Entry31Actcde,
                              hist.GOExpHist_Entry31ActType,
                              hist.GOExpHist_Entry31ActNo,
                              hist.GOExpHist_Entry32Type,
                              hist.GOExpHist_Entry32Amt,
                              hist.GOExpHist_Entry32Actcde,
                              hist.GOExpHist_Entry32ActType,
                              hist.GOExpHist_Entry32ActNo,
                              hist.GOExpHist_Entry41Type,
                              hist.GOExpHist_Entry41Amt,
                              hist.GOExpHist_Entry41Actcde,
                              hist.GOExpHist_Entry41ActType,
                              hist.GOExpHist_Entry41ActNo,
                              hist.GOExpHist_Entry42Type,
                              hist.GOExpHist_Entry42Amt,
                              hist.GOExpHist_Entry42Actcde,
                              hist.GOExpHist_Entry42ActType,
                              hist.GOExpHist_Entry42ActNo,
                              trans.TL_ID,
                              trans.TL_GoExpress_ID,
                              trans.TL_TransID,
                              trans.TL_Liquidation
                          }).ToList();

            //Process 1:
            //Check reversal entry of Cash Advance if exist.
            //Process 2:
            //Check Liquidation entry. Must be For Closing or For Printing status
            //Process 3:
            //Filter All GOExpress that credited to selected suspense account

            int cnt = 0;
            foreach (var i in dbDebit)
            {
                cnt++;
                bool flag = false;

                //Add debited suspense account transaction
                esamsData.Add(AddToESAMViewList(cnt, i, selectedAccount, userList, GlobalSystemValues.TRANS_DEBIT));

                //============================SPECIAL REQUEST START======================================
                //2019/12/20 requested by Sir Albert.
                //Requested email proof: 2019/12/20 11:13AM
                //Email title: ExPReSS - Sundry/Suspense Control Sheet
                //Hardcode the specific liquidation entry in Non-cash due to wrong remarks inputted by user.
                //Cannot modify the remarks due to mismatch record with G-base side and ExPReSS side.
                //Only applicable to below account:
                //Account no: H79-767-151446
                //Account code: 14025
                //Account name: ADVANCE EXPENSES
                //Condition to add this record:
                //1. Selected account info must be mentioned account info above
                //2. Based on the database of ExPReSS in Mizuho bank, target record is Seq.No.17.
                if (cnt == 17 && selectedAccount.Account_No.Replace("-", "") == "H79767151446" && selectedAccount.Account_Code == "14025")
                {
                    esamsData.Add(new HomeReportESAMSViewModel
                    {
                        SeqNo = "(17)",
                        DebCredType = "C",
                        GbaseRemark = "AOCBDSH:MEETING AC HEALTH",
                        SettleDate = ConvGbDateToDateTime("112719"),
                        DRAmount = 0,
                        CRAmount = 3431.25M,
                        BudgetAmount = 0,
                        Balance = 0,
                        DHName = "",
                        ApprvName = "Advincula, Albert",
                        MakerName = "Madrinan, Tonierose"
                    });
                    continue;
                }
                //============================SPECIAL REQUEST END======================================

                //Check if debited transaction's expense entry was reversed or not.
                var revCA = dbCARev.Where(x => x.ExpenseEntryID == i.ExpenseEntryID &&
                                               x.ExpenseDetailID == i.ExpenseDetailID).ToList();
                if (revCA.Count > 0)
                {
                    foreach (var credRev in revCA)
                    {
                        if (!usedGOExpHistID.Contains(credRev.GOExpHist_Id))
                        {
                            HomeReportESAMSViewModel res = AddToESAMViewList(cnt, credRev, selectedAccount, userList, GlobalSystemValues.TRANS_CREDIT);
                            if (res != null)
                            {
                                res.Remarks = HomeReportConstantValue.ESAMS_CA_REV_MESSAGE;
                                esamsData.Add(res);
                                flag = true;
                                usedGOExpHistID.Add(credRev.GOExpHist_Id);
                                break;
                            }
                        }
                    }
                }
                if (flag) continue;

                //Check if debited transaction's expense entry was liquidated or not.
                var liqCA = dbLiq.Where(x => x.ExpenseEntryID == i.ExpenseEntryID &&
                                               x.ExpenseDetailID == i.ExpenseDetailID).ToList();
                if (liqCA.Count > 0)
                {
                    RepESAMSViewModel newLiq = new RepESAMSViewModel();
                    foreach (var credLiq in liqCA)
                    {
                        if (!usedGOExpHistID.Contains(credLiq.GOExpHist_Id))
                        {
                            //populate new LIQ viewmodel
                            newLiq = PopulateNewLiq(newLiq, credLiq);

                            if ((credLiq.GOExpHist_Entry11Type == "C") ||
                               (credLiq.GOExpHist_Entry12Type == "C") ||
                               (credLiq.GOExpHist_Entry21Type == "C") ||
                               (credLiq.GOExpHist_Entry22Type == "C") ||
                               (credLiq.GOExpHist_Entry31Type == "C") ||
                               (credLiq.GOExpHist_Entry32Type == "C") ||
                               (credLiq.GOExpHist_Entry41Type == "C") ||
                               (credLiq.GOExpHist_Entry42Type == "C"))
                            {
                                //add up values of similar entries
                                newLiq.GOExpHist_Entry11Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry11Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry11Amt ?? "0"));
                                newLiq.GOExpHist_Entry12Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry12Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry12Amt ?? "0"));
                                newLiq.GOExpHist_Entry21Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry21Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry21Amt ?? "0"));
                                newLiq.GOExpHist_Entry22Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry22Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry22Amt ?? "0"));
                                newLiq.GOExpHist_Entry31Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry31Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry31Amt ?? "0"));
                                newLiq.GOExpHist_Entry32Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry32Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry32Amt ?? "0"));
                                newLiq.GOExpHist_Entry41Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry41Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry41Amt ?? "0"));
                                newLiq.GOExpHist_Entry42Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry42Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry42Amt ?? "0"));
                            }
                        }
                    }

                    decimal debitAmount = (esamsData.LastOrDefault() != null) ? esamsData.LastOrDefault().DRAmount : 0.00M;
                    //compare if LIQ amount equates to the debit amount from CA
                    if ((newLiq.GOExpHist_Entry11Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry11Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry12Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry12Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry21Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry21Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry22Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry22Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry31Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry31Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry32Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry32Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry41Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry41Amt) == debitAmount) ||
                       (newLiq.GOExpHist_Entry42Type == "C" && decimal.Parse(newLiq.GOExpHist_Entry42Amt) == debitAmount))
                    {
                        HomeReportESAMSViewModel res = AddToESAMViewList(cnt, newLiq, selectedAccount, userList, GlobalSystemValues.TRANS_CREDIT);
                        if (res != null)
                        {
                            res.Remarks = "";
                            esamsData.Add(res);
                            flag = true;
                            usedGOExpHistID.Add(newLiq.GOExpHist_Id);
                            //break; //<-- ADDED COMMENT
                        }
                    }
                }
                if (flag) continue;

                //Check if CA Entry's liquidation status is in REVERSED or REVERSED DUE TO G-BASE ERROR.
                //If yes, check the non-cash transaction to find its liquidation entry.
                //If not, will not proceed to non-cash entry since it was not yet processed in liquidation screen.
                if (i.Expense_Type == GlobalSystemValues.TYPE_SS)
                {
                    var liqExist = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == i.Expense_ID).FirstOrDefault();
                    int liqstat = (liqExist != null) ? liqExist.Liq_Status : 0;

                    if (liqstat != GlobalSystemValues.STATUS_REVERSED &&
                        liqstat != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR) continue;
                }

                //Filter to value > debit transaction
                var ncCA = dbCANC.Where(x => ConvGbDateToDateTime(i.GOExpHist_ValueDate).Date <= ConvGbDateToDateTime(x.GOExpHist_ValueDate).Date).ToList();

                if (ncCA.Count > 0)
                {
                    string prefix = (i.Expense_Type == GlobalSystemValues.TYPE_SS || status.Contains(i.Expense_Status)) ? "S" : "";
                    string credNCRemark = "";
                    RepESAMSViewModel newLiqCA = new RepESAMSViewModel();

                    foreach (var credNC in ncCA)
                    {
                        if (!usedGOExpHistID.Contains(credNC.GOExpHist_Id))
                        {
                            //populate new LIQ viewmodel
                            newLiqCA = PopulateNewLiq(newLiqCA, credNC);
                            credNCRemark = (credNC.GOExpHist_Remarks.Trim().Length == 30) ? credNC.GOExpHist_Remarks.Trim().Substring(0, 29) : credNC.GOExpHist_Remarks.Trim();

                            if ((prefix + i.GOExpHist_Remarks.Trim()).Contains(credNCRemark) &&
                                ((credNC.GOExpHist_Entry11Type == "C") ||
                                 (credNC.GOExpHist_Entry12Type == "C") ||
                                 (credNC.GOExpHist_Entry21Type == "C") ||
                                 (credNC.GOExpHist_Entry22Type == "C") ||
                                 (credNC.GOExpHist_Entry31Type == "C") ||
                                 (credNC.GOExpHist_Entry32Type == "C") ||
                                 (credNC.GOExpHist_Entry41Type == "C") ||
                                 (credNC.GOExpHist_Entry42Type == "C")))
                            {
                                //add up values of similar entries
                                newLiqCA.GOExpHist_Entry11Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry11Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry11Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry12Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry12Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry12Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry21Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry21Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry21Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry22Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry22Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry22Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry31Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry31Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry31Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry32Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry32Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry32Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry41Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry41Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry41Amt ?? "0"));
                                newLiqCA.GOExpHist_Entry42Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry42Amt ?? "0") + decimal.Parse(newLiqCA.GOExpHist_Entry42Amt ?? "0"));
                            }
                        }
                    }

                    decimal debitAmount = (esamsData.LastOrDefault() != null) ? esamsData.LastOrDefault().DRAmount : 0.00M;
                    credNCRemark = (newLiqCA.GOExpHist_Remarks.Trim().Length == 30) ? newLiqCA.GOExpHist_Remarks.Trim().Substring(0, 29) : newLiqCA.GOExpHist_Remarks.Trim();

                    if ((prefix + i.GOExpHist_Remarks.Trim()).Contains(credNCRemark) &&
                        ((newLiqCA.GOExpHist_Entry11Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry11Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry12Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry12Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry21Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry21Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry22Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry22Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry31Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry31Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry32Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry32Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry41Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry41Amt) == debitAmount) ||
                         (newLiqCA.GOExpHist_Entry42Type == "C" && decimal.Parse(newLiqCA.GOExpHist_Entry42Amt) == debitAmount)))
                    {
                        HomeReportESAMSViewModel res = AddToESAMViewList(cnt, newLiqCA, selectedAccount, userList, GlobalSystemValues.TRANS_CREDIT);
                        if (res != null)
                        {
                            res.Remarks = (!String.IsNullOrEmpty(prefix)) ? HomeReportConstantValue.ESAMS_NC_MESSAGE : HomeReportConstantValue.ESAMS_CA_REV_MESSAGE;
                            esamsData.Add(res);
                            flag = true;
                            usedGOExpHistID.Add(newLiqCA.GOExpHist_Id);
                            //break; //<-- ADDED COMMENT
                        }
                    }
                }
            }

            var esamsDataSorted = esamsData.OrderBy(x => x.SettleDate).ThenBy(x => int.Parse(x.SeqNo.Replace("(", "").Replace(")", "")));
            decimal totBal = 0.00M;

            foreach (var i in esamsDataSorted)
            {
                if (i.DRAmount > 0)
                {
                    i.Balance = totBal = totBal + i.DRAmount;
                }
                else
                {
                    i.Balance = totBal = totBal - i.CRAmount;
                }
            }

            return esamsDataSorted.Where(x => startDT.Date <= x.SettleDate.Date && x.SettleDate.Date <= endDT.Date);
        }

        private HomeReportESAMSViewModel AddToESAMViewList(int seqno, dynamic record, DMAccountModel selectedAccount, List<UserModel> userList, int type)
        {
            var maker = userList.Where(x => x.User_ID == record.Expense_Creator_ID).FirstOrDefault();
            var approver = userList.Where(x => x.User_ID == record.Expense_Approver).FirstOrDefault();
            string seqNumber = (type == GlobalSystemValues.TRANS_DEBIT) ? seqno.ToString() : "(" + seqno + ")";

            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry11ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry11ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry11Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry11Type == "D") ? Decimal.Parse(record.GOExpHist_Entry11Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry11Type == "C") ? Decimal.Parse(record.GOExpHist_Entry11Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry12ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry12ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry12Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry12Type == "D") ? Decimal.Parse(record.GOExpHist_Entry12Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry12Type == "C") ? Decimal.Parse(record.GOExpHist_Entry12Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry21ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry21ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry21Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry21Type == "D") ? Decimal.Parse(record.GOExpHist_Entry21Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry21Type == "C") ? Decimal.Parse(record.GOExpHist_Entry21Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry22ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry22ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry22Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry22Type == "D") ? Decimal.Parse(record.GOExpHist_Entry22Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry22Type == "C") ? Decimal.Parse(record.GOExpHist_Entry22Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry31ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry31ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry31Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry31Type == "D") ? Decimal.Parse(record.GOExpHist_Entry31Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry31Type == "C") ? Decimal.Parse(record.GOExpHist_Entry31Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry32ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry32ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry32Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry32Type == "D") ? Decimal.Parse(record.GOExpHist_Entry32Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry32Type == "C") ? Decimal.Parse(record.GOExpHist_Entry32Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry41ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry41ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry41Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry41Type == "D") ? Decimal.Parse(record.GOExpHist_Entry41Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry41Type == "C") ? Decimal.Parse(record.GOExpHist_Entry41Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry42ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry42ActNo)
            {
                return new HomeReportESAMSViewModel
                {
                    SeqNo = seqNumber,
                    DebCredType = record.GOExpHist_Entry42Type,
                    GbaseRemark = record.GOExpHist_Remarks,
                    SettleDate = ConvGbDateToDateTime(record.GOExpHist_ValueDate),
                    DRAmount = (record.GOExpHist_Entry42Type == "D") ? Decimal.Parse(record.GOExpHist_Entry42Amt) : 0,
                    CRAmount = (record.GOExpHist_Entry42Type == "C") ? Decimal.Parse(record.GOExpHist_Entry42Amt) : 0,
                    BudgetAmount = 0,
                    Balance = 0,
                    DHName = "",
                    ApprvName = approver.User_LName + ", " + approver.User_FName,
                    MakerName = maker.User_LName + ", " + maker.User_FName
                };
            }

            return null;
        }

        public IEnumerable<HomeReportAccountSummaryViewModel> GetOutstandingAdvancesData(HomeReportViewModel model)
        {
            List<HomeReportAccountSummaryViewModel> outstandResult = new List<HomeReportAccountSummaryViewModel>();

            DateTime startDT = model.PeriodFrom;
            DateTime endDT = model.PeriodTo;
            DMAccountModel selectedAccount = _context.DMAccount.Where(x => x.Account_ID == model.ReportSubType).FirstOrDefault();
            List<DMAccountModel> accList = getAccountListIncHist();
            List<UserModel> userList = getAllUsers();
            List<int> usedGOExpHistID = new List<int>();
            var getAllReversalEntry = _context.ReversalEntry.ToList();

            //Get all debit transaction of selected suspense account transactions in GOExpress
            var dbDebit = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           where trans.TL_Liquidation == false &&
                                (exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING ||
                                 exp.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING ||
                                 (exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED &&
                                  !getAllReversalEntry.Select(x => x.Reversal_GOExpressHistID).Contains(hist.GOExpHist_Id))) &&
                                   ((hist.GOExpHist_Entry11Type == "D" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                                    (hist.GOExpHist_Entry12Type == "D" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                                    (hist.GOExpHist_Entry21Type == "D" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                                    (hist.GOExpHist_Entry22Type == "D" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                                    (hist.GOExpHist_Entry31Type == "D" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                                    (hist.GOExpHist_Entry32Type == "D" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                                    (hist.GOExpHist_Entry41Type == "D" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                                    (hist.GOExpHist_Entry42Type == "D" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               exp.Expense_Creator_ID,
                               exp.Expense_Approver,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID,
                               exp.Expense_Status, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                               hist.GOExpHist_Id,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_ReferenceNo,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Section,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Ccy,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Cust,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry11ExchRate,
                               hist.GOExpHist_Entry11ExchCcy,
                               hist.GOExpHist_Entry11Fund,
                               hist.GOExpHist_Entry11AdvcPrnt,
                               hist.GOExpHist_Entry11Details,
                               hist.GOExpHist_Entry11Entity,
                               hist.GOExpHist_Entry11Division,
                               hist.GOExpHist_Entry11InterAmt,
                               hist.GOExpHist_Entry11InterRate,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Ccy,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Cust,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry12ExchRate,
                               hist.GOExpHist_Entry12ExchCcy,
                               hist.GOExpHist_Entry12Fund,
                               hist.GOExpHist_Entry12AdvcPrnt,
                               hist.GOExpHist_Entry12Details,
                               hist.GOExpHist_Entry12Entity,
                               hist.GOExpHist_Entry12Division,
                               hist.GOExpHist_Entry12InterAmt,
                               hist.GOExpHist_Entry12InterRate,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Ccy,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Cust,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry21ExchRate,
                               hist.GOExpHist_Entry21ExchCcy,
                               hist.GOExpHist_Entry21Fund,
                               hist.GOExpHist_Entry21AdvcPrnt,
                               hist.GOExpHist_Entry21Details,
                               hist.GOExpHist_Entry21Entity,
                               hist.GOExpHist_Entry21Division,
                               hist.GOExpHist_Entry21InterAmt,
                               hist.GOExpHist_Entry21InterRate,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Ccy,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Cust,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry22ExchRate,
                               hist.GOExpHist_Entry22ExchCcy,
                               hist.GOExpHist_Entry22Fund,
                               hist.GOExpHist_Entry22AdvcPrnt,
                               hist.GOExpHist_Entry22Details,
                               hist.GOExpHist_Entry22Entity,
                               hist.GOExpHist_Entry22Division,
                               hist.GOExpHist_Entry22InterAmt,
                               hist.GOExpHist_Entry22InterRate,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Ccy,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Cust,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry31ExchRate,
                               hist.GOExpHist_Entry31ExchCcy,
                               hist.GOExpHist_Entry31Fund,
                               hist.GOExpHist_Entry31AdvcPrnt,
                               hist.GOExpHist_Entry31Details,
                               hist.GOExpHist_Entry31Entity,
                               hist.GOExpHist_Entry31Division,
                               hist.GOExpHist_Entry31InterAmt,
                               hist.GOExpHist_Entry31InterRate,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Ccy,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Cust,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry32ExchRate,
                               hist.GOExpHist_Entry32ExchCcy,
                               hist.GOExpHist_Entry32Fund,
                               hist.GOExpHist_Entry32AdvcPrnt,
                               hist.GOExpHist_Entry32Details,
                               hist.GOExpHist_Entry32Entity,
                               hist.GOExpHist_Entry32Division,
                               hist.GOExpHist_Entry32InterAmt,
                               hist.GOExpHist_Entry32InterRate,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Ccy,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Cust,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry41ExchRate,
                               hist.GOExpHist_Entry41ExchCcy,
                               hist.GOExpHist_Entry41Fund,
                               hist.GOExpHist_Entry41AdvcPrnt,
                               hist.GOExpHist_Entry41Details,
                               hist.GOExpHist_Entry41Entity,
                               hist.GOExpHist_Entry41Division,
                               hist.GOExpHist_Entry41InterAmt,
                               hist.GOExpHist_Entry41InterRate,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Ccy,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Cust,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               hist.GOExpHist_Entry42ExchRate,
                               hist.GOExpHist_Entry42ExchCcy,
                               hist.GOExpHist_Entry42Fund,
                               hist.GOExpHist_Entry42AdvcPrnt,
                               hist.GOExpHist_Entry42Details,
                               hist.GOExpHist_Entry42Entity,
                               hist.GOExpHist_Entry42Division,
                               hist.GOExpHist_Entry42InterAmt,
                               hist.GOExpHist_Entry42InterRate,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_Liquidation
                           }).ToList();

            //Get all liquidated credit transaction of selected suspense account transactions in GOExpress
            var dbLiq = (from hist in _context.GOExpressHist
                         join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                         join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                         join liqDtl in _context.LiquidationEntryDetails on hist.ExpenseEntryID equals liqDtl.ExpenseEntryModel.Expense_ID
                         where status.Contains(liqDtl.Liq_Status) &&
                                 ((hist.GOExpHist_Entry11Type == "C" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                                  (hist.GOExpHist_Entry12Type == "C" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                                  (hist.GOExpHist_Entry21Type == "C" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                                  (hist.GOExpHist_Entry22Type == "C" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                                  (hist.GOExpHist_Entry31Type == "C" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                                  (hist.GOExpHist_Entry32Type == "C" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                                  (hist.GOExpHist_Entry41Type == "C" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                                  (hist.GOExpHist_Entry42Type == "C" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                         select new
                         {
                             exp.Expense_ID,
                             exp.Expense_Type,
                             exp.Expense_Last_Updated,
                             exp.Expense_Date,
                             exp.Expense_Number,
                             exp.Expense_CheckNo,
                             exp.Expense_Creator_ID,
                             exp.Expense_Approver,
                             hist.ExpenseEntryID,
                             hist.ExpenseDetailID, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                             hist.GOExpHist_Id,
                             hist.GOExpHist_ValueDate,
                             hist.GOExpHist_ReferenceNo,
                             hist.GOExpHist_Branchno,
                             hist.GOExpHist_Section,
                             hist.GOExpHist_Remarks,
                             hist.GOExpHist_Entry11Type,
                             hist.GOExpHist_Entry11Ccy,
                             hist.GOExpHist_Entry11Amt,
                             hist.GOExpHist_Entry11Cust,
                             hist.GOExpHist_Entry11Actcde,
                             hist.GOExpHist_Entry11ActType,
                             hist.GOExpHist_Entry11ActNo,
                             hist.GOExpHist_Entry11ExchRate,
                             hist.GOExpHist_Entry11ExchCcy,
                             hist.GOExpHist_Entry11Fund,
                             hist.GOExpHist_Entry11AdvcPrnt,
                             hist.GOExpHist_Entry11Details,
                             hist.GOExpHist_Entry11Entity,
                             hist.GOExpHist_Entry11Division,
                             hist.GOExpHist_Entry11InterAmt,
                             hist.GOExpHist_Entry11InterRate,
                             hist.GOExpHist_Entry12Type,
                             hist.GOExpHist_Entry12Ccy,
                             hist.GOExpHist_Entry12Amt,
                             hist.GOExpHist_Entry12Cust,
                             hist.GOExpHist_Entry12Actcde,
                             hist.GOExpHist_Entry12ActType,
                             hist.GOExpHist_Entry12ActNo,
                             hist.GOExpHist_Entry12ExchRate,
                             hist.GOExpHist_Entry12ExchCcy,
                             hist.GOExpHist_Entry12Fund,
                             hist.GOExpHist_Entry12AdvcPrnt,
                             hist.GOExpHist_Entry12Details,
                             hist.GOExpHist_Entry12Entity,
                             hist.GOExpHist_Entry12Division,
                             hist.GOExpHist_Entry12InterAmt,
                             hist.GOExpHist_Entry12InterRate,
                             hist.GOExpHist_Entry21Type,
                             hist.GOExpHist_Entry21Ccy,
                             hist.GOExpHist_Entry21Amt,
                             hist.GOExpHist_Entry21Cust,
                             hist.GOExpHist_Entry21Actcde,
                             hist.GOExpHist_Entry21ActType,
                             hist.GOExpHist_Entry21ActNo,
                             hist.GOExpHist_Entry21ExchRate,
                             hist.GOExpHist_Entry21ExchCcy,
                             hist.GOExpHist_Entry21Fund,
                             hist.GOExpHist_Entry21AdvcPrnt,
                             hist.GOExpHist_Entry21Details,
                             hist.GOExpHist_Entry21Entity,
                             hist.GOExpHist_Entry21Division,
                             hist.GOExpHist_Entry21InterAmt,
                             hist.GOExpHist_Entry21InterRate,
                             hist.GOExpHist_Entry22Type,
                             hist.GOExpHist_Entry22Ccy,
                             hist.GOExpHist_Entry22Amt,
                             hist.GOExpHist_Entry22Cust,
                             hist.GOExpHist_Entry22Actcde,
                             hist.GOExpHist_Entry22ActType,
                             hist.GOExpHist_Entry22ActNo,
                             hist.GOExpHist_Entry22ExchRate,
                             hist.GOExpHist_Entry22ExchCcy,
                             hist.GOExpHist_Entry22Fund,
                             hist.GOExpHist_Entry22AdvcPrnt,
                             hist.GOExpHist_Entry22Details,
                             hist.GOExpHist_Entry22Entity,
                             hist.GOExpHist_Entry22Division,
                             hist.GOExpHist_Entry22InterAmt,
                             hist.GOExpHist_Entry22InterRate,
                             hist.GOExpHist_Entry31Type,
                             hist.GOExpHist_Entry31Ccy,
                             hist.GOExpHist_Entry31Amt,
                             hist.GOExpHist_Entry31Cust,
                             hist.GOExpHist_Entry31Actcde,
                             hist.GOExpHist_Entry31ActType,
                             hist.GOExpHist_Entry31ActNo,
                             hist.GOExpHist_Entry31ExchRate,
                             hist.GOExpHist_Entry31ExchCcy,
                             hist.GOExpHist_Entry31Fund,
                             hist.GOExpHist_Entry31AdvcPrnt,
                             hist.GOExpHist_Entry31Details,
                             hist.GOExpHist_Entry31Entity,
                             hist.GOExpHist_Entry31Division,
                             hist.GOExpHist_Entry31InterAmt,
                             hist.GOExpHist_Entry31InterRate,
                             hist.GOExpHist_Entry32Type,
                             hist.GOExpHist_Entry32Ccy,
                             hist.GOExpHist_Entry32Amt,
                             hist.GOExpHist_Entry32Cust,
                             hist.GOExpHist_Entry32Actcde,
                             hist.GOExpHist_Entry32ActType,
                             hist.GOExpHist_Entry32ActNo,
                             hist.GOExpHist_Entry32ExchRate,
                             hist.GOExpHist_Entry32ExchCcy,
                             hist.GOExpHist_Entry32Fund,
                             hist.GOExpHist_Entry32AdvcPrnt,
                             hist.GOExpHist_Entry32Details,
                             hist.GOExpHist_Entry32Entity,
                             hist.GOExpHist_Entry32Division,
                             hist.GOExpHist_Entry32InterAmt,
                             hist.GOExpHist_Entry32InterRate,
                             hist.GOExpHist_Entry41Type,
                             hist.GOExpHist_Entry41Ccy,
                             hist.GOExpHist_Entry41Amt,
                             hist.GOExpHist_Entry41Cust,
                             hist.GOExpHist_Entry41Actcde,
                             hist.GOExpHist_Entry41ActType,
                             hist.GOExpHist_Entry41ActNo,
                             hist.GOExpHist_Entry41ExchRate,
                             hist.GOExpHist_Entry41ExchCcy,
                             hist.GOExpHist_Entry41Fund,
                             hist.GOExpHist_Entry41AdvcPrnt,
                             hist.GOExpHist_Entry41Details,
                             hist.GOExpHist_Entry41Entity,
                             hist.GOExpHist_Entry41Division,
                             hist.GOExpHist_Entry41InterAmt,
                             hist.GOExpHist_Entry41InterRate,
                             hist.GOExpHist_Entry42Type,
                             hist.GOExpHist_Entry42Ccy,
                             hist.GOExpHist_Entry42Amt,
                             hist.GOExpHist_Entry42Cust,
                             hist.GOExpHist_Entry42Actcde,
                             hist.GOExpHist_Entry42ActType,
                             hist.GOExpHist_Entry42ActNo,
                             hist.GOExpHist_Entry42ExchRate,
                             hist.GOExpHist_Entry42ExchCcy,
                             hist.GOExpHist_Entry42Fund,
                             hist.GOExpHist_Entry42AdvcPrnt,
                             hist.GOExpHist_Entry42Details,
                             hist.GOExpHist_Entry42Entity,
                             hist.GOExpHist_Entry42Division,
                             hist.GOExpHist_Entry42InterAmt,
                             hist.GOExpHist_Entry42InterRate,
                             trans.TL_ID,
                             trans.TL_GoExpress_ID,
                             trans.TL_TransID,
                             trans.TL_Liquidation
                         }).ToList();

            //Get all reversed(CA) credit transaction of selected suspense account transactions in GOExpress
            var dbCARev = (from hist in _context.GOExpressHist
                           join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                           join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                           join rev in _context.ReversalEntry on hist.GOExpHist_Id equals rev.Reversal_GOExpressHistID
                           where exp.Expense_Type == GlobalSystemValues.TYPE_SS &&
                                (exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED ||
                                 exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR) &&
                                ((hist.GOExpHist_Entry11Type == "C" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                                 (hist.GOExpHist_Entry12Type == "C" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                                 (hist.GOExpHist_Entry21Type == "C" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                                 (hist.GOExpHist_Entry22Type == "C" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                                 (hist.GOExpHist_Entry31Type == "C" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                                 (hist.GOExpHist_Entry32Type == "C" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                                 (hist.GOExpHist_Entry41Type == "C" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                                 (hist.GOExpHist_Entry42Type == "C" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                           select new
                           {
                               exp.Expense_ID,
                               exp.Expense_Type,
                               exp.Expense_Last_Updated,
                               exp.Expense_Date,
                               exp.Expense_Number,
                               exp.Expense_CheckNo,
                               exp.Expense_Creator_ID,
                               exp.Expense_Approver,
                               hist.ExpenseEntryID,
                               hist.ExpenseDetailID, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                               hist.GOExpHist_Id,
                               hist.GOExpHist_ValueDate,
                               hist.GOExpHist_ReferenceNo,
                               hist.GOExpHist_Branchno,
                               hist.GOExpHist_Section,
                               hist.GOExpHist_Remarks,
                               hist.GOExpHist_Entry11Type,
                               hist.GOExpHist_Entry11Ccy,
                               hist.GOExpHist_Entry11Amt,
                               hist.GOExpHist_Entry11Cust,
                               hist.GOExpHist_Entry11Actcde,
                               hist.GOExpHist_Entry11ActType,
                               hist.GOExpHist_Entry11ActNo,
                               hist.GOExpHist_Entry11ExchRate,
                               hist.GOExpHist_Entry11ExchCcy,
                               hist.GOExpHist_Entry11Fund,
                               hist.GOExpHist_Entry11AdvcPrnt,
                               hist.GOExpHist_Entry11Details,
                               hist.GOExpHist_Entry11Entity,
                               hist.GOExpHist_Entry11Division,
                               hist.GOExpHist_Entry11InterAmt,
                               hist.GOExpHist_Entry11InterRate,
                               hist.GOExpHist_Entry12Type,
                               hist.GOExpHist_Entry12Ccy,
                               hist.GOExpHist_Entry12Amt,
                               hist.GOExpHist_Entry12Cust,
                               hist.GOExpHist_Entry12Actcde,
                               hist.GOExpHist_Entry12ActType,
                               hist.GOExpHist_Entry12ActNo,
                               hist.GOExpHist_Entry12ExchRate,
                               hist.GOExpHist_Entry12ExchCcy,
                               hist.GOExpHist_Entry12Fund,
                               hist.GOExpHist_Entry12AdvcPrnt,
                               hist.GOExpHist_Entry12Details,
                               hist.GOExpHist_Entry12Entity,
                               hist.GOExpHist_Entry12Division,
                               hist.GOExpHist_Entry12InterAmt,
                               hist.GOExpHist_Entry12InterRate,
                               hist.GOExpHist_Entry21Type,
                               hist.GOExpHist_Entry21Ccy,
                               hist.GOExpHist_Entry21Amt,
                               hist.GOExpHist_Entry21Cust,
                               hist.GOExpHist_Entry21Actcde,
                               hist.GOExpHist_Entry21ActType,
                               hist.GOExpHist_Entry21ActNo,
                               hist.GOExpHist_Entry21ExchRate,
                               hist.GOExpHist_Entry21ExchCcy,
                               hist.GOExpHist_Entry21Fund,
                               hist.GOExpHist_Entry21AdvcPrnt,
                               hist.GOExpHist_Entry21Details,
                               hist.GOExpHist_Entry21Entity,
                               hist.GOExpHist_Entry21Division,
                               hist.GOExpHist_Entry21InterAmt,
                               hist.GOExpHist_Entry21InterRate,
                               hist.GOExpHist_Entry22Type,
                               hist.GOExpHist_Entry22Ccy,
                               hist.GOExpHist_Entry22Amt,
                               hist.GOExpHist_Entry22Cust,
                               hist.GOExpHist_Entry22Actcde,
                               hist.GOExpHist_Entry22ActType,
                               hist.GOExpHist_Entry22ActNo,
                               hist.GOExpHist_Entry22ExchRate,
                               hist.GOExpHist_Entry22ExchCcy,
                               hist.GOExpHist_Entry22Fund,
                               hist.GOExpHist_Entry22AdvcPrnt,
                               hist.GOExpHist_Entry22Details,
                               hist.GOExpHist_Entry22Entity,
                               hist.GOExpHist_Entry22Division,
                               hist.GOExpHist_Entry22InterAmt,
                               hist.GOExpHist_Entry22InterRate,
                               hist.GOExpHist_Entry31Type,
                               hist.GOExpHist_Entry31Ccy,
                               hist.GOExpHist_Entry31Amt,
                               hist.GOExpHist_Entry31Cust,
                               hist.GOExpHist_Entry31Actcde,
                               hist.GOExpHist_Entry31ActType,
                               hist.GOExpHist_Entry31ActNo,
                               hist.GOExpHist_Entry31ExchRate,
                               hist.GOExpHist_Entry31ExchCcy,
                               hist.GOExpHist_Entry31Fund,
                               hist.GOExpHist_Entry31AdvcPrnt,
                               hist.GOExpHist_Entry31Details,
                               hist.GOExpHist_Entry31Entity,
                               hist.GOExpHist_Entry31Division,
                               hist.GOExpHist_Entry31InterAmt,
                               hist.GOExpHist_Entry31InterRate,
                               hist.GOExpHist_Entry32Type,
                               hist.GOExpHist_Entry32Ccy,
                               hist.GOExpHist_Entry32Amt,
                               hist.GOExpHist_Entry32Cust,
                               hist.GOExpHist_Entry32Actcde,
                               hist.GOExpHist_Entry32ActType,
                               hist.GOExpHist_Entry32ActNo,
                               hist.GOExpHist_Entry32ExchRate,
                               hist.GOExpHist_Entry32ExchCcy,
                               hist.GOExpHist_Entry32Fund,
                               hist.GOExpHist_Entry32AdvcPrnt,
                               hist.GOExpHist_Entry32Details,
                               hist.GOExpHist_Entry32Entity,
                               hist.GOExpHist_Entry32Division,
                               hist.GOExpHist_Entry32InterAmt,
                               hist.GOExpHist_Entry32InterRate,
                               hist.GOExpHist_Entry41Type,
                               hist.GOExpHist_Entry41Ccy,
                               hist.GOExpHist_Entry41Amt,
                               hist.GOExpHist_Entry41Cust,
                               hist.GOExpHist_Entry41Actcde,
                               hist.GOExpHist_Entry41ActType,
                               hist.GOExpHist_Entry41ActNo,
                               hist.GOExpHist_Entry41ExchRate,
                               hist.GOExpHist_Entry41ExchCcy,
                               hist.GOExpHist_Entry41Fund,
                               hist.GOExpHist_Entry41AdvcPrnt,
                               hist.GOExpHist_Entry41Details,
                               hist.GOExpHist_Entry41Entity,
                               hist.GOExpHist_Entry41Division,
                               hist.GOExpHist_Entry41InterAmt,
                               hist.GOExpHist_Entry41InterRate,
                               hist.GOExpHist_Entry42Type,
                               hist.GOExpHist_Entry42Ccy,
                               hist.GOExpHist_Entry42Amt,
                               hist.GOExpHist_Entry42Cust,
                               hist.GOExpHist_Entry42Actcde,
                               hist.GOExpHist_Entry42ActType,
                               hist.GOExpHist_Entry42ActNo,
                               hist.GOExpHist_Entry42ExchRate,
                               hist.GOExpHist_Entry42ExchCcy,
                               hist.GOExpHist_Entry42Fund,
                               hist.GOExpHist_Entry42AdvcPrnt,
                               hist.GOExpHist_Entry42Details,
                               hist.GOExpHist_Entry42Entity,
                               hist.GOExpHist_Entry42Division,
                               hist.GOExpHist_Entry42InterAmt,
                               hist.GOExpHist_Entry42InterRate,
                               trans.TL_ID,
                               trans.TL_GoExpress_ID,
                               trans.TL_TransID,
                               trans.TL_Liquidation
                           }).ToList();

            //Get all Non-cash credit transaction of selected suspense account transactions in GOExpress
            var dbCANC = (from hist in _context.GOExpressHist
                          join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                          join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                          where trans.TL_Liquidation == false &&
                                (exp.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING ||
                                 exp.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING ||
                                (exp.Expense_Status == GlobalSystemValues.STATUS_REVERSED &&
                                 getAllReversalEntry.Select(x => x.Reversal_GOExpressHistID).Contains(hist.GOExpHist_Id))) &&
                              ((hist.GOExpHist_Entry11Type == "C" && hist.GOExpHist_Entry11Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry11ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry11ActNo) ||
                               (hist.GOExpHist_Entry12Type == "C" && hist.GOExpHist_Entry12Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry12ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry12ActNo) ||
                               (hist.GOExpHist_Entry21Type == "C" && hist.GOExpHist_Entry21Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry21ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry21ActNo) ||
                               (hist.GOExpHist_Entry22Type == "C" && hist.GOExpHist_Entry22Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry22ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry22ActNo) ||
                               (hist.GOExpHist_Entry31Type == "C" && hist.GOExpHist_Entry31Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry31ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry31ActNo) ||
                               (hist.GOExpHist_Entry32Type == "C" && hist.GOExpHist_Entry32Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry32ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry32ActNo) ||
                               (hist.GOExpHist_Entry41Type == "C" && hist.GOExpHist_Entry41Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry41ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry41ActNo) ||
                               (hist.GOExpHist_Entry42Type == "C" && hist.GOExpHist_Entry42Actcde == selectedAccount.Account_Code && selectedAccount.Account_No.Replace("-", "") == hist.GOExpHist_Entry42ActType + hist.GOExpHist_Branchno + hist.GOExpHist_Entry42ActNo))
                          select new
                          {
                              exp.Expense_ID,
                              exp.Expense_Type,
                              exp.Expense_Last_Updated,
                              exp.Expense_Date,
                              exp.Expense_Number,
                              exp.Expense_CheckNo,
                              exp.Expense_Creator_ID,
                              exp.Expense_Approver,
                              hist.ExpenseEntryID,
                              hist.ExpenseDetailID, /*Non-NC: ExpDtlID. NC: NCDtlID*/
                              hist.GOExpHist_Id,
                              hist.GOExpHist_ValueDate,
                              hist.GOExpHist_ReferenceNo,
                              hist.GOExpHist_Branchno,
                              hist.GOExpHist_Section,
                              hist.GOExpHist_Remarks,
                              hist.GOExpHist_Entry11Type,
                              hist.GOExpHist_Entry11Ccy,
                              hist.GOExpHist_Entry11Amt,
                              hist.GOExpHist_Entry11Cust,
                              hist.GOExpHist_Entry11Actcde,
                              hist.GOExpHist_Entry11ActType,
                              hist.GOExpHist_Entry11ActNo,
                              hist.GOExpHist_Entry11ExchRate,
                              hist.GOExpHist_Entry11ExchCcy,
                              hist.GOExpHist_Entry11Fund,
                              hist.GOExpHist_Entry11AdvcPrnt,
                              hist.GOExpHist_Entry11Details,
                              hist.GOExpHist_Entry11Entity,
                              hist.GOExpHist_Entry11Division,
                              hist.GOExpHist_Entry11InterAmt,
                              hist.GOExpHist_Entry11InterRate,
                              hist.GOExpHist_Entry12Type,
                              hist.GOExpHist_Entry12Ccy,
                              hist.GOExpHist_Entry12Amt,
                              hist.GOExpHist_Entry12Cust,
                              hist.GOExpHist_Entry12Actcde,
                              hist.GOExpHist_Entry12ActType,
                              hist.GOExpHist_Entry12ActNo,
                              hist.GOExpHist_Entry12ExchRate,
                              hist.GOExpHist_Entry12ExchCcy,
                              hist.GOExpHist_Entry12Fund,
                              hist.GOExpHist_Entry12AdvcPrnt,
                              hist.GOExpHist_Entry12Details,
                              hist.GOExpHist_Entry12Entity,
                              hist.GOExpHist_Entry12Division,
                              hist.GOExpHist_Entry12InterAmt,
                              hist.GOExpHist_Entry12InterRate,
                              hist.GOExpHist_Entry21Type,
                              hist.GOExpHist_Entry21Ccy,
                              hist.GOExpHist_Entry21Amt,
                              hist.GOExpHist_Entry21Cust,
                              hist.GOExpHist_Entry21Actcde,
                              hist.GOExpHist_Entry21ActType,
                              hist.GOExpHist_Entry21ActNo,
                              hist.GOExpHist_Entry21ExchRate,
                              hist.GOExpHist_Entry21ExchCcy,
                              hist.GOExpHist_Entry21Fund,
                              hist.GOExpHist_Entry21AdvcPrnt,
                              hist.GOExpHist_Entry21Details,
                              hist.GOExpHist_Entry21Entity,
                              hist.GOExpHist_Entry21Division,
                              hist.GOExpHist_Entry21InterAmt,
                              hist.GOExpHist_Entry21InterRate,
                              hist.GOExpHist_Entry22Type,
                              hist.GOExpHist_Entry22Ccy,
                              hist.GOExpHist_Entry22Amt,
                              hist.GOExpHist_Entry22Cust,
                              hist.GOExpHist_Entry22Actcde,
                              hist.GOExpHist_Entry22ActType,
                              hist.GOExpHist_Entry22ActNo,
                              hist.GOExpHist_Entry22ExchRate,
                              hist.GOExpHist_Entry22ExchCcy,
                              hist.GOExpHist_Entry22Fund,
                              hist.GOExpHist_Entry22AdvcPrnt,
                              hist.GOExpHist_Entry22Details,
                              hist.GOExpHist_Entry22Entity,
                              hist.GOExpHist_Entry22Division,
                              hist.GOExpHist_Entry22InterAmt,
                              hist.GOExpHist_Entry22InterRate,
                              hist.GOExpHist_Entry31Type,
                              hist.GOExpHist_Entry31Ccy,
                              hist.GOExpHist_Entry31Amt,
                              hist.GOExpHist_Entry31Cust,
                              hist.GOExpHist_Entry31Actcde,
                              hist.GOExpHist_Entry31ActType,
                              hist.GOExpHist_Entry31ActNo,
                              hist.GOExpHist_Entry31ExchRate,
                              hist.GOExpHist_Entry31ExchCcy,
                              hist.GOExpHist_Entry31Fund,
                              hist.GOExpHist_Entry31AdvcPrnt,
                              hist.GOExpHist_Entry31Details,
                              hist.GOExpHist_Entry31Entity,
                              hist.GOExpHist_Entry31Division,
                              hist.GOExpHist_Entry31InterAmt,
                              hist.GOExpHist_Entry31InterRate,
                              hist.GOExpHist_Entry32Type,
                              hist.GOExpHist_Entry32Ccy,
                              hist.GOExpHist_Entry32Amt,
                              hist.GOExpHist_Entry32Cust,
                              hist.GOExpHist_Entry32Actcde,
                              hist.GOExpHist_Entry32ActType,
                              hist.GOExpHist_Entry32ActNo,
                              hist.GOExpHist_Entry32ExchRate,
                              hist.GOExpHist_Entry32ExchCcy,
                              hist.GOExpHist_Entry32Fund,
                              hist.GOExpHist_Entry32AdvcPrnt,
                              hist.GOExpHist_Entry32Details,
                              hist.GOExpHist_Entry32Entity,
                              hist.GOExpHist_Entry32Division,
                              hist.GOExpHist_Entry32InterAmt,
                              hist.GOExpHist_Entry32InterRate,
                              hist.GOExpHist_Entry41Type,
                              hist.GOExpHist_Entry41Ccy,
                              hist.GOExpHist_Entry41Amt,
                              hist.GOExpHist_Entry41Cust,
                              hist.GOExpHist_Entry41Actcde,
                              hist.GOExpHist_Entry41ActType,
                              hist.GOExpHist_Entry41ActNo,
                              hist.GOExpHist_Entry41ExchRate,
                              hist.GOExpHist_Entry41ExchCcy,
                              hist.GOExpHist_Entry41Fund,
                              hist.GOExpHist_Entry41AdvcPrnt,
                              hist.GOExpHist_Entry41Details,
                              hist.GOExpHist_Entry41Entity,
                              hist.GOExpHist_Entry41Division,
                              hist.GOExpHist_Entry41InterAmt,
                              hist.GOExpHist_Entry41InterRate,
                              hist.GOExpHist_Entry42Type,
                              hist.GOExpHist_Entry42Ccy,
                              hist.GOExpHist_Entry42Amt,
                              hist.GOExpHist_Entry42Cust,
                              hist.GOExpHist_Entry42Actcde,
                              hist.GOExpHist_Entry42ActType,
                              hist.GOExpHist_Entry42ActNo,
                              hist.GOExpHist_Entry42ExchRate,
                              hist.GOExpHist_Entry42ExchCcy,
                              hist.GOExpHist_Entry42Fund,
                              hist.GOExpHist_Entry42AdvcPrnt,
                              hist.GOExpHist_Entry42Details,
                              hist.GOExpHist_Entry42Entity,
                              hist.GOExpHist_Entry42Division,
                              hist.GOExpHist_Entry42InterAmt,
                              hist.GOExpHist_Entry42InterRate,
                              trans.TL_ID,
                              trans.TL_GoExpress_ID,
                              trans.TL_TransID,
                              trans.TL_Liquidation
                          }).ToList();

            //Process 1:
            //Check reversal entry of Cash Advance if exist.
            //Process 2:
            //Check Liquidation entry. Must be For Closing or For Printing status
            //Process 3:
            //Filter All GOExpress that credited to selected suspense account

            int cnt = 0;
            foreach (var i in dbDebit)
            {
                cnt++;
                bool flag = false;

                //Add debited suspense account transaction
                outstandResult.Add(AddToOutstandingViewList(i, selectedAccount));

                //Check if debited transaction's expense entry was reversed or not.
                var revCA = dbCARev.Where(x => x.ExpenseEntryID == i.ExpenseEntryID &&
                                               x.ExpenseDetailID == i.ExpenseDetailID).ToList();
                if (revCA.Count > 0)
                {
                    foreach (var credRev in revCA)
                    {
                        if (!usedGOExpHistID.Contains(credRev.GOExpHist_Id))
                        {
                            HomeReportAccountSummaryViewModel res = AddToOutstandingViewList(credRev, selectedAccount);
                            if (res != null)
                            {
                                var removeItem = outstandResult.Where(x => x.HistGOExpHist_Id == i.GOExpHist_Id).FirstOrDefault();
                                if (removeItem != null) outstandResult.Remove(removeItem);
                                flag = true;
                                usedGOExpHistID.Add(credRev.GOExpHist_Id);
                                break;
                            }
                        }
                    }
                }
                if (flag) continue;

                //Check if debited transaction's expense entry was liquidated or not.
                var liqCA = dbLiq.Where(x => x.ExpenseEntryID == i.ExpenseEntryID &&
                                               x.ExpenseDetailID == i.ExpenseDetailID).ToList();
                if (liqCA.Count > 0)
                {
                    RepOutstandingViewModel newLiq = new RepOutstandingViewModel();
                    foreach (var credLiq in liqCA)
                    {
                        if (!usedGOExpHistID.Contains(credLiq.GOExpHist_Id))
                        {
                            //populate new LIQ viewmodel
                            newLiq = PopulateNewLiqOutstanding(newLiq, credLiq);

                            if ((credLiq.GOExpHist_Entry11Type == "C") ||
                               (credLiq.GOExpHist_Entry12Type == "C") ||
                               (credLiq.GOExpHist_Entry21Type == "C") ||
                               (credLiq.GOExpHist_Entry22Type == "C") ||
                               (credLiq.GOExpHist_Entry31Type == "C") ||
                               (credLiq.GOExpHist_Entry32Type == "C") ||
                               (credLiq.GOExpHist_Entry41Type == "C") ||
                               (credLiq.GOExpHist_Entry42Type == "C"))
                            {
                                //add up values of similar entries
                                newLiq.GOExpHist_Entry11Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry11Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry11Amt ?? "0"));
                                newLiq.GOExpHist_Entry12Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry12Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry12Amt ?? "0"));
                                newLiq.GOExpHist_Entry21Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry21Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry21Amt ?? "0"));
                                newLiq.GOExpHist_Entry22Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry22Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry22Amt ?? "0"));
                                newLiq.GOExpHist_Entry31Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry31Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry31Amt ?? "0"));
                                newLiq.GOExpHist_Entry32Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry32Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry32Amt ?? "0"));
                                newLiq.GOExpHist_Entry41Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry41Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry41Amt ?? "0"));
                                newLiq.GOExpHist_Entry42Amt = "" + (decimal.Parse(credLiq.GOExpHist_Entry42Amt ?? "0") + decimal.Parse(newLiq.GOExpHist_Entry42Amt ?? "0"));
                            }
                        }
                    }
                    string debitAmount = (outstandResult.LastOrDefault() != null) ? outstandResult.LastOrDefault().Trans_Amount : "";

                    if ((newLiq.GOExpHist_Entry11Type == "C" && newLiq.GOExpHist_Entry11Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry12Type == "C" && newLiq.GOExpHist_Entry12Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry21Type == "C" && newLiq.GOExpHist_Entry21Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry22Type == "C" && newLiq.GOExpHist_Entry22Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry31Type == "C" && newLiq.GOExpHist_Entry31Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry32Type == "C" && newLiq.GOExpHist_Entry32Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry41Type == "C" && newLiq.GOExpHist_Entry41Amt == debitAmount) ||
                       (newLiq.GOExpHist_Entry42Type == "C" && newLiq.GOExpHist_Entry42Amt == debitAmount))
                    {
                        HomeReportAccountSummaryViewModel res = AddToOutstandingViewList(newLiq, selectedAccount);
                        if (res != null)
                        {
                            var removeItem = outstandResult.Where(x => x.HistGOExpHist_Id == i.GOExpHist_Id).FirstOrDefault();
                            if (removeItem != null) outstandResult.Remove(removeItem);
                            flag = true;
                            usedGOExpHistID.Add(newLiq.GOExpHist_Id);
                        }
                    }
                }
                if (flag) continue;

                //Check if CA Entry's liquidation status is in REVERSED or REVERSED DUE TO G-BASE ERROR.
                //If yes, check the non-cash transaction to find its liquidation entry.
                //If not, will not proceed to non-cash entry since it was not yet processed in liquidation screen.
                if (i.Expense_Type == GlobalSystemValues.TYPE_SS)
                {
                    var liqExist = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == i.Expense_ID).FirstOrDefault();
                    int liqstat = (liqExist != null) ? liqExist.Liq_Status : 0;

                    if (liqstat != GlobalSystemValues.STATUS_REVERSED &&
                        liqstat != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR) continue;
                }

                //Filter to value > debit transaction
                var ncCA = dbCANC.Where(x => ConvGbDateToDateTime(i.GOExpHist_ValueDate).Date <= ConvGbDateToDateTime(x.GOExpHist_ValueDate).Date).ToList();

                if (ncCA.Count > 0)
                {
                    string prefix = (i.Expense_Type == GlobalSystemValues.TYPE_SS || status.Contains(i.Expense_Status)) ? "S" : "";
                    RepOutstandingViewModel newNC = new RepOutstandingViewModel();

                    foreach (var credNC in ncCA)
                    {
                        if (!usedGOExpHistID.Contains(credNC.GOExpHist_Id))
                        {
                            //populate new LIQ viewmodel
                            newNC = PopulateNewLiqOutstanding(newNC, credNC);

                            if ((credNC.GOExpHist_Entry11Type == "C") ||
                               (credNC.GOExpHist_Entry12Type == "C") ||
                               (credNC.GOExpHist_Entry21Type == "C") ||
                               (credNC.GOExpHist_Entry22Type == "C") ||
                               (credNC.GOExpHist_Entry31Type == "C") ||
                               (credNC.GOExpHist_Entry32Type == "C") ||
                               (credNC.GOExpHist_Entry41Type == "C") ||
                               (credNC.GOExpHist_Entry42Type == "C"))
                            {
                                //add up values of similar entries
                                newNC.GOExpHist_Entry11Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry11Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry11Amt ?? "0"));
                                newNC.GOExpHist_Entry12Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry12Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry12Amt ?? "0"));
                                newNC.GOExpHist_Entry21Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry21Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry21Amt ?? "0"));
                                newNC.GOExpHist_Entry22Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry22Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry22Amt ?? "0"));
                                newNC.GOExpHist_Entry31Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry31Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry31Amt ?? "0"));
                                newNC.GOExpHist_Entry32Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry32Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry32Amt ?? "0"));
                                newNC.GOExpHist_Entry41Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry41Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry41Amt ?? "0"));
                                newNC.GOExpHist_Entry42Amt = "" + (decimal.Parse(credNC.GOExpHist_Entry42Amt ?? "0") + decimal.Parse(newNC.GOExpHist_Entry42Amt ?? "0"));
                            }
                        }
                    }

                    string debitAmount = (outstandResult.LastOrDefault() != null) ? outstandResult.LastOrDefault().Trans_Amount : "";

                    if (prefix + i.GOExpHist_Remarks.Trim() == newNC.GOExpHist_Remarks.Trim() &&
                        ((newNC.GOExpHist_Entry11Type == "C" && newNC.GOExpHist_Entry11Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry12Type == "C" && newNC.GOExpHist_Entry12Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry21Type == "C" && newNC.GOExpHist_Entry21Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry22Type == "C" && newNC.GOExpHist_Entry22Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry31Type == "C" && newNC.GOExpHist_Entry31Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry32Type == "C" && newNC.GOExpHist_Entry32Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry41Type == "C" && newNC.GOExpHist_Entry41Amt == debitAmount) ||
                         (newNC.GOExpHist_Entry42Type == "C" && newNC.GOExpHist_Entry42Amt == debitAmount)))
                    {
                        HomeReportESAMSViewModel res = AddToESAMViewList(cnt, newNC, selectedAccount, userList, GlobalSystemValues.TRANS_CREDIT);
                        if (res != null)
                        {
                            var removeItem = outstandResult.Where(x => x.HistGOExpHist_Id == i.GOExpHist_Id).FirstOrDefault();
                            if (removeItem != null) outstandResult.Remove(removeItem);
                            flag = true;
                            usedGOExpHistID.Add(newNC.GOExpHist_Id);
                            break;
                        }
                    }
                }
            }

            return outstandResult;
        }

        private HomeReportAccountSummaryViewModel AddToOutstandingViewList(dynamic record, DMAccountModel selectedAccount)
        {
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry11ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry11ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry11Type,
                    Trans_Currency = record.GOExpHist_Entry11Ccy,
                    Trans_Amount = record.GOExpHist_Entry11Amt,
                    Trans_Customer = record.GOExpHist_Entry11Cust,
                    Trans_Account_Code = record.GOExpHist_Entry11Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry11ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry11ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry11ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry11ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry11Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry11AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry11Details,
                    Trans_Entity = record.GOExpHist_Entry11Entity,
                    Trans_Division = record.GOExpHist_Entry11Division,
                    Trans_InterAmount = record.GOExpHist_Entry11InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry11InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry12ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry12ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry12Type,
                    Trans_Currency = record.GOExpHist_Entry12Ccy,
                    Trans_Amount = record.GOExpHist_Entry12Amt,
                    Trans_Customer = record.GOExpHist_Entry12Cust,
                    Trans_Account_Code = record.GOExpHist_Entry12Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry12ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry12ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry12ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry12ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry12Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry12AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry12Details,
                    Trans_Entity = record.GOExpHist_Entry12Entity,
                    Trans_Division = record.GOExpHist_Entry12Division,
                    Trans_InterAmount = record.GOExpHist_Entry12InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry12InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry21ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry21ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry21Type,
                    Trans_Currency = record.GOExpHist_Entry21Ccy,
                    Trans_Amount = record.GOExpHist_Entry21Amt,
                    Trans_Customer = record.GOExpHist_Entry21Cust,
                    Trans_Account_Code = record.GOExpHist_Entry21Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry21ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry21ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry21ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry21ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry21Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry21AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry21Details,
                    Trans_Entity = record.GOExpHist_Entry21Entity,
                    Trans_Division = record.GOExpHist_Entry21Division,
                    Trans_InterAmount = record.GOExpHist_Entry21InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry21InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry22ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry22ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry22Type,
                    Trans_Currency = record.GOExpHist_Entry22Ccy,
                    Trans_Amount = record.GOExpHist_Entry22Amt,
                    Trans_Customer = record.GOExpHist_Entry22Cust,
                    Trans_Account_Code = record.GOExpHist_Entry22Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry22ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry22ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry22ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry22ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry22Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry22AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry22Details,
                    Trans_Entity = record.GOExpHist_Entry22Entity,
                    Trans_Division = record.GOExpHist_Entry22Division,
                    Trans_InterAmount = record.GOExpHist_Entry22InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry22InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry31ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry31ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry31Type,
                    Trans_Currency = record.GOExpHist_Entry31Ccy,
                    Trans_Amount = record.GOExpHist_Entry31Amt,
                    Trans_Customer = record.GOExpHist_Entry31Cust,
                    Trans_Account_Code = record.GOExpHist_Entry31Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry31ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry31ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry31ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry31ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry31Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry31AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry31Details,
                    Trans_Entity = record.GOExpHist_Entry31Entity,
                    Trans_Division = record.GOExpHist_Entry31Division,
                    Trans_InterAmount = record.GOExpHist_Entry31InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry31InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry32ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry32ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry32Type,
                    Trans_Currency = record.GOExpHist_Entry32Ccy,
                    Trans_Amount = record.GOExpHist_Entry32Amt,
                    Trans_Customer = record.GOExpHist_Entry32Cust,
                    Trans_Account_Code = record.GOExpHist_Entry32Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry32ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry32ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry32ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry32ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry32Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry32AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry32Details,
                    Trans_Entity = record.GOExpHist_Entry32Entity,
                    Trans_Division = record.GOExpHist_Entry32Division,
                    Trans_InterAmount = record.GOExpHist_Entry32InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry32InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry41ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry41ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry41Type,
                    Trans_Currency = record.GOExpHist_Entry41Ccy,
                    Trans_Amount = record.GOExpHist_Entry41Amt,
                    Trans_Customer = record.GOExpHist_Entry41Cust,
                    Trans_Account_Code = record.GOExpHist_Entry41Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry41ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry41ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry41ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry41ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry41Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry41AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry41Details,
                    Trans_Entity = record.GOExpHist_Entry41Entity,
                    Trans_Division = record.GOExpHist_Entry41Division,
                    Trans_InterAmount = record.GOExpHist_Entry41InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry41InterRate
                };
            }
            if (selectedAccount.Account_No.Replace("-", "") == record.GOExpHist_Entry42ActType + record.GOExpHist_Branchno + record.GOExpHist_Entry42ActNo)
            {
                return new HomeReportAccountSummaryViewModel
                {
                    HistGOExpHist_Id = record.GOExpHist_Id,
                    Trans_Voucher_Number = getVoucherNo(record.Expense_Type, record.Expense_Last_Updated, record.Expense_Number, false),
                    Trans_Check_Number = record.Expense_CheckNo + "",
                    Trans_Value_Date = record.GOExpHist_ValueDate,
                    Trans_Reference_No = "",
                    Trans_Section = record.GOExpHist_Section,
                    Trans_Remarks = record.GOExpHist_Remarks,
                    Trans_DebitCredit = record.GOExpHist_Entry42Type,
                    Trans_Currency = record.GOExpHist_Entry42Ccy,
                    Trans_Amount = record.GOExpHist_Entry42Amt,
                    Trans_Customer = record.GOExpHist_Entry42Cust,
                    Trans_Account_Code = record.GOExpHist_Entry42Actcde,
                    Trans_Account_Number = record.GOExpHist_Entry42ActType + "-" + record.GOExpHist_Branchno + "-" + record.GOExpHist_Entry42ActNo,
                    Trans_Account_Name = selectedAccount.Account_Name,
                    Trans_Exchange_Rate = record.GOExpHist_Entry42ExchRate,
                    Trans_Contra_Currency = record.GOExpHist_Entry42ExchCcy,
                    Trans_Fund = record.GOExpHist_Entry42Fund,
                    Trans_Advice_Print = record.GOExpHist_Entry42AdvcPrnt,
                    Trans_Details = record.GOExpHist_Entry42Details,
                    Trans_Entity = record.GOExpHist_Entry42Entity,
                    Trans_Division = record.GOExpHist_Entry42Division,
                    Trans_InterAmount = record.GOExpHist_Entry42InterAmt,
                    Trans_InterRate = record.GOExpHist_Entry42InterRate
                };
            }

            return null;
        }

        //Get account name for Non-cash related transaction for Transaction List report.
        public string GetAccountNameForNonCash(List<DMAccountModel> accList, string accType, string accNo, string accCode, List<ExpenseEntryNCDtlViewModel> ncDtlList, int dtlID)
        {
            if (String.IsNullOrEmpty(accType) || String.IsNullOrEmpty(accNo) || String.IsNullOrEmpty(accCode))
                return "";

            var ncdata = ncDtlList.Where(x => x.ExpNCDtl_ID == dtlID).FirstOrDefault();

            foreach (var i in ncdata.ExpenseEntryNCDtlAccs)
            {
                var acc = accList.Where(x => x.Account_ID == i.ExpNCDtlAcc_Acc_ID).FirstOrDefault();

                if (acc.Account_No.Contains(accType) && acc.Account_No.Contains(accNo) && acc.Account_Code == accCode)
                {
                    return acc.Account_Name;
                }
            }

            return "";
        }
        //Get Non-cash entry details and non-cash entry details accounts list. For report purpose.
        public List<ExpenseEntryNCDtlViewModel> GetEntryDetailAccountListForNonCash()
        {
            List<ExpenseEntryNCDtlViewModel> list = new List<ExpenseEntryNCDtlViewModel>();

            var ncdtl = (from g
                        in _context.ExpenseEntryNonCashDetails
                         select new
                         {
                             g,
                             ExpenseEntryNCDtlAccs = from a
                             in _context.ExpenseEntryNonCashDetailAccounts
                                                     where a.ExpenseEntryNCDtlModel.ExpNCDtl_ID == g.ExpNCDtl_ID
                                                     select new
                                                     {
                                                         a
                                                     }
                         }).ToList();

            List<ExpenseEntryNCDtlViewModel> ncDtls = new List<ExpenseEntryNCDtlViewModel>();

            foreach (var ncDtl in ncdtl)
            {
                List<ExpenseEntryNCDtlAccViewModel> ncDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>();

                foreach (var ncDtlAcc in ncDtl.ExpenseEntryNCDtlAccs)
                {
                    ncDtlAccs.Add(new ExpenseEntryNCDtlAccViewModel()
                    {
                        ExpNCDtlAcc_Acc_ID = ncDtlAcc.a.ExpNCDtlAcc_Acc_ID
                    });
                }

                ncDtls.Add(new ExpenseEntryNCDtlViewModel()
                {
                    ExpNCDtl_ID = ncDtl.g.ExpNCDtl_ID,
                    ExpenseEntryNCDtlAccs = ncDtlAccs
                });
            }

            return ncDtls;
        }
        //Get account name for CV, PC, DDV, SS for Transaction List report
        public string GetAccountNameForCADDVPCSS(List<DMAccountModel> accList, string accType, string accNo, string accCode, string currAbbrev, int acc1, int? acc2, int? acc3, int expType, List<EntryDDVViewModel> entryDtlListDDV, int expID, int dtlID)
        {
            if (String.IsNullOrEmpty(accType) || String.IsNullOrEmpty(accNo))
                return "";

            DMAccountModel accno1 = (acc1 != 0) ? accList.Where(x => x.Account_ID == acc1).FirstOrDefault() : null;
            DMAccountModel accno2 = (acc2 != 0) ? accList.Where(x => x.Account_ID == acc2).FirstOrDefault() : null;
            DMAccountModel accno3 = (acc3 != 0) ? accList.Where(x => x.Account_ID == acc3).FirstOrDefault() : null;

            if (expType == GlobalSystemValues.TYPE_CV || expType == GlobalSystemValues.TYPE_PC || expType == GlobalSystemValues.TYPE_SS)
            {
                if (accno1 != null && accno1.Account_No.Contains(accType) && accno1.Account_No.Contains(accNo) && accno1.Account_Code == accCode && getCurrencyByMasterID(accno1.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                {
                    return accno1.Account_Name;
                }

                if (accno2 != null && accno2.Account_No.Contains(accType) && accno2.Account_No.Contains(accNo) && accno2.Account_Code == accCode && getCurrencyByMasterID(accno2.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                {
                    return accno2.Account_Name;
                }

                if (accno3 != null && accno3.Account_No.Contains(accType) && accno3.Account_No.Contains(accNo) && accno3.Account_Code == accCode && getCurrencyByMasterID(accno3.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                {
                    return accno3.Account_Name;
                }
            }

            if (expType == GlobalSystemValues.TYPE_DDV)
            {
                var ddvData = entryDtlListDDV.Where(x => x.dtlID == dtlID).FirstOrDefault();

                if (ddvData.inter_entity)
                {
                    foreach (var i in ddvData.interDetails.interPartList)
                    {
                        foreach (var j in i.ExpenseEntryInterEntityAccs)
                        {
                            DMAccountModel interAcc = accList.Where(x => x.Account_ID == j.Inter_Acc_ID).FirstOrDefault();

                            if (interAcc.Account_No.Contains(accType) && interAcc.Account_No.Contains(accNo) && interAcc.Account_Code == accCode && getCurrencyByMasterID(interAcc.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return interAcc.Account_Name;
                            }
                        }
                    }
                }
                else
                {
                    if (accno1 != null && accno1.Account_No.Contains(accType) && accno1.Account_No.Contains(accNo) && accno1.Account_Code == accCode && getCurrencyByMasterID(accno1.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                    {
                        return accno1.Account_Name;
                    }

                    if (accno2 != null && accno2.Account_No.Contains(accType) && accno2.Account_No.Contains(accNo) && accno2.Account_Code == accCode && getCurrencyByMasterID(accno2.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                    {
                        return accno2.Account_Name;
                    }

                    if (accno3 != null && accno3.Account_No.Contains(accType) && accno3.Account_No.Contains(accNo) && accno3.Account_Code == accCode && getCurrencyByMasterID(accno3.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                    {
                        return accno3.Account_Name;
                    }
                }
            }

            //For FBT purpose.
            var fbt_Account = accList.Where(x => x.Account_MasterID == int.Parse(xelemAcc.Element("HOUSE_RENT").Value)).FirstOrDefault();
            if (fbt_Account != null && fbt_Account.Account_No.Contains(accType) && fbt_Account.Account_No.Contains(accNo) && fbt_Account.Account_Code == accCode)
            {
                return fbt_Account.Account_Name;
            }
            fbt_Account = accList.Where(x => x.Account_MasterID == int.Parse(xelemAcc.Element("D_FBT_RENT").Value)).FirstOrDefault();
            if (fbt_Account != null && fbt_Account.Account_No.Contains(accType) && fbt_Account.Account_No.Contains(accNo) && fbt_Account.Account_Code == accCode)
            {
                return fbt_Account.Account_Name;
            }
            fbt_Account = accList.Where(x => x.Account_MasterID == int.Parse(xelemAcc.Element("D_FBT_EXPAT").Value)).FirstOrDefault();
            if (fbt_Account != null && fbt_Account.Account_No.Contains(accType) && fbt_Account.Account_No.Contains(accNo) && fbt_Account.Account_Code == accCode)
            {
                return fbt_Account.Account_Name;
            }
            fbt_Account = accList.Where(x => x.Account_MasterID == int.Parse(xelemAcc.Element("D_FBT_LOCAL").Value)).FirstOrDefault();
            if (fbt_Account != null && fbt_Account.Account_No.Contains(accType) && fbt_Account.Account_No.Contains(accNo) && fbt_Account.Account_Code == accCode)
            {
                return fbt_Account.Account_Name;
            }
            fbt_Account = accList.Where(x => x.Account_MasterID == int.Parse(xelemAcc.Element("C_FBT").Value)).FirstOrDefault();
            if (fbt_Account != null && fbt_Account.Account_No.Contains(accType) && fbt_Account.Account_No.Contains(accNo) && fbt_Account.Account_Code == accCode)
            {
                return fbt_Account.Account_Name;
            }

            //For Liquidation purpose.
            if (expType == GlobalSystemValues.TYPE_SS)
            {
                var liqDtl = _context.LiquidationInterEntity.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == dtlID);
                if (liqDtl.Count() > 0)
                {
                    foreach (var i in liqDtl)
                    {
                        if (i.Liq_AccountID_1_1 != 0)
                        {
                            var liq = accList.Where(x => x.Account_ID == i.Liq_AccountID_1_1).FirstOrDefault();
                            if (liq.Account_No.Contains(accType) && liq.Account_No.Contains(accNo) && liq.Account_Code == accCode && getCurrencyByMasterID(liq.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return liq.Account_Name;
                            }
                        }

                        if (i.Liq_AccountID_1_2 != 0)
                        {
                            var liq = accList.Where(x => x.Account_ID == i.Liq_AccountID_1_2).FirstOrDefault();
                            if (liq.Account_No.Contains(accType) && liq.Account_No.Contains(accNo) && liq.Account_Code == accCode && getCurrencyByMasterID(liq.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return liq.Account_Name;
                            }
                        }

                        if (i.Liq_AccountID_2_1 != 0)
                        {
                            var liq = accList.Where(x => x.Account_ID == i.Liq_AccountID_2_1).FirstOrDefault();
                            if (liq.Account_No.Contains(accType) && liq.Account_No.Contains(accNo) && liq.Account_Code == accCode && getCurrencyByMasterID(liq.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return liq.Account_Name;
                            }
                        }

                        if (i.Liq_AccountID_2_2 != 0)
                        {
                            var liq = accList.Where(x => x.Account_ID == i.Liq_AccountID_2_2).FirstOrDefault();
                            if (liq.Account_No.Contains(accType) && liq.Account_No.Contains(accNo) && liq.Account_Code == accCode && getCurrencyByMasterID(liq.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return liq.Account_Name;
                            }
                        }

                        if (i.Liq_AccountID_3_1 != 0)
                        {
                            var liq = accList.Where(x => x.Account_ID == i.Liq_AccountID_3_1).FirstOrDefault();
                            if (liq.Account_No.Contains(accType) && liq.Account_No.Contains(accNo) && liq.Account_Code == accCode && getCurrencyByMasterID(liq.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return liq.Account_Name;
                            }
                        }

                        if (i.Liq_AccountID_3_2 != 0)
                        {
                            var liq = accList.Where(x => x.Account_ID == i.Liq_AccountID_3_2).FirstOrDefault();
                            if (liq.Account_No.Contains(accType) && liq.Account_No.Contains(accNo) && liq.Account_Code == accCode && getCurrencyByMasterID(liq.Account_Currency_MasterID).Curr_CCY_ABBR == currAbbrev)
                            {
                                return liq.Account_Name;
                            }
                        }
                    }
                }
            }

            return "";
        }
        //Get DDV entry details and DDV inter entity list. For report purpose.
        public List<EntryDDVViewModel> GetEntryDetailsListForDDV()
        {
            var EntryDetails = (from d
                  in _context.ExpenseEntryDetails
                                select new
                                {
                                    d,
                                    ExpenseEntryGbaseDtls = from g
                                                            in _context.ExpenseEntryGbaseDtls
                                                            where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                            select g,
                                    ExpenseEntryInterEntity = from a
                                                                in _context.ExpenseEntryInterEntity
                                                              where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                              select new
                                                              {
                                                                  a,
                                                                  ExpenseEntryInterEntityParticular = from p
                                                                                                      in _context.ExpenseEntryInterEntityParticular
                                                                                                      where p.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == a.ExpDtl_DDVInter_ID
                                                                                                      select new
                                                                                                      {
                                                                                                          p,
                                                                                                          ExpenseEntryEntityAccounts = from acc
                                                                                                                                       in _context.ExpenseEntryInterEntityAccs
                                                                                                                                       where acc.ExpenseEntryInterEntityParticular.InterPart_ID == p.InterPart_ID
                                                                                                                                       select acc
                                                                                                      }
                                                              }
                                }).ToList();

            List<EntryDDVViewModel> ddvList = new List<EntryDDVViewModel>();

            foreach (var dtl in EntryDetails)
            {
                DDVInterEntityViewModel interDetail = new DDVInterEntityViewModel();
                ExpenseEntryInterEntityAccsViewModel interDetailAccs = new ExpenseEntryInterEntityAccsViewModel();
                foreach (var inter in dtl.ExpenseEntryInterEntity)
                {
                    interDetail = new DDVInterEntityViewModel
                    {
                        interPartList = new List<ExpenseEntryInterEntityParticularViewModel>()
                    };

                    if (interDetail.Inter_Currency1_ID > 0)
                    {
                        interDetail.Inter_Currency1_ABBR = _context.DMCurrency.Where(x => x.Curr_ID == inter.a.ExpDtl_DDVInter_Curr1_ID &&
                                                       x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                    }
                    if (interDetail.Inter_Currency2_ID > 0)
                    {
                        interDetail.Inter_Currency2_ABBR = _context.DMCurrency.Where(x => x.Curr_ID == inter.a.ExpDtl_DDVInter_Curr2_ID &&
                                                    x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                    }

                    foreach (var interPart in inter.ExpenseEntryInterEntityParticular)
                    {
                        var acc = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account).FirstOrDefault();
                        ExpenseEntryInterEntityParticularViewModel interParticular = new ExpenseEntryInterEntityParticularViewModel
                        {
                            InterPart_ID = interPart.p.InterPart_ID,
                            ExpenseEntryInterEntityAccs = new List<ExpenseEntryInterEntityAccsViewModel>()
                        };
                        foreach (var interAcc in interPart.ExpenseEntryEntityAccounts)
                        {
                            ExpenseEntryInterEntityAccsViewModel interDetailAcc = new ExpenseEntryInterEntityAccsViewModel()
                            {
                                Inter_Acc_ID = interAcc.InterAcc_Acc_ID,
                                Inter_Type_ID = interAcc.InterAcc_Type_ID
                            };
                            interParticular.ExpenseEntryInterEntityAccs.Add(interDetailAcc);
                        }
                        interDetail.interPartList.Add(interParticular);
                    }
                }

                EntryDDVViewModel ddvDtl = new EntryDDVViewModel()
                {
                    dtlID = dtl.d.ExpDtl_ID,
                    account = dtl.d.ExpDtl_Account,
                    inter_entity = dtl.d.ExpDtl_Inter_Entity,
                    fbt = dtl.d.ExpDtl_Fbt,
                    creditAccount1 = dtl.d.ExpDtl_CreditAccount1,
                    creditAccount2 = dtl.d.ExpDtl_CreditAccount2,
                    interDetails = interDetail
                };
                ddvList.Add(ddvDtl);
            }

            return ddvList;
        }
        //Get Account number based on its acc no, acc type and acc code.
        public string GetAccountNoByAccNoAccTypeAccCde(List<DMAccountModel> accList, string accType, string accNo, string accCode)
        {

            if (!String.IsNullOrEmpty(accType) && !String.IsNullOrEmpty(accNo) && !String.IsNullOrEmpty(accCode))
            {
                return accList.Where(x => x.Account_No.Contains(accType) && x.Account_No.Contains(accNo) && x.Account_Code == accCode).FirstOrDefault().Account_No;
            }
            else
            {
                return "";
            }
        }
        public DateTime GetSelectedYearMonthOfTerm(int month, int year)
        {
            int[] firstTermMonths = { 4, 5, 6, 7, 8, 9 };
            int[] secodnTermNextYearMonths = { 1, 2, 3 };
            DateTime startOfTermDate;

            if (firstTermMonths.Contains(month))
            {
                startOfTermDate = DateTime.ParseExact(year + "-04-01", "yyyy-M-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                if (secodnTermNextYearMonths.Contains(month))
                {
                    startOfTermDate = DateTime.ParseExact((year - 1) + "-10-01", "yyyy-M-dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    startOfTermDate = DateTime.ParseExact(year + "-10-01", "yyyy-M-dd", CultureInfo.InvariantCulture);

                }
            }
            return startOfTermDate;
        }
        //month now, year now, true == start, false == end 
        public DateTime GetStartOfFiscal(int month, int year, bool opt)
        {
            int[] firstTermMonths = { 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            int[] secodnTermNextYearMonths = { 1, 2, 3 };

            if (opt == true)
            {
                if (firstTermMonths.Contains(month))
                {
                    return DateTime.ParseExact(year + "-04-01", "yyyy-M-dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    return DateTime.ParseExact((year - 1) + "-04-01", "yyyy-M-dd", CultureInfo.InvariantCulture);
                }
            }
            else
            {
                if (firstTermMonths.Contains(month))
                {
                    return DateTime.ParseExact((year + 1) + "-03-31", "yyyy-M-dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    return DateTime.ParseExact((year) + "-03-31", "yyyy-M-dd", CultureInfo.InvariantCulture);
                }
            }
        }
        public List<float> PopulateTaxRaxListIncludeHist()
        {
            var taxRate = _context.DMTR.OrderBy(x => x.TR_Tax_Rate).Select(x => x.TR_Tax_Rate).ToList().Distinct();
            List<float> taxRateList = new List<float>();
            foreach (var i in taxRate)
            {
                taxRateList.Add(i);
            }

            return taxRateList;
        }

        public DateTime ConvGbDateToDateTime(string date)
        {
            string month = date.Substring(0, 2);
            string day = date.Substring(2, 2);
            string year = (int.Parse(date.Substring(4, 2)) + 2000).ToString();

            return DateTime.ParseExact(month + "-" + day + "-" + year, "M-dd-yyyy", CultureInfo.InvariantCulture);
        }
        public List<VoucherNoOptions> PopulateVoucherNo()
        {
            var vn = _context.ExpenseEntry
                .Where(x => x.Expense_Payee_Type == GlobalSystemValues.PAYEETYPE_REGEMP
                        && x.Expense_Status != GlobalSystemValues.STATUS_PENDING)
                .ToList().Distinct();
            List<VoucherNoOptions> vnList = new List<VoucherNoOptions>();
            foreach (var x in vn)
            {
                vnList.Add(new VoucherNoOptions
                {
                    vchr_ID = x.Expense_ID,
                    vchr_No = GlobalSystemValues.getApplicationCode(x.Expense_Type) + "-" + x.Expense_Date.Year + "-" + x.Expense_Number.ToString().PadLeft(5, '0'),
                    vchr_EmployeeName = getVendorName(x.Expense_Payee, x.Expense_Payee_Type)
                });
            }
            return vnList;
        }
        public List<VoucherNoOptions> PopulateVoucherNoDDV()
        {
            var vn = _context.ExpenseEntry
                .Where(x => x.Expense_Payee_Type == GlobalSystemValues.PAYEETYPE_REGEMP
                        && x.Expense_Type == GlobalSystemValues.TYPE_DDV
                        && (x.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING
                        || x.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING
                        || x.Expense_Status == GlobalSystemValues.STATUS_POSTED)
                        && x.Expense_Last_Updated.Date == DateTime.Now.Date)
                .ToList().Distinct();
            List<VoucherNoOptions> vnList = new List<VoucherNoOptions>();
            foreach (var x in vn)
            {
                vnList.Add(new VoucherNoOptions
                {
                    vchr_ID = x.Expense_ID,
                    vchr_No = GlobalSystemValues.getApplicationCode(x.Expense_Type) + "-" + GetSelectedYearMonthOfTerm(x.Expense_Date.Month, x.Expense_Date.Year).Year + "-" + x.Expense_Number.ToString().PadLeft(5, '0'),
                    vchr_EmployeeName = getVendorName(x.Expense_Payee, x.Expense_Payee_Type),
                    vchr_Status = getStatus(x.Expense_Status)
                });
            }
            return vnList;
        }
        public List<VoucherNoOptions> PopulateVoucherNoCV()
        {
            var vn = (from recs in (from ent in _context.ExpenseEntry
                                    from dtls in _context.ExpenseEntryDetails
                                    where ent.Expense_ID == dtls.ExpenseEntryModel.Expense_ID
                                    select new
                                    {
                                        ent,
                                        dtls
                                    })
                      from amort in _context.ExpenseEntryAmortizations
                      where recs.dtls.ExpDtl_ID == amort.ExpenseEntryDetailModel.ExpDtl_ID
                      select new
                      {
                          recs
                      }).ToList().Distinct();
            List<VoucherNoOptions> vnList = new List<VoucherNoOptions>();
            foreach (var x in vn)
            {
                vnList.Add(new VoucherNoOptions
                {
                    vchr_ID = x.recs.ent.Expense_ID,
                    vchr_No = GlobalSystemValues.getApplicationCode(x.recs.ent.Expense_Type) + "-" + x.recs.ent.Expense_Date.Year + "-" + x.recs.ent.Expense_Number.ToString().PadLeft(5, '0'),
                    vchr_EmployeeName = getVendorName(x.recs.ent.Expense_Payee, x.recs.ent.Expense_Payee_Type),
                    vchr_Status = getStatus(x.recs.ent.Expense_Status)
                });
            }
            return vnList;
        }
        public List<VoucherNoOptions> PopulateVoucherNo(List<int> ids)
        {
            var vn = _context.ExpenseEntry
                .Where(x => x.Expense_Payee_Type == GlobalSystemValues.PAYEETYPE_REGEMP
                        && x.Expense_Status != GlobalSystemValues.STATUS_PENDING && ids.Contains(x.Expense_ID))
                .ToList().Distinct();
            List<VoucherNoOptions> vnList = new List<VoucherNoOptions>();
            foreach (var x in vn)
            {
                vnList.Add(new VoucherNoOptions
                {
                    vchr_ID = x.Expense_ID,
                    vchr_No = GetSelectedYearMonthOfTerm(x.Expense_Date.Month, x.Expense_Date.Year).Year.ToString().Substring(2, 2) + "-" + x.Expense_Number.ToString().PadLeft(5, '0'),
                    vchr_EmployeeName = getVendorName(x.Expense_Payee, x.Expense_Payee_Type)
                });
            }
            return vnList;
        }
        public IEnumerable<DMBCSViewModel> PopulateSignatoryList()
        {
            return _context.DMBCS.Where(x => x.BCS_isActive == true && x.BCS_isDeleted == false).Join(_context.User, b => b.BCS_User_ID,
                e => e.User_ID, (b, e) => new DMBCSViewModel
                { BCS_ID = b.BCS_ID, BCS_Name = e.User_LName + ", " + e.User_FName }).OrderBy(x => x.BCS_Name).ToList();
        }
        public RepESAMSViewModel PopulateNewLiq(RepESAMSViewModel newLiq, dynamic credLiq)
        {

            //Assign values to newLiq
            newLiq.GOExpHist_Id = credLiq.GOExpHist_Id;
            newLiq.Expense_Creator_ID = credLiq.Expense_Creator_ID;
            newLiq.Expense_Approver = credLiq.Expense_Approver;
            newLiq.GOExpHist_Branchno = credLiq.GOExpHist_Branchno;
            newLiq.GOExpHist_Remarks = credLiq.GOExpHist_Remarks;
            newLiq.GOExpHist_ValueDate = credLiq.GOExpHist_ValueDate;
            //entry 1-1
            newLiq.GOExpHist_Entry11ActType = credLiq.GOExpHist_Entry11ActType;
            newLiq.GOExpHist_Entry11ActNo = credLiq.GOExpHist_Entry11ActNo;
            newLiq.GOExpHist_Entry11Type = credLiq.GOExpHist_Entry11Type;
            //entry 1-2
            newLiq.GOExpHist_Entry12ActType = credLiq.GOExpHist_Entry12ActType;
            newLiq.GOExpHist_Entry12ActNo = credLiq.GOExpHist_Entry12ActNo;
            newLiq.GOExpHist_Entry12Type = credLiq.GOExpHist_Entry12Type;
            //entry 2-1
            newLiq.GOExpHist_Entry21ActType = credLiq.GOExpHist_Entry21ActType;
            newLiq.GOExpHist_Entry21ActNo = credLiq.GOExpHist_Entry21ActNo;
            newLiq.GOExpHist_Entry21Type = credLiq.GOExpHist_Entry21Type;
            //entry 2-2
            newLiq.GOExpHist_Entry22ActType = credLiq.GOExpHist_Entry22ActType;
            newLiq.GOExpHist_Entry22ActNo = credLiq.GOExpHist_Entry22ActNo;
            newLiq.GOExpHist_Entry22Type = credLiq.GOExpHist_Entry22Type;
            //entry 3-1
            newLiq.GOExpHist_Entry31ActType = credLiq.GOExpHist_Entry31ActType;
            newLiq.GOExpHist_Entry31ActNo = credLiq.GOExpHist_Entry31ActNo;
            newLiq.GOExpHist_Entry31Type = credLiq.GOExpHist_Entry31Type;
            //entry 3-2
            newLiq.GOExpHist_Entry32ActType = credLiq.GOExpHist_Entry32ActType;
            newLiq.GOExpHist_Entry32ActNo = credLiq.GOExpHist_Entry32ActNo;
            newLiq.GOExpHist_Entry32Type = credLiq.GOExpHist_Entry32Type;
            //entry 4-1
            newLiq.GOExpHist_Entry41ActType = credLiq.GOExpHist_Entry41ActType;
            newLiq.GOExpHist_Entry41ActNo = credLiq.GOExpHist_Entry41ActNo;
            newLiq.GOExpHist_Entry41Type = credLiq.GOExpHist_Entry41Type;
            //entry 4-2
            newLiq.GOExpHist_Entry42ActType = credLiq.GOExpHist_Entry42ActType;
            newLiq.GOExpHist_Entry42ActNo = credLiq.GOExpHist_Entry42ActNo;
            newLiq.GOExpHist_Entry42Type = credLiq.GOExpHist_Entry42Type;

            return newLiq;
        }
        public RepOutstandingViewModel PopulateNewLiqOutstanding(RepOutstandingViewModel newLiq, dynamic credLiq)
        {
            //Assign values to newLiq
            newLiq.GOExpHist_Id = credLiq.GOExpHist_Id;
            //newLiq.Expense_Creator_ID = credLiq.Expense_Creator_ID;
            //newLiq.Expense_Approver = credLiq.Expense_Approver;
            newLiq.GOExpHist_Branchno = credLiq.GOExpHist_Branchno;
            newLiq.GOExpHist_Remarks = credLiq.GOExpHist_Remarks;
            newLiq.GOExpHist_ValueDate = credLiq.GOExpHist_ValueDate;
            newLiq.Expense_Type = credLiq.Expense_Type;
            newLiq.Expense_Last_Updated = credLiq.Expense_Last_Updated;
            newLiq.Expense_Number = credLiq.Expense_Number;
            newLiq.Expense_CheckNo = credLiq.Expense_CheckNo ?? 0;
            newLiq.GOExpHist_Section = credLiq.GOExpHist_Section;

            //entry 1-1
            newLiq.GOExpHist_Entry11Type = credLiq.GOExpHist_Entry11Type;
            newLiq.GOExpHist_Entry11Ccy = credLiq.GOExpHist_Entry11Ccy;
            newLiq.GOExpHist_Entry11Cust = credLiq.GOExpHist_Entry11Cust;
            newLiq.GOExpHist_Entry11Actcde = credLiq.GOExpHist_Entry11Actcde;
            newLiq.GOExpHist_Entry11ActType = credLiq.GOExpHist_Entry11ActType;
            newLiq.GOExpHist_Entry11ActNo = credLiq.GOExpHist_Entry11ActNo;
            newLiq.GOExpHist_Entry11ExchRate = credLiq.GOExpHist_Entry11ExchRate;
            newLiq.GOExpHist_Entry11ExchCcy = credLiq.GOExpHist_Entry11ExchCcy;
            newLiq.GOExpHist_Entry11Fund = credLiq.GOExpHist_Entry11Fund;
            newLiq.GOExpHist_Entry11AdvcPrnt = credLiq.GOExpHist_Entry11AdvcPrnt;
            newLiq.GOExpHist_Entry11Details = credLiq.GOExpHist_Entry11Details;
            newLiq.GOExpHist_Entry11Entity = credLiq.GOExpHist_Entry11Entity;
            newLiq.GOExpHist_Entry11Division = credLiq.GOExpHist_Entry11Division;
            newLiq.GOExpHist_Entry11InterAmt = credLiq.GOExpHist_Entry11InterAmt;
            newLiq.GOExpHist_Entry11InterRate = credLiq.GOExpHist_Entry11InterRate;
            //entry 1-2
            newLiq.GOExpHist_Entry12Type = credLiq.GOExpHist_Entry12Type;
            newLiq.GOExpHist_Entry12Ccy = credLiq.GOExpHist_Entry12Ccy;
            newLiq.GOExpHist_Entry12Cust = credLiq.GOExpHist_Entry12Cust;
            newLiq.GOExpHist_Entry12Actcde = credLiq.GOExpHist_Entry12Actcde;
            newLiq.GOExpHist_Entry12ActType = credLiq.GOExpHist_Entry12ActType;
            newLiq.GOExpHist_Entry12ActNo = credLiq.GOExpHist_Entry12ActNo;
            newLiq.GOExpHist_Entry12ExchRate = credLiq.GOExpHist_Entry12ExchRate;
            newLiq.GOExpHist_Entry12ExchCcy = credLiq.GOExpHist_Entry12ExchCcy;
            newLiq.GOExpHist_Entry12Fund = credLiq.GOExpHist_Entry12Fund;
            newLiq.GOExpHist_Entry12AdvcPrnt = credLiq.GOExpHist_Entry12AdvcPrnt;
            newLiq.GOExpHist_Entry12Details = credLiq.GOExpHist_Entry12Details;
            newLiq.GOExpHist_Entry12Entity = credLiq.GOExpHist_Entry12Entity;
            newLiq.GOExpHist_Entry12Division = credLiq.GOExpHist_Entry12Division;
            newLiq.GOExpHist_Entry12InterAmt = credLiq.GOExpHist_Entry12InterAmt;
            newLiq.GOExpHist_Entry12InterRate = credLiq.GOExpHist_Entry12InterRate;
            //entry 2-1
            newLiq.GOExpHist_Entry21Type = credLiq.GOExpHist_Entry21Type;
            newLiq.GOExpHist_Entry21Ccy = credLiq.GOExpHist_Entry21Ccy;
            newLiq.GOExpHist_Entry21Cust = credLiq.GOExpHist_Entry21Cust;
            newLiq.GOExpHist_Entry21Actcde = credLiq.GOExpHist_Entry21Actcde;
            newLiq.GOExpHist_Entry21ActType = credLiq.GOExpHist_Entry21ActType;
            newLiq.GOExpHist_Entry21ActNo = credLiq.GOExpHist_Entry21ActNo;
            newLiq.GOExpHist_Entry21ExchRate = credLiq.GOExpHist_Entry21ExchRate;
            newLiq.GOExpHist_Entry21ExchCcy = credLiq.GOExpHist_Entry21ExchCcy;
            newLiq.GOExpHist_Entry21Fund = credLiq.GOExpHist_Entry21Fund;
            newLiq.GOExpHist_Entry21AdvcPrnt = credLiq.GOExpHist_Entry21AdvcPrnt;
            newLiq.GOExpHist_Entry21Details = credLiq.GOExpHist_Entry21Details;
            newLiq.GOExpHist_Entry21Entity = credLiq.GOExpHist_Entry21Entity;
            newLiq.GOExpHist_Entry21Division = credLiq.GOExpHist_Entry21Division;
            newLiq.GOExpHist_Entry21InterAmt = credLiq.GOExpHist_Entry21InterAmt;
            newLiq.GOExpHist_Entry21InterRate = credLiq.GOExpHist_Entry21InterRate;
            //entry 2-2
            newLiq.GOExpHist_Entry22Type = credLiq.GOExpHist_Entry22Type;
            newLiq.GOExpHist_Entry22Ccy = credLiq.GOExpHist_Entry22Ccy;
            newLiq.GOExpHist_Entry22Cust = credLiq.GOExpHist_Entry22Cust;
            newLiq.GOExpHist_Entry22Actcde = credLiq.GOExpHist_Entry22Actcde;
            newLiq.GOExpHist_Entry22ActType = credLiq.GOExpHist_Entry22ActType;
            newLiq.GOExpHist_Entry22ActNo = credLiq.GOExpHist_Entry22ActNo;
            newLiq.GOExpHist_Entry22ExchRate = credLiq.GOExpHist_Entry22ExchRate;
            newLiq.GOExpHist_Entry22ExchCcy = credLiq.GOExpHist_Entry22ExchCcy;
            newLiq.GOExpHist_Entry22Fund = credLiq.GOExpHist_Entry22Fund;
            newLiq.GOExpHist_Entry22AdvcPrnt = credLiq.GOExpHist_Entry22AdvcPrnt;
            newLiq.GOExpHist_Entry22Details = credLiq.GOExpHist_Entry22Details;
            newLiq.GOExpHist_Entry22Entity = credLiq.GOExpHist_Entry22Entity;
            newLiq.GOExpHist_Entry22Division = credLiq.GOExpHist_Entry22Division;
            newLiq.GOExpHist_Entry22InterAmt = credLiq.GOExpHist_Entry22InterAmt;
            newLiq.GOExpHist_Entry22InterRate = credLiq.GOExpHist_Entry22InterRate;
            //entry 3-1
            newLiq.GOExpHist_Entry31Type = credLiq.GOExpHist_Entry31Type;
            newLiq.GOExpHist_Entry31Ccy = credLiq.GOExpHist_Entry31Ccy;
            newLiq.GOExpHist_Entry31Cust = credLiq.GOExpHist_Entry31Cust;
            newLiq.GOExpHist_Entry31Actcde = credLiq.GOExpHist_Entry31Actcde;
            newLiq.GOExpHist_Entry31ActType = credLiq.GOExpHist_Entry31ActType;
            newLiq.GOExpHist_Entry31ActNo = credLiq.GOExpHist_Entry31ActNo;
            newLiq.GOExpHist_Entry31ExchRate = credLiq.GOExpHist_Entry31ExchRate;
            newLiq.GOExpHist_Entry31ExchCcy = credLiq.GOExpHist_Entry31ExchCcy;
            newLiq.GOExpHist_Entry31Fund = credLiq.GOExpHist_Entry31Fund;
            newLiq.GOExpHist_Entry31AdvcPrnt = credLiq.GOExpHist_Entry31AdvcPrnt;
            newLiq.GOExpHist_Entry31Details = credLiq.GOExpHist_Entry31Details;
            newLiq.GOExpHist_Entry31Entity = credLiq.GOExpHist_Entry31Entity;
            newLiq.GOExpHist_Entry31Division = credLiq.GOExpHist_Entry31Division;
            newLiq.GOExpHist_Entry31InterAmt = credLiq.GOExpHist_Entry31InterAmt;
            newLiq.GOExpHist_Entry31InterRate = credLiq.GOExpHist_Entry31InterRate;
            //entry 3-2
            newLiq.GOExpHist_Entry32Type = credLiq.GOExpHist_Entry32Type;
            newLiq.GOExpHist_Entry32Ccy = credLiq.GOExpHist_Entry32Ccy;
            newLiq.GOExpHist_Entry32Cust = credLiq.GOExpHist_Entry32Cust;
            newLiq.GOExpHist_Entry32Actcde = credLiq.GOExpHist_Entry32Actcde;
            newLiq.GOExpHist_Entry32ActType = credLiq.GOExpHist_Entry32ActType;
            newLiq.GOExpHist_Entry32ActNo = credLiq.GOExpHist_Entry32ActNo;
            newLiq.GOExpHist_Entry32ExchRate = credLiq.GOExpHist_Entry32ExchRate;
            newLiq.GOExpHist_Entry32ExchCcy = credLiq.GOExpHist_Entry32ExchCcy;
            newLiq.GOExpHist_Entry32Fund = credLiq.GOExpHist_Entry32Fund;
            newLiq.GOExpHist_Entry32AdvcPrnt = credLiq.GOExpHist_Entry32AdvcPrnt;
            newLiq.GOExpHist_Entry32Details = credLiq.GOExpHist_Entry32Details;
            newLiq.GOExpHist_Entry32Entity = credLiq.GOExpHist_Entry32Entity;
            newLiq.GOExpHist_Entry32Division = credLiq.GOExpHist_Entry32Division;
            newLiq.GOExpHist_Entry32InterAmt = credLiq.GOExpHist_Entry32InterAmt;
            newLiq.GOExpHist_Entry32InterRate = credLiq.GOExpHist_Entry32InterRate;
            //entry 4-1
            newLiq.GOExpHist_Entry41Type = credLiq.GOExpHist_Entry41Type;
            newLiq.GOExpHist_Entry41Ccy = credLiq.GOExpHist_Entry41Ccy;
            newLiq.GOExpHist_Entry41Cust = credLiq.GOExpHist_Entry41Cust;
            newLiq.GOExpHist_Entry41Actcde = credLiq.GOExpHist_Entry41Actcde;
            newLiq.GOExpHist_Entry41ActType = credLiq.GOExpHist_Entry41ActType;
            newLiq.GOExpHist_Entry41ActNo = credLiq.GOExpHist_Entry41ActNo;
            newLiq.GOExpHist_Entry41ExchRate = credLiq.GOExpHist_Entry41ExchRate;
            newLiq.GOExpHist_Entry41ExchCcy = credLiq.GOExpHist_Entry41ExchCcy;
            newLiq.GOExpHist_Entry41Fund = credLiq.GOExpHist_Entry41Fund;
            newLiq.GOExpHist_Entry41AdvcPrnt = credLiq.GOExpHist_Entry41AdvcPrnt;
            newLiq.GOExpHist_Entry41Details = credLiq.GOExpHist_Entry41Details;
            newLiq.GOExpHist_Entry41Entity = credLiq.GOExpHist_Entry41Entity;
            newLiq.GOExpHist_Entry41Division = credLiq.GOExpHist_Entry41Division;
            newLiq.GOExpHist_Entry41InterAmt = credLiq.GOExpHist_Entry41InterAmt;
            newLiq.GOExpHist_Entry41InterRate = credLiq.GOExpHist_Entry41InterRate;
            //entry 4-2
            newLiq.GOExpHist_Entry42Type = credLiq.GOExpHist_Entry42Type;
            newLiq.GOExpHist_Entry42Ccy = credLiq.GOExpHist_Entry42Ccy;
            newLiq.GOExpHist_Entry42Cust = credLiq.GOExpHist_Entry42Cust;
            newLiq.GOExpHist_Entry42Actcde = credLiq.GOExpHist_Entry42Actcde;
            newLiq.GOExpHist_Entry42ActType = credLiq.GOExpHist_Entry42ActType;
            newLiq.GOExpHist_Entry42ActNo = credLiq.GOExpHist_Entry42ActNo;
            newLiq.GOExpHist_Entry42ExchRate = credLiq.GOExpHist_Entry42ExchRate;
            newLiq.GOExpHist_Entry42ExchCcy = credLiq.GOExpHist_Entry42ExchCcy;
            newLiq.GOExpHist_Entry42Fund = credLiq.GOExpHist_Entry42Fund;
            newLiq.GOExpHist_Entry42AdvcPrnt = credLiq.GOExpHist_Entry42AdvcPrnt;
            newLiq.GOExpHist_Entry42Details = credLiq.GOExpHist_Entry42Details;
            newLiq.GOExpHist_Entry42Entity = credLiq.GOExpHist_Entry42Entity;
            newLiq.GOExpHist_Entry42Division = credLiq.GOExpHist_Entry42Division;
            newLiq.GOExpHist_Entry42InterAmt = credLiq.GOExpHist_Entry42InterAmt;
            newLiq.GOExpHist_Entry42InterRate = credLiq.GOExpHist_Entry42InterRate;

            return newLiq;
        }
        public DMBCSViewModel GetSignatoryInfo(int id)
        {
            return _context.DMBCS.Where(x => x.BCS_ID == id).Join(_context.User, b => b.BCS_User_ID,
                e => e.User_ID, (b, e) => new DMBCSViewModel
                { BCS_Position = b.BCS_Position, BCS_Name = e.User_LName + ", " + e.User_FName }).FirstOrDefault();
        }

        // [Entry Petty Cash Voucher]
        public IEnumerable<DMVendorModel> PopulateVendorList()
        {
            return _context.DMVendor.Where(x => x.Vendor_isActive == true
                && x.Vendor_isDeleted == false).OrderBy(x => x.Vendor_Name).ToList();
        }
        public IEnumerable<DMAccountModel> PopulateAccountList()
        {
            return _context.DMAccount.Where(x => x.Account_isActive == true
                && x.Account_isDeleted == false).OrderBy(x => x.Account_Name).ToList();
        }
        public IEnumerable<DMDeptModel> PopulateDepartmentList()
        {
            return _context.DMDept.Where(x => x.Dept_isActive == true
                && x.Dept_isDeleted == false).OrderBy(x => x.Dept_Name).ToList();
        }
        public IEnumerable<DMTRModel> PopulateTaxRateList()
        {
            return _context.DMTR.Where(x => x.TR_isActive == true
                && x.TR_isDeleted == false).OrderBy(x => x.TR_Tax_Rate).ToList();
        }

        //MISC

        //--------------------TEMP LOCATION-->MOVE TO ACCOUNT SERVICE-----------------------
        public bool sendEmail(ForgotPWViewModel model)
        {
            EmailService _email = new EmailService();
            var email = "monina.martinn@gmail.com";
            var subject = "EPS New PW";
            var hmtlMessage = "You will recieve new password.";
            _email.SendEmailAsync(email, subject, hmtlMessage);
            return true;
        }

        //--------------------Expense Entries--------------------------------

        ////============[Access Entry Tables]===============================
        //save expense details
        public int addExpense_CV(EntryCVViewModelList entryModel, int userId, int expenseType)
        {
            decimal TotalDebit = 0;
            decimal credEwtTotal = 0;
            decimal credCashTotal = 0;

            foreach (EntryCVViewModel cv in entryModel.EntryCV)
            {
                TotalDebit += cv.debitGross;
                credEwtTotal += cv.credEwt;
                credCashTotal += cv.credCash;
            }

            if (_modelState.IsValid)
            {
                List<ExpenseEntryDetailModel> expenseDtls = new List<ExpenseEntryDetailModel>();

                foreach (EntryCVViewModel cv in entryModel.EntryCV)
                {
                    List<ExpenseEntryAmortizationModel> expenseAmor = new List<ExpenseEntryAmortizationModel>();
                    List<ExpenseEntryCashBreakdownModel> expenseCashBreakdown = new List<ExpenseEntryCashBreakdownModel>();
                    List<ExpenseEntryGbaseDtl> expenseGbase = new List<ExpenseEntryGbaseDtl>();

                    int creditAccMasterID1 = 0;
                    int creditAccMasterID2 = 0;

                    if (expenseType == GlobalSystemValues.TYPE_CV)
                    {
                        creditAccMasterID1 = int.Parse(xelemAcc.Element("C_CV1").Value);
                        creditAccMasterID2 = int.Parse(xelemAcc.Element("C_CV2").Value);

                        foreach (var amorSchedule in cv.amtDetails)
                        {
                            ExpenseEntryAmortizationModel amortization = new ExpenseEntryAmortizationModel
                            {
                                Amor_Sched_Date = amorSchedule.amtDate,
                                Amor_Price = amorSchedule.amtAmount,
                                Amor_Status = GlobalSystemValues.STATUS_PENDING,
                                Amor_Number = "21",
                                Amor_Account = cv.amorAcc
                            };

                            expenseAmor.Add(amortization);
                        }
                    }
                    else if (expenseType == GlobalSystemValues.TYPE_PC)
                    {
                        creditAccMasterID1 = int.Parse(xelemAcc.Element("C_PC1").Value);
                        creditAccMasterID2 = int.Parse(xelemAcc.Element("C_PC2").Value);

                        foreach (var cashbd in cv.cashBreakdown)
                        {
                            expenseCashBreakdown.Add(new ExpenseEntryCashBreakdownModel
                            {
                                CashBreak_Denomination = cashbd.cashDenomination,
                                CashBreak_NoPcs = cashbd.cashNoPC,
                                CashBreak_Amount = cashbd.cashAmount
                            });
                        }
                    }
                    else if (expenseType == GlobalSystemValues.TYPE_SS && cv.ccyAbbrev == "PHP")
                    {
                        foreach (var cashbd in cv.cashBreakdown)
                        {
                            expenseCashBreakdown.Add(new ExpenseEntryCashBreakdownModel
                            {
                                CashBreak_Denomination = cashbd.cashDenomination,
                                CashBreak_NoPcs = cashbd.cashNoPC,
                                CashBreak_Amount = cashbd.cashAmount
                            });
                        }
                    }

                    if (expenseType == GlobalSystemValues.TYPE_SS)
                    {
                        if (getAccount(cv.account).Account_MasterID == int.Parse(xelemAcc.Element("D_SS1").Value))
                        {
                            creditAccMasterID2 = int.Parse(xelemAcc.Element("C_SS1").Value);
                        }
                        else if (getAccount(cv.account).Account_MasterID == int.Parse(xelemAcc.Element("D_SS4").Value))
                        {
                            creditAccMasterID2 = int.Parse(xelemAcc.Element("C_SS2").Value);
                        }
                        else
                        {
                            creditAccMasterID2 = int.Parse(xelemAcc.Element("C_SS3").Value);
                        }
                    }

                    foreach (var gbaseRemark in cv.gBaseRemarksDetails)
                    {
                        ExpenseEntryGbaseDtl remarks = new ExpenseEntryGbaseDtl
                        {
                            GbaseDtl_Document_Type = gbaseRemark.docType,
                            GbaseDtl_InvoiceNo = gbaseRemark.invNo,
                            GbaseDtl_Description = gbaseRemark.desc,
                            GbaseDtl_Amount = gbaseRemark.amount
                        };
                        expenseGbase.Add(remarks);
                    }
                    //(dtl.d.ExpDtl_Vat <= 0) ? false : true
                    ExpenseEntryDetailModel expenseDetails = new ExpenseEntryDetailModel
                    {
                        ExpDtl_Gbase_Remarks = cv.GBaseRemarks,
                        ExpDtl_Account = cv.account,
                        ExpDtl_Fbt = cv.fbt,
                        ExpDtl_FbtID = (cv.fbt) ? getFbt(getAccount(cv.account).Account_FBT_MasterID) : 0,
                        ExpDtl_Dept = cv.dept,
                        ExpDtl_Vat = cv.vat,
                        ExpDtl_Ewt = cv.ewt,
                        ExpDtl_Ccy = cv.ccy,
                        ExpDtl_Debit = cv.debitGross,
                        ExpDtl_isEwt = (cv.ewt <= 0) ? false : true,
                        ExpDtl_Credit_Ewt = cv.credEwt,
                        ExpDtl_Credit_Cash = cv.credCash,
                        ExpDtl_SS_Payee = cv.dtlSSPayee,
                        ExpDtl_Amor_Month = cv.month,
                        ExpDtl_Amor_Day = cv.day,
                        ExpDtl_Amor_Duration = cv.duration,
                        ExpDtl_CreditAccount1 = (cv.credEwt > 0 && expenseType != GlobalSystemValues.TYPE_SS) ? getAccountByMasterID(creditAccMasterID1).Account_ID : 0,
                        ExpDtl_CreditAccount2 = getAccountByMasterID(creditAccMasterID2).Account_ID,
                        ExpenseEntryAmortizations = expenseAmor,
                        ExpenseEntryGbaseDtls = expenseGbase,
                        ExpDtl_Ewt_Payor_Name_ID = (cv.chkEwt == false && cv.vat == 0) ? 0 : cv.dtl_Ewt_Payor_Name_ID,
                        ExpenseEntryCashBreakdowns = expenseCashBreakdown
                    };
                    expenseDtls.Add(expenseDetails);
                }

                //var checkNo = getCheckNo();

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Type = expenseType,
                    Expense_Date = entryModel.expenseDate,
                    Expense_Payee = entryModel.selectedPayee,
                    Expense_Payee_Type = entryModel.payee_type,
                    Expense_Debit_Total = TotalDebit,
                    Expense_Credit_Total = credEwtTotal + credCashTotal,
                    Expense_Creator_ID = userId,
                    Expense_Created_Date = (entryModel.entryID == 0) ? DateTime.Now : entryModel.createdDate,
                    Expense_Last_Updated = DateTime.Now,
                    Expense_isDeleted = false,
                    Expense_Status = 1,
                    Expense_Number = (String.IsNullOrEmpty(entryModel.expenseId)) ? 0 : int.Parse(entryModel.expenseId),
                    Expense_CheckNo = entryModel.checkNo,
                    Expense_CheckId = entryModel.checkId,
                    ExpenseEntryDetails = expenseDtls
                };


                _context.ExpenseEntry.Add(expenseEntry);
                _context.SaveChanges();
                return expenseEntry.Expense_ID;
            }
            return -1;
        }
        public bool generateNewCheck(int entryID)
        {
            var expense = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);

            if (expense != null)
            {

                var newCheckNo = getCheckNo();
                expense.Expense_CheckId = int.Parse(newCheckNo["id"]);
                expense.Expense_CheckNo = newCheckNo["check"];

                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public AmortizationList getAmortizationList()
        {
            AmortizationList model = new AmortizationList();

            List<int> expectedStatus = new List<int> { GlobalSystemValues.STATUS_FOR_PRINTING,
                                                       GlobalSystemValues.STATUS_FOR_CLOSING,
                                                       GlobalSystemValues.STATUS_POSTED};

            var amorPending = _context.ExpenseEntryAmortizations.Include(x => x.ExpenseEntryDetailModel.ExpenseEntryModel)
                                      .Join(_context.DMAccount,
                                            amo => amo.Amor_Account,
                                            acc => acc.Account_ID,
                                            (amo, acc) => new
                                            {
                                                amo.ExpenseEntryDetailModel.ExpenseEntryModel.Expense_ID,
                                                amo.ExpenseEntryDetailModel.ExpenseEntryModel.Expense_Number,
                                                amo.ExpenseEntryDetailModel.ExpenseEntryModel.Expense_Date,
                                                amo.ExpenseEntryDetailModel.ExpenseEntryModel.Expense_Status,
                                                amo.ExpenseEntryDetailModel.ExpenseEntryModel.Expense_Creator_ID,
                                                amo.Amor_ID,
                                                amo.Amor_Price,
                                                amo.Amor_Sched_Date,
                                                amo.Amor_Account,
                                                amo.Amor_Status,
                                                acc.Account_Name
                                            })
                                      .Where(y => y.Amor_Status == GlobalSystemValues.STATUS_PENDING
                                               && expectedStatus.Contains(y.Expense_Status)
                                               && y.Amor_Sched_Date <= DateTime.Now);

            foreach (var item in amorPending)
            {
                model.amortizations.Add(new amorViewModel
                {
                    account = item.Account_Name,
                    amount = item.Amor_Price,
                    maker = getUserFullName(item.Expense_Creator_ID),
                    sched = item.Amor_Sched_Date,
                    voucherNo = getVoucherNo(1, item.Expense_Date, item.Expense_Number),
                    link = "NCAmortization?AmorID=" + item.Amor_ID
                });
            }

            return model;
        }
        public amorViewModel getAmortizationDetails(int amorID)
        {
            amorViewModel model = new amorViewModel();

            var dbItem = _context.ExpenseEntryAmortizations
                                 .Include(x => x.ExpenseEntryDetailModel.ExpenseEntryModel)
                                 .FirstOrDefault(x => x.Amor_ID == amorID);

            model.debit_acc_id = dbItem.Amor_Account;
            model.amount = dbItem.Amor_Price;
            model.sched = dbItem.Amor_Sched_Date;
            model.credit_acc_id = dbItem.ExpenseEntryDetailModel.ExpDtl_Account;
            model.vendor_id = dbItem.ExpenseEntryDetailModel.ExpenseEntryModel.Expense_Payee;

            return model;
        }
        public void updateAmorStatus(int amorID)
        {
            var amortizationRecord = _context.ExpenseEntryAmortizations.FirstOrDefault(x => x.Amor_ID == amorID);

            amortizationRecord.Amor_Status = GlobalSystemValues.STATUS_APPROVED;
            _context.SaveChanges();
        }
        //retrieve expense details
        public EntryCVViewModelList getExpense(int transID)
        {
            List<EntryCVViewModel> cvList = new List<EntryCVViewModel>();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryDetails = from d
                                                          in _context.ExpenseEntryDetails
                                                          where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                          select new
                                                          {
                                                              d,
                                                              ExpenseEntryGbaseDtls = from g
                                                                                      in _context.ExpenseEntryGbaseDtls
                                                                                      where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                      select g,
                                                              ExpenseEntryAmortizations = from a
                                                                                          in _context.ExpenseEntryAmortizations
                                                                                          where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                          select a,
                                                              ExpenseEntryCashBreakdown = (from c
                                                                                               in _context.ExpenseEntryCashBreakdown
                                                                                           where c.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                           select c).OrderByDescending(db => db.ExpenseEntryDetailModel.ExpDtl_ID).OrderByDescending(db => db.CashBreak_Denomination)
                                                          }
                                }).FirstOrDefault();

            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                List<amortizationSchedule> amtDetails = new List<amortizationSchedule>();
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                List<CashBreakdown> cashBreakdown = new List<CashBreakdown>();

                int amorAcc = 0;

                foreach (var amor in dtl.ExpenseEntryAmortizations)
                {
                    amortizationSchedule amorTemp = new amortizationSchedule()
                    {
                        amtDate = amor.Amor_Sched_Date,
                        amtAmount = amor.Amor_Price
                    };
                    amorAcc = amor.Amor_Account;
                    amtDetails.Add(amorTemp);
                }

                foreach (var gbase in dtl.ExpenseEntryGbaseDtls)
                {
                    EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel()
                    {
                        amount = gbase.GbaseDtl_Amount,
                        desc = gbase.GbaseDtl_Description,
                        docType = gbase.GbaseDtl_Document_Type,
                        invNo = gbase.GbaseDtl_InvoiceNo
                    };

                    remarksDtl.Add(gbaseTemp);
                }

                foreach (var cashbd in dtl.ExpenseEntryCashBreakdown)
                {
                    CashBreakdown cashbdTemp = new CashBreakdown()
                    {
                        cashDenomination = cashbd.CashBreak_Denomination,
                        cashNoPC = cashbd.CashBreak_NoPcs,
                        cashAmount = cashbd.CashBreak_Amount
                    };

                    cashBreakdown.Add(cashbdTemp);
                }

                EntryCVViewModel cvDtl = new EntryCVViewModel()
                {
                    expenseDtlID = dtl.d.ExpDtl_ID,
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    account = dtl.d.ExpDtl_Account,
                    fbt = dtl.d.ExpDtl_Fbt,
                    fbtID = dtl.d.ExpDtl_FbtID,
                    dept = dtl.d.ExpDtl_Dept,
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vat = dtl.d.ExpDtl_Vat,
                    chkEwt = dtl.d.ExpDtl_isEwt,
                    ewt = dtl.d.ExpDtl_Ewt,
                    ccy = dtl.d.ExpDtl_Ccy,
                    ccyMasterID = (dtl.d.ExpDtl_Ccy != 0) ? getCurrency(dtl.d.ExpDtl_Ccy).Curr_MasterID : 0,
                    ccyAbbrev = (dtl.d.ExpDtl_Ccy != 0) ? getCurrency(dtl.d.ExpDtl_Ccy).Curr_CCY_ABBR : "",
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    creditAccount1 = dtl.d.ExpDtl_CreditAccount1,
                    creditAccount2 = dtl.d.ExpDtl_CreditAccount2,
                    dtlSSPayee = dtl.d.ExpDtl_SS_Payee,
                    dtl_Ewt_Payor_Name_ID = dtl.d.ExpDtl_Ewt_Payor_Name_ID,
                    month = dtl.d.ExpDtl_Amor_Month,
                    day = dtl.d.ExpDtl_Amor_Day,
                    duration = dtl.d.ExpDtl_Amor_Duration,
                    amtDetails = amtDetails,
                    gBaseRemarksDetails = remarksDtl,
                    cashBreakdown = cashBreakdown,
                    modalInputFlag = (cashBreakdown == null || cashBreakdown.Count == 0) ? 0 : 1,
                    amorAcc = amorAcc
                };
                cvList.Add(cvDtl);
            }

            EntryCVViewModelList cvModel = new EntryCVViewModelList()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                selectedPayee = EntryDetails.e.Expense_Payee,
                payee_type = EntryDetails.e.Expense_Payee_Type,
                expenseYear = EntryDetails.e.Expense_Date.Year.ToString(),
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5, '0'),
                checkNo = EntryDetails.e.Expense_CheckNo,
                checkId = EntryDetails.e.Expense_CheckId,
                status = getStatus(EntryDetails.e.Expense_Status),
                statusID = EntryDetails.e.Expense_Status,
                approver_id = EntryDetails.e.Expense_Approver,
                verifier_1_id = EntryDetails.e.Expense_Verifier_1,
                verifier_2_id = EntryDetails.e.Expense_Verifier_2,
                approver = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Approver),
                verifier_1 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_1),
                verifier_2 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_2),
                maker = EntryDetails.e.Expense_Creator_ID,
                lastUpdatedDate = EntryDetails.e.Expense_Last_Updated,
                createdDate = EntryDetails.e.Expense_Created_Date,
                expenseType = EntryDetails.e.Expense_Type,
                EntryCV = cvList
            };

            return cvModel;
        }

        public List<CONSTANT_NC_VALS> getInterEntityAccs(string Loc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("wwwroot/xml/DDVInterEntityAccounts.xml");
            XmlNodeList nodeList = doc.SelectNodes(Loc);
            List<CONSTANT_NC_VALS> valList = new List<CONSTANT_NC_VALS>();
            foreach (XmlNode no in nodeList)
            {
                var rawVal = no.InnerText;
                var acc = _context.DMAccount.Where(x => (x.Account_MasterID == int.Parse(rawVal))
                                                    && x.Account_isActive == true && x.Account_isDeleted == false).FirstOrDefault();
                CONSTANT_NC_VALS vals = new CONSTANT_NC_VALS
                {
                    accID = acc.Account_ID,
                    accNo = acc.Account_No,
                    accName = acc.Account_Name
                };
                valList.Add(vals);
            }
            return valList;
        }
        // [RETRIEVE DDV EXPENSE DETAILS]
        public EntryDDVViewModelList getExpenseDDV(int transID)
        {
            List<EntryDDVViewModel> ddvList = new List<EntryDDVViewModel>();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryDetails = from d
                                                          in _context.ExpenseEntryDetails
                                                          where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                          select new
                                                          {
                                                              d,
                                                              ExpenseEntryGbaseDtls = from g
                                                                                      in _context.ExpenseEntryGbaseDtls
                                                                                      where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                      select g,
                                                              ExpenseEntryInterEntity = from a
                                                                                          in _context.ExpenseEntryInterEntity
                                                                                        where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                        select new
                                                                                        {
                                                                                            a,
                                                                                            ExpenseEntryInterEntityParticular = from p
                                                                                                                                in _context.ExpenseEntryInterEntityParticular
                                                                                                                                where p.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == a.ExpDtl_DDVInter_ID
                                                                                                                                select new
                                                                                                                                {
                                                                                                                                    p,
                                                                                                                                    ExpenseEntryEntityAccounts = from acc
                                                                                                                                                                 in _context.ExpenseEntryInterEntityAccs
                                                                                                                                                                 where acc.ExpenseEntryInterEntityParticular.InterPart_ID == p.InterPart_ID
                                                                                                                                                                 select acc
                                                                                                                                }
                                                                                        }
                                                          }
                                }).FirstOrDefault();
            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                DDVInterEntityViewModel interDetail = new DDVInterEntityViewModel();
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                ExpenseEntryInterEntityAccsViewModel interDetailAccs = new ExpenseEntryInterEntityAccsViewModel();
                foreach (var inter in dtl.ExpenseEntryInterEntity)
                {
                    interDetail = new DDVInterEntityViewModel
                    {
                        Inter_Check1 = inter.a.ExpDtl_DDVInter_Check1,
                        Inter_Check2 = inter.a.ExpDtl_DDVInter_Check2,
                        Inter_Convert1_Amount = inter.a.ExpDtl_DDVInter_Conv_Amount1,
                        Inter_Convert2_Amount = inter.a.ExpDtl_DDVInter_Conv_Amount2,
                        Inter_Currency1_ID = inter.a.ExpDtl_DDVInter_Curr1_ID,
                        Inter_Currency1_ABBR = "",
                        Inter_Currency1_Amount = inter.a.ExpDtl_DDVInter_Amount1,
                        Inter_Currency2_ID = inter.a.ExpDtl_DDVInter_Curr2_ID,
                        Inter_Currency2_ABBR = "",
                        Inter_Currency2_Amount = inter.a.ExpDtl_DDVInter_Amount2,
                        Inter_Rate = (inter.a.ExpDtl_DDVInter_Rate > 0) ? inter.a.ExpDtl_DDVInter_Rate : 1,
                        interPartList = new List<ExpenseEntryInterEntityParticularViewModel>()

                    };

                    if (interDetail.Inter_Currency1_ID > 0)
                    {
                        interDetail.Inter_Currency1_ABBR = _context.DMCurrency.Where(x => x.Curr_ID == inter.a.ExpDtl_DDVInter_Curr1_ID &&
                                                       x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                    }
                    if (interDetail.Inter_Currency2_ID > 0)
                    {
                        interDetail.Inter_Currency2_ABBR = _context.DMCurrency.Where(x => x.Curr_ID == inter.a.ExpDtl_DDVInter_Curr2_ID &&
                                                    x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                    }
                    //ERROR HERE    
                    foreach (var interPart in inter.ExpenseEntryInterEntityParticular)
                    {
                        var acc = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account).FirstOrDefault();
                        ExpenseEntryInterEntityParticularViewModel interParticular = new ExpenseEntryInterEntityParticularViewModel
                        {
                            InterPart_ID = interPart.p.InterPart_ID,
                            InterPart_Particular_Title = interPart.p.InterPart_Particular_Title,
                            Inter_Particular1 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular1(acc.Account_Name ?? "", interDetail.Inter_Currency1_ABBR ?? "", interDetail.Inter_Currency1_Amount, interDetail.Inter_Currency2_Amount, interDetail.Inter_Rate, acc.Account_ID, interDetail.Inter_Currency1_ID, getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry1']")),
                            Inter_Particular2 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular2(interDetail.Inter_Currency1_ABBR, interDetail.Inter_Currency2_ABBR, interDetail.Inter_Currency2_Amount, interDetail.Inter_Rate, interDetail.Inter_Currency1_ID, interDetail.Inter_Currency2_ID, getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry2']")),
                            Inter_Particular3 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular3(interDetail.Inter_Currency2_ABBR, interDetail.Inter_Currency2_Amount, interDetail.Inter_Currency2_ID, getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry3']")),
                            ExpenseEntryInterEntityAccs = new List<ExpenseEntryInterEntityAccsViewModel>()
                        };
                        foreach (var interAcc in interPart.ExpenseEntryEntityAccounts)
                        {
                            ExpenseEntryInterEntityAccsViewModel interDetailAcc = new ExpenseEntryInterEntityAccsViewModel()
                            {
                                Inter_Acc_ID = interAcc.InterAcc_Acc_ID,
                                Inter_Amount = interAcc.InterAcc_Amount,
                                Inter_Curr_ID = interAcc.InterAcc_Curr_ID,
                                Inter_Rate = interAcc.InterAcc_Rate,
                                Inter_Type_ID = interAcc.InterAcc_Type_ID
                            };
                            interParticular.ExpenseEntryInterEntityAccs.Add(interDetailAcc);
                        }
                        interDetail.interPartList.Add(interParticular);
                    }
                }

                foreach (var gbase in dtl.ExpenseEntryGbaseDtls)
                {
                    EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel()
                    {
                        amount = gbase.GbaseDtl_Amount,
                        desc = gbase.GbaseDtl_Description,
                        docType = gbase.GbaseDtl_Document_Type,
                        invNo = gbase.GbaseDtl_InvoiceNo
                    };

                    remarksDtl.Add(gbaseTemp);
                }


                EntryDDVViewModel ddvDtl = new EntryDDVViewModel()
                {
                    dtlID = dtl.d.ExpDtl_ID,
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    account = dtl.d.ExpDtl_Account,
                    account_Name = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account && x.Account_isActive == true).Select(x => x.Account_No + " - " + x.Account_Name).FirstOrDefault(),
                    inter_entity = dtl.d.ExpDtl_Inter_Entity,
                    fbt = dtl.d.ExpDtl_Fbt,
                    dept = dtl.d.ExpDtl_Dept,
                    dept_Name = _context.DMDept.Where(x => x.Dept_ID == dtl.d.ExpDtl_Dept && x.Dept_isActive == true).Select(x => x.Dept_Name).FirstOrDefault(),
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vat = dtl.d.ExpDtl_Vat,
                    //vat_Name is actually the rate
                    vat_Name = (dtl.d.ExpDtl_Vat > 0) ? _context.DMVAT.Where(x => x.VAT_ID == dtl.d.ExpDtl_Vat && x.VAT_isActive == true).Select(x => (x.VAT_Rate * 100).ToString()).FirstOrDefault() : "0",
                    chkEwt = (dtl.d.ExpDtl_Ewt <= 0) ? false : true,
                    ewt = dtl.d.ExpDtl_Ewt,
                    ewt_Name = (dtl.d.ExpDtl_Ewt > 0) ? _context.DMTR.Where(x => x.TR_ID == dtl.d.ExpDtl_Ewt).Select(x => (x.TR_Tax_Rate * 100).ToString()).FirstOrDefault() : "0",
                    ewt_Payor_Name = (dtl.d.ExpDtl_Ewt_Payor_Name_ID >= 0) ? _context.DMVendor.Where(x => x.Vendor_ID == dtl.d.ExpDtl_Ewt_Payor_Name_ID).Select(x => x.Vendor_Name).FirstOrDefault() : "",
                    ccy = dtl.d.ExpDtl_Ccy,
                    ccy_Name = _context.DMCurrency.Where(x => x.Curr_ID == dtl.d.ExpDtl_Ccy && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault(),
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    creditAccount1 = dtl.d.ExpDtl_CreditAccount1,
                    creditAccount2 = dtl.d.ExpDtl_CreditAccount2,
                    ewt_Payor_Name_ID = (dtl.d.ExpDtl_Ewt_Payor_Name_ID >= 0) ? dtl.d.ExpDtl_Ewt_Payor_Name_ID : 0,
                    interDetails = interDetail,
                    gBaseRemarksDetails = remarksDtl
                };
                ddvList.Add(ddvDtl);
            }

            EntryDDVViewModelList ddvModel = new EntryDDVViewModelList()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                vendor = EntryDetails.e.Expense_Payee,
                expenseYear = EntryDetails.e.Expense_Date.Year.ToString(),
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5, '0'),
                checkNo = EntryDetails.e.Expense_CheckNo,
                status = getStatus(EntryDetails.e.Expense_Status),
                statusID = EntryDetails.e.Expense_Status,
                approver = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Approver),
                verifier_1 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_1),
                verifier_2 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_2),
                approver_id = EntryDetails.e.Expense_Approver,
                verifier_1_id = EntryDetails.e.Expense_Verifier_1,
                verifier_2_id = EntryDetails.e.Expense_Verifier_2,
                maker = EntryDetails.e.Expense_Creator_ID,
                lastUpdatedDate = EntryDetails.e.Expense_Last_Updated,
                vendor_Name = (EntryDetails.e.Expense_Payee == 0) ? "" : getVendorName(EntryDetails.e.Expense_Payee, EntryDetails.e.Expense_Payee_Type),
                payee_type = EntryDetails.e.Expense_Payee_Type,
                payee_type_Name = (EntryDetails.e.Expense_Payee_Type == 0) ? "" : getPayeeTypeName(EntryDetails.e.Expense_Payee_Type),
                EntryDDV = ddvList
            };

            return ddvModel;
        }
        // [RETRIEVE NC EXPENSE DETAILS]
        public EntryNCViewModelList getExpenseNC(int transID)
        {
            List<EntryNCViewModel> ncList = new List<EntryNCViewModel>();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryNC = from d
                                                          in _context.ExpenseEntryNonCash
                                                     where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                     select new
                                                     {
                                                         d,
                                                         ExpenseEntryNCDtls = from g
                                                                                 in _context.ExpenseEntryNonCashDetails
                                                                              where g.ExpenseEntryNCModel.ExpNC_ID == d.ExpNC_ID
                                                                              select new
                                                                              {
                                                                                  g,
                                                                                  ExpenseEntryNCDtlAccs = from a
                                                                                                           in _context.ExpenseEntryNonCashDetailAccounts
                                                                                                          where a.ExpenseEntryNCDtlModel.ExpNCDtl_ID == g.ExpNCDtl_ID
                                                                                                          orderby a.ExpNCDtlAcc_Type_ID
                                                                                                          select a
                                                                              }

                                                     }
                                }).FirstOrDefault();
            EntryNCViewModel ncDtlVM = new EntryNCViewModel();
            foreach (var dtl in EntryDetails.ExpenseEntryNC)
            {
                List<ExpenseEntryNCDtlViewModel> ncDtls = new List<ExpenseEntryNCDtlViewModel>();
                ExpenseEntryNCDtlViewModel entryNCDtl;
                foreach (var ncDtl in dtl.ExpenseEntryNCDtls)
                {
                    List<ExpenseEntryNCDtlAccViewModel> ncDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>();
                    ExpenseEntryNCDtlAccViewModel entryNCDtlAcc;
                    foreach (var ncDtlAcc in ncDtl.ExpenseEntryNCDtlAccs)
                    {
                        entryNCDtlAcc = new ExpenseEntryNCDtlAccViewModel()
                        {
                            ExpNCDtlAcc_Acc_ID = ncDtlAcc.ExpNCDtlAcc_Acc_ID,
                            ExpNCDtlAcc_Acc_Name = ncDtlAcc.ExpNCDtlAcc_Acc_Name ?? "",
                            ExpNCDtlAcc_Curr_ID = ncDtlAcc.ExpNCDtlAcc_Curr_ID,
                            ExpNCDtlAcc_Curr_Name = "",
                            ExpNCDtlAcc_Inter_Rate = ncDtlAcc.ExpNCDtlAcc_Inter_Rate,
                            ExpNCDtlAcc_Amount = ncDtlAcc.ExpNCDtlAcc_Amount,
                            ExpNCDtlAcc_Type_ID = ncDtlAcc.ExpNCDtlAcc_Type_ID
                        };
                        if (entryNCDtlAcc.ExpNCDtlAcc_Acc_ID != 0)
                        {
                            entryNCDtlAcc.ExpNCDtlAcc_Acc_Name = _context.DMAccount.Where(x => x.Account_ID == entryNCDtlAcc.ExpNCDtlAcc_Acc_ID).Select(x => x.Account_No + " - " + x.Account_Name).FirstOrDefault() ?? "";
                        }
                        if (entryNCDtlAcc.ExpNCDtlAcc_Curr_ID != 0)
                        {
                            entryNCDtlAcc.ExpNCDtlAcc_Curr_Name = _context.DMCurrency.Where(x => x.Curr_ID == entryNCDtlAcc.ExpNCDtlAcc_Curr_ID).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                        }
                        ncDtlAccs.Add(entryNCDtlAcc);
                    }
                    //sort accs by debit, then tax accounts, then credit
                    ncDtlAccs = ncDtlAccs.OrderByDescending(x => x.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT).ThenByDescending(x => x.ExpNCDtlAcc_Type_ID).ToList();
                    entryNCDtl = new ExpenseEntryNCDtlViewModel()
                    {
                        ExpNCDtl_ID = ncDtl.g.ExpNCDtl_ID,
                        ExpNCDtl_Remarks_Desc = ncDtl.g.ExpNCDtl_Remarks_Desc,
                        ExpNCDtl_Remarks_Period = ncDtl.g.ExpNCDtl_Remarks_Period,
                        ExpNCDtl_TR_ID = ncDtl.g.ExpNCDtl_TR_ID,
                        ExpNCDtl_TR_Title = ncDtl.g.ExpNCDtl_TR_ID > 0 ? _context.DMTR.FirstOrDefault(x => x.TR_ID == ncDtl.g.ExpNCDtl_TR_ID).TR_WT_Title : "",
                        ExpNCDtl_Vendor_ID = ncDtl.g.ExpNCDtl_Vendor_ID,
                        ExpNCDtl_Vendor_Name = ncDtl.g.ExpNCDtl_Vendor_ID > 0 ? _context.DMVendor.FirstOrDefault(x => x.Vendor_ID == ncDtl.g.ExpNCDtl_Vendor_ID).Vendor_Name : "",
                        ExpNCDtl_TaxBasedAmt = ncDtl.g.ExpNCDtl_TaxBasedAmt,
                        ExpenseEntryNCDtlAccs = ncDtlAccs
                    };
                    ncDtls.Add(entryNCDtl);
                }
                ncDtlVM = new EntryNCViewModel()
                {
                    NC_ID = dtl.d.ExpNC_ID,
                    NC_Category_ID = dtl.d.ExpNC_Category_ID,
                    NC_CredAmt = dtl.d.ExpNC_CredAmt,
                    NC_DebitAmt = dtl.d.ExpNC_DebitAmt,
                    NC_CS_CredAmt = dtl.d.ExpNC_CS_CredAmt,
                    NC_CS_DebitAmt = dtl.d.ExpNC_CS_DebitAmt,
                    NC_CS_Period = dtl.d.ExpNC_CS_Period,
                    NC_IE_CredAmt = dtl.d.ExpNC_IE_CredAmt,
                    NC_IE_DebitAmt = dtl.d.ExpNC_IE_DebitAmt,
                    NC_TotalAmt = dtl.d.ExpNC_CredAmt + dtl.d.ExpNC_DebitAmt,
                    NC_CS_TotalAmt = dtl.d.ExpNC_CS_CredAmt + dtl.d.ExpNC_CS_DebitAmt,
                    NC_IE_TotalAmt = dtl.d.ExpNC_IE_CredAmt + dtl.d.ExpNC_IE_DebitAmt,
                    ExpenseEntryNCDtls = ncDtls,
                };
            }
            EntryNCViewModelList ncModel = new EntryNCViewModelList()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                status = getStatus(EntryDetails.e.Expense_Status),
                statusID = EntryDetails.e.Expense_Status,
                approver = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Approver),
                verifier_1 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_1),
                verifier_2 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_2),
                approver_id = EntryDetails.e.Expense_Approver,
                verifier_1_id = EntryDetails.e.Expense_Verifier_1,
                verifier_2_id = EntryDetails.e.Expense_Verifier_2,
                maker = EntryDetails.e.Expense_Creator_ID,
                lastUpdatedDate = EntryDetails.e.Expense_Last_Updated,
                EntryNC = ncDtlVM
            };
            return ncModel;
        }

        //update status of entry
        public bool updateExpenseStatus(int transID, int status, int userid)
        {
            ExpenseEntryModel m = new ExpenseEntryModel
            {
                Expense_ID = transID,
            };

            if (_modelState.IsValid)
            {
                List<int> forbiddenStatus = new List<int>{
                    GlobalSystemValues.STATUS_APPROVED,
                    GlobalSystemValues.STATUS_FOR_PRINTING,
                    GlobalSystemValues.STATUS_REJECTED,
                    GlobalSystemValues.STATUS_FOR_CLOSING,
                    GlobalSystemValues.STATUS_POSTED
                };

                ExpenseEntryModel dbExpenseEntry = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == transID);

                if (dbExpenseEntry == null)
                {
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE3;
                    return false;
                }
                if (forbiddenStatus.Contains(dbExpenseEntry.Expense_Status))
                {
                    if (dbExpenseEntry.Expense_Status == GlobalSystemValues.STATUS_APPROVED)
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE4;
                    }
                    if (dbExpenseEntry.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING)
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE5;
                    }
                    if (dbExpenseEntry.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING)
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE6;
                    }
                    if (dbExpenseEntry.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING)
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE7;
                    }
                    return false;
                }

                if (status == GlobalSystemValues.STATUS_VERIFIED)
                {
                    if (dbExpenseEntry.Expense_Verifier_1 == 0)
                    {
                        dbExpenseEntry.Expense_Verifier_1 = userid;
                    }
                    else if (dbExpenseEntry.Expense_Verifier_1 != userid)
                    {
                        if (dbExpenseEntry.Expense_Verifier_2 == 0)
                        {
                            dbExpenseEntry.Expense_Verifier_2 = userid;
                        }
                        else
                        {
                            GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE8;
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (status == GlobalSystemValues.STATUS_APPROVED || status == GlobalSystemValues.STATUS_REJECTED)
                {
                    dbExpenseEntry.Expense_Approver = userid;

                    if (status == GlobalSystemValues.STATUS_APPROVED)
                    {
                        dbExpenseEntry.Expense_Number = getExpTransNo(dbExpenseEntry.Expense_Type);
                        if (dbExpenseEntry.Expense_Type == GlobalSystemValues.TYPE_CV)
                        {
                            var checkNo = getCheckNo();
                            if (checkNo != null)
                            {
                                dbExpenseEntry.Expense_CheckNo = checkNo["check"];
                                dbExpenseEntry.Expense_CheckId = int.Parse(checkNo["id"]);
                            }
                            else
                            {
                                GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE9;
                                return false;
                            }
                        }
                    }
                }
                dbExpenseEntry.Expense_Status = status;
                dbExpenseEntry.Expense_Last_Updated = DateTime.Now;

                if (dbExpenseEntry.Expense_Status == GetCurrentEntryStatus(transID))
                {
                    _context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        //update status of entry of Reversal process
        public bool updateReversalStatus(int transID, int status, int userid)
        {
            ExpenseEntryModel m = new ExpenseEntryModel
            {
                Expense_ID = transID,
            };

            if (_modelState.IsValid)
            {
                ExpenseEntryModel dbExpenseEntry = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == transID);

                if (dbExpenseEntry == null)
                {
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE3;
                    return false;
                }

                dbExpenseEntry.Expense_Status = status;
                dbExpenseEntry.Expense_Last_Updated = DateTime.Now;

                if (dbExpenseEntry.Expense_Status == GetCurrentEntryStatus(transID))
                {
                    _context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        //Delete expense entry
        public bool deleteExpenseEntry(int expense_ID)
        {
            var entry = _context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID).First();
            var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
            foreach (var i in entryDtl)
            {
                _context.ExpenseEntryAmortizations.RemoveRange(_context.ExpenseEntryAmortizations
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                _context.ExpenseEntryCashBreakdown.RemoveRange(_context.ExpenseEntryCashBreakdown
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                _context.ExpenseEntryGbaseDtls.RemoveRange(_context.ExpenseEntryGbaseDtls
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
            }

            _context.ExpenseEntryDetails.RemoveRange(_context.ExpenseEntryDetails
                .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            _context.ExpenseEntry.RemoveRange(_context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID));

            _context.SaveChanges();

            return true;
        }
        //Delete expense entry NC / DDV
        public bool deleteExpenseEntry(int expense_ID, int expenseType)
        {
            var entry = _context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID).First();
            if (expenseType == GlobalSystemValues.TYPE_NC)
            {
                var entryDtlNC = _context.ExpenseEntryNonCash.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
                foreach (var nc in entryDtlNC)
                {
                    var entryDtlNCDtl = _context.ExpenseEntryNonCashDetails.Where(x => x.ExpenseEntryNCModel.ExpNC_ID == nc.ExpNC_ID).ToList();
                    foreach (var dtl in entryDtlNCDtl)
                    {
                        _context.ExpenseEntryNonCashDetailAccounts.RemoveRange(_context.ExpenseEntryNonCashDetailAccounts
                            .Where(x => x.ExpenseEntryNCDtlModel.ExpNCDtl_ID == dtl.ExpNCDtl_ID));

                    }
                    _context.ExpenseEntryNonCashDetails.RemoveRange(_context.ExpenseEntryNonCashDetails
                        .Where(x => x.ExpenseEntryNCModel.ExpNC_ID == nc.ExpNC_ID));
                }
                _context.ExpenseEntryNonCash.RemoveRange(_context.ExpenseEntryNonCash
                    .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            }
            else if (expenseType == GlobalSystemValues.TYPE_DDV)
            {
                var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
                foreach (var i in entryDtl)
                {
                    var interList = _context.ExpenseEntryInterEntity.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID).ToList();
                    var gbaseList = _context.ExpenseEntryGbaseDtls.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID).ToList();
                    if (interList.Count > 0)
                    {
                        foreach (var inter in interList)
                        {
                            var partList = _context.ExpenseEntryInterEntityParticular.Where(x => x.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == inter.ExpDtl_DDVInter_ID).ToList();
                            foreach (var part in partList)
                            {
                                var accList = _context.ExpenseEntryInterEntityAccs.Where(x => x.ExpenseEntryInterEntityParticular.InterPart_ID == part.InterPart_ID).ToList();
                                foreach (var accs in accList)
                                {
                                    _context.ExpenseEntryInterEntityAccs.RemoveRange(_context.ExpenseEntryInterEntityAccs
                                        .Where(x => x.ExpenseEntryInterEntityParticular.InterPart_ID == part.InterPart_ID));
                                }
                                _context.ExpenseEntryInterEntityParticular.RemoveRange(_context.ExpenseEntryInterEntityParticular
                                    .Where(x => x.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == inter.ExpDtl_DDVInter_ID));
                            }
                            _context.ExpenseEntryInterEntity.RemoveRange(_context.ExpenseEntryInterEntity
                                .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                        }
                    }
                    if (gbaseList.Count > 0)
                    {
                        _context.ExpenseEntryGbaseDtls.RemoveRange(_context.ExpenseEntryGbaseDtls
                            .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                    }
                }
                _context.ExpenseEntryDetails.RemoveRange(_context.ExpenseEntryDetails
                    .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            }
            else
            {
                var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
                foreach (var i in entryDtl)
                {
                    _context.ExpenseEntryAmortizations.RemoveRange(_context.ExpenseEntryAmortizations
                        .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                    _context.ExpenseEntryCashBreakdown.RemoveRange(_context.ExpenseEntryCashBreakdown
                        .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                    _context.ExpenseEntryGbaseDtls.RemoveRange(_context.ExpenseEntryGbaseDtls
                        .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                }
                _context.ExpenseEntryDetails.RemoveRange(_context.ExpenseEntryDetails
                    .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            }


            _context.ExpenseEntry.RemoveRange(_context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID));

            _context.SaveChanges();

            return true;
        }
        public int GetCurrentEntryStatus(int expense_ID)
        {
            return _context.ExpenseEntry.Where(db => db.Expense_ID == expense_ID).SingleOrDefault().Expense_Status;
        }

        public int addExpense_DDV(EntryDDVViewModelList entryModel, int userId, int expenseType)
        {
            decimal TotalDebit = 0;
            decimal credEwtTotal = 0;
            decimal credCashTotal = 0;
            int entryID = entryModel.entryID;
            foreach (EntryDDVViewModel cv in entryModel.EntryDDV)
            {
                TotalDebit += cv.debitGross;
                credEwtTotal += cv.credEwt;
                credCashTotal += cv.credCash;
            }

            if (_modelState.IsValid)
            {
                List<ExpenseEntryDetailModel> expenseDtls = new List<ExpenseEntryDetailModel>();

                foreach (EntryDDVViewModel ddv in entryModel.EntryDDV)
                {
                    List<ExpenseEntryInterEntityModel> expenseInter = new List<ExpenseEntryInterEntityModel>();
                    List<ExpenseEntryGbaseDtl> expenseGbase = new List<ExpenseEntryGbaseDtl>();

                    ExpenseEntryInterEntityModel interDetail = new ExpenseEntryInterEntityModel();
                    if (ddv.interDetails.Inter_Currency1_ID > 0)
                    {
                        var inter = ddv.interDetails;

                        interDetail = new ExpenseEntryInterEntityModel
                        {
                            ExpDtl_DDVInter_Check1 = inter.Inter_Check1,
                            ExpDtl_DDVInter_Check2 = inter.Inter_Check2,
                            ExpDtl_DDVInter_Conv_Amount1 = inter.Inter_Convert1_Amount,
                            ExpDtl_DDVInter_Conv_Amount2 = inter.Inter_Convert2_Amount,
                            ExpDtl_DDVInter_Curr1_ID = inter.Inter_Currency1_ID,
                            ExpDtl_DDVInter_Amount1 = inter.Inter_Currency1_Amount,
                            ExpDtl_DDVInter_Curr2_ID = inter.Inter_Currency2_ID,
                            ExpDtl_DDVInter_Amount2 = inter.Inter_Currency2_Amount,
                            ExpDtl_DDVInter_Rate = (inter.Inter_Rate > 0) ? inter.Inter_Rate : 1,
                            ExpenseEntryInterEntityParticular = new List<ExpenseEntryInterEntityParticularModel>()

                        };

                        foreach (ExpenseEntryInterEntityParticularViewModel interPart in inter.interPartList)
                        {
                            var accName = _context.DMAccount.Where(x => x.Account_ID == ddv.account).Select(x => x.Account_Name).FirstOrDefault();
                            ExpenseEntryInterEntityParticularModel interParticular = new ExpenseEntryInterEntityParticularModel
                            {
                                InterPart_ID = interPart.InterPart_ID,
                                InterPart_Particular_Title = interPart.InterPart_Particular_Title
                            };
                            List<ExpenseEntryInterEntityAccsModel> interAccsList = new List<ExpenseEntryInterEntityAccsModel>();
                            foreach (ExpenseEntryInterEntityAccsViewModel interAcc in interPart.ExpenseEntryInterEntityAccs)
                            {
                                ExpenseEntryInterEntityAccsModel interDetailAcc = new ExpenseEntryInterEntityAccsModel()
                                {
                                    InterAcc_Acc_ID = interAcc.Inter_Acc_ID,
                                    InterAcc_Amount = interAcc.Inter_Amount,
                                    InterAcc_Curr_ID = interAcc.Inter_Curr_ID,
                                    InterAcc_Rate = interAcc.Inter_Rate,
                                    InterAcc_Type_ID = interAcc.Inter_Type_ID
                                };
                                interAccsList.Add(interDetailAcc);
                            }
                            interParticular.ExpenseEntryInterEntityAccs = interAccsList;
                            interDetail.ExpenseEntryInterEntityParticular.Add(interParticular);
                        }

                        expenseInter.Add(interDetail);
                    }
                    if (ddv.gBaseRemarksDetails.Count > 0)
                    {
                        foreach (var gbaseRemark in ddv.gBaseRemarksDetails)
                        {
                            ExpenseEntryGbaseDtl remarks = new ExpenseEntryGbaseDtl
                            {
                                GbaseDtl_Document_Type = gbaseRemark.docType,
                                GbaseDtl_InvoiceNo = gbaseRemark.invNo,
                                GbaseDtl_Description = gbaseRemark.desc,
                                GbaseDtl_Amount = gbaseRemark.amount
                            };

                            expenseGbase.Add(remarks);
                        }
                    }
                    int creditAccMasterID1 = creditAccMasterID1 = int.Parse(xelemAcc.Element("C_CV1").Value);
                    int creditAccMasterID2 = creditAccMasterID2 = int.Parse(xelemAcc.Element("C_CV2").Value);

                    ExpenseEntryDetailModel expenseDetails = new ExpenseEntryDetailModel
                    {
                        ExpDtl_Gbase_Remarks = ddv.GBaseRemarks,
                        ExpDtl_Account = ddv.account,
                        ExpDtl_Inter_Entity = ddv.inter_entity,
                        ExpDtl_Fbt = ddv.fbt,
                        ExpDtl_FbtID = (ddv.fbt) ? getFbt(getAccount(ddv.account).Account_FBT_MasterID) : 0,
                        ExpDtl_Dept = ddv.dept,
                        ExpDtl_Vat = ddv.vat,
                        ExpDtl_Ewt = ddv.ewt,
                        ExpDtl_Ccy = ddv.ccy,
                        ExpDtl_Debit = ddv.debitGross,
                        ExpDtl_Credit_Ewt = ddv.credEwt,
                        ExpDtl_Credit_Cash = ddv.credCash,
                        ExpDtl_CreditAccount1 = (ddv.credEwt > 0) ? getAccountByMasterID(creditAccMasterID1).Account_ID : 0,
                        ExpDtl_CreditAccount2 = getAccountByMasterID(creditAccMasterID2).Account_ID,
                        ExpDtl_Ewt_Payor_Name_ID = ddv.ewt_Payor_Name_ID,
                        ExpenseEntryInterEntity = expenseInter,
                        ExpDtl_isEwt = (ddv.ewt <= 0) ? false : true,
                        ExpenseEntryGbaseDtls = expenseGbase
                    };
                    expenseDtls.Add(expenseDetails);
                }

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Type = expenseType,
                    Expense_Date = entryModel.expenseDate,
                    Expense_Payee = entryModel.vendor,
                    Expense_Payee_Type = entryModel.payee_type,
                    Expense_Debit_Total = TotalDebit,
                    Expense_Credit_Total = credEwtTotal + credCashTotal,
                    Expense_Creator_ID = userId,
                    Expense_Created_Date = entryModel.expenseDate,
                    Expense_Number = (String.IsNullOrEmpty(entryModel.expenseId)) ? 0 : int.Parse(entryModel.expenseId),
                    Expense_Last_Updated = DateTime.Now,
                    Expense_isDeleted = false,
                    Expense_Status = 1,
                    ExpenseEntryDetails = expenseDtls
                };

                if (entryModel.entryID == 0)
                {
                    _context.ExpenseEntry.Add(expenseEntry);
                    //----------------------------- NOTIF----------------------------------
                    insertIntoNotif(userId, GlobalSystemValues.TYPE_DDV, GlobalSystemValues.STATUS_NEW, 0);
                    //----------------------------- NOTIF----------------------------------
                }
                //else
                //{
                //    List<int> EditableStatus = new List<int>{
                //    GlobalSystemValues.STATUS_PENDING,
                //    GlobalSystemValues.STATUS_REJECTED
                //    };
                //    int currentStat = getSingleEntryRecord(entryID).Expense_Status;
                //    if (EditableStatus.Contains(currentStat))
                //    {
                //        // Update entity in DbSet
                //        expenseEntry.Expense_ID = entryModel.entryID;
                //        removeDDVChild(entryModel.entryID);
                //        _context.ExpenseEntry.Update(expenseEntry);
                //        //----------------------------- NOTIF----------------------------------
                //        insertIntoNotif(userId, GlobalSystemValues.TYPE_DDV, GlobalSystemValues.STATUS_EDIT, 0);
                //        //----------------------------- NOTIF----------------------------------
                //    }
                //    else
                //    {
                //        expenseEntry.Expense_ID = entryID;
                //        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE1;
                //    }
                //}
                else
                {
                    if (deleteExpenseEntry(entryModel.entryID, GlobalSystemValues.TYPE_DDV))
                    {
                        _context.ExpenseEntry.Add(expenseEntry);
                        //----------------------------- NOTIF----------------------------------
                        insertIntoNotif(userId, GlobalSystemValues.TYPE_DDV, GlobalSystemValues.STATUS_EDIT, 0);
                        //----------------------------- NOTIF----------------------------------
                    }
                }
                _context.SaveChanges();
                return expenseEntry.Expense_ID;
            }

            return -1;
        }
        public int addExpense_NC(EntryNCViewModelList entryModel, int userId, int expenseType)
        {
            int entryID = entryModel.entryID;

            if (_modelState.IsValid)
            {
                List<ExpenseEntryNCDtlModel> expenseDtls = new List<ExpenseEntryNCDtlModel>();
                foreach (var ncDtls in entryModel.EntryNC.ExpenseEntryNCDtls)
                {
                    decimal dbAmt = 0;
                    decimal cdAmt = 0;
                    //checks if debit and credit have values
                    foreach (var accs in ncDtls.ExpenseEntryNCDtlAccs)
                    {
                        if (accs.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                        {
                            dbAmt += accs.ExpNCDtlAcc_Amount;
                        }
                        else if (accs.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_CREDIT ||
                               accs.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_EWT)
                        {
                            cdAmt += accs.ExpNCDtlAcc_Amount;
                        }
                    }
                    //Only if Debit and Credit is Not Equal to 0
                    if ((dbAmt != 0) && (cdAmt != 0))
                    {
                        List<ExpenseEntryNCDtlAccModel> accountDtls = new List<ExpenseEntryNCDtlAccModel>();
                        foreach (var accDtls in ncDtls.ExpenseEntryNCDtlAccs)
                        {
                            if (accDtls.ExpNCDtlAcc_Amount > 0)
                            {
                                ExpenseEntryNCDtlAccModel acc = new ExpenseEntryNCDtlAccModel
                                {
                                    ExpNCDtlAcc_Acc_ID = accDtls.ExpNCDtlAcc_Acc_ID,
                                    ExpNCDtlAcc_Acc_Name = _context.DMAccount.Where(x => x.Account_ID == accDtls.ExpNCDtlAcc_Acc_ID).Select(x => x.Account_Name).FirstOrDefault(),
                                    ExpNCDtlAcc_Curr_ID = accDtls.ExpNCDtlAcc_Curr_ID,
                                    ExpNCDtlAcc_Amount = accDtls.ExpNCDtlAcc_Amount,
                                    ExpNCDtlAcc_Inter_Rate = accDtls.ExpNCDtlAcc_Inter_Rate,
                                    ExpNCDtlAcc_Type_ID = accDtls.ExpNCDtlAcc_Type_ID
                                };
                                accountDtls.Add(acc);
                            }
                        }
                        ExpenseEntryNCDtlModel expenseDetail = new ExpenseEntryNCDtlModel
                        {
                            ExpNCDtl_Remarks_Desc = ncDtls.ExpNCDtl_Remarks_Desc,
                            ExpNCDtl_Remarks_Period = ncDtls.ExpNCDtl_Remarks_Period,
                            ExpNCDtl_Vendor_ID = ncDtls.ExpNCDtl_Vendor_ID,
                            ExpNCDtl_TR_ID = ncDtls.ExpNCDtl_TR_ID,
                            ExpNCDtl_TaxBasedAmt = ncDtls.ExpNCDtl_TaxBasedAmt,
                            ExpenseEntryNCDtlAccs = accountDtls
                        };
                        expenseDtls.Add(expenseDetail);
                    }
                }

                List<ExpenseEntryNCModel> expenseNCList = new List<ExpenseEntryNCModel>
                {
                    new ExpenseEntryNCModel
                    {
                        ExpNC_Category_ID = entryModel.EntryNC.NC_Category_ID,
                        ExpNC_DebitAmt = entryModel.EntryNC.NC_DebitAmt,
                        ExpNC_CredAmt = entryModel.EntryNC.NC_CredAmt,
                        ExpNC_CS_DebitAmt = (entryModel.EntryNC.NC_CS_DebitAmt > 0) ? entryModel.EntryNC.NC_CS_DebitAmt : (entryModel.EntryNC.ExpenseEntryNCDtls_CDD.Count >= 1 ) ? entryModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount : 0,
                        ExpNC_CS_CredAmt = (entryModel.EntryNC.NC_CS_CredAmt > 0) ? entryModel.EntryNC.NC_CS_CredAmt :  (entryModel.EntryNC.ExpenseEntryNCDtls_CDD.Count >= 1 ) ? entryModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount : 0,
                        ExpNC_CS_Period = entryModel.EntryNC.ExpenseEntryNCDtls_CDD.Count > 0 ? entryModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpNCDtl_Remarks_Period : "",
                        ExpNC_IE_DebitAmt = entryModel.EntryNC.NC_IE_DebitAmt,
                        ExpNC_IE_CredAmt = entryModel.EntryNC.NC_IE_CredAmt,
                        ExpenseEntryNCDtls = expenseDtls
                    }
                };

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Type = expenseType,
                    Expense_Date = DateTime.Now.Date,
                    Expense_Debit_Total = entryModel.EntryNC.NC_DebitAmt,
                    Expense_Credit_Total = entryModel.EntryNC.NC_CredAmt,
                    Expense_Creator_ID = userId,
                    Expense_Created_Date = DateTime.Now,
                    Expense_Last_Updated = DateTime.Now,
                    Expense_Number = entryModel.voucherNumber,
                    Expense_isDeleted = false,
                    Expense_Status = 1,
                    ExpenseEntryNC = expenseNCList
                };

                int currentStat = (entryModel.entryID > 0) ? getSingleEntryRecord(entryID).Expense_Status : 0;

                if (currentStat == GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR)
                {
                    entryModel.entryID = 0;
                    expenseEntry.Expense_CheckId = 0;
                    expenseEntry.Expense_CheckNo = null;
                    expenseEntry.Expense_Number = 0;
                }

                if (entryModel.entryID == 0)
                {
                    _context.ExpenseEntry.Add(expenseEntry);
                    //----------------------------- NOTIF----------------------------------
                    insertIntoNotif(userId, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_NEW, 0);
                    //----------------------------- NOTIF----------------------------------
                    _context.SaveChanges();
                }
                else
                {
                    List<int> EditableStatus = new List<int>{
                    GlobalSystemValues.STATUS_PENDING,
                    GlobalSystemValues.STATUS_REJECTED
                    };
                    if (EditableStatus.Contains(currentStat))
                    {
                        // Update entity in DbSet
                        expenseEntry.Expense_ID = entryModel.entryID;
                        removeNCChild(entryModel.entryID);
                        _context.ExpenseEntry.Update(expenseEntry);
                        //----------------------------- NOTIF----------------------------------
                        insertIntoNotif(userId, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_EDIT, 0);
                        //----------------------------- NOTIF----------------------------------
                        _context.SaveChanges();
                    }
                    else
                    {
                        expenseEntry.Expense_ID = entryID;
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE1;
                    }
                }
                return expenseEntry.Expense_ID;
            }
            return -1;
        }
        public bool removeDDVChild(int expense_ID)
        {
            var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
            foreach (var i in entryDtl)
            {
                var interList = _context.ExpenseEntryInterEntity.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID).ToList();
                if (interList.Count > 0)
                {
                    foreach (var inter in interList)
                    {
                        var partList = _context.ExpenseEntryInterEntityParticular.Where(x => x.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == inter.ExpDtl_DDVInter_ID).ToList();
                        foreach (var part in partList)
                        {
                            var accList = _context.ExpenseEntryInterEntityAccs.Where(x => x.ExpenseEntryInterEntityParticular.InterPart_ID == part.InterPart_ID).ToList();
                            foreach (var accs in accList)
                            {
                                _context.ExpenseEntryInterEntityAccs.RemoveRange(_context.ExpenseEntryInterEntityAccs
                                    .Where(x => x.ExpenseEntryInterEntityParticular.InterPart_ID == part.InterPart_ID));
                            }
                            _context.ExpenseEntryInterEntityParticular.RemoveRange(_context.ExpenseEntryInterEntityParticular
                                .Where(x => x.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == inter.ExpDtl_DDVInter_ID));
                        }
                        _context.ExpenseEntryInterEntity.RemoveRange(_context.ExpenseEntryInterEntity
                            .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                    }
                }
                var gbaseList = _context.ExpenseEntryGbaseDtls.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID).ToList();
                if (gbaseList.Count > 0)
                {
                    _context.ExpenseEntryGbaseDtls.RemoveRange(_context.ExpenseEntryGbaseDtls
                        .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                }

            }
            _context.ExpenseEntryDetails.RemoveRange(_context.ExpenseEntryDetails
                .Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID));
            return true;
        }
        public bool removeNCChild(int expense_ID)
        {
            var entryDtlNC = _context.ExpenseEntryNonCash.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
            foreach (var nc in entryDtlNC)
            {
                var entryDtlNCDtl = _context.ExpenseEntryNonCashDetails.Where(x => x.ExpenseEntryNCModel.ExpNC_ID == nc.ExpNC_ID).ToList();
                foreach (var dtl in entryDtlNCDtl)
                {
                    _context.ExpenseEntryNonCashDetailAccounts.RemoveRange(_context.ExpenseEntryNonCashDetailAccounts
                        .Where(x => x.ExpenseEntryNCDtlModel.ExpNCDtl_ID == dtl.ExpNCDtl_ID));

                }
                _context.ExpenseEntryNonCashDetails.RemoveRange(_context.ExpenseEntryNonCashDetails
                    .Where(x => x.ExpenseEntryNCModel.ExpNC_ID == nc.ExpNC_ID));
            }
            _context.ExpenseEntryNonCash.RemoveRange(_context.ExpenseEntryNonCash
                .Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID));
            return true;
        }
        //Liquidation
        public List<LiquidationMainListViewModel> populateLiquidationList(int userID)
        {
            List<LiquidationMainListViewModel> postedEntryList = new List<LiquidationMainListViewModel>();

            var dbPostedEntry = from p in _context.ExpenseEntry
                                where (p.Expense_Status == GlobalSystemValues.STATUS_FOR_CLOSING
                                || p.Expense_Status == GlobalSystemValues.STATUS_POSTED)
                                && p.Expense_Type == GlobalSystemValues.TYPE_SS
                                && p.Expense_Last_Updated.Date < DateTime.Now.Date
                                orderby p.Expense_Last_Updated
                                select new
                                {
                                    p.Expense_ID,
                                    p.Expense_Type,
                                    p.Expense_Debit_Total,
                                    p.Expense_Payee,
                                    p.Expense_Payee_Type,
                                    p.Expense_Creator_ID,
                                    p.Expense_Approver,
                                    p.Expense_Last_Updated,
                                    p.Expense_Date,
                                    p.Expense_Created_Date
                                };

            foreach (var item in dbPostedEntry)
            {
                if (_context.LiquidationEntryDetails.Where(
                    x => x.ExpenseEntryModel.Expense_ID == item.Expense_ID).Count() != 0)
                    continue;

                var linktionary = new Dictionary<int, string>
                {
                    {0,"Data Maintenance" },
                    {GlobalSystemValues.TYPE_CV,""},
                    {GlobalSystemValues.TYPE_DDV,""},
                    {GlobalSystemValues.TYPE_NC,""},
                    {GlobalSystemValues.TYPE_PC,""},
                    {GlobalSystemValues.TYPE_SS,"Liquidation_SS"},
                };

                postedEntryList.Add(new LiquidationMainListViewModel
                {
                    App_ID = item.Expense_ID,
                    App_Type = GlobalSystemValues.getApplicationType(item.Expense_Type),
                    App_Amount = item.Expense_Debit_Total,
                    App_Payee = getVendorName(item.Expense_Payee, item.Expense_Payee_Type),
                    App_Maker = getUserName(item.Expense_Creator_ID),
                    App_Approver = getUserName(item.Expense_Approver),
                    App_Date = item.Expense_Created_Date,
                    App_Last_Updated = item.Expense_Last_Updated,
                    App_Link = linktionary[item.Expense_Type]
                });
            }

            return new PaginatedList<LiquidationMainListViewModel>(postedEntryList, postedEntryList.Count, 1, 10);
        }

        //retrieve expense details to Liqudate
        public LiquidationViewModel getExpenseToLiqudate(int transID)
        {
            List<LiquidationDetailsViewModel> liqList = new List<LiquidationDetailsViewModel>();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryDetails = from d
                                                          in _context.ExpenseEntryDetails
                                                          where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                          select new
                                                          {
                                                              d,
                                                              ExpenseEntryGbaseDtls = from g
                                                                                      in _context.ExpenseEntryGbaseDtls
                                                                                      where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                      select g,
                                                              LiquidationCashBreakdown = from l
                                                                                      in _context.LiquidationCashBreakdown
                                                                                         where l.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                         select l,
                                                              LiquidationInterEntity = from i
                                                                                      in _context.LiquidationInterEntity
                                                                                       where i.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                       select i,
                                                              ExpenseEntryCashBreakdown = (from c
                                                                                               in _context.ExpenseEntryCashBreakdown
                                                                                           where c.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                           select c).OrderByDescending(db => db.ExpenseEntryDetailModel.ExpDtl_ID).OrderByDescending(db => db.CashBreak_Denomination)
                                                          }
                                }).FirstOrDefault();

            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                List<LiquidationCashBreakdown> cashBreakdown = new List<LiquidationCashBreakdown>();
                List<LiquidationCashBreakdown> liqCashBreakdown = new List<LiquidationCashBreakdown>();
                List<LiquidationInterEntity> liqInterEntity = new List<LiquidationInterEntity>();

                foreach (var gbase in dtl.ExpenseEntryGbaseDtls)
                {
                    EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel()
                    {
                        amount = gbase.GbaseDtl_Amount,
                        desc = gbase.GbaseDtl_Description,
                        docType = gbase.GbaseDtl_Document_Type,
                        invNo = gbase.GbaseDtl_InvoiceNo
                    };

                    remarksDtl.Add(gbaseTemp);
                }

                foreach (var cashbd in dtl.ExpenseEntryCashBreakdown)
                {
                    LiquidationCashBreakdown cashbdTemp = new LiquidationCashBreakdown()
                    {
                        cashDenomination = cashbd.CashBreak_Denomination,
                        cashNoPC = cashbd.CashBreak_NoPcs,
                        cashAmount = cashbd.CashBreak_Amount
                    };

                    cashBreakdown.Add(cashbdTemp);
                }

                foreach (var liqCashbd in dtl.LiquidationCashBreakdown)
                {
                    LiquidationCashBreakdown liqCashbdTemp = new LiquidationCashBreakdown()
                    {
                        cashDenomination = liqCashbd.LiqCashBreak_Denomination,
                        cashNoPC = liqCashbd.LiqCashBreak_NoPcs,
                        cashAmount = liqCashbd.LiqCashBreak_Amount
                    };

                    liqCashBreakdown.Add(liqCashbdTemp);
                }

                foreach (var liqIE in dtl.LiquidationInterEntity)
                {
                    LiquidationInterEntity liqIETemp = new LiquidationInterEntity()
                    {
                        Liq_InterEntityID = liqIE.id,
                        Liq_AccountID_1_1 = liqIE.Liq_AccountID_1_1,
                        Liq_AccountID_1_2 = liqIE.Liq_AccountID_1_2,
                        Liq_AccountID_2_1 = liqIE.Liq_AccountID_2_1,
                        Liq_AccountID_2_2 = liqIE.Liq_AccountID_2_2,
                        Liq_AccountID_3_1 = liqIE.Liq_AccountID_3_1,
                        Liq_AccountID_3_2 = liqIE.Liq_AccountID_3_2,
                        Liq_Amount_1_1 = liqIE.Liq_Amount_1_1,
                        Liq_Amount_1_2 = liqIE.Liq_Amount_1_2,
                        Liq_Amount_2_1 = liqIE.Liq_Amount_2_1,
                        Liq_Amount_2_2 = liqIE.Liq_Amount_2_2,
                        Liq_Amount_3_1 = liqIE.Liq_Amount_3_1,
                        Liq_Amount_3_2 = liqIE.Liq_Amount_3_2,
                        Liq_CCY_1_1 = liqIE.Liq_CCY_1_1,
                        Liq_CCY_1_2 = liqIE.Liq_CCY_1_2,
                        Liq_CCY_2_1 = liqIE.Liq_CCY_2_1,
                        Liq_CCY_2_2 = liqIE.Liq_CCY_2_2,
                        Liq_CCY_3_1 = liqIE.Liq_CCY_3_1,
                        Liq_CCY_3_2 = liqIE.Liq_CCY_3_2,
                        Liq_DebitCred_1_1 = liqIE.Liq_DebitCred_1_1,
                        Liq_DebitCred_1_2 = liqIE.Liq_DebitCred_1_2,
                        Liq_DebitCred_2_1 = liqIE.Liq_DebitCred_2_1,
                        Liq_DebitCred_2_2 = liqIE.Liq_DebitCred_2_2,
                        Liq_DebitCred_3_1 = liqIE.Liq_DebitCred_3_1,
                        Liq_DebitCred_3_2 = liqIE.Liq_DebitCred_3_2,
                        Liq_InterRate_1_1 = liqIE.Liq_InterRate_1_1,
                        Liq_InterRate_1_2 = liqIE.Liq_InterRate_1_2,
                        Liq_InterRate_2_1 = liqIE.Liq_InterRate_2_1,
                        Liq_InterRate_2_2 = liqIE.Liq_InterRate_2_2,
                        Liq_InterRate_3_1 = liqIE.Liq_InterRate_3_1,
                        Liq_InterRate_3_2 = liqIE.Liq_InterRate_3_2,
                        Liq_Tax_Rate = liqIE.Liq_TaxRate,
                        Liq_VendorID = liqIE.Liq_VendorID
                    };

                    liqInterEntity.Add(liqIETemp);
                }

                var accountInfo = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account).Single();
                int liqFlag = 0;
                if (liqCashBreakdown.Count != 0)
                {
                    liqFlag = 1;
                }
                if (liqInterEntity.Count != 0 && liqCashBreakdown.Count == 0)
                {
                    liqFlag = 2;
                }
                LiquidationDetailsViewModel liqDtl = new LiquidationDetailsViewModel()
                {
                    EntryDetailsID = dtl.d.ExpDtl_ID,
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    accountID = dtl.d.ExpDtl_Account,
                    accountName = accountInfo.Account_Name,
                    accountNumber = accountInfo.Account_No,
                    accountCode = accountInfo.Account_Code,
                    fbt = dtl.d.ExpDtl_Fbt,
                    deptID = dtl.d.ExpDtl_Dept,
                    deptName = GetDeptName(dtl.d.ExpDtl_Dept),
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vatID = dtl.d.ExpDtl_Vat,
                    vatValue = (dtl.d.ExpDtl_Vat <= 0) ? 0 : (float)Mizuho.round(getVat(dtl.d.ExpDtl_Vat) * 100, 2),
                    chkEwt = dtl.d.ExpDtl_isEwt,
                    ewtID = dtl.d.ExpDtl_Ewt,
                    ewtValue = (dtl.d.ExpDtl_Ewt <= 0) ? 0 : GetEWTValue(dtl.d.ExpDtl_Ewt) * 100,
                    ccyID = dtl.d.ExpDtl_Ccy,
                    ccyMasterID = getCurrency(dtl.d.ExpDtl_Ccy).Curr_MasterID,
                    ccyAbbrev = GetCurrencyAbbrv(dtl.d.ExpDtl_Ccy),
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    dtlSSPayee = dtl.d.ExpDtl_SS_Payee,
                    dtl_Ewt_Payor_Name_ID = dtl.d.ExpDtl_Ewt_Payor_Name_ID,
                    dtlSSPayeeName = getVendorName(dtl.d.ExpDtl_SS_Payee, GlobalSystemValues.PAYEETYPE_REGEMP),
                    //vendTRList = (dtl.d.ExpDtl_Ewt_Payor_Name_ID > 0) ? getVendorTaxList(getVendor(dtl.d.ExpDtl_Ewt_Payor_Name_ID).Vendor_MasterID) : new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } },
                    gBaseRemarksDetails = remarksDtl,
                    cashBreakdown = cashBreakdown,
                    liqCashBreakdown = liqCashBreakdown,
                    liqInterEntity = liqInterEntity,
                    modalInputFlag = (cashBreakdown == null || cashBreakdown.Count == 0) ? 0 : 1,
                    liqInputFlag = liqFlag
                };
                liqList.Add(liqDtl);
            }

            var liqStatus = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == transID).FirstOrDefault();

            LiquidationViewModel liqModel = new LiquidationViewModel()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                vendor = EntryDetails.e.Expense_Payee,
                expenseYear = EntryDetails.e.Expense_Date.Year.ToString(),
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5, '0'),
                checkNo = EntryDetails.e.Expense_CheckNo,
                statusID = (liqStatus == null) ? 0 : liqStatus.Liq_Status,
                status = (liqStatus == null) ? "" : getStatus(liqStatus.Liq_Status),
                maker = (liqStatus == null) ? EntryDetails.e.Expense_Creator_ID : liqStatus.Liq_Created_UserID,
                verifier_1 = (liqStatus == null) ? "" : (liqStatus.Liq_Verifier1 > 0) ? getUserName(liqStatus.Liq_Verifier1) : "",
                verifier_2 = (liqStatus == null) ? "" : (liqStatus.Liq_Verifier2 > 0) ? getUserName(liqStatus.Liq_Verifier2) : "",
                approver = (liqStatus == null) ? "" : (liqStatus.Liq_Approver > 0) ? getUserName(liqStatus.Liq_Approver) : "",
                approver_id = (liqStatus == null) ? 0 : liqStatus.Liq_Approver,
                verifier_1_id = (liqStatus == null) ? 0 : liqStatus.Liq_Verifier1,
                verifier_2_id = (liqStatus == null) ? 0 : liqStatus.Liq_Verifier2,
                createdDate = EntryDetails.e.Expense_Created_Date,
                LiquidationDetails = liqList,
                LiqEntryDetails = (liqStatus == null) ? new LiquidationEntryDetailModel() : liqStatus
            };

            return liqModel;
        }

        //Add liquidation details
        public int addLiquidationDetail(LiquidationViewModel vm, int userid, int count)
        {
            LiquidationCashBreakdownModel model = new LiquidationCashBreakdownModel();
            foreach (var i in vm.LiquidationDetails)
            {
                ExpenseEntryDetailModel dtlModel = _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.EntryDetailsID).FirstOrDefault();

                if (getCurrency(i.ccyID).Curr_MasterID == int.Parse(xelemLiq.Element("CURRENCY_PHP").Value))
                {
                    foreach (var j in i.liqCashBreakdown)
                    {
                        _context.LiquidationCashBreakdown.Add(new LiquidationCashBreakdownModel
                        {
                            ExpenseEntryDetailModel = dtlModel,
                            LiqCashBreak_Denomination = j.cashDenomination,
                            LiqCashBreak_NoPcs = j.cashNoPC,
                            LiqCashBreak_Amount = j.cashAmount
                        });
                        _context.SaveChanges();
                    }

                    _context.LiquidationInterEntity.Add(new LiquidationInterEntityModel
                    {
                        ExpenseEntryDetailModel = dtlModel,
                        Liq_DebitCred_1_1 = i.liqInterEntity[0].Liq_DebitCred_1_1,
                        Liq_AccountID_1_1 = i.liqInterEntity[0].Liq_AccountID_1_1,
                        Liq_Amount_1_1 = i.liqInterEntity[0].Liq_Amount_1_1,
                        Liq_DebitCred_1_2 = i.liqInterEntity[0].Liq_DebitCred_1_2,
                        Liq_AccountID_1_2 = i.liqInterEntity[0].Liq_AccountID_1_2,
                        Liq_Amount_1_2 = i.liqInterEntity[0].Liq_Amount_1_2,
                        Liq_DebitCred_2_1 = i.liqInterEntity[0].Liq_DebitCred_2_1,
                        Liq_AccountID_2_1 = i.liqInterEntity[0].Liq_AccountID_2_1,
                        Liq_Amount_2_1 = i.liqInterEntity[0].Liq_Amount_2_1,
                        Liq_DebitCred_2_2 = i.liqInterEntity[0].Liq_DebitCred_2_2,
                        Liq_AccountID_2_2 = i.liqInterEntity[0].Liq_AccountID_2_2,
                        Liq_Amount_2_2 = i.liqInterEntity[0].Liq_Amount_2_2,
                        Liq_DebitCred_3_1 = i.liqInterEntity[0].Liq_DebitCred_3_1,
                        Liq_AccountID_3_1 = i.liqInterEntity[0].Liq_AccountID_3_1,
                        Liq_Amount_3_1 = i.liqInterEntity[0].Liq_Amount_3_1,
                        Liq_DebitCred_3_2 = i.liqInterEntity[0].Liq_DebitCred_3_2,
                        Liq_AccountID_3_2 = i.liqInterEntity[0].Liq_AccountID_3_2,
                        Liq_Amount_3_2 = i.liqInterEntity[0].Liq_Amount_3_2,
                        Liq_TaxRate = i.liqInterEntity[0].Liq_Tax_Rate,
                        Liq_VendorID = i.liqInterEntity[0].Liq_VendorID
                    });
                    _context.SaveChanges();
                }
                else
                {
                    foreach (var j in i.liqInterEntity)
                    {
                        _context.LiquidationInterEntity.Add(new LiquidationInterEntityModel
                        {
                            ExpenseEntryDetailModel = dtlModel,
                            Liq_DebitCred_1_1 = j.Liq_DebitCred_1_1,
                            Liq_AccountID_1_1 = j.Liq_AccountID_1_1,
                            Liq_InterRate_1_1 = j.Liq_InterRate_1_1,
                            Liq_CCY_1_1 = j.Liq_CCY_1_1,
                            Liq_Amount_1_1 = j.Liq_Amount_1_1,
                            Liq_DebitCred_1_2 = j.Liq_DebitCred_1_2,
                            Liq_AccountID_1_2 = j.Liq_AccountID_1_2,
                            Liq_InterRate_1_2 = j.Liq_InterRate_1_2,
                            Liq_CCY_1_2 = j.Liq_CCY_1_2,
                            Liq_Amount_1_2 = j.Liq_Amount_1_2,
                            Liq_DebitCred_2_1 = j.Liq_DebitCred_2_1,
                            Liq_AccountID_2_1 = j.Liq_AccountID_2_1,
                            Liq_InterRate_2_1 = j.Liq_InterRate_2_1,
                            Liq_CCY_2_1 = j.Liq_CCY_2_1,
                            Liq_Amount_2_1 = j.Liq_Amount_2_1,
                            Liq_DebitCred_2_2 = j.Liq_DebitCred_2_2,
                            Liq_AccountID_2_2 = j.Liq_AccountID_2_2,
                            Liq_InterRate_2_2 = j.Liq_InterRate_2_2,
                            Liq_CCY_2_2 = j.Liq_CCY_2_2,
                            Liq_Amount_2_2 = j.Liq_Amount_2_2,
                            Liq_TaxRate = j.Liq_Tax_Rate,
                            Liq_VendorID = j.Liq_VendorID
                        });
                        _context.SaveChanges();
                    }
                }

            }

            ExpenseEntryModel expenseModel = _context.ExpenseEntry.Where(x => x.Expense_ID == vm.entryID).FirstOrDefault();
            _context.LiquidationEntryDetails.Add(new LiquidationEntryDetailModel
            {
                ExpenseEntryModel = expenseModel,
                Liq_Status = GlobalSystemValues.STATUS_PENDING,
                Liq_Created_Date = (count == 0) ? DateTime.Now : vm.LiqEntryDetails.Liq_Created_Date,
                Liq_LastUpdated_Date = DateTime.Now,
                Liq_Created_UserID = userid
            });
            _context.SaveChanges();

            return vm.entryID;
        }

        //Update liquidation status
        public bool updateLiquidateStatus(int id, int status, int userid)
        {
            if (_modelState.IsValid)
            {
                List<int> forbiddenStatus = new List<int>{
                    GlobalSystemValues.STATUS_APPROVED,
                    GlobalSystemValues.STATUS_FOR_PRINTING,
                    GlobalSystemValues.STATUS_REJECTED,
                    GlobalSystemValues.STATUS_FOR_CLOSING
                };


                LiquidationEntryDetailModel dbLiquidation = _context.LiquidationEntryDetails.FirstOrDefault(x => x.ExpenseEntryModel.Expense_ID == id);

                if (dbLiquidation == null)
                {
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE3;
                    return false;
                }

                if (forbiddenStatus.Contains(dbLiquidation.Liq_Status))
                    return false;

                var liquidateEntry = _context.LiquidationEntryDetails.Include(x => x.ExpenseEntryModel)
                .Where(x => x.ExpenseEntryModel.Expense_ID == id).FirstOrDefault();

                if (status == GlobalSystemValues.STATUS_VERIFIED)
                {
                    if (liquidateEntry.Liq_Verifier1 == 0)
                    {
                        liquidateEntry.Liq_Verifier1 = userid;
                    }
                    else if (liquidateEntry.Liq_Verifier1 != userid)
                    {
                        if (liquidateEntry.Liq_Verifier2 == 0)
                        {
                            liquidateEntry.Liq_Verifier2 = userid;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (status == GlobalSystemValues.STATUS_APPROVED)
                {
                    liquidateEntry.Liq_Approver = userid;
                }

                liquidateEntry.Liq_Status = status;
                liquidateEntry.Liq_LastUpdated_Date = DateTime.Now;
                _context.SaveChanges();
            }
            else { return false; }
            return true;
        }
        //Update liquidation status for Reversal process
        public bool updateLiquidateReversalStatus(int id, int status, int userid)
        {
            LiquidationEntryDetailModel dbLiquidation = _context.LiquidationEntryDetails.FirstOrDefault(x => x.ExpenseEntryModel.Expense_ID == id);

            if (dbLiquidation == null)
            {
                GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE3;
                return false;
            }

            var liquidateEntry = _context.LiquidationEntryDetails.Include(x => x.ExpenseEntryModel)
            .Where(x => x.ExpenseEntryModel.Expense_ID == id).FirstOrDefault();

            liquidateEntry.Liq_Status = status;
            liquidateEntry.Liq_LastUpdated_Date = DateTime.Now;
            _context.SaveChanges();

            return true;
        }
        //Delete liquidation entry
        public bool deleteLiquidationEntry(int entryID)
        {
            var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).ToList();
            foreach (var i in entryDtl)
            {
                _context.LiquidationCashBreakdown.RemoveRange(_context.LiquidationCashBreakdown
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
            }

            foreach (var i in entryDtl)
            {
                _context.LiquidationInterEntity.RemoveRange(_context.LiquidationInterEntity
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
            }

            _context.LiquidationEntryDetails.RemoveRange(_context.LiquidationEntryDetails
                .Where(x => x.ExpenseEntryModel.Expense_ID == entryID));

            _context.SaveChanges();

            return true;
        }

        //Check liquidation record existence
        public int getLiquidationExistence(int entryID)
        {
            return _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).Count();
        }

        //Check current status of liqudation entry
        public int getCurrentLiquidationStatus(int entryID)
        {
            return _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).SingleOrDefault().Liq_Status;
        }

        ////============[End Access Entry Tables]=========================

        ///==============[Post Entries]==============
        //public bool postCV(int expID)
        //{
        //    var expenseDetails = getExpense(expID);

        //    var list = new[] {
        //        new { expEntryID = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
        //    }.ToList();
        //    TblCm10 goExpData = new TblCm10();
        //    GOExpressHistModel goExpHistData = new GOExpressHistModel();

        //    list.Clear();

        //    foreach (var item in expenseDetails.EntryCV)
        //    {
        //        gbaseContainer tempGbase = new gbaseContainer();

        //        tempGbase.valDate = expenseDetails.expenseDate;
        //        tempGbase.remarks = item.GBaseRemarks;
        //        tempGbase.maker = expenseDetails.maker;
        //        tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver;

        //        entryContainer debit = new entryContainer();
        //        entryContainer credit = new entryContainer();

        //        debit.type = "D";

        //        //Debit
        //        debit.ccy = item.ccy;
        //        debit.amount = item.debitGross;
        //        debit.vendor = expenseDetails.vendor;
        //        debit.account = item.account;
        //        debit.chkNo = expenseDetails.checkNo;
        //        debit.dept = item.dept;

        //        tempGbase.entries.Add(debit);

        //        //Credit 1 - tax withheld if only has tax amount and EWT Account 
        //        if (item.credEwt > 0 && item.creditAccount1 > 0)
        //        {
        //            credit.type = "C";
        //            credit.ccy = item.ccy;
        //            credit.amount = item.credEwt;
        //            credit.vendor = expenseDetails.vendor;
        //            credit.account = item.creditAccount1;
        //            credit.dept = item.dept;

        //            tempGbase.entries.Add(credit);
        //        }

        //        //Credit 2 - Credit amount
        //        credit = new entryContainer();
        //        credit.type = "C";
        //        credit.ccy = item.ccy;
        //        credit.amount = item.credCash;
        //        credit.vendor = expenseDetails.vendor;
        //        credit.account = item.creditAccount2;
        //        credit.dept = item.dept;

        //        tempGbase.entries.Add(credit);

        //        goExpData = InsertGbaseEntry(tempGbase, expID);
        //        goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.expenseDtlID);
        //        list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });

        //        if (item.fbt)
        //        {
        //            tempGbase.entries = new List<entryContainer>();

        //            //((ExpenseAmount*.50)/.65)*.35
        //            string fbt = getFbtFormula(getAccount(item.account).Account_FBT_MasterID);

        //            string equation = fbt.Replace("ExpenseAmount", item.debitGross.ToString());
        //             decimal fbtAmount = Mizuho.round((decimal)(new DataTable().Compute(equation, null)), 2);
        //            Console.WriteLine(equation);

        //            debit.account = getAccountByMasterID(int.Parse(xelemAcc.Element("D_FBT").Value)).Account_ID;
        //            debit.amount = fbtAmount;

        //            credit.account = getAccountByMasterID(int.Parse(xelemAcc.Element("C_FBT").Value)).Account_ID;
        //            credit.amount = fbtAmount;

        //            tempGbase.entries.Add(debit);
        //            tempGbase.entries.Add(credit);

        //            goExpData = InsertGbaseEntry(tempGbase, expID);
        //            goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.expenseDtlID);
        //            list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
        //        }
        //    }

        //    _GOContext.SaveChanges();
        //    _context.SaveChanges();

        //    List<ExpenseTransList> transactions = new List<ExpenseTransList>();

        //    foreach (var item in list)
        //    {
        //        ExpenseTransList tran = new ExpenseTransList
        //        {
        //            TL_ExpenseID = item.expEntryID,
        //            TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
        //            TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
        //            TL_Liquidation = false
        //            TL_StatusID = GlobalSystemValues.STATUS_PENDING
        //        };
        //        transactions.Add(tran);
        //    }

        //    _context.ExpenseTransLists.AddRange(transactions);
        //    _context.SaveChanges();
        //    return true;
        //}
        public bool postLiq_SS(int expID, string command, int userID)
        {
            var liquidationDetails = getExpenseToLiqudate(expID);
            var list = new[] {
                new { expEntryID = 0, expDtl = 0, expType = 0, LiqDtlID = 0, LiqInterEntityID = 0,  goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in liquidationDetails.LiquidationDetails)
            {
                if (item.liqCashBreakdown.Count() != 0)
                {
                    gbaseContainer tempGbase = new gbaseContainer();

                    tempGbase.valDate = liquidationDetails.LiqEntryDetails.Liq_Created_Date.Date;
                    tempGbase.remarks = "S" + item.GBaseRemarks;
                    tempGbase.maker = liquidationDetails.LiqEntryDetails.Liq_Created_UserID;
                    tempGbase.approver = liquidationDetails.LiqEntryDetails.Liq_Approver;

                    if (item.liqInterEntity[0].Liq_Amount_1_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = (item.liqInterEntity[0].Liq_DebitCred_1_1 == "D") ?
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_1_1 : "C" :
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_1_1 : "D",
                            amount = item.liqInterEntity[0].Liq_Amount_1_1,
                            account = item.liqInterEntity[0].Liq_AccountID_1_1,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_1_2 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = (item.liqInterEntity[0].Liq_DebitCred_1_2 == "D") ?
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_1_2 : "C" :
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_1_2 : "D",
                            amount = item.liqInterEntity[0].Liq_Amount_1_2,
                            account = item.liqInterEntity[0].Liq_AccountID_1_2,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_2_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = (item.liqInterEntity[0].Liq_DebitCred_2_1 == "D") ?
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_2_1 : "C" :
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_2_1 : "D",
                            amount = item.liqInterEntity[0].Liq_Amount_2_1,
                            account = item.liqInterEntity[0].Liq_AccountID_2_1,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_2_2 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = (item.liqInterEntity[0].Liq_DebitCred_2_2 == "D") ?
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_2_2 : "C" :
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_2_2 : "D",
                            amount = item.liqInterEntity[0].Liq_Amount_2_2,
                            account = item.liqInterEntity[0].Liq_AccountID_2_2,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_3_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = (item.liqInterEntity[0].Liq_DebitCred_3_1 == "D") ?
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_3_1 : "C" :
                                (command != "R") ? item.liqInterEntity[0].Liq_DebitCred_3_1 : "D",
                            amount = item.liqInterEntity[0].Liq_Amount_3_1,
                            account = item.liqInterEntity[0].Liq_AccountID_3_1,
                        });
                    }
                    tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                    goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                    goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.EntryDetailsID);
                    list.Add(new
                    {
                        expEntryID = expID,
                        expDtl = item.EntryDetailsID,
                        expType = GlobalSystemValues.TYPE_SS,
                        LiqDtlID = liquidationDetails.LiqEntryDetails.Liq_DtlID,
                        LiqInterEntityID = item.liqInterEntity[0].Liq_InterEntityID,
                        goExp = goExpData,
                        goExpHist = goExpHistData
                    });
                }
                else if (item.liqInterEntity.Count() != 0 && item.liqCashBreakdown.Count() == 0)
                {
                    foreach (var i in item.liqInterEntity)
                    {
                        if (i.Liq_Amount_1_1 == 0 && i.Liq_Amount_1_2 == 0)
                            continue;

                        gbaseContainer tempGbase = new gbaseContainer();

                        tempGbase.valDate = liquidationDetails.LiqEntryDetails.Liq_Created_Date.Date;
                        tempGbase.remarks = "S" + item.GBaseRemarks;
                        tempGbase.maker = liquidationDetails.LiqEntryDetails.Liq_Created_UserID;
                        tempGbase.approver = liquidationDetails.LiqEntryDetails.Liq_Approver;

                        if (i.Liq_Amount_1_1 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = (i.Liq_DebitCred_1_1 == "D") ?
                                (command != "R") ? i.Liq_DebitCred_1_1 : "C" :
                                (command != "R") ? i.Liq_DebitCred_1_1 : "D",
                                ccy = i.Liq_CCY_1_1,
                                amount = i.Liq_Amount_1_1,
                                account = i.Liq_AccountID_1_1,
                                interate = i.Liq_InterRate_1_2,
                                contraCcy = (i.Liq_CCY_1_1 != i.Liq_CCY_1_2) ? i.Liq_CCY_1_2 : 0
                            });
                        }

                        if (i.Liq_Amount_1_2 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = (i.Liq_DebitCred_1_2 == "D") ?
                                (command != "R") ? i.Liq_DebitCred_1_2 : "C" :
                                (command != "R") ? i.Liq_DebitCred_1_2 : "D",
                                ccy = i.Liq_CCY_1_2,
                                amount = i.Liq_Amount_1_2,
                                account = i.Liq_AccountID_1_2,
                                interate = i.Liq_InterRate_1_2,
                                contraCcy = (i.Liq_CCY_1_1 != i.Liq_CCY_1_2) ? i.Liq_CCY_1_1 : 0
                            });
                        }

                        if (i.Liq_Amount_2_1 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = (i.Liq_DebitCred_2_1 == "D") ?
                                (command != "R") ? i.Liq_DebitCred_2_1 : "C" :
                                (command != "R") ? i.Liq_DebitCred_2_1 : "D",
                                ccy = i.Liq_CCY_2_1,
                                amount = i.Liq_Amount_2_1,
                                account = i.Liq_AccountID_2_1,
                                interate = i.Liq_InterRate_2_1
                            });
                        }

                        if (i.Liq_Amount_2_2 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = (i.Liq_DebitCred_2_2 == "D") ?
                                (command != "R") ? i.Liq_DebitCred_2_2 : "C" :
                                (command != "R") ? i.Liq_DebitCred_2_2 : "D",
                                ccy = i.Liq_CCY_2_2,
                                amount = i.Liq_Amount_2_2,
                                account = i.Liq_AccountID_2_2,
                                interate = i.Liq_InterRate_2_2
                            });
                        }

                        tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                        goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                        goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.EntryDetailsID);
                        list.Add(new
                        {
                            expEntryID = expID,
                            expDtl = item.EntryDetailsID,
                            expType = GlobalSystemValues.TYPE_SS,
                            LiqDtlID = liquidationDetails.LiqEntryDetails.Liq_DtlID,
                            LiqInterEntityID = i.Liq_InterEntityID,
                            goExp = goExpData,
                            goExpHist = goExpHistData
                        });
                    }
                }
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = true,
                    TL_StatusID = GlobalSystemValues.STATUS_PENDING
                };
                transactions.Add(tran);
                if (command == "R")
                {
                    _context.ReversalEntry.Add(new ReversalEntryModel
                    {
                        Reversal_ExpenseEntryID = expID,
                        Reversal_ExpenseDtlID = item.expDtl,
                        Reversal_ExpenseType = item.expType,
                        Reversal_LiqDtlID = item.LiqDtlID,
                        Reversal_LiqInterEntityID = item.LiqInterEntityID,
                        Reversal_GOExpressID = int.Parse(item.goExp.Id.ToString()),
                        Reversal_GOExpressHistID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                        Reversal_ReversedDate = DateTime.Now,
                        Reversal_ReversedUserID = userID
                    });
                }
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        public bool postCV(int expID, string command, int userID)
        {
            var expenseDetails = getExpense(expID);

            var list = new[] {
                new { expEntryID = 0, expDtl = 0, expType = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in expenseDetails.EntryCV)
            {
                gbaseContainer tempGbase = new gbaseContainer();

                tempGbase.valDate = expenseDetails.expenseDate;
                tempGbase.remarks = item.GBaseRemarks;
                tempGbase.maker = expenseDetails.maker;
                tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver;

                entryContainer debit = new entryContainer();
                entryContainer credit = new entryContainer();

                //Debit
                debit.type = (command != "R") ? "D" : "C";
                debit.ccy = item.ccy;
                debit.amount = item.debitGross;
                debit.vendor = expenseDetails.selectedPayee;
                debit.account = item.account;
                debit.chkNo = expenseDetails.checkNo;
                debit.dept = item.dept;

                tempGbase.entries.Add(debit);

                //Credit 1 - tax withheld if only has tax amount and EWT Account 
                if (item.credEwt > 0 && item.creditAccount1 > 0)
                {
                    credit.type = (command != "R") ? "C" : "D";
                    credit.ccy = item.ccy;
                    credit.amount = item.credEwt;
                    credit.vendor = expenseDetails.selectedPayee;
                    credit.account = item.creditAccount1;
                    credit.dept = item.dept;

                    tempGbase.entries.Add(credit);
                }

                //Credit 2 - Credit amount
                credit = new entryContainer();
                credit.type = (command != "R") ? "C" : "D";
                credit.ccy = item.ccy;
                credit.amount = item.credCash;
                credit.vendor = expenseDetails.selectedPayee;
                credit.account = item.creditAccount2;
                credit.dept = item.dept;

                tempGbase.entries.Add(credit);
                tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.expenseDtlID);

                list.Add(new { expEntryID = expID, expDtl = item.expenseDtlID, expType = expenseDetails.expenseType, goExp = goExpData, goExpHist = goExpHistData });

                if (item.fbt)
                {
                    int exp_pre = int.Parse(xelemAcc.Element("C_Amort_EXP").Value);
                    int pe_pre = int.Parse(xelemAcc.Element("C_Amort_PE").Value);


                    tempGbase.entries = new List<entryContainer>();

                    Dictionary<string, entryContainer> fbt;

                    if ((item.account == exp_pre || item.account == pe_pre) && item.amorAcc != 0)
                    {
                        fbt = createFbt(expenseDetails.selectedPayee, item.amorAcc, item.debitGross, debit, credit);
                    }
                    else
                    {
                        fbt = createFbt(expenseDetails.selectedPayee, item.account, item.debitGross, debit, credit);
                    }

                    fbt["debit"].type = (command != "R") ? "D" : "C";
                    fbt["credit"].type = (command != "R") ? "C" : "D";

                    tempGbase.entries.Add(fbt["debit"]);
                    tempGbase.entries.Add(fbt["credit"]);

                    tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                    goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                    goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.expenseDtlID);
                    list.Add(new { expEntryID = expID, expDtl = item.expenseDtlID, expType = expenseDetails.expenseType, goExp = goExpData, goExpHist = goExpHistData });
                }
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false,
                    TL_StatusID = GlobalSystemValues.STATUS_PENDING
                };
                transactions.Add(tran);

                if (command == "R")
                {
                    _context.ReversalEntry.Add(new ReversalEntryModel
                    {
                        Reversal_ExpenseEntryID = expID,
                        Reversal_ExpenseDtlID = item.expDtl,
                        Reversal_ExpenseType = item.expType,
                        Reversal_GOExpressID = int.Parse(item.goExp.Id.ToString()),
                        Reversal_GOExpressHistID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                        Reversal_ReversedDate = DateTime.Now,
                        Reversal_ReversedUserID = userID
                    });
                }
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        public bool postDDV(int expID, string command, int userID)
        {
            var expenseDDV = getExpenseDDV(expID);

            var list = new[] {
                new { expEntryID = 0, expDtl = 0, expType = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in expenseDDV.EntryDDV)
            {
                gbaseContainer tempGbase = new gbaseContainer();
                if (item.inter_entity)
                {
                    DDVInterEntityViewModel interDtls = item.interDetails;

                    foreach (var particular in item.interDetails.interPartList)
                    {
                        tempGbase = new gbaseContainer
                        {
                            valDate = expenseDDV.expenseDate,
                            remarks = item.GBaseRemarks,
                            maker = expenseDDV.maker,
                            approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver
                        };

                        var entryType = "";
                        var sameCurr = true;
                        var firstCurr = particular.ExpenseEntryInterEntityAccs[0].Inter_Curr_ID;
                        //checks if particular have same currency
                        foreach (var accCurrs in particular.ExpenseEntryInterEntityAccs)
                        {
                            if (accCurrs.Inter_Curr_ID != firstCurr)
                            {
                                sameCurr = false;
                            }
                        }
                        if (sameCurr)
                        {
                            foreach (var accs in particular.ExpenseEntryInterEntityAccs)
                            {
                                entryType = (accs.Inter_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" :
                                                (accs.Inter_Type_ID == GlobalSystemValues.NC_CREDIT ||
                                                accs.Inter_Type_ID == GlobalSystemValues.NC_EWT) ? "C" : "";
                                if (command == "R")
                                {
                                    entryType = (entryType == "D") ? "C" : (entryType == "C") ? "D" : "";
                                }
                                if (entryType != "")
                                {
                                    entryContainer entry = new entryContainer
                                    {
                                        account = accs.Inter_Acc_ID,
                                        amount = accs.Inter_Amount,
                                        ccy = accs.Inter_Curr_ID,
                                        dept = item.dept,
                                        //this is for exchange rate
                                        interate = accs.Inter_Rate,
                                        type = entryType
                                    };
                                    tempGbase.entries.Add(entry);
                                }
                            }
                        }
                        else
                        {
                            tempGbase = new gbaseContainer
                            {
                                valDate = expenseDDV.expenseDate,
                                remarks = item.GBaseRemarks,
                                maker = expenseDDV.maker,
                                approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver
                            };

                            if (particular.ExpenseEntryInterEntityAccs.Count > 0)
                            {
                                entryType = (particular.ExpenseEntryInterEntityAccs[0].Inter_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" :
                                        (particular.ExpenseEntryInterEntityAccs[0].Inter_Type_ID == GlobalSystemValues.NC_CREDIT ||
                                        particular.ExpenseEntryInterEntityAccs[0].Inter_Type_ID == GlobalSystemValues.NC_EWT) ? "C" : "";
                                entryContainer entryDB = new entryContainer()
                                {
                                    account = particular.ExpenseEntryInterEntityAccs[0].Inter_Acc_ID,
                                    amount = particular.ExpenseEntryInterEntityAccs[0].Inter_Amount,
                                    ccy = particular.ExpenseEntryInterEntityAccs[0].Inter_Curr_ID,
                                    contraCcy = particular.ExpenseEntryInterEntityAccs[1].Inter_Curr_ID,
                                    dept = item.dept,
                                    //this is for exchange rate
                                    interate = particular.ExpenseEntryInterEntityAccs[1].Inter_Rate,
                                    type = entryType
                                };
                                tempGbase.entries.Add(entryDB);
                            }
                            if (particular.ExpenseEntryInterEntityAccs.Count > 1)
                            {
                                entryType = (particular.ExpenseEntryInterEntityAccs[1].Inter_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" :
                                        (particular.ExpenseEntryInterEntityAccs[1].Inter_Type_ID == GlobalSystemValues.NC_CREDIT ||
                                        particular.ExpenseEntryInterEntityAccs[1].Inter_Type_ID == GlobalSystemValues.NC_EWT) ? "C" : "";
                                entryContainer entryCD = new entryContainer()
                                {
                                    account = particular.ExpenseEntryInterEntityAccs[1].Inter_Acc_ID,
                                    amount = particular.ExpenseEntryInterEntityAccs[1].Inter_Amount,
                                    ccy = particular.ExpenseEntryInterEntityAccs[1].Inter_Curr_ID,
                                    dept = item.dept,
                                    //this is for exchange rate and inter rate
                                    interate = particular.ExpenseEntryInterEntityAccs[1].Inter_Rate,
                                    type = entryType,
                                    contraCcy = particular.ExpenseEntryInterEntityAccs[0].Inter_Curr_ID,
                                };
                                tempGbase.entries.Add(entryCD);
                            }
                        }
                        tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                        goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                        goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.dtlID);
                        list.Add(new { expEntryID = expID, expDtl = item.dtlID, expType = GlobalSystemValues.TYPE_DDV, goExp = goExpData, goExpHist = goExpHistData });

                        //FBT FOR INTER-ENTITY
                        if (item.fbt)
                        {
                            tempGbase.entries = new List<entryContainer>();
                            entryContainer credit = new entryContainer
                            {
                                amount = tempGbase.entries.Where(x => x.type == "C").Sum(x => x.amount),
                                dept = item.dept,
                                type = "C"
                            };
                            entryContainer debit = new entryContainer
                            {
                                amount = tempGbase.entries.Where(x => x.type == "D").Sum(x => x.amount),
                                dept = item.dept,
                                type = "D"
                            };
                            Dictionary<string, entryContainer> fbt = createFbt(expenseDDV.vendor, item.account, item.debitGross, debit, credit);

                            fbt["debit"].type = (command != "R") ? "D" : "C";
                            fbt["credit"].type = (command != "R") ? "C" : "D";

                            tempGbase.entries.Add(fbt["debit"]);
                            tempGbase.entries.Add(fbt["credit"]);

                            tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                            goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                            goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.dtlID);
                            list.Add(new { expEntryID = expID, expDtl = item.dtlID, expType = GlobalSystemValues.TYPE_DDV, goExp = goExpData, goExpHist = goExpHistData });
                        }
                    }
                }
                else
                {
                    tempGbase = new gbaseContainer
                    {
                        valDate = expenseDDV.expenseDate,
                        remarks = item.GBaseRemarks,
                        maker = expenseDDV.maker,
                        approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver
                    };

                    entryContainer debit = new entryContainer();
                    entryContainer credit = new entryContainer();

                    //Debit
                    debit.type = (command != "R") ? "D" : "C";
                    debit.ccy = item.ccy;
                    debit.amount = item.debitGross;
                    debit.vendor = expenseDDV.vendor;
                    debit.account = item.account;
                    debit.chkNo = expenseDDV.checkNo;
                    debit.dept = item.dept;

                    tempGbase.entries.Add(debit);

                    //Credit 1 - tax withheld if only has tax amount and EWT Account 
                    if (item.credEwt > 0 && item.creditAccount1 > 0)
                    {
                        entryContainer ewt = new entryContainer();
                        ewt.type = (command != "R") ? "C" : "D";
                        ewt.ccy = item.ccy;
                        ewt.amount = item.credEwt;
                        ewt.vendor = expenseDDV.vendor;
                        ewt.account = item.creditAccount1;
                        ewt.dept = item.dept;

                        tempGbase.entries.Add(ewt);
                    }

                    //Credit 2 - Credit amount
                    credit.type = (command != "R") ? "C" : "D";
                    credit.ccy = item.ccy;
                    credit.amount = item.credCash;
                    credit.vendor = expenseDDV.vendor;
                    credit.account = item.creditAccount2;
                    credit.dept = item.dept;

                    tempGbase.entries.Add(credit);

                    tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                    goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                    goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.dtlID);
                    list.Add(new { expEntryID = expID, expDtl = item.dtlID, expType = GlobalSystemValues.TYPE_DDV, goExp = goExpData, goExpHist = goExpHistData });
                    //FBT FOR GBASE REMARKS
                    if (item.fbt)
                    {
                        tempGbase.entries = new List<entryContainer>();

                        Dictionary<string, entryContainer> fbt = createFbt(expenseDDV.vendor, item.account, item.debitGross, debit, credit);

                        fbt["debit"].type = (command != "R") ? "D" : "C";
                        fbt["credit"].type = (command != "R") ? "C" : "D";

                        tempGbase.entries.Add(fbt["debit"]);
                        tempGbase.entries.Add(fbt["credit"]);

                        tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                        goExpData = InsertGbaseEntry(tempGbase, expID, userID);
                        goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.dtlID);
                        list.Add(new { expEntryID = expID, expDtl = item.dtlID, expType = GlobalSystemValues.TYPE_DDV, goExp = goExpData, goExpHist = goExpHistData });
                    }
                }
            }
            _GOContext.SaveChanges();
            _context.SaveChanges();

            //----------------------------- NOTIF----------------------------------
            var stats = command != "R" ? GlobalSystemValues.STATUS_APPROVED : GlobalSystemValues.STATUS_REVERSED;
            insertIntoNotif(userID, GlobalSystemValues.TYPE_DDV, stats, expenseDDV.maker);
            //----------------------------- NOTIF----------------------------------
            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false,
                    TL_StatusID = GlobalSystemValues.STATUS_PENDING
                };
                transactions.Add(tran);

                if (command == "R")
                {
                    _context.ReversalEntry.Add(new ReversalEntryModel
                    {
                        Reversal_ExpenseEntryID = expID,
                        Reversal_ExpenseDtlID = item.expDtl,
                        Reversal_ExpenseType = item.expType,
                        Reversal_GOExpressID = int.Parse(item.goExp.Id.ToString()),
                        Reversal_GOExpressHistID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                        Reversal_ReversedDate = DateTime.Now,
                        Reversal_ReversedUserID = userID
                    });
                }
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        public bool postNC(int expID, string command, int userID)
        {
            var expenseDetails = getExpenseNC(expID);
            var list = new[] {
                new { expEntryID = 0, nonCashDtlID = 0, expType = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();
            foreach (var dtls in expenseDetails.EntryNC.ExpenseEntryNCDtls)
            {
                gbaseContainer tempGbase = new gbaseContainer();

                tempGbase.valDate = expenseDetails.expenseDate;
                tempGbase.remarks = dtls.ExpNCDtl_Remarks_Desc + " " + dtls.ExpNCDtl_Remarks_Period;
                tempGbase.maker = expenseDetails.maker;
                tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver;

                var sameCurr = true;
                var firstCurr = dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_ID;
                //checks if particular have same currency
                foreach (var accCurrs in dtls.ExpenseEntryNCDtlAccs)
                {
                    if (accCurrs.ExpNCDtlAcc_Curr_ID != firstCurr)
                    {
                        sameCurr = false;
                    }
                }
                if (sameCurr)
                {
                    foreach (var item in dtls.ExpenseEntryNCDtlAccs)
                    {
                        if (item.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_CREDIT || item.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_EWT)
                        {
                            entryContainer credit = new entryContainer
                            {
                                type = (command != "R") ? "C" : "D",
                                ccy = item.ExpNCDtlAcc_Curr_ID,
                                amount = item.ExpNCDtlAcc_Amount,
                                account = item.ExpNCDtlAcc_Acc_ID,
                                interate = item.ExpNCDtlAcc_Inter_Rate
                            };
                            tempGbase.entries.Add(credit);
                        }
                        else if (item.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                        {
                            entryContainer debit = new entryContainer
                            {
                                type = (command != "R") ? "D" : "C",
                                ccy = item.ExpNCDtlAcc_Curr_ID,
                                amount = item.ExpNCDtlAcc_Amount,
                                account = item.ExpNCDtlAcc_Acc_ID,
                                interate = item.ExpNCDtlAcc_Inter_Rate
                            };
                            tempGbase.entries.Add(debit);
                        }
                    }
                }
                else
                {
                    var entryType = "";
                    if (dtls.ExpenseEntryNCDtlAccs.Count > 0)
                    {
                        entryType = (dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" :
                                (dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_CREDIT ||
                                dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_EWT) ? "C" : "";
                        if (command == "R")
                        {
                            entryType = (entryType == "D") ? "C" : "D";
                        }
                        entryContainer entryDB = new entryContainer()
                        {
                            account = dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Acc_ID,
                            amount = dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount,
                            ccy = dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_ID,
                            contraCcy = dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Curr_ID,
                            //this is for exchange rate
                            interate = (dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Inter_Rate > 0) ? dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Inter_Rate : dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Inter_Rate,
                            type = entryType,
                            vendor = dtls.ExpNCDtl_Vendor_ID
                        };
                        tempGbase.entries.Add(entryDB);
                    }
                    if (dtls.ExpenseEntryNCDtlAccs.Count > 1)
                    {
                        entryType = (dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" :
                                (dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_CREDIT ||
                                dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_EWT) ? "C" : "";
                        if (command == "R")
                        {
                            entryType = (entryType == "D") ? "C" : "D";
                        }
                        entryContainer entryCD = new entryContainer()
                        {
                            account = dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Acc_ID,
                            amount = dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount,
                            ccy = dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Curr_ID,
                            contraCcy = dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_ID,
                            //this is for exchange rate and inter rate
                            interate = (dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Inter_Rate > 0) ? dtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Inter_Rate : dtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Inter_Rate,
                            type = entryType,
                            vendor = dtls.ExpNCDtl_Vendor_ID
                        };
                        tempGbase.entries.Add(entryCD);
                    }
                }

                tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type == "D").ToList();
                //insert
                goExpData = InsertGbaseEntryNonCash(tempGbase, expID, userID, sameCurr);
                goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, dtls.ExpNCDtl_ID);
                list.Add(new
                {
                    expEntryID = expID,
                    nonCashDtlID = dtls.ExpNCDtl_ID,
                    expType = GlobalSystemValues.TYPE_NC,
                    goExp = goExpData,
                    goExpHist = goExpHistData
                });
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false,
                    TL_StatusID = GlobalSystemValues.STATUS_PENDING
                };
                transactions.Add(tran);
                if (command == "R")
                {
                    _context.ReversalEntry.Add(new ReversalEntryModel
                    {
                        Reversal_ExpenseEntryID = expID,
                        Reversal_ExpenseType = item.expType,
                        Reversal_NonCashDtlID = item.nonCashDtlID,
                        Reversal_GOExpressID = int.Parse(item.goExp.Id.ToString()),
                        Reversal_GOExpressHistID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                        Reversal_ReversedDate = DateTime.Now,
                        Reversal_ReversedUserID = userID
                    });
                }
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }

        internal Dictionary<string, entryContainer> createFbt(int payee, int acc, decimal gross, entryContainer d, entryContainer c)
        {
            //((ExpenseAmount*.50)/.65)*.35
            string fbt = getFbtFormula(getAccount(acc).Account_FBT_MasterID);

            string equation = fbt.Replace("ExpenseAmount", gross.ToString());

            var fbtCompute = Convert.ToDecimal(new DataTable().Compute(equation, null));
            decimal fbtAmount = Mizuho.round(fbtCompute, 2);
            Console.WriteLine("-=-=-=-=-=->" + equation);

            #region Get elements fron xml (ohr,rentDebit,expatDebit,localDebit,fbtCred)
            int ohr = int.Parse(xelemAcc.Element("HOUSE_RENT").Value);
            int rentDebit = int.Parse(xelemAcc.Element("D_FBT_RENT").Value);
            int expatDebit = int.Parse(xelemAcc.Element("D_FBT_EXPAT").Value);
            int localDebit = int.Parse(xelemAcc.Element("D_FBT_LOCAL").Value);
            int fbtCred = int.Parse(xelemAcc.Element("C_FBT").Value);
            #endregion

            int masterId = _context.DMAccount.FirstOrDefault(x => x.Account_ID == acc).Account_MasterID;
            int userType = _context.DMEmp.FirstOrDefault(x => x.Emp_ID == payee).Emp_Category_ID;
            if (masterId == ohr)
            {
                d.account = rentDebit;
                var rentDebitAcc = _context.DMAccount.FirstOrDefault(x => x.Account_MasterID == rentDebit
                                                                       && x.Account_isActive == true);
                d.ccy = _context.DMCurrency.FirstOrDefault(x => x.Curr_MasterID == rentDebitAcc.Account_Currency_MasterID
                                                             && x.Curr_isActive == true).Curr_ID;
            }
            else
            {
                int currMaster;
                switch (userType)
                {
                    case GlobalSystemValues.EMPCAT_EXPAT:
                        d.account = expatDebit;
                        currMaster = _context.DMAccount.FirstOrDefault(x => x.Account_MasterID == expatDebit
                                                       && x.Account_isActive == true).Account_Currency_MasterID;
                        d.ccy = _context.DMCurrency.FirstOrDefault(x => x.Curr_MasterID == currMaster
                                                                     && x.Curr_isActive == true).Curr_ID;
                        break;
                    case GlobalSystemValues.EMPCAT_LOCAL:
                        d.account = localDebit;
                        currMaster = _context.DMAccount.FirstOrDefault(x => x.Account_MasterID == localDebit
                                                                      && x.Account_isActive == true).Account_Currency_MasterID;
                        d.ccy = _context.DMCurrency.FirstOrDefault(x => x.Curr_MasterID == currMaster
                                                                     && x.Curr_isActive == true).Curr_ID;
                        break;
                };
            }


            d.amount = fbtAmount;

            c.account = getAccountByMasterID(int.Parse(xelemAcc.Element("C_FBT").Value)).Account_ID;
            c.amount = fbtAmount;
            var cAccount = _context.DMAccount.FirstOrDefault(x => x.Account_MasterID == c.account
                                                                    && x.Account_isActive == true);
            d.ccy = _context.DMCurrency.FirstOrDefault(x => x.Curr_MasterID == cAccount.Account_Currency_MasterID
                                                         && x.Curr_isActive == true).Curr_ID;

            Dictionary<string, entryContainer> fbtDic = new Dictionary<string, entryContainer>();

            fbtDic.Add("credit", c);
            fbtDic.Add("debit", d);

            return fbtDic;
        }
        ///============[End Post Entries]============

        ///================[Closing]=================
        public bool closeTransaction(string transactionType, string username, string password, string operation)
        {
            string closeCommand = "cm00@E*1@E@E17-@E1_10@E";

            TblRequestDetails rqDtl = new TblRequestDetails();
            GwriteTransList gWriteModel = new GwriteTransList();

            closeCommand = closeCommand.Replace("*", transactionType);
            closeCommand = closeCommand.Replace("-", username.Substring(username.Length - 4));
            closeCommand = operation.Equals("close") ? closeCommand.Replace("_", "2") : closeCommand.Replace("_", "3");

            rqDtl = postToGwrite(closeCommand, username, password);

            var close = _context.Closing.Where(x => x.Close_Type == transactionType)
                                           .OrderByDescending(x => x.Close_ID).FirstOrDefault();

            close.Close_Status = GlobalSystemValues.STATUS_PENDING;

            gWriteModel.GW_GWrite_ID = int.Parse(rqDtl.RequestId.ToString());
            gWriteModel.GW_Status = GlobalSystemValues.STATUS_PENDING;
            gWriteModel.GW_Type = "closing";
            gWriteModel.GW_TransID = close.Close_ID;

            _context.GwriteTransLists.Add(gWriteModel);

            _context.SaveChanges();

            return true;
        }

        public ClosingViewModel ClosingGetRecords()
        {
            var closeModel = _context.Closing.OrderByDescending(x => x.Close_ID).FirstOrDefault();

            DateTime opening = closeModel.Close_Open_Date.Date + new TimeSpan(0, 0, 0);
            DateTime closing = DateTime.Today.AddHours(23.9999);

            ClosingViewModel closeVM = new ClosingViewModel();

            int pcMasterID = int.Parse(xelemAcc.Element("PC_MASTERID").Value);

            ClosingModel closingItemsRBU = _context.Closing.Where(x => x.Close_Type == GlobalSystemValues.BRANCH_RBU)
                                                           .OrderByDescending(x => x.Close_Open_Date).FirstOrDefault();
            ClosingModel closingItemsFCDU = _context.Closing.Where(x => x.Close_Type == GlobalSystemValues.BRANCH_FCDU)
                                               .OrderByDescending(x => x.Close_Open_Date).FirstOrDefault();

            closeVM.fcduStatus = GlobalSystemValues.getStatus(closingItemsFCDU.Close_Status);
            closeVM.rbuStatus = GlobalSystemValues.getStatus(closingItemsRBU.Close_Status);

            var nmItems = getNM(opening, closing);
            var ddvItems = getDDV(opening, closing);
            var ncvItems = getNCV(opening, closing);
            var caItems = getCA(opening, closing);
            var liqItems = getLiq(opening, closing);
            var revItems = getReversed(opening);
            var revError = getErrReversed(opening, closing);

            //add results of getter methods to the return view model
            closeVM.fcduItems.AddRange(nmItems[GlobalSystemValues.BRANCH_FCDU]);
            closeVM.rbuItems.AddRange(nmItems[GlobalSystemValues.BRANCH_RBU]);

            closeVM.fcduItems.AddRange(ddvItems[GlobalSystemValues.BRANCH_FCDU]);
            closeVM.rbuItems.AddRange(ddvItems[GlobalSystemValues.BRANCH_RBU]);

            closeVM.fcduItems.AddRange(ncvItems[GlobalSystemValues.BRANCH_FCDU]);
            closeVM.rbuItems.AddRange(ncvItems[GlobalSystemValues.BRANCH_RBU]);

            closeVM.fcduItems.AddRange(caItems[GlobalSystemValues.BRANCH_FCDU]);
            closeVM.rbuItems.AddRange(caItems[GlobalSystemValues.BRANCH_RBU]);

            closeVM.fcduItems.AddRange(liqItems[GlobalSystemValues.BRANCH_FCDU]);
            closeVM.rbuItems.AddRange(liqItems[GlobalSystemValues.BRANCH_RBU]);

            closeVM.rbuItems.AddRange(revItems[GlobalSystemValues.BRANCH_RBU]);
            closeVM.fcduItems.AddRange(revItems[GlobalSystemValues.BRANCH_FCDU]);

            closeVM.rbuItems.AddRange(revError[GlobalSystemValues.BRANCH_RBU]);
            closeVM.fcduItems.AddRange(revError[GlobalSystemValues.BRANCH_FCDU]);

            Dictionary<int, List<int>> expenseRbuId = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> expenseFcduId = new Dictionary<int, List<int>>();

            PettyCashModel currentPC = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();

            closeVM.pettyBegBalance = currentPC.PC_StartBal;
            closeVM.closeBal = currentPC.PC_EndBal;
            closeVM.Disbursed = currentPC.PC_Disbursed;
            closeVM.recieve = currentPC.PC_Recieved;
            closeVM.date = DateTime.Now;

            return closeVM;
        }

        public Dictionary<string, List<CloseItems>> getNM(DateTime opening, DateTime closing)
        {
            #region diagnostic runtime checker begin
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            #endregion
            Dictionary<string, List<CloseItems>> nmDic = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> nmCloseItemsRBU = new List<CloseItems>();
            List<CloseItems> nmCloseItemsFCDU = new List<CloseItems>();

            var nmEntries = from expense in (from exp in _context.ExpenseEntry
                                             from dtl in _context.ExpenseEntryDetails
                                             from acc in _context.DMAccount
                                             from ccy in _context.DMCurrency
                                             where exp.Expense_ID == dtl.ExpenseEntryModel.Expense_ID
                                             && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                                             && dtl.ExpDtl_Account == acc.Account_ID
                                             && dtl.ExpDtl_Ccy == ccy.Curr_ID
                                             && dtl.ExpDtl_Inter_Entity == false
                                             && new List<int> { GlobalSystemValues.TYPE_CV,
                                                                GlobalSystemValues.TYPE_PC,
                                                                GlobalSystemValues.TYPE_DDV }.Contains(exp.Expense_Type)
                                             && (opening <= exp.Expense_Date
                                             && closing >= exp.Expense_Date)
                                             select new
                                             {
                                                 exp.Expense_ID,
                                                 dtl.ExpDtl_ID,
                                                 exp.Expense_Number,
                                                 exp.Expense_Date,
                                                 exp.Expense_Type,
                                                 exp.Expense_Status,
                                                 exp.Expense_Payee,
                                                 acc.Account_MasterID,
                                                 acc.Account_No,
                                                 dtl.ExpDtl_Gbase_Remarks,
                                                 acc.Account_Code,
                                                 ccy.Curr_CCY_ABBR,
                                                 dtl.ExpDtl_Debit,
                                                 dtl.ExpDtl_Fbt
                                             })
                            join goHist in _context.GOExpressHist
                            on new { expenseId = expense.Expense_ID, dtlId = expense.ExpDtl_ID }
                            equals new { expenseId = goHist.ExpenseEntryID, dtlId = goHist.ExpenseDetailID }
                            into x
                            from goHist in x.DefaultIfEmpty()
                            select new
                            {
                                expense.Expense_ID,
                                expense.ExpDtl_ID,
                                expense.Expense_Number,
                                expense.Account_No,
                                expense.ExpDtl_Debit,
                                expense.Curr_CCY_ABBR,
                                expense.Expense_Date,
                                expense.Expense_Status,
                                expense.Expense_Type,
                                expense.ExpDtl_Fbt,
                                expense.ExpDtl_Gbase_Remarks,
                                expense.Expense_Payee,
                                expense.Account_MasterID,
                                histId = goHist.GOExpHist_Id == null ? 0 : goHist.GOExpHist_Id
                            };

            foreach (var item in nmEntries)
            {
                CloseItems temp = new CloseItems();
                if (item.histId > 0)
                {
                    int transNo = _context.ExpenseTransLists.Select(x => new { x.TL_TransID, x.TL_GoExpHist_ID, x.TL_ExpenseID })
                                                            .Where(x => x.TL_ExpenseID == item.Expense_ID
                                                                     && x.TL_GoExpHist_ID == item.histId).FirstOrDefault().TL_TransID;
                    temp.expTrans = GlobalSystemValues.getApplicationCode(item.Expense_Type) + "-" + GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                        item.Expense_Number.ToString().PadLeft(5, '0');
                    temp.gBaseTrans = transNo.ToString();
                }
                else
                {
                    temp.expTrans = "";
                    temp.gBaseTrans = "";
                }

                temp.amount = item.ExpDtl_Debit;
                temp.ccy = item.Curr_CCY_ABBR;
                temp.status = GlobalSystemValues.getStatus(item.Expense_Status);
                temp.particulars = item.ExpDtl_Gbase_Remarks;
                temp.transCount = 1;

                if (item.ExpDtl_Fbt)
                {
                    CloseItems fbtItem = new CloseItems();
                    #region Get elements fron xml (ohr,rentDebit,expatDebit,localDebit,fbtCred)
                    int ohr = int.Parse(xelemAcc.Element("HOUSE_RENT").Value);
                    int rentDebit = int.Parse(xelemAcc.Element("D_FBT_RENT").Value);
                    int expatDebit = int.Parse(xelemAcc.Element("D_FBT_EXPAT").Value);
                    int localDebit = int.Parse(xelemAcc.Element("D_FBT_LOCAL").Value);
                    int fbtCred = int.Parse(xelemAcc.Element("C_FBT").Value);
                    #endregion

                    #region assign fbtAcc
                    string fbtAcc = "";

                    int userType = _context.DMEmp.FirstOrDefault(x => x.Emp_ID == item.Expense_Payee).Emp_Category_ID;
                    if (item.Account_MasterID == ohr)
                    {
                        fbtAcc = _context.DMAccount.Where(x => x.Account_MasterID == rentDebit)
                                                   .OrderByDescending(x => x.Account_ID)
                                                   .FirstOrDefault().Account_No;
                    }
                    else
                    {
                        switch (userType)
                        {
                            case GlobalSystemValues.EMPCAT_EXPAT:
                                fbtAcc = _context.DMAccount.Where(x => x.Account_MasterID == expatDebit)
                                               .OrderByDescending(x => x.Account_ID)
                                               .FirstOrDefault().Account_No;
                                break;
                            case GlobalSystemValues.EMPCAT_LOCAL:
                                fbtAcc = _context.DMAccount.Where(x => x.Account_MasterID == localDebit)
                                                       .OrderByDescending(x => x.Account_ID)
                                                       .FirstOrDefault().Account_No;
                                break;
                        };
                    }
                    #endregion

                    var gTrans = (from tran in _context.ExpenseTransLists
                                  join hist in _context.GOExpressHist
                                  on tran.TL_GoExpHist_ID equals hist.GOExpHist_Id
                                  where hist.ExpenseDetailID == item.ExpDtl_ID
                                  && hist.ExpenseEntryID == item.Expense_ID
                                  && hist.GOExpHist_Entry11ActNo == fbtAcc.Substring(Math.Max(0, fbtAcc.Length - 6))
                                  select new { tran.TL_TransID, hist.GOExpHist_Entry11Amt }).FirstOrDefault();

                    if (gTrans != null)
                    {
                        temp.gBaseTrans += "," + gTrans.TL_TransID;
                        temp.amount += Decimal.Parse(gTrans.GOExpHist_Entry11Amt);
                        temp.transCount = 2;
                    }
                }

                if (getBranchNo(item.Account_No) == GlobalSystemValues.BRANCH_RBU)
                {
                    int itemIndex = nmCloseItemsRBU.FindIndex(x => x.expTrans == temp.expTrans
                                                                && x.gBaseTrans == temp.gBaseTrans
                                                                && x.particulars == temp.particulars
                                                                && x.ccy == temp.ccy);
                    if (item.ExpDtl_Fbt)
                    {
                        if (!nmCloseItemsRBU.Any(x => x.particulars == temp.particulars && x.expTrans == temp.expTrans))
                            nmCloseItemsRBU.Add(temp);
                    }
                    else
                    {
                        var reversedToday = _context.ReversalEntry.Where(x => x.Reversal_ReversedDate >= opening);

                        if (itemIndex < 0 && !reversedToday.Any(x => x.Reversal_GOExpressHistID == item.histId))
                            nmCloseItemsRBU.Add(temp);
                    }
                }
                else
                {
                    int itemIndex = nmCloseItemsFCDU.FindIndex(x => x.expTrans == temp.expTrans
                                            && x.gBaseTrans == temp.gBaseTrans
                                            && x.particulars == temp.particulars
                                            && x.ccy == temp.ccy);

                    var reversedToday = _context.ReversalEntry.Where(x => x.Reversal_ReversedDate >= opening);

                    if (itemIndex < 0 && !reversedToday.Any(x => x.Reversal_GOExpressHistID == item.histId))
                        nmCloseItemsFCDU.Add(temp);
                }
            }

            nmDic.Add(GlobalSystemValues.BRANCH_RBU, nmCloseItemsRBU);
            nmDic.Add(GlobalSystemValues.BRANCH_FCDU, nmCloseItemsFCDU);

            #region diagnostic runtime checker end
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("NM Runtime ======>>> " + elapsedTime);
            #endregion

            return nmDic;
        }
        public Dictionary<string, List<CloseItems>> getDDV(DateTime opening, DateTime closing)
        {
            #region diagnostic runtime checker begin
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            #endregion

            Dictionary<string, List<CloseItems>> ddvDic = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> ddvCloseItemsRBU = new List<CloseItems>();
            List<CloseItems> ddvCloseItemsFCDU = new List<CloseItems>();

            #region Data Retrieval Linq for InterRate DDV
            var ddvInter = _context.ExpenseEntry.Join(_context.ExpenseEntryDetails,
                                                            a => a.Expense_ID,
                                                            b => b.ExpenseEntryModel.Expense_ID,
                                                            (a, b) => new { a, b })
                                                  .Join(_context.ExpenseEntryInterEntity,
                                                            b => b.b.ExpDtl_ID,
                                                            c => c.ExpenseEntryDetailModel.ExpDtl_ID,
                                                            (b, c) => new { b, c })
                                                  .Join(_context.ExpenseEntryInterEntityParticular,
                                                            c => c.c.ExpDtl_DDVInter_ID,
                                                            d => d.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID,
                                                            (c, d) => new { c, d })
                                                  .Join(_context.ExpenseEntryInterEntityAccs,
                                                            d => d.d.InterPart_ID,
                                                            e => e.ExpenseEntryInterEntityParticular.InterPart_ID,
                                                            (d, e) => new
                                                            {
                                                                d.c.b.a.Expense_ID,
                                                                d.c.b.a.Expense_Number,
                                                                d.c.b.a.Expense_Date,
                                                                d.c.b.a.Expense_Status,
                                                                d.c.b.a.Expense_Type,
                                                                d.c.b.b.ExpDtl_ID,
                                                                d.c.b.b.ExpDtl_Gbase_Remarks,
                                                                d.c.b.b.ExpDtl_Inter_Entity,
                                                                d.c.c.ExpDtl_DDVInter_ID,
                                                                d.d.InterPart_ID,
                                                                e.InterAcc_Acc_ID,
                                                                e.InterAcc_Curr_ID,
                                                                e.InterAcc_Amount,
                                                                e.InterAcc_Type_ID
                                                            })
                                                  .Join(_context.DMAccount, e => e.InterAcc_Acc_ID, f => f.Account_ID,
                                                            (e, f) => new
                                                            {
                                                                e.Expense_ID,
                                                                e.Expense_Number,
                                                                e.Expense_Date,
                                                                e.Expense_Status,
                                                                e.Expense_Type,
                                                                e.ExpDtl_ID,
                                                                e.ExpDtl_Gbase_Remarks,
                                                                e.ExpDtl_Inter_Entity,
                                                                e.ExpDtl_DDVInter_ID,
                                                                e.InterPart_ID,
                                                                f.Account_No,
                                                                f.Account_Code,
                                                                e.InterAcc_Curr_ID,
                                                                e.InterAcc_Amount,
                                                                e.InterAcc_Type_ID
                                                            })
                                                  .Where(x => x.ExpDtl_Inter_Entity == true
                                                           && x.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                                                           && x.ExpDtl_Inter_Entity == true
                                                           && x.Expense_Type == GlobalSystemValues.TYPE_DDV
                                                           && x.InterAcc_Type_ID == GlobalSystemValues.NC_DEBIT
                                                           && (opening <= x.Expense_Date
                                                           && closing >= x.Expense_Date));
            #endregion 

            var interRbu = ddvInter.Where(x => x.Account_No.Substring(4, 3) == "767");
            var interFcdu = ddvInter.Where(x => x.Account_No.Substring(4, 3) == "789");

            foreach (var item in interRbu)
            {
                CloseItems temp = new CloseItems();
                var goHist = _context.GOExpressHist.Select(x => new {
                    x.GOExpHist_Id,
                    x.GOExpHist_Entry11Amt,
                    x.ExpenseEntryID,
                    x.ExpenseDetailID,
                    x.GOExpHist_Entry11Actcde,
                    x.GOExpHist_Entry11ActNo
                }).FirstOrDefault(x => x.ExpenseEntryID == item.Expense_ID
                                    && x.ExpenseDetailID == item.ExpDtl_ID
                                    && x.GOExpHist_Entry11ActNo == item.Account_No.Substring(Math.Max(0, item.Account_No.Length - 6))
                                    && x.GOExpHist_Entry11Actcde == item.Account_Code);


                var index = 0;

                if (goHist != null && ddvCloseItemsRBU.Count > 0)
                    index = ddvCloseItemsRBU.FindIndex(x => x.ccy == GetCurrencyAbbrv(item.InterAcc_Curr_ID)
                                                         && x.expTrans != ""
                                                         && int.Parse(x.expTrans.Substring(9)) == item.Expense_Number);
                else
                    index = -1;
                if (index < 0)
                {
                    if (goHist != null)
                    {
                        var transList = _context.ExpenseTransLists.FirstOrDefault(x => x.TL_GoExpHist_ID == goHist.GOExpHist_Id
                                                                                    && x.TL_ExpenseID == item.Expense_ID);
                        temp.gBaseTrans = transList.TL_TransID.ToString();
                        temp.expTrans = GlobalSystemValues.getApplicationCode(item.Expense_Type) + "-" +
                                        GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                        item.Expense_Number.ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        temp.gBaseTrans = "";
                        temp.expTrans = "";
                    }
                    temp.particulars = item.ExpDtl_Gbase_Remarks;
                    temp.ccy = GetCurrencyAbbrv(item.InterAcc_Curr_ID);
                    temp.amount = item.InterAcc_Amount;
                    temp.transCount = 1;
                    temp.status = GlobalSystemValues.getStatus(item.Expense_Status);

                    ddvCloseItemsRBU.Add(temp);
                }
                else
                {
                    if (goHist != null)
                    {
                        var transList = _context.ExpenseTransLists.FirstOrDefault(x => x.TL_GoExpHist_ID == goHist.GOExpHist_Id
                                                                                    && x.TL_ExpenseID == item.Expense_ID);
                        ddvCloseItemsRBU[index].gBaseTrans += "," + transList.TL_TransID.ToString();
                    }
                    ddvCloseItemsRBU[index].amount += item.InterAcc_Amount;
                    ddvCloseItemsRBU[index].transCount += 1;
                }
            }
            foreach (var item in interFcdu)
            {
                CloseItems temp = new CloseItems();
                var goHist = _context.GOExpressHist.Select(x => new {
                    x.GOExpHist_Id,
                    x.GOExpHist_Entry11Amt,
                    x.ExpenseEntryID,
                    x.ExpenseDetailID,
                    x.GOExpHist_Entry11Actcde,
                    x.GOExpHist_Entry11ActNo
                }).FirstOrDefault(x => x.ExpenseEntryID == item.Expense_ID
                                    && x.ExpenseDetailID == item.ExpDtl_ID
                                    && x.GOExpHist_Entry11ActNo == item.Account_No.Substring(Math.Max(0, item.Account_No.Length - 6))
                                    && x.GOExpHist_Entry11Actcde == item.Account_Code);
                var index = 0;

                if (goHist != null && ddvCloseItemsFCDU.Count > 0)
                    index = ddvCloseItemsFCDU.FindIndex(x => x.ccy == GetCurrencyAbbrv(item.InterAcc_Curr_ID)
                                                         && x.expTrans != ""
                                                         && int.Parse(x.expTrans.Substring(9)) == item.Expense_Number);
                else
                    index = -1;

                if (index < 0)
                {
                    if (goHist != null)
                    {
                        var transList = _context.ExpenseTransLists.FirstOrDefault(x => x.TL_GoExpHist_ID == goHist.GOExpHist_Id
                                                                                    && x.TL_ExpenseID == item.Expense_ID);
                        temp.gBaseTrans = transList.TL_TransID.ToString();
                        temp.expTrans = GlobalSystemValues.getApplicationCode(item.Expense_Type) + "-" +
                                        GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                        item.Expense_Number.ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        temp.gBaseTrans = "";
                        temp.expTrans = "";
                    }
                    temp.particulars = item.ExpDtl_Gbase_Remarks;
                    temp.ccy = GetCurrencyAbbrv(item.InterAcc_Curr_ID);
                    temp.amount = item.InterAcc_Amount;
                    temp.transCount = 1;
                    temp.status = GlobalSystemValues.getStatus(item.Expense_Status);

                    ddvCloseItemsFCDU.Add(temp);
                }
                else
                {
                    if (goHist != null)
                    {
                        var transList = _context.ExpenseTransLists.FirstOrDefault(x => x.TL_GoExpHist_ID == goHist.GOExpHist_Id
                                                                                    && x.TL_ExpenseID == item.Expense_ID);
                        ddvCloseItemsFCDU[index].gBaseTrans += "," + transList.TL_TransID.ToString();
                    }
                    ddvCloseItemsFCDU[index].amount += item.InterAcc_Amount;
                    ddvCloseItemsFCDU[index].transCount += 1;
                }
            }

            ddvDic.Add(GlobalSystemValues.BRANCH_RBU, ddvCloseItemsRBU);
            ddvDic.Add(GlobalSystemValues.BRANCH_FCDU, ddvCloseItemsFCDU);

            #region diagnostic runtime checker end
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("DDV Runtime ======>>> " + elapsedTime);
            #endregion

            return ddvDic;
        }
        public Dictionary<string, List<CloseItems>> getNCV(DateTime opening, DateTime closing)
        {
            #region diagnostic runtime checker begin
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            #endregion

            Dictionary<string, List<CloseItems>> ncvDic = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> ncvCloseItemsRBU = new List<CloseItems>();
            List<CloseItems> ncvCloseItemsFCDU = new List<CloseItems>();
            #region Data retrieval linq
            var ncItems = _context.ExpenseEntry
                                  .Join(_context.ExpenseEntryNonCash,
                                        a => a.Expense_ID,
                                        b => b.ExpenseEntryModel.Expense_ID,
                                        (a, b) => new { a, b })
                                  .Join(_context.ExpenseEntryNonCashDetails,
                                        b => b.b.ExpNC_ID,
                                        c => c.ExpenseEntryNCModel.ExpNC_ID,
                                        (b, c) => new { b, c })
                                  .Join(_context.ExpenseEntryNonCashDetailAccounts,
                                        c => c.c.ExpNCDtl_ID,
                                        d => d.ExpenseEntryNCDtlModel.ExpNCDtl_ID,
                                        (c, d) => new { c, d })
                                  .Join(_context.DMAccount,
                                        d => d.d.ExpNCDtlAcc_Acc_ID,
                                        acc => acc.Account_ID,
                                        (d, acc) => new { d, acc.Account_No, acc.Account_Code })
                                  .Join(_context.DMCurrency,
                                        d => d.d.d.ExpNCDtlAcc_Curr_ID,
                                        ccy => ccy.Curr_ID,
                                        (d, ccy) => new {
                                            d.d.c.b.a.Expense_ID,
                                            d.d.c.b.a.Expense_Number,
                                            d.d.c.b.a.Expense_Date,
                                            d.d.c.b.a.Expense_Status,
                                            d.d.c.b.a.Expense_Type,
                                            d.d.c.b.b.ExpNC_ID,
                                            d.d.c.c.ExpNCDtl_ID,
                                            d.d.c.c.ExpNCDtl_Remarks_Desc,
                                            d.d.c.c.ExpNCDtl_Remarks_Period,
                                            d.d.d.ExpNCDtlAcc_ID,
                                            d.d.d.ExpNCDtlAcc_Type_ID,
                                            d.d.d.ExpNCDtlAcc_Amount,
                                            d.Account_Code,
                                            d.Account_No,
                                            ccy.Curr_CCY_ABBR
                                        })
                                  .Where(x => x.Expense_Type == GlobalSystemValues.TYPE_NC
                                           && x.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                                           && (opening <= x.Expense_Date
                                           && closing >= x.Expense_Date)
                                           && x.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT);
            #endregion
            foreach (var item in ncItems)
            {
                CloseItems temp = new CloseItems();

                var gh = _context.GOExpressHist.FirstOrDefault(x => x.ExpenseEntryID == item.Expense_ID
                                                                       && x.ExpenseDetailID == item.ExpNCDtl_ID);

                if (gh != null)
                {
                    temp.gBaseTrans = _context.ExpenseTransLists.FirstOrDefault(x => x.TL_ExpenseID == item.Expense_ID
                                    && x.TL_GoExpHist_ID == gh.GOExpHist_Id).TL_TransID.ToString();
                    temp.expTrans = "NCV-" + GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                        item.Expense_Number.ToString().PadLeft(5, '0');
                }
                else
                {
                    temp.gBaseTrans = "";
                    temp.expTrans = "";
                }

                temp.amount = item.ExpNCDtlAcc_Amount;
                temp.ccy = item.Curr_CCY_ABBR;
                temp.status = GlobalSystemValues.getStatus(item.Expense_Status);
                temp.particulars = item.ExpNCDtl_Remarks_Desc + item.ExpNCDtl_Remarks_Period;
                temp.transCount = 1;

                if (getBranchNo(item.Account_No) == GlobalSystemValues.BRANCH_RBU)
                    ncvCloseItemsRBU.Add(temp);
                else
                    ncvCloseItemsFCDU.Add(temp);
            }

            ncvDic.Add(GlobalSystemValues.BRANCH_RBU, ncvCloseItemsRBU);
            ncvDic.Add(GlobalSystemValues.BRANCH_FCDU, ncvCloseItemsFCDU);

            #region diagnostic runtime checker end
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("NCV Runtime ======>>> " + elapsedTime);
            #endregion

            return ncvDic;
        }
        public Dictionary<string, List<CloseItems>> getCA(DateTime opening, DateTime closing)
        {
            #region diagnostic runtime checker begin
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            #endregion

            Dictionary<string, List<CloseItems>> caDic = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> caCloseItemsRBU = new List<CloseItems>();
            List<CloseItems> caCloseItemsFCDU = new List<CloseItems>();

            var caItems = from expense in (from a in _context.ExpenseEntry
                                           from b in _context.ExpenseEntryDetails
                                           where a.Expense_ID == b.ExpenseEntryModel.Expense_ID
                                           && a.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                                           && opening <= a.Expense_Date && closing >= a.Expense_Date
                                           && a.Expense_Type == GlobalSystemValues.TYPE_SS
                                           select new
                                           {
                                               a.Expense_ID,
                                               b.ExpDtl_ID,
                                               a.Expense_Number,
                                               a.Expense_Date,
                                               b.ExpDtl_Gbase_Remarks,
                                               a.Expense_Status,
                                               b.ExpDtl_Ccy,
                                               b.ExpDtl_Debit,
                                               b.ExpDtl_Account
                                           })
                          join goHist in _context.GOExpressHist
                          on new { expenseId = expense.Expense_ID, dtlId = expense.ExpDtl_ID }
                          equals new { expenseId = goHist.ExpenseEntryID, dtlId = goHist.ExpenseDetailID }
                          into x
                          from goHist in x.DefaultIfEmpty()
                          join acc in _context.DMAccount
                          on expense.ExpDtl_Account equals acc.Account_ID
                          into accCode
                          from acc in accCode.DefaultIfEmpty()
                          join ccy in _context.DMCurrency
                          on expense.ExpDtl_Ccy equals ccy.Curr_ID
                          into ccyAbbr
                          from ccy in ccyAbbr.DefaultIfEmpty()
                          select new
                          {
                              expense.Expense_ID,
                              expense.ExpDtl_ID,
                              expense.Expense_Number,
                              acc.Account_No,
                              ccy.Curr_CCY_ABBR,
                              expense.Expense_Date,
                              expense.Expense_Status,
                              expense.ExpDtl_Debit,
                              expense.ExpDtl_Gbase_Remarks,
                              histId = goHist.GOExpHist_Id == null ? 0 : goHist.GOExpHist_Id
                          };

            foreach (var item in caItems)
            {
                CloseItems temp = new CloseItems();
                if (item.histId > 0)
                {
                    int transNo = _context.ExpenseTransLists.Select(x => new { x.TL_TransID, x.TL_GoExpHist_ID, x.TL_ExpenseID })
                                                            .Where(x => x.TL_ExpenseID == item.Expense_ID
                                                                     && x.TL_GoExpHist_ID == item.histId).FirstOrDefault().TL_TransID;
                    temp.expTrans = "SSV-" + GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                        item.Expense_Number.ToString().PadLeft(5, '0');
                    temp.gBaseTrans = transNo.ToString();
                }
                else
                {
                    temp.expTrans = "";
                    temp.gBaseTrans = "";
                }

                temp.amount = item.ExpDtl_Debit;
                temp.ccy = item.Curr_CCY_ABBR;
                temp.status = GlobalSystemValues.getStatus(item.Expense_Status);
                temp.particulars = item.ExpDtl_Gbase_Remarks;
                temp.transCount = 1;

                var reversedToday = _context.ReversalEntry.Where(x => x.Reversal_ReversedDate >= opening);
                if (!reversedToday.Any(x => x.Reversal_GOExpressHistID == item.histId))
                {
                    if (getBranchNo(item.Account_No) == GlobalSystemValues.BRANCH_RBU)
                        caCloseItemsRBU.Add(temp);
                    else
                        caCloseItemsFCDU.Add(temp);
                }
            }

            caDic.Add(GlobalSystemValues.BRANCH_RBU, caCloseItemsRBU);
            caDic.Add(GlobalSystemValues.BRANCH_FCDU, caCloseItemsFCDU);

            #region diagnostic runtime checker end
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("cav Runtime ======>>> " + elapsedTime);
            #endregion

            return caDic;
        }
        public Dictionary<string, List<CloseItems>> getLiq(DateTime opening, DateTime closing)
        {
            #region diagnostic runtime checker begin
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            #endregion

            Dictionary<string, List<CloseItems>> liqDic = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> liqCloseItemsRBU = new List<CloseItems>();
            List<CloseItems> liqCloseItemsFCDU = new List<CloseItems>();

            var liqItems = from exp in _context.ExpenseEntry
                           from expDtl in _context.ExpenseEntryDetails
                           from liqDtl in _context.LiquidationEntryDetails
                           from liqInter in _context.LiquidationInterEntity
                           from ccy in _context.DMCurrency
                           where exp.Expense_ID == expDtl.ExpenseEntryModel.Expense_ID
                           && liqDtl.Liq_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                           && exp.Expense_ID == liqDtl.ExpenseEntryModel.Expense_ID
                           && expDtl.ExpDtl_ID == liqInter.ExpenseEntryDetailModel.ExpDtl_ID
                           && ((liqInter.Liq_CCY_1_1 == 0) ? ccy.Curr_ID == 31 : ccy.Curr_ID == liqInter.Liq_CCY_1_1)
                           && (opening <= liqDtl.Liq_Created_Date
                           && closing >= liqDtl.Liq_Created_Date)
                           select new
                           {
                               exp.Expense_ID,
                               expDtl.ExpDtl_ID,
                               exp.Expense_Number,
                               liqDtl.Liq_Created_Date,
                               remarks = string.Concat('S', expDtl.ExpDtl_Gbase_Remarks),
                               liqDtl.Liq_Status,
                               ccy.Curr_CCY_ABBR,
                               liqInter
                           };
            //join goHist in _context.GOExpressHist
            //on new { expenseId = expense.Expense_ID, dtlId = expense.ExpDtl_ID,
            //    rmrk = expense.remarks, acc = expense.Account_No.Substring(Math.Max(0, expense.Account_No.Length - 6)),
            //    cde = expense.Account_Code }
            //equals new { expenseId = goHist.ExpenseEntryID, dtlId = goHist.ExpenseDetailID,
            //    rmrk = goHist.GOExpHist_Remarks, acc = goHist.GOExpHist_Entry11ActNo,
            //    cde = goHist.GOExpHist_Entry11Actcde }
            //into x
            //from goHist in x.DefaultIfEmpty()
            //select new
            //{ expense.Expense_ID, expense.ExpDtl_ID, expense.Expense_Number, expense.Account_No, expense.Liq_DebitCred_1_2,
            //    expense.Curr_CCY_ABBR, expense.Liq_Created_Date, expense.Liq_Status, expense.Liq_Amount_1_1,
            //    expense.Liq_Amount_1_2, expense.remarks, histId = goHist.GOExpHist_Id == null ? 0 : goHist.GOExpHist_Id };

            foreach (var item in liqItems)
            {
                decimal total = 0.00M;
                DMAccountModel accItem = new DMAccountModel();
                GOExpressHistModel goHist = new GOExpressHistModel();
                bool ignoreFlag = false;
                total += item.liqInter.Liq_Amount_1_1 > 0 ? item.liqInter.Liq_Amount_1_1 : 0;
                total += item.liqInter.Liq_Amount_1_2 > 0 && item.liqInter.Liq_DebitCred_1_2 == "D" ? item.liqInter.Liq_Amount_1_2 : 0;
                total += item.liqInter.Liq_Amount_2_1 > 0 && item.liqInter.Liq_DebitCred_2_1 == "D" ? item.liqInter.Liq_Amount_2_1 : 0;
                total += item.liqInter.Liq_Amount_2_2 > 0 && item.liqInter.Liq_DebitCred_2_2 == "D" ? item.liqInter.Liq_Amount_2_2 : 0;
                total += item.liqInter.Liq_Amount_3_1 > 0 && item.liqInter.Liq_DebitCred_3_1 == "D" ? item.liqInter.Liq_Amount_3_1 : 0;
                total += item.liqInter.Liq_Amount_3_2 > 0 && item.liqInter.Liq_DebitCred_3_2 == "D" ? item.liqInter.Liq_Amount_3_2 : 0;

                if (item.liqInter.Liq_Amount_1_1 > 0)
                {
                    accItem = getAccount(item.liqInter.Liq_AccountID_1_1);
                }
                else if (item.liqInter.Liq_Amount_1_2 > 0 && item.liqInter.Liq_DebitCred_1_2 == "D")
                {
                    accItem = getAccount(item.liqInter.Liq_AccountID_1_2);
                }
                else if (item.liqInter.Liq_Amount_2_1 > 0 && item.liqInter.Liq_DebitCred_2_1 == "D")
                {
                    accItem = getAccount(item.liqInter.Liq_AccountID_2_1);
                }
                else if (item.liqInter.Liq_Amount_2_2 > 0 && item.liqInter.Liq_DebitCred_2_2 == "D")
                {
                    accItem = getAccount(item.liqInter.Liq_AccountID_2_2);
                }
                else if (item.liqInter.Liq_Amount_3_1 > 0 && item.liqInter.Liq_DebitCred_3_1 == "D")
                {
                    accItem = getAccount(item.liqInter.Liq_AccountID_3_1);
                }
                else if (item.liqInter.Liq_Amount_3_2 > 0 && item.liqInter.Liq_DebitCred_3_2 == "D")
                {
                    accItem = getAccount(item.liqInter.Liq_AccountID_3_2);
                }
                else
                {
                    ignoreFlag = true;
                }

                if (!ignoreFlag)
                {
                    goHist = _context.GOExpressHist.FirstOrDefault(x => x.ExpenseEntryID == item.Expense_ID
                                                                     && x.ExpenseDetailID == item.ExpDtl_ID
                                                                     && x.GOExpHist_Remarks == item.remarks
                                                                     && x.GOExpHist_Entry11Actcde == accItem.Account_Code
                                                                     && x.GOExpHist_Branchno == accItem.Account_No.Substring(4, 3)
                                                                     && x.GOExpHist_Entry11ActNo == accItem.Account_No.Substring(Math.Max(0, accItem.Account_No.Length - 6))
                                                                     && x.GOExpHist_Entry11ActType == accItem.Account_No.Substring(0, 3));
                }

                CloseItems temp = new CloseItems();
                if (goHist != null)
                {
                    if (goHist.GOExpHist_Id > 0)
                    {
                        int transNo = _context.ExpenseTransLists.Select(x => new { x.TL_TransID, x.TL_GoExpHist_ID, x.TL_ExpenseID })
                                                                .Where(x => x.TL_ExpenseID == item.Expense_ID
                                                                         && x.TL_GoExpHist_ID == goHist.GOExpHist_Id).FirstOrDefault().TL_TransID;
                        temp.expTrans = "LIQ-" + GetSelectedYearMonthOfTerm(item.Liq_Created_Date.Month, item.Liq_Created_Date.Year).Year + "-" +
                                            item.Expense_Number.ToString().PadLeft(5, '0');
                        temp.gBaseTrans = transNo.ToString();
                    }
                    else
                    {
                        temp.expTrans = "";
                        temp.gBaseTrans = "";
                    }
                }
                else
                {
                    temp.expTrans = "";
                    temp.gBaseTrans = "";
                }


                temp.amount = total;
                temp.ccy = item.Curr_CCY_ABBR;
                temp.status = GlobalSystemValues.getStatus(item.Liq_Status);
                temp.particulars = item.remarks;
                temp.transCount = 1;

                if (!ignoreFlag)
                {
                    if (accItem.Account_No.Substring(4, 3) == GlobalSystemValues.BRANCH_RBU)
                        liqCloseItemsRBU.Add(temp);
                    else
                        liqCloseItemsFCDU.Add(temp);
                }
            }

            liqDic.Add(GlobalSystemValues.BRANCH_RBU, liqCloseItemsRBU);
            liqDic.Add(GlobalSystemValues.BRANCH_FCDU, liqCloseItemsFCDU);

            #region diagnostic runtime checker end
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("liq Runtime ======>>> " + elapsedTime);
            #endregion

            return liqDic;
        }
        public Dictionary<string, List<CloseItems>> getReversed(DateTime opening)
        {
            Dictionary<string, List<CloseItems>> returnModel = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> rbuList = new List<CloseItems>();
            List<CloseItems> fcduList = new List<CloseItems>();

            var model = from rev in _context.ReversalEntry
                        join expTrans in _context.ExpenseTransLists
                        on rev.Reversal_GOExpressHistID
                        equals expTrans.TL_GoExpHist_ID
                        into x
                        from expTrans in x.DefaultIfEmpty()
                        where rev.Reversal_ReversedDate >= opening
                        && expTrans.TL_StatusID != GlobalSystemValues.STATUS_ERROR
                        select new
                        {
                            rev.Reversal_ID,
                            rev.Reversal_ExpenseEntryID,
                            rev.Reversal_ExpenseType,
                            rev.Reversal_ExpenseDtlID,
                            rev.Reversal_NonCashDtlID,
                            rev.Reversal_LiqDtlID,
                            rev.Reversal_LiqInterEntityID,
                            rev.Reversal_GOExpressHistID,
                            expTrans.TL_TransID,
                            expTrans.TL_StatusID,
                            expTrans.TL_Liquidation
                        };

            foreach (var item in model)
            {
                var expense = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == item.Reversal_ExpenseEntryID);
                string stat = item.TL_StatusID == 3 ? "REVERSED" : GlobalSystemValues.getStatus(item.TL_StatusID);
                var goHist = _context.GOExpressHist.FirstOrDefault(x => x.GOExpHist_Id == item.Reversal_GOExpressHistID);

                decimal amtTotal = 0.00M;
                amtTotal += Decimal.Parse(goHist.GOExpHist_Entry11Amt);
                if (goHist.GOExpHist_Entry12Type != null)
                    amtTotal += goHist.GOExpHist_Entry12Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry12Amt);
                if (goHist.GOExpHist_Entry21Type != null)
                    amtTotal += goHist.GOExpHist_Entry21Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry21Amt);
                if (goHist.GOExpHist_Entry22Type != null)
                    amtTotal += goHist.GOExpHist_Entry22Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry22Amt);
                if (goHist.GOExpHist_Entry31Type != null)
                    amtTotal += goHist.GOExpHist_Entry31Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry31Amt);
                if (goHist.GOExpHist_Entry32Type != null)
                    amtTotal += goHist.GOExpHist_Entry32Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry32Amt);
                if (goHist.GOExpHist_Entry41Type != null)
                    amtTotal += goHist.GOExpHist_Entry41Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry41Amt);
                if (goHist.GOExpHist_Entry42Type != null)
                    amtTotal += goHist.GOExpHist_Entry42Type != "D" ? 0 : Decimal.Parse(goHist.GOExpHist_Entry42Amt);

                string expTransNo = "";

                if (item.TL_Liquidation)
                {
                    expTransNo = "LIQ-" + GetSelectedYearMonthOfTerm(expense.Expense_Date.Month, expense.Expense_Date.Year).Year + "-" +
                                          expense.Expense_Number.ToString().PadLeft(5, '0');
                }
                else
                {
                    expTransNo = GlobalSystemValues.getApplicationCode(expense.Expense_Type) + "-" + GetSelectedYearMonthOfTerm(expense.Expense_Date.Month, expense.Expense_Date.Year).Year + "-" +
                                        expense.Expense_Number.ToString().PadLeft(5, '0');
                }

                CloseItems tempItem = new CloseItems
                {
                    gBaseTrans = item.TL_TransID.ToString(),
                    transCount = 1,
                    status = stat,
                    expTrans = expTransNo,
                    particulars = goHist.GOExpHist_Remarks,
                    amount = amtTotal,
                    ccy = goHist.GOExpHist_Entry11Ccy
                };

                if (goHist.GOExpHist_Branchno == GlobalSystemValues.BRANCH_RBU)
                {
                    rbuList.Add(tempItem);
                }
                else
                {
                    fcduList.Add(tempItem);
                }
            }

            returnModel.Add(GlobalSystemValues.BRANCH_RBU, rbuList);
            returnModel.Add(GlobalSystemValues.BRANCH_FCDU, fcduList);

            return returnModel;
        }
        public Dictionary<string, List<CloseItems>> getErrReversed(DateTime opening, DateTime closing)
        {
            Dictionary<string, List<CloseItems>> returnModel = new Dictionary<string, List<CloseItems>>();
            List<CloseItems> rbuList = new List<CloseItems>();
            List<CloseItems> fcduList = new List<CloseItems>();

            var model = from exp in _context.ExpenseEntry
                        join tl in _context.ExpenseTransLists
                        on exp.Expense_ID
                        equals tl.TL_ExpenseID
                        join gh in _context.GOExpressHist
                        on tl.TL_GoExpHist_ID equals gh.GOExpHist_Id
                        where exp.Expense_Status == 21
                        && tl.TL_StatusID == 3
                        && (exp.Expense_Last_Updated >= opening && exp.Expense_Last_Updated <= closing)
                        select new { exp.Expense_Status, exp.Expense_Date, exp.Expense_Type, exp.Expense_Number, tl.TL_TransID, tl.TL_Liquidation, tl.TL_GoExpHist_ID, gh };

            foreach (var item in model)
            {
                decimal amtTotal = 0.00M;
                amtTotal += Decimal.Parse(item.gh.GOExpHist_Entry11Amt);
                if (item.gh.GOExpHist_Entry12Type != null)
                    amtTotal += item.gh.GOExpHist_Entry12Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry12Amt);
                if (item.gh.GOExpHist_Entry21Type != null)
                    amtTotal += item.gh.GOExpHist_Entry21Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry21Amt);
                if (item.gh.GOExpHist_Entry22Type != null)
                    amtTotal += item.gh.GOExpHist_Entry22Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry22Amt);
                if (item.gh.GOExpHist_Entry31Type != null)
                    amtTotal += item.gh.GOExpHist_Entry31Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry31Amt);
                if (item.gh.GOExpHist_Entry32Type != null)
                    amtTotal += item.gh.GOExpHist_Entry32Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry32Amt);
                if (item.gh.GOExpHist_Entry41Type != null)
                    amtTotal += item.gh.GOExpHist_Entry41Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry41Amt);
                if (item.gh.GOExpHist_Entry42Type != null)
                    amtTotal += item.gh.GOExpHist_Entry42Type != "D" ? 0 : Decimal.Parse(item.gh.GOExpHist_Entry42Amt);

                string expTransNo = "";

                if (item.TL_Liquidation)
                {
                    expTransNo = "LIQ-" + GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                          item.Expense_Number.ToString().PadLeft(5, '0');
                }
                else
                {
                    expTransNo = GlobalSystemValues.getApplicationCode(item.Expense_Type) + "-" + GetSelectedYearMonthOfTerm(item.Expense_Date.Month, item.Expense_Date.Year).Year + "-" +
                                        item.Expense_Number.ToString().PadLeft(5, '0');
                }

                CloseItems tempItem = new CloseItems
                {
                    gBaseTrans = item.TL_TransID.ToString(),
                    transCount = 1,
                    status = GlobalSystemValues.getStatus(item.Expense_Status),
                    expTrans = expTransNo,
                    particulars = item.gh.GOExpHist_Remarks,
                    amount = amtTotal,
                    ccy = item.gh.GOExpHist_Entry11Ccy
                };

                if (item.gh.GOExpHist_Branchno == GlobalSystemValues.BRANCH_RBU)
                {
                    rbuList.Add(tempItem);
                }
                else
                {
                    fcduList.Add(tempItem);
                }

            }

            returnModel.Add(GlobalSystemValues.BRANCH_RBU, rbuList);
            returnModel.Add(GlobalSystemValues.BRANCH_FCDU, fcduList);

            return returnModel;
        }

        public ClosingViewModel ClosingOpenDailyBook()
        {
            ClosingModel fcduModel = new ClosingModel();
            ClosingModel rbuModel = new ClosingModel();

            DateTime openDate = DateTime.Now;

            fcduModel.Close_Open_Date = openDate;
            fcduModel.Close_Type = GlobalSystemValues.BRANCH_FCDU;
            fcduModel.Close_Status = 12;

            rbuModel.Close_Open_Date = openDate;
            rbuModel.Close_Type = GlobalSystemValues.BRANCH_RBU;
            rbuModel.Close_Status = 12;

            List<ClosingModel> listClosing = new List<ClosingModel>() { fcduModel, rbuModel };

            _context.Closing.AddRange(listClosing);
            _context.SaveChanges();

            _context.Entry<ClosingModel>(fcduModel).State = EntityState.Detached;
            _context.Entry<ClosingModel>(rbuModel).State = EntityState.Detached;

            ClosingViewModel closeVM = ClosingGetRecords();

            return closeVM;
        }
        public bool ClosingCheckStatus(string branchCode)
        {
            var closeModel = _context.Closing.Where(x => x.Close_Status == GlobalSystemValues.STATUS_OPEN || x.Close_Status == GlobalSystemValues.STATUS_ERROR).FirstOrDefault();

            DateTime opening = closeModel.Close_Open_Date.Date + new TimeSpan(0, 0, 0);

            var nmStatus = (from exp in _context.ExpenseEntry
                            from dtl in _context.ExpenseEntryDetails
                            from acc in _context.DMAccount
                            where exp.Expense_ID == dtl.ExpenseEntryModel.Expense_ID
                            && dtl.ExpDtl_Account == acc.Account_ID
                            && dtl.ExpDtl_Inter_Entity == false
                            && new List<int> { 1, 2, 4, 3 }.Contains(exp.Expense_Type)
                            && acc.Account_No.Substring(4, 3) == branchCode
                            && exp.Expense_Status != GlobalSystemValues.STATUS_FOR_CLOSING
                            && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED
                            && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                            && opening <= exp.Expense_Date
                            select new { exp.Expense_ID, exp.Expense_Number, dtl.ExpDtl_ID, acc.Account_No, exp.Expense_Status }).ToList().Count;

            var ddvInterStatus = (from exp in _context.ExpenseEntry
                                  from dtl in _context.ExpenseEntryDetails
                                  from inter in _context.ExpenseEntryInterEntity
                                  from parts in _context.ExpenseEntryInterEntityParticular
                                  from interAcc in _context.ExpenseEntryInterEntityAccs
                                  from acc in _context.DMAccount
                                  where exp.Expense_ID == dtl.ExpenseEntryModel.Expense_ID
                                  && dtl.ExpDtl_ID == inter.ExpenseEntryDetailModel.ExpDtl_ID
                                  && inter.ExpDtl_DDVInter_ID == parts.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID
                                  && parts.InterPart_ID == interAcc.ExpenseEntryInterEntityParticular.InterPart_ID
                                  && interAcc.InterAcc_Acc_ID == acc.Account_ID
                                  && interAcc.InterAcc_Type_ID == 1
                                  && dtl.ExpDtl_Inter_Entity == true
                                  && exp.Expense_Type == 2
                                  && acc.Account_No.Substring(4, 3) == branchCode
                                  && exp.Expense_Status != GlobalSystemValues.STATUS_FOR_CLOSING
                                  && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED
                                  && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                                  && opening <= exp.Expense_Date
                                  select new { exp.Expense_ID, dtl.ExpDtl_ID, acc.Account_No, exp.Expense_Status }).ToList().Count;

            var liqStatus = (from exp in _context.ExpenseEntry
                             from expDtl in _context.ExpenseEntryDetails
                             from liqDtl in _context.LiquidationEntryDetails
                             from liqInter in _context.LiquidationInterEntity
                             from acc in _context.DMAccount
                             where exp.Expense_ID == expDtl.ExpenseEntryModel.Expense_ID
                             && exp.Expense_ID == liqDtl.ExpenseEntryModel.Expense_ID
                             && expDtl.ExpDtl_ID == liqInter.ExpenseEntryDetailModel.ExpDtl_ID
                             && acc.Account_ID == liqInter.Liq_AccountID_1_1
                             && liqInter.Liq_Amount_1_1 > 0
                             && acc.Account_No.Substring(4, 3) == branchCode
                             && liqDtl.Liq_Status != GlobalSystemValues.STATUS_FOR_CLOSING
                             && liqDtl.Liq_Status != GlobalSystemValues.STATUS_REVERSED
                             && liqDtl.Liq_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                             && opening <= liqDtl.Liq_Created_Date
                             select new
                             { exp.Expense_ID, expDtl.ExpDtl_ID, exp.Expense_Number, liqDtl.Liq_Status, acc.Account_No }).ToList().Count;

            var ncStatus = (from exp in _context.ExpenseEntry
                            where exp.Expense_Type == 5
                            && exp.Expense_Status != GlobalSystemValues.STATUS_FOR_CLOSING
                            && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED
                            && exp.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR
                            && opening <= exp.Expense_Date
                            select new { exp.Expense_ID }).ToList().Count;

            if (liqStatus > 0 || ddvInterStatus > 0 || nmStatus > 0 || ncStatus > 0)
                return true;

            return false;
        }

        public bool ClosingCheckStatus()
        {
            var rbuStatus = _context.Closing.OrderByDescending(x => x.Close_Open_Date).FirstOrDefault(x => x.Close_Type == GlobalSystemValues.BRANCH_RBU);
            var fcduStatus = _context.Closing.OrderByDescending(x => x.Close_Open_Date).FirstOrDefault(x => x.Close_Type == GlobalSystemValues.BRANCH_FCDU);

            if (rbuStatus.Close_Status == GlobalSystemValues.STATUS_CLOSED &&
                fcduStatus.Close_Status == GlobalSystemValues.STATUS_CLOSED)
            {
                return true;
            }

            return false;
        }
        ///==============[End Closing]===============

        ///============[Post to GWrite]==============
        public TblRequestDetails postToGwrite(string command, string username, string password)
        {
            TblRequestDetails rqDtlModel = new TblRequestDetails();
            rqDtlModel.tblRequestItems = new List<TblRequestItem>();
            TblRequestItem rqItemModel = new TblRequestItem();

            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(password);
            string encodedPass = "";
            int index = 0;

            foreach (byte b in asciiBytes)
            {
                string hexValue = b.ToString("X");
                encodedPass += hexValue + ",0";
                if (index != asciiBytes.Length - 1)
                    encodedPass += ",";
                index++;
            }

            rqDtlModel.RacfId = username;
            rqDtlModel.RacfPassword = encodedPass;
            rqDtlModel.RequestCreated = DateTime.Now;
            rqDtlModel.Status = "WAITING";
            rqDtlModel.SystemAbbr = "EXPRESS";
            rqDtlModel.Priority = 1;

            rqItemModel.SequenceNo = 1;
            rqItemModel.ReturnFlag = true;
            rqItemModel.Command = command;

            rqDtlModel.tblRequestItems.Add(rqItemModel);

            _gWriteContext.Add(rqDtlModel);
            _gWriteContext.SaveChanges();

            return rqDtlModel;
        }
        ///==========[End Post to Gwrite]============

        ///==============[Begin Gbase Entry Section]================
        private TblCm10 InsertGbaseEntry(gbaseContainer containerModel, int expenseID, int userID)
        {
            TblCm10 goModel = new TblCm10();

            var user = _context.User.FirstOrDefault(x => x.User_ID == userID);
            var phpID = getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_PHP").Value)).Curr_ID;

            //goModel.Id = -1;
            goModel.SystemName = "EXPRESS";
            goModel.Branchno = getAccount(containerModel.entries[0].account).Account_No.Substring(4, 3);
            goModel.AutoApproved = "Y";
            goModel.ValueDate = DateTime.Now.ToString("MMddyy");
            goModel.Section = "10";
            goModel.WarningOverride = "Y";
            goModel.Remarks = containerModel.remarks;
            goModel.MakerEmpno = user.User_EmpCode; //Replace with user ID later when user module is finished.
            goModel.Empno = user.User_EmpCode.Substring(2);  //Replace with user ID later when user module is finished.
            goModel.Recstatus = "READY";
            goModel.Datestamp = DateTime.Now;
            goModel.Timerespond = DateTime.Now;
            goModel.Timesent = DateTime.Now;

            if (containerModel.entries.Count > 0)
            {
                goModel.Entry11Type = containerModel.entries[0].type;
                goModel.Entry11Ccy = GetCurrencyAbbrv(containerModel.entries[0].ccy);
                goModel.Entry11Amt = containerModel.entries[0].amount.ToString();

                var entry11Account = getAccount(containerModel.entries[0].account);
                goModel.Entry11Cust = entry11Account.Account_Cust;
                goModel.Entry11Actcde = entry11Account.Account_Code;
                goModel.Entry11ActType = entry11Account.Account_No.Substring(0, 3);
                goModel.Entry11ActNo = entry11Account.Account_No.Substring(Math.Max(0, entry11Account.Account_No.Length - 6));
                goModel.Entry11ExchRate = containerModel.entries[0].interate == 0 ? "" : containerModel.entries[0].interate.ToString();
                goModel.Entry11ExchCcy = (containerModel.entries[0].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[0].contraCcy) : "";
                goModel.Entry11Fund = (entry11Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                goModel.Entry11Available = "";//Replace with proper available default.
                goModel.Entry11Details = "";//Replace with proper details default.
                goModel.Entry11Entity = "010";//Replace with proper entity default.
                goModel.Entry11Division = entry11Account.Account_Div;//Replace with proper division default.
                //if(containerModel.entries[0].type == "C")
                if (containerModel.entries[0].ccy != containerModel.entries[1].ccy && containerModel.entries[0].ccy != phpID)
                    goModel.Entry11InterRate = (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : "";//Replace with proper interate default.
                goModel.Entry11InterAmt = "";//Replace with proper interamt default.

                if (containerModel.entries.Count > 1)
                {
                    goModel.Entry12Type = containerModel.entries[1].type;
                    goModel.Entry12Ccy = GetCurrencyAbbrv(containerModel.entries[1].ccy);
                    goModel.Entry12Amt = containerModel.entries[1].amount.ToString();

                    var entry12Account = getAccount(containerModel.entries[1].account);
                    goModel.Entry12Cust = entry12Account.Account_Cust;
                    goModel.Entry12Actcde = entry12Account.Account_Code;
                    goModel.Entry12ActType = entry12Account.Account_No.Substring(0, 3);
                    goModel.Entry12ActNo = entry12Account.Account_No.Substring(Math.Max(0, entry12Account.Account_No.Length - 6));
                    goModel.Entry12ExchRate = containerModel.entries[1].interate == 0 ? "" : containerModel.entries[1].interate.ToString();
                    goModel.Entry12ExchCcy = (containerModel.entries[1].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[1].contraCcy) : "";
                    goModel.Entry12Fund = (entry12Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry12Available = "";//Replace with proper available default.
                    goModel.Entry12Details = "";//Replace with proper details default.
                    goModel.Entry12Entity = "010";//Replace with proper entity default.
                    goModel.Entry12Division = entry12Account.Account_Div;//Replace with proper division default.
                    //if (containerModel.entries[1].type == "C")
                    if ((containerModel.entries[0].ccy != containerModel.entries[1].ccy) && containerModel.entries[1].ccy != phpID)
                        goModel.Entry12InterRate = (containerModel.entries[1].interate > 0) ? containerModel.entries[1].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry12InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 2)
                {
                    goModel.Entry21Type = containerModel.entries[2].type;
                    goModel.Entry21Ccy = GetCurrencyAbbrv(containerModel.entries[2].ccy);
                    goModel.Entry21Amt = containerModel.entries[2].amount.ToString();

                    var entry21Account = getAccount(containerModel.entries[2].account);
                    goModel.Entry21Cust = entry21Account.Account_Cust;
                    goModel.Entry21Actcde = entry21Account.Account_Code;
                    goModel.Entry21ActType = entry21Account.Account_No.Substring(0, 3);
                    goModel.Entry21ActNo = entry21Account.Account_No.Substring(Math.Max(0, entry21Account.Account_No.Length - 6));
                    goModel.Entry21ExchRate = containerModel.entries[2].interate == 0 ? "" : containerModel.entries[2].interate.ToString();
                    goModel.Entry21ExchCcy = (containerModel.entries[2].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[2].contraCcy) : "";
                    goModel.Entry21Fund = (entry21Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry21Available = "";//Replace with proper available default.
                    goModel.Entry21Details = "";//Replace with proper details default.
                    goModel.Entry21Entity = "010";//Replace with proper entity default.
                    goModel.Entry21Division = entry21Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[2].type == "C")
                        goModel.Entry21InterRate = (containerModel.entries[2].interate > 0) ? containerModel.entries[2].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry21InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 3)
                {
                    goModel.Entry22Type = containerModel.entries[3].type;
                    goModel.Entry22Ccy = GetCurrencyAbbrv(containerModel.entries[3].ccy);
                    goModel.Entry22Amt = containerModel.entries[3].amount.ToString();

                    var entry22Account = getAccount(containerModel.entries[3].account);
                    goModel.Entry22Cust = entry22Account.Account_Cust;
                    goModel.Entry22Actcde = entry22Account.Account_Code;
                    goModel.Entry22ActType = entry22Account.Account_No.Substring(0, 3);
                    goModel.Entry22ActNo = entry22Account.Account_No.Substring(Math.Max(0, entry22Account.Account_No.Length - 6));
                    goModel.Entry22ExchRate = containerModel.entries[3].interate == 0 ? "" : containerModel.entries[3].interate.ToString();
                    goModel.Entry22ExchCcy = (containerModel.entries[3].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[3].contraCcy) : "";
                    goModel.Entry22Fund = (entry22Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry22Available = "";//Replace with proper available default.
                    goModel.Entry22Details = "";//Replace with proper details default.
                    goModel.Entry22Entity = "010";//Replace with proper entity default.
                    goModel.Entry22Division = entry22Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[3].type == "C")
                        goModel.Entry22InterRate = (containerModel.entries[3].interate > 0) ? containerModel.entries[3].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry22InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 4)
                {
                    goModel.Entry31Type = containerModel.entries[4].type;
                    goModel.Entry31Ccy = GetCurrencyAbbrv(containerModel.entries[4].ccy);
                    goModel.Entry31Amt = containerModel.entries[4].amount.ToString();

                    var entry31Account = getAccount(containerModel.entries[4].account);
                    goModel.Entry31Cust = entry31Account.Account_Cust;
                    goModel.Entry31Actcde = entry31Account.Account_Code;
                    goModel.Entry31ActType = entry31Account.Account_No.Substring(0, 3);
                    goModel.Entry31ActNo = entry31Account.Account_No.Substring(Math.Max(0, entry31Account.Account_No.Length - 6));
                    goModel.Entry31ExchRate = containerModel.entries[4].interate == 0 ? "" : containerModel.entries[4].interate.ToString();
                    goModel.Entry31ExchCcy = (containerModel.entries[4].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[4].contraCcy) : "";
                    goModel.Entry31Fund = (entry31Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry31Available = "";//Replace with proper available default.
                    goModel.Entry31Details = "";//Replace with proper details default.
                    goModel.Entry31Entity = "010";//Replace with proper entity default.
                    goModel.Entry31Division = entry31Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[4].type == "C")
                        goModel.Entry31InterRate = (containerModel.entries[4].interate > 0) ? containerModel.entries[4].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry31InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 5)
                {
                    goModel.Entry32Type = containerModel.entries[5].type;
                    goModel.Entry32Ccy = GetCurrencyAbbrv(containerModel.entries[5].ccy);
                    goModel.Entry32Amt = containerModel.entries[5].amount.ToString();

                    var entry32Account = getAccount(containerModel.entries[5].account);
                    goModel.Entry32Cust = entry32Account.Account_Cust;
                    goModel.Entry32Actcde = entry32Account.Account_Code;
                    goModel.Entry32ActType = entry32Account.Account_No.Substring(0, 3);
                    goModel.Entry32ActNo = entry32Account.Account_No.Substring(Math.Max(0, entry32Account.Account_No.Length - 6));
                    goModel.Entry32ExchRate = containerModel.entries[5].interate == 0 ? "" : containerModel.entries[5].interate.ToString();
                    goModel.Entry32ExchCcy = (containerModel.entries[5].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[5].contraCcy) : "";
                    goModel.Entry32Fund = (entry32Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry32Available = "";//Replace with proper available default.
                    goModel.Entry32Details = "";//Replace with proper details default.
                    goModel.Entry32Entity = "010";//Replace with proper entity default.
                    goModel.Entry32Division = entry32Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[5].type == "C")
                        goModel.Entry32InterRate = (containerModel.entries[5].interate > 0) ? containerModel.entries[5].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry32InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 6)
                {
                    goModel.Entry41Type = containerModel.entries[6].type;
                    goModel.Entry41Ccy = GetCurrencyAbbrv(containerModel.entries[6].ccy);
                    goModel.Entry41Amt = containerModel.entries[6].amount.ToString();

                    var entry41Account = getAccount(containerModel.entries[6].account);
                    goModel.Entry41Cust = entry41Account.Account_Cust;
                    goModel.Entry41Actcde = entry41Account.Account_Code;
                    goModel.Entry41ActType = entry41Account.Account_No.Substring(0, 3);
                    goModel.Entry41ActNo = entry41Account.Account_No.Substring(Math.Max(0, entry41Account.Account_No.Length - 6));
                    goModel.Entry41ExchRate = containerModel.entries[6].interate == 0 ? "" : containerModel.entries[6].interate.ToString();
                    goModel.Entry41ExchCcy = (containerModel.entries[6].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[6].contraCcy) : "";
                    goModel.Entry41Fund = (entry41Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry41Available = "";//Replace with proper available default.
                    goModel.Entry41Details = "";//Replace with proper details default.
                    goModel.Entry41Entity = "010";//Replace with proper entity default.
                    goModel.Entry41Division = entry41Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[6].type == "C")
                        goModel.Entry41InterRate = (containerModel.entries[6].interate > 0) ? containerModel.entries[6].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry41InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 7)
                {
                    goModel.Entry42Type = containerModel.entries[7].type;
                    goModel.Entry42Ccy = GetCurrencyAbbrv(containerModel.entries[7].ccy);
                    goModel.Entry42Amt = containerModel.entries[7].amount.ToString();

                    var entry42Account = getAccount(containerModel.entries[7].account);
                    goModel.Entry42Cust = entry42Account.Account_Cust;
                    goModel.Entry42Actcde = entry42Account.Account_Code;
                    goModel.Entry42ActType = entry42Account.Account_No.Substring(0, 3);
                    goModel.Entry42ActNo = entry42Account.Account_No.Substring(Math.Max(0, entry42Account.Account_No.Length - 6));
                    goModel.Entry42ExchRate = containerModel.entries[7].interate == 0 ? "" : containerModel.entries[7].interate.ToString();
                    goModel.Entry42ExchCcy = (containerModel.entries[7].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[7].contraCcy) : "";
                    goModel.Entry42Fund = (entry42Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry42Available = "";//Replace with proper available default.
                    goModel.Entry42Details = "";//Replace with proper details default.
                    goModel.Entry42Entity = "010";//Replace with proper entity default.
                    goModel.Entry42Division = entry42Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[7].type == "C")
                        goModel.Entry42InterRate = (containerModel.entries[7].interate > 0) ? containerModel.entries[7].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry42InterAmt = "";//Replace with proper interamt default.
                }
            }
            else
            {
                return goModel;
            }

            _GOContext.TblCm10.Add(goModel);

            //_context.GOExpressHist.Add(convertTblCm10ToGOExHist(goModel, expenseID, expDtlID));

            return goModel;
        }
        private TblCm10 InsertGbaseEntryNonCash(gbaseContainer containerModel, int expenseID, int userID, bool sameCurr)
        {
            TblCm10 goModel = new TblCm10();

            var user = _context.User.FirstOrDefault(x => x.User_ID == userID);
            var phpID = getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_PHP").Value)).Curr_ID;

            //goModel.Id = -1;
            goModel.SystemName = "EXPRESS";
            goModel.Branchno = getAccount(containerModel.entries[0].account).Account_No.Substring(4, 3);
            goModel.AutoApproved = "Y";
            goModel.ValueDate = DateTime.Now.ToString("MMddyy");
            goModel.Section = "10";
            goModel.WarningOverride = "Y";
            goModel.Remarks = containerModel.remarks;
            goModel.MakerEmpno = user.User_EmpCode; //Replace with user ID later when user module is finished.
            goModel.Empno = user.User_EmpCode.Substring(2);  //Replace with user ID later when user module is finished.
            goModel.Recstatus = "READY";
            goModel.Datestamp = DateTime.Now;
            goModel.Timerespond = DateTime.Now;
            goModel.Timesent = DateTime.Now;

            if (containerModel.entries.Count > 0)
            {
                goModel.Entry11Type = containerModel.entries[0].type;
                goModel.Entry11Ccy = GetCurrencyAbbrv(containerModel.entries[0].ccy);
                goModel.Entry11Amt = containerModel.entries[0].amount.ToString();

                var entry11Account = getAccount(containerModel.entries[0].account);
                goModel.Entry11Cust = entry11Account.Account_Cust;
                goModel.Entry11Actcde = entry11Account.Account_Code;
                goModel.Entry11ActType = entry11Account.Account_No.Substring(0, 3);
                goModel.Entry11ActNo = entry11Account.Account_No.Substring(Math.Max(0, entry11Account.Account_No.Length - 6));
                goModel.Entry11ExchRate = (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : (containerModel.entries[1].interate > 0) ? containerModel.entries[1].interate.ToString() : "";
                goModel.Entry11ExchCcy = (containerModel.entries[0].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[0].contraCcy) : "";
                goModel.Entry11Fund = (entry11Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                goModel.Entry11Available = "";//Replace with proper available default.
                goModel.Entry11Details = "";//Replace with proper details default.
                goModel.Entry11Entity = "010";//Replace with proper entity default.
                goModel.Entry11Division = entry11Account.Account_Div;//Replace with proper division default.
                if ((!sameCurr) && (containerModel.entries[0].ccy != phpID))
                    goModel.Entry11InterRate = (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : (containerModel.entries[1].interate > 0) ? containerModel.entries[1].interate.ToString() : "";
                goModel.Entry11InterAmt = "";//Replace with proper interamt default.

                if (containerModel.entries.Count > 1)
                {
                    goModel.Entry12Type = containerModel.entries[1].type;
                    goModel.Entry12Ccy = GetCurrencyAbbrv(containerModel.entries[1].ccy);
                    goModel.Entry12Amt = containerModel.entries[1].amount.ToString();

                    var entry12Account = getAccount(containerModel.entries[1].account);
                    goModel.Entry12Cust = entry12Account.Account_Cust;
                    goModel.Entry12Actcde = entry12Account.Account_Code;
                    goModel.Entry12ActType = entry12Account.Account_No.Substring(0, 3);
                    goModel.Entry12ActNo = entry12Account.Account_No.Substring(Math.Max(0, entry12Account.Account_No.Length - 6));
                    goModel.Entry12ExchRate = (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : (containerModel.entries[1].interate > 0) ? containerModel.entries[1].interate.ToString() : "";
                    goModel.Entry12ExchCcy = (containerModel.entries[1].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[1].contraCcy) : "";
                    goModel.Entry12Fund = (entry12Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry12Available = "";//Replace with proper available default.
                    goModel.Entry12Details = "";//Replace with proper details default.
                    goModel.Entry12Entity = "010";//Replace with proper entity default.
                    goModel.Entry12Division = entry12Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[1].ccy != phpID))
                        goModel.Entry12InterRate = (containerModel.entries[1].interate > 0) ? containerModel.entries[1].interate.ToString() : (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : "";
                    goModel.Entry12InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 2)
                {
                    goModel.Entry21Type = containerModel.entries[2].type;
                    goModel.Entry21Ccy = GetCurrencyAbbrv(containerModel.entries[2].ccy);
                    goModel.Entry21Amt = containerModel.entries[2].amount.ToString();

                    var entry21Account = getAccount(containerModel.entries[2].account);
                    goModel.Entry21Cust = entry21Account.Account_Cust;
                    goModel.Entry21Actcde = entry21Account.Account_Code;
                    goModel.Entry21ActType = entry21Account.Account_No.Substring(0, 3);
                    goModel.Entry21ActNo = entry21Account.Account_No.Substring(Math.Max(0, entry21Account.Account_No.Length - 6));
                    goModel.Entry21ExchRate = containerModel.entries[2].interate == 0 ? "" : containerModel.entries[2].interate.ToString();
                    goModel.Entry21ExchCcy = (containerModel.entries[2].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[2].contraCcy) : "";
                    goModel.Entry21Fund = (entry21Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry21Available = "";//Replace with proper available default.
                    goModel.Entry21Details = "";//Replace with proper details default.
                    goModel.Entry21Entity = "010";//Replace with proper entity default.
                    goModel.Entry21Division = entry21Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[2].ccy != phpID))
                        goModel.Entry21InterRate = (containerModel.entries[2].interate > 0) ? containerModel.entries[2].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry21InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 3)
                {
                    goModel.Entry22Type = containerModel.entries[3].type;
                    goModel.Entry22Ccy = GetCurrencyAbbrv(containerModel.entries[3].ccy);
                    goModel.Entry22Amt = containerModel.entries[3].amount.ToString();

                    var entry22Account = getAccount(containerModel.entries[3].account);
                    goModel.Entry22Cust = entry22Account.Account_Cust;
                    goModel.Entry22Actcde = entry22Account.Account_Code;
                    goModel.Entry22ActType = entry22Account.Account_No.Substring(0, 3);
                    goModel.Entry22ActNo = entry22Account.Account_No.Substring(Math.Max(0, entry22Account.Account_No.Length - 6));
                    goModel.Entry22ExchRate = containerModel.entries[3].interate == 0 ? "" : containerModel.entries[3].interate.ToString();
                    goModel.Entry22ExchCcy = (containerModel.entries[3].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[3].contraCcy) : "";
                    goModel.Entry22Fund = (entry22Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry22Available = "";//Replace with proper available default.
                    goModel.Entry22Details = "";//Replace with proper details default.
                    goModel.Entry22Entity = "010";//Replace with proper entity default.
                    goModel.Entry22Division = entry22Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[3].ccy != phpID))
                        goModel.Entry22InterRate = (containerModel.entries[3].interate > 0) ? containerModel.entries[3].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry22InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 4)
                {
                    goModel.Entry31Type = containerModel.entries[4].type;
                    goModel.Entry31Ccy = GetCurrencyAbbrv(containerModel.entries[4].ccy);
                    goModel.Entry31Amt = containerModel.entries[4].amount.ToString();

                    var entry31Account = getAccount(containerModel.entries[4].account);
                    goModel.Entry31Cust = entry31Account.Account_Cust;
                    goModel.Entry31Actcde = entry31Account.Account_Code;
                    goModel.Entry31ActType = entry31Account.Account_No.Substring(0, 3);
                    goModel.Entry31ActNo = entry31Account.Account_No.Substring(Math.Max(0, entry31Account.Account_No.Length - 6));
                    goModel.Entry31ExchRate = containerModel.entries[4].interate == 0 ? "" : containerModel.entries[4].interate.ToString();
                    goModel.Entry31ExchCcy = (containerModel.entries[4].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[4].contraCcy) : "";
                    goModel.Entry31Fund = (entry31Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry31Available = "";//Replace with proper available default.
                    goModel.Entry31Details = "";//Replace with proper details default.
                    goModel.Entry31Entity = "010";//Replace with proper entity default.
                    goModel.Entry31Division = entry31Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[4].ccy != phpID))
                        goModel.Entry31InterRate = (containerModel.entries[4].interate > 0) ? containerModel.entries[4].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry31InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 5)
                {
                    goModel.Entry32Type = containerModel.entries[5].type;
                    goModel.Entry32Ccy = GetCurrencyAbbrv(containerModel.entries[5].ccy);
                    goModel.Entry32Amt = containerModel.entries[5].amount.ToString();

                    var entry32Account = getAccount(containerModel.entries[5].account);
                    goModel.Entry32Cust = entry32Account.Account_Cust;
                    goModel.Entry32Actcde = entry32Account.Account_Code;
                    goModel.Entry32ActType = entry32Account.Account_No.Substring(0, 3);
                    goModel.Entry32ActNo = entry32Account.Account_No.Substring(Math.Max(0, entry32Account.Account_No.Length - 6));
                    goModel.Entry32ExchRate = containerModel.entries[5].interate == 0 ? "" : containerModel.entries[5].interate.ToString();
                    goModel.Entry32ExchCcy = (containerModel.entries[5].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[5].contraCcy) : "";
                    goModel.Entry32Fund = (entry32Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry32Available = "";//Replace with proper available default.
                    goModel.Entry32Details = "";//Replace with proper details default.
                    goModel.Entry32Entity = "010";//Replace with proper entity default.
                    goModel.Entry32Division = entry32Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[5].ccy != phpID))
                        goModel.Entry32InterRate = (containerModel.entries[5].interate > 0) ? containerModel.entries[5].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry32InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 6)
                {
                    goModel.Entry41Type = containerModel.entries[6].type;
                    goModel.Entry41Ccy = GetCurrencyAbbrv(containerModel.entries[6].ccy);
                    goModel.Entry41Amt = containerModel.entries[6].amount.ToString();

                    var entry41Account = getAccount(containerModel.entries[6].account);
                    goModel.Entry41Cust = entry41Account.Account_Cust;
                    goModel.Entry41Actcde = entry41Account.Account_Code;
                    goModel.Entry41ActType = entry41Account.Account_No.Substring(0, 3);
                    goModel.Entry41ActNo = entry41Account.Account_No.Substring(Math.Max(0, entry41Account.Account_No.Length - 6));
                    goModel.Entry41ExchRate = containerModel.entries[6].interate == 0 ? "" : containerModel.entries[6].interate.ToString();
                    goModel.Entry41ExchCcy = (containerModel.entries[6].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[6].contraCcy) : "";
                    goModel.Entry41Fund = (entry41Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry41Available = "";//Replace with proper available default.
                    goModel.Entry41Details = "";//Replace with proper details default.
                    goModel.Entry41Entity = "010";//Replace with proper entity default.
                    goModel.Entry41Division = entry41Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[6].ccy != phpID))
                        goModel.Entry41InterRate = (containerModel.entries[6].interate > 0) ? containerModel.entries[6].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry41InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 7)
                {
                    goModel.Entry42Type = containerModel.entries[7].type;
                    goModel.Entry42Ccy = GetCurrencyAbbrv(containerModel.entries[7].ccy);
                    goModel.Entry42Amt = containerModel.entries[7].amount.ToString();

                    var entry42Account = getAccount(containerModel.entries[7].account);
                    goModel.Entry42Cust = entry42Account.Account_Cust;
                    goModel.Entry42Actcde = entry42Account.Account_Code;
                    goModel.Entry42ActType = entry42Account.Account_No.Substring(0, 3);
                    goModel.Entry42ActNo = entry42Account.Account_No.Substring(Math.Max(0, entry42Account.Account_No.Length - 6));
                    goModel.Entry42ExchRate = containerModel.entries[7].interate == 0 ? "" : containerModel.entries[7].interate.ToString();
                    goModel.Entry42ExchCcy = (containerModel.entries[7].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[7].contraCcy) : "";
                    goModel.Entry42Fund = (entry42Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry42Available = "";//Replace with proper available default.
                    goModel.Entry42Details = "";//Replace with proper details default.
                    goModel.Entry42Entity = "010";//Replace with proper entity default.
                    goModel.Entry42Division = entry42Account.Account_Div;//Replace with proper division default.
                    if ((!sameCurr) && (containerModel.entries[7].ccy != phpID))
                        goModel.Entry42InterRate = (containerModel.entries[7].interate > 0) ? containerModel.entries[7].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry42InterAmt = "";//Replace with proper interamt default.
                }
            }
            else
            {
                return goModel;
            }

            _GOContext.TblCm10.Add(goModel);

            //_context.GOExpressHist.Add(convertTblCm10ToGOExHist(goModel, expenseID, expDtlID));

            return goModel;
        }
        ///===============[End Gbase Entry Section]=================
        ///==============[Begin Print Status Section]===============
        public bool updatePrintStatus(int docID, int entryID)
        {
            try
            {
                var rec = _context.PrintStatus.Where(x => x.PS_EntryID == entryID).ToList();
                if (rec != null)
                {
                    switch (docID)
                    {
                        //GlobalSystemValues.PS_LOI == 1
                        case 1:
                            foreach (var i in rec)
                            {
                                i.PS_LOI = true;
                                _context.Entry(i).State = EntityState.Modified;
                            }
                            break;
                        //GlobalSystemValues.PS_BIR2307 == 2
                        case 2:
                            foreach (var i in rec)
                            {
                                i.PS_BIR2307 = true;
                                _context.Entry(i).State = EntityState.Modified;
                            }
                            break;
                        //GlobalSystemValues.PS_CDD == 3
                        case 3:
                            foreach (var i in rec)
                            {
                                i.PS_CDD = true;
                                _context.Entry(i).State = EntityState.Modified;
                            }
                            break;
                        //GlobalSystemValues.PS_Check == 4
                        case 4:
                            foreach (var i in rec)
                            {
                                i.PS_Check = true;
                                _context.Entry(i).State = EntityState.Modified;
                            }
                            break;
                        //GlobalSystemValues.PS_Voucher == 5
                        case 5:
                            foreach (var i in rec)
                            {
                                i.PS_Voucher = true;
                                _context.Entry(i).State = EntityState.Modified;
                            }
                            break;
                        //
                        default:
                            break;
                    }
                    if (_modelState.IsValid)
                    {
                        //_context.Entry(rec).State = EntityState.Modified;
                        _context.SaveChanges();
                        updatePrintStatusForCLosing(entryID);
                    }
                }
            }
            catch (Exception e)
            {
                //redirect to Error Screen
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        ///===============[End Print Status Section]================
        ///============[Other Functions]============
        //get fbt id
        public int getFbt(int id)
        {
            return _context.DMFBT.FirstOrDefault(x => x.FBT_MasterID == id && x.FBT_isActive == true && x.FBT_isDeleted == false).FBT_ID;
        }
        public string getFbtFormula(int id)
        {
            return _context.DMFBT.FirstOrDefault(x => x.FBT_ID == id).FBT_Formula;
        }
        //get currency
        public DMCurrencyModel getCurrency(int id)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_ID == id);
        }
        //get currency master id
        public int getCurrencyMasterID(int id)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_ID == id).Curr_MasterID;
        }
        //get currency master id
        public int getMasterID(int id)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_ID == id).Curr_MasterID;
        }
        //get xml currency details
        public List<CONSTANT_CCY_VALS> getXMLCurrency()
        {
            List<CONSTANT_CCY_VALS> valList = new List<CONSTANT_CCY_VALS>();
            InterEntityValues interEntityValues = new InterEntityValues();
            List<string> consCurrMasterId = new List<string> {
                InterEntityValues.Currency_US,
                InterEntityValues.Currency_Yen,
                InterEntityValues.Currency_PHP
            };
            foreach (string mId in consCurrMasterId)
            {
                var acc = _context.DMCurrency.Where(x => (x.Curr_MasterID == int.Parse(mId))
                                                    && x.Curr_isActive == true && x.Curr_isDeleted == false).FirstOrDefault();
                CONSTANT_CCY_VALS vals = new CONSTANT_CCY_VALS
                {
                    currID = acc.Curr_ID,
                    currMasterID = acc.Curr_MasterID,
                    currABBR = acc.Curr_CCY_ABBR
                };
                valList.Add(vals);
            }
            return valList;
        }
        //get specific xml currency details
        public CONSTANT_CCY_VALS getXMLCurrency(string type)
        {
            var mId = "";
            InterEntityValues interEntityValues = new InterEntityValues();
            if (type == "USD")
            {
                mId = InterEntityValues.Currency_US;
            }
            else if (type == "YEN")
            {
                mId = InterEntityValues.Currency_Yen;
            }
            else if (type == "PHP")
            {
                mId = InterEntityValues.Currency_PHP;
            }
            var acc = _context.DMCurrency.Where(x => (x.Curr_MasterID == int.Parse(mId))
                                                && x.Curr_isActive == true && x.Curr_isDeleted == false).FirstOrDefault();
            return new CONSTANT_CCY_VALS
            {
                currID = acc.Curr_ID,
                currMasterID = acc.Curr_MasterID,
                currABBR = acc.Curr_CCY_ABBR
            };
        }
        public List<CONSTANT_NC_VALS> getNCAccs(string Loc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("wwwroot/xml/NonCashAccounts.xml");
            //var xLSPayroll = xelem.Element("LSPAYROLL").Value;
            XmlNodeList nodeList = doc.SelectNodes(Loc);
            List<CONSTANT_NC_VALS> valList = new List<CONSTANT_NC_VALS>();
            foreach (XmlNode no in nodeList)
            {
                var rawVal = no.InnerText;
                var acc = _context.DMAccount.Where(x => (x.Account_MasterID == int.Parse(rawVal))
                                                    && x.Account_isActive == true && x.Account_isDeleted == false).FirstOrDefault();
                CONSTANT_NC_VALS vals = new CONSTANT_NC_VALS
                {
                    accID = acc.Account_ID,
                    accNo = acc.Account_No,
                    accName = acc.Account_Name
                };
                valList.Add(vals);
            }
            return valList;
        }
        public List<CONSTANT_NC_VALS> getNCAccsForFilter()
        {
            var acc_pettyCash = getNCAccs("/NONCASHACCOUNTS/PCR/ACCOUNT[@class='entry1' and @id='ACCOUNT1']");
            var acc_computerSus = getNCAccs("/NONCASHACCOUNTS/PCR/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            var acc_computerSusUSD = getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLL/ACCOUNT[@class='entry1' and @id='ACCOUNT1']");
            var acc_interRF = getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLL/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            var acc_interFR = getNCAccs("/NONCASHACCOUNTS/JSPAYROLL/ACCOUNT[@class='entry3' and @id='ACCOUNT1']");
            var acc_ewtTax = getNCAccs("/NONCASHACCOUNTS/PSSC/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            //Populate Constant NC Accounts
            List<CONSTANT_NC_VALS> valList = new List<CONSTANT_NC_VALS>
            {
                new CONSTANT_NC_VALS()
                {
                    accID = acc_computerSus.FirstOrDefault().accID,
                    accNo = acc_computerSus.FirstOrDefault().accNo+"",
                    accName = acc_computerSus.FirstOrDefault().accName
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_computerSusUSD.FirstOrDefault().accID,
                    accNo = acc_computerSusUSD.FirstOrDefault().accNo+"",
                    accName = acc_computerSusUSD.FirstOrDefault().accName
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_interRF.FirstOrDefault().accID,
                    accNo = acc_interRF.FirstOrDefault().accNo+"",
                    accName = acc_interRF.FirstOrDefault().accName
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_interFR.FirstOrDefault().accID,
                    accNo = acc_interFR.FirstOrDefault().accNo+"",
                    accName = acc_interFR.FirstOrDefault().accName
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_pettyCash.FirstOrDefault().accID,
                    accNo = acc_pettyCash.FirstOrDefault().accNo+"",
                    accName = acc_pettyCash.FirstOrDefault().accName
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_ewtTax.FirstOrDefault().accID,
                    accNo = acc_ewtTax.FirstOrDefault().accNo+"",
                    accName = acc_ewtTax.FirstOrDefault().accName
                }
            };
            return valList;
        }
        //get vendor
        public DMVendorModel getVendor(int id)
        {
            return _context.DMVendor.FirstOrDefault(x => x.Vendor_ID == id);
        }
        //get vendor
        public DMEmpModel getEmployee(int id)
        {
            return _context.DMEmp.FirstOrDefault(x => x.Emp_ID == id);
        }
        //get vendor list
        public List<DMVendorModel> getVendorList()
        {
            return _context.DMVendor.Where(x => x.Vendor_isActive == true && x.Vendor_isDeleted == false)
                                        .OrderBy(x => x.Vendor_Name).ToList();
        }
        //get all vendor list
        public List<DMVendorModel> getAllVendorList()
        {
            return _context.DMVendor.OrderBy(x => x.Vendor_Name).ToList();
        }
        //get account
        public DMAccountModel getAccount(int id)
        {
            return _context.DMAccount.FirstOrDefault(x => x.Account_ID == id);
        }
        //get account name
        public string GetAccountName(int id)
        {
            return _context.DMAccount.Where(x => x.Account_ID == id).Single().Account_Name;
        }
        //Get lastest account by its account master ID.
        public DMAccountModel getAccountByMasterID(int masterID)
        {
            return _context.DMAccount.Where(x => x.Account_MasterID == masterID && x.Account_isActive == true
                    && x.Account_isDeleted == false).OrderBy(x => x.Account_Name).FirstOrDefault();
        }
        public int getAccountID(string accountNo)
        {
            return _context.DMAccount.Where(x => x.Account_No.Replace("-", "") == accountNo.Replace("-", "").Substring(0, 12) && x.Account_isActive == true
            && x.Account_isDeleted == false).FirstOrDefault().Account_ID;
        }
        public List<accDetails> getAccDetailsEntry()
        {
            List<accDetails> accDetails = new List<accDetails>();

            var accDbDetails = _context.DMAccount.Where(x => x.Account_isDeleted == false && x.Account_isActive == true).OrderByDescending(x => x.Account_No.Contains("H90")).ThenBy(x => x.Account_No).Select(q => new { q.Account_ID, q.Account_Name, q.Account_No, q.Account_Code });

            foreach (var detail in accDbDetails)
            {
                accDetails temp = new accDetails();
                temp.accId = detail.Account_ID;
                temp.accName = detail.Account_No + " - " + detail.Account_Name;
                temp.accCode = detail.Account_Code;

                accDetails.Add(temp);
            }

            return accDetails;
        }
        public List<accDetails> getAccDetailsEntry(int account)
        {
            List<accDetails> accDetails = new List<accDetails>();

            var accDbDetails = _context.DMAccount.Where(x => x.Account_ID == account).Select(q => new { q.Account_ID, q.Account_Name, q.Account_Code });

            foreach (var detail in accDbDetails)
            {
                accDetails temp = new accDetails();
                temp.accId = detail.Account_ID;
                temp.accName = detail.Account_Name;
                temp.accCode = detail.Account_Code;

                accDetails.Add(temp);
            }

            return accDetails;
        }

        public int getAccIDByMasterID(int mID)
        {
            var detail = _context.DMAccount.Where(x => x.Account_MasterID == mID && x.Account_isActive == true).Select(q => new { q.Account_ID, q.Account_Name, q.Account_Code }).FirstOrDefault();
            return detail.Account_ID;
        }
        //Account list only Active and not deleted
        public List<DMAccountModel> getAccountList()
        {
            List<DMAccountModel> accList = new List<DMAccountModel>();
            var acc = _context.DMAccount.Where(x => x.Account_isActive == true
                        && x.Account_isDeleted == false).ToList().OrderByDescending(x => x.Account_No.Contains("H90")).ThenBy(x => x.Account_No);
            foreach (var i in acc)
            {
                accList.Add(new DMAccountModel
                {
                    Account_ID = i.Account_ID,
                    Account_MasterID = i.Account_MasterID,
                    Account_Name = i.Account_No + " - " + i.Account_Name,
                    Account_No = i.Account_No,
                    Account_Code = i.Account_Code
                });
            }

            return accList;
        }
        //Account list including history
        public List<DMAccountModel> getAccountListIncHist()
        {
            return _context.DMAccount.OrderByDescending(x => x.Account_No.Contains("H90")).ThenBy(x => x.Account_No).ToList();
        }
        //get dept name
        public string GetDeptName(int id)
        {
            return _context.DMDept.Where(x => x.Dept_ID == id).First().Dept_Name;
        }
        //get Status
        public string getStatus(int id)
        {
            var status = _context.StatusList.SingleOrDefault(q => q.Status_ID == id);
            return status.Status_Name;
        }
        //get userName
        public string getUserName(int id)
        {
            var name = _context.User.SingleOrDefault(q => q.User_ID == id);

            if (name == null)
            {
                return null;
            }

            return name.User_UserName;
        }
        //get userName
        public string getName(int id)
        {
            var name = _context.User.SingleOrDefault(q => q.User_ID == id);

            if (name == null)
            {
                return null;
            }

            return name.User_LName + ", " + name.User_FName;
        }
        public List<UserModel> getAllUsers()
        {
            return _context.User.ToList();
        }
        //get bcs name
        public string getBCSName(int id)
        {
            var name = _context.DMBCS.Where(q => q.BCS_ID == id).Join(_context.User, b => b.BCS_User_ID,
                e => e.User_ID, (b, e) => new DMBCSViewModel
                { BCS_Name = e.User_FName + " " + e.User_LName }).SingleOrDefault();

            if (name == null)
            {
                return null;
            }

            return name.BCS_Name.ToUpper();
        }
        //get vat value
        public decimal getVat()
        {
            var vat = _context.DMVAT.FirstOrDefault(q => q.VAT_isActive == true);

            if (vat == null)
            {
                return 0;
            }

            return (decimal)vat.VAT_Rate;
        }
        public decimal getVat(int id)
        {
            var vat = _context.DMVAT.Where(x => x.VAT_ID == id).FirstOrDefault();
            if (vat == null)
            {
                return 0;
            }
            return (decimal)vat.VAT_Rate;
        }
        //get EWT(Tax Rate) value
        public float GetEWTValue(int id)
        {
            return _context.DMTR.Where(x => x.TR_ID == id).First().TR_Tax_Rate;
        }
        public DMTRModel GetEWT(int id)
        {
            return _context.DMTR.Where(x => x.TR_ID == id).FirstOrDefault();
        }
        //get currency abbreviation
        public string GetCurrencyAbbrv(int id)
        {
            return (id != 0) ? _context.DMCurrency.Where(x => x.Curr_ID == id).First().Curr_CCY_ABBR : "PHP";
        }
        //Get lastest currency by its currency master ID.
        public DMCurrencyModel getCurrencyByMasterID(int masterID)
        {
            return _context.DMCurrency.Where(x => x.Curr_MasterID == masterID && x.Curr_isActive == true
                    && x.Curr_isDeleted == false).FirstOrDefault();
        }
        public DMCurrencyModel getCurrencyByAbbrev(string abbrev)
        {
            return _context.DMCurrency.Where(x => x.Curr_CCY_ABBR == abbrev).FirstOrDefault();
        }
        //Get lastest currency by its currency master ID.
        public DMCurrencyModel getCurrencyByID(int currID)
        {
            return _context.DMCurrency.Where(x => x.Curr_ID == currID && x.Curr_isActive == true
                    && x.Curr_isDeleted == false).FirstOrDefault();
        }
        //get Tax Rate list for specific vendor in List<T>
        public List<DMTRModel> getVendorTaxList(int vendorMasterID)
        {
            return (from vendTr in _context.DMVendorTRVAT
                    join tr in _context.DMTR on vendTr.VTV_TR_ID equals tr.TR_MasterID
                    where tr.TR_isActive == true && tr.TR_isDeleted == false
                        && vendTr.VTV_Vendor_ID == vendorMasterID
                    select new DMTRModel
                    {
                        TR_ID = tr.TR_ID,
                        TR_MasterID = tr.TR_MasterID,
                        TR_Tax_Rate = Mizuho.round(tr.TR_Tax_Rate * 100, 2),
                        TR_WT_Title = tr.TR_WT_Title
                    }).ToList();
        }
        //get Tax Rate list for specific user
        public SelectList getVendorTaxRate(int vendorID)
        {
            var vendorMasterID = _context.DMVendor.Where(x => x.Vendor_ID == vendorID).Select(id => id.Vendor_MasterID).FirstOrDefault();
            var vendorTRIDList = _context.DMVendorTRVAT.Where(x => x.VTV_Vendor_ID == vendorMasterID
                                                                && x.VTV_TR_ID > 0)
                                                       .OrderBy(x => x.VTV_TR_ID)
                                                       .Select(q => q.VTV_TR_ID).ToList();

            var select = new SelectList(_context.DMTR.Where(x => vendorTRIDList.Contains(x.TR_MasterID)
                                                            && x.TR_isActive == true
                                                            && x.TR_isDeleted == false)
                                                     .OrderBy(x => x.TR_Tax_Rate)
                                                     .Select(q => new { q.TR_ID, TR_Tax_Rate = Mizuho.round((q.TR_Tax_Rate * 100), 2) }),
                        "TR_ID", "TR_Tax_Rate");

            return select;
        }
        //get Tax Rate list for specific user
        public SelectList getAllTaxRate()
        {
            var select = new SelectList(_context.DMTR.Where(x => x.TR_isActive == true
                                                            && x.TR_isDeleted == false)
                                                     .OrderBy(x => x.TR_Tax_Rate)
                                                     .Select(q => new { q.TR_ID, TR_Tax_Rate = Mizuho.round((q.TR_Tax_Rate * 100), 2) }),
                        "TR_ID", "TR_Tax_Rate");

            return select;
        }
        //get all Tax Rate including history
        public List<DMTRModel> getAllTRList()
        {
            return _context.DMTR.OrderBy(x => x.TR_Tax_Rate).Select(x => new DMTRModel
            {
                TR_ID = x.TR_ID,
                TR_MasterID = x.TR_MasterID,
                TR_Tax_Rate = Mizuho.round(x.TR_Tax_Rate * 100, 2),
                TR_WT_Title = x.TR_WT_Title
            }).OrderBy(x => x.TR_Tax_Rate).ToList();
        }
        //get Vat list for specific user
        public SelectList getAllVat()
        {
            var select = new SelectList(_context.DMVAT.Where(x => x.VAT_isActive == true
                                                             && x.VAT_isDeleted == false)
                                                      .OrderBy(x => x.VAT_Rate)
                                                      .Select(q => new { q.VAT_ID, VAT_Rate = Mizuho.round((q.VAT_Rate * 100), 2) }),
                        "VAT_ID", "VAT_Rate");
            return select;
        }
        //get VAT Rate list for specific vendor in List<T>
        public List<DMVATModel> getVendorVatList(int vendorMasterID)
        {
            return (from vendTr in _context.DMVendorTRVAT
                    join vat in _context.DMVAT on vendTr.VTV_VAT_ID equals vat.VAT_MasterID
                    where vat.VAT_isActive == true && vat.VAT_isDeleted == false
                        && vendTr.VTV_Vendor_ID == vendorMasterID
                    select new DMVATModel
                    {
                        VAT_ID = vat.VAT_ID,
                        VAT_MasterID = vat.VAT_MasterID,
                        VAT_Rate = Mizuho.round(vat.VAT_Rate * 100, 2),
                        VAT_Name = vat.VAT_Name
                    }).OrderBy(x => x.VAT_Rate).ToList();
        }
        //get all VAT including history
        public List<DMVATModel> getAllVATList()
        {
            return _context.DMVAT.OrderBy(x => x.VAT_Rate).Select(x => new DMVATModel
            {
                VAT_ID = x.VAT_ID,
                VAT_MasterID = x.VAT_MasterID,
                VAT_Rate = Mizuho.round(x.VAT_Rate * 100, 2),
                VAT_Name = x.VAT_Name
            }).OrderBy(x => x.VAT_Rate).ToList();
        }
        //get user account without username and password
        public string getUserFullName(int id)
        {
            var user = _context.User.FirstOrDefault(x => x.User_ID == id);

            return user.User_FName + " " + user.User_LName;
        }
        //get Vat list for specific user
        public SelectList getVendorVat(int vendorID)
        {
            var vendorMasterID = _context.DMVendor.Where(x => x.Vendor_ID == vendorID).Select(id => id.Vendor_MasterID).FirstOrDefault();
            var vendorVatIDList = _context.DMVendorTRVAT.Where(x => x.VTV_Vendor_ID == vendorMasterID
                                                                && x.VTV_VAT_ID > 0)
                                                       .Select(q => q.VTV_VAT_ID).ToList();

            var select = new SelectList(_context.DMVAT.Where(x => vendorVatIDList.Contains(x.VAT_MasterID)
                                                             && x.VAT_isActive == true
                                                             && x.VAT_isDeleted == false)
                                                      .Select(q => new { q.VAT_ID, VAT_Rate = Mizuho.round((q.VAT_Rate * 100), 2) }),
                        "VAT_ID", "VAT_Rate");

            return select;
        }
        //get vendor name
        public string getVendorName(int vendorID, int payeeTypeID)
        {
            if (payeeTypeID == GlobalSystemValues.PAYEETYPE_VENDOR)
            {
                return _context.DMVendor.Where(x => x.Vendor_ID == vendorID).Select(x => x.Vendor_Name).FirstOrDefault();
            }
            if (payeeTypeID == GlobalSystemValues.PAYEETYPE_REGEMP || payeeTypeID == GlobalSystemValues.PAYEETYPE_TEMPEMP)
            {
                return _context.DMEmp.Where(x => x.Emp_ID == vendorID).Select(x => x.Emp_Name).FirstOrDefault();
            }
            if (payeeTypeID == GlobalSystemValues.PAYEETYPE_CUST)
            {
                return _context.DMCust.Where(x => x.Cust_ID == vendorID).Select(x => x.Cust_Name).FirstOrDefault();
            }
            return null;
        }
        //get payee type name
        public string getPayeeTypeName(int payeeTypeID)
        {
            return GlobalSystemValues.PAYEETYPE_SELECT_ALL.Where(x => x.Value == payeeTypeID.ToString()).Select(x => x.Text).FirstOrDefault();
        }
        public int getCurrencyID(string ccyAbbr)
        {
            return _context.DMCurrency.Where(x => x.Curr_CCY_ABBR == ccyAbbr && x.Curr_isActive == true
            && x.Curr_isDeleted == false).DefaultIfEmpty(new DMCurrencyModel { Curr_ID = 0 }).FirstOrDefault().Curr_ID;
        }
        //get all currency active only
        public List<DMCurrencyModel> getAllCurrency()
        {
            return _context.DMCurrency.Where(x => x.Curr_isActive == true && x.Curr_isDeleted == false).OrderBy(x => x.Curr_CCY_ABBR).ToList();
        }
        //get all currency active include history
        public SelectList getAllCurrencyIncHist()
        {
            var select = new SelectList(_context.DMCurrency.OrderBy(x => x.Curr_CCY_ABBR)
                                                     .Select(q => new { q.Curr_ID, q.Curr_CCY_ABBR }),
                        "Curr_ID", "Curr_CCY_ABBR");

            return select;
        }
        //retrieve vendor list
        public List<SelectList> getEntrySystemVals()
        {
            List<SelectList> listOfLists = new List<SelectList>();

            listOfLists.Add(new SelectList(_context.DMVendor.Where(x => x.Vendor_isActive == true && x.Vendor_isDeleted == false).OrderBy(x => x.Vendor_Name).Select(q => new { q.Vendor_ID, q.Vendor_Name }),
                                                "Vendor_ID", "Vendor_Name"));

            listOfLists.Add(new SelectList(_context.DMDept.Where(x => x.Dept_isActive == true && x.Dept_isDeleted == false).OrderBy(x => x.Dept_Name).Select(q => new { q.Dept_ID, q.Dept_Name }),
                                                "Dept_ID", "Dept_Name"));

            listOfLists.Add(new SelectList(_context.DMCurrency.Where(x => x.Curr_isActive == true && x.Curr_isDeleted == false).OrderBy(x => x.Curr_CCY_ABBR).Select(q => new { q.Curr_ID, q.Curr_CCY_ABBR }),
                                    "Curr_ID", "Curr_CCY_ABBR"));

            listOfLists.Add(new SelectList(_context.DMTR.Where(x => x.TR_isActive == true && x.TR_isDeleted == false).OrderBy(x => x.TR_Tax_Rate).Select(q => new { q.TR_ID, q.TR_Tax_Rate }),
                        "TR_ID", "TR_Tax_Rate"));

            listOfLists.Add(new SelectList(_context.DMEmp.Where(x => x.Emp_isActive == true && x.Emp_isDeleted == false && x.Emp_Type == "Regular").OrderBy(x => x.Emp_Name).Select(q => new { q.Emp_ID, q.Emp_Name }),
                        "Emp_ID", "Emp_Name"));

            listOfLists.Add(new SelectList(_context.DMEmp.Where(x => x.Emp_isActive == true && x.Emp_isDeleted == false).OrderBy(x => x.Emp_Name).Select(q => new { q.Emp_ID, q.Emp_Name }),
                        "Emp_ID", "Emp_Name"));
            return listOfLists;
        }
        //Get all active tax rates
        public List<DMTRModel> getAllTaxRateList()
        {
            return _context.DMTR.Where(x => x.TR_isActive == true && x.TR_isDeleted == false)
                .OrderBy(x => x.TR_Tax_Rate)
                .Select(x => new DMTRModel
                {
                    TR_ID = x.TR_ID,
                    TR_MasterID = x.TR_MasterID,
                    TR_Tax_Rate = Mizuho.round(x.TR_Tax_Rate * 100, 2),
                    TR_WT_Title = x.TR_Tax_Rate * 100 + "% " + x.TR_WT_Title
                }).ToList();
        }
        //retrieve account details
        public ExpenseEntryModel getExpenseDetail(int entryID)
        {
            return _context.ExpenseEntry.Include("ExpenseEntryDetails").Where(x => x.Expense_ID == entryID).FirstOrDefault();
        }
        //retrieve latest Express transation no.
        public int getExpTransNo(int transType)
        {
            ExpenseEntryModel transNoMax;

            int transno;
            int maxNumber = 0;
            //FISCAL YEAR 
            DateTime now = DateTime.Now;
            DateTime StartFiscal = GetStartOfFiscal(now.Month, now.Year, true);
            DateTime EndFiscal = GetStartOfFiscal(now.Month, now.Year, false);
            var maxNumberObj = _context.ExpenseEntry
                        .Where(y => (StartFiscal <= y.Expense_Date && y.Expense_Date <= EndFiscal) && y.Expense_Number != 0 && y.Expense_Type == transType);
            do
            {

                if (_context.ExpenseEntry.Where(x => (StartFiscal <= x.Expense_Date && x.Expense_Date <= EndFiscal) && x.Expense_Number != 0 && x.Expense_Type == transType).Count() > 0)
                {
                    if (maxNumber != _context.ExpenseEntry
                                        .Where(y => (StartFiscal <= y.Expense_Date && y.Expense_Date <= EndFiscal) && y.Expense_Number != 0 && y.Expense_Type == transType)
                                        .Max(y => y.Expense_Number))
                    {
                        maxNumberObj = _context.ExpenseEntry
                        .Where(y => (StartFiscal <= y.Expense_Date && y.Expense_Date <= EndFiscal) && y.Expense_Number != 0 && y.Expense_Type == transType);
                    }

                    maxNumber = maxNumberObj.Max(y => y.Expense_Number);
                    transNoMax = _context.ExpenseEntry.OrderByDescending(x => x.Expense_ID).First();

                    transno = (transNoMax.Expense_Created_Date.Year < DateTime.Now.Year) ? 1 : (maxNumber + 1);
                }
                else
                {
                    return 1;
                }
            } while (maxNumber != _context.ExpenseEntry
                                        .Where(y => (StartFiscal <= y.Expense_Date && y.Expense_Date <= EndFiscal) && y.Expense_Number != 0 && y.Expense_Type == transType)
                                        .Max(y => y.Expense_Number));
            //_context.Entry<ExpenseEntryModel>(transNoMax).State = EntityState.Detached;
            return transno;
        }
        //retrieve latest gbase transaction no.
        public int getGbaseTransNo(int expId, int dtlId)
        {
            var goHist = _context.GOExpressHist.FirstOrDefault(x => x.ExpenseEntryID == expId
                                                                  && x.ExpenseDetailID == dtlId);

            if (goHist == null)
            {
                return 0;
            }

            return _context.ExpenseTransLists.FirstOrDefault(x => x.TL_GoExpHist_ID == goHist.GOExpHist_Id
                                                      && x.TL_ExpenseID == expId).TL_TransID;
        }
        //check if account no. is RBU or FCDU account.
        public string getBranchNo(string accountNo)
        {
            return accountNo.Substring(4, 3);
        }
        //retrieve latest check number.
        public Dictionary<string, string> getCheckNo()
        {
            Dictionary<string, string> checkNo = new Dictionary<string, string>();

            List<int> expectedStatus = new List<int> {
                    GlobalSystemValues.STATUS_APPROVED,
                    GlobalSystemValues.STATUS_FOR_CLOSING,
                    GlobalSystemValues.STATUS_FOR_PRINTING,
                    GlobalSystemValues.STATUS_REVERSED
                };

            var expense = _context.ExpenseEntry.Where(x => x.Expense_Type == GlobalSystemValues.TYPE_CV
                                                        && expectedStatus.Contains(x.Expense_Status))
                                               .OrderByDescending(x => x.Expense_CheckId)
                                               .ThenByDescending(y => int.Parse(y.Expense_CheckNo)).FirstOrDefault();
            DMCheckModel checkNoModel;

            if (expense == null)
            {
                checkNoModel = _context.DMCheck.OrderBy(x => x.Check_ID).Where(x => x.Check_isActive == true).FirstOrDefault();
            }
            else if (expense.Expense_CheckId != 0)
            {
                checkNoModel = _context.DMCheck.Where(x => x.Check_ID == expense.Expense_CheckId).FirstOrDefault();
            }
            else
            {
                checkNoModel = _context.DMCheck.OrderBy(x => x.Check_ID).Where(x => x.Check_isActive == true).FirstOrDefault();
            }

            if (checkNoModel == null)
                return null;

            if (expense != null)
            {
                if (int.Parse(checkNoModel.Check_Series_To) > int.Parse(expense.Expense_CheckNo))
                {
                    checkNo.Add("check", (int.Parse(expense.Expense_CheckNo) + 1).ToString());
                    checkNo.Add("id", checkNoModel.Check_ID.ToString());
                }
                else
                {
                    checkNoModel.Check_isActive = false;
                    _context.SaveChanges();

                    var newCheck = _context.DMCheck.Where(x => x.Check_isActive == true).OrderBy(x => x.Check_ID).FirstOrDefault();

                    checkNo.Add("check", newCheck.Check_Series_From);
                    checkNo.Add("id", newCheck.Check_ID.ToString());
                }
            }
            else
            {
                var newCheck = _context.DMCheck.Where(x => x.Check_isActive == true).OrderBy(x => x.Check_ID).FirstOrDefault();

                checkNo.Add("check", newCheck.Check_Series_From);
                checkNo.Add("id", newCheck.Check_ID.ToString());
            }


            return checkNo;
        }
        public GOExpressHistModel convertTblCm10ToGOExHist(TblCm10 tblcm10, int entryID, int entryDtlID)
        {
            var goExpHist = new GOExpressHistModel
            {
                GOExpHist_SystemName = tblcm10.SystemName,
                GOExpHist_Groupcode = tblcm10.Groupcode,
                GOExpHist_Branchno = tblcm10.Branchno,
                GOExpHist_OpeKind = tblcm10.OpeKind,
                GOExpHist_AutoApproved = tblcm10.AutoApproved,
                GOExpHist_WarningOverride = tblcm10.WarningOverride,
                GOExpHist_CcyFormat = tblcm10.CcyFormat,
                GOExpHist_OpeBranch = tblcm10.OpeBranch,
                GOExpHist_ValueDate = tblcm10.ValueDate,
                GOExpHist_ReferenceType = tblcm10.ReferenceType,
                GOExpHist_ReferenceNo = tblcm10.ReferenceNo,
                GOExpHist_Comment = tblcm10.Comment,
                GOExpHist_Section = tblcm10.Section,
                GOExpHist_Remarks = tblcm10.Remarks,
                GOExpHist_Memo = tblcm10.Memo,
                GOExpHist_SchemeNo = tblcm10.SchemeNo,
                GOExpHist_Entry11Type = tblcm10.Entry11Type,
                GOExpHist_Entry11IbfCode = tblcm10.Entry11IbfCode,
                GOExpHist_Entry11Ccy = tblcm10.Entry11Ccy,
                GOExpHist_Entry11Amt = tblcm10.Entry11Amt,
                GOExpHist_Entry11Cust = tblcm10.Entry11Cust,
                GOExpHist_Entry11Actcde = tblcm10.Entry11Actcde,
                GOExpHist_Entry11ActType = tblcm10.Entry11ActType,
                GOExpHist_Entry11ActNo = tblcm10.Entry11ActNo,
                GOExpHist_Entry11ExchRate = tblcm10.Entry11ExchRate,
                GOExpHist_Entry11ExchCcy = tblcm10.Entry11ExchCcy,
                GOExpHist_Entry11Fund = tblcm10.Entry11Fund,
                GOExpHist_Entry11CheckNo = tblcm10.Entry11CheckNo,
                GOExpHist_Entry11Available = tblcm10.Entry11Available,
                GOExpHist_Entry11AdvcPrnt = tblcm10.Entry11AdvcPrnt,
                GOExpHist_Entry11Details = tblcm10.Entry11Details,
                GOExpHist_Entry11Entity = tblcm10.Entry11Entity,
                GOExpHist_Entry11Division = tblcm10.Entry11Division,
                GOExpHist_Entry11InterAmt = tblcm10.Entry11InterAmt,
                GOExpHist_Entry11InterRate = tblcm10.Entry11InterRate,
                GOExpHist_Entry12Type = tblcm10.Entry12Type,
                GOExpHist_Entry12IbfCode = tblcm10.Entry12IbfCode,
                GOExpHist_Entry12Ccy = tblcm10.Entry12Ccy,
                GOExpHist_Entry12Amt = tblcm10.Entry12Amt,
                GOExpHist_Entry12Cust = tblcm10.Entry12Cust,
                GOExpHist_Entry12Actcde = tblcm10.Entry12Actcde,
                GOExpHist_Entry12ActType = tblcm10.Entry12ActType,
                GOExpHist_Entry12ActNo = tblcm10.Entry12ActNo,
                GOExpHist_Entry12ExchRate = tblcm10.Entry12ExchRate,
                GOExpHist_Entry12ExchCcy = tblcm10.Entry12ExchCcy,
                GOExpHist_Entry12Fund = tblcm10.Entry12Fund,
                GOExpHist_Entry12CheckNo = tblcm10.Entry12CheckNo,
                GOExpHist_Entry12Available = tblcm10.Entry12Available,
                GOExpHist_Entry12AdvcPrnt = tblcm10.Entry12AdvcPrnt,
                GOExpHist_Entry12Details = tblcm10.Entry12Details,
                GOExpHist_Entry12Entity = tblcm10.Entry12Entity,
                GOExpHist_Entry12Division = tblcm10.Entry12Division,
                GOExpHist_Entry12InterAmt = tblcm10.Entry12InterAmt,
                GOExpHist_Entry12InterRate = tblcm10.Entry12InterRate,
                GOExpHist_Entry21Type = tblcm10.Entry21Type,
                GOExpHist_Entry21IbfCode = tblcm10.Entry21IbfCode,
                GOExpHist_Entry21Ccy = tblcm10.Entry21Ccy,
                GOExpHist_Entry21Amt = tblcm10.Entry21Amt,
                GOExpHist_Entry21Cust = tblcm10.Entry21Cust,
                GOExpHist_Entry21Actcde = tblcm10.Entry21Actcde,
                GOExpHist_Entry21ActType = tblcm10.Entry21ActType,
                GOExpHist_Entry21ActNo = tblcm10.Entry21ActNo,
                GOExpHist_Entry21ExchRate = tblcm10.Entry21ExchRate,
                GOExpHist_Entry21ExchCcy = tblcm10.Entry21ExchCcy,
                GOExpHist_Entry21Fund = tblcm10.Entry21Fund,
                GOExpHist_Entry21CheckNo = tblcm10.Entry21CheckNo,
                GOExpHist_Entry21Available = tblcm10.Entry21Available,
                GOExpHist_Entry21AdvcPrnt = tblcm10.Entry21AdvcPrnt,
                GOExpHist_Entry21Details = tblcm10.Entry21Details,
                GOExpHist_Entry21Entity = tblcm10.Entry21Entity,
                GOExpHist_Entry21Division = tblcm10.Entry21Division,
                GOExpHist_Entry21InterAmt = tblcm10.Entry21InterAmt,
                GOExpHist_Entry21InterRate = tblcm10.Entry21InterRate,
                GOExpHist_Entry22Type = tblcm10.Entry22Type,
                GOExpHist_Entry22IbfCode = tblcm10.Entry22IbfCode,
                GOExpHist_Entry22Ccy = tblcm10.Entry22Ccy,
                GOExpHist_Entry22Amt = tblcm10.Entry22Amt,
                GOExpHist_Entry22Cust = tblcm10.Entry22Cust,
                GOExpHist_Entry22Actcde = tblcm10.Entry22Actcde,
                GOExpHist_Entry22ActType = tblcm10.Entry22ActType,
                GOExpHist_Entry22ActNo = tblcm10.Entry22ActNo,
                GOExpHist_Entry22ExchRate = tblcm10.Entry22ExchRate,
                GOExpHist_Entry22ExchCcy = tblcm10.Entry22ExchCcy,
                GOExpHist_Entry22Fund = tblcm10.Entry22Fund,
                GOExpHist_Entry22CheckNo = tblcm10.Entry22CheckNo,
                GOExpHist_Entry22Available = tblcm10.Entry22Available,
                GOExpHist_Entry22AdvcPrnt = tblcm10.Entry22AdvcPrnt,
                GOExpHist_Entry22Details = tblcm10.Entry22Details,
                GOExpHist_Entry22Entity = tblcm10.Entry22Entity,
                GOExpHist_Entry22Division = tblcm10.Entry22Division,
                GOExpHist_Entry22InterAmt = tblcm10.Entry22InterAmt,
                GOExpHist_Entry22InterRate = tblcm10.Entry22InterRate,
                GOExpHist_Entry31Type = tblcm10.Entry31Type,
                GOExpHist_Entry31IbfCode = tblcm10.Entry31IbfCode,
                GOExpHist_Entry31Ccy = tblcm10.Entry31Ccy,
                GOExpHist_Entry31Amt = tblcm10.Entry31Amt,
                GOExpHist_Entry31Cust = tblcm10.Entry31Cust,
                GOExpHist_Entry31Actcde = tblcm10.Entry31Actcde,
                GOExpHist_Entry31ActType = tblcm10.Entry31ActType,
                GOExpHist_Entry31ActNo = tblcm10.Entry31ActNo,
                GOExpHist_Entry31ExchRate = tblcm10.Entry31ExchRate,
                GOExpHist_Entry31ExchCcy = tblcm10.Entry31ExchCcy,
                GOExpHist_Entry31Fund = tblcm10.Entry31Fund,
                GOExpHist_Entry31CheckNo = tblcm10.Entry31CheckNo,
                GOExpHist_Entry31Available = tblcm10.Entry31Available,
                GOExpHist_Entry31AdvcPrnt = tblcm10.Entry31AdvcPrnt,
                GOExpHist_Entry31Details = tblcm10.Entry31Details,
                GOExpHist_Entry31Entity = tblcm10.Entry31Entity,
                GOExpHist_Entry31Division = tblcm10.Entry31Division,
                GOExpHist_Entry31InterAmt = tblcm10.Entry31InterAmt,
                GOExpHist_Entry31InterRate = tblcm10.Entry31InterRate,
                GOExpHist_Entry32Type = tblcm10.Entry32Type,
                GOExpHist_Entry32IbfCode = tblcm10.Entry32IbfCode,
                GOExpHist_Entry32Ccy = tblcm10.Entry32Ccy,
                GOExpHist_Entry32Amt = tblcm10.Entry32Amt,
                GOExpHist_Entry32Cust = tblcm10.Entry32Cust,
                GOExpHist_Entry32Actcde = tblcm10.Entry32Actcde,
                GOExpHist_Entry32ActType = tblcm10.Entry32ActType,
                GOExpHist_Entry32ActNo = tblcm10.Entry32ActNo,
                GOExpHist_Entry32ExchRate = tblcm10.Entry32ExchRate,
                GOExpHist_Entry32ExchCcy = tblcm10.Entry32ExchCcy,
                GOExpHist_Entry32Fund = tblcm10.Entry32Fund,
                GOExpHist_Entry32CheckNo = tblcm10.Entry32CheckNo,
                GOExpHist_Entry32Available = tblcm10.Entry32Available,
                GOExpHist_Entry32AdvcPrnt = tblcm10.Entry32AdvcPrnt,
                GOExpHist_Entry32Details = tblcm10.Entry32Details,
                GOExpHist_Entry32Entity = tblcm10.Entry32Entity,
                GOExpHist_Entry32Division = tblcm10.Entry32Division,
                GOExpHist_Entry32InterAmt = tblcm10.Entry32InterAmt,
                GOExpHist_Entry32InterRate = tblcm10.Entry32InterRate,
                GOExpHist_Entry41Type = tblcm10.Entry41Type,
                GOExpHist_Entry41IbfCode = tblcm10.Entry41IbfCode,
                GOExpHist_Entry41Ccy = tblcm10.Entry41Ccy,
                GOExpHist_Entry41Amt = tblcm10.Entry41Amt,
                GOExpHist_Entry41Cust = tblcm10.Entry41Cust,
                GOExpHist_Entry41Actcde = tblcm10.Entry41Actcde,
                GOExpHist_Entry41ActType = tblcm10.Entry41ActType,
                GOExpHist_Entry41ActNo = tblcm10.Entry41ActNo,
                GOExpHist_Entry41ExchRate = tblcm10.Entry41ExchRate,
                GOExpHist_Entry41ExchCcy = tblcm10.Entry41ExchCcy,
                GOExpHist_Entry41Fund = tblcm10.Entry41Fund,
                GOExpHist_Entry41CheckNo = tblcm10.Entry41CheckNo,
                GOExpHist_Entry41Available = tblcm10.Entry41Available,
                GOExpHist_Entry41AdvcPrnt = tblcm10.Entry41AdvcPrnt,
                GOExpHist_Entry41Details = tblcm10.Entry41Details,
                GOExpHist_Entry41Entity = tblcm10.Entry41Entity,
                GOExpHist_Entry41Division = tblcm10.Entry41Division,
                GOExpHist_Entry41InterAmt = tblcm10.Entry41InterAmt,
                GOExpHist_Entry41InterRate = tblcm10.Entry41InterRate,
                GOExpHist_Entry42Type = tblcm10.Entry42Type,
                GOExpHist_Entry42IbfCode = tblcm10.Entry42IbfCode,
                GOExpHist_Entry42Ccy = tblcm10.Entry42Ccy,
                GOExpHist_Entry42Amt = tblcm10.Entry42Amt,
                GOExpHist_Entry42Cust = tblcm10.Entry42Cust,
                GOExpHist_Entry42Actcde = tblcm10.Entry42Actcde,
                GOExpHist_Entry42ActType = tblcm10.Entry42ActType,
                GOExpHist_Entry42ActNo = tblcm10.Entry42ActNo,
                GOExpHist_Entry42ExchRate = tblcm10.Entry42ExchRate,
                GOExpHist_Entry42ExchCcy = tblcm10.Entry42ExchCcy,
                GOExpHist_Entry42Fund = tblcm10.Entry42Fund,
                GOExpHist_Entry42CheckNo = tblcm10.Entry42CheckNo,
                GOExpHist_Entry42Available = tblcm10.Entry42Available,
                GOExpHist_Entry42AdvcPrnt = tblcm10.Entry42AdvcPrnt,
                GOExpHist_Entry42Details = tblcm10.Entry42Details,
                GOExpHist_Entry42Entity = tblcm10.Entry42Entity,
                GOExpHist_Entry42Division = tblcm10.Entry42Division,
                GOExpHist_Entry42InterAmt = tblcm10.Entry42InterAmt,
                GOExpHist_Entry42InterRate = tblcm10.Entry42InterRate,
                ExpenseEntryID = entryID,
                ExpenseDetailID = entryDtlID
            };

            _context.GOExpressHist.Add(goExpHist);

            return goExpHist;
        }
        public string getVoucherNo(int type, DateTime year, int number, bool liq = false)
        {
            string type_code = "";
            if (liq == false)
                type_code = GlobalSystemValues.getApplicationCode(type);
            else
                type_code = "LIQ";

            return type_code + "-" + GetSelectedYearMonthOfTerm(year.Month, year.Year).Year + "-" +
                                       number.ToString().PadLeft(5, '0'); ;
        }
        public bool UpdateCDDPrintingStatus(int entryID, int entryDtlID, int type)
        {
            var cddStatus = _context.PrintStatus.Where(x => x.PS_EntryID == entryID && x.PS_EntryDtlID == entryDtlID
                                        && x.PS_Type == type).FirstOrDefault();
            if (cddStatus != null)
            {
                cddStatus.PS_CDD = true;
                _context.Entry(cddStatus).State = EntityState.Modified;
                _context.SaveChanges();
                if (type == GlobalSystemValues.TYPE_LIQ)
                {
                    updatePrintStatusLiquidationForCLosing(entryID);
                }
                else
                {
                    updatePrintStatusForCLosing(entryID);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UpdateBIR2307PrintingStatus(int entryID)
        {
            var birStatus = _context.PrintStatus.Where(x => x.PS_EntryID == entryID).ToList();
            foreach (var i in birStatus)
            {
                i.PS_BIR2307 = true;
                _context.Entry(i).State = EntityState.Modified;
            }

            if (birStatus != null)
            {
                _context.SaveChanges();
                updatePrintStatusForCLosing(entryID);
                return true;
            }
            return false;
        }
        public bool UpdatePrintVoucherPrintStatus(int entryID)
        {
            var ps = _context.PrintStatus.Where(x => x.PS_EntryID == entryID).ToList();

            foreach (var item in ps)
            {
                item.PS_Voucher = true;
                _context.Entry(item).State = EntityState.Modified;
            }

            if (ps != null)
            {
                _context.SaveChanges();
                updatePrintStatusForCLosing(entryID);
                return true;
            }
            return false;
        }

        public void updatePrintStatusForCLosing(int entryID)
        {
            var expPrintStatus = _context.PrintStatus.Where(x => x.PS_EntryID == entryID);

            bool updStats = true;

            foreach (var item in expPrintStatus)
            {
                if (!item.PS_BIR2307 || !item.PS_CDD || !item.PS_Check || !item.PS_LOI || !item.PS_Voucher)
                    updStats = false;
            }

            if (updStats)
            {
                ExpenseEntryModel updItem = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);

                //Update to For Closing only if status is in For Printing.
                if (updItem.Expense_Status == GlobalSystemValues.STATUS_FOR_PRINTING)
                {
                    updItem.Expense_Status = GlobalSystemValues.STATUS_FOR_CLOSING;
                    updItem.Expense_Last_Updated = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }

        public void updatePrintStatusLiquidationForCLosing(int entryID)
        {
            var expPrintStatus = _context.PrintStatus.Where(x => x.PS_EntryID == entryID && x.PS_Type == GlobalSystemValues.TYPE_LIQ);

            bool updStats = true;

            foreach (var item in expPrintStatus)
            {
                if (!item.PS_BIR2307 || !item.PS_CDD || !item.PS_Check || !item.PS_LOI || !item.PS_Voucher)
                    updStats = false;
            }

            if (updStats)
            {
                LiquidationEntryDetailModel updItem = _context.LiquidationEntryDetails.FirstOrDefault(x => x.ExpenseEntryModel.Expense_ID == entryID);
                //Update to For Closing only if status is in For Printing.
                if (updItem.Liq_Status == GlobalSystemValues.STATUS_FOR_PRINTING)
                {
                    updItem.Liq_Status = GlobalSystemValues.STATUS_FOR_CLOSING;
                    updItem.Liq_LastUpdated_Date = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }
        public bool UpdatePrintCheckPrintStatus(int entryID)
        {
            var ps = _context.PrintStatus.Where(x => x.PS_EntryID == entryID).ToList();

            foreach (var item in ps)
            {
                item.PS_Check = true;
                _context.Entry(item).State = EntityState.Modified;
            }

            if (ps != null)
            {
                _context.SaveChanges();
                updatePrintStatusForCLosing(entryID);
                return true;
            }
            return false;
        }
        public ExpenseEntryModel getSingleEntryRecord(int entryID)
        {
            return _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).AsNoTracking().FirstOrDefault();
        }
        //Get Transaction No for Liquidation entries
        public int getTransactionNoLiquidation(int entryID, int detailID)
        {
            return _context.ExpenseTransLists.Where(x => x.TL_GoExpHist_ID == _context.GOExpressHist.Where(
                    y => y.ExpenseEntryID == entryID && y.ExpenseDetailID == detailID).FirstOrDefault().GOExpHist_Id)
                    .FirstOrDefault().TL_TransID;
        }

        //Check if latest pettycash entry is open
        public bool lastPCEntry()
        {
            var lastEntry = _context.PettyCash.OrderByDescending(x => x.PC_ID).Select(x => x.PC_Status).FirstOrDefault();

            if (lastEntry == GlobalSystemValues.STATUS_OPEN)
                return true;
            else
                return false;
        }
        public bool confirmBrkDown()
        {

            PettyCashModel model = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();

            decimal oneK = Mizuho.round(model.PCB_OneThousand * 1000M, 2);
            decimal fiveH = Mizuho.round(model.PCB_FiveHundred * 500M, 2);
            decimal twoH = Mizuho.round(model.PCB_TwoHundred * 200M, 2);
            decimal oneH = Mizuho.round(model.PCB_OneHundred * 100M, 2);
            decimal fifty = Mizuho.round(model.PCB_Fifty * 50M, 2);
            decimal twenty = Mizuho.round(model.PCB_Twenty * 20M, 2);
            decimal ten = Mizuho.round(model.PCB_Ten * 10M, 2);
            decimal five = Mizuho.round(model.PCB_Five * 5M, 2);
            decimal one = Mizuho.round(model.PCB_One * 1M, 2);
            decimal c25 = Mizuho.round(model.PCB_TwentyFiveCents * .25M, 2);
            decimal c10 = Mizuho.round(model.PCB_TenCents * .10M, 2);
            decimal c5 = Mizuho.round(model.PCB_FiveCents * .05M, 2);
            decimal c1 = Mizuho.round(model.PCB_OneCents * .01M, 2);

            decimal total = Mizuho.round(oneK + fiveH + twoH + oneH + fifty + twenty + ten + five + one + c25 + c10 + c5 + c1, 2);

            if (total == model.PC_EndBal)
                return true;
            else
                return false;
        }
        public bool closePC(int userID)
        {
            PettyCashModel pc = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();

            pc.PC_Status = GlobalSystemValues.STATUS_CLOSED;
            pc.PC_CloseDate = DateTime.Now;
            pc.PC_CloseUser = userID;

            _context.SaveChanges();

            return true;
        }
        public bool reopenPC()
        {
            PettyCashModel pc = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();

            pc.PC_Status = GlobalSystemValues.STATUS_OPEN;
            _context.SaveChanges();

            return true;
        }
        //Update Status for closing
        public bool forClosingStatus(int expenseID)
        {
            var printStatus = _context.PrintStatus.Where(x => x.PS_EntryID == expenseID).ToList();

            int completed = 0;

            foreach (var item in printStatus)
            {
                if (item.PS_LOI && item.PS_Check && item.PS_CDD && item.PS_BIR2307 && item.PS_Voucher)
                    completed++;
            }

            if (completed == (printStatus.Count() - 1))
            {
                ExpenseEntryModel updItem = _context.ExpenseEntry.Where(x => x.Expense_ID == expenseID).FirstOrDefault();

                updItem.Expense_Status = GlobalSystemValues.STATUS_FOR_CLOSING;

                _context.SaveChanges();

                return true;
            }

            forClosingStatus(expenseID);

            return false;
        }
        ///========[End of Other Functions]============
    }

    internal class gbaseContainer
    {
        public DateTime valDate { get; set; }
        public string remarks { get; set; }
        public int maker { get; set; }
        public int approver { get; set; }
        public List<entryContainer> entries { get; set; }

        public gbaseContainer()
        {
            entries = new List<entryContainer>();
        }
    }

    internal class entryContainer
    {
        public string type { get; set; }
        public int ccy { get; set; }
        public decimal amount { get; set; }
        public int vendor { get; set; }
        public int account { get; set; }
        public decimal interate { get; set; }
        public decimal interamount { get; set; }
        public int contraCcy { get; set; }
        public string chkNo { get; set; }
        public int dept { get; set; }
    }
}
