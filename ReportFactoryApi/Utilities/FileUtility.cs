namespace ReportFactoryApi.Utilities
{
    public class FileUtility
    {
        public static string? FindFileInDirectory(string directoryPath, string fileNameWithoutExtension)
        {
            try
            {
                Path.GetFullPath(directoryPath);
            }
            catch (Exception)
            {
                throw;
            }

            string resultFileName = String.Empty;

            foreach (string fullFileName in Directory.GetFiles(directoryPath))
            {
                string filename = Path.GetFileNameWithoutExtension(fullFileName);
                if (filename.Equals(fileNameWithoutExtension))
                {
                    resultFileName = fullFileName;
                    break;
                }
            }

            if (resultFileName != String.Empty)
            {
                return resultFileName;
            }
            else
            {
                return null;
            }

        }

        public static string AddTimeStampToFilename(string filename)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{timeStamp}_{filename}";

        }
    }
}
