using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using System.Web.Services;
using RegexAnalizer;

namespace WebService1
{
    /// <summary>

    /// Descripción breve de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        public RegexAnalizer.Analizador analizador = new RegexAnalizer.Analizador();

        [WebMethod]
        public RegexAnalizer.RegExItem analizar(string text)
        {
            RegexAnalizer.RegExItem inputWeb = analizador.analize(text,true);
            return inputWeb;
        }
        [WebMethod]
        public ArrayList analizar_match(string reg, string m)
        {
            ArrayList match_web = analizador.analize_macht(reg, m);
            return match_web;
        }
    }
}