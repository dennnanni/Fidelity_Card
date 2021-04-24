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
        private List<Card> Cards
        {
            get => (List<Card>)Session["cards"];

            set
            {
                Session["cards"] = value;
            }
        }

        private bool IsCreating
        {
            get => (bool)Session["creating"];
            set
            {
                Session["creating"] = value;
            }
        }

        private bool IsEditing
        {
            get => (bool)Session["updating"];
            set
            {
                Session["updating"] = value;
            }
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

                IsCreating = false;
                IsEditing = false;

                // Show cards
                BindCardsData();
            }
        }

        #region Master
        private void BindCardsData()
        {
            grdMaster.DataSource = Cards;
            grdMaster.DataBind();
        }

        private void BindTransactionData(int index)
        {
            if (index != -1)
            {
                grdDetails.DataSource = Cards[index].Transactions;
                grdDetails.DataBind();
            }
        }

        /// <summary>
        /// Check if there is a blank row created and deletes it
        /// </summary>
        private void DeleteCreatedRowOnLostFocus()
        {
            if (IsCreating)
            {
                Cards.RemoveAt(Cards.Count - 1);
                Card.EditStaticCounter();
                grdMaster.EditIndex = -1;
                IsCreating = false;
                BindCardsData();
            }
        }

        private void CancelEditingOnLostFocus()
        {
            if (IsEditing)
            {
                grdMaster.EditIndex = -1;
                IsEditing = false;
                BindCardsData();
            }
        }

        protected void btnAddCard_Click(object sender, EventArgs e)
        {
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();
            Cards.Add(new Card());
            grdMaster.EditIndex = grdMaster.Rows.Count;
            IsCreating = true;
            BindCardsData();
        }

        protected void grdMaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();
            BindTransactionData(grdMaster.SelectedRow.RowIndex);
        }

        protected void grdMaster_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();
            // Rebinds transactions only if there is something to delete
            if(Cards[e.RowIndex].Transactions.Count != 0)
            {
                Cards[e.RowIndex].Transactions.Clear();
                BindTransactionData(e.RowIndex);
            }
            Cards.RemoveAt(e.RowIndex);
            BindCardsData();
        }

        protected void grdMaster_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();
            grdMaster.EditIndex = -1;
            IsCreating = false;
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
            IsCreating = false;
            BindCardsData();
        }

        protected void grdMaster_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();
            IsEditing = true;
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
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();

            if (grdMaster.SelectedRow.RowIndex == -1)
            {
                lblError.Text = "Nessuna carta selezionata";
                return;
            }

            int selected = grdMaster.SelectedRow.RowIndex;
            lblError.Text = "";

            if (selected != -1)
            {
                double value = 0;
                string check = txtAmout.Text.Split(new char[] { '.', ',' })[0];
                if(!double.TryParse(check, out value) || value <= 0)
                {
                    lblError.Text = "Il valore deve essere un numero positivo";
                    return;
                }

                Cards[selected].InsertTransaction(value, DateTime.Now);
                BindTransactionData(selected);
            }
        }
    }
}