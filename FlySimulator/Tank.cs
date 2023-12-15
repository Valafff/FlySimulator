

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlySimulator
{

	internal class Tank
	{
		readonly double VOLUME = 50000;
		double fuelLevel;
		public double Change_Fuel_Level
		{
			get { return fuelLevel; }
			set
			{
				if (value > VOLUME) { value = VOLUME; fuelLevel = value; }
				else if (value <= 0) { value = 0; fuelLevel = value; }
				else { fuelLevel = value; }
			}
		}
		public double Volume { get { return VOLUME;} }

		public double giveFuel(double amountFuel)
		{
			fuelLevel -= amountFuel;
			if (fuelLevel < 0) { fuelLevel = 0;}
			return fuelLevel;
		}

		public override string ToString()
		{
			//return $"Объем бака {VOLUME} л\nОбъем топлива в баке {fuelLevel} л";
			return $"Объем топлива в баке {fuelLevel} л";
		}

		public Tank(double arg_vol, double arg_fuelLevel)
        {
			VOLUME = arg_vol;
			Change_Fuel_Level = arg_fuelLevel;
        }


    }
}
