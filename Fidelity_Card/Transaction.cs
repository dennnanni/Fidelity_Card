using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity_Card
{
    public class Transaction
    {
        private int _currentPoints;

        public int CurrentPoints
        {
            get => _currentPoints;
            set
            {
                if (value <= 0)
                    throw new Exception("La spesa non può essere negativa.");

                _currentPoints = value;
            }
        }
        public double FirstThreshold
        {
            get; set;

        }
        public double SecondThreshold {
            get; set;

        }
        public DateTime Date { get; set; }
        public string Message { get; set; }

        public Transaction()
        {

        }

        public Transaction(int amount, DateTime date) : this()
        {
            _currentPoints = amount;
            Date = date;
            FirstThreshold = CalculatePercentage(100);
            SecondThreshold = CalculatePercentage(200);
            Message = VerifyThreshold();
            
        }

        public double CalculatePercentage(int checkpoint)
        {
            double percentage = (double)_currentPoints * 100 / checkpoint;

            if (percentage > 100)
                percentage = 100;

            return percentage;
        }

        private string VerifyThreshold() 
        { 
            if(_currentPoints == 0)
            {
                return "Buona raccolta";
            }
            else if(FirstThreshold < 100)
            {
                return $"Mancano ancora {100 - _currentPoints} punti per raggiungere il primo traguardo";
            }
            else if (_currentPoints == 100)
            {
                return "Complimenti il cliente può ritirare il frullatore";
            }
            else if (SecondThreshold < 100)
            {
                return $"Mancano ancora {200 - _currentPoints} punti per raggiungere il secondo traguardo";
            }
            else if (_currentPoints == 200)
            {
                return "Complimenti il cliente può ritirare il Tv color";
            }
            else
            {
                return "Complimenti il cliente può ritirare il Tv color";
            }
        }
    }
}