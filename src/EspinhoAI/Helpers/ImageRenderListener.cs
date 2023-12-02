using System;
using System.Collections.Generic;
using System.IO;
//using UglyToad.PdfPig.Content;
//using UglyToad.PdfPig;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace EspinhoAI
{
    public class ImageRenderListener : IEventListener
    {
        public ImageRenderListener(string format)
        {
            this.format = format;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (data is ImageRenderInfo imageData)
            {
                try
                {
                    PdfImageXObject imageObject = imageData.GetImage();
                    if (imageObject == null)
                    {
                        Console.WriteLine("Image could not be read.");
                    }
                    else
                    {
                        File.WriteAllBytes(string.Format(format, index++, imageObject.IdentifyImageFileExtension()), imageObject.GetImageBytes());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Image could not be read: {0}.", ex.Message);
                }
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }

        string format;
        int index = 0;
    }

}
