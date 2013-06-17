using System;

public static class EnumExtensionMethods
{
	public static int IntValue (this HydraControllerId id)
	{
		return (int) id;
	}
}
