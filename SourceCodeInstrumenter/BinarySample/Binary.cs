namespace BinarySample
{
    public class Binary
    {
        public Binary(IFramework0 callbackReceiver)
        {
            receiver0 = callbackReceiver;
        }

        public Binary(IFramework1 callbackReceiver)
        {
            receiver1 = callbackReceiver;
        }

        public Binary(IFramework1R callbackReceiver)
        {
            receiver2 = callbackReceiver;
        }

        public void performCallback0_Once()
        {
            receiver0.Callback();
        }

        public void performCallback0_Twice()
        {
            receiver0.Callback();
            receiver0.Callback();
        }

        public void performCallback1_Once()
        {
            object arg = new object();
            receiver1.Callback(arg);
        }

        public object performCallback1_Once_Returning(object arg0)
        {
            object val = receiver2.Callback(arg0);
            return val;
        }
        public object performCallback1_Once_Returning()
        {
            object arg = "hola";
            object val = receiver2.Callback(arg);
            return val;
        }

        public static object Test(object a)
        {
            return new object();
        }

        public static object Test(object a, object b)
        {
            return new object();
        }

        public static object Test(object a, object b, object c)
        {
            return new object();
        }

        public static int TestScalarRet(object a, object b)
        {
            return 0;
        }

        public IFramework0 receiver0 { get; set; }
        public IFramework1 receiver1 { get; set; }
        public IFramework1R receiver2 { get; set; }
    }
}