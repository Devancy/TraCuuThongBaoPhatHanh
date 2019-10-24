// Decompiled with JetBrains decompiler
// Type: TraCuuThongBaoPhatHanh_v2.ReleaseDetailResponse
// Assembly: TraCuuThongBaoPhatHanh_v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A03554FB-459D-443E-B916-13C92B22B6CB
// Assembly location: D:\EasyInvoice\crawler\TraCuuThongBaoPhatHanh_v2V3\TraCuuThongBaoPhatHanh_v2.exe

using System.Collections.Generic;

namespace TraCuuThongBaoPhatHanh_v2
{
  public class ReleaseDetailResponse : General
  {
    public bool brcr { get; set; }

    public List<ReleaseDetail> dtls { get; set; }
  }
}
