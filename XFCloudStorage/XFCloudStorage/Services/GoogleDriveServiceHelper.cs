using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFCloudStorage.Services
{
    public class GoogleDriveServiceHelper
    {
        private DriveService driveService;

        public GoogleDriveServiceHelper(DriveService driveService)
        {
            this.driveService = driveService;
        }

        public string GetFileID(string fileName)
        {
            string fileID = "";

            FilesResource.ListRequest request = driveService.Files.List();
            request.Q = $"name='{fileName}'";
            request.Fields = "files(id, name, trashed)";
            File file = request.Execute().Files.Where(x => !((bool)x.Trashed)).FirstOrDefault();

            if (file != null)
                fileID = file.Id;

            return fileID;
        }

        public async Task<string> CreateFile()
        {
            File metadata = new File()
            {
                Parents = new List<string>() { "root" },
                MimeType = "text/plain",
                Name = "Untitled file"
            };

            var googleFile = await this.driveService.Files.Create(metadata).ExecuteAsync();
            if (googleFile == null)
            {
                throw new System.IO.IOException("Null result when requesting file creation.");
            }
            return googleFile.Id;
        }

        public async Task<FileData> ReadFile(string fileId)
        {
            FileData data = new FileData();
            var metadata = await driveService.Files.Get(fileId).ExecuteAsync();
            data.Name = metadata.Name;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                await driveService.Files.Get(fileId).DownloadAsync(ms);
                ms.Position = 0;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ms, Encoding.Default))
                {
                    var content = sr.ReadToEnd();
                    data.Content = content;
                }
            }
            return data;
        }

        public async Task SaveFile(string fileId, string name, string content)
        {
            File metadata = new File()
            {
                Name = name,

            };
            byte[] byteArray = Encoding.Default.GetBytes(content);
            await driveService.Files.Update(metadata, fileId, new System.IO.MemoryStream(byteArray), "text/plain").UploadAsync();
        }

    }

    public class FileData
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
