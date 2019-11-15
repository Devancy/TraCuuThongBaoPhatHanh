using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TraCuuThongBaoPhatHanh_v2
{
    public partial class FormEntry : Form
    {
        public const string SoftdreamsTaxCode = "0105987432";
        private IWebDriver _driver;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly string _mainUrl = "http://tracuuhoadon.gdt.gov.vn/tbphtc.html";


        private string _token = string.Empty;
        private CookieCollection _cookies;
        private long _tokenName;

        private bool _autoCaptcha = ConfigurationManager.AppSettings["auto-captcha"] == "1";
        private List<TINResponse> _data;
        private string _output = null;

        public FormEntry()
        {
            InitializeComponent();
            this.Text = this.Text + $" [Version {Program.Version}]";
        }

        private async void FormEntry_Load(object sender, EventArgs e)
        {
            Program.CheckForUpdate();

            comboBoxExportType.SelectedIndex = 0;
            dateTimePickerFrom.Value = DateTime.Today.AddYears(-10);
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
            foreach (var process in Process.GetProcessesByName("Chromium"))
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

        private static ChromeDriver InitializeChrome(bool headless = true)
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var option = new ChromeOptions();
            // chrome 77: https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F676760%2Fchrome-win.zip?generation=1562910729402389&alt=media
            option.BinaryLocation = "./chrome-win/chrome.exe";
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

            string currentTaxCode = string.Empty;

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _cookies = null;

                _output = !string.IsNullOrWhiteSpace(labelSourcePath.Text) ? Path.GetDirectoryName(labelSourcePath.Text) : AppDomain.CurrentDomain.BaseDirectory + "Output";
                List<TINResponse> data = new List<TINResponse>();

                if (ValidateCode(textBoxCaptcha.Text.Trim()))
                {
                    ShowAnimation("Đang đọc danh sách mã số thuế");
                    List<string> taxCodes = this.ReadDataInputData();
                    if (taxCodes != null && taxCodes.Any())
                    {
                        ShowAnimation("Đang truy xuất thông tin từ Tổng cục Thuế");
                        int count = taxCodes.Count;

                        if (count > 0)
                        {
                            Invoker((MethodInvoker)(() => { this.progressBar.Maximum = count; }));
                        }

                        for (int index = 0; index < count; ++index)
                        {
                            ShowProgressBar();
                            currentTaxCode = taxCodes[index];
                            try
                            {
                                TINResponse releaseByTaxCode = GetInvoiceReleaseByTaxCode(currentTaxCode);
                                if (releaseByTaxCode?.Releases == null || releaseByTaxCode.Releases.Count <= 0)
                                    releaseByTaxCode.tin = currentTaxCode?.Trim();
                                data.Add(releaseByTaxCode);
                            }
                            catch (Exception ex)
                            {
                                if (ex is InvalidResponseJsonException)
                                {
                                    TINResponse error = CreateErrorTINResponse(currentTaxCode);
                                    data.Add(error);
                                }
                                else
                                    throw ex;
                            }
                        }
                        HideProgressBar();
                    }
                    if (data.Count > 0)
                    {
                        ShowAnimation("Đang xuất thông tin chi tiết");
                        _data = data;
                        stopwatch.Stop();
                        ShowResults(data, $"Thời gian xử lý {TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).ToString("hh\\:mm\\:ss")}");
                    }
                    else
                    {
                        BridgePopupMessage("Không tìm thấy kết quả");
                    }
                }
                else
                {
                    BridgePopupMessage("Captcha sai hoặc hết hạn, nhập captcha mới để tiếp tục");
                    ButtonF5_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                BridgePopupMessage($"Có lỗi xảy ra! Mã số thuế đang xử lý - {currentTaxCode}\r\n{ex.Message}");
                LogException(new Exception($"Mã số thuế đang xử lý - {currentTaxCode}", ex));
            }
            Invoker((MethodInvoker)(() =>
            {
                Cursor.Current = Cursors.Default;
                buttonExecute.Enabled = true;
                buttonExecute.Text = "Go";
            }));
            HideAnimation();
            return Task.FromResult((object)null);
        }

        private static TINResponse CreateErrorTINResponse(string taxCode)
        {
            TINResponse response = new TINResponse
            {
                Releases = new List<Release> {
                    new Release {
                    dtnt_ten = "Lỗi truy vấn, kiểm tra lại MST",
                    dtnt_tin = taxCode,
                    ngay_phathanh = " ",
                    dtls =
                        new List<ReleaseDetail> {
                            new ReleaseDetail {
                                soluong = 0,
                                tu_so = 0,
                                den_so = 0,
                                ach_lan_thaydoi = 0,
                                kyhieu = " "
                            }
                        }
                    }
                }
            };

            return response;
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
                ShowAnimation("Đang khởi tạo dịch vụ, vui lòng chờ");
                Invoker((MethodInvoker)(() =>
                {
                    buttonExecute.Enabled = false;
                }));
                string captchaBase64 = string.Empty;

                try
                {
                    if (_driver == null || !_driver.WindowHandles.Any()/* || _headless*/)
                        _driver = InitializeChrome();

                    if (_driver.Url != _mainUrl)
                    {
                        _driver.Navigate().GoToUrl(_mainUrl);
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    BridgePopupMessage("Lỗi khởi tạo dịch vụ, liên hệ để được hỗ trợ", icon: MessageBoxIcon.Error);
                    HideAnimation();
                    return;
                    //_driver = InitializeChrome();
                }

                do
                {
                    ShowAnimation("Đang lấy mã captcha");
                    // refresh captcha
                    if (captchaBase64 != string.Empty || !(sender is FormEntry))
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
                    captchaBase64 = base64String?.Split(',').Last();

                    Invoker((MethodInvoker)(() =>
                    {
                        pictureBoxCaptcha.Image = Base64ToImage(captchaBase64);
                    }));

                    var captchaFile = SaveBytesToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Captcha.jpg"), Convert.FromBase64String(captchaBase64));
                    if (_autoCaptcha)
                    {
                        try
                        {
                            var captchaText = Ocr.ImageOcrToText(captchaFile);
                            captchaText = Regex.Replace(captchaText, "[^0-9a-zA-Z]", "");
                            Invoker((MethodInvoker)(() =>
                            {
                                textBoxCaptcha.Text = captchaText;
                            }));
                        }
                        catch (Exception ex)
                        {
                            LogException(ex);
                            _autoCaptcha = false;
                            UpdateAppConfigAppSettings("auto-captcha", "0");
                            BridgePopupMessage("Máy tính không hỗ trợ captcha tự động, vui lòng nhập thủ công");
                        }
                    }

                } while (string.IsNullOrWhiteSpace(captchaBase64));

                Invoker((MethodInvoker)(() =>
                {
                    buttonExecute.Enabled = true;
                }));
                HideAnimation();
            });
        }

        private void ShowAnimation(string text)
        {
            Invoker((MethodInvoker)(() =>
            {
                pictureBoxLoading.Visible = true;
                labelInfo.Text = text;
            }));
        }

        private void HideAnimation()
        {
            Invoker((MethodInvoker)(() =>
            {
                pictureBoxLoading.Visible = false;
                labelInfo.Text = "";
            }));
        }

        private void ShowProgressBar()
        {
            Invoker((MethodInvoker)(() =>
            {
                this.progressBar.Visible = true;
                this.progressBar.PerformStep();
                this.progressBar.Update();
            }));
        }

        private void HideProgressBar()
        {
            Invoker((MethodInvoker)(() =>
            {
                this.progressBar.Value = 0;
                this.progressBar.Visible = false;
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

        private void LabelSourcePath_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(labelSourcePath.Text))
                buttonClearSourcePath.Visible = true;
        }

        private void ButtonClearSourcePath_Click(object sender, EventArgs e)
        {
            labelSourcePath.Text = null;
            buttonClearSourcePath.Visible = false;
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

            // Create a FileStream object to write a stream to a file
            using (Stream stream = new MemoryStream(bytes))
            using (var fileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }

            return fileFullPath;
        }

        private void UpdateAppConfigAppSettings(string key, string value)
        {
            var exeConfig = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".config";
            if (System.IO.File.Exists(exeConfig))
            {
                XmlDocument config = new XmlDocument();
                config.Load(exeConfig);
                var node = config.SelectSingleNode($"/configuration/appSettings[@key='{key}']");
                if (node?.Attributes != null)
                {
                    node.Attributes["value"].Value = value;
                }
                else
                {
                    var appSettings = config.SelectSingleNode("/configuration/appSettings");
                    XmlNode kvp = config.CreateElement("add");
                    XmlAttribute addAttribute = config.CreateAttribute("key");
                    addAttribute.Value = key;
                    kvp.Attributes.Append(addAttribute);
                    addAttribute = config.CreateAttribute("value");
                    addAttribute.Value = value;
                    kvp.Attributes.Append(addAttribute);
                }
                config.Save(exeConfig);
            }
            else
            {
                LogException(new Exception("Không tìm thấy file App.config"));
            }
        }

        //--------------------------------
        #region worker

        private void ShowResults(IReadOnlyList<TINResponse> data, string elapsed)
        {
            DataTable masterData = new DataTable("Master");
            DataTable detailsData = new DataTable("Details");
            BuildDataToExport(data, ref masterData, ref detailsData);
            Invoker((MethodInvoker)(() =>
            {
                new Grid(detailsData, elapsed).ShowDialog(this);
            }));
        }

        private string ExportDataToExcel(IReadOnlyList<TINResponse> data, string exportDirectory)
        {
            DataTable masterData = new DataTable("Company");
            //DataTable detailsData = new DataTable("Details");
            var detailTables = BuildDataToExport(data, ref masterData, comboBoxExportType.SelectedIndex);
            string fileName = Path.Combine(exportDirectory, $"EasyInvoice_KetQua_{DateTime.Now:dd.MM.yyyy_hh.mm.ss}.xlsx");

            using (var excelPackage = new ExcelPackage())
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "http://easyinvoice.vn";
                excelPackage.Workbook.Properties.Title = "Easy Invoice - Chi tiết thông báo phát hành hoá đơn điện tử của doanh nghiệp";
                excelPackage.Workbook.Properties.Subject = "Exported automatically by Easy Invoice";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheetCompany = excelPackage.Workbook.Worksheets.Add(masterData.TableName);
                worksheetCompany.Cells["A1"].LoadFromDataTable(masterData, true, TableStyles.Dark9);
                worksheetCompany.Cells[worksheetCompany.Dimension.Address].AutoFitColumns();


                foreach (var table in detailTables)
                {
                    ExcelWorksheet worksheetDetails = excelPackage.Workbook.Worksheets.Add(table.TableName);
                    worksheetDetails.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Dark9);
                    worksheetDetails.Cells[worksheetDetails.Dimension.Address].AutoFitColumns();
                }

                excelPackage.Workbook.Worksheets.Add("A product of Softdreams");

                FileInfo fi = new FileInfo(fileName);
                excelPackage.SaveAs(fi);
            }

            return fileName;
        }

        private void BuildDataToExport(IReadOnlyList<TINResponse> data, ref DataTable masterSheetData, ref DataTable detailsSheetData)
        {
            masterSheetData.Columns.Add("STT", typeof(int));
            masterSheetData.Columns.Add("MST", typeof(string));
            masterSheetData.Columns.Add("Tên đơn vị", typeof(string));
            masterSheetData.Columns.Add("Điện thoại", typeof(string));
            masterSheetData.Columns.Add("Địa chỉ", typeof(string));
            masterSheetData.Columns.Add("Thủ trưởng đơn vị", typeof(string));
            DateTime now = DateTime.Now;
            masterSheetData.Columns.Add((now.Year - 2).ToString(), typeof(string));
            masterSheetData.Columns.Add($"{(now.Year - 2)}E", typeof(string));
            masterSheetData.Columns.Add((now.Year - 1).ToString(), typeof(string));
            masterSheetData.Columns.Add($"{(now.Year - 1)}E", typeof(string));
            masterSheetData.Columns.Add(now.Year.ToString(), typeof(string));
            masterSheetData.Columns.Add($"{now.Year}E", typeof(string));
            masterSheetData.Columns.Add("Nhà cung cấp HĐĐT gần nhất", typeof(string));
            
            detailsSheetData.Columns.Add("STT", typeof(int)); // 0
            detailsSheetData.Columns.Add("MST", typeof(string)); // 1
            detailsSheetData.Columns.Add("Tên đơn vị", typeof(string)); // 2
            detailsSheetData.Columns.Add("Ngày bắt đầu sử dụng", typeof(string)); // 7 - 3
            detailsSheetData.Columns.Add("Mẫu số", typeof(string)); // 8 - 4
            detailsSheetData.Columns.Add("Ký hiệu", typeof(string)); // 9 - 5
            detailsSheetData.Columns.Add("Số lượng", typeof(int)); // 10 - 6
            detailsSheetData.Columns.Add("Từ số", typeof(int)); // 11 - 7
            detailsSheetData.Columns.Add("Đến số", typeof(int)); // 12 - 8
            detailsSheetData.Columns.Add("Ngày thông báo", typeof(string)); // 3 - 9
            detailsSheetData.Columns.Add("Số thông báo", typeof(string)); // 4 - 10
            detailsSheetData.Columns.Add("Cơ quan thuế quản lý", typeof(string)); //5 - 11
            detailsSheetData.Columns.Add("Tên loại hóa đơn", typeof(string)); //6 - 12
            
            detailsSheetData.Columns.Add("Đã sử dụng", typeof(int)); //13
            detailsSheetData.Columns.Add("Doanh nghiệp in", typeof(string)); // 14
            detailsSheetData.Columns.Add("Mã số thuế", typeof(string)); // 15
            detailsSheetData.Columns.Add("Hợp đồng đặt in số", typeof(string)); // 16
            detailsSheetData.Columns.Add("Hợp đồng đặt in ngày", typeof(string)); // 17
            //detailsSheetData.Columns.Add("Link", typeof(string));
            int num1 = 1;
            for (int index1 = 0; index1 < data.Count; ++index1)
            {
                DataRow row2 = masterSheetData.NewRow();
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
                    List<Release> releases = data[index1].Releases;
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
                            foreach (ReleaseDetail dtl in releases[index2].dtls)
                            {
                                DataRow row3 = detailsSheetData.NewRow();
                                row3[0] = num1;
                                row3[1] = releases[index2].dtnt_tin;
                                row3[2] = releases[index2].dtnt_ten;
                                DateTime dateTime;
                                if (DateTime.TryParse(dtl.ngay_sdung, out dateTime))
                                {
                                    row3[3] = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    row3[3] = dtl.ngay_sdung;
                                }

                                row3[4] = dtl.ach_ma;
                                row3[5] = dtl.kyhieu;
                                row3[6] = dtl.soluong;
                                row3[7] = dtl.tu_so;
                                row3[8] = dtl.den_so;
                                row3[9] = releases[index2].ngay_phathanh;
                                row3[10] = releases[index2].so_thong_bao;
                                row3[11] = releases[index2].cqt_ten;
                                row3[12] = dtl.ach_ten;

                                if (dtl.da_sdung != null)
                                    row3[13] = dtl.da_sdung;
                                row3[14] = dtl.nin_ten;
                                row3[15] = dtl.nin_tin;
                                row3[16] = dtl.so_hopdong;
                                row3[17] = dtl.ngay_hopdong;
                                //row3[18] = dtl.link;
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
                                if (dtl.kyhieu.LastOrDefault().ToString().ToLower() == "e" && !string.IsNullOrWhiteSpace(dtl.nin_ten))
                                    stringBuilder.Append($"\n{dtl.nin_ten}");
                                detailsSheetData.Rows.Add(row3);
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
                    //row2[2] = "Không tìm thấy kết quả tra cứu";
                    row2[2] = data[index1].tinModel?.norm_name;//"Không tìm thấy kết quả tra cứu";
                    row2[3] = data[index1].tinModel?.tran_tel;
                    row2[4] = data[index1].tinModel?.tran_addr;
                }
                masterSheetData.Rows.Add(row2);
            }
        }

        private IEnumerable<DataTable> BuildDataToExport(IReadOnlyList<TINResponse> data, ref DataTable masterSheetData, int exportType)
        {
            List<DataTable> detailDataTables = new List<DataTable>();

            masterSheetData.Columns.Add("STT", typeof(int));
            masterSheetData.Columns.Add("Đã sử dụng HĐĐT", typeof(int));
            masterSheetData.Columns.Add("MST", typeof(string));
            masterSheetData.Columns.Add("Tên đơn vị", typeof(string));
            masterSheetData.Columns.Add("Điện thoại", typeof(string));
            masterSheetData.Columns.Add("Địa chỉ", typeof(string));
            masterSheetData.Columns.Add("Thủ trưởng đơn vị", typeof(string));
            masterSheetData.Columns.Add((DateTime.Today.Year - 2).ToString(), typeof(string));
            masterSheetData.Columns.Add($"{(DateTime.Today.Year - 2)}E", typeof(string));
            masterSheetData.Columns.Add((DateTime.Today.Year - 1).ToString(), typeof(string));
            masterSheetData.Columns.Add($"{(DateTime.Today.Year - 1)}E", typeof(string));
            masterSheetData.Columns.Add(DateTime.Today.Year.ToString(), typeof(string));
            masterSheetData.Columns.Add($"{DateTime.Today.Year}E", typeof(string));
            masterSheetData.Columns.Add("NCC HĐĐT gần nhất", typeof(string));

            DataTable pInvoice = new DataTable("P_Invoice");
            pInvoice.Columns.Add("STT", typeof(int)); // 0
            pInvoice.Columns.Add("MST", typeof(string)); // 1
            pInvoice.Columns.Add("Tên đơn vị", typeof(string)); // 2
            pInvoice.Columns.Add("Ngày bắt đầu sử dụng", typeof(string)); // 7 - 3
            pInvoice.Columns.Add("Mẫu số", typeof(string)); // 8 - 4
            pInvoice.Columns.Add("Ký hiệu", typeof(string)); // 9 - 5
            pInvoice.Columns.Add("Số lượng", typeof(int)); // 10 - 6
            pInvoice.Columns.Add("Từ số", typeof(int)); // 11 - 7
            pInvoice.Columns.Add("Đến số", typeof(int)); // 12 - 8
            pInvoice.Columns.Add("Ngày thông báo", typeof(string)); // 3 - 9
            pInvoice.Columns.Add("Số thông báo", typeof(string)); // 4 - 10
            pInvoice.Columns.Add("Cơ quan thuế quản lý", typeof(string)); //5 - 11
            pInvoice.Columns.Add("Tên loại hóa đơn", typeof(string)); //6 - 12
            pInvoice.Columns.Add("Đã sử dụng", typeof(int)); //13
            pInvoice.Columns.Add("Doanh nghiệp in", typeof(string)); // 14
            pInvoice.Columns.Add("Mã số thuế", typeof(string)); // 15
            pInvoice.Columns.Add("Hợp đồng đặt in số", typeof(string)); // 16
            pInvoice.Columns.Add("Hợp đồng đặt in ngày", typeof(string)); // 17

            DataTable eInvoice = new DataTable("E_Invoice");
            eInvoice.Columns.Add("STT", typeof(int)); // 0
            eInvoice.Columns.Add("MST", typeof(string)); // 1
            eInvoice.Columns.Add("Tên đơn vị", typeof(string)); // 2
            eInvoice.Columns.Add("Ngày bắt đầu sử dụng", typeof(string)); // 7 - 3
            eInvoice.Columns.Add("Mẫu số", typeof(string)); // 8 - 4
            eInvoice.Columns.Add("Ký hiệu", typeof(string)); // 9 - 5
            eInvoice.Columns.Add("Số lượng", typeof(int)); // 10 - 6
            eInvoice.Columns.Add("Từ số", typeof(int)); // 11 - 7
            eInvoice.Columns.Add("Đến số", typeof(int)); // 12 - 8
            eInvoice.Columns.Add("Ngày thông báo", typeof(string)); // 3 - 9
            eInvoice.Columns.Add("Số thông báo", typeof(string)); // 4 - 10
            eInvoice.Columns.Add("Cơ quan thuế quản lý", typeof(string)); //5 - 11
            eInvoice.Columns.Add("Tên loại hóa đơn", typeof(string)); //6 - 12
            eInvoice.Columns.Add("Đã sử dụng", typeof(int)); //13
            eInvoice.Columns.Add("Doanh nghiệp in", typeof(string)); // 14
            eInvoice.Columns.Add("Mã số thuế", typeof(string)); // 15
            eInvoice.Columns.Add("Hợp đồng đặt in số", typeof(string)); // 16
            eInvoice.Columns.Add("Hợp đồng đặt in ngày", typeof(string)); // 17

            DataTable easyInvoice = new DataTable("EasyInvoice");
            easyInvoice.Columns.Add("STT", typeof(int)); // 0
            easyInvoice.Columns.Add("MST", typeof(string)); // 1
            easyInvoice.Columns.Add("Tên đơn vị", typeof(string)); // 2
            easyInvoice.Columns.Add("Ngày bắt đầu sử dụng", typeof(string)); // 7 - 3
            easyInvoice.Columns.Add("Mẫu số", typeof(string)); // 8 - 4
            easyInvoice.Columns.Add("Ký hiệu", typeof(string)); // 9 - 5
            easyInvoice.Columns.Add("Số lượng", typeof(int)); // 10 - 6
            easyInvoice.Columns.Add("Từ số", typeof(int)); // 11 - 7
            easyInvoice.Columns.Add("Đến số", typeof(int)); // 12 - 8
            easyInvoice.Columns.Add("Ngày thông báo", typeof(string)); // 3 - 9
            easyInvoice.Columns.Add("Số thông báo", typeof(string)); // 4 - 10
            easyInvoice.Columns.Add("Cơ quan thuế quản lý", typeof(string)); //5 - 11
            easyInvoice.Columns.Add("Tên loại hóa đơn", typeof(string)); //6 - 12
            easyInvoice.Columns.Add("Đã sử dụng", typeof(int)); //13
            easyInvoice.Columns.Add("Doanh nghiệp in", typeof(string)); // 14
            easyInvoice.Columns.Add("Mã số thuế", typeof(string)); // 15
            easyInvoice.Columns.Add("Hợp đồng đặt in số", typeof(string)); // 16
            easyInvoice.Columns.Add("Hợp đồng đặt in ngày", typeof(string)); // 17

            for (int comIndex = 0; comIndex < data.Count; ++comIndex)
            {
                bool alreadyUseEInvoice = false;
                DataRow comRow = masterSheetData.NewRow();
                comRow[0] = comIndex + 1;
                int year = DateTime.Now.Year;
                int year_1 = year - 1;
                int year_2 = year - 2;
                int totalCurrentYear = 0;
                int totalCurrentYear_E = 0;
                int totalYear_1 = 0;
                int totalYear_1_E = 0;
                int totalYear_2 = 0;
                int totalYear_2_E = 0;
                string lastProvider = null;
                if (data[comIndex].Releases != null)
                {
                    List<Release> releases = data[comIndex].Releases;
                    Release release2 = data[comIndex].Releases.Last<Release>();
                    try
                    {
                        //row2[1] = release2.dtnt_tin; // Đã sử dụng HĐĐT
                        comRow[2] = release2.dtnt_tin;
                        comRow[3] = release2.dtnt_ten;
                        comRow[4] = release2.dtnt_tel;
                        comRow[5] = release2.dtnt_diachi;
                        comRow[6] = release2.thu_truong;
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                    foreach (var release in releases)
                    {
                        if (release.dtls == null) continue;

                        foreach (ReleaseDetail dtl in release.dtls)
                        {
                            DataTable target = null;
                            if(!alreadyUseEInvoice)
                                alreadyUseEInvoice = dtl.kyhieu != null && dtl.kyhieu.Trim().ToUpperInvariant().EndsWith("E", StringComparison.InvariantCultureIgnoreCase);
                            if (exportType == 0) // group by Easy Invoice
                            {
                                target = dtl.nin_tin != null && dtl.nin_tin.Trim() == SoftdreamsTaxCode ? easyInvoice : dtl.kyhieu != null && dtl.kyhieu.Trim().ToUpperInvariant().EndsWith("E", StringComparison.InvariantCultureIgnoreCase) ? eInvoice : pInvoice;
                            }
                            else
                            {
                                target = dtl.kyhieu != null && dtl.kyhieu.Trim().ToUpperInvariant().EndsWith("E", StringComparison.InvariantCultureIgnoreCase) ? eInvoice : pInvoice;
                            }

                            DataRow dtRow = target.NewRow();
                            dtRow[0] = target.Rows.Count + 1;
                            dtRow[1] = release.dtnt_tin;
                            dtRow[2] = release.dtnt_ten;
                            if (DateTime.TryParse(dtl.ngay_sdung, out var dateTime))
                            {
                                dtRow[3] = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                dtRow[3] = dtl.ngay_sdung;
                            }

                            dtRow[4] = dtl.ach_ma;
                            dtRow[5] = dtl.kyhieu;
                            dtRow[6] = dtl.soluong;
                            dtRow[7] = dtl.tu_so;
                            dtRow[8] = dtl.den_so;
                            dtRow[9] = release.ngay_phathanh;
                            dtRow[10] = release.so_thong_bao;
                            dtRow[11] = release.cqt_ten;
                            dtRow[12] = dtl.ach_ten;

                            if (dtl.da_sdung != null)
                                dtRow[13] = dtl.da_sdung;
                            dtRow[14] = dtl.nin_ten;
                            dtRow[15] = dtl.nin_tin;
                            dtRow[16] = dtl.so_hopdong;
                            dtRow[17] = dtl.ngay_hopdong;
                                
                            if (release.ngay_phathanh.Contains(year.ToString()))
                            {
                                totalCurrentYear += dtl.soluong.Value;
                                if (dtl.kyhieu != null && dtl.kyhieu.Last().ToString().ToUpperInvariant() == "E")
                                    totalCurrentYear_E += dtl.soluong.Value;
                            }
                            else if (release.ngay_phathanh.Contains(year_1.ToString()))
                            {
                                totalYear_1 += dtl.soluong.Value;
                                if (dtl.kyhieu != null && dtl.kyhieu.Last<char>().ToString().ToUpperInvariant() == "E")
                                    totalYear_1_E += dtl.soluong.Value;
                            }
                            else if (release.ngay_phathanh.Contains(year_2.ToString()))
                            {
                                totalYear_2 += dtl.soluong.Value;
                                if (dtl.kyhieu != null && dtl.kyhieu.Last<char>().ToString().ToUpperInvariant() == "E")
                                    totalYear_2_E += dtl.soluong.Value;
                            }
                            if (dtl.kyhieu != null && dtl.kyhieu.Last().ToString().ToUpperInvariant() == "E" && !string.IsNullOrWhiteSpace(dtl.nin_ten))
                                lastProvider = dtl.nin_ten;
                            target.Rows.Add(dtRow);
                        }
                    }
                    comRow[7] = totalYear_2 - totalYear_2_E;
                    comRow[8] = totalYear_2_E;
                    comRow[9] = totalYear_1 - totalYear_1_E;
                    comRow[10] = totalYear_1_E;
                    comRow[11] = totalCurrentYear - totalCurrentYear_E;
                    comRow[12] = totalCurrentYear_E;
                    comRow[13] = lastProvider;
                }
                else
                {
                    comRow[1] = 0;
                    comRow[2] = data[comIndex].tin;
                    comRow[3] = data[comIndex].tinModel?.norm_name;//"Không tìm thấy kết quả tra cứu";
                    comRow[4] = data[comIndex].tinModel?.tran_tel;
                    comRow[5] = data[comIndex].tinModel?.tran_addr;
                }
                comRow[1] = alreadyUseEInvoice ? 1 : 0; // Đã sử dụng HĐĐT
                masterSheetData.Rows.Add(comRow);
            }

            if (exportType == 0) // group by Easy Invoice
            {
                detailDataTables.Add(pInvoice);
                detailDataTables.Add(eInvoice);
                detailDataTables.Add(easyInvoice);
            }
            else
            {
                detailDataTables.Add(pInvoice);
                detailDataTables.Add(eInvoice);
            }

            return detailDataTables;
        }

        public List<string> ReadDataInputData()
        {
            List<string> results = new List<string>();
            var textArea = textBoxTaxCodeList.Text;
            if (!string.IsNullOrWhiteSpace(textArea))
            {
                var temp = textArea.Split(new[] { ";", ",", "\t", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Any())
                {
                    results.AddRange(temp);
                }

                return results;
            }

            if (!results.Any())
            {
                var lines = File.ReadAllLines(labelSourcePath.Text);
                foreach (var line in lines)
                {
                    var alias = line?.Trim();
                    if (!string.IsNullOrEmpty(alias))
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
            string[] from = dateTimePickerFrom.Value.ToString("dd/MM/yyyy").Split('/');
            string[] to = dateTimePickerTo.Value.ToString("dd/MM/yyyy").Split('/');
            string fromDate = string.Format(format, from[0], from[1], from[2]);
            string toDate = string.Format(format, to[0], to[1], to[2]);
            TINResponse companyInfo = GetCompanyInfo(taxCode, text);
            if (companyInfo.tinModel != null)
            {
                ReleaseResponse invoiceReleases1 = GetInvoiceReleases(taxCode, fromDate, toDate, text, 1, kind);
                if (invoiceReleases1?.list != null && invoiceReleases1.list.Count > 0)
                {
                    List<Release> list = invoiceReleases1.list;
                    int num = int.Parse(invoiceReleases1.total);
                    if (num > 1)
                    {
                        for (int page = 2; page <= num; ++page)
                        {
                            ReleaseResponse invoiceReleases2 = GetInvoiceReleases(taxCode, fromDate, toDate, text, page, kind);
                            if (invoiceReleases2?.list != null && invoiceReleases2.list.Count > 0)
                                list.AddRange(invoiceReleases2.list);
                        }
                    }
                    int count = list.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        ReleaseDetailResponse invoiceReleaseDetails = GetInvoiceReleaseDetails(list[index].id, list[index].lan_thaydoi, list[index].search, taxCode, list[index].loaitb_phanh, "www");
                        if (invoiceReleaseDetails?.dtls != null && invoiceReleaseDetails.dtls.Count > 0)
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
            if (_tokenName == 0L)
                _tokenName = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            else
                ++_tokenName;
            string empty = string.Empty;
            var token = GetRequest($"http://www.tracuuhoadon.gdt.gov.vn/getnewtoken.html?token={_token}&struts.token.name=token&_={_tokenName}");
            if (!IsValidJson(token))
                throw new InvalidResponseJsonException(token);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(token);
            if (dictionary.ContainsKey("newToken") && dictionary["newToken"] != null)
            {
                empty = dictionary["newToken"].ToString();
                _token = empty;
            }
            return empty;
        }

        public TINResponse GetCompanyInfo(string taxCode, string captchaCode)
        {
            var tinResponse = new TINResponse();
            string str = PostJsonRequest("", $"http://www.tracuuhoadon.gdt.gov.vn/gettin.html?tin={taxCode}&captchaCode={captchaCode}");

            if (!IsValidJson(str))
                throw new InvalidResponseJsonException(str);

            if (!string.IsNullOrWhiteSpace(str))
            {
                tinResponse = JsonConvert.DeserializeObject<TINResponse>(str);
            }
            return tinResponse;
        }

        public ReleaseResponse GetInvoiceReleases(string taxCode, string fromDate, string toDate, string captchaCode, int page, string kind)
        {
            ReleaseResponse releaseResponse = new ReleaseResponse();
            double totalMilliseconds = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if (!string.IsNullOrEmpty(GetNewToken()))
            {
                string tbph = GetRequest($"http://www.tracuuhoadon.gdt.gov.vn/searchtbph.html?_search=false&nd={totalMilliseconds}&rows=1000&page={page}&sidx=&sord=asc&kind={kind}&tin={taxCode}&ngayTu={fromDate}&ngayDen={toDate}&captchaCode={captchaCode}&token={_token}&struts.token.name=token&_={_tokenName}");
                if (!IsValidJson(tbph))
                    throw new InvalidResponseJsonException(tbph);

                if (!string.IsNullOrWhiteSpace(tbph))
                {
                    releaseResponse = JsonConvert.DeserializeObject<ReleaseResponse>(tbph);
                }
            }
            return releaseResponse;
        }

        public ReleaseDetailResponse GetInvoiceReleaseDetails(string releaseId, int? ltd, bool search, string taxCode, string type, string urlPrefix = "www")
        {
            ReleaseDetailResponse releaseDetailResponse = new ReleaseDetailResponse();
            if (!ltd.HasValue)
                ltd = 0;
            string paramz = $"id={releaseId}&ltd={ltd}&dtnt_tin={taxCode}&loaitb_phanh={type}";
            string viewUrl = $"http://{urlPrefix}.tracuuhoadon.gdt.gov.vn/viewtbph.html?{paramz}";
            if (!string.IsNullOrWhiteSpace(PostJsonRequest(paramz, viewUrl)))
            {
                string detailsUrl = $"http://{urlPrefix}.tracuuhoadon.gdt.gov.vn/gettbphdtl.html?id={releaseId}&ltd={ltd}";
                ++_tokenName;
                string details = GetRequest(detailsUrl);
                if (!IsValidJson(details))
                    throw new InvalidResponseJsonException(details);
                if (!string.IsNullOrWhiteSpace(details))
                {
                    releaseDetailResponse = JsonConvert.DeserializeObject<ReleaseDetailResponse>(details);
                }
            }
            return releaseDetailResponse;
        }

        public string PostJsonRequest(string data, string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.ContentLength = bytes.Length;
            if (_cookies == null)
                GetCookies();
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(_cookies);
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                UpdateCookie(response);
                using (var responseStream = response.GetResponseStream())
                using (var streamReader = new StreamReader(responseStream))
                {
                    string end = streamReader.ReadToEnd();
                    return end;
                }
            }
        }

        public string PostParamsRequest(string postData, string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
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
            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                UpdateCookie(response);
                using (var responseStream = response.GetResponseStream())
                using (var streamReader = new StreamReader(responseStream))
                {
                    string end = streamReader.ReadToEnd();
                    return end;
                }
            }
        }

        public string GetRequest(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            if (_cookies == null)
                GetCookies();
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(_cookies);
            using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                UpdateCookie(response);
                using (var responseStream = response.GetResponseStream())
                using (var streamReader = new StreamReader(responseStream))
                {
                    string end = streamReader.ReadToEnd();
                    return end;
                }
            }
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
            System.IO.File.AppendAllText(string.Format("{0}\\Log.txt", AppDomain.CurrentDomain.BaseDirectory), $"{DateTime.Now} - {ex}");
            System.IO.File.AppendAllText(string.Format("{0}\\Log.txt", AppDomain.CurrentDomain.BaseDirectory), $"{DateTime.Now} - {ex.StackTrace}");
        }

        private bool ValidateCode(string captchaCode)
        {
            try
            {
                var response = PostParamsRequest("captchaCode=" + captchaCode, "http://tracuuhoadon.gdt.gov.vn/validcode.html");
                return !response.Contains("Sai mã");
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        #endregion

        public static bool IsValidJson(string value)
        {
            try
            {
                var json = JToken.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void PictureBoxLogo_Click(object sender, EventArgs e)
        {
            Process.Start("https://easyinvoice.vn/");
        }

        private void ButtonExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (_data != null && _data.Any())
                {
                    Directory.CreateDirectory(_output);
                    var excelFile = ExportDataToExcel(_data, _output);
                    System.Diagnostics.Process.Start(excelFile);
                }
                else
                {
                    BridgePopupMessage("Không có dữ liệu để xuất");
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                BridgePopupMessage("Có lỗi trong quá trình xuất dữ liệu");
            }
        }
    }
}
