using Aspose.Cells;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TraCuuThongBaoPhatHanh_v2
{
    public partial class FormEntry : Form
    {
        private IWebDriver _driver;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly string _mainUrl = "http://tracuuhoadon.gdt.gov.vn/tbphtc.html";
        //private volatile bool _headless = false;


        private string _token = string.Empty;
        private CookieCollection _cookies;
        private long _tokenName;

        public FormEntry()
        {
            InitializeComponent();
            Run();
        }

        private async void FormEntry_Load(object sender, EventArgs e)
        {
            dateTimePickerFrom.Value = DateTime.Today.AddYears(-20);
            dateTimePickerTo.Value = DateTime.Today;
            var token = _tokenSource.Token;
            await Task.Factory.StartNew(() => GetCaptcha(sender, e, token), token).ConfigureAwait(false);
        }

        private void FormEntry_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            ShowInTaskbar = false;
            _driver?.Quit();
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }
            Application.Exit();
        }

        private ChromeDriver InitializeChrome(bool headless = true)
        {
            ShowProgress("Đang khởi tạo dịch vụ, vui lòng chờ");

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var option = new ChromeOptions();
            // chrome 77: https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F676760%2Fchrome-win.zip?generation=1562910729402389&alt=media
            // option.BinaryLocation = "./chrome-win/chrome.exe";
            if (headless)
            {
                //option.AddArguments("--headless", "--no-sandbox", "--disable-web-security", "--disable-gpu", "--incognito", "--proxy-bypass-list=*", "--proxy-server='direct://'", "--log-level=3", "--hide-scrollbars");
                option.AddArguments("--headless");
            }
            //option.AddArguments("--incognito", "--disable-web-security");

            return new ChromeDriver(chromeDriverService, option);
        }

        private Task GetCaptcha(object sender, EventArgs e, CancellationToken ct)
        {
            try
            {
                ButtonF5_Click(sender, e);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    buttonExecute.Text = "GO";
                }));
                if (ex is OperationCanceledException)
                {
                    _tokenSource = new CancellationTokenSource();
                    return Task.FromResult((object)null);
                }

                BridgePopupMessage($"Có lỗi xảy ra: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return Task.FromResult((object)null);
        }

        private bool ValidateParams()
        {
            bool flag = false;
            string text = string.Empty;
            if (string.IsNullOrWhiteSpace(labelSourcePath.Text) && string.IsNullOrWhiteSpace(textBoxTaxCodeList.Text?.Trim()))
                text = "Vui lòng nhập danh sách mã số thuế hoặc chọn tệp dữ liệu";
            else if (string.IsNullOrWhiteSpace(textBoxCaptcha.Text))
                text = "Vui lòng nhập mã Captcha";
            else
                flag = true;
            if (!string.IsNullOrWhiteSpace(text))
            {
                BridgePopupMessage(text);
            }
            return flag;
        }

        public Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        private async void ButtonExecute_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() => Execute(sender, e)).ConfigureAwait(false);
        }

        private Task Execute(object sender, EventArgs e)
        {
            if (!ValidateParams())
            {
                return Task.FromResult((object)null);
            }
            Invoker((MethodInvoker)(() =>
            {
                Cursor.Current = Cursors.WaitCursor;
                buttonExecute.Enabled = false;
                buttonExecute.Text = "Executing";
            }));
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _cookies = null;

                string directory = !string.IsNullOrWhiteSpace(labelSourcePath.Text) ? labelSourcePath.Text : AppDomain.CurrentDomain.BaseDirectory;
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                List<TINResponse> data = new List<TINResponse>();

                if (ValidateCode(textBoxCaptcha.Text.Trim()))
                {
                    ShowProgress("Đang đọc danh sách mã số thuế");
                    List<string> taxCodes = this.ReadDataInputData(directory);
                    if (taxCodes != null && taxCodes.Any())
                    {
                        ShowProgress("Đang truy xuất thông tin từ Tổng cục Thuế");
                        int count = taxCodes.Count;
                        for (int index = 0; index < count; ++index)
                        {
                            TINResponse tinResponse = new TINResponse();
                            TINResponse releaseByTaxCode = GetInvoiceReleaseByTaxCode(taxCodes[index]);
                            if (releaseByTaxCode == null || releaseByTaxCode.Releases == null || releaseByTaxCode.Releases.Count <= 0)
                                releaseByTaxCode.tin = taxCodes[index]?.Trim();
                            data.Add(releaseByTaxCode);
                        }
                    }
                    if (data.Count > 0)
                    {
                        ShowProgress("Đang xuất thông tin chi tiết");
                        var output = ExportDataToExcel(data, directory);
                        stopwatch.Stop();
                        BridgePopupMessage(string.Format("Lấy dữ liệu thành công! Thời gian: {0}", TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).ToString("hh\\:mm\\:ss")));

                        System.Diagnostics.Process.Start(output);
                    }
                }
                else
                {
                    BridgePopupMessage(string.Format("Captcha sai hoặc hết hạn, nhập captcha mới để tiếp tục"));
                    ButtonF5_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                BridgePopupMessage(string.Format("Có lỗi xảy ra! {0}", ex.ToString()));
            }
            Invoker((MethodInvoker)(() =>
            {
                Cursor.Current = Cursors.Default;
                buttonExecute.Enabled = true;
                buttonExecute.Text = "Go";
            }));
            HideProgress();
            return Task.FromResult((object)null);
        }

        private void ButtonSelectDataSource_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            labelSourcePath.Text = openFileDialog.FileName;
        }

        private void ButtonF5_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                string captcha = string.Empty;
                try
                {
                    if (_driver == null || !_driver.WindowHandles.Any()/* || _headless*/)
                        _driver = InitializeChrome(true);
                }
                catch
                {
                    _driver = InitializeChrome(true);
                }

                if (_driver.Url != _mainUrl)
                {
                    _driver.Navigate().GoToUrl(_mainUrl);
                }
                //else
                //{
                //    if(captcha == string.Empty)
                //        _driver.Navigate().Refresh();
                //}

                do
                {
                    ShowProgress("Đang lấy mã captcha");
                    // refresh captcha
                    if (captcha != string.Empty || !(sender is FormEntry))
                    {
                        _driver.FindElement(By.ClassName("ui-icon-refresh")).Click();
                    }

                    //CheckCancellation();
                    var js = (IJavaScriptExecutor)_driver;

                    // save captcha
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
                    captcha = base64String?.Split(',').Last();

                    Invoker((MethodInvoker)(() =>
                    {
                        pictureBoxCaptcha.Image = Base64ToImage(captcha);
                    }));

                } while (string.IsNullOrWhiteSpace(captcha));

                HideProgress();
            });
        }

        private void ShowProgress(string text)
        {
            Invoker((MethodInvoker)(() =>
            {
                pictureBoxLoading.Visible = true;
                labelInfo.Text = text;
            }));
        }

        private void HideProgress()
        {
            Invoker((MethodInvoker)(() =>
            {
                pictureBoxLoading.Visible = false;
                labelInfo.Text = "";
            }));
        }

        private void Invoker(Delegate action, params object[] arg)
        {
            if (InvokeRequired)
            {
                Invoke(action, arg);
            }
            else
                action.DynamicInvoke(arg);
        }

        private void BridgePopupMessage(string message, string title = "Thông báo", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(this, message, title, buttons, icon);
            });
        }

        //--------------------------------
        #region worker
        public string ExportDataToExcel(List<TINResponse> data, string exportDirectory)
        {
            DataTable dataTable = new DataTable("Result");
            DataTable masterData = new DataTable("Master");
            DataTable detailData = new DataTable("Detail");
            BuildDataToExport(data, ref masterData, ref detailData);
            string directoryName = Path.GetDirectoryName(exportDirectory);
            string fileName = Path.Combine(directoryName, $"KetQua_{DateTime.Now:dd.MM.yyyy_hh.mm.ss}.xlsx");
            ImportTableOptions options = new ImportTableOptions { IsFieldNameShown = true, IsHtmlString = true };
            Workbook workbook = new Workbook();

            Worksheet worksheet1 = workbook.Worksheets[0];
            worksheet1.Name = "Company";
            worksheet1.Cells.ImportData(masterData, 0, 0, options);
            Aspose.Cells.Cells cells = worksheet1.Cells;
            cells.Merge(0, 6, 1, 2);
            cells.Merge(0, 8, 1, 2);
            cells.Merge(0, 10, 1, 2);
            worksheet1.Cells.Rows[0].Style.Font.IsBold = true;
            worksheet1.Cells.Rows[0].Style.BackgroundColor = Color.Cyan;
            worksheet1.Cells.Rows[1].Style.Font.IsBold = true;
            worksheet1.AutoFitRows();
            worksheet1.AutoFitColumns();
            Worksheet worksheet2 = workbook.Worksheets.Add("Details");
            worksheet2.Cells.ImportData(detailData, 0, 0, options);
            worksheet2.Cells.Rows[0].Style.Font.IsBold = true;
            worksheet2.Cells.Rows[0].Style.BackgroundColor = Color.Cyan;
            worksheet2.Cells.Rows[1].Style.Font.IsBold = true;
            worksheet2.AutoFitRows();
            worksheet2.AutoFitColumns();
            workbook.Settings.FirstVisibleTab = 0;
            try
            {
                workbook.Save(fileName);
            }
            catch (Exception ex)
            {
                BridgePopupMessage(ex.ToString());
                fileName = null;
            }

            return fileName;
        }

        private void BuildDataToExport(List<TINResponse> data, ref DataTable masterData, ref DataTable detailData)
        {
            masterData.Columns.Add("STT", typeof(int));
            masterData.Columns.Add("MST", typeof(string));
            masterData.Columns.Add("Tên đơn vị", typeof(string));
            masterData.Columns.Add("Điện thoại", typeof(string));
            masterData.Columns.Add("Địa chỉ", typeof(string));
            masterData.Columns.Add("Thủ trưởng đơn vị", typeof(string));
            DateTime now = DateTime.Now;
            masterData.Columns.Add((now.Year - 2).ToString(), typeof(string));
            masterData.Columns.Add("year2", typeof(string));
            masterData.Columns.Add((now.Year - 1).ToString(), typeof(string));
            masterData.Columns.Add("year1", typeof(string));
            masterData.Columns.Add(now.Year.ToString(), typeof(string));
            masterData.Columns.Add("year", typeof(string));
            masterData.Columns.Add("Nhà cung cấp HĐĐT gần nhất", typeof(string));
            DataRow row1 = masterData.NewRow();
            string str1 = "Hóa đơn giấy";
            string str2 = "Hóa đơn điện tử";
            row1[6] = row1[8] = row1[10] = str1;
            row1[7] = row1[9] = row1[11] = str2;
            masterData.Rows.Add(row1);
            detailData.Columns.Add("STT", typeof(int));
            detailData.Columns.Add("MST", typeof(string));
            detailData.Columns.Add("Ngày phát hành", typeof(string));
            detailData.Columns.Add("Số thông báo", typeof(string));
            detailData.Columns.Add("Cơ quan thuế quản lý", typeof(string));
            detailData.Columns.Add("Tên loại hóa đơn", typeof(string));
            detailData.Columns.Add("Mẫu số", typeof(string));
            detailData.Columns.Add("Kí hiệu", typeof(string));
            detailData.Columns.Add("Số lượng", typeof(int));
            detailData.Columns.Add("Từ số", typeof(int));
            detailData.Columns.Add("Đến số", typeof(int));
            detailData.Columns.Add("Ngày bắt đầu sử dụng", typeof(string));
            detailData.Columns.Add("Đã sử dụng", typeof(int));
            detailData.Columns.Add("Doanh nghiệp in", typeof(string));
            detailData.Columns.Add("Mã số thuế", typeof(string));
            detailData.Columns.Add("Hợp đồng đặt in số", typeof(string));
            detailData.Columns.Add("Hợp đồng đặt in ngày", typeof(string));
            detailData.Columns.Add("Link", typeof(string));
            int num1 = 1;
            for (int index1 = 0; index1 < data.Count; ++index1)
            {
                DataRow row2 = masterData.NewRow();
                row2[0] = index1 + 1;
                int year = DateTime.Now.Year;
                int num2 = year - 1;
                int num3 = year - 2;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                int num8 = 0;
                int num9 = 0;
                StringBuilder stringBuilder = new StringBuilder();
                if (data[index1].Releases != null)
                {
                    List<Release> releaseList = new List<Release>();
                    List<Release> releases = data[index1].Releases;
                    Release release1 = new Release();
                    Release release2 = data[index1].Releases.Last<Release>();
                    try
                    {
                        row2[1] = release2.dtnt_tin;
                        row2[2] = release2.dtnt_ten;
                        row2[3] = release2.dtnt_tel;
                        row2[4] = release2.dtnt_diachi;
                        row2[5] = release2.thu_truong;
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                    for (int index2 = 0; index2 < releases.Count; ++index2)
                    {
                        if (releases[index2].dtls != null)
                        {
                            List<ReleaseDetail> releaseDetailList = new List<ReleaseDetail>();
                            foreach (ReleaseDetail dtl in releases[index2].dtls)
                            {
                                DataRow row3 = detailData.NewRow();
                                row3[0] = num1;
                                row3[1] = releases[index2].dtnt_tin;
                                row3[2] = releases[index2].ngay_phathanh;
                                row3[3] = releases[index2].so_thong_bao;
                                row3[4] = releases[index2].cqt_ten;
                                row3[5] = dtl.ach_ten;
                                row3[6] = dtl.ach_ma;
                                row3[7] = dtl.kyhieu;
                                row3[8] = dtl.soluong;
                                row3[9] = dtl.tu_so;
                                row3[10] = dtl.den_so;
                                row3[11] = dtl.ngay_sdung;
                                if (dtl.da_sdung != null)
                                    row3[12] = dtl.da_sdung;
                                row3[13] = dtl.nin_ten;
                                row3[14] = dtl.nin_tin;
                                row3[15] = dtl.so_hopdong;
                                row3[16] = dtl.ngay_hopdong;
                                row3[17] = dtl.link;
                                if (releases[index2].ngay_phathanh.Contains(year.ToString()))
                                {
                                    num4 += dtl.soluong.Value;
                                    if (dtl.kyhieu.Last<char>().ToString().ToLower() == "e")
                                        num7 += dtl.soluong.Value;
                                }
                                else if (releases[index2].ngay_phathanh.Contains(num2.ToString()))
                                {
                                    num5 += dtl.soluong.Value;
                                    if (dtl.kyhieu.Last<char>().ToString().ToLower() == "e")
                                        num8 += dtl.soluong.Value;
                                }
                                else if (releases[index2].ngay_phathanh.Contains(num3.ToString()))
                                {
                                    num6 += dtl.soluong.Value;
                                    if (dtl.kyhieu.Last<char>().ToString().ToLower() == "e")
                                        num9 += dtl.soluong.Value;
                                }
                                if (dtl.kyhieu.Last<char>().ToString().ToLower() == "e" && !string.IsNullOrWhiteSpace(dtl.nin_ten))
                                    stringBuilder.Append(string.Format("\n{0}", dtl.nin_ten));
                                detailData.Rows.Add(row3);
                                ++num1;
                            }
                        }
                    }
                    row2[6] = num6 - num9;
                    row2[7] = num9;
                    row2[8] = num5 - num8;
                    row2[9] = num8;
                    row2[10] = num4 - num7;
                    row2[11] = num7;
                    row2[12] = stringBuilder;
                }
                else
                {
                    row2[1] = data[index1].tin;
                    row2[2] = "Không tìm thấy kết quả tra cứu";
                }
                masterData.Rows.Add(row2);
            }
        }

        public List<string> ReadDataInputData(string filePath)
        {
            List<string> results = new List<string>();
            var textArea = textBoxTaxCodeList.Text;
            if (!string.IsNullOrWhiteSpace(textArea))
            {
                var temp = textArea.Split(new[] { ";", ",", "\t", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if(temp.Any())
                {
                    results.AddRange(temp);
                }
            }

            if(!results.Any())
            {
                var lines = File.ReadAllLines(filePath);
                foreach(var line in lines)
                {
                    var alias = line?.Trim();
                    if(!string.IsNullOrEmpty(alias))
                        results.Add(alias);
                }
            }
            
            return results;
        }

        public TINResponse GetInvoiceReleaseByTaxCode(string taxCode)
        {
            string format = "{0}%2F{1}%2F{2}";
            string text = textBoxCaptcha.Text;
            string kind = "tc";
            string[] strArray1 = dateTimePickerFrom.Value.ToString("dd/MM/yyyy").Split('/');
            string[] strArray2 = dateTimePickerTo.Value.ToString("dd/MM/yyyy").Split('/');
            string fromDate = string.Format(format, strArray1[0], strArray1[1], strArray1[2]);
            string toDate = string.Format(format, strArray2[0], strArray2[1], strArray2[2]);
            TINResponse companyInfo = GetCompanyInfo(taxCode, text);
            if (companyInfo.tinModel != null)
            {
                ReleaseResponse invoiceReleases1 = GetInvoiceReleases(taxCode, fromDate, toDate, text, 1, kind);
                if (invoiceReleases1 != null && invoiceReleases1.list != null && invoiceReleases1.list.Count > 0)
                {
                    List<Release> list = invoiceReleases1.list;
                    int num = int.Parse(invoiceReleases1.total);
                    if (num > 1)
                    {
                        for (int page = 2; page <= num; ++page)
                        {
                            ReleaseResponse invoiceReleases2 = GetInvoiceReleases(taxCode, fromDate, toDate, text, page, kind);
                            if (invoiceReleases2 != null && invoiceReleases2.list != null && invoiceReleases2.list.Count > 0)
                                list.AddRange(invoiceReleases2.list);
                        }
                    }
                    int count = list.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        ReleaseDetailResponse invoiceReleaseDetails = GetInvoiceReleaseDetails(list[index].id, list[index].lan_thaydoi, list[index].search, taxCode, list[index].loaitb_phanh, "www");
                        if (invoiceReleaseDetails != null && invoiceReleaseDetails.dtls != null && invoiceReleaseDetails.dtls.Count > 0)
                        {
                            list[index].dtls = new List<ReleaseDetail>();
                            list[index].dtls = invoiceReleaseDetails.dtls;
                        }
                    }
                    companyInfo.Releases = new List<Release>();
                    companyInfo.Releases = list;
                }
            }
            return companyInfo;
        }

        public string GetNewToken()
        {
            if (string.IsNullOrEmpty(_token))
                _token = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            Random random = new Random();
            if (_tokenName == 0L)
                _tokenName = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            else
                ++_tokenName;
            string empty = string.Empty;
            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(WebGetMethod(string.Format("http://www.tracuuhoadon.gdt.gov.vn/getnewtoken.html?token={0}&struts.token.name=token&_={1}", _token, _tokenName)));
            if (dictionary.ContainsKey("newToken") && dictionary["newToken"] != null)
            {
                empty = dictionary["newToken"].ToString();
                _token = empty;
            }
            return empty;
        }

        public TINResponse GetCompanyInfo(string taxCode, string captchaCode)
        {
            TINResponse tinResponse = new TINResponse();
            string str = WebPostMethodJson("", string.Format("http://www.tracuuhoadon.gdt.gov.vn/gettin.html?tin={0}&captchaCode={1}", taxCode, captchaCode));
            if (!string.IsNullOrWhiteSpace(str))
            {
                try
                {
                    tinResponse = JsonConvert.DeserializeObject<TINResponse>(str);
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }
            return tinResponse;
        }

        public ReleaseResponse GetInvoiceReleases(string taxCode, string fromDate, string toDate, string captchaCode, int page, string kind)
        {
            ReleaseResponse releaseResponse = new ReleaseResponse();
            double totalMilliseconds = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if (!string.IsNullOrEmpty(GetNewToken()))
            {
                string empty = string.Empty;
                string.Format("{0}{1}{2}", "http://www.tracuuhoadon.gdt.gov.vn/", "searchtbph.html", string.Format("?_search=false&nd={0}&rows=1000&page={1}&sidx=&sord=asc&kind={2}&tin={3}&ngayTu={4}&ngayDen={5}&captchaCode={6}&token={7}&struts.token.name=token&_={8}", totalMilliseconds, page, kind, taxCode, fromDate, toDate, captchaCode, _token, _tokenName));
                string method = WebGetMethod(string.Format("http://www.tracuuhoadon.gdt.gov.vn/searchtbph.html?_search=false&nd={0}&rows=1000&page={7}&sidx=&sord=asc&kind={8}&tin={1}&ngayTu={2}&ngayDen={3}&captchaCode={4}&token={5}&struts.token.name=token&_={6}", totalMilliseconds, taxCode, fromDate, toDate, captchaCode, _token, _tokenName, page, kind));
                if (!string.IsNullOrWhiteSpace(method))
                {
                    try
                    {
                        releaseResponse = JsonConvert.DeserializeObject<ReleaseResponse>(method);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                }
            }
            return releaseResponse;
        }

        public ReleaseDetailResponse GetInvoiceReleaseDetails(string releasId, int? ltd, bool search, string taxCode, string type, string prefixUrl = "www")
        {
            ReleaseDetailResponse releaseDetailResponse = new ReleaseDetailResponse();
            if (!ltd.HasValue)
                ltd = new int?(0);
            string paramz = string.Format("id={0}&ltd={1}&dtnt_tin={2}&loaitb_phanh={3}", releasId, ltd, taxCode, type);
            string URL1 = string.Format("http://{0}.tracuuhoadon.gdt.gov.vn/viewtbph.html?{1}", prefixUrl, paramz);
            string empty1 = string.Empty;
            if (!string.IsNullOrWhiteSpace(WebPostMethodJson(paramz, URL1)))
            {
                string empty2 = string.Empty;
                string URL2 = string.Format("http://{0}.tracuuhoadon.gdt.gov.vn/gettbphdtl.html?id={1}&ltd={2}", prefixUrl, releasId, ltd);
                double totalMilliseconds = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                ++_tokenName;
                string method = WebGetMethod(URL2);
                if (!string.IsNullOrWhiteSpace(method))
                {
                    try
                    {
                        releaseDetailResponse = JsonConvert.DeserializeObject<ReleaseDetailResponse>(method);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                }
            }
            return releaseDetailResponse;
        }

        public string WebPostMethodJson(string postData, string URL)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.Method = "POST";
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.ContentLength = bytes.Length;
            if (_cookies == null)
                GetCookies();
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(_cookies);
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            UpdateCookie(response);
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            response.Close();
            return end;
        }

        public string WebPostMethodParams(string postData, string URL)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.Method = "POST";
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            httpWebRequest.ContentLength = bytes.Length;
            if (_cookies == null)
                GetCookies();
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(_cookies);
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            UpdateCookie(response);
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            response.Close();
            return end;
        }

        public string WebGetMethod(string URL)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.Method = "GET";
            if (_cookies == null)
                GetCookies();
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(_cookies);
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            UpdateCookie(response);
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            string end = streamReader.ReadToEnd();
            streamReader.Close();
            return end;
        }

        private static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = new CookieContainer();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK
                 || response.StatusCode == HttpStatusCode.Moved
                 || response.StatusCode == HttpStatusCode.Redirect)
                /*&& response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)*/)
            {

                // if the remote file was found, download it
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
                return true;
            }
            return false;
        }

        private void UpdateCookie(HttpWebResponse response)
        {
            var responseCookies = response.Cookies.Cast<System.Net.Cookie>().ToArray();
            var originCookies = _cookies.Cast<System.Net.Cookie>().ToArray();
            var resultCookie = new CookieCollection();
            foreach (System.Net.Cookie cookie in originCookies)
            {
                var match = responseCookies.FirstOrDefault(_ => _.Name == cookie.Name);
                if (match != null)
                    cookie.Value = match.Value;
                resultCookie.Add(cookie);
            }

            _cookies = resultCookie;
            //return resultCookie;
        }

        public CookieCollection GetCookies()
        {
            CookieCollection cookieCollection = new CookieCollection();
            var cookies = _driver.Manage().Cookies.AllCookies;
            foreach (var cookie in cookies)
            {
                cookieCollection.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
            _cookies = cookieCollection;
            return cookieCollection;
        }

        private void LogException(Exception ex)
        {
            System.IO.File.AppendAllText(string.Format("{0}\\Log.txt", AppDomain.CurrentDomain.BaseDirectory), ex.ToString());
            System.IO.File.AppendAllText(string.Format("{0}\\Log.txt", AppDomain.CurrentDomain.BaseDirectory), ex.StackTrace);
        }

        private bool ValidateCode(string captchaCode)
        {
            try
            {
                var response = WebPostMethodParams("captchaCode=" + captchaCode, "http://tracuuhoadon.gdt.gov.vn/validcode.html");
                return !response.Contains("Sai mã");
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        #endregion

        public static void Run()
        {
            Aspose.Cells.License license = new Aspose.Cells.License();
            using (var stream = new MemoryStream(global::TraCuuThongBaoPhatHanh_v2.Properties.Resources.License))
                license.SetLicense(stream);
        }

        private void LabelUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel)sender).Text);
        }

        private void LabelSourcePath_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(labelSourcePath.Text))
                buttonClearSourcePath.Visible = true;
        }

        private void ButtonClearSourcePath_Click(object sender, EventArgs e)
        {
            labelSourcePath.Text = null;
        }
    }
}
