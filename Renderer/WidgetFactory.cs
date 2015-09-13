using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Renderer {
    public static class WidgetFactory {
        public static Button Button(string val, Action callback) {
            var toReturn = new Button();
            toReturn.Content = val;
            toReturn.Click += (s, e) => {
                callback();
            };
            return toReturn;
        }
    }
}
