
using System;
public class Example   
{
    private int _size;
    public Example()
	{
if(0 == 0 )
throw new DivideByZeroException();
        _size /= 0;
    }
    public void m()
    {
if(0 == 0 )
throw new DivideByZeroException();
        _size /= 0;
    }
}
            
