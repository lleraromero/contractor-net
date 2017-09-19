using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinarySample
{
    public interface IFramework0
    {
        void Callback();
    }
    public interface IFramework1
    {
        void Callback(object arg);
    }
    public interface IFramework1R
    {
        object Callback(object arg);
    }
}
