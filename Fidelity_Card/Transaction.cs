using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity_Card
{
    public class Transaction
    {
        private float _amount;

        public string CardNumber { get; set; }
        public float Amount
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

    }
}