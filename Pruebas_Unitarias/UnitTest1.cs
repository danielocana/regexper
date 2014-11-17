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

            RegExItem grupoTres = new RegExItem(null, 2, 0, "");

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
            grupoTres.Componentes.Add(tercero);

            RegExItem grupoCinco = new RegExItem(null, 2, 0, "");

            RegExItem Tipo_4 = new RegExItem(null, 4, 2, "");

            RegExItem bcd = new RegExItem("-", 1, 0, null);
            Tipo_4.Componentes.Add(bcd);

            RegExItem barra_inversa = new RegExItem("/", 1, 0, null);
            Tipo_4.Componentes.Add(barra_inversa);
            grupoCinco.Componentes.Add(Tipo_4);

            RegExItem grupoSeis = new RegExItem(null, 2, 0, "");

            RegExItem Tipo_3 = new RegExItem(null, 3, 0, null);

            RegExItem Tipo_3_Inter1 = new RegExItem(null, 0, 0, null);

            RegExItem Tipo_3_Inter2 = new RegExItem("0", 1, 0, "");
            Tipo_3_Inter1.Componentes.Add(Tipo_3_Inter2);

            RegExItem Tipo_3_Inter_Cor = new RegExItem(null, 4, 2, "");

            RegExItem rango1 = new RegExItem("1-9", 9, 0, null);
            Tipo_3_Inter_Cor.Componentes.Add(rango1);
            Tipo_3_Inter1.Componentes.Add(Tipo_3_Inter_Cor);
            Tipo_3.Componentes.Add(Tipo_3_Inter1);

            RegExItem Tipo3_Inter3 = new RegExItem(null, 0, 0, null);

            RegExItem Tipo_3_simple = new RegExItem("1", 1, 0, "");
            Tipo3_Inter3.Componentes.Add(Tipo_3_simple);

            RegExItem Corchetes = new RegExItem(null, 4, 2, "");

            RegExItem rango2 = new RegExItem("0-2", 9, 0, null);
            Corchetes.Componentes.Add(rango2);
            Tipo3_Inter3.Componentes.Add(Corchetes);
            Tipo_3.Componentes.Add(Tipo3_Inter3);

            grupoSeis.Componentes.Add(Tipo_3);

            grupoDos.Componentes.Add(grupoTres);
            grupoDos.Componentes.Add(grupoCinco);
            grupoDos.Componentes.Add(grupoSeis);

            RegExItem beforeGrupo2 = new RegExItem(null, 0, 0, null);
            beforeGrupo2.Componentes.Add(grupoDos);

            big.Componentes.Add(beforeGrupo2);

            RegExItem grupo7 = new RegExItem(null, 2, 0, "");

            RegExItem numeroTres = new RegExItem("3", 1, 0, "");
            grupo7.Componentes.Add(numeroTres);

            RegExItem grupo8 = new RegExItem(null, 2, 0, "");

            RegExItem Tipo_3_segundo = new RegExItem(null, 3, 0, null);

            RegExItem interno_Tipo_3_segundo = new RegExItem(null, 0, 0, null);

            RegExItem tipo_3_grupo1 = new RegExItem(null, 2, 0, "");
            RegExItem tip_3_simple = new RegExItem("0", 1, 0, "");
            tipo_3_grupo1.Componentes.Add(tip_3_simple);
            interno_Tipo_3_segundo.Componentes.Add(tipo_3_grupo1);
            RegExItem ref_grupoCinco = new RegExItem("5", 1, 300, "");
            interno_Tipo_3_segundo.Componentes.Add(ref_grupoCinco);

            RegExItem grupoDoce_cero = new RegExItem(null, 2, 0, "");

            RegExItem tipo_3_doce = new RegExItem(null, 3, 0, null);

            RegExItem interno_tipo3_doce = new RegExItem(null, 0, 0, null);

            RegExItem ceroKO = new RegExItem("0", 1, 0, "");
            interno_tipo3_doce.Componentes.Add(ceroKO);
            RegExItem tipo4_doce = new RegExItem(null, 4, 2, "");
            RegExItem simple_tipo4 = new RegExItem("4", 1, 0, null);
            tipo4_doce.Componentes.Add(simple_tipo4);
            RegExItem tipo4_simple2 = new RegExItem("6", 1, 0, null);
            tipo4_doce.Componentes.Add(tipo4_simple2);
            RegExItem tipo9 = new RegExItem("9", 1, 0, null);
            tipo4_doce.Componentes.Add(tipo9);

            interno_tipo3_doce.Componentes.Add(tipo4_doce);
            tipo_3_doce.Componentes.Add(interno_tipo3_doce);

            RegExItem tipo3_segund = new RegExItem(null, 0, 0, null);
            RegExItem simple_once = new RegExItem("11", 1, 0, "");
            tipo3_segund.Componentes.Add(simple_once);

            tipo_3_doce.Componentes.Add(tipo3_segund);

            grupoDoce_cero.Componentes.Add(tipo_3_doce);
            interno_Tipo_3_segundo.Componentes.Add(grupoDoce_cero);

            Tipo_3_segundo.Componentes.Add(interno_Tipo_3_segundo);

            RegExItem grupo_cero_siguiente = new RegExItem(null, 0, 0, null);

            RegExItem grupo_X = new RegExItem(null, 2, 0, "");
            RegExItem itIN = new RegExItem("1", 1, 0, "");
            grupo_X.Componentes.Add(itIN);
            grupo_cero_siguiente.Componentes.Add(grupo_X);
            RegExItem refe_cinco = new RegExItem("5", 1, 300, "");
            grupo_cero_siguiente.Componentes.Add(refe_cinco);

            RegExItem grupoDoce = new RegExItem(null, 2, 0, "");

            RegExItem tipo3_num = new RegExItem(null, 3, 0, null);

            RegExItem tipo0_uno = new RegExItem(null, 0, 0, null);

            RegExItem tipo3_cero_X = new RegExItem("0", 1, 0, "");
            tipo0_uno.Componentes.Add(tipo3_cero_X);
            RegExItem cuatro_numeros = new RegExItem(null, 4, 2, "");
            RegExItem uno_tipo3 = new RegExItem("1", 1, 0, null);
            cuatro_numeros.Componentes.Add(uno_tipo3);
            RegExItem uno_tipo3_2 = new RegExItem("3", 1, 0, null);
            cuatro_numeros.Componentes.Add(uno_tipo3_2);
            RegExItem simple_cinco = new RegExItem("5", 1, 0, null);
            cuatro_numeros.Componentes.Add(simple_cinco);
            RegExItem simple_siete = new RegExItem("7", 1, 0, null);
            cuatro_numeros.Componentes.Add(simple_siete);
            RegExItem simple_ocho = new RegExItem("8", 1, 0, null);
            cuatro_numeros.Componentes.Add(simple_ocho);
            tipo0_uno.Componentes.Add(cuatro_numeros);
            tipo3_num.Componentes.Add(tipo0_uno);

            RegExItem tipo3_sig = new RegExItem(null, 0, 0, null);
            RegExItem simple_uno = new RegExItem("1", 1, 0, "");
            tipo3_sig.Componentes.Add(simple_uno);
            RegExItem tipo4 = new RegExItem(null, 4, 2, "");

            RegExItem tipo4_uno = new RegExItem("0", 1, 0, null);

            RegExItem tipo4_dos = new RegExItem("2", 1, 0, null);

            tipo4.Componentes.Add(tipo4_uno);
            tipo4.Componentes.Add(tipo4_dos);
            tipo3_sig.Componentes.Add(tipo4);

            tipo3_num.Componentes.Add(tipo3_sig);

            grupoDoce.Componentes.Add(tipo3_num);
            grupo_cero_siguiente.Componentes.Add(grupoDoce);

            Tipo_3_segundo.Componentes.Add(grupo_cero_siguiente);
            grupo8.Componentes.Add(Tipo_3_segundo);
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
