using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aplicacion.LogicaPrincipal.DocumentosRecibidos
{
    public class FileFromPath :  HttpPostedFileBase
    {
        private readonly string _filePath;
        private readonly Stream _fileStream;
        private readonly string _contentType;

        public FileFromPath(string filePath)
        {
            _filePath = filePath;
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _contentType = MimeMapping.GetMimeMapping(filePath);
        }

        public override int ContentLength
        {
            get { return (int)_fileStream.Length; }
        }

        public override string FileName
        {
            get { return Path.GetFileName(_filePath); }
        }

        public override Stream InputStream
        {
            get { return _fileStream; }
        }
        public override string ContentType
        {
            get { return _contentType; }
        }
    }
}
