using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace WebAppTracingMvc.Helpers
{
    public class DocumentSettings
    {
        public static string UploadImage(IFormFile file, string FolderName)
        {
            //1-Get Located folder path
            //string FolderPath=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);
            string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", FolderName);

            //2-get file name and make it uniqe
            //string FileName =$"{Guid.NewGuid()}{file.FileName}";
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            string FileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            //3- Get file Path=> save
            string FilePath=Path.Combine(FolderPath,FileName);

            //4- open file stream to save imagename in folder 
            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            //5- Return filename
            return FileName;

        }

        public static void DeleteImage(string FileName, string FolderName) 
        {
            if(FileName is not null && FolderName is not null)
            {
                string FilePath= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Files", FolderName, FileName);
                if (File.Exists(FilePath)) 
                {
                    File.Delete(FilePath);
                }

            }
        
        }
    }
}
