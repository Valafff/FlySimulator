using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySimulator
{
	internal class JetEngine
	{
		public bool isStarted { get; set; }
		readonly double enginemaxthrust = 15000;
		double currentThrust;

        public double ENGINEMAXTHRUST { get { return enginemaxthrust; } }
        public double CURRENTTHRUST
		{
			get { return currentThrust; }

			set
			{
				if (value < 0) { currentThrust = 0; }
				else if (value > ENGINEMAXTHRUST) { currentThrust = ENGINEMAXTHRUST; }
				else { currentThrust = value; }
			}
		}

		public double ConsumptionPerSecond;
		public double CONSUMPTIONPERSECOND
		{
			get { return ConsumptionPerSecond; }
			set
			{
				if (value <= 0) { ConsumptionPerSecond = 0; }
				else if (value > 500) { ConsumptionPerSecond = 500; }
				else { ConsumptionPerSecond = value; }
			}
		}
		public override string ToString()
		{
			string status;
			if (isStarted)
			{
				status = "ЗАПУЩЕН";
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				status = "ОСТАНОВЛЕН";
				Console.ResetColor();
			}
			return $"Двигатель {status}! Расход топлива {CONSUMPTIONPERSECOND} л/c\n" +
				$"Тяга двигателя {CURRENTTHRUST} кгс";
		}

		public void startEngine() 	{isStarted = true; 	}
		public void stopEngine() { isStarted = false; }

		public void consumptionTable()
		{
			if (isStarted == false)
			{
				CONSUMPTIONPERSECOND = 0;
			}
			if (CURRENTTHRUST == 0 && isStarted)
			{
				CONSUMPTIONPERSECOND = 1;
			}
			if (CURRENTTHRUST >0 && CURRENTTHRUST <500)
			{
				CONSUMPTIONPERSECOND = 1;
			}
			if (CURRENTTHRUST >= 500 && CURRENTTHRUST < 1000)
			{
				CONSUMPTIONPERSECOND = 2;
			}
			if (CURRENTTHRUST >= 1000 && CURRENTTHRUST < 1500)
			{
				CONSUMPTIONPERSECOND = 3;
			}
			if (CURRENTTHRUST >= 1500 && CURRENTTHRUST < 2000)
			{
				CONSUMPTIONPERSECOND = 4;
			}
			if (CURRENTTHRUST >= 2000 && CURRENTTHRUST < 3500)
			{
				CONSUMPTIONPERSECOND = 5;
			}
			if (CURRENTTHRUST >= 3500 && CURRENTTHRUST < 5000)
			{
				CONSUMPTIONPERSECOND = 6;
			}
			if (CURRENTTHRUST >= 5000 && CURRENTTHRUST < 7500)
			{
				CONSUMPTIONPERSECOND = 7;
			}
			if (CURRENTTHRUST >= 7500 && CURRENTTHRUST < 9000)
			{
				CONSUMPTIONPERSECOND = 8;
			}
			if (CURRENTTHRUST >= 9000 && CURRENTTHRUST < 11000)
			{
				CONSUMPTIONPERSECOND = 8;
			}
			if (CURRENTTHRUST >= 11000 && CURRENTTHRUST < 13000)
			{
				CONSUMPTIONPERSECOND = 15;
			}
			if (CURRENTTHRUST >= 13000 && CURRENTTHRUST < 15000)
			{
				CONSUMPTIONPERSECOND = 30;
			}




		}

		public JetEngine(double arg_max_Thrust, double arg_curr_Thrust, double arg_consumptionPerSecond, bool arg_is_started = false)
		{
			enginemaxthrust = arg_max_Thrust;
			CURRENTTHRUST = arg_curr_Thrust;
			CONSUMPTIONPERSECOND = arg_consumptionPerSecond;
			isStarted = arg_is_started;
		}

	}
}
