using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace RegexAnalizer
{
    public class Analizador
    {
        //Metodo que genera el RegExItem de Error con tipo = -1
        public RegExItem GenerarError(string valor)
        {
            RegExItem error = new RegExItem();
            error.Tipo = -1;
            error.Valor = valor;
            return error;
        }

        //metodo para crear un RegExItem simple solo con valor
        public RegExItem GenerarSimple(string valor)
        {
            RegExItem simple = new RegExItem();
            simple.Tipo = 1;
            simple.Valor = valor;
            return simple;
        }

        //metodo para crear un RegExItem simple con valor y repeticiones
        public RegExItem GenerarSimple(string valor,string repeticiones)
        {
            RegExItem simple = new RegExItem();
            simple.Tipo = 1;
            simple.Valor = valor;
            simple.Repeticiones = Tratar_repeticiones(repeticiones);
            return simple;
        }

        //metodo que se encargar de elimiar los ceros a la izquierda de las repeticiones
        public string quitar_cero(string cadena)
        {
            while (cadena.StartsWith("0"))
            {
                cadena = cadena.Substring(1);
            }
            return cadena;
        }

        public int[] rep_digitos(string cadena)
        {
            char[] MyChar = { '{', '}' };
            string nueva_repeticion = cadena.TrimEnd(MyChar);
            nueva_repeticion = nueva_repeticion.TrimStart(MyChar);
            int[] numero = new int[2];
            string primer_numero = nueva_repeticion.Substring(0, nueva_repeticion.IndexOf(','));
            string segundo_numero = nueva_repeticion.Substring(nueva_repeticion.IndexOf(',') + 1);
            int uno = 0;
            int dos = 0;
            bool result = int.TryParse(primer_numero, out uno);
            bool res = int.TryParse(segundo_numero, out dos);
            numero[0] = uno;
            numero[1] = dos;
            return numero;
        }

        public string Tratar_repeticiones(string repeticiones)
        {
            if (repeticiones == null)
            {
                return "";
            }
            else 
            {
                char[] MyChar = { '{', '}' };
                string nueva_repeticion = repeticiones.TrimEnd(MyChar);
                nueva_repeticion = nueva_repeticion.TrimStart(MyChar);
                if (nueva_repeticion == "+" || nueva_repeticion == "*" || nueva_repeticion == "?")
                {
                    return nueva_repeticion;
                }
                else
                {
                    if (Regex.IsMatch(nueva_repeticion, @"^\d+$"))
                    {
                        if (quitar_cero(nueva_repeticion) == "1")
                        {
                            return nueva_repeticion = "";
                        }
                        else { return nueva_repeticion = "b" + quitar_cero(nueva_repeticion); }
                    }
                    if (Regex.IsMatch(nueva_repeticion, @"^\d+,\d+$"))
                    {
                        int[] comparar = rep_digitos(repeticiones);
                        if (comparar[0] <= comparar[1])
                        {
                            if (Regex.IsMatch(comparar[0].ToString(), @"^0+$"))
                            /*este caso es a{0,4}, lo que hago es darle una marca al inicio de las repeticiones y asi se cuando en javascript pintarlo de una forma u otra*/
                            {
                                nueva_repeticion = "o" + "< " + comparar[1];
                            }
                            else { nueva_repeticion = "b" + comparar[0] + ".." + comparar[1]; }
                        }
                        else { nueva_repeticion = "m" + comparar[0] + ".." + comparar[1]; }
                    }
                    if (Regex.IsMatch(nueva_repeticion, @"\d+,"))
                    {
                        if (Regex.IsMatch(nueva_repeticion, @"^0+,$"))
                        {
                            return nueva_repeticion = "*";
                        }
                        else
                        {
                            nueva_repeticion = nueva_repeticion.Replace(",", "+");
                            return nueva_repeticion = "b" + quitar_cero(nueva_repeticion);
                        }
                    }
                }
                return nueva_repeticion;
            }
        }

//Este metodo se llamara antes de insertar en el result dado que si las repeticiones son pe: a{0} || a{0,0} no poner ese elemento en el result
        public void insertar(RegExItem result, RegExItem actual, string repeticiones)
        {
            if ((repeticiones== null) || (!(Regex.IsMatch(repeticiones, @"^{0+}$")) && !(Regex.IsMatch(repeticiones, @"^{0+,0+}$"))))
            {
                actual.Repeticiones = Tratar_repeticiones(repeticiones);
                result.Componentes.Add(actual); 
            }
        }

        //Este metodo se encargara en caso de encontrar los simbolos ?! en el principio de los parentesis.
        public RegExItem busqueda_negativa(string variable)
        {
            RegExItem eltosParentesisInter;
            string inicio = "";
            if (variable.Length > 3)
            {
                inicio = variable.Substring(1, 2);
            }
            if (inicio == "?!")
            {
                eltosParentesisInter = analize(variable.Substring(3, (variable.Length - 4)), false);
                eltosParentesisInter.Tipo = 2;
                eltosParentesisInter.Subtipo = 1; //Para este caso es negativo
            }
            else
            {
                //Para este caso el proceso es normal
                eltosParentesisInter = analize(variable.Substring(1, (variable.Length - 2)), false);
                eltosParentesisInter.Tipo = 2; 
            }
            return eltosParentesisInter;
        }

        public RegExItem analize(string input, bool inDeep)
        {
            RegExItem result = new RegExItem();
            result.Tipo = 0;
            int prev = 0;
            int prevLength = 0;

            foreach (Match CurrentMatch in getTokens(input))
            {
                if (CurrentMatch.Index != prev + prevLength)
                {
                    RegExItem ultimoEltodeArrayTipo = new RegExItem();
                    if (result.Componentes.Count != 0)
                    {
                        ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                    }
                    else { ultimoEltodeArrayTipo.Tipo = 0; }
                    if (ultimoEltodeArrayTipo.Tipo == 3)
                    {
                        ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(GenerarError(input.Substring(prev + prevLength, CurrentMatch.Index - (prev + prevLength))));
                    }
                    else
                    {
                        result.Componentes.Add(GenerarError(input.Substring(prev + prevLength, CurrentMatch.Index - (prev + prevLength))));
                    } 
                }
                prev = CurrentMatch.Index;
                prevLength = CurrentMatch.Length;                
                {
                    /*Los subtipos 15 seran los caracertes que por si solo representan algo*/
                    if (CurrentMatch.Groups[1].Value[0] == '^')
                    {                        
                        RegExItem startL = new RegExItem();
                        startL.Valor = "Inicio de Linea";
                        startL.Tipo = 1;
                        startL.Subtipo = 15;
                        if (result.Componentes.Count != 0)
                        {
                            RegExItem ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                            if (ultimoEltodeArrayTipo.Tipo == 3)
                            {
                                insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], startL, CurrentMatch.Groups[3].Value);
                            }
                            else 
                            {
                                startL.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                result.Componentes.Add(startL); } 
                        }
                        else {
                            startL.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                            result.Componentes.Add(startL); }                        
                    }
                    if (CurrentMatch.Groups[1].Value[0] == '$')
                    {
                        RegExItem endL = new RegExItem();
                        endL.Valor = "Final de linea";
                        endL.Tipo = 1;
                        endL.Subtipo = 15;
                        if (result.Componentes.Count != 0)
                        {
                            RegExItem ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                            if (ultimoEltodeArrayTipo.Tipo == 3)
                            {
                                insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], endL, CurrentMatch.Groups[3].Value);
                            }
                            else {
                                endL.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                result.Componentes.Add(endL); } 
                        }
                        else {
                            endL.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                            result.Componentes.Add(endL); } 
                    }
                    if (CurrentMatch.Groups[1].Value[0] == '.')
                    {
                        RegExItem anyL = new RegExItem();
                        anyL.Valor = "Cualquier carácter";
                        anyL.Tipo = 1;
                        anyL.Subtipo = 15;
                        if (result.Componentes.Count != 0)
                        {
                            RegExItem ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                            if (ultimoEltodeArrayTipo.Tipo == 3)
                            {
                                insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], anyL, CurrentMatch.Groups[3].Value);
                            }
                            else
                            {
                                anyL.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                result.Componentes.Add(anyL);
                            }
                        }                        
                        else {
                            anyL.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                            result.Componentes.Add(anyL); }
                    }
                    if (CurrentMatch.Groups[1].Value[0] != '.' && CurrentMatch.Groups[1].Value[0] != '$' && CurrentMatch.Groups[1].Value[0] != '^' && CurrentMatch.Groups[1].Value[0] != '|' && CurrentMatch.Groups[1].Value[0] != '(' && CurrentMatch.Groups[1].Value[0] != '[' && CurrentMatch.Groups[1].Value[0] != '\\')
                    {
                        if (result.Componentes.Count == 0)
                        {
                            //result esta vacio
                            insertar(result, GenerarSimple(CurrentMatch.Groups[1].Value), CurrentMatch.Groups[3].Value);
                        }
                        else
                        {
                            RegExItem ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                            if (result.Componentes.Count != 0)
                            {
                                if (ultimoEltodeArrayTipo.Tipo == 3)
                                {
                                    if ((((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1])).Componentes.Count == 0 && (((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1])).Valor == null)
                                    {
                                        RegExItem actual = new RegExItem();
                                        actual.Valor = CurrentMatch.Groups[1].Value;
                                        actual.Tipo = 1;
                                        insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], actual, CurrentMatch.Groups[3].Value);
                                    }
                                    else
                                    {
 //mirar si es tipo 1 el ultimo elemento de ultimoEltoArray, si no tiene repeticiones y que el currentMatch tampoco tenga repeticiones
                                        var eltoAnterior = ((RegExItem)((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes[((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Count - 1]);
                                        if (eltoAnterior.Tipo == 1 && (eltoAnterior.Subtipo >= 100 && eltoAnterior.Subtipo <=150 || eltoAnterior.Subtipo == 0) && CurrentMatch.Groups[3].Value == "" && eltoAnterior.Repeticiones == "")
                                        {
                                            string cadena = eltoAnterior.Valor;
                                            eltoAnterior.Valor = cadena + CurrentMatch.ToString();
                                        }
                                        else
                                        {
                                            RegExItem actual = new RegExItem();
                                            actual.Valor = CurrentMatch.Groups[1].Value;
                                            actual.Tipo = 1;
                                            insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1],actual,CurrentMatch.Groups[3].Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (ultimoEltodeArrayTipo.Tipo == 1 && CurrentMatch.Groups[3].Value == "" && ultimoEltodeArrayTipo.Repeticiones == "" && (ultimoEltodeArrayTipo.Subtipo >=100 && ultimoEltodeArrayTipo.Subtipo <= 150 || ultimoEltodeArrayTipo.Subtipo == 0))
                                    {
                                        ultimoEltodeArrayTipo.Valor = ultimoEltodeArrayTipo.Valor + CurrentMatch.ToString();
                                        ultimoEltodeArrayTipo.Subtipo = 0;
                                    }
                                    else
                                    {
                                        //result no esta vacio y es simple el elemento aun no hay barra
                                        insertar(result, GenerarSimple(CurrentMatch.Groups[1].Value), CurrentMatch.Groups[3].Value);
                                    }
                                }
                            }
                        }
                    }
                    if (CurrentMatch.Groups[1].Value[0] == '\\')
                    {
                        //Si antes habia una barra
                        RegExItem ultimoEltodeArrayTipo = new RegExItem();
                        if (result.Componentes.Count != 0)
                        {
                            ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                        }
                        else
                        {
                            ultimoEltodeArrayTipo.Tipo = 0;
                        }
                        if (ultimoEltodeArrayTipo.Tipo == 3)
                        {
                            if (char.IsDigit(CurrentMatch.Groups[1].Value[1]))
                            {
                                RegExItem group_reference = new RegExItem();
                                group_reference.Subtipo = 300;
                                group_reference.Tipo = 1;
                                group_reference.Valor = CurrentMatch.Groups[1].Value[1].ToString();
                                insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], group_reference, CurrentMatch.Groups[3].Value);
                            }
                            else
                            {
                                RegExItem letraBarra = caracter(CurrentMatch.Value[1]);
                                letraBarra.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                //ACCEDER AL COMPONENTE DEL COMPONENTE DE RESULT
                                RegExItem ultimo_elemento_tipo3 = new RegExItem();
                                if (((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Count != 0)
                                {
                                    ultimo_elemento_tipo3 = ((RegExItem)((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]));
                                    ultimo_elemento_tipo3 = (RegExItem)ultimo_elemento_tipo3.Componentes[ultimo_elemento_tipo3.Componentes.Count - 1];
                                }
                                else
                                {
                                    ultimo_elemento_tipo3.Tipo = 0;
                                }
                                if (letraBarra.Subtipo >= 100 && letraBarra.Subtipo <= 150 && ultimo_elemento_tipo3.Tipo == 1 && (letraBarra.Repeticiones == "" || letraBarra.Repeticiones == null) && (ultimo_elemento_tipo3.Repeticiones == "" || ultimo_elemento_tipo3.Repeticiones == null) && (ultimo_elemento_tipo3.Subtipo == 0 || ultimo_elemento_tipo3.Subtipo == 100))
                                {
                                    ultimo_elemento_tipo3.Valor = ultimo_elemento_tipo3.Valor + CurrentMatch.Groups[1].Value[1];
                                }
                                else
                                {
                                    insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], letraBarra, CurrentMatch.Groups[3].Value);
                                }
                            }
                        }
                        //Aun no ha llegado ninguna barra 
                        else 
                        {
                            try
                            {
                                if (char.IsDigit(CurrentMatch.Groups[1].Value[1]))
                                {
                                    RegExItem group_reference = new RegExItem();
                                    group_reference.Subtipo = 300;
                                    group_reference.Tipo = 1;
                                    group_reference.Valor = CurrentMatch.Groups[1].Value[1].ToString();
                                    insertar(result, group_reference, CurrentMatch.Groups[3].Value);
                                }
                                else
                                {
                                    RegExItem letraBarra = caracter(CurrentMatch.Value[1]);
                                    letraBarra.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                    /*Todo lo que encuentre despues de una barra pierde su significado exepto los metacaracteres*/
                                    if (letraBarra.Subtipo >= 100 && letraBarra.Subtipo <= 150 && ultimoEltodeArrayTipo.Tipo == 1 && (letraBarra.Repeticiones == "" || letraBarra.Repeticiones == null) && (ultimoEltodeArrayTipo.Repeticiones == "" || ultimoEltodeArrayTipo.Repeticiones == null) && ultimoEltodeArrayTipo.Subtipo == 0)
                                    {
                                        ultimoEltodeArrayTipo.Valor = ultimoEltodeArrayTipo.Valor + CurrentMatch.Groups[1].Value[1];
                                    }
                                    else
                                    {
                                        insertar(result, letraBarra, CurrentMatch.Groups[3].Value);
                                    }
                                }
                            }
                            catch (System.IndexOutOfRangeException e)
                            {
                                result.Componentes.Add(GenerarError(CurrentMatch.ToString()));
                            }                           
                        }
                    }
                    if (CurrentMatch.Groups[1].Value[0] == '[')
                    {
                        /*si antes habia una barra*/
                        RegExItem ultimoEltodeArrayTipo = new RegExItem();
                        if (result.Componentes.Count != 0)
                        {
                            ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                        }
                        else
                        {
                            ultimoEltodeArrayTipo.Tipo = 0;
                        }
                        if (ultimoEltodeArrayTipo.Tipo == 3)
                        {
                            RegExItem cor = corchetes(CurrentMatch.Groups[1].Value.Substring(1, CurrentMatch.Groups[1].Value.Length - 2));
                            insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], cor, CurrentMatch.Groups[3].Value);
                        }
                        else
                        {
                            RegExItem cor = corchetes(CurrentMatch.Groups[1].Value.Substring(1, CurrentMatch.Groups[1].Value.Length - 2));
                            insertar(result, cor, CurrentMatch.Groups[3].Value);
                        }
                    }
                    if (CurrentMatch.Groups[1].Value[0] == '(')
                    {
                        /*si antes habia una barra*/
                        RegExItem ultimoEltodeArrayTipo = new RegExItem();
                        if (result.Componentes.Count != 0)
                        {
                            ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                        }
                        else
                        {
                            ultimoEltodeArrayTipo.Tipo = 0;
                        }
                        if (ultimoEltodeArrayTipo.Tipo == 3)
                        {
                            ArrayList pair = splitPar(input.Substring(CurrentMatch.Groups[1].Index, CurrentMatch.Groups[1].Length));
                            bool repeticiones = pair.Count == 1;
                            foreach (string variable in pair)
                            {
                                /*si antes habia una barra*/
                                if (result.Componentes.Count != 0)
                                {
                                    ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                                }
                                else
                                {
                                    ultimoEltodeArrayTipo.Tipo = 0;
                                }
                                if (variable[0] == '(' && variable[variable.Length - 1] == ')'  && splitPar(variable).Count == 1)
                                {
                                    //AKI
                                    //var eltosParentesisInter = analize(variable.Substring(1, (variable.Length - 2)), false);
                                    //eltosParentesisInter.Tipo = 2;
                                    RegExItem eltosParentesisInter = busqueda_negativa(variable);
                                    
                                    if (repeticiones)
                                    {
                                        eltosParentesisInter.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                    }
                                    //si antes habia una barra
                                    if (ultimoEltodeArrayTipo.Tipo == 3)
                                    {
                                        if (eltosParentesisInter.Tipo == 3)
                                        {
                                            foreach (RegExItem componets in ((RegExItem)eltosParentesisInter.Componentes[0]).Componentes)
                                            {
                                                //añadir el primer elto de eltosParentesisInter al componente de anterior 
                                                //añadir los otros eltos de eltosParentesisInter a anterior
                                                ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(componets);
                                            }
                                            ultimoEltodeArrayTipo.Componentes.Add(eltosParentesisInter.Componentes[eltosParentesisInter.Componentes.Count - 1]);
                                        }
                                        else
                                        {
                                            insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], eltosParentesisInter, eltosParentesisInter.Repeticiones);
                                        }
                                    }
                                    else
                                    {
                                        result.Componentes.Add(eltosParentesisInter);
                                    }
                                }
                                else
                                {
                                    var eltosParentesis = analize((string)variable + CurrentMatch.Groups[3].Value, false);
                                    if (repeticiones)
                                    {
                                        eltosParentesis.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                    }
                                    foreach (RegExItem Otroscomponentes in eltosParentesis.Componentes)
                                    {
                                        if (ultimoEltodeArrayTipo.Tipo == 3)
                                        {
                                            if (Otroscomponentes.Tipo == 3)
                                            {
                                                foreach (RegExItem componets in ((RegExItem)Otroscomponentes.Componentes[0]).Componentes)
                                                {
                                                    ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(componets);
                                                }
                                                ultimoEltodeArrayTipo.Componentes.Add(Otroscomponentes.Componentes[Otroscomponentes.Componentes.Count - 1]);
                                            }
                                            else
                                            {
                                                ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(Otroscomponentes);
                                            }
                                        }
                                        else 
                                        {
                                            result.Componentes.Add(Otroscomponentes);
                                        }
                                    }
                                }
                                repeticiones = true;
                            }                            
                        }
                        //si no hay barra
                        else
                        {
                            ArrayList pair = splitPar(input.Substring(CurrentMatch.Groups[1].Index, CurrentMatch.Groups[1].Length));
                            bool segundo = pair.Count == 1;
                            foreach (string variable in pair)
                            {
                                /*si antes habia una barra*/
                                if (result.Componentes.Count != 0)
                                {
                                    ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                                }
                                else
                                {
                                    ultimoEltodeArrayTipo.Tipo = 0;
                                }
                                if (variable[0] == '(' && variable[variable.Length - 1] == ')' && splitPar(variable).Count == 1)
                                {
                                    //AKI
                                    //var eltosParentesisInter = analize(variable.Substring(1, (variable.Length - 2)), false);
                                    //eltosParentesisInter.Tipo = 2;
                                    RegExItem eltosParentesisInter = busqueda_negativa(variable);
                                    
                                    if (segundo)
                                    {
                                        eltosParentesisInter.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                    }
                                    /*si antes habia una barra*/
                                    if (ultimoEltodeArrayTipo.Tipo == 3)
                                    {
                                        if (eltosParentesisInter.Tipo == 3)
                                        {
                                            foreach (RegExItem componets in ((RegExItem)eltosParentesisInter.Componentes[0]).Componentes)
                                            {
                      //añadir el primer elto de eltosParentesisInter al componente de anterior;añadir los otros eltos de eltosParentesisInter a anterior  
                                                ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(componets);
                                            }
                                            ultimoEltodeArrayTipo.Componentes.Add(eltosParentesisInter.Componentes[eltosParentesisInter.Componentes.Count - 1]);   
                                        }
                                        else 
                                        {
                                            insertar((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1], eltosParentesisInter, CurrentMatch.Groups[3].Value);
                                        }
                                    }
                                    else
                                    {
                                        insertar(result, eltosParentesisInter, eltosParentesisInter.Repeticiones == null ? "" : CurrentMatch.Groups[3].Value);
                                    }
                                }
                                else
                                {
                                    var eltosParentesis = analize((string)variable + (segundo ? CurrentMatch.Groups[3].Value : ""), false);
                                    if (segundo)
                                    {
                                        eltosParentesis.Repeticiones = Tratar_repeticiones(CurrentMatch.Groups[3].Value);
                                    }
                                    foreach (RegExItem Otroscomponentes in eltosParentesis.Componentes)
                                    {
                                        if (ultimoEltodeArrayTipo.Tipo == 3)
                                        {
                                            if (Otroscomponentes.Tipo != 3)
                                            {
                                                ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(Otroscomponentes);
                                            }
                                            else
                                            {
                                                foreach (RegExItem r in ((RegExItem)Otroscomponentes.Componentes[0]).Componentes)
                                                {
                                                    ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(r);
                                                }
                                                for (int i = 1; i < Otroscomponentes.Componentes.Count;i++ )
                                                {
                                                    ultimoEltodeArrayTipo.Componentes.Add(Otroscomponentes.Componentes[i]);
                                                }                                                
                                            }
                                        }
                                        else
                                        {
    //ver si Otroscomponentes es tipo 3 insertar lo ke esta en result a la primera posicion de Otroscomponentes borrar result y luego copiar Otroscomponentes a result
                                            if (Otroscomponentes.Tipo == 3)
                                            {
                                                if (result.Componentes.Count > 0)
                                                {
                         //bucle para copiar todos los elementos de result hacia otrosComponentes colocandolos primeros que los que ya tenia otrosComponentes
                                                    for(int i = result.Componentes.Count-1; i >= 0; i--)
                                                    {
                                                        ((RegExItem)Otroscomponentes.Componentes[0]).Componentes.Insert(0, result.Componentes[i]);
                                                    }
                                                    result.Componentes.RemoveRange(0, result.Componentes.Count);     
                                                }
                                            }
                                            result.Componentes.Add(Otroscomponentes); 
                                        }
                                    }
                                }
                                segundo = true;
                            }
                        }
                    }

                    if (CurrentMatch.Groups[1].Value[0] == '|')
                    {
                        //ya antes habia una barra
                        RegExItem ultimoEltodeArrayTipo = new RegExItem();
                        if (result.Componentes.Count != 0)
                        {
                            ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                        }
                        else
                        {
                            ultimoEltodeArrayTipo.Tipo = 0;
                        }
                        if (ultimoEltodeArrayTipo.Tipo == 3)
                        {
                            RegExItem nuevoVacio1 = new RegExItem();
                            nuevoVacio1.Tipo = 0;
                            ((RegExItem)ultimoEltodeArrayTipo).Componentes.Add(nuevoVacio1);
                        }
                        else 
                        {
                            //caso de primera barra |
                            RegExItem barraVertical = new RegExItem();
                            barraVertical.Tipo = 3;
                            barraVertical.Componentes.Add((RegExItem)result.Clone());
                            result.Componentes.RemoveRange(0, result.Componentes.Count);
                            RegExItem nuevoVacio = new RegExItem();
                            nuevoVacio.Tipo = 0;
                            barraVertical.Componentes.Add(nuevoVacio);
                            result.Componentes.Add(barraVertical);
                        }
                    }
                }      
            }
            //Error porque no termino de leer el input completo
            //Al agregar este error tengo que comprobrar si antes habia un tipo 3
            if (prev + prevLength != input.Length)
            {
                RegExItem ultimoEltodeArrayTipo = new RegExItem();
                if (result.Componentes.Count != 0)
                {
                    ultimoEltodeArrayTipo = (RegExItem)result.Componentes[result.Componentes.Count - 1];
                }
                else { ultimoEltodeArrayTipo.Tipo = 0; }
                if (ultimoEltodeArrayTipo.Tipo == 3)
                {
                    ((RegExItem)ultimoEltodeArrayTipo.Componentes[ultimoEltodeArrayTipo.Componentes.Count - 1]).Componentes.Add(GenerarError(input.Substring(prev + prevLength, input.Length - (prev + prevLength))));
                }
                else
                {
                    result.Componentes.Add(GenerarError(input.Substring(prev + prevLength, input.Length - (prev + prevLength))));
                }
            }

            return result;
        }

        private ArrayList splitPar(string token)
        {
            ArrayList result = new ArrayList();
            int contador = 1;
            int pos = 1;
            while (pos < token.Length && contador > 0)
            {
                if (token[pos] == '(') contador++;
                else if (token[pos] == ')') contador--;
                pos++;
            }
            while (pos < token.Length && token[pos] != '(') pos++;

            result.Add(token.Substring(0, pos));
            if (pos != token.Length)
            {
                result.Add(token.Substring(pos));
            }
            return result;
        }

        private List<Match> getTokens(string exp)
        {
            List<Match> result = new List<Match>();
            Regex regex = new Regex("(\\\\.|\\[([^\\]])+]|\\]|\\(.+\\)|\\||\\.|[^\\.\\|\\(\\)\\[\\]\\{\\}\\?\\*\\+])(\\?|\\*|\\+|\\{\\d+(,\\d*)?})?");
            Match match = regex.Match(exp);

            while (match.Success)
            {
                result.Add(match);
                match = match.NextMatch();
            }

            return result;
        }

        //metodo para mostrar los Match
        public ArrayList m_r(string regexp, string ma)
        {
            try
            {
                ArrayList total = new ArrayList();
                WebService1.clase.My_Match result;
                Regex regex = new Regex(regexp);
                Match match = regex.Match(ma);
                while (match.Success)
                {
                    result = new WebService1.clase.My_Match();
                    foreach (Group g in match.Groups)
                    {
                        result.Grupos.Add(g.Index);
                        result.Grupos.Add(g.Length);
                    }
                    result.Ind = match.Index;
                    result.Len = match.Length;
                    result.Valor = match.Value;
                    total.Add(result);
                    match = match.NextMatch();
                }
                return total;
            }
            catch { ArrayList myAL = new ArrayList(); return myAL; }
            
        }

        //Corchetes
        public RegExItem corchetes(String exp)
        {
            RegExItem result = new RegExItem();
            result.Tipo = 4; /*el tipo es 4 como indica la clase RegExItem*/
            int j = 0;
            /*si el primer elemento del string es ^ entonces el subitpo es 1 y todo lo demas es negado 
            * en caso contrario el subtipo es 2 y el analisis es normal*/
            if (exp.StartsWith("^"))
            {
                result.Subtipo = 1;
                j++;
            }
            else { result.Subtipo = 2; }

            for (; j < exp.Length; j++)
            {
                var mirar = exp[j];
                switch (exp[j])
                {
                    case '-':
                        //que sea el primero que lo pilla normal al igual que sea el ultimo de la lista
                        if (result.Componentes.Count == 0)
                        {
                            result.Componentes.Add(GenerarSimple(exp[j].ToString()));
                            break;
                        }
                        //que sea el ultimo lo añado normal
                        if (j == exp.Length - 1)
                        {
                            result.Componentes.Add(GenerarSimple(exp[j].ToString()));
                            break;
                        }
                        try
                        {
                            //guardo el ultimo elto del array
                            RegExItem ultimoEltodeArray = (RegExItem)result.Componentes[result.Componentes.Count - 1];

                            if (ultimoEltodeArray.Tipo == 9)
                            {
                                //result.Componentes.Add(ultimoEltodeArray);
                                result.Componentes.Add(GenerarSimple(exp[j].ToString()));
                            }
                            else 
                            {
                                //este es el elemento siguiente despues del guion en caso de que sea barra llamar al metodo caracter y pasarle exp[j+2]
                                if (exp[j + 1] == '\\')
                                {
                                    RegExItem next = caracter(exp[j + 2]);
                                    if (OrdenValido(ultimoEltodeArray, next))
                                    {
                                        //borro el ultimo elto del array
                                        result.Componentes.RemoveAt(result.Componentes.Count - 1);
                                        RegExItem rango = new RegExItem();
                                        string concatenar = string.Concat(ultimoEltodeArray.Valor, exp[j], next.Valor);
                                        if (exp[j + 1] == '\\')
                                        {
                                            j++;
                                        }
                                        j++;
                                        rango.Valor = concatenar;
                                        rango.Tipo = 9;
                                        result.Componentes.Add(rango);
                                    }
                                    //caso que no sea valido el orden de los regex
                                    else
                                    {
                                        result.Componentes.Add(GenerarError(next.Valor));
                                        if (exp[j + 1] == '\\')
                                        {
                                            j++;
                                        }
                                        j++;
                                    }
                                }
                                else
                                {
                                    RegExItem simple = GenerarSimple(exp[j+1].ToString());
                                    if (OrdenValido(ultimoEltodeArray, simple))
                                    {
                                        //borro el ultimo elto del array
                                        result.Componentes.RemoveAt(result.Componentes.Count - 1);
                                        RegExItem rango = new RegExItem();
                                        string concatenar = string.Concat(ultimoEltodeArray.Valor, exp[j], simple.Valor);
                                        j++;
                                        rango.Valor = concatenar;
                                        rango.Tipo = 9;
                                        result.Componentes.Add(rango); 
                                    }
                                    else
                                    {
                                        result.Componentes.Add(GenerarError(string.Concat(simple.Valor)));
                                        j++;
                                    }
                                }
                            }
                        }
                    //Capturar la exepcion para cuando despues de \ dentro de los corchetes no encuentre nada example [1-\]
                        catch (System.IndexOutOfRangeException e)
                        {
                            result.Componentes.Add(GenerarError(exp[j].ToString()));
                            j++;
                        }
                        break;
                    case '\\':
                        if (j < exp.Length - 1)
                        {
                            result.Componentes.Add(caracter(exp[j + 1]));
                            j++;
                        }                      
                        break;
                    default:
                        result.Componentes.Add(GenerarSimple(exp[j].ToString()));
                        break;
                }
            }
            return result;
        }

        public bool OrdenValido(RegExItem anterior, RegExItem siguiente)
        {
            if (anterior.Subtipo == 0 && siguiente.Subtipo == 0)
            {
                if (anterior.Valor[0] <= siguiente.Valor[0])
                {
                    return true;
                }
                else { return false; }
            }
            else
            {
                if (anterior.Subtipo == 0 && siguiente.Subtipo >= 200)
                {
                    return false;
                }
                if (anterior.Subtipo == 0 && siguiente.Subtipo <= 200)
                {
                    if (anterior.Valor[0] <= siguiente.Valor[0])
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    if (anterior.Subtipo <= 200 && siguiente.Subtipo == 0)
                    {
                        if (anterior.Valor[0] <= siguiente.Valor[0])
                        {
                            return true;
                        }
                        else 
                        { return false; }
                    }
                    else
                    {
                        if (anterior.Subtipo >=200)
                        {
                            return false;
                        }
                        else { return false; }
                    }
                }
            }
        }

        //metodo para buscar despues de \ tambien retorna un RegExItem (con subtipo) ESTOS CASOS-->;";';\\ 
        public RegExItem caracter(char c)
        {
            RegExItem letraBarra = new RegExItem();
            letraBarra.Tipo = 1;
            switch (c)
            {
                    //los que representen algo tendran entre 100-150
                case 'D':
                    letraBarra.Subtipo = 251;
                    letraBarra.Valor = "No digito";
                    break;
                case 'v':
                    letraBarra.Subtipo = 156;
                    letraBarra.Valor = "Tabulador Vertical";
                    break;
                case 'w':
                    letraBarra.Subtipo = 252;
                    letraBarra.Valor = "Caracter Alfanumerico";
                    break;                    
                case 'W':
                    letraBarra.Subtipo = 253;
                    letraBarra.Valor = "No Caracter Alfanumerico";
                    break;
                case 's':
                    letraBarra.Subtipo = 155;
                    letraBarra.Valor = "Espacio en Blanco";
                    break;
                case 'S':
                    letraBarra.Subtipo = 254;
                    letraBarra.Valor = "No Espacio en Blanco";
                    break;
                case 'd':
                    letraBarra.Subtipo = 255;
                    letraBarra.Valor = "Digito";
                    break;
                case 'B':
                    letraBarra.Subtipo = 256;
                    letraBarra.Valor = "No Limite de Palabra";
                    break;
                case 'b':
                    letraBarra.Subtipo = 257;
                    letraBarra.Valor = "Limite de Palabra";
                    break;
                case 't':
                    letraBarra.Subtipo = 154;
                    letraBarra.Valor = "tabulador";
                    break;
                case 'n':
                    letraBarra.Subtipo = 153;
                    letraBarra.Valor = "Nueva linea";
                    break;
                case 'f':
                    letraBarra.Subtipo = 152;
                    letraBarra.Valor = "Salto de página";
                    break;
                case 'r':
                    letraBarra.Subtipo = 151;
                    letraBarra.Valor = "Retorno de carro";
                    break;
                default:
                    letraBarra.Subtipo = 100;
                    letraBarra.Valor = c.ToString();
                    break;
            }
            return letraBarra;
        }
    }
}
