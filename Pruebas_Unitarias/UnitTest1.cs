using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegexAnalizer;

namespace Pruebas_Unitarias
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DNI()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("\\b\\d{8}[A-Z]\\b", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Componentes = new System.Collections.ArrayList();
            RegExItem primero = new RegExItem("Limite de Palabra",1,257,"");
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem("Digito",1,255,"b8");
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem(null,4,2,"");
            RegExItem tercero1 = new RegExItem("A-Z",9,0,null);
            tercero.Componentes.Add(tercero1);
            esperado.Componentes.Add(tercero);
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado);
        }

        //un DNI está formado por 8 dígitos seguidos de una de las letras “ABCDEFGHJKLMNPQRSTVWXYZ”
        [TestMethod]
        public void DNI_Dif()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("\\b\\d{8}[A-HJ-NP-TV-Z]\\b", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Componentes = new System.Collections.ArrayList();
            RegExItem primero = new RegExItem("Limite de Palabra",1,257,"");
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem("Digito",1,255,"b8");
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem(null,4,2,"");
            RegExItem tercero1 = new RegExItem("A-H",9,0,null);
            RegExItem tercero2 = new RegExItem("J-N",9,0,null);
            RegExItem tercero3 = new RegExItem("P-T",9,0,null);
            RegExItem tercero4 = new RegExItem("V-Z",9,0,null);
            tercero.Componentes.Add(tercero1);
            tercero.Componentes.Add(tercero2);
            tercero.Componentes.Add(tercero3);
            tercero.Componentes.Add(tercero4);
            esperado.Componentes.Add(tercero);
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado);
        }

        //empieza con la secuencia “0x”, para indicar que se trata de un número en hexadecimal, seguida de una combinación de dígitos y de las letras “ABCDEF”
        [TestMethod]
        public void Numero_Hexadecimal()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("\\b0x[\\dA-F]+\\b", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Componentes = new System.Collections.ArrayList();
            RegExItem primero = new RegExItem("Limite de Palabra",1,257,"");
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem("0x",1,0,"");
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem(null,4,2,"+");
            RegExItem tercero1 = new RegExItem("Digito",1,255,null);
            tercero.Componentes.Add(tercero1);
            RegExItem tercero2 = new RegExItem("A-F",9,0,null);
            tercero.Componentes.Add(tercero2);
            esperado.Componentes.Add(tercero);
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado);
        }

        //Número entero: una secuencia de dígitos precedidos opcionalmente por un carácter de signo (‘+’ o ‘-’)
        [TestMethod]
        public void Numero_Entero()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("\\b[+-]?\\d+\\b", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Componentes = new System.Collections.ArrayList();
            RegExItem primero = new RegExItem("Limite de Palabra",1,257,"");
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem(null,4,2,"?");
            RegExItem segundo1 = new RegExItem("+",1,0,null);
            RegExItem segundo2 = new RegExItem("-",1,0,null);
            segundo.Componentes.Add(segundo1);
            segundo.Componentes.Add(segundo2);
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem("Digito",1,255,"+");
            esperado.Componentes.Add(tercero);
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado);
        }

        //Una palabra que termina igual que empieza (por ejemplo: “agua”, “ascetas”)
        [TestMethod]
        public void Palabra()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("\\b(\\w)+(\\w)*\\1\\b", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Componentes = new System.Collections.ArrayList();
            RegExItem primero = new RegExItem("Limite de Palabra",1,257,"");
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem(null,2,0,"+");
            RegExItem segundo1 = new RegExItem("Caracter Alfanumerico",1,252,"");
            segundo.Componentes.Add(segundo1);
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem(null,2,0,"*");
            RegExItem tercero1 = new RegExItem("Caracter Alfanumerico",1,252,"");
            tercero.Componentes.Add(tercero1);
            esperado.Componentes.Add(tercero);
            RegExItem cuarto = new RegExItem("1",1,300,"");
            esperado.Componentes.Add(cuarto);  
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado);
        }

        //Caso de la barra alternativa
        [TestMethod]
        public void Barra()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("mañana|tarde", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Repeticiones = null;
            esperado.Componentes = new System.Collections.ArrayList();
            RegExItem primero = new RegExItem(null,3,0,null);
            RegExItem primero1 = new RegExItem(null,0,0,null);
            RegExItem primero1_1 = new RegExItem("mañana",1,0,"");
            primero1.Componentes.Add(primero1_1);
            RegExItem primero2 = new RegExItem(null,0,0,null);
            RegExItem primero2_2 = new RegExItem("tarde",1,0,"");
            primero2.Componentes.Add(primero2_2);
            primero.Componentes.Add(primero1);
            primero.Componentes.Add(primero2);
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado); 
        }

        /*Fecha compleja: formada por dos dígitos para el día, un separador, que puede ser un guion ‘-’o una barra ‘/’, dos dígitos para el mes, otro separador igual al anterior, y cuatro dígitos para el año. Hay que tener en cuenta que:
             El mes de febrero (“02”) no puede tener nunca más de 29 días.
             Los meses de abril (“04”), junio (“06”), septiembre (“09”), y noviembre (“11”) llegan a los 30 días
             El resto de los meses llegan a los 31 días. Ningún mes tiene más de 31 días*/
        [TestMethod]
        public void Fecha_Compleja()
        {
            Analizador ana = new Analizador();
            RegExItem resultado = ana.analize("\\b(((0[1-9]|(1|2)\\d)([-/])(0[1-9]|1[0-2]))|(3((0)\\5(0[469]|11)|(1)\\5(0[13578]|1[02]))))\\5\\d{4}\\b", true);
            RegExItem esperado = new RegExItem();
            esperado.Tipo = 0;
            esperado.Subtipo = 0;
            esperado.Repeticiones = null;
            esperado.Componentes = new System.Collections.ArrayList();

            RegExItem limitePalabra = new RegExItem("Limite de Palabra", 1, 257, "");
            esperado.Componentes.Add(limitePalabra);

            RegExItem Grupo1 = new RegExItem(null, 2, 0, "");

            RegExItem big = new RegExItem(null,3,0,null);

            RegExItem grupoDos = new RegExItem(null, 2, 0, "");

            RegExItem a = new RegExItem(null, 2, 0, "");

            RegExItem tercero = new RegExItem(null, 3, 0, null);

            RegExItem PrimerCamino = new RegExItem(null, 0, 0, null);

            RegExItem PC = new RegExItem("0", 1, 0, "");
            PrimerCamino.Componentes.Add(PC);

            RegExItem cuat = new RegExItem(null, 4, 2, "");

            RegExItem cuatIn = new RegExItem("1-9", 9, 0, null);
            cuat.Componentes.Add(cuatIn);
            PrimerCamino.Componentes.Add(cuat);
            tercero.Componentes.Add(PrimerCamino);

            RegExItem SegundoCamino = new RegExItem(null, 0, 0, null);

            RegExItem segundo = new RegExItem(null, 2, 0, "");

            RegExItem primero = new RegExItem(null, 3, 0, null);

            RegExItem primero1 = new RegExItem(null, 0, 0, null);

            RegExItem primero11 = new RegExItem("1", 1, 0, "");
            primero1.Componentes.Add(primero11);

            RegExItem primero2 = new RegExItem(null, 0, 0, null);

            RegExItem primero22 = new RegExItem("2", 1, 0, "");
            primero2.Componentes.Add(primero22);
            primero.Componentes.Add(primero1);
            primero.Componentes.Add(primero2);
            segundo.Componentes.Add(primero);
            SegundoCamino.Componentes.Add(segundo);

            RegExItem digito = new RegExItem("Digito", 1, 255, "");
            SegundoCamino.Componentes.Add(digito);
            tercero.Componentes.Add(SegundoCamino);
            a.Componentes.Add(tercero);

            RegExItem b = new RegExItem(null, 2, 0, "");

            RegExItem bc = new RegExItem(null, 4, 2, "");

            RegExItem bcd = new RegExItem("-", 1, 0, null);
            bc.Componentes.Add(bcd);

            RegExItem bce = new RegExItem("/", 1, 0, null);
            bc.Componentes.Add(bce);
            b.Componentes.Add(bc);

            RegExItem c = new RegExItem(null, 2, 0, "");

            RegExItem cIn = new RegExItem(null, 3, 0, null);

            RegExItem cIn1 = new RegExItem(null, 0, 0, null);

            RegExItem cInO = new RegExItem("0", 1, 0, "");
            cIn1.Componentes.Add(cInO);

            RegExItem cInOI = new RegExItem(null, 4, 2, "");

            RegExItem neun = new RegExItem("1-9", 9, 0, null);
            cInOI.Componentes.Add(neun);
            cIn1.Componentes.Add(cInOI);
            cIn.Componentes.Add(cIn1);

            RegExItem cIn2 = new RegExItem(null, 0, 0, null);

            RegExItem cIn2O = new RegExItem("1", 1, 0, "");
            cIn2.Componentes.Add(cIn2O);

            RegExItem cInrOI = new RegExItem(null, 4, 2, "");

            RegExItem rr = new RegExItem("0-2", 9, 0, null);
            cInrOI.Componentes.Add(rr);
            cIn2.Componentes.Add(cInrOI);
            cIn.Componentes.Add(cIn2);

            c.Componentes.Add(cIn);

            grupoDos.Componentes.Add(a);
            grupoDos.Componentes.Add(b);
            grupoDos.Componentes.Add(c);

            RegExItem beforeGrupo2 = new RegExItem(null, 0, 0, null);
            beforeGrupo2.Componentes.Add(grupoDos);

            big.Componentes.Add(beforeGrupo2);

            RegExItem grupo7 = new RegExItem(null, 2, 0, "");

            RegExItem numeroTres = new RegExItem("3", 1, 0, "");
            grupo7.Componentes.Add(numeroTres);

            RegExItem grupo8 = new RegExItem(null, 2, 0, "");

            RegExItem tt = new RegExItem(null, 3, 0, null);

            RegExItem re = new RegExItem(null, 0, 0, null);

            RegExItem itO = new RegExItem(null, 2, 0, "");
            RegExItem itINO = new RegExItem("0", 1, 0, "");
            itO.Componentes.Add(itINO);
            re.Componentes.Add(itO);
            RegExItem back5O = new RegExItem("5", 1, 300, "");
            re.Componentes.Add(back5O);

            RegExItem grupoDoceO = new RegExItem(null, 2, 0, "");

            RegExItem tresKO = new RegExItem(null, 3, 0, null);

            RegExItem tresKCO = new RegExItem(null, 0, 0, null);

            RegExItem ceroKO = new RegExItem("0", 1, 0, "");
            tresKCO.Componentes.Add(ceroKO);
            RegExItem unoKO = new RegExItem(null, 4, 2, "");
            RegExItem unoK0O = new RegExItem("4", 1, 0, null);
            unoKO.Componentes.Add(unoK0O);
            RegExItem unoK1O = new RegExItem("6", 1, 0, null);
            unoKO.Componentes.Add(unoK1O);
            RegExItem unoK2O = new RegExItem("9", 1, 0, null);
            unoKO.Componentes.Add(unoK2O);

            tresKCO.Componentes.Add(unoKO);
            tresKO.Componentes.Add(tresKCO);

            RegExItem tresKUO = new RegExItem(null, 0, 0, null);
            RegExItem oneO = new RegExItem("11", 1, 0, "");
            tresKUO.Componentes.Add(oneO);

            tresKO.Componentes.Add(tresKUO);

            grupoDoceO.Componentes.Add(tresKO);
            re.Componentes.Add(grupoDoceO);

            tt.Componentes.Add(re);

            RegExItem rw = new RegExItem(null, 0, 0, null);

            RegExItem it = new RegExItem(null, 2, 0, "");
            RegExItem itIN = new RegExItem("1", 1, 0, "");
            it.Componentes.Add(itIN);
            rw.Componentes.Add(it);
            RegExItem back5 = new RegExItem("5", 1, 300, "");
            rw.Componentes.Add(back5);

            RegExItem grupoDoce = new RegExItem(null, 2, 0, "");

            RegExItem tresK = new RegExItem(null, 3, 0, null);

            RegExItem tresKC = new RegExItem(null, 0, 0, null);

            RegExItem ceroK = new RegExItem("0", 1, 0, "");
            tresKC.Componentes.Add(ceroK);
            RegExItem unoK = new RegExItem(null, 4, 2, "");
            RegExItem unoK0 = new RegExItem("1", 1, 0, null);
            unoK.Componentes.Add(unoK0);
            RegExItem unoK1 = new RegExItem("3", 1, 0, null);
            unoK.Componentes.Add(unoK1);
            RegExItem unoK2 = new RegExItem("5", 1, 0, null);
            unoK.Componentes.Add(unoK2);
            RegExItem unoK3 = new RegExItem("7", 1, 0, null);
            unoK.Componentes.Add(unoK3);
            RegExItem unoK4 = new RegExItem("8", 1, 0, null);
            unoK.Componentes.Add(unoK4);
            tresKC.Componentes.Add(unoK);
            tresK.Componentes.Add(tresKC);

            RegExItem tresKU = new RegExItem(null, 0, 0, null);
            RegExItem one = new RegExItem("1", 1, 0, "");
            tresKU.Componentes.Add(one);
            RegExItem tw = new RegExItem(null, 4, 2, "");

            RegExItem tw1 = new RegExItem("0", 1, 0, null);

            RegExItem tw2 = new RegExItem("2", 1, 0, null);

            tw.Componentes.Add(tw1);
            tw.Componentes.Add(tw2);
            tresKU.Componentes.Add(tw);

            tresK.Componentes.Add(tresKU);

            grupoDoce.Componentes.Add(tresK);
            rw.Componentes.Add(grupoDoce);

            tt.Componentes.Add(rw);
            grupo8.Componentes.Add(tt);
            grupo7.Componentes.Add(grupo8);
            RegExItem before7 = new RegExItem(null, 0, 0, null);

            before7.Componentes.Add(grupo7);
            big.Componentes.Add(before7);
            Grupo1.Componentes.Add(big);
            esperado.Componentes.Add(Grupo1);

            RegExItem referencia5 = new RegExItem("5", 1, 300, "");
            esperado.Componentes.Add(referencia5);

            RegExItem digit = new RegExItem("Digito", 1, 255, "b4");
            esperado.Componentes.Add(digit);
            esperado.Componentes.Add(limitePalabra);
            Assert.AreEqual(esperado, resultado);
        }
    }
}
