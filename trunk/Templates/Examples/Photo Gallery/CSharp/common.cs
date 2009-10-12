using System;
using System.Collections.Generic;
using System.IO;
using CodeSmith.Engine;

namespace CodeSmith.Samples
{
    public partial class PhotoCodeTemplate : CodeTemplate
	{
        public struct PhotoInformation
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Path { get; set; }
            public string PreviousImage { get; set; }
            public string NextImage { get; set; }
        }
        
        public string GetName(FileInfo photo)
        {
            return photo.Name.Replace(photo.Extension, "");
        }
        
        public string GetPath(FileInfo photo, bool copyImagesToOutputDirectory)
        {
            if(copyImagesToOutputDirectory)
                return Path.Combine("images", photo.Name);
        
            return photo.FullName;
        }
        
        public List<PhotoInformation> GetPhotos(string sourceDirectory, bool copyImagesToOutputDirectory)
        {
            string[] temp = Directory.GetFiles(sourceDirectory, "*.jpg");
            List<PhotoInformation> images = new List<PhotoInformation>(temp.Length);
            
            string previousPath = string.Empty;
            for(int index = 0; index < temp.Length; index++)
            {
                FileInfo info = new FileInfo(temp[index]);
                PhotoInformation photo = new PhotoInformation();
                photo.Name = GetName(info);
                photo.Description = GetName(info);
                photo.PreviousImage = previousPath;
                photo.Path = GetPath(info, copyImagesToOutputDirectory);
                
                if((index + 1) < temp.Length)
                    photo.NextImage = string.Format("{0}.html", GetName( new FileInfo(temp[index + 1])));
        
                previousPath = string.Format("{0}.html", GetName(info));
                
                images.Add(photo);
            }
            
            return images;
        }   
    }
}