namespace ATIS.Helpers;

public static class CastHelper
{

    // Needed for the ObjToNullableIntCast BindBack workaround to be used, since BindBack can only be used when function binding is used in the source -> target binding
    public static object NullableIntToObjCast(int? num)
    {
        return num;
    }

    // Workaround to perform the obj -> int? cast
    public static int? ObjToNullableIntCast(object obj)
    {
        return (int?)obj;
    }
}
