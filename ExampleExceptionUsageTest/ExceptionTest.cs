using System.ComponentModel.Design;
using ExampleExceptionUsage;

namespace ExampleExceptionUsageTest
{
    [TestClass]
    public class ExceptionTest
    {

        private static String m_CurrentFolder = "";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {

            m_CurrentFolder = Environment.CurrentDirectory;

            if (!File.Exists($@"{m_CurrentFolder}\TestingFile.txt"))
            {
                File.WriteAllText($@"{m_CurrentFolder}\TestingFile.txt", "This is a test file");
            }

            if (!File.Exists($@"{m_CurrentFolder}\TestingFileEmpty.txt"))
            {
                File.WriteAllText($@"{m_CurrentFolder}\TestingFileEmpty.txt", "");
            }
        }


        [TestMethod]
        [DataRow(@"C:\NoFile.txt")]
        [DataRow(@"C:\pagefile.sys")]
        [DataRow(@"TestingFile.txt")]
        [DataRow(@"TestingFileEmpty.txt")]
        public void GetFileContentsSimpleTest(String fileName)
        {
            FileGetter fg = null;

            String fullyQualifiedFileName = "";

            try
            {
                fg = new FileGetter();

                if (fileName.StartsWith(@"C:\"))
                {
                    fullyQualifiedFileName = fileName;
                }
                else
                {
                    fullyQualifiedFileName = $@"{m_CurrentFolder}\{fileName}";
                }

                String results = fg.GetFileContentsSimple(fullyQualifiedFileName);

                if (results == null)
                {
                    Assert.Fail($"File [{fullyQualifiedFileName}] not able to be read.");
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"Success. File [{fullyQualifiedFileName}] found and read.");
                }
            }
            catch (Exception ex)
            {
                String exceptionMessage = ex.GetExceptionMessages("", "");
                String exceptionData = ex.GetExceptionData();

                Assert.Fail($"File [{fullyQualifiedFileName}] Exception thrown: {exceptionMessage}  Data collected: {exceptionData}");
            }
            finally
            {
                fg = null;
            }

        }

        [TestMethod]
        [DataRow(@"C:\NoFile.txt")]
        [DataRow(@"C:\pagefile.sys")]
        [DataRow(@"TestingFile.txt")]
        [DataRow(@"TestingFileEmpty.txt")]
        public void GetFileContentsTraditionalStackedCatchTest(String fileName)
        {
            FileGetter fg = null;

            String fullyQualifiedFileName = "";

            try
            {
                fg = new FileGetter();

                if (fileName.StartsWith(@"C:\"))
                {
                    fullyQualifiedFileName = fileName;
                }
                else
                {
                    fullyQualifiedFileName = $@"{m_CurrentFolder}\{fileName}";
                }

                String results = fg.GetFileContentsTraditionalStackedCatch(fullyQualifiedFileName);

                if (results == null)
                {
                    Assert.Fail($"File [{fullyQualifiedFileName}] not able to be read.");
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"Success. File [{fullyQualifiedFileName}] found and read.");
                }
            }
            catch (Exception ex)
            {
                String exceptionMessage = ex.GetExceptionMessages("", "");
                String exceptionData = ex.GetExceptionData();

                Assert.Fail($"File [{fullyQualifiedFileName}] Exception thrown: {exceptionMessage}  Data collected: {exceptionData}");
            }
            finally
            {
                fg = null;
            }

        }

        [TestMethod]
        [DataRow(@"C:\NoFile.txt")]
        [DataRow(@"C:\pagefile.sys")]
        [DataRow(@"TestingFile.txt")]
        [DataRow(@"TestingFileEmpty.txt")]
        public void GetFileContentsFilteredCatchTest(String fileName)
        {
            FileGetter fg = null;

            String fullyQualifiedFileName = "";

            try
            {
                fg = new FileGetter();

                if (fileName.StartsWith(@"C:\"))
                {
                    fullyQualifiedFileName = fileName;
                }
                else
                {
                    fullyQualifiedFileName = $@"{m_CurrentFolder}\{fileName}";
                }

                String results = fg.GetFileContentsFilteredCatch(fullyQualifiedFileName);

                if (results == null)
                {
                    Assert.Fail($"File [{fullyQualifiedFileName}] not able to be read.");
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"Success. File [{fullyQualifiedFileName}] found and read.");
                }
            }
            catch (Exception ex)
            {
                String exceptionMessage = ex.GetExceptionMessages("", "");
                String exceptionData = ex.GetExceptionData();

                Assert.Fail($"File [{fullyQualifiedFileName}] Exception thrown: {exceptionMessage}  Data collected: {exceptionData}");
            }
            finally
            {
                fg = null;
            }

        }
    }
}