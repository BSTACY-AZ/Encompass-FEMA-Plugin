using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.ComponentModel;
using TMLEnSFNetworking;

namespace TMLFemaPlugin
{
    [Plugin]
    public class Main
    {
        public static readonly string FEMAHASRUNEFIELDNAME = "CX.FEMA.HASRUN";

        private Loan _loan;
        private readonly string _LAUNCHFIELDNAME = "CX.FEMA.LAUNCH";

        public Main()
        {
            EncompassApplication.LoanOpened += new EventHandler(Loan_Opened);
            EncompassApplication.LoanClosing += new EventHandler(Loan_Closing);
        }

        ~Main()
        {
            EncompassApplication.LoanOpened -= new EventHandler(Loan_Opened);
            EncompassApplication.LoanClosing -= new EventHandler(Loan_Closing);
        }

        private void Loan_Opened(object sender, EventArgs e)
        {
            _loan = EncompassApplication.CurrentLoan;
            _loan.FieldChange += new FieldChangeEventHandler(Loan_FieldChange);
        }

        private void Loan_Closing(object sender, EventArgs e)
        {
            _loan.FieldChange -= new FieldChangeEventHandler(Loan_FieldChange);
        }

        private void Loan_FieldChange(object sender, FieldChangeEventArgs e)
        {
            if (e.FieldID.Equals(_LAUNCHFIELDNAME))
            {
                //  Check if plugin has been run before
                bool hasRunBefore = _loan.Fields[FEMAHASRUNEFIELDNAME].UnformattedValue.CompareTo("Y") == 0 ? true : false;

                if (hasRunBefore)
                {
                    Macro.Alert("ERROR: FEMA Plugin has already been run on this loan!");
                    return;
                }
                else
                {
                    //  Open the form if it hasn't been run before
                    using (MainForm frm = new MainForm(_loan))
                    {
                        frm.ShowDialog();
                    }
                }
            }
        }
    }
}
