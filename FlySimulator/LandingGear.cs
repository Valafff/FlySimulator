using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySimulator
{
	internal class LandingGear
	{
        public bool GearHidden { get; set; }
		public bool BrakeIsOn { get; set;}
        public LandingGear(bool arg_brake_is_on = true, bool arg_gearhidden = false)
        {
            BrakeIsOn = arg_brake_is_on;
            GearHidden = arg_gearhidden;
        }
		public override string ToString()
		{
			string brakeStatus;
			string gearStatus;
			if (GearHidden){gearStatus = "Шасси убраны";}
			else{gearStatus = "Шасси выпущены";}
			if (BrakeIsOn) { brakeStatus = "Тормоз работает"; }
			else{ brakeStatus = "Тормоз убран";}
			return $"{gearStatus}\n" +$"{brakeStatus}";
		}
	}
}
