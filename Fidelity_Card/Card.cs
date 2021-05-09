using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity_Card
{
    public class Card
    {
        const int NUMBER_MAX_CHAR = 5;
        private int _age = 18;
        private int _points = 0;
        private int _cardNumber;

        // Properties
        public string Number
        {
            get
            {
                int length = _cardNumber.ToString().Length;
                string value = _cardNumber.ToString();
                
                // Returns a string with fixed number of chars
                if (length < 10)
                {
                    var filler = new string('0', NUMBER_MAX_CHAR - length);
                    value = filler + value;
                }
                return string.Format("{0:x8}", value);
            }
            set
            {
                _cardNumber = int.Parse(value);
            }
        }

        public static int Counter { get; set; } = 0;

        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age
        {
            get => _age;
            set
            {
                if (value < 18)
                    throw new Exception("Il cliente deve essere maggiorenne.");

                _age = value;
            }
        }
        public string Address { get; set; }
        public string City { get; set; }
        public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

        public Card()
        {
            Counter++;
            Number = Counter.ToString();
        }

        public void InsertTransaction(double amount, DateTime date)
        {
            _points += (int)amount;
            Transactions.Add(new Transaction(_points, date));
        }

        public static void EditStaticCounter(int value)
        {
            Counter = value;
        }

    }
}