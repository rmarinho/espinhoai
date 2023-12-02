// using Microsoft.ML.OnnxRuntime;
// using Microsoft.ML.OnnxRuntime.Tensors;

namespace EspinhoAI
{
    public class ImageProcessingResult
    {
        public byte[] Image { get; private set; }
        public string Caption { get; private set; }
        public string Filename { get; private set; }

        internal ImageProcessingResult(byte[] image, string caption = null)
        {
            Image = image;
            Caption = caption;
        }
    }
}

