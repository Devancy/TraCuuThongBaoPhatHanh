using Tesseract;

namespace TraCuuThongBaoPhatHanh_v2
{
    public class Ocr
    {
        public static string ImageOcrToText(string imagePath)
        {
            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            using (var img = Pix.LoadFromFile(imagePath))
            using (var page = engine.Process(img))
            {
                return page.GetText();
            }
        }
    }
}
