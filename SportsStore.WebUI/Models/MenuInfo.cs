using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Models {
    public class MenuInfo {
        public IEnumerable<string> Categories       { get; set; }
        public string              SelectedCategory { get; set; }
    }
}