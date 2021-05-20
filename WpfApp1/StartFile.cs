using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public class StartFile
    {
        public BitmapFrame Source { get; private set; }
        public string Name { get; }
        public string Path { get;}
        public string Arguments { get; private set; }
        public string CompanyName { get; private set; }
        public bool IsSignatureExist { get; private set; }
        public bool IsSignatureCorrect { get; private set; }
        public string Type { get; }


        public StartFile(string name, string path, string type)
        {
            Name = name;
            Path = path;
            Type = type;
            GetData();
        }

        private void GetData()
        {
            if (File.Exists(Path))
            {
                SetImage();
                SetArguments();
                SetCertificateData();
            }
        }

        private void SetCertificateData()
        {
            try
            {
                var certificate = X509Certificate.CreateFromSignedFile(Path);
                CompanyName = Regex.Match(certificate.Subject, @"^CN=(.+?),").Groups[1].ToString();
                IsSignatureExist = true;
                if (DateTime.Parse(certificate.GetExpirationDateString()) >= DateTime.Now)
                {
                    var cert2 = new X509Certificate2(certificate.Handle);
                    if (cert2.Verify())
                        IsSignatureCorrect = true;
                }
            }
            catch
            {
                var versionInf = FileVersionInfo.GetVersionInfo(Path);
                CompanyName = versionInf.CompanyName;
            }
        }

        private void SetArguments()
        {
            var res = new ProcessStartInfo(Path);
            Arguments = res.Arguments;
        }

        private void SetImage()
        {
            using var icon = Icon.ExtractAssociatedIcon(Path);
            using var bitmap = icon.ToBitmap();
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            Source = BitmapFrame.Create(stream);
        }
    }
}
