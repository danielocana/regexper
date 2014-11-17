using System;
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
        public RegExItem(string Valor, int Tipo, int Subtipo, string Repeticiones)
        {
            this.valor = Valor;
            this.tipo = Tipo;
            this.subtipo = Subtipo;
            this.repeticiones = Repeticiones;
            componentes = new ArrayList();
        }
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

        public override int GetHashCode()
        {
            return this.Valor.GetHashCode();
        }

        public override bool Equals(Object e)
        {
            if (e == null)
            {
                return false;
            }
            else
            {
                if (((RegExItem)e).Tipo != this.Tipo)
                {
                    return false;
                }
                if (((RegExItem)e).Subtipo != this.Subtipo)
                {
                    return false;
                }
                if (((RegExItem)e).Valor != this.Valor)
                {
                    return false;
                }
                if (((RegExItem)e).Repeticiones != this.Repeticiones)
                {
                    return false;
                }
                if (((RegExItem)e).Componentes.Count != this.Componentes.Count)
                {
                    return false;
                }
                for (int i = 0; i < ((RegExItem)e).Componentes.Count; i++)
                {

                    if (!((RegExItem)((RegExItem)e).Componentes[i]).Equals((RegExItem)this.Componentes[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
