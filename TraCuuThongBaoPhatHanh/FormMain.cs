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
using TraCuuThongBaoPhatHanh.Service;

namespace TraCuuThongBaoPhatHanh
{
    public partial class FormMain : Form
    {
        private IWebDriver _driver;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly string _mainUrl = ConfigurationManager.AppSettings["host"] ?? "http://tracuuhoadon.gdt.gov.vn/tbphtc.html";
        private static string _noteText;
        private static Color _noteTextColor;
        private readonly IBaseService _baseService;
        private readonly int[] _years = new[] { DateTime.Today.Year, DateTime.Today.Year - 1, DateTime.Today.Year - 2 };
        public FormMain()
        {
            InitializeComponent();
            _baseService = ServiceHelper.GetService();
            _noteText = labelNote.Text;
            _noteTextColor = labelNote.ForeColor;
            comboBoxYearFrom.DataSource = _years;
            comboBoxYearTo.DataSource = _years;
        }

        private async void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (buttonSubmit.Text.StartsWith("T"))
            {
                var token = _tokenSource.Token;
                await Action(token);
            }
            else
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();
            }
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

                string captcha = string.Empty;
                int yearFrom = 0;
                int yearTo = 0;
                Invoke((MethodInvoker)(() =>
                {
                    yearFrom = int.Parse(comboBoxYearFrom.SelectedItem.ToString());
                    yearTo = int.Parse(comboBoxYearTo.SelectedItem.ToString());
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
                    await TypeSlowMo(_driver, By.Id("ngayTu"), $" 01/01/{yearFrom}", 2).ConfigureAwait(false);

                    CheckCancellation();
                    // date To
                    await TypeSlowMo(_driver, By.Id("ngayDen"), " " + new DateTime(yearTo + 1, 1, 1).AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), 2).ConfigureAwait(false);

                    CheckCancellation();

                    // solve captcha
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
                    //using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                    //using (var bitmap = new Bitmap(stream))
                    //{
                    //    bitmap.Save("Captcha.png", System.Drawing.Imaging.ImageFormat.Png);
                    //}
                    var captchaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Captcha.png");
                    captchaFile = SaveBytesToFile(captchaFile, Convert.FromBase64String(base64));

                    CheckCancellation();
                    captcha = _baseService.ImageOcrToText(captchaFile);
                    captcha = Regex.Replace(captcha, "[^0-9a-zA-Z]", "");

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
                        var links = _driver.FindElements(By.PartialLinkText($"{yearTo}"));
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
                label.Text = _noteText;
                label.ForeColor = _noteTextColor;
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

        private void ComboBoxYearTo_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedFrom = int.Parse(comboBoxYearFrom.SelectedValue.ToString());
            var rest = _years.Where(_ => _ <= int.Parse(comboBoxYearTo.SelectedValue.ToString())).ToArray();
            comboBoxYearFrom.DataSource = rest.ToArray();
            if (rest.Contains(selectedFrom))
            {
                comboBoxYearFrom.SelectedIndex = Array.IndexOf(rest, selectedFrom);
            }
        }

        private static string SaveBytesToFile(string fileFullPath, byte[] bytes)
        {
            var directory = Path.GetDirectoryName(fileFullPath);
            try
            {
                var extenstion = Path.GetExtension(fileFullPath);
                foreach (var file in Directory.GetFiles(directory, $"*{extenstion}"))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var desiredFileFullName = ServiceHelper.GetDesiredFileFullPath(directory, fileFullPath);
            // Create a FileStream object to write a stream to a file
            using (Stream stream = new MemoryStream(bytes))
            using (var fileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }

            return desiredFileFullName;
        }
    }
}
