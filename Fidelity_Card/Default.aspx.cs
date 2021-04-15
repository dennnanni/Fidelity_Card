using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fidelity_Card
{
    public partial class Default : System.Web.UI.Page
    {
        private List<Card> _cards;
        private List<Transaction> _transactions;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Creates the lists
                _cards = new List<Card>();
                Session["cards"] = _cards;
                _transactions = new List<Transaction>();
                Session["transactions"] = _transactions;
            }
            else
            {
                // Restore values
                _cards = (List<Card>)Session["cards"];
                _transactions = (List<Transaction>)Session["transactions"];
            }
        }
    }
}