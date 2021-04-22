﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fidelity_Card
{
    public partial class Default : System.Web.UI.Page
    {
        private List<Card> Cards { get; set; }
        private bool _updating = false;

        private int FindCardByNumber(string number)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].Number == number)
                    return i;
            }

            return -1;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Creates the lists
                Cards = new List<Card>();

                //Some card examples
                Cards.Add(
                    new Card()
                    {
                        Name = "Luca",
                        Surname = "Rossi",
                        Age = 34,
                        Address = "Viale Portorico, 10",
                        City = "Empoli",

                    }
                );

                Cards.Add(
                    new Card()
                    {
                        Name = "Matteo",
                        Surname = "Brandolini",
                        Age = 45,
                        Address = "Via Anna Frank, 58",
                        City = "Gallipoli"
                    }
                );

                Cards[0].InsertTransaction(100, DateTime.Now);
                Cards[0].InsertTransaction(10, DateTime.Now);
                Cards[0].InsertTransaction(26.2, DateTime.Now);
                Cards[1].InsertTransaction(50, DateTime.Now);

                Session["cards"] = Cards;
                Session["updating"] = _updating;

                // Show cards
                BindCardsData();
            }
            else
            {
                // Restore values
                Cards = (List<Card>)Session["cards"];
                _updating = (bool)Session["updating"];
            }
        }

        #region Master
        private void BindCardsData()
        {
            grdMaster.DataSource = Cards;
            grdMaster.DataBind();
        }

        private void BindTransactionData()
        {
            if (grdMaster.SelectedRow.RowIndex != -1)
            {
                grdDetails.DataSource = Cards[grdMaster.SelectedRow.RowIndex].Transactions;
                grdDetails.DataBind();
            }
        }

        /// <summary>
        /// Verify if there is a row being updating and cancel the edit event
        /// </summary>
        private void DeleteUpdatingRowOnLostFocus()
        {
            if (_updating)
            {
                grdMaster.EditIndex = -1;
                grdMaster_RowCancelingEdit(null, null);
            }
        }

        protected void btnAddCard_Click(object sender, EventArgs e)
        {
            //DeleteUpdatingRowOnLostFocus();
            Cards.Add(new Card());
            grdMaster.EditIndex = grdMaster.Rows.Count;
            Session["cards"] = Cards;
            _updating = true;
            Session["updating"] = _updating;
            BindCardsData();
        }

        protected void grdMaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DeleteUpdatingRowOnLostFocus();
            BindTransactionData();
        }

        protected void grdMaster_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //DeleteUpdatingRowOnLostFocus();
            Cards[e.RowIndex].Transactions.Clear();
            BindTransactionData();
            Cards.RemoveAt(e.RowIndex);
            Session["cards"] = Cards;
            BindCardsData();
        }

        protected void grdMaster_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        { 
            // Controllare se il numero della card è nella lista e l'oggetto è valorizzato
            // se lo è non lo si elimina, altrimenti lo si elimina
            Cards.RemoveAt(Cards.Count - 1);
            Card.EditStaticCounter();
            grdMaster.EditIndex = -1;
            Session["cards"] = Cards;
            BindCardsData();
        }

        protected void grdMaster_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            GridViewRow row = grdMaster.Rows[e.RowIndex];



            // Valuates the attributes
            Cards[row.DataItemIndex].Name = ((TextBox)row.Cells[2].Controls[1]).Text;
            Cards[row.DataItemIndex].Surname = ((TextBox)row.Cells[3].Controls[1]).Text;
            Cards[row.DataItemIndex].Address = ((TextBox)row.Cells[5].Controls[1]).Text;
            Cards[row.DataItemIndex].City = ((TextBox)row.Cells[6].Controls[1]).Text;
            string age = ((TextBox)row.Cells[4].Controls[1]).Text;
            Cards[row.DataItemIndex].Age = int.Parse(age);

            grdMaster.EditIndex = -1;
            _updating = false;
            Session["updating"] = _updating;
            Session["cards"] = Cards;
            BindCardsData();
        }

        protected void grdMaster_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //DeleteUpdatingRowOnLostFocus();
            grdMaster.EditIndex = e.NewEditIndex;
            BindCardsData();
        }

        #endregion

        protected void AgeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custval = new CustomValidator();
            custval = (CustomValidator)source;
            GridViewRow gvr = (GridViewRow)custval.NamingContainer;
            TextBox txtNm = (TextBox)gvr.FindControl("TextBox3");
            string value = txtNm.Text;
            if (int.TryParse(value, out int age) && age >= 18)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void btnAddTransaction_Click(object sender, EventArgs e)
        {
            int selected = grdMaster.SelectedRow.RowIndex;
            lblError.Text = "";

            if (selected != -1)
            {
                double value = 0;
                if(!double.TryParse(txtAmout.Text, out value) || value <= 0)
                {
                    lblError.Text = "Il valore deve essere un numero positivo";
                    return;
                }

                Cards[selected].InsertTransaction(value, DateTime.Now);
                BindTransactionData();
            }
        }
    }
}