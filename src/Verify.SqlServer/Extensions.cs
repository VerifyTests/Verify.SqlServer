using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

static class Extensions
{
    public static Dictionary<string, object> ToDictionary(this DbParameterCollection collection)
    {
        Dictionary<string, object> dictionary = new();
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