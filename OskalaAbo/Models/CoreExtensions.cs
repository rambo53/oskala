using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models
{
    public class CoreExtensions
    {
        public static void AddParameterWithValue(DbCommand command, string parameterName, object parameterValue)
        {
            dynamic parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }
    }
}