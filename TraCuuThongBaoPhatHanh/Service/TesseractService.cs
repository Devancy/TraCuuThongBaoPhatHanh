using System;
using System.Diagnostics;
using Tesseract;

namespace TraCuuThongBaoPhatHanh.Service
{
    public class TesseractService : IBaseService
    {
        public string ImageOcrToText(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
                using (var img = Pix.LoadFromFile(imagePath))
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
    }
}
