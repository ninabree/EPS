using ExpenseProcessingSystem.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class TEMP_HomeReportWTSDummyData
    {
        public static IEnumerable<RepWTSViewModel> GetTEMP_HomeReportWTSOutputModelData()
        {
            RepWTSViewModel[] rep = new RepWTSViewModel[60];
            for (int runs = 0; runs < 30; runs++)
            {
                rep[runs] = new RepWTSViewModel
                {
                    WTS_Voucher_No = 10000000 + runs,
                    WTS_Check_No = 2019000 + runs,
                    WTS_Val_Date = DateTime.Now.AddDays(runs),
                    WTS_Ref_No = 900000000 + runs,
                    WTS_Section = "Sample Section",
                    WTS_Remarks = "Sample Remarks",
                    WTS_Deb_Cred = 4282.00,
                    WTS_Currency_ID = 2,
                    WTS_Amount = 4282.00,
                    WTS_Cust = "Sample Customer",
                    WTS_Acc_Code = 3000 + runs,
                    WTS_Acc_No = 1000 + runs,
                    WTS_Acc_Name = "Sample Account Name",
                    WTS_Exchange_Rate = 0.02,
                    WTS_Contra_Currency_ID = 4,
                    WTS_Fund = "Sample Fund",
                    WTS_Advice_Print = "Sample Advice Print",
                    WTS_Details = "Sample Details",
                    WTS_Entity = "Sample Entity",
                    WTS_Division = "Sample Division",
                    WTS_Inter_Amount = 4282.00,
                    WTS_Inter_Rate = 0.42,
                    WTS_TR_ID = 149
                };
            }
            for (int runs = 0; runs < 30; runs++)
            {
                rep[runs + 30] = new RepWTSViewModel
                {
                    WTS_Voucher_No = 10000000 + runs,
                    WTS_Check_No = 2019000 + runs,
                    WTS_Val_Date = DateTime.Now.AddDays(runs).AddMonths(1),
                    WTS_Ref_No = 900000000 + runs,
                    WTS_Section = "Sample Section",
                    WTS_Remarks = "Sample Remarks",
                    WTS_Deb_Cred = 4282.00,
                    WTS_Currency_ID = 2,
                    WTS_Amount = 4282.00,
                    WTS_Cust = "Sample Customer",
                    WTS_Acc_Code = 3000 + runs,
                    WTS_Acc_No = 1000 + runs,
                    WTS_Acc_Name = "Sample Account Name",
                    WTS_Exchange_Rate = 0.02,
                    WTS_Contra_Currency_ID = 4,
                    WTS_Fund = "Sample Fund",
                    WTS_Advice_Print = "Sample Advice Print",
                    WTS_Details = "Sample Details",
                    WTS_Entity = "Sample Entity",
                    WTS_Division = "Sample Division",
                    WTS_Inter_Amount = 4282.00,
                    WTS_Inter_Rate = 0.42,
                    WTS_TR_ID = 153
                };
            }
            return rep;
        }
        public static IEnumerable<RepWTSViewModel> GetTEMP_HomeReportWTSOutputModelData_Month(string _year, string _month, IEnumerable<RepWTSViewModel> data, string _subType)
        {
            return data.Where(x => x.WTS_Val_Date >= new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), 1)
                                && x.WTS_Val_Date <= new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), 30) 
                                && x.WTS_TR_ID == Convert.ToInt32(_subType));
        }
        public static IEnumerable<RepWTSViewModel> GetTEMP_HomeReportWTSOutputModelData_Semester(string _year, string _sem, IEnumerable<RepWTSViewModel> data, string _subType)
        {
            DateTime periodFrom = new DateTime();
            DateTime periodTo = new DateTime();
            CultureInfo ci = CultureInfo.InvariantCulture;
            switch (_sem)
            {
                case "1":
                    periodFrom = DateTime.ParseExact("1/1/" + _year, "MM/dd/yyyy", ci);
                    periodFrom = DateTime.ParseExact("6/1/" + _year, "MM/dd/yyyy", ci);
                    break;
                case "2":
                    periodFrom = DateTime.ParseExact("7/1/" + _year, "MM/dd/yyyy", ci);
                    periodFrom = DateTime.ParseExact("12/1/" + _year, "MM/dd/yyyy", ci);
                    break;
            }
            return data.Where(x => x.WTS_Val_Date >= periodFrom
                                && x.WTS_Val_Date <= periodTo
                                && x.WTS_TR_ID == Convert.ToInt32(_subType));
        }
        public static IEnumerable<RepWTSViewModel> GetTEMP_HomeReportWTSOutputModelData_Period(DateTime _periodFrom, DateTime _periodTo, IEnumerable<RepWTSViewModel> data, string _subType)
        {
            return data.Where(x => x.WTS_Val_Date >= _periodFrom
                                && x.WTS_Val_Date <= _periodTo
                                && x.WTS_TR_ID == Convert.ToInt32(_subType));
        }
    }
}