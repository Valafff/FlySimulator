using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlySimulator
{

	 public class flightRecorder
	{

		public struct flightRecorderData
		{
			public	DateTime contorlTime;
			public string controlStatus;
			public int controlSpeed;
			public int controlHeight;
			public double controlFuelLevel;
			public double controlEngineThrust;
			[XmlIgnore]//Вместо [NonSerialized]
			public bool inAir;
			[XmlIgnore]//Вместо [NonSerialized]
			public bool onGround;

        }
		public flightRecorderData temp;
		public List<flightRecorderData> blackRecorder;

        //public flightRecorder()  {  }
        public flightRecorder()
        {
			blackRecorder = new List<flightRecorderData>();
			temp = new flightRecorderData();
		}
        public void record(bool arg_airPlaneOnGround, bool arg_airplaneInAir, int arg_speed, int arg_height, double arg_fuel, double arg_thrust)
        {
			string status;
			if (arg_airplaneInAir && !arg_airPlaneOnGround)
			{
				status = "самолет в воздухе";
			}
			else
			{
				status = "самолет на земле";
			}
			temp.contorlTime = DateTime.Now;
			temp.controlStatus = status;
			temp.controlSpeed = arg_speed; 
			temp.controlHeight = arg_height;
			temp.controlFuelLevel = arg_fuel;
			temp.controlEngineThrust = arg_thrust;
			temp.inAir = arg_airplaneInAir;
			temp.onGround = arg_airPlaneOnGround;
			blackRecorder.Add(temp);
		}
		public void recorderSerialize()
		{
			string filename;
			//Запись в файл данных черного ящика
			XmlSerializer flightrecorder = new XmlSerializer(typeof(List<flightRecorderData>));
			filename = $"FlightRecorder_{Convert.ToString(DateTime.Now.ToShortDateString())}_{Convert.ToString(DateTime.Now.Hour)}_{Convert.ToString(DateTime.Now.Minute)}.xml";
			try
			{
				using (Stream flightrecorderStream = File.Create(filename))
				{
					flightrecorder.Serialize(flightrecorderStream, blackRecorder);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

    }
}
