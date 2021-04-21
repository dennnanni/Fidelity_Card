using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity_Card
{
    public class Transaction
    {
        private double _amount;

        public double Amount
        {
            get => _amount;
            set
            {
                if (value <= 0)
                    throw new Exception("La spesa non può essere negativa.");

                _amount = value;
            }
        }
        public DateTime Date { get; set; }

        public Transaction(double amount, DateTime date)
        {
            _amount = amount;
            Date = date;
        }
    }
}