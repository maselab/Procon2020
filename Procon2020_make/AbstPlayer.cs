using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Procon2020 {
  public abstract class AbstPlayer {
    public AbstPlayer() { }
    public AbstPlayer(string name) { this.name = name; }
    public abstract Dicision Dicision();
        public abstract Init_pos Init_pos();
        public string name = null;
  }
}
