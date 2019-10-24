// Decompiled with JetBrains decompiler
// Type: TraCuuThongBaoPhatHanh_v2.TINResponse
// Assembly: TraCuuThongBaoPhatHanh_v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A03554FB-459D-443E-B916-13C92B22B6CB
// Assembly location: D:\EasyInvoice\crawler\TraCuuThongBaoPhatHanh_v2V3\TraCuuThongBaoPhatHanh_v2.exe

using System.Collections.Generic;

namespace TraCuuThongBaoPhatHanh_v2
{
  public class TINResponse
  {
    public string tin { get; set; }

    public string captchaCode { get; set; }

    public string strMess { get; set; }

    public string dvsd { get; set; }

    public string kind { get; set; }

    public string ltd { get; set; }

    public string ngayDen { get; set; }

    public string ngayTu { get; set; }

    public TINModel tinModel { get; set; }

    public List<Release> Releases { get; set; }
  }
}
