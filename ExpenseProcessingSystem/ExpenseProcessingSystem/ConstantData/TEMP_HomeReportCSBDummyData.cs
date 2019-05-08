using ExpenseProcessingSystem.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class TEMP_HomeReportCSBDummyData
    {
        public static Temp_RepCSBViewModelList GetTEMP_HomeReportCSBOutputModelData()
        {
            Temp_RepCSBViewModelList vmList = new Temp_RepCSBViewModelList();
            List<Temp_RepCSBViewModel> list = new List<Temp_RepCSBViewModel>();
            for (int runs = 0; runs < 20; runs++)
            {
                Temp_RepCSBViewModel tmp = new Temp_RepCSBViewModel
                {
                    CSB_Date = DateTime.Now.AddDays(runs),
                    CSB_Debit = 1000 + runs,
                    CSB_Credit = 1000 + runs,
                    CSB_Remarks = "Sample Remarks",
                    CSB_Ref_No = 900000000 + runs,
                    CSB_Balance = 1000 + runs
                };
                list.Add(tmp);
            }
            for (int runs = 0; runs < 20; runs++)
            {
                Temp_RepCSBViewModel tmp = new Temp_RepCSBViewModel
                {
                    CSB_Date = DateTime.Now.AddDays(runs).AddMonths(1),
                    CSB_Debit = 1000 + runs,
                    CSB_Credit = 1000 + runs,
                    CSB_Remarks = "Sample Remarks",
                    CSB_Ref_No = 900000000 + runs,
                    CSB_Balance = 1000 + runs
                };
                list.Add(tmp);
            }
            vmList.CSBList = list;
            return vmList;
        }
        public static List<Temp_RepCSBViewModel> GetTEMP_HomeReportCSBOutputModelData_Month(int _year, int _month, List<Temp_RepCSBViewModel> data, int _subType)
        {
            return data.Where(x => x.CSB_Date >= new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), 1)
                                && x.CSB_Date <= new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), 30)).ToList();
        }
        public static List<Temp_RepCSBViewModel> GetTEMP_HomeReportCSBOutputModelData_Semester(int _year, int _sem, List<Temp_RepCSBViewModel> data, int _subType)
        {
            DateTime periodFrom = new DateTime();
            DateTime periodTo = new DateTime();
            CultureInfo ci = CultureInfo.InvariantCulture;
            switch (_sem)
            {
                case 1:
                    periodFrom = DateTime.ParseExact("01/01/" + _year, "MM/dd/yyyy", ci);
                    periodTo = DateTime.ParseExact("06/01/" + _year, "MM/dd/yyyy", ci);
                    break;
                case 2:
                    periodFrom = DateTime.ParseExact("07/01/" + _year, "MM/dd/yyyy", ci);
                    periodTo = DateTime.ParseExact("12/01/" + _year, "MM/dd/yyyy", ci);
                    break;
            }
            return data.Where(x => x.CSB_Date >= periodFrom
                                && x.CSB_Date <= periodTo).ToList();
        }
        public static List<Temp_RepCSBViewModel> GetTEMP_HomeReportCSBOutputModelData_Period(DateTime _periodFrom, DateTime _periodTo, List<Temp_RepCSBViewModel> data, int _subType)
        {
            return data.Where(x => x.CSB_Date >= _periodFrom
                                && x.CSB_Date <= _periodTo).ToList();
        }
    }
}
