using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlySimulator
{
	internal class Pilot
	{
		public string Name { get; set; }
        public int PenaltyPoints { get; set; }
		public List<string> exercises;

        public Pilot() { }

        public Pilot(string name, int marks = 0)
		{
			Name = name;
			PenaltyPoints = marks;
			exercises = new List<string>();
		}
	}
}
