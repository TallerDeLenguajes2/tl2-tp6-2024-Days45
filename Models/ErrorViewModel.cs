using System;

namespace EspacioTp5
{
  public class ErrorViewModel
  {
    public string RequestId { get; set; }
    public Exception Exception { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
  }
}
