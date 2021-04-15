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
                _transactions = new List<Transaction>();

                //Some card examples
                _cards.Add(
                    new Card()
                    {
                        Name = "Luca",
                        Surname = "Rossi",
                        Age = 34,
                        Address = "Viale Portorico, 10",
                        City = "Empoli",

                    }
                );

                _cards.Add(
                    new Card()
                    {
                        Name = "Matteo",
                        Surname = "Brandolini",
                        Age = 45,
                        Address = "Via Anna Frank, 58",
                        City = "Gallipoli",

                    }
                );


                Session["cards"] = _cards;
                Session["transactions"] = _transactions;

                // Show cards
                grdMaster.DataSource = _cards;
                grdMaster.DataBind();
            }
            else
            {
                // Restore values
                _cards = (List<Card>)Session["cards"];
                _transactions = (List<Transaction>)Session["transactions"];
            }
        }

        protected void btnAddCard_Click(object sender, EventArgs e)
        {

        }
    }
}