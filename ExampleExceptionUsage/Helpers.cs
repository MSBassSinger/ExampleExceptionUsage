using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleExceptionUsage
{
    public static class Helpers
    {


        /// <summary>
        /// Method to get all the exception messages, generally when the outer 
        /// exception has, or may have, inner exceptions.
        /// 
        /// NOTE: An instance of the StringBuilder class is not used here as the number of inner 
        ///       exceptions is too small to justify it.
        /// </summary>
        /// <param name="ex2Examine">Exception to examine.</param>
        /// <param name="messageDelimiter">Delimiter to use between messages.  If null or zero-length, the Environment.NewLine string will be used.</param>
        /// <param name="inlineDelimiter">Delimiter to use within a message line.  Do NOT use a space, =, [, or ], as these are reserved for internal use.  If null or zero-length, a semicolon will be used.</param>
        /// <returns>String with the error messages.</returns>
        public static String GetExceptionMessages(this Exception ex2Examine, String messageDelimiter, String inlineDelimiter)
        {

            String retValue = "";
            String message = "";

            try
            {
                // Make sure there is a delimiter string to use.
                if (String.IsNullOrEmpty(messageDelimiter))
                {
                    messageDelimiter = Environment.NewLine;
                }

                // Make sure there is a delimiter string to use.
                if (String.IsNullOrEmpty(inlineDelimiter))
                {
                    inlineDelimiter = ";";
                }

                // Make sure the exception is not null.  As an extension method, the exception
                // being null is highly unlikely.
                if (ex2Examine != null)
                {
                    // Get an anchor for the initial exception.
                    dynamic nextException = ex2Examine;

                    // Initialize the string to use for this message.
                    message = "";

                    // We need to loop through all child exceptions to get all the messages.
                    // For example, an exception caught when using a SqlClient may not
                    // show a message that explains the problem.  There may be 1, 2, or even 3 
                    // inner exceptions stacked up. The deepest will likely have the cause
                    // of the failure in its message.  So it is a good practice to capture
                    // all the messages, pulled from each instance.
                    while (nextException != null)
                    {
                        // Get the exception type and the message, and account for the message being null.
                        message += nextException.GetType().FullName + "=[" + (nextException.Message ?? "NULL") + "]";

                        // If there is a Source value, add it to the message.  If not, ignore it.
                        if (!String.IsNullOrWhiteSpace(nextException.Source))
                        {
                            message += $"{inlineDelimiter} Source=[" + nextException.Source + "]";
                        }

                        // If no inner exception, then we are done.
                        if (nextException.InnerException == null)
                        {
                            break;
                        }
                        else
                        {
                            // Move to the next inner exception.
                            nextException = nextException.InnerException;
                        }

                        // add the message delimiter to the string
                        message += messageDelimiter;

                    }

                }

                // Remove any leading or trailing spaces.
                retValue = message.Trim();

                // Remove any trailing message delimiter.
                if (retValue.EndsWith(messageDelimiter))
                {
                    retValue = retValue.Substring(0, retValue.Length - messageDelimiter.Length);
                }

            }
            catch (Exception exUnhandled)
            {
                exUnhandled.Data.AddCheck("ex2Examine", ex2Examine == null ? "NULL" : ex2Examine.GetType().Name);

                throw;
            }

            return retValue;

        }  // END public static String GetExceptionMessages( ... )


        /// <summary>
        /// IDictionary extension method that is an enhanced Add to check to see if a key exists, and if so, 
        /// adds the key with an ordinal appended to the key name to prevent overwrite.
        /// This is useful with the Exception.Data IDictionary collection, among other
        /// IDictionary implementations.
        /// </summary>
        /// <param name="dct">The IDictionaryimplementation</param>
        /// <param name="dataKey">The string key for the name-value pair.</param>
        /// <param name="dataValue">The value for the name-value pair.  Accepts any data type, which is resolved to the type at runtime.</param>
        public static void AddCheck(this IDictionary dct, String dataKey, dynamic dataValue)
        {

            if (dct != null)
            {
                if (dct.Contains(dataKey))
                {
                    for (int i = 1; i < 101; i++)
                    {
                        String newKey = dataKey + "-" + i.ToString();

                        if (!dct.Contains(newKey))
                        {
                            if (dataValue == null)
                            {
                                dct.Add(newKey, "NULL");
                            }
                            else
                            {
                                dct.Add(newKey, dataValue);
                            }

                            break;
                        }
                    }
                }
                else
                {
                    dct.Add(dataKey, dataValue);
                }
            }
        }  // END public static void AddCheck( ... )

        /// <summary>
        /// Iterates through all the exceptions (outer and inner) for the name-value pairs in each Exception's Data collection.
        /// </summary>
        /// <param name="ex2Examine">Outer exception to check.</param>
        /// <returns></returns>
        public static String GetExceptionData(this Exception ex2Examine)
        {

            String retVal = "";
            String data = "";

            try
            {

                if (ex2Examine != null)
                {

                    Exception nextException = ex2Examine;

                    // We need to loop through all child exceptions to get all the Data collection 
                    // name-value pairs. For example, an exception caught when using a SqlClient may not
                    // show data that helps explains the problem.  There may be 1, 2, or even 3 
                    // inner exceptions stacked up. The deepest will likely have data (if it has any) related 
                    // to the failure in its data collection.  So it is a good practice to capture
                    // all the data collection, pulled from each exception/inner exception.
                    while (nextException != null)
                    {

                        data = "";

                        // The Exception provides a Data collection of name-value
                        // pairs.  This provides a means, at each method level from 
                        // initiation up through the stack, to capture the runtime data
                        // which helps diagnose the problem.
                        if (nextException.Data.Count > 0)
                        {
                            foreach (DictionaryEntry item in nextException.Data)
                            {
                                data += "{" + item.Key.ToString() + "}={" + (item.Value ?? "NULL").ToString() + "}|";
                            }

                            data = data.Substring(0, data.Length - 1);
                        }

                        if ((data.Length > 0))
                        {
                            retVal += nextException.GetType().Name + " Data=[" + data + "]";
                        }
                        else
                        {
                            retVal += nextException.GetType().Name + " Data=[None]";
                        }

                        if (nextException.InnerException == null)
                        {
                            break;
                        }
                        else
                        {
                            nextException = nextException.InnerException;
                        }

                        retVal += "::";

                    }

                    retVal = retVal.Trim();

                }

                if (retVal.EndsWith("::"))
                {
                    retVal = retVal.Substring(0, retVal.Length - 2);
                }

            }
            catch (Exception exUnhandled)
            {
                exUnhandled.Data.AddCheck("ex2Examine", ex2Examine == null ? "NULL" : ex2Examine.GetType().Name);

                throw;
            }

            return retVal;

        }  // END public static String GetExceptionData( ... )




    }
}
