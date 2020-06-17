using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Procon2020 {
  static class Program {
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      Application.Run(new Form1( new Iwamoto("Iwamoto"),new geister.taiyo.HaraPlayer("hara")));
      //                              ↑ここと                ↑ここを変える
    }
  }
}
