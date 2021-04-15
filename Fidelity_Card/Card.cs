using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity_Card
{
    public class Card
    {
        private int _age = 0;
        private int _points = 0;
        static private int _counter = 0;
        private int _cardNumber;
        const int NUMBER_MAX_CHAR = 10;

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
                    var filler = new string('0', 10 - length);
                    value = filler + value;
                }
                return string.Format("{0:x8}", value);
            }
            private set
            {
                _cardNumber = int.Parse(value);
            }
        }
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
        public int Points
        {
            get => _points;
            private set
            {
                if (value < 0)
                    throw new Exception("I punti non possono andare in negativo.");

                _points = value;
            }
        }

        public Card()
        {
            _counter++;
            Number = _counter.ToString();
        }


        public string VerifyCheckpoint(int checkpoint)
        {
            if(checkpoint - this.Points == 0)
            {
                return $"Complimenti il cliente può ritirare {(Prizes.Prize)checkpoint}";
            }
            else
            {
                return $"Mancano ancora {(float)checkpoint - this.Points} per poter ritirare {(Prizes.Prize)checkpoint}";
            }
        }

        public float CalculatePercentage(int checkpoint)
        {
            float percentage = this.Points * 100 / checkpoint;
            if (percentage > 100)
                percentage = 100;

            return percentage;
        }
    }
}