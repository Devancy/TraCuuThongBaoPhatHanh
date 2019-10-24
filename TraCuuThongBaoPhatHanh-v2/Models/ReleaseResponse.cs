// Decompiled with JetBrains decompiler
// Type: TraCuuThongBaoPhatHanh_v2.ReleaseResponse
// Assembly: TraCuuThongBaoPhatHanh_v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A03554FB-459D-443E-B916-13C92B22B6CB
// Assembly location: D:\EasyInvoice\crawler\TraCuuThongBaoPhatHanh_v2V3\TraCuuThongBaoPhatHanh_v2.exe

using System.Collections.Generic;

namespace TraCuuThongBaoPhatHanh_v2
{
  public class ReleaseResponse : General
  {
    public string tin { get; set; }

    public string captchaCode { get; set; }

    public string ngayTu { get; set; }

    public string ngayDen { get; set; }

    public TINModel tinModel { get; set; }

    public string tinProc { get; set; }

    public string page { get; set; }

    public string pager { get; set; }

    public string total { get; set; }

    public string rows { get; set; }

    public string records { get; set; }

    public ParamSearch paramSearch { get; set; }

    public List<Release> list { get; set; }
  }
}
