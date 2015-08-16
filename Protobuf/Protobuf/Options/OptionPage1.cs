
namespace MichaelReukauff.Protobuf.Options
{
    using System.ComponentModel;

    using Microsoft.VisualStudio.Shell;

    [Category("Protobuf")]
    [DisplayName("Protobuf")]
    [Description("Protobuf Settings")]
    class OptionPage1 : DialogPage
    {
        public int OptionNo { get; set; }

        public bool AutomaticConvert { get; set; }
    }
}
