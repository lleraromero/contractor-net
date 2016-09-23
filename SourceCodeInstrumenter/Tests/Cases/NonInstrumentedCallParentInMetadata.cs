using System;

namespace Tests.Cases
{
    class NonInstrumentedCallParentInMetadata
    {
        public static void Main(string[] args)
        {
            Persona persona = new Persona();
            string val = persona.ToString();
            Console.WriteLine(val);
        }
        class Persona { }
    }
}
