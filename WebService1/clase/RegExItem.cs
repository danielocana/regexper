﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace RegexAnalizer
{
    public class RegExItem :ICloneable
    {
        int tipo; // 0 -> secuencia; 1 -> elemento simple; 2 -> grupo (paréntesis)
        // 3 -> alternativa (|); 4 -> alternativa (corchetes) // -1 -> error
        public RegExItem()
        {
            componentes = new ArrayList();
        }
        public int Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        int subtipo; // (si tipo == 1)
        // 0 -> caracter; 1 -> \d; 2 -> \D; ...
        public int Subtipo
        {
            get { return subtipo; }
            set { subtipo = value; }
        }

        private string valor;
        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        ArrayList componentes;
        public ArrayList Componentes
        {
            get { return componentes; }
            set { componentes = value; }
        }

        string repeticiones;
        public string Repeticiones
        {
            get { return repeticiones; }
            set { repeticiones = value; }
        }
        public object Clone()
        {
            RegExItem copy = new RegExItem();
            copy.tipo = this.tipo;
            copy.subtipo = this.subtipo;
            copy.componentes = (ArrayList)this.componentes.Clone();
            copy.valor = valor;
            return copy;
        }
    }
}