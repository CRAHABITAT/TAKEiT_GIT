using System.Collections.Generic;

namespace TickitNewFace.Models
{
    public class ResultatUploadFile
    {
        public List<string> uploadErrors { get; set; }
        public int nombreLignesSuccess { get; set; }
        public int nombreMagasinsSuccess { get; set; }
        public bool MAJAllLangue { get; set; }
    }
}