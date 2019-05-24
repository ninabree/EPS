using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Gbase
{
    public class tblCM10Model
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string GROUPCODE { get; set; }
        public string BRANCHNO { get; set; }
        public string OPE_KIND { get; set; }
        public string AUTO_APPROVED { get; set; }
        public string WARNING_OVERRIDE { get; set; }
        public string CCY_FORMAT { get; set; }
        public string OPE_BRANCH { get; set; }
        public string VALUE_DATE { get; set; }
        public string REFERENCE_TYPE { get; set; }
        public string REFERENCE_NO { get; set; }
        public string COMMENT { get; set; }
        public string SECTION { get; set; }
        public string REMARKS { get; set; }
        public string MEMO { get; set; }
        public string SCHEME_NO { get; set; }
        public string ENTRY11_TYPE { get; set; }
        public string ENTRY11_IBF_CODE { get; set; }
        public string ENTRY11_CCY { get; set; }
        public string ENTRY11_AMT { get; set; }
        public string ENTRY11_CUST { get; set; }
        public string ENTRY11_ACTCDE { get; set; }
        public string ENTRY11_ACT_TYPE { get; set; }
        public string ENTRY11_ACT_NO { get; set; }
        public string ENTRY11_EXCH_RATE { get; set; }
        public string ENTRY11_EXCH_CCY { get; set; }
        public string ENTRY11_FUND { get; set; }
        public string ENTRY11_CHECK_NO { get; set; }
        public string ENTRY11_AVAILABLE { get; set; }
        public string ENTRY11_ADVC_PRNT { get; set; }
        public string ENTRY11_DETAILS { get; set; }
        public string ENTRY11_DIVISION { get; set; }
        public string ENTRY11_INTER_AMT { get; set; }
        public string ENTRY11_INTER_RATE { get; set; }
        public string ENTRY22_TYPE { get; set; }
        public string ENTRY22_IBF_CODE { get; set; }
        public string ENTRY22_CCY { get; set; }
        public string ENTRY22_AMT { get; set; }
        public string ENTRY22_CUST { get; set; }
        public string ENTRY22_ACTCDE { get; set; }
        public string ENTRY22_ACT_TYPE { get; set; }
        public string ENTRY22_ACT_NO { get; set; }
        public string ENTRY22_EXCH_RATE { get; set; }
        public string ENTRY22_EXCH_CCY { get; set; }
        public string ENTRY22_FUND { get; set; }
        public string ENTRY22_CHECK_NO { get; set; }
        public string ENTRY22_AVAILABLE { get; set; }
        public string ENTRY22_ADVC_PRNT { get; set; }
        public string ENTRY22_DETAILS { get; set; }
        public string ENTRY22_DIVISION { get; set; }
        public string ENTRY22_INTER_AMT { get; set; }
        public string ENTRY22_INTER_RATE { get; set; }
        public string ENTRY33_TYPE { get; set; }
        public string ENTRY33_IBF_CODE { get; set; }
        public string ENTRY33_CCY { get; set; }
        public string ENTRY33_AMT { get; set; }
        public string ENTRY33_CUST { get; set; }
        public string ENTRY33_ACTCDE { get; set; }
        public string ENTRY33_ACT_TYPE { get; set; }
        public string ENTRY33_ACT_NO { get; set; }
        public string ENTRY33_EXCH_RATE { get; set; }
        public string ENTRY33_EXCH_CCY { get; set; }
        public string ENTRY33_FUND { get; set; }
        public string ENTRY33_CHECK_NO { get; set; }
        public string ENTRY33_AVAILABLE { get; set; }
        public string ENTRY33_ADVC_PRNT { get; set; }
        public string ENTRY33_DETAILS { get; set; }
        public string ENTRY33_DIVISION { get; set; }
        public string ENTRY33_INTER_AMT { get; set; }
        public string ENTRY33_INTER_RATE { get; set; }
        public string ENTRY44_TYPE { get; set; }
        public string ENTRY44_IBF_CODE { get; set; }
        public string ENTRY44_CCY { get; set; }
        public string ENTRY44_AMT { get; set; }
        public string ENTRY44_CUST { get; set; }
        public string ENTRY44_ACTCDE { get; set; }
        public string ENTRY44_ACT_TYPE { get; set; }
        public string ENTRY44_ACT_NO { get; set; }
        public string ENTRY44_EXCH_RATE { get; set; }
        public string ENTRY44_EXCH_CCY { get; set; }
        public string ENTRY44_FUND { get; set; }
        public string ENTRY44_CHECK_NO { get; set; }
        public string ENTRY44_AVAILABLE { get; set; }
        public string ENTRY44_ADVC_PRNT { get; set; }
        public string ENTRY44_DETAILS { get; set; }
        public string ENTRY44_DIVISION { get; set; }
        public string ENTRY44_INTER_AMT { get; set; }
        public string ENTRY44_INTER_RATE { get; set; }
        public string MAKER_EMPNO { get; set; }
        public string EMPNO { get; set; }
        public DateTime DATESTAMP { get; set; }
        public string TRANSNO { get; set; }
        public string XMLMSG { get; set; }
        public string RECSTATUS { get; set; }
        public DateTime TIMESENT { get; set; }
        public DateTime TIMERESPOND { get; set; }
    }
}
