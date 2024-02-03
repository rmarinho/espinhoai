using System;
namespace EspinhoAI.Models
{
    public class DocPage
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public double PageWidth { get; set; }
        public double PageHeight { get; set; }
        public string Path { get; set; }
        public string PdfPath { get; set; }
        public string? Publication { get; set; }
        public int DocId { get; set; }
        public bool IsImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? PdfPigOcrPagePath { get; set; }
        public string? AzureOcrPagePath { get; set; }
    }
}


