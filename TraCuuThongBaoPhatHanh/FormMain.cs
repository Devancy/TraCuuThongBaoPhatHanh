using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace TraCuuThongBaoPhatHanh
{
    public partial class FormMain : Form
    {
        private IWebDriver _driver;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly string _mainUrl = ConfigurationManager.AppSettings["host"] ?? "http://tracuuhoadon.gdt.gov.vn/tbphtc.html";
        public FormMain()
        {
            InitializeComponent();

            comboBoxYear.DataSource = new[] { DateTime.Today.Year, DateTime.Today.Year - 1, DateTime.Today.Year - 2 };
        }

        private async void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (buttonSubmit.Text.StartsWith("T"))
            {
                var token = _tokenSource.Token;
                await (Task.Factory.StartNew(() => Action(token), token));
            }
            else
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();
            }

            //await Action(new CancellationToken());
        }

        private async Task Action(CancellationToken ct)
        {
            try
            {
                Invoke((MethodInvoker)(() =>
                {
                    buttonSubmit.Text = "Huỷ tra cứu";
                }));

                if (string.IsNullOrWhiteSpace(textBoxTaxCode.Text))
                {
                    Blink(labelNote, "Nhập hộ em cái mã số thuế", Color.Red);
                    Invoke((MethodInvoker)(() =>
                    {
                        textBoxTaxCode.Focus();
                        buttonSubmit.Text = "Tra cứu";
                    }));
                    return;
                }

                Invoke((MethodInvoker)(() =>
                {
                    textBoxTaxCode.Text = textBoxTaxCode.Text.Replace(" ", "");
                }));

                if (!Regex.IsMatch(textBoxTaxCode.Text, "^[0-9-]*$"))
                {
                    Blink(labelNote, "Mã số thuế thì chỉ có số thôi mợ ạ", Color.Red);
                    Invoke((MethodInvoker)(() =>
                    {
                        textBoxTaxCode.Focus();
                        buttonSubmit.Text = "Tra cứu";
                    }));
                    return;
                }

                try
                {
                    if (_driver == null || !_driver.WindowHandles.Any())
                        _driver = InitializeChrome();
                }
                catch
                {
                    _driver = InitializeChrome();
                }

                if (string.IsNullOrWhiteSpace(_driver.Url) || _driver.Url == "data:,")
                {
                    _driver.Navigate().GoToUrl(_mainUrl);
                }
                else
                {
                    _driver.Navigate().Refresh();
                }
                // solve captcha
                string captcha = string.Empty;
                int year = 0;
                Invoke((MethodInvoker)(() =>
                {
                    year = int.Parse(comboBoxYear.SelectedItem.ToString());
                }));

                do
                {
                    CheckCancellation();
                    // refresh captcha
                    if (captcha != string.Empty)
                        _driver.FindElement(By.ClassName("ui-icon-refresh")).Click();

                    CheckCancellation();
                    // tax code
                    await TypeSlowMo(_driver, By.Id("tin"), textBoxTaxCode.Text, 2).ConfigureAwait(false);

                    CheckCancellation();
                    // date From
                    await TypeSlowMo(_driver, By.Id("ngayTu"), $" 01/01/{year}", 2).ConfigureAwait(false);

                    CheckCancellation();
                    // date To
                    await TypeSlowMo(_driver, By.Id("ngayDen"), " " + new DateTime(year + 1, 1, 1).AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), 2).ConfigureAwait(false);

                    CheckCancellation();
                    var js = (IJavaScriptExecutor)_driver;
                    var base64String = js.ExecuteScript(@"
                    var c = document.createElement('canvas');
                    var ctx = c.getContext('2d');
                    //var img = document.getElementsByTagName('img')[0];
                    var img = document.querySelector(""img[src = 'Captcha.jpg']"");
                    c.height=img.height;
                    c.width=img.width;
                    ctx.drawImage(img, 0, 0,img.width, img.height);
                    var base64String = c.toDataURL();
                    return base64String;
                    ") as string;
                    var base64 = base64String?.Split(',').Last();
                    using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                    using (var bitmap = new Bitmap(stream))
                    {
                        bitmap.Save("Captcha.png", System.Drawing.Imaging.ImageFormat.Png);
                    }

                    CheckCancellation();
                    captcha = GetText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Captcha.png"));

                    if (!string.IsNullOrWhiteSpace(captcha))
                    {
                        CheckCancellation();
                        await TypeSlowMo(_driver, By.Id("captchaCodeVerify"), captcha, 5).ConfigureAwait(false);

                        CheckCancellation();
                        // click submit
                        _driver.FindElement(By.XPath("//*[@id=\"searchBtn\"]/span[2]")).Click();

                        var err = _driver.FindElement(By.Id("lbLoiCode"));
                        if (err.Displayed || !string.IsNullOrWhiteSpace(err.Text))
                        {
                            captcha = null;
                        }
                    }
                    else
                    {
                        captcha = null;
                        _tokenSource.Cancel();
                        _tokenSource = new CancellationTokenSource();
                    }

                } while (string.IsNullOrWhiteSpace(captcha));

                if (checkBoxAutoOpenLatest.Checked)
                {
                    // click view result
                    var nextDelay = TimeSpan.FromMilliseconds(300);
                    for (var i = 0; i < 3; i++)
                    {
                        CheckCancellation();
                        var links = _driver.FindElements(By.PartialLinkText($"{year}"));
                        var latest = links.LastOrDefault();
                        if (latest != null)
                        {
                            latest.Click();
                            break;
                        }
                        await Task.Delay(nextDelay, ct).ConfigureAwait(false);
                        nextDelay = nextDelay + nextDelay;
                    }
                }

                Invoke((MethodInvoker)(() =>
                {
                    buttonSubmit.Text = "Tra cứu";
                }));
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    buttonSubmit.Text = "Tra cứu";
                }));
                if (ex is OperationCanceledException)
                {
                    _tokenSource = new CancellationTokenSource();
                    return;
                }

                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // local functions
            void Invoke(Delegate method)
            {
                panelMain?.Invoke(method);
            }

            void CheckCancellation()
            {
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private static async Task TypeSlowMo(IWebDriver driver, By by, string value, int milliseconds = 10)
        {
            var element = driver.FindElement(by);
            element.Clear();
            foreach (var character in value)
            {
                await Task.Delay(milliseconds).ConfigureAwait(false);
                element.SendKeys(character.ToString());
            }
        }

        public static Bitmap ScreenShotElement(IWebDriver driver, By by)
        {
            var elements = driver.FindElements(by);
            if (elements.Count == 0)
                return null;

            var element = elements[0];
            var screenShot = ((ITakesScreenshot)driver).GetScreenshot();
            using (var ms = new MemoryStream(screenShot.AsByteArray))
            {
                var screenBitmap = new Bitmap(ms);
                return screenBitmap.Clone(
                    new Rectangle(
                        element.Location.X,
                        element.Location.Y,
                        element.Size.Width,
                        element.Size.Height
                    ),
                    screenBitmap.PixelFormat
                );
            }
        }

        private static string GetText(string imgPath)
        {
            try
            {
                using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
                using (var img = Pix.LoadFromFile(imgPath))
                using (var page = engine.Process(img))
                {
                    return Regex.Replace(page.GetText(), "[^0-9a-zA-Z]", "");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Có lỗi xảy ra: {e}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private static ChromeDriver InitializeChrome()
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var option = new ChromeOptions();
            //option.AddArguments("--headless", "--no-sandbox", "--disable-web-security", "--disable-gpu", "--incognito", "--proxy-bypass-list=*", "--proxy-server='direct://'", "--log-level=3", "--hide-scrollbars");
            return new ChromeDriver(chromeDriverService, option);
        }

        private async void Blink(Label label, string text, Color color)
        {
            var originalText = label.Text;
            var originColor = label.ForeColor;
            panelMain?.Invoke((MethodInvoker)(() => label.Text = text));
            int i = 1;
            while (i <= 10)
            {
                if (label.ForeColor == color)
                {
                    await Task.Delay(750).ConfigureAwait(false);
                    label.ForeColor = Color.Transparent;
                }
                else
                {
                    await Task.Delay(250).ConfigureAwait(false);
                    label.ForeColor = color;
                }
                ++i;
            }
            panelMain?.Invoke((MethodInvoker)(() =>
            {
                label.Text = originalText;
                label.ForeColor = originColor;
            }));
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            _driver?.Quit();
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
            Application.Exit();
        }

        private void TextBoxTaxCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                ButtonSubmit_Click(this, new EventArgs());
            }
        }
    }
}
