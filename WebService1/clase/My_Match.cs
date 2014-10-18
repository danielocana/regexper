using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace WebService1.clase
{
    public class My_Match
    {
        ArrayList grupos; //--> contendra los grupos que hagan match
        string valor;
        int ind;
        int len;

        public My_Match()
        {
            grupos = new ArrayList();
        }

        public ArrayList Grupos
        {
            get { return grupos; }
            set { grupos = value; }
        }

        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public int Ind
        {
            get { return ind; }
            set { ind = value; }
        }

        public int Len
        {
            get { return len; }
            set { len = value; }
        }
    }
}