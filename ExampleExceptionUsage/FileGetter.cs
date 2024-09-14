using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;

namespace ExampleExceptionUsage
{

    /// <summary>
    /// This library DLL shows example of catching exceptions.  Many times, simpler is better.
    /// It should be noted that once the code enters a catch block, the stack is unwound.  
    /// The StackTrace value is not changed. Even with "when" filters, the stack is unwound when
    /// it does enter one of the catch blocks.  See https://thomaslevesque.com/2015/06/21/exception-filters-in-c-6/
    /// for more info on how when filtering affects the stack.
    /// </summary>
    public class FileGetter
    {
        ILogger<FileGetter> m_Logger = null;

        public FileGetter()
        {
        }

        public FileGetter(ILogger<FileGetter> logger)
        {
            m_Logger = logger;
        }


        /// <summary>
        /// Simple extraction of text to be returned.
        /// </summary>
        /// <param name="fullyQualifiedFileName">Name of the file including where it is at.</param>
        /// <returns>The string value of the contents.  Null if the file file contents are not found.</returns>
        public String GetFileContentsSimple(String fullyQualifiedFileName)
        {
            String retVal = null;

            try
            {
                // Code that may throw an exception
                if (File.Exists(fullyQualifiedFileName))
                {
                    // Code that may throw an exception
                    retVal = File.ReadAllText(fullyQualifiedFileName);
                }
                else
                {
                    // Normally, I'd use the return value of null to indicate the file does not 
                    // exist.  However, for illustration purposes in this example, I'm throwing an exception.
                    throw new FileNotFoundException($"{fullyQualifiedFileName} does not exist.");
                }
            }
            catch (Exception ex)
            {
                // Code to handle the exception 
                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                if (m_Logger != null)
                {
                    String messages = ex.GetExceptionMessages("", "");

                    // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                    m_Logger.LogError(ex, $"Error reading file [{fullyQualifiedFileName}]. Messages: [{messages}]");
                }

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }

            return retVal;
        }

        /// <summary>
        /// Extraction of text to be returned, with detailed exception handling
        /// </summary>
        /// <param name="fullyQualifiedFileName">Name of the file including where it is at.</param>
        /// <returns>The string value of the contents.  Null if the file file contents are not found.</returns>
        public String GetFileContentsTraditionalStackedCatch(String fullyQualifiedFileName)
        {
            String retVal = null;

            try
            {
                // Code that may throw an exception
                if (File.Exists(fullyQualifiedFileName))
                {
                    // Code that may throw an exception
                    retVal = File.ReadAllText(fullyQualifiedFileName);
                }
                else
                {
                    // Normally, I'd use the return value of null to indicate the file does not 
                    // exist.  However, for illustration purposes in this example, I'm throwing an exception.
                    throw new FileNotFoundException($"{fullyQualifiedFileName} does not exist.");
                }
            }

            catch (Exception ex)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                if (ex is System.ArgumentException ||
                    ex is System.ArgumentNullException)
                {
                    specificMessage = $"The value used for the fully qualified file name was incorrect. [{messages}]";
                }
                else if (ex is System.IO.PathTooLongException ||
                         ex is System.IO.DirectoryNotFoundException ||
                         ex is System.IO.IOException)
                {
                    specificMessage = $"The fully qualified file name was not usable or could not be reached. [{messages}]";
                }
                else if (ex is System.IO.FileNotFoundException)
                {
                    specificMessage = $"The file matching the fully qualified file name could not be found. [{messages}]";
                }
                else if (ex is System.NotSupportedException)
                {
                    specificMessage = $"Accessing this file is not supported. [{messages}]";
                }
                else if (ex is System.UnauthorizedAccessException ||
                         ex is System.Security.SecurityException)
                {
                    specificMessage = $"The user does not have access rights to read [{fullyQualifiedFileName}]. [{messages}]";
                }
                else
                {
                    specificMessage = $"An unanticipated exception [{ex.GetBaseException().GetType().Name ?? "NULL"}] occurred.  Please check the exception messages for more information. [{messages}]";
                }

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;

            }

            return retVal;
        }

        /// <summary>
        /// Extraction of text to be returned, with detailed exception handling using when filters.
        /// NOTE: The when filter does not allow for a common set of variables to be used in whatever
        ///       catch block is used.  The variables could be defined and filled before a catch block, 
        ///       but that means doing so in normal program flow.  That adds unnecessary overhead.
        /// </summary>
        /// <param name="fullyQualifiedFileName">Name of the file including where it is at.</param>
        /// <returns>The string value of the contents.  Null if the file contents are not found.</returns>
        public String GetFileContentsFilteredCatch(String fullyQualifiedFileName)
        {
            String retVal = null;

            try
            {
                // Code that may throw an exception
                if (File.Exists(fullyQualifiedFileName))
                {
                    // Code that may throw an exception
                    retVal = File.ReadAllText(fullyQualifiedFileName);
                }
                else
                {
                    // Normally, I'd use the return value of null to indicate the file does not 
                    // exist.  However, for illustration purposes in this example, I'm throwing an exception.
                    throw new FileNotFoundException($"{fullyQualifiedFileName} does not exist.");
                }
            }

            catch (Exception ex)
            when (ex is System.ArgumentException ||
                  ex is System.ArgumentNullException)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                specificMessage = $"The value used for the fully qualified file name was incorrect. [{messages}]";

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }

            catch (Exception ex)
            when (ex is System.IO.PathTooLongException ||
                  ex is System.IO.DirectoryNotFoundException ||
                  ex is System.IO.IOException)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                specificMessage = $"The fully qualified file name was not usable or could not be reached. [{messages}]";

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }

            catch (Exception ex)
            when (ex is System.IO.FileNotFoundException)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                specificMessage = $"The file matching the fully qualified file name could not be found. [{messages}]";

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }

            catch (Exception ex)
            when (ex is System.NotSupportedException)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                specificMessage = $"Accessing this file is not supported. [{messages}]";

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }
            catch (Exception ex)
            when (ex is System.UnauthorizedAccessException ||
                  ex is System.Security.SecurityException)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                specificMessage = $"The user does not have access rights to read [{fullyQualifiedFileName}]. [{messages}]";

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }
            catch (Exception ex)
            {
                // Code to handle the exception 
                String specificMessage = "";

                // Capture runtime data that may be useful in troubleshooting.
                ex.Data.AddCheck("Fully Qualified file Name", fullyQualifiedFileName ?? "NULL");
                ex.Data.AddCheck("Machine Name", Environment.MachineName ?? "NULL");
                ex.Data.AddCheck("Current Directory", Environment.CurrentDirectory ?? "NULL");
                ex.Data.AddCheck("User Domain Name", Environment.UserDomainName ?? "NULL");
                ex.Data.AddCheck("User Name", Environment.UserName ?? "NULL");

                // Get the messages from the exception and any inner exceptions.
                String messages = ex.GetExceptionMessages(Environment.NewLine, ";");

                specificMessage = $"An unanticipated exception [{ex.GetBaseException().GetType().Name ?? "NULL"}] occurred.  Please check the exception messages for more information. [{messages}]";

                // Log the exception, which if written to do so, will capture the Data collection name-value pairs.
                m_Logger?.LogError(ex, specificMessage, fullyQualifiedFileName);

                // If you want to "fail fast", log and throw the exception again (preserves stack info).
                // If you want normal program flow to handle a return value of null, then don't throw, just log.
                throw;
            }

            return retVal;
        }

    }
}
