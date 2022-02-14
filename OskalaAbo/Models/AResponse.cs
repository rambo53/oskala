using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models
{
    public class AResponse
    {
        public bool isSuccess { get; set; } //false
        public string comments { get; set; }
        public int errorCode { get; set; } //0
        public string errorMessage { get; set; } //0
        public long timeticks { get; set; }

        public AResponse()
        {
            isSuccess = false;
            comments = "";
            errorCode = 0;
            errorMessage = "";
            timeticks = System.DateTime.Now.TimeOfDay.Ticks;
        }
    }



    #region "Types primitifs"
    public class ABooleanResponse : AResponse
    {
        public bool result { get; set; }
        public long queryTime { get; set; }
        public ABooleanResponse() { result = false; }
    }




    public class AStringResponse : AResponse
    {
        public string result { get; set; }
        public long queryTime { get; set; }
        public AStringResponse() { result = ""; }
    }



    public class AIntegerResponse : AResponse
    {
        public int result { get; set; }
        public long queryTime { get; set; }
        public AIntegerResponse() { result = -1; }
    }



    public class ALongResponse : AResponse
    {
        public long result { get; set; }
        public long queryTime { get; set; }
        public ALongResponse() { result = -1; }
    }



    public class AFloatResponse : AResponse
    {
        public double result { get; set; }
        public long queryTime { get; set; }
        public AFloatResponse() { result = -1; }
    }
    #endregion






    public class CObjectsTResponse<T> : AResponse
    {
        public T[] result { get; set; }
        public int count { get; set; }
        public long queryTime { get; set; }
        public CObjectsTResponse() { result = default(T[]); count = 0; queryTime = 0; }
    }



    public class AObjectTResponse<T> : AResponse
    {
        public T result { get; set; }
        public long queryTime { get; set; }
        public AObjectTResponse() { result = default(T); queryTime = 0; }
    }
}