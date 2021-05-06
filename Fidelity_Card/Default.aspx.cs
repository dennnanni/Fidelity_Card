using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace Fidelity_Card
{
    // CARDS UPDATE IS MISSING
    // TO DO: DELETE AND MULTI DELETION
    public partial class Default : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get => Session["ConnectionString"].ToString();
            set
            {
                Session["ConnectionString"] = value;
            }
        }

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

        #region SQL commands
        private int InsertTransaction(SqlConnection connection, Transaction t, int selected) // Parametri
        {
            SqlCommand insert = new SqlCommand(
                @"INSERT INTO Operation
                VALUES (@Points, @Threshold1, @Threshold2, @Date, @Fk, @Message);", connection);

            insert.Parameters.AddWithValue("@Points", t.CurrentPoints);
            insert.Parameters.AddWithValue("@Threshold1", t.FirstThreshold);
            insert.Parameters.AddWithValue("@Threshold2", t.SecondThreshold);
            insert.Parameters.AddWithValue("@Date", t.Date);
            insert.Parameters.AddWithValue("@Fk", Cards[selected].Number);
            insert.Parameters.AddWithValue("@Message", t.Message);

            return insert.ExecuteNonQuery();
        }

        private int InsertNewCard(SqlConnection connection, string idCard, string name, string surname, int age, string address, string city)
        {
            SqlCommand insert = new SqlCommand(
                @"INSERT INTO Card(IDCard, Name, Surname, Age, Address, City)
                VALUES (@IDCard, @Name, @Surname, @Age, @Address, @City);", connection);

            insert.Parameters.AddWithValue("@IDCard", idCard);
            insert.Parameters.AddWithValue("@Name", name);
            insert.Parameters.AddWithValue("@Surname", surname);
            insert.Parameters.AddWithValue("@Age", age);
            insert.Parameters.AddWithValue("@Address", address);
            insert.Parameters.AddWithValue("@City", city);

            return insert.ExecuteNonQuery();

        }

        private int UpdateCard(SqlConnection connection, string name, string surname, int age, string address, string city)
        {
            SqlCommand insert = new SqlCommand(
                @"UPDATE Card
                SET Name = @Name, Surname = @Surname, Age = @Age, Address = @Address, City = @City;", connection);

            insert.Parameters.AddWithValue("@Name", name);
            insert.Parameters.AddWithValue("@Surname", surname);
            insert.Parameters.AddWithValue("@Age", age);
            insert.Parameters.AddWithValue("@Address", address);
            insert.Parameters.AddWithValue("@City", city);

            return insert.ExecuteNonQuery();
        }

        private void ReadData(SqlConnection conn)
        {
            SqlCommand selCommand = new SqlCommand("SELECT * FROM Card", conn);

            SqlDataReader cards = selCommand.ExecuteReader();

            while (cards.Read())
            {
                Cards.Add(
                    new Card()
                    {
                        Number = cards["IDCard"].ToString().TrimEnd(null),
                        Name = cards["Name"].ToString().TrimEnd(null),
                        Surname = cards["Surname"].ToString().TrimEnd(null),
                        Age = int.Parse(cards["Age"].ToString().TrimEnd(null)),
                        Address = cards["Address"].ToString().TrimEnd(null),
                        City = cards["City"].ToString().TrimEnd(null)
                    }
                );
                
            }
            cards.Close();
        }

        private void ReadTransactionData(SqlConnection conn, int index)
        {
            SqlCommand selCommand = new SqlCommand(@"SELECT CurrentPoints, FirstThreshold, SecondThreshold, CurrentDate, Message
                                                    FROM Operation INNER JOIN Card ON Operation.IDCard = Card.IDCard
                                                    WHERE Operation.IDCard = @Selected;", conn);

            selCommand.Parameters.AddWithValue("@Selected", Cards[index].Number);

            SqlDataReader operations = selCommand.ExecuteReader();

            while (operations.Read())
            {
                Cards[index].Transactions.Add(
                    new Transaction()
                    {
                        CurrentPoints = int.Parse(operations["CurrentPoints"].ToString().TrimEnd(null)),
                        FirstThreshold = double.Parse(operations["FirstThreshold"].ToString().TrimEnd(null)),
                        SecondThreshold = double.Parse(operations["SecondThreshold"].ToString().TrimEnd(null)),
                        Date = DateTime.Parse(operations["CurrentDate"].ToString().TrimEnd(null)),
                        Message = operations["Message"].ToString().TrimEnd(null)
                    }
                );

            }
            operations.Close();
        }

        private void DeleteCard(SqlConnection conn, string number)
        {
            // To do
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Tentativo di connessione ad ambedue i database 
                try
                {

                    ConnectionString = "Data Source = localhost;Initial Catalog = Fidelity;Integrated Security = True;Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                        connection.Open();

                }
                catch
                {
                    try
                    {
                        ConnectionString = "Data Source=PC1304;Initial Catalog = Fidelity;User ID=sa;Password=burbero2020;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                            connection.Open();
                    }
                    catch
                    {
                        ConnectionString = "";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Impossibile collegarsi al database');", true);
                        return;
                    }
                    
                }

                // Creates the lists
                Cards = new List<Card>();

                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        ReadData(connection);
                        if(Cards.Count > 0)
                        {
                            Card.EditStaticCounter(Cards[Cards.Count - 1].Counter);
                            for(int i = 0; i < Cards.Count; i++)
                            {
                                ReadTransactionData(connection, i);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", $"alert('{ex.Message}');", true);
                    return;
                }

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
                Card.EditStaticCounter(Cards[Cards.Count - 1].Counter - 1);
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
            if(grdMaster.SelectedIndex!=-1)
                BindTransactionData(grdMaster.SelectedIndex);
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

            try
            {
                using(SqlConnection connection = new SqlConnection(ConnectionString))
                {

                    connection.Open();
                    int i = row.DataItemIndex;
                    InsertNewCard(connection, Cards[i].Number, Cards[i].Name, Cards[i].Surname, Cards[i].Age, Cards[i].Address, Cards[i].City);
                    Cards[i].Transactions.Add(new Transaction(0, DateTime.Now));
                    InsertTransaction(connection, Cards[i].Transactions[0], i);

                }

                grdMaster.EditIndex = -1;
                IsCreating = false;
                BindCardsData();
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Errore di connessione');", true);
            }
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

            if (grdMaster.SelectedIndex == -1)
            {
                lblError.Text = "Nessuna carta selezionata";
                return;
            }

            int selected = grdMaster.SelectedRow.RowIndex;
            lblError.Text = "";

            if (selected != -1)
            {
                try
                {
                    using(SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        double value = 0;
                        string check = txtAmout.Text.Split(new char[] { '.', ',' })[0];
                        if (!double.TryParse(check, out value) || value <= 0)
                        {
                            lblError.Text = "Il valore deve essere un numero positivo";
                            return;
                        }
                        
                        Cards[selected].InsertTransaction(value, DateTime.Now);
                        InsertTransaction(connection, Cards[selected].Transactions[Cards[selected].Transactions.Count - 1], selected);

                        BindTransactionData(selected);
                    }
                }
                catch
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Errore di connessione');", true);
                }
            }
        }

        protected void btnMultipleDeletion_Click(object sender, EventArgs e)
        {
            // Undo creating or updating
            DeleteCreatedRowOnLostFocus();
            CancelEditingOnLostFocus();

            int counter = -1;
            // The counter goes backwards in order to do not slide the indexes every deletion
            for(int i = Cards.Count - 1 ; i >= 0; i--)
            {
                if (((CheckBox)grdMaster.Rows[i].Cells[7].Controls[1]).Checked)
                {
                    Cards.RemoveAt(i);
                    counter++;
                }
            }

            if(counter == -1)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Nessuna card selezionata');", true);
            }
            else
            {
                BindCardsData();
                grdDetails.DataSource = null;
                grdDetails.DataBind();
            }
        }
    }
}