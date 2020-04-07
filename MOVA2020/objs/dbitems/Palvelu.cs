﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOVA2020.objs.dbitems
{
    class Palvelu
    {
        private int palvelu_id, tyyppi;
        private Toimintaalue toimintaalue;
        private string nimi, kuvaus;
        private double hinta, alv;

        public Palvelu(int palvelu_id, int tyyppi, string nimi, string kuvaus, double hinta, double alv, Toimintaalue toimintaalue)
        {
            this.palvelu_id = palvelu_id;
            Tyyppi = tyyppi;
            Nimi = nimi;
            Kuvaus = kuvaus;
            Hinta = hinta;
            Alv = alv;
            Toimintaalue = toimintaalue;
        }

        public int Palvelu_id { get => palvelu_id;}
        public int Tyyppi { get => tyyppi; set => tyyppi = value; }
        public string Nimi { get => nimi; set => nimi = value; }
        public string Kuvaus { get => kuvaus; set => kuvaus = value; }
        public double Hinta { get => hinta; set => hinta = value; }
        public double Alv { get => alv; set => alv = value; }
        internal Toimintaalue Toimintaalue { get => toimintaalue; set => toimintaalue = value; }
    }
}
