using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace ExpenseProcessingSystem.Services
{
    public class FileService
    {
        public void CreateFileAndFolder()
        {
            // Specify a name for your top-level folder.
            string folderName = @"c:\Top-Level Folder";

            // To create a string that specifies the path to a subfolder under your 
            // top-level folder, add a name for the subfolder to folderName.
            string pathString = Path.Combine(folderName, "SubFolder");

            // You can write out the path name directly instead of using the Combine
            // method. Combine just makes the process easier.
            //string pathString2 = @"c:\Top-Level Folder\SubFolder2";

            // You can extend the depth of your path if you want to.
            //pathString = System.IO.Path.Combine(pathString, "SubSubFolder");

            // Create the subfolder. You can verify in File Explorer that you have this
            // structure in the C: drive.
            //    Local Disk (C:)
            //        Top-Level Folder
            //            SubFolder
            Directory.CreateDirectory(pathString);

            // Create a file name for the file you want to create. 
            string fileName = System.IO.Path.GetRandomFileName();

            // This example uses a random string for the name, but you also can specify
            // a particular name.
            //string fileName = "MyNewFile.txt";

            // Use Combine again to add the file name to the path.
            pathString = System.IO.Path.Combine(pathString, fileName);

            // Verify the path that you have constructed.
            Console.WriteLine("Path to my file: {0}\n", pathString);

            // Check that the file doesn't already exist. If it doesn't exist, create
            // the file and write integers 0 - 99 to it.
            // DANGER: System.IO.File.Create will overwrite the file if it already exists.
            // This could happen even with random file names, although it is unlikely.
            if (!System.IO.File.Exists(pathString))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                {
                    for (byte i = 0; i < 100; i++)
                    {
                        fs.WriteByte(i);
                    }
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", fileName);
                return;
            }

            // Read and display the data from your file.
            try
            {
                byte[] readBuffer = System.IO.File.ReadAllBytes(pathString);
                foreach (byte b in readBuffer)
                {
                    Console.Write(b + " ");
                }
                Console.WriteLine();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

            //// Keep the console window open in debug mode.
            //System.Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();
        }

        public void CopyFileToLocation(string fileName)
        {
            fileName = fileName ?? "00283_martin.nina.png";
            string sourcePath = @"C:\Top-Level Folder\TestFolder";
            string targetPath = @"C:\Top-Level Folder\SubFolder";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }
        public void ConvertImageToBytes()
        {
        }
        public void CopyFile(string uploadedfile)
        {
            // HttpPostedFileBase myfile = Request.Files[0];
            var file = uploadedfile;
            if (/*file.ContentLength > 0*/ file.Length > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file);
                // store the file inside ~/App_Data/uploads folder
                //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                //file.SaveAs(path);
            }
        }

        //-------------------------------------------------------------------------------------
        public async Task<string> SaveFileAsync(IFormFile file, string pathToUplaod, string category)
        {
            string imageUrl = string.Empty;
            if (!Directory.Exists(pathToUplaod))
                System.IO.Directory.CreateDirectory(pathToUplaod);//Create Path of not exists

            string pathwithfileName = pathToUplaod + "\\" + GetFileName(file, true, category);
            using (var fileStream = new FileStream(pathwithfileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            imageUrl = pathwithfileName;
            return imageUrl;
        }

        public string SaveFile(IFormFile file, string pathToUplaod, string name)
        {
            string imageUrl = string.Empty;
            imageUrl = GetFileName(file, true, name);
            string pathwithfileName = pathToUplaod + "\\" + imageUrl;
            using (var fileStream = new FileStream(pathwithfileName, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return imageUrl;
        }
        public string GetFileName(IFormFile file, bool BuildUniqeName, string name)
        {
            string fileName = string.Empty;
            string strFileName = file.FileName;

            string fileExtension = GetFileExtension(file);
            if (BuildUniqeName)
            {
                string strUniqName = GetUniqueName(name, strFileName);
                fileName = strUniqName + fileExtension;
            }
            else
            {
                fileName = strFileName;
            }
            return fileName;
        }
        public string GetUniqueName(string preFix, string fileName)
        {
            string uName = preFix + "_" + DateTime.Now.ToString();
            return uName
                .Replace("/", "_")
                .Replace(":", "_")
                .Replace(" ", string.Empty)
                .Replace("PM", string.Empty)
                .Replace("AM", string.Empty);
        }
        public string GetFileExtension(IFormFile file)
        {
            string fileExtension;
            fileExtension = (file != null) ?
                file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()
                : string.Empty;
            return fileExtension;
        }
    }
}
