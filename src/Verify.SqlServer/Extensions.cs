using System.Data;
using System.Data.Common;

static class Extensions
{
    public static Dictionary<string, object> ToDictionary(this DbParameterCollection collection)
    {
        var dictionary = new Dictionary<string, object>();
        foreach (DbParameter parameter in collection)
        {
            var direction = parameter.Direction;
            if (direction == ParameterDirection.Output ||
                direction == ParameterDirection.ReturnValue)
            {
                continue;
            }

            dictionary[parameter.ParameterName] = parameter.Value;
        }

        return dictionary;
    }
}