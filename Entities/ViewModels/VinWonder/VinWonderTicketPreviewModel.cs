using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.VinWonder
{
    public class VinWonderTicketPreviewModel
    {
        public string subject { get; set; }
        public List<VinWonderTicketPreviewEmail> to_email { get; set; }
        public List<VinWonderTicketPreviewEmail> cc_email { get; set; }
        public List<VinWonderTicketPreviewEmail> bcc_email { get; set; }
        public string body { get; set; }
        public List<LightGalleryViewModel> file_attachment { get; set; }
        public ContactClient contact_client { get; set; }
        public bool can_sendemail { get; set; }
    }
    public class VinWonderTicketPreviewEmail
    {
        public string email { get; set; }
        public string username { get; set; }
      
    } 
    public class LightGalleryViewModel
    {
        public string thumb { get; set; }
        public string url { get; set; }
        public string ext { get; set; }
      
    }
}
