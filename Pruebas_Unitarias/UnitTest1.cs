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
            RegExItem primero = new RegExItem();
            primero.Tipo = 1;
            primero.Subtipo = 257;
            primero.Valor = "Limite de Palabra";
            primero.Repeticiones = "";
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem();
            segundo.Tipo = 1;
            segundo.Subtipo = 255;
            segundo.Valor = "Digito";
            segundo.Repeticiones = "b8";
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem();
            tercero.Tipo = 4;
            tercero.Subtipo = 2;
            tercero.Valor = null;
            tercero.Repeticiones = "";
            RegExItem tercero1 = new RegExItem();
            tercero1.Tipo = 9;
            tercero1.Subtipo = 0;
            tercero1.Valor = "A-Z";
            tercero1.Repeticiones = null;
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
            RegExItem primero = new RegExItem();
            primero.Tipo = 1;
            primero.Subtipo = 257;
            primero.Valor = "Limite de Palabra";
            primero.Repeticiones = "";
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem();
            segundo.Tipo = 1;
            segundo.Subtipo = 255;
            segundo.Valor = "Digito";
            segundo.Repeticiones = "b8";
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem();
            tercero.Tipo = 4;
            tercero.Subtipo = 2;
            tercero.Valor = null;
            tercero.Repeticiones = "";
            RegExItem tercero1 = new RegExItem();
            tercero1.Tipo = 9;
            tercero1.Subtipo = 0;
            tercero1.Valor = "A-H";
            tercero1.Repeticiones = null;
            RegExItem tercero2 = new RegExItem();
            tercero2.Tipo = 9;
            tercero2.Subtipo = 0;
            tercero2.Valor = "J-N";
            tercero2.Repeticiones = null;
            RegExItem tercero3 = new RegExItem();
            tercero3.Tipo = 9;
            tercero3.Subtipo = 0;
            tercero3.Valor = "P-T";
            tercero3.Repeticiones = null;
            RegExItem tercero4 = new RegExItem();
            tercero4.Tipo = 9;
            tercero4.Subtipo = 0;
            tercero4.Valor = "V-Z";
            tercero4.Repeticiones = null;
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
            RegExItem primero = new RegExItem();
            primero.Tipo = 1;
            primero.Subtipo = 257;
            primero.Valor = "Limite de Palabra";
            primero.Repeticiones = "";
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem();
            segundo.Tipo = 1;
            segundo.Subtipo = 0;
            segundo.Valor = "0x";
            segundo.Repeticiones = "";
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem();
            tercero.Tipo = 4;
            tercero.Subtipo = 2;
            tercero.Valor = null;
            tercero.Repeticiones = "+";
            RegExItem tercero1 = new RegExItem();
            tercero1.Tipo = 1;
            tercero1.Subtipo = 255;
            tercero1.Valor = "Digito";
            tercero1.Repeticiones = null;
            tercero.Componentes.Add(tercero1);
            RegExItem tercero2 = new RegExItem();
            tercero2.Tipo = 9;
            tercero2.Subtipo = 0;
            tercero2.Valor = "A-F";
            tercero2.Repeticiones = null;
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
            RegExItem primero = new RegExItem();
            primero.Tipo = 1;
            primero.Subtipo = 257;
            primero.Valor = "Limite de Palabra";
            primero.Repeticiones = "";
            esperado.Componentes.Add(primero);
            RegExItem segundo = new RegExItem();
            segundo.Tipo = 4;
            segundo.Subtipo = 2;
            segundo.Valor = null;
            segundo.Repeticiones = "?";
            RegExItem segundo1 = new RegExItem();
            segundo1.Tipo = 1;
            segundo1.Subtipo = 0;
            segundo1.Valor = "+";
            segundo1.Repeticiones = null;
            RegExItem segundo2 = new RegExItem();
            segundo2.Tipo = 1;
            segundo2.Subtipo = 0;
            segundo2.Valor = "-";
            segundo2.Repeticiones = null;
            segundo.Componentes.Add(segundo1);
            segundo.Componentes.Add(segundo2);
            esperado.Componentes.Add(segundo);
            RegExItem tercero = new RegExItem();
            tercero.Tipo = 1;
            tercero.Subtipo = 255;
            tercero.Valor = "Digito";
            tercero.Repeticiones = "+";
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
            RegExItem primero = new RegExItem();
            primero.Tipo = 1;
            primero.Subtipo = 257;
            primero.Valor = "Limite de Palabra";
            primero.Repeticiones = "";
            esperado.Componentes.Add(primero);

            RegExItem segundo = new RegExItem();
            segundo.Tipo = 2;
            segundo.Subtipo = 0;
            segundo.Valor = null;
            segundo.Repeticiones = "+";
            RegExItem segundo1 = new RegExItem();
            segundo1.Tipo = 1;
            segundo1.Subtipo = 252;
            segundo1.Valor = "Caracter Alfanumerico";
            segundo1.Repeticiones = "";
            segundo.Componentes.Add(segundo1);
            esperado.Componentes.Add(segundo);


            RegExItem tercero = new RegExItem();
            tercero.Tipo = 2;
            tercero.Subtipo = 0;
            tercero.Valor = null;
            tercero.Repeticiones = "*";
            RegExItem tercero1 = new RegExItem();
            tercero1.Tipo = 1;
            tercero1.Subtipo = 252;
            tercero1.Valor = "Caracter Alfanumerico";
            tercero1.Repeticiones = "";
            tercero.Componentes.Add(tercero1);
            esperado.Componentes.Add(tercero);


            RegExItem cuarto = new RegExItem();
            cuarto.Tipo = 1;
            cuarto.Subtipo = 300;
            cuarto.Valor = "1";
            cuarto.Repeticiones = "";
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
            RegExItem primero = new RegExItem();
            primero.Tipo = 3;
            primero.Subtipo = 0;
            primero.Valor = null;
            primero.Repeticiones = null;
            RegExItem primero1 = new RegExItem();
            primero1.Tipo = 0;
            primero1.Subtipo = 0;
            primero1.Valor = null;
            primero1.Repeticiones = null;
            RegExItem primero11 = new RegExItem();
            primero11.Tipo = 1;
            primero11.Subtipo = 0;
            primero11.Valor = "mañana";
            primero11.Repeticiones = "";
            primero1.Componentes.Add(primero11);
            RegExItem primero2 = new RegExItem();
            primero2.Tipo = 0;
            primero2.Subtipo = 0;
            primero2.Valor = null;
            primero2.Repeticiones = null;
            RegExItem primero22 = new RegExItem();
            primero22.Tipo = 1;
            primero22.Subtipo = 0;
            primero22.Valor = "tarde";
            primero22.Repeticiones = "";
            primero2.Componentes.Add(primero22);
            primero.Componentes.Add(primero1);
            primero.Componentes.Add(primero2);
            esperado.Componentes.Add(primero);
            Assert.AreEqual(esperado, resultado);
 
        }
    }
}
