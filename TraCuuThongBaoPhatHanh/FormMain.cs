using AutoUpdaterDotNET;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private readonly string _alterUrl = ConfigurationManager.AppSettings["host2"] ?? "http://nopthue.gdt.gov.vn/";
        private static string _noteText;
        private static Color _noteTextColor;
        private readonly IBaseService _baseService;
        private readonly int[] _years = new[] { DateTime.Today.Year, DateTime.Today.Year - 1, DateTime.Today.Year - 2 };
        private volatile bool _running = false;
        private volatile bool _headless = false;
        public FormMain()
        {
            InitializeComponent();
            _baseService = ServiceHelper.GetService();
            _noteText = labelNote.Text;
            _noteTextColor = labelNote.ForeColor;
            comboBoxYearFrom.DataSource = _years;
            comboBoxYearTo.DataSource = _years;
            labelVersion.Text = $"v.{new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version}";
        }

        private async void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (!_running)
            {
                _running = true;
                var token = _tokenSource.Token;
                await Task.Factory.StartNew(() => ActionNotice(token), token).ConfigureAwait(false);
            }
            else
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();
            }
        }

        private async Task ActionNotice(CancellationToken ct)
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
                    _running = false;
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
                    _running = false;
                    return;
                }

                try
                {
                    if (_driver == null || !_driver.WindowHandles.Any() || _headless)
                        _driver = InitializeWebDriver(false);
                }
                catch
                {
                    _driver = InitializeWebDriver(false);
                }

                if (_driver.Url != _mainUrl)
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
                    var js = (IJavaScriptExecutor)_driver;

                    // tax code
                    await TypeSlowMo(_driver, By.Id("tin"), textBoxTaxCode.Text, 2).ConfigureAwait(false);
                    //SetElementText(js, "tin", textBoxTaxCode.Text);

                    // date From
                    var dateFrom = $" 01/01/{yearFrom}";
                    while (_driver.FindElement(By.Id("ngayTu")).GetAttribute("value") != dateFrom.Trim())
                    {
                        CheckCancellation();
                        await TypeSlowMo(_driver, By.Id("ngayTu"), dateFrom, 50).ConfigureAwait(false);
                        //SetElementText(js, "ngayTu", $" 01/01/{yearFrom}"); 
                    }

                    // date To
                    var dateTo = new DateTime(yearTo + 1, 1, 1).AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    while (_driver.FindElement(By.Id("ngayDen")).GetAttribute("value") != dateTo.Trim())
                    {
                        CheckCancellation();
                        await TypeSlowMo(_driver, By.Id("ngayDen"), " " + dateTo, 50).ConfigureAwait(false);
                        //SetElementText(js, "ngayDen", " " + new DateTime(yearTo + 1, 1, 1).AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)); 
                    }

                    CheckCancellation();

                    // solve captcha
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
                    var captchaFile = SaveBytesToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Captcha.png"), Convert.FromBase64String(base64));

                    CheckCancellation();
                    captcha = _baseService.ImageOcrToText(captchaFile);
                    captcha = Regex.Replace(captcha, "[^0-9a-zA-Z]", "");

                    if (!string.IsNullOrWhiteSpace(captcha))
                    {
                        CheckCancellation();
                        await TypeSlowMo(_driver, By.Id("captchaCodeVerify"), captcha, 5).ConfigureAwait(false);
                        //SetElementText(js, "captchaCodeVerify", captcha);

                        CheckCancellation();
                        // click submit
                        _driver.FindElement(By.XPath("//*[@id=\"searchBtn\"]/span[2]")).Click();

                        await Task.Delay(500, ct);
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
                        var links = _driver.FindElements(By.CssSelector("a[class='textlink']"));
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
                    _running = false;
                    return;
                }

                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            _running = false;

            // local functions
            void Invoke(Delegate method)
            {
                tabControl.Invoke(method);
            }

            void CheckCancellation()
            {
                if (ct.IsCancellationRequested)
                {
                    _running = false;
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private async void ButtonSubmit2_Click(object sender, EventArgs e)
        {
            if (!_running)
            {
                _running = true;
                var token = _tokenSource.Token;
                await Task.Factory.StartNew(() => ActionSerial(token), token).ConfigureAwait(false);
            }
            else
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();
            }
        }

        private async Task ActionSerial(CancellationToken ct)
        {
            try
            {
                Invoke((MethodInvoker)(() =>
                {
                    buttonSubmit2.Text = "Huỷ tra cứu";
                }));

                if (string.IsNullOrWhiteSpace(textBoxTaxCode2.Text))
                {
                    Blink(labelNote, "Nhập hộ em cái mã số thuế", Color.Red);
                    Invoke((MethodInvoker)(() =>
                    {
                        textBoxTaxCode2.Focus();
                        buttonSubmit2.Text = "Tra cứu";
                    }));
                    _running = false;
                    return;
                }

                Invoke((MethodInvoker)(() =>
                {
                    textBoxTaxCode2.Text = textBoxTaxCode2.Text.Replace(" ", "");
                }));

                if (!Regex.IsMatch(textBoxTaxCode2.Text, "^[0-9-]*$"))
                {
                    Blink(labelNote, "Mã số thuế thì chỉ có số thôi mợ ạ", Color.Red);
                    Invoke((MethodInvoker)(() =>
                    {
                        textBoxTaxCode2.Focus();
                        buttonSubmit2.Text = "Tra cứu";
                    }));
                    _running = false;
                    return;
                }

                try
                {
                    if (_driver == null || !_driver.WindowHandles.Any() || !checkBoxHeadless.Checked)
                    {
                        _driver = InitializeWebDriver(checkBoxHeadless.Checked);
                    }
                    else
                    {
                        _driver.Close();
                        _driver = InitializeWebDriver(checkBoxHeadless.Checked);
                    }
                }
                catch
                {
                    _driver = InitializeWebDriver(checkBoxHeadless.Checked);
                }

                if (_driver.Url != _alterUrl)
                {
                    _driver.Navigate().GoToUrl(_alterUrl);
                }
                else
                {
                    _driver.Navigate().Refresh();
                }

                CheckCancellation();
                var js = (IJavaScriptExecutor)_driver;

                // click register
                _driver.FindElement(By.CssSelector("img[alt='register']")).Click();

                var nextDelay = TimeSpan.FromMilliseconds(300);
                bool timeout = true;
                // tax code
                for (var i = 0; i < 3; i++)
                {
                    CheckCancellation();
                    var taxCodeInput = _driver.FindElement(By.Id("tin"));
                    if (taxCodeInput != null)
                    {
                        SetElementText(js, "tin", textBoxTaxCode2.Text);
                        timeout = false;
                        break;
                    }
                    await Task.Delay(nextDelay, ct).ConfigureAwait(false);
                    nextDelay = nextDelay + nextDelay;
                    CheckCancellation();
                }

                if (timeout)
                {
                    MessageBox.Show($"Máy chủ phản hồi quá lâu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    timeout = true;
                }

                // click continue
                _driver.FindElement(By.CssSelector("input[class='btn_type1']")).Click();

                // get serial
                nextDelay = TimeSpan.FromMilliseconds(300);
                for (var i = 0; i < 3; i++)
                {
                    CheckCancellation();
                    var serial = _driver.FindElement(By.Id("serial"));
                    if (serial != null)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            textBoxSerial.Text = serial.GetAttribute("value").Replace(" ", "");
                            textBoxSerial.Focus();
                        }));
                        timeout = false;
                        break;
                    }
                    await Task.Delay(nextDelay, ct).ConfigureAwait(false);
                    nextDelay = nextDelay + nextDelay;
                    CheckCancellation();
                }
                if (timeout)
                {
                    MessageBox.Show($"Máy chủ phản hồi quá lâu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                Invoke((MethodInvoker)(() =>
                {
                    buttonSubmit2.Text = "Tra cứu";
                }));
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    buttonSubmit2.Text = "Tra cứu";
                }));
                if (ex is OperationCanceledException)
                {
                    _tokenSource = new CancellationTokenSource();
                    _running = false;
                    return;
                }

                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            _running = false;

            // local functions
            void Invoke(Delegate method)
            {
                tabControl.Invoke(method);
            }

            void CheckCancellation()
            {
                if (ct.IsCancellationRequested)
                {
                    _running = false;
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private async Task RetryAsync(Action action, bool breaker)
        {
            // Retry after 1 second, then after 2 seconds, then 4 if query returns null invoice by key
            var nextDelay = TimeSpan.FromSeconds(1);
            for (var i = 0; i < 3; i++)
            {
                //package = await RepoService.GetPackageByBrokerElement(Config, element, connectionString, table);
                //if (!(package?.Exception is InvoiceNotFoundException))
                //{
                //    break;
                //}

                await Task.Delay(nextDelay);
                nextDelay = nextDelay + nextDelay;
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

        private static void SetElementText(IJavaScriptExecutor js, string inputId, string inputValue)
        {
            js.ExecuteScript($"document.getElementById('{inputId}').setAttribute('value', '{inputValue}');");
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

        private IWebDriver InitializeWebDriver(bool headless = true)
        {
            return InitializeChrome(headless); // default browser
        }

        private ChromeDriver InitializeChrome(bool headless = true)
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var option = new ChromeOptions();
            // chrome 77: https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F676760%2Fchrome-win.zip?generation=1562910729402389&alt=media
            // option.BinaryLocation = "./chrome-win/chrome.exe";
            if (headless)
            {
                //option.AddArguments("--headless", "--no-sandbox", "--disable-web-security", "--disable-gpu", "--incognito", "--proxy-bypass-list=*", "--proxy-server='direct://'", "--log-level=3", "--hide-scrollbars");
                option.AddArguments("--headless");
                _headless = true;
            }
            else
            {
                _headless = false;
            }
            //option.AddArguments("--incognito", "--disable-web-security");

            return new ChromeDriver(chromeDriverService, option);
        }

        private async void Blink(Label label, string text, Color color)
        {
            tabControl.Invoke((MethodInvoker)(() => label.Text = text));
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
            tabControl.Invoke((MethodInvoker)(() =>
            {
                label.Text = _noteText;
                label.ForeColor = _noteTextColor;
            }));
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            ShowInTaskbar = false;
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
                if (tabControl.SelectedIndex == 0)
                {
                    ButtonSubmit_Click(this, new EventArgs());
                }
                else if (tabControl.SelectedIndex == 1)
                {
                    ButtonSubmit2_Click(this, new EventArgs());
                }

            }
        }

        private void ComboBoxYearTo_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedFrom = int.Parse(comboBoxYearFrom.SelectedValue.ToString());
            var rest = _years.Where(_ => _ <= int.Parse(comboBoxYearTo.SelectedValue.ToString())).ToArray();
            comboBoxYearFrom.DataSource = rest;
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
            using (var fileStream = new FileStream(desiredFileFullName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }

            return desiredFileFullName;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //Uncomment below line to run update process using non administrator account.
            AutoUpdater.RunUpdateAsAdmin = false;

            //Don't want user to select remind later time in AutoUpdater notification window then uncomment 3 lines below so default remind later time will be set to 2 days.
            //AutoUpdater.LetUserSelectRemindLater = false;
            //AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            //AutoUpdater.RemindLaterAt = 1;

            //Don't want to show Skip button then uncomment below line.
            //AutoUpdater.ShowSkipButton = false;

            //Don't want to show Remind Later button then uncomment below line.
            //AutoUpdater.ShowRemindLaterButton = false;

            //Want to show custom application title then uncomment below line.
            //AutoUpdater.AppTitle = "My Custom Application Title";

            //Want to show errors then uncomment below line.
            //AutoUpdater.ReportErrors = true;

            //Want to handle how your application will exit when application finished downloading then uncomment below line.
            //AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;

            //Want to handle update logic yourself then uncomment below line.
            //AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;

            //Want to check for updates frequently then uncomment following lines.
            //System.Timers.Timer timer = new System.Timers.Timer
            //{
            //    Interval = 2 * 60 * 1000,
            //    SynchronizingObject = this
            //};
            //timer.Elapsed += delegate
            //{
            //    AutoUpdater.Start("https://raw.githubusercontent.com/Devancy/TraCuuThongBaoPhatHanh/master/TraCuuThongBaoPhatHanh/Deploy/AutoUpdater.xml");
            //};
            //timer.Start();

            //Uncomment following lines to enable forced download.
            //AutoUpdater.Mandatory = true;
            //AutoUpdater.UpdateMode = Mode.ForcedDownload;

            //Want to change update form size then uncomment below line.
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);

            AutoUpdater.Start("https://raw.githubusercontent.com/Devancy/TraCuuThongBaoPhatHanh/master/TraCuuThongBaoPhatHanh/Deploy/AutoUpdater.xml");
        }

        private void AutoUpdater_ApplicationExitEvent()
        {
            Text = @"Closing application...";
            //Thread.Sleep(5000);
            Application.Exit();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                if (string.IsNullOrWhiteSpace(textBoxTaxCode.Text) && Regex.IsMatch(textBoxTaxCode2.Text, "^[0-9-]*$"))
                {
                    textBoxTaxCode.Text = textBoxTaxCode2.Text;
                }
            }
            else if (tabControl.SelectedIndex == 1)
            {
                if (string.IsNullOrWhiteSpace(textBoxTaxCode2.Text) && Regex.IsMatch(textBoxTaxCode.Text, "^[0-9-]*$"))
                {
                    textBoxTaxCode2.Text = textBoxTaxCode.Text;
                }
            }
        }
    }
}
