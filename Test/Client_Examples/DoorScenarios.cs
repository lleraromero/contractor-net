using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API_Examples;

namespace Client_Examples
{
	//using Door = EnrichedDoor;

	public class DoorScenarios
	{

		public void AlarmScenario()
		{
			Door door = new Door();

			door.Start();	// Departing station
			door.Alarm();	// Emergency button pressed...
			door.Safe(); 	// ...but it was a false alarm
			door.Stop();	// Arriving at next station
			door.Open();	// Opening doors
			door.Close();	// Getting ready to depart again
		}

		//public void NormalScenario()
		//{
		//    var door = new Door();
		//    // In station
		//    door.Open();
		//    door.Close();
		//    //Traveling
		//    door.Start();
		//    door.Stop();
		//    //In station
		//    door.Open();
		//    door.Close();
		//}

		///* 
		// * if (llego a estacion y no emergencia)
		// *  abrir puerta
		// * 
		// * tiene un error porque no contempla el siguiente escenario:
		// * 
		// * 1. startmoving
		// * 2. beginemergency (se abrio la puerta)
		// * 3. endemergency
		// * 4. llega a estacion
		// * 5. intenta abrir puerta (ver codigo arriba)
		// * 6. falla porque ya estaba abierta
		// * 
		// * esto es encontrado gracias a verificar con CC la version instrumentada
		// * y se puede validar con el grafo
		// */
		//public void AlarmScenario2()
		//{
		//    var door = new Door();
		//    // In station
		//    door.Open();
		//    door.Close();
		//    //Traveling
		//    door.Start();
		//    door.Alarm();
		//    //Emergency
		//    door.Safe();
		//    door.Stop();
		//    //In station
		//    door.Open();
		//    door.Close();
		//}
	}
}
